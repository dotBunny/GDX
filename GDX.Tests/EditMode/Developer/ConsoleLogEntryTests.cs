// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ConsoleLogEntry" /> struct.
    /// </summary>
    public class ConsoleLogEntryTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void ConsoleLogEntry_OnAnotherThread_TakesTheMainThreadsLastFrame()
        {
            Core.InitializeOnMainThread();
            ConsoleLogEntry onMain = new ConsoleLogEntry(LogType.Log, "main");
            Assert.AreEqual(Time.frameCount, onMain.Frame);

            ConsoleLogEntry onWorker = default;
            Exception thrown = null;
            Thread worker = new Thread(() =>
            {
                try
                {
                    onWorker = new ConsoleLogEntry(LogType.Warning, "worker");
                }
                catch (Exception e)
                {
                    thrown = e;
                }
            });
            worker.Start();
            worker.Join();

            Assert.IsNull(thrown, "Time.frameCount is read only on the main thread");
            Assert.AreEqual(onMain.Frame, onWorker.Frame);
            Assert.AreEqual(LogType.Warning, onWorker.Level);
            Assert.AreEqual("worker", onWorker.Message);
        }
    }
}
