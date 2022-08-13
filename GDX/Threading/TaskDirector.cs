// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using GDX.Collections;

namespace GDX.Threading
{
    public static class TaskDirector
    {
        static readonly object k_LogLock = new object();
        static readonly object k_StatusChangeLock = new object();
        static readonly List<TaskBase> k_TasksBusy = new List<TaskBase>();
        static readonly List<TaskBase> k_TasksFinished = new List<TaskBase>();
        static readonly List<TaskBase> k_TasksProcessed = new List<TaskBase>();
        static readonly List<TaskBase> k_TasksWaiting = new List<TaskBase>();

        static readonly List<string> k_BlockedNames = new List<string>();
        static readonly int[] k_BlockedBits = new int[16];

        static readonly Queue<string> k_TaskLog = new Queue<string>(10);

        public static Action<bool> OnBlockUserInput;
        public static Action<string[]> OnLogAdded;

        static bool s_BlockInput;

        static int s_TasksBusyCount;
        static int s_TasksWaitingCount;
        static int s_BlockInputCount;
        static int s_BlockAllTasksCount;

        public static void AddLog(string message)
        {
            lock (k_LogLock)
            {
                k_TaskLog.Enqueue(message);
            }
        }

        public static bool HasTasks()
        {
            return s_TasksBusyCount > 0 || s_TasksWaitingCount > 0;
        }


        public static void QueueTask(TaskBase task)
        {
            if (task.IsExecuting())
            {
                AddBusyTask(task);
                return;
            }

            lock (k_StatusChangeLock)
            {
                if (k_TasksWaiting.Contains(task))
                {
                    return;
                }

                k_TasksWaiting.Add(task);
                s_TasksWaitingCount++;
            }
        }

        public static void UpdateTask(TaskBase task)
        {
            if (task.IsDone())
            {
                RemoveBusyTask(task);
            }
            else if (task.IsExecuting())
            {
                AddBusyTask(task);
            }
        }

        public static void Complete()
        {
            while (HasTasks())
            {
                Tick();
            }
            Tick();
        }


        // tick on main thread????
        public static void Tick()
        {
            // We are blocked by a running task from adding anything else.
            lock (k_StatusChangeLock)
            {
                int finishedWorkersCount = k_TasksFinished.Count;
                if (finishedWorkersCount > 0)
                {
                    for (int i = 0; i < finishedWorkersCount; i++)
                    {
                        TaskBase taskBase = k_TasksFinished[i];
                        taskBase.CompletedMainThread?.Invoke(taskBase);
                    }

                    k_TasksFinished.Clear();
                }

                if (s_BlockAllTasksCount == 0)
                {
                    // Spin up workers needed to process
                    int count = k_TasksWaiting.Count;

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            TaskBase task = k_TasksWaiting[i];

                            // Check if task has a blocked name
                            if (k_BlockedNames.Contains(task.GetName()))
                            {
                                continue;
                            }

                            BitArray16 bits = task.GetBits();
                            if (IsBlockedByBits(ref bits))
                            {
                                continue;
                            }

                            AddBusyTask(task);
                            ThreadPool.QueueUserWorkItem(delegate { task.Run(); });
                            k_TasksProcessed.Add(task);
                        }

                        int processedCount = k_TasksProcessed.Count;
                        for (int i = 0; i < processedCount; i++)
                        {
                            k_TasksWaiting.Remove(k_TasksProcessed[i]);
                        }

                        s_TasksWaitingCount = k_TasksWaiting.Count;
                        k_TasksProcessed.Clear();
                    }
                }
            }

            // Dispatch logging
            lock (k_LogLock)
            {
                if (k_TaskLog.Count > 0)
                {
                    OnLogAdded?.Invoke(k_TaskLog.ToArray());
                    k_TaskLog.Clear();
                }
            }

            // Invoke notification to anything subscribed to block input
            if (s_BlockInputCount > 0 && !s_BlockInput)
            {
                OnBlockUserInput?.Invoke(true);
                s_BlockInput = true;
            }
            else if (s_BlockInputCount <= 0 && s_BlockInput)
            {
                OnBlockUserInput?.Invoke(false);
                s_BlockInput = false;
            }
        }

        public static string GetTaskStatus()
        {
            return s_TasksBusyCount > 0 ? $"{s_TasksBusyCount} Running / {s_TasksWaitingCount} Waiting" : null;
        }

        public static int GetBusyCount()
        {
            return s_TasksBusyCount;
        }

        public static int GetWaitingCount()
        {
            return s_TasksWaitingCount;
        }

        static void AddBusyTask(TaskBase task)
        {
            lock (k_StatusChangeLock)
            {
                if (!k_TasksBusy.Contains(task))
                {
                    if (task.IsBlockingAllTasks())
                    {
                        s_BlockAllTasksCount++;
                    }

                    // Add to the count of tasks that block input so we can update based off it
                    if (task.IsBlockingUserInterface())
                    {
                        s_BlockInputCount++;
                    }

                    if (task.IsBlockingSameName())
                    {
                        k_BlockedNames.Add(task.GetName());
                    }

                    if (task.IsBlockingBits())
                    {
                        BitArray16 blockedBits = task.GetBlockedBits();
                        for (int i = 0; i < 16; i++)
                        {
                            if (blockedBits[(byte)i])
                            {
                                k_BlockedBits[i]++;
                            }
                        }
                    }

                    k_TasksBusy.Add(task);
                    s_TasksBusyCount++;
                }
            }
        }

        static bool IsBlockedByBits(ref BitArray16 task)
        {
            for (int i = 0; i < 16; i++)
            {
                if (task[(byte)i] && k_BlockedBits[i] > 0)
                {
                    return true;
                }
            }
            return false;
        }

        static void RemoveBusyTask(TaskBase task)
        {
            lock (k_StatusChangeLock)
            {
                if (k_TasksBusy.Contains(task))
                {
                    k_TasksBusy.Remove(task);
                    s_TasksBusyCount--;

                    // Add to list of tasks so that the next tick the main thread will call their completion callbacks.
                    k_TasksFinished.Add(task);

                    if (task.IsBlockingAllTasks())
                    {
                        s_BlockAllTasksCount--;
                    }

                    if (task.IsBlockingUserInterface())
                    {
                        s_BlockInputCount--;
                    }

                    if (task.IsBlockingSameName())
                    {
                        k_BlockedNames.Remove(task.GetName());
                    }

                    if (task.IsBlockingBits())
                    {
                        BitArray16 blockedBits = task.GetBlockedBits();
                        for (int i = 0; i < 16; i++)
                        {
                            if (blockedBits[(byte)i])
                            {
                                k_BlockedBits[i]--;
                            }
                        }
                    }
                }
            }
        }
    }
}