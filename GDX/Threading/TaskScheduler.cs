﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using GDX.Collections.Generic;

namespace GDX.Threading
{
    public static class TaskScheduler
    {
        // TODO: Log to progress worker?
        // TODO: build better messaging to worker?
        static readonly object k_LogLock = new();
        static readonly object k_StatusChangeLock = new();
        static readonly List<TaskBase> k_TasksBusy = new();
        static readonly List<TaskBase> k_TasksFinished = new();
        static readonly List<TaskBase> k_TasksProcessed = new();
        static readonly List<TaskBase> k_TasksWaiting = new();

        static readonly List<string> k_BlockedNames = new();

        static SimpleList<string> s_Log;

        public static Action<bool> OnBlockUserInput;
        static bool s_BlockInput;

        static int s_TasksBusyCount;
        static int s_TasksWaitingCount;
        static int s_BlockInputCount;
        static int s_BlockAllTasksCount;

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
                AddBusyTask(task);
            }
            else if (task.IsExecuting())
            {
                RemoveBusyTask(task);
            }
        }

        public static void AddLog(string message)
        {
            lock (k_LogLock)
            {
                s_Log.AddWithExpandCheck(message);
            }
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

                    k_TasksBusy.Add(task);
                    s_TasksBusyCount++;
                }
            }
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
                }
            }
        }

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
                //
                // // Update background worker status?
                // s_TasksBusyCount = k_TasksBusy.Count;
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
    }
}