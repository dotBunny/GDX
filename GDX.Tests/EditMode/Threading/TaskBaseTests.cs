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
        List<string> m_Log = new List<string>();
        void LogAdded(string[] values)
        {
            m_Log.AddRange(values);
        }

        [SetUp]
        public void Setup()
        {
            TaskDirector.OnLogAdded += LogAdded;
        }

        [TearDown]
        public void Teardown()
        {
            TaskDirector.OnLogAdded -= LogAdded;
            m_Log.Clear();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TaskDirector_Logging()
        {
            new TestTask(TestLiterals.HelloWorld, 5, null, null).Enqueue();

            TaskDirector.Wait();

            Assert.IsTrue(m_Log.Count == 5, $"{m_Log.Count} != 5");
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
