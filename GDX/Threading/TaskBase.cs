using System;
using System.Diagnostics;
using GDX.Collections;

namespace GDX.Threading
{
    /// <summary>
    ///     The base of a task used by <see cref="TaskDirector"/>.
    /// </summary>
    public abstract class TaskBase
    {
        /// <summary>
        ///     A set of flags indicating what type of blocking this task does when it comes to other tasks,
        ///     or even user experience.
        /// </summary>
        [Flags]
        public enum BlockingModeFlags : ushort
        {
            None = 0,
            All = 1,
            SameName = 2,
            Bits = 4,
            UserInteraction = 8
        }

        /// <summary>
        ///     The default user-friendly name given to <see cref="TaskBase"/>.
        /// </summary>
        const string k_DefaultName = "GDX Task";

        /// <summary>
        ///     A timing mechanism to measure the duration of execution of the <see cref="TaskBase"/>.
        /// </summary>
        readonly Stopwatch m_Stopwatch = new Stopwatch();

        /// <summary>
        ///     An event that is triggered when the <see cref="TaskBase"/> has finished its execution.
        /// </summary>
        /// <remarks>
        ///     It is <b>super important</b> to remember that subscribed actions will invoke off of the main thread.
        ///     Any logic that requires the main thread will not work. Many of Unity's APIs are not safe for this.
        /// </remarks>
        public Action<TaskBase> completed;

        /// <summary>
        ///     An event that is triggered once the <see cref="TaskBase"/> has finished, during the next tick of the
        ///     <see cref="TaskDirector"/>.
        /// </summary>
        /// <remarks>
        ///     This is a safe way to do work which requires being executed on the main thread.
        /// </remarks>
        public Action<TaskBase> completedMainThread;

        /// <summary>
        ///     The <see cref="TaskBase"/>'s descriptive bits.
        /// </summary>
        protected BitArray16 m_Bits;

        /// <summary>
        ///     A quick set of bits used to describe tasks that should be blocked when
        ///     <see cref="m_BlockingModes"/> contains <see cref="BlockingModeFlags.Bits"/>. The opposing
        ///     <see cref="TaskBase"/> will have its <see cref="m_Bits"/> checked for positives that match.
        /// </summary>
        /// <remarks>If any of them are the sam</remarks>
        protected BitArray16 m_BlockingBits;

        /// <summary>
        ///     The <see cref="TaskBase"/>'s blocking modes for other tasks.
        /// </summary>
        protected BlockingModeFlags m_BlockingModes = BlockingModeFlags.None;

        /// <summary>
        ///     The exception that occured.
        /// </summary>
        /// <remarks>See <see cref="IsFaulted"/>.</remarks>
        protected Exception m_Exception;

        /// <summary>
        ///     Should the task report information to the <see cref="TaskDirector"/> log.
        /// </summary>
        protected bool m_IsLogging = false;

        /// <summary>
        ///     The user-friendly name of the task, used by different feedback systems.
        /// </summary>
        /// <remarks>It's important to set this in inherited constructors.</remarks>
        protected string m_Name = k_DefaultName;

        /// <summary>
        ///     Did an exception occur while executing this <see cref="TaskBase"/>?
        /// </summary>
        bool m_IsFaulted;

        /// <summary>
        ///     Has the task finished its execution/work.
        /// </summary>
        bool m_IsDone;

        /// <summary>
        ///     A flag indicating if the task is executing, or if it is still waiting.
        /// </summary>
        bool m_IsExecuting;

        /// <summary>
        ///     The core logic to be defined for a task.
        /// </summary>
        public abstract void DoWork();

        /// <summary>
        ///     Enqueue the current <see cref="TaskBase"/> with the <see cref="TaskDirector"/> for execution.
        /// </summary>
        public void Enqueue()
        {
            TaskDirector.QueueTask(this);
        }

        /// <summary>
        ///     Gets the associated <see cref="BitArray16"/> with this task.
        /// </summary>
        /// <returns>The defined flags.</returns>
        public BitArray16 GetBits()
        {
            return m_Bits;
        }

        /// <summary>
        ///     Gets the <see cref="BitArray16"/> to evaluate other tasks against.
        /// </summary>
        /// <returns>The defined bits.</returns>
        public BitArray16 GetBlockedBits()
        {
            return m_BlockingBits;
        }

