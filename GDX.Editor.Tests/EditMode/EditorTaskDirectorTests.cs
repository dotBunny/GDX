// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Threading;
using GDX.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Application = UnityEngine.Device.Application;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="EditorTaskDirector"/>.
    /// </summary>
    public class EditorTaskDirectorTests
    {
        // [UnityTearDown]
        // public IEnumerator TearDown()
        // {
        //     if (Application.isPlaying)
        //     {
        //         yield return new ExitPlayMode();
        //     }
        // }
        //
        // [UnityTest]
        // [Category(Core.TestCategory)]
        // public IEnumerator SetTickInPlayMode_False_NoTick()
        // {
        //     EditorTaskDirector.SetTickInPlayMode(false);
        //
        //     new CallbackTestTask().Enqueue();
        //
        //     yield return new EnterPlayMode();
        //     yield return new WaitForSeconds(1);
        //
        //     Assert.IsTrue(TaskDirector.GetBusyCount() == 0);
        //     Assert.IsTrue(TaskDirector.GetQueueCount() == 1);
        //
        //     yield return new ExitPlayMode();
        //
        //     EditorTaskDirector.SetTickInPlayMode(true);
        //
        //     yield return new WaitForSeconds(1);
        //
        //     Assert.IsTrue(TaskDirector.GetQueueCount() == 0);
        //
        // }

        // [UnityTest]
        // [Category(Core.TestCategory)]
        // public IEnumerator SetTickInPlayMode_True_Tick()
        // {
        //     yield return new EnterPlayMode();
        // }

        // GetTickInPlayMode
        // SetTickInPlayMode
        // EditorApplicationOnplayModeStateChanged
        // SetTickRate
        // EditorUpdate
        // EditorUpdateCallback
        class CallbackTestTask : TaskBase
        {
            public CallbackTestTask()
            {
                m_BlockingModes = BlockingModeFlags.All;
            }

            public override void DoWork()
            {
                Thread.Sleep(5);
            }
        }
    }
}
