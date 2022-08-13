// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GDX.Threading
{
    /// <summary>
    ///     A simple unit test to validate the behaviour of the <see cref="TaskDirector"/>.
    /// </summary>
    public class TaskDirectorTests
    {
        readonly List<string> m_Log = new List<string>();
        readonly object m_OnCompleteCalledLock = new object();
        bool m_BlockInput;
        bool m_OnCompleteCalled ;
        bool m_OnCompleteMainThreadCalled;

        [SetUp]
        public void Setup()
        {
            // There should be nothing going on when we go to do a test
            TaskDirector.Wait();

            TaskDirector.OnLogAdded += LogAdded;
            TaskDirector.OnBlockUserInput += BlockInput;
        }

        [TearDown]
        public void Teardown()
        {
            TaskDirector.OnLogAdded -= LogAdded;
            TaskDirector.OnBlockUserInput -= BlockInput;

            m_BlockInput = false;
            m_OnCompleteCalled = false;
            m_OnCompleteMainThreadCalled = false;
            m_Log.Clear();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddLog_OnLogAdded_ExpectedCount()
        {
            new AddLogTestTask().Enqueue();

            TaskDirector.Wait();

            Assert.IsTrue(m_Log.Count == AddLogTestTask.LogCount,
                $"{m_Log.Count.ToString()} != {AddLogTestTask.LogCount.ToString()}");
        }

        // [Test]
        // [Category(Core.TestCategory)]
        // public async Task WaitAsync_OnBlockedUserInput_Blocked()
        // {
        //     new OneSecondTestTask().Enqueue();
        //     TaskDirector.Tick();
        //     Assert.IsTrue(m_BlockInput);
        //     await TaskDirector.WaitAsync();
        //     Assert.IsTrue(!m_BlockInput);
        // }

        [Test]
        [Category(Core.TestCategory)]
        public void GetCounts_BlockingName_CorrectCounts()
        {
            new NameBlockingTestTask().Enqueue();
            new NameBlockingTestTask().Enqueue();
            new NameBlockingTestTask().Enqueue();
            TaskDirector.Tick();

            Assert.IsTrue(TaskDirector.HasTasks());
            Assert.IsTrue(TaskDirector.GetBusyCount() == 1);
            Assert.IsTrue(TaskDirector.GetQueueCount() == 2);

            TaskDirector.Wait();
            Assert.IsTrue(TaskDirector.GetBusyCount() == 0);
            Assert.IsTrue(TaskDirector.GetQueueCount() == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasTasks_BlockingName_Valid()
        {
            Assert.IsFalse(TaskDirector.HasTasks());

            new NameBlockingTestTask().Enqueue();
            new NameBlockingTestTask().Enqueue();
            Assert.IsTrue(TaskDirector.HasTasks());

            TaskDirector.Tick();
            Assert.IsTrue(TaskDirector.HasTasks());

            TaskDirector.Wait();
            Assert.IsFalse(TaskDirector.HasTasks());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void BlockingBits_BlocksBits_CompletesCalled()
        {
            CallbackBitTestTask a = new CallbackBitTestTask(null, OnCallbackMainThread);
            CallbackBitTestTask b = new CallbackBitTestTask(OnCallback, null);

            a.Enqueue();
            TaskDirector.Tick();
            Assert.IsTrue(TaskDirector.IsBlockingBit(1));
            b.Enqueue();
            TaskDirector.Tick();

            // Make sure that the second task was actually blocked
            Assert.IsTrue(TaskDirector.GetBusyCount() == 1);

            // Finish
            TaskDirector.Wait();

            Assert.IsTrue(m_OnCompleteCalled);
            Assert.IsTrue(m_OnCompleteMainThreadCalled);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetStatus_Busy_Content()
        {
            new NameBlockingTestTask().Enqueue();
            TaskDirector.Tick();
            string message = TaskDirector.GetStatus();
            Assert.IsNotNull(message);
            Assert.IsTrue(message.Contains("Busy"));

            TaskDirector.Wait();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetStatus_Queued_ReturnsContent()
        {
            new NameBlockingTestTask().Enqueue();

            string message = TaskDirector.GetStatus();
            Assert.IsNotNull(message);
            Assert.IsFalse(message.Contains("Busy"));
            Assert.IsTrue(message.Contains("Queued"));

            TaskDirector.Wait();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetStatus_Nothing_ReturnsNull()
        {
            Assert.IsNull(TaskDirector.GetStatus());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void QueueTask_AddAlreadyExecuting_TaskAdded()
        {
            OneSecondTestTask task = new OneSecondTestTask();
            ThreadPool.QueueUserWorkItem(delegate { task.Run(); });
            Thread.Sleep(100);
            TaskDirector.QueueTask(task);
            Assert.IsTrue(TaskDirector.GetBusyCount() == 1);
            Assert.IsTrue(TaskDirector.GetQueueCount() == 0);
            TaskDirector.Wait();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void QueueTask_AlreadyQueued_NotQueued()
        {
            OneSecondTestTask task = new OneSecondTestTask();
            TaskDirector.QueueTask(task);
            TaskDirector.QueueTask(task);
            Assert.IsTrue(TaskDirector.GetQueueCount() == 1);
            TaskDirector.Wait();
        }
        void LogAdded(string[] values)
        {
            m_Log.AddRange(values);
        }

        void BlockInput(bool inputBlocked)
        {
            m_BlockInput = inputBlocked;
        }

        void OnCallback(TaskBase task)
        {
            lock (m_OnCompleteCalledLock)
            {
                m_OnCompleteCalled = true;
            }
        }

        void OnCallbackMainThread(TaskBase task)
        {
            m_OnCompleteMainThreadCalled = true;
        }

        class AddLogTestTask : TaskBase
        {
            public const int LogCount = 5;

            public AddLogTestTask()
            {
                m_Name = "AddLogTestTask";
                m_IsLogging = false;
                m_BlockingModes = BlockingModeFlags.All;
            }

            public override void DoWork()
            {
                for (int i = 0; i < LogCount; i++)
                {
                    TaskDirector.AddLog(m_Name);
                }
            }
        }
        class OneSecondTestTask : TaskBase
        {
            public OneSecondTestTask()
            {
                m_Name = "OneSecondTestTask";
                m_BlockingModes = BlockingModeFlags.UserInteraction;
            }

            public override void DoWork()
            {
                Thread.Sleep(1000);
            }
        }

        class NameBlockingTestTask : TaskBase
        {
            public NameBlockingTestTask()
            {
                m_Name = "NameBlockingTestTask";
                m_BlockingModes = BlockingModeFlags.SameName;
            }

            public override void DoWork()
            {
                Thread.Sleep(100);
            }
        }

        class CallbackBitTestTask : TaskBase
        {
            public CallbackBitTestTask(Action<TaskBase> onComplete, Action<TaskBase> onCompleteMainThread)
            {
                m_Name = "CallbackBitTestTask";
                m_BlockingBits[1] = true;
                m_Bits[1] = true;
                m_BlockingModes = BlockingModeFlags.Bits;

                Completed += onComplete;
                CompletedMainThread += onCompleteMainThread;
            }

            public override void DoWork()
            {
                Thread.Sleep(100);
            }
        }
    }
}