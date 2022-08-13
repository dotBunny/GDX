// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GDX.Threading
{
    /// <summary>
    ///     A simple unit test to validate the behaviour of the <see cref="TaskDirector"/>.
    /// </summary>
    public class TaskDirectorTests
    {
        List<string> m_Log = new List<string>();
        bool m_BlockInput = false;
        bool m_OnCompleteCalled = false;
        bool m_OnCompleteMainThreadCalled = false;

        void LogAdded(string[] values)
        {
            m_Log.AddRange(values);
        }

        void BlockInput(bool inputBlocked)
        {
            m_BlockInput = inputBlocked;
        }

        [SetUp]
        public void Setup()
        {
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
            new TaskBaseTests.TestTask(
                TestLiterals.HelloWorld, 5, null, null).Enqueue();

            TaskDirector.Wait();

            Assert.IsTrue(m_Log.Count == 5, $"{m_Log.Count.ToString()} != 5");
        }

       

        // OnBlockUserInput


        // GetBusyCount
        // GetQueueCount
        // GetStatus
        // HasTasks
        // QueueTask
        // Tick
        // UpdateTask
        // Wait
        // WaitFor/
    }
}