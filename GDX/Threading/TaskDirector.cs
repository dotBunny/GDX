// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GDX.Collections;

namespace GDX.Threading
{
    /// <summary>
    ///     A simple control mechanism for distributed <see cref="TaskBase"/> work across the
    ///     thread pool. Tasks should be short-lived and can queue up additional work.
    /// </summary>
    public static class TaskDirector
    {
        /// <summary>
        ///     An event invoked when a <see cref="TaskBase"/> throws an exception.
        /// </summary>
        public static Action<Exception> exceptionOccured;

        /// <summary>
        ///     An event invoked during <see cref="Tick"/> when user input should be blocked.
        /// </summary>
        public static Action<bool> inputBlocked;

        /// <summary>
        ///     An event invoked during <see cref="Tick"/> with new log content.
        /// </summary>
        public static Action<string[]> logAdded;

        /// <summary>
        ///     A running tally of bits that are blocked by the currently executing tasks.
        /// </summary>
        static readonly int[] k_BlockedBits = new int[16];

        /// <summary>
        ///     A collection of task names which are currently blocked from beginning to executed based
        ///     on the currently executing tasks.
        /// </summary>
        static readonly List<string> k_BlockedNames = new List<string>();

        /// <summary>
        ///     An accumulating collection of log content which will be passed to <see cref="logAdded"/>
        ///     subscribed methods during <see cref="Tick"/>.
        /// </summary>
        static readonly Queue<string> k_Log = new Queue<string>(10);

        /// <summary>
        ///     A locking mechanism used for log entries ensuring thread safety.
        /// </summary>
        static readonly object k_LogLock = new object();

        /// <summary>
        ///     A locking mechanism used for changes to task lists ensuring thread safety.
        /// </summary>
        static readonly object k_StatusChangeLock = new object();

        /// <summary>
        ///     A list of tasks currently being executed by the thread pool.
        /// </summary>
        static readonly List<TaskBase> k_TasksBusy = new List<TaskBase>();

        /// <summary>
        ///     A working list of tasks that recently finished, used in <see cref="Tick"/> to ensure
        ///     callbacks occur on the main thread.
        /// </summary>
        static readonly List<TaskBase> k_TasksFinished = new List<TaskBase>();

        /// <summary>
        ///     A list of tasks that were moved from waiting state to a working/busy state during
        ///     <see cref="Tick"/>.
        /// </summary>
        static readonly List<TaskBase> k_TasksProcessed = new List<TaskBase>();

        /// <summary>
        ///     A list of tasks currently waiting to start work.
        /// </summary>
        static readonly List<TaskBase> k_TasksQueue = new List<TaskBase>();

        /// <summary>
        ///     The number of tasks that are busy executing which block all other tasks from executing.
        /// </summary>
        /// <remarks>
        ///     This number can be higher then one, when tasks are forcibly started and then added to the
        ///     <see cref="TaskDirector"/>.
        /// </remarks>
        static int s_BlockAllTasksCount;

        /// <summary>
        ///     Is user input blocked?
        /// </summary>
        static bool s_BlockInput;

        /// <summary>
        ///     The number of tasks that are busy executing which block user input.
        /// </summary>
        static int s_BlockInputCount;

        /// <summary>
        ///     A cached count of <see cref="k_TasksBusy"/>.
        /// </summary>
        static int s_TasksBusyCount;

        /// <summary>
        ///     A cached count of <see cref="k_TasksQueue"/>.
        /// </summary>
        static int s_TasksQueueCount;

        /// <summary>
        ///     The number of tasks currently in process or awaiting execution by the thread pool.
        /// </summary>
        /// <returns>The number of tasks sitting in <see cref="k_TasksBusy"/>.</returns>
        public static int GetBusyCount()
        {
            return s_TasksBusyCount;
        }

        /// <summary>
        ///     The number of tasks waiting in the queue.
        /// </summary>
        /// <returns>The number of tasks sitting in <see cref="k_TasksQueue"/>.</returns>
        public static int GetQueueCount()
        {
            return s_TasksQueueCount;
        }

        /// <summary>
        ///     Get the status message for the <see cref="TaskDirector"/>.
        /// </summary>
        /// <returns>A pre-formatted status message.</returns>
        public static string GetStatus()
        {
            if (s_TasksBusyCount > 0)
            {
                return $"{s_TasksBusyCount.ToString()} Busy / {s_TasksQueueCount.ToString()} Queued";
            }
            return s_TasksQueueCount > 0 ? $"{s_TasksQueueCount.ToString()} Queued" : null;
        }

        /// <summary>
        ///     Does the <see cref="TaskDirector"/> have any known busy or queued tasks?
        /// </summary>
        /// <remarks>
        ///     It's not performant to poll this.
        /// </remarks>
        /// <returns>A true/false value indicating tasks.</returns>
        public static bool HasTasks()
        {
            return s_TasksBusyCount > 0 || s_TasksQueueCount > 0;
        }