        /// <summary>
        ///     Returns the <see cref="BlockingModeFlags"/> used to determine other task execution.
        /// </summary>
        /// <returns>A set of flags indicating if other tasks should be able to start execution.</returns>
        public BlockingModeFlags GetBlockingModes()
        {
            return m_BlockingModes;
        }

        /// <summary>
        ///     Returns the <see cref="Exception"/> that was created when the task faulted.
        /// </summary>
        /// <remarks>See <see cref="IsFaulted"/>.</remarks>
        /// <returns>An <see cref="Exception"/> object.</returns>
        public Exception GetException()
        {
            return m_Exception;
        }

        /// <summary>
        ///     Gets the user-friendly name of the task.
        /// </summary>
        /// <returns>The defined <see cref="string"/> name of the task.</returns>
        public string GetName()
        {
            return m_Name;
        }

        /// <summary>
        ///     Does this <see cref="TaskBase"/> block all other tasks after it from starting execution?
        /// </summary>
        /// <remarks>
        ///     This will keep all tasks after it sitting waiting for this task to complete.
        /// </remarks>
        /// <returns>true if this task blocks all after it, otherwise false.</returns>
        public bool IsBlockingAllTasks()
        {
            return m_BlockingModes.HasFlags(BlockingModeFlags.All);
        }

        /// <summary>
        ///     Does this <see cref="TaskBase"/> block other tasks from executing based on its
        ///     <see cref="m_BlockingBits"/>?
        /// </summary>
        /// <returns>true if this tasks blocks based on bits, otherwise false.</returns>
        public bool IsBlockingBits()
        {
            return m_BlockingModes.HasFlags(BlockingModeFlags.Bits);
        }

        /// <summary>
        ///     Does this <see cref="TaskBase"/> block all other tasks of the same name from starting during
        ///     its execution?
        /// </summary>
        /// <returns>true if this tasks blocks same named tasks, otherwise false.</returns>
        public bool IsBlockingSameName()
        {
            return m_BlockingModes.HasFlags(BlockingModeFlags.SameName);
        }

        /// <summary>
        ///     Should the execution of this <see cref="TaskBase"/> prevent the user from providing input to the
        ///     user interface?
        /// </summary>
        /// <remarks>
        ///     This directly relates to the <see cref="TaskDirector.inputBlocked"/>, altering the count used
        ///     to trigger that particular event.
        /// </remarks>
        /// <returns>true if this task should prevent user input, otherwise false.</returns>
        public bool IsBlockingUserInterface()
        {
            return m_BlockingModes.HasFlags(BlockingModeFlags.UserInteraction);
        }

        /// <summary>
        ///     Is the <see cref="TaskBase"/> finished executing?
        /// </summary>
        /// <returns>
        ///     Returns true if the execution phase of the task has been completed. This will be
        ///     true if an exception has occured.
        /// </returns>
        public bool IsDone()
        {
            return m_IsDone;
        }

        /// <summary>
        ///     Is the <see cref="TaskBase"/> currently executing on the thread pool?
        /// </summary>
        /// <returns>Returns true if the task is executing, otherwise false.</returns>
        public bool IsExecuting()
        {
            return m_IsExecuting;
        }

        /// <summary>
        ///     Did an exception occur while executing off thread?
        /// </summary>
        /// <remarks>
        ///     <see cref="GetStatusMessage"/> for more details.
        /// </remarks>
        /// <returns>Returns true if an exception occured.</returns>
        public bool IsFaulted()
        {
            return m_IsFaulted;
        }

        /// <summary>
        ///     Execute task logic.
        /// </summary>
        public void Run()
        {
            // Set flag at the start of the execution
            m_IsExecuting = true;
            m_IsDone = false;

            // Start timing
            m_Stopwatch.Start();

            // Update task
            TaskDirector.UpdateTask(this);

            try
            {
                if (m_IsLogging)
                {
                    TaskDirector.Log($"Starting {m_Name}");
                }

                DoWork();
            }
            catch (Exception e)
            {
                m_IsFaulted = true;
                m_Exception = e;
                if (m_IsLogging)
                {
                    TaskDirector.Log(e.Message);
                }
            }
            finally
            {
                m_IsDone = true;
                m_IsExecuting = false;

                // Invoke off thread (before main thread)
                completed?.Invoke(this);

                TaskDirector.UpdateTask(this);

                m_Stopwatch.Stop();

                if (m_IsLogging)
                {
                    TaskDirector.Log($"{m_Name} finished in {m_Stopwatch.ElapsedMilliseconds}ms.");
                }
            }
        }
    }
}