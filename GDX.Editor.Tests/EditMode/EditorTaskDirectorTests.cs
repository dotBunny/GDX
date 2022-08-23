// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using GDX.Developer;
using GDX.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


namespace GDX.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="EditorTaskDirector"/>.
    /// </summary>
    public class EditorTaskDirectorTests
    {
        readonly WaitForMilliseconds m_WaitForOneSecond = new WaitForMilliseconds(WaitForMilliseconds.OneSecond);
        EnterPlayModeOptions m_PreviousOptions;
        bool m_PreviousToggle;
        bool m_PreviousTickInPlayMode;
        double m_PreviousTickRate;
        Scene m_TestScene;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Ensure we are not in a scene
            m_TestScene = TestFramework.ForceEmptyScene();

            // Cache previous settings we are bound to play with
            m_PreviousToggle = EditorSettings.enterPlayModeOptionsEnabled;
            m_PreviousOptions = EditorSettings.enterPlayModeOptions;
            m_PreviousTickInPlayMode = EditorTaskDirector.GetTickInPlayMode();
            m_PreviousTickRate = EditorTaskDirector.GetTickRate();

            EditorSettings.enterPlayModeOptionsEnabled = true;
#if UNITY_2022_1_OR_NEWER
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload |
                                                  EnterPlayModeOptions.DisableSceneReload |
                                                  EnterPlayModeOptions.DisableSceneBackupUnlessDirty;
#else
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload |
                                                  EnterPlayModeOptions.DisableSceneReload;
#endif
            m_WaitForOneSecond.Reset();

            // Wait for any outstanding to finish
            yield return TaskDirector.WaitAsync().AsIEnumerator();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (Application.isPlaying)
            {
                yield return new ExitPlayMode();
            }
            yield return null;

            EditorTaskDirector.SetTickInPlayMode(m_PreviousTickInPlayMode);
            EditorTaskDirector.SetTickRate(m_PreviousTickRate);
            EditorSettings.enterPlayModeOptionsEnabled = m_PreviousToggle;
            EditorSettings.enterPlayModeOptions = m_PreviousOptions;

            // Only unload if there is more then one
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(m_TestScene);
            }

            yield return TaskDirector.WaitAsync().AsIEnumerator();
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator SetTickRate_QueuesOnZero_BusyWhenSet()
        {
            EditorTaskDirector.SetTickRate(0);
            new CallbackTestTask(WaitForMilliseconds.TwoSeconds).Enqueue();

            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            EditorTaskDirector.SetTickRate(0.1f);

            yield return m_WaitForOneSecond.While();

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 1 && queueCount == 0,
                $"Expected 1/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator SetTickInPlayMode_True_Ticks()
        {
            // Ensure that before we start the test that we're zeroed out.
            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            // Set tick in playmode
            EditorTaskDirector.SetTickRate(0.1f);
            EditorTaskDirector.SetTickInPlayMode(true);

            float preRuntimeTick = TaskDirectorSystem.GetTickRate();
            TaskDirectorSystem.SetTickRate(0.1f);

            PlayerLoopSystem beforePlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsFalse(beforePlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));

            yield return new EnterPlayMode(false);

            PlayerLoopSystem duringPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsTrue(duringPlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));

            new CallbackTestTask(1).Enqueue();

            yield return m_WaitForOneSecond.While();

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            yield return new ExitPlayMode();
            TaskDirectorSystem.SetTickRate(preRuntimeTick);

            PlayerLoopSystem afterPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsFalse(afterPlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator SetTickInPlayMode_False_NoTick()
        {
            // Ensure that before we start the test that we're zeroed out.
            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            // Set tick in playmode
            EditorTaskDirector.SetTickRate(0.1f);
            EditorTaskDirector.SetTickInPlayMode(false);

            yield return new EnterPlayMode(true);
            m_WaitForOneSecond.Reset();
            yield return m_WaitForOneSecond.While();
            m_WaitForOneSecond.Reset();
            Assert.IsTrue(EditorApplication.isPlaying);

            new CallbackTestTask(1).Enqueue();

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            yield return m_WaitForOneSecond.While(); // ISSUE TODO It is ticking for some reason

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        }

        class CallbackTestTask : TaskBase
        {
            readonly int m_Delay;
            public CallbackTestTask(int delay)
            {
                m_Delay = delay;
                m_BlockingModes = BlockingModeFlags.All;
                m_Name = "CallbackTestTask";
            }

            public override void DoWork()
            {
                Thread.Sleep(m_Delay);
            }
        }
    }
}