        /// <summary>
        ///     Is the <see cref="TaskDirector"/> blocking tasks with a specific bit?
        /// </summary>
        /// <remarks>
        ///     It isn't ideal to constantly poll this method, ideally this could be used to block things outside of
        ///     the <see cref="TaskDirector"/>'s control based on tasks running.
        /// </remarks>
        /// <returns>A true/false value indicating if a <see cref="BitArray16"/> index is being blocked.</returns>
        public static bool IsBlockingBit(int index)
        {
            return k_BlockedBits[index] > 0;
        }

        /// <summary>
        ///     Adds a thread-safe log entry to a queue which will be dispatched to <see cref="logAdded"/> on
        ///     the <see cref="Tick"/> invoking thread.
        /// </summary>
        /// <param name="message">The log content.</param>
        public static void Log(string message)
        {
            lock (k_LogLock)
            {
                k_Log.Enqueue(message);
            }
        }

        /// <summary>
        ///     Add a task to the queue, to be later started when possible.
        /// </summary>
        /// <remarks>
        ///     If the <paramref name="task"/> is already executing it will be added to the known busy list.
        /// </remarks>
        /// <param name="task">An established task.</param>
        public static void QueueTask(TaskBase task)
        {
            if (task.IsExecuting())
            {
                // Already running tasks self subscribe
                return;
            }

            lock (k_StatusChangeLock)
            {
                if (k_TasksQueue.Contains(task))
                {
                    return;
                }

                k_TasksQueue.Add(task);
                s_TasksQueueCount++;
            }
        }

        /// <summary>
        ///     Update the <see cref="TaskDirector"/>, evaluating known tasks for work eligibility and execution.
        /// </summary>
        /// <remarks>
        ///     This should occur on the main thread. If the <see cref="TaskDirector"/> is used during play mode,
        ///     something needs to call this every global tick. While in edit mode the EditorTaskDirector triggers this
        ///     method.
        /// </remarks>
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
                        taskBase.completedMainThread?.Invoke(taskBase);
                    }

                    k_TasksFinished.Clear();
                }

                if (s_BlockAllTasksCount == 0)
                {
                    // Spin up workers needed to process
                    int count = k_TasksQueue.Count;

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            TaskBase task = k_TasksQueue[i];

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
                            k_TasksQueue.Remove(k_TasksProcessed[i]);
                        }

                        s_TasksQueueCount = k_TasksQueue.Count;
                        k_TasksProcessed.Clear();
                    }
                }
            }

            // Dispatch logging
            lock (k_LogLock)
            {
                if (k_Log.Count > 0)
                {
                    logAdded?.Invoke(k_Log.ToArray());
                    k_Log.Clear();
                }
            }

            // Invoke notification to anything subscribed to block input
            if (s_BlockInputCount > 0 && !s_BlockInput)
            {
                inputBlocked?.Invoke(true);
                s_BlockInput = true;
            }
            else if (s_BlockInputCount <= 0 && s_BlockInput)
            {
                inputBlocked?.Invoke(false);
                s_BlockInput = false;
            }
        }

        /// <summary>
        ///     Evaluate the provided task and update its state inside of the <see cref="TaskDirector"/>.
        /// </summary>
        /// <remarks>
        ///     This will add a task to the <see cref="TaskDirector"/> if it does not already know about it, regardless
        ///     of the current blocking mode status. Do not use this method to add non executing tasks, they will not
        ///     be added to the <see cref="TaskDirector"/> in this way.
        /// </remarks>
        /// <param name="task">An established task.</param>
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

        /// <summary>
        ///     Wait on the completion of all known tasks, blocking the invoking thread.
        /// </summary>
        /// <remarks>
        ///     Useful to force the main thread to wait for completion of tasks.
        /// </remarks>
        public static void Wait()
        {
            while (HasTasks())
            {
                Thread.Sleep(1);
                Tick();
            }
            Tick();
        }

        /// <summary>
        ///     Asynchronously wait on the completion of all known tasks.
        /// </summary>
        public static async Task WaitAsync()
        {
            while (HasTasks())
            {
                await Task.Delay(1);
                Tick();
            }
            Tick();
        }


        /// <summary>
        ///     Add a <see cref="TaskBase"/> to the known list of working tasks.
        /// </summary>
        /// <remarks>
        ///     This will add the blocking mode settings to the current settings.
        /// </remarks>
        /// <param name="task">An established task.</param>
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

        /// <summary>
        ///     Is the provided bit array blocked by the current blocking settings.
        /// </summary>
        /// <param name="bits">A <see cref="TaskBase"/>'s bits.</param>
        /// <returns>true/false if the task should be blocked from executing.</returns>
        static bool IsBlockedByBits(ref BitArray16 bits)
        {
            for (int i = 0; i < 16; i++)
            {
                if (bits[(byte)i] && k_BlockedBits[i] > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Remove a <see cref="TaskBase"/> from the known list of working tasks.
        /// </summary>
        /// <remarks>
        ///     This will remove the blocking mode settings to the current settings.
        /// </remarks>
        /// <param name="task">An established task.</param>
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

                if (task.IsFaulted())
                {
                    exceptionOccured?.Invoke(task.GetException());
                }
            }
        }
    }
}