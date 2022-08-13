// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GDX.Threading
{
    /// <summary>
    ///     A simple unit test to validate the behaviour of the <see cref="TaskBase"/>.
    /// </summary>
    public class TaskBaseTests
    {
        List<string> m_TestLog = new List<string>();

        void AddToLog(string[] values)
        {
            Debug.Log("ADDTOLOG CALLED");
            m_TestLog.AddRange(values);
        }

        [SetUp]
        public void Setup()
        {
            Debug.Log("SETUP CALLED");
            TaskDirector.OnLogAdded += AddToLog;
        }

        [TearDown]
        public void Teardown()
        {
            TaskDirector.OnLogAdded -= AddToLog;
            Debug.Log($"TEARDOWN CALLED {m_TestLog.Count}");

            m_TestLog.Clear();
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator TaskDirector_Logging()
        {
            new TestTask(TestLiterals.HelloWorld, 5, null, null).Enqueue();

            TaskDirector.Complete();
            //
            // // Force the tick on the director during the test
            // while (TaskDirector.HasTasks())
            // {
            //     TaskDirector.Tick();
            //     yield return null;
            // }
            //
            // // One final tick for our main thread callbacks
            // TaskDirector.Tick();

            // One more update
            yield return null;

            Assert.IsTrue(m_TestLog.Count == 5, $"{m_TestLog.Count} != 5");
        }
        public class TestTask : TaskBase
        {
            readonly string m_Message;
            readonly int m_Count;

            public TestTask(string message, int count, Action<TaskBase> onComplete, Action<TaskBase> onCompleteMainThread)
            {
                m_Message = message;
                m_Count = count;

                m_Name = $"TestTask ({count})";

                m_IsLogging = false;

                Completed += onComplete;
                CompletedMainThread += onCompleteMainThread;

                m_Bits[1] = true;
                m_BlockingBits[1] = true;

                m_BlockingModes = BlockingModeFlags.UserInteraction | BlockingModeFlags.SameName | BlockingModeFlags.Bits;
            }

            /// <inheritdoc />
            public override void DoWork()
            {
                for (int i = 0; i < m_Count; i++)
                {
                    TaskDirector.AddLog(m_Message);
                }

            }
        }
    }
}
