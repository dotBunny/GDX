// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Threading;
using GDX.Developer;
using GDX.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
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
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;

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

            Assert.IsTrue(TaskDirector.GetBusyCount() == 0,
                $"Expected 0, {TaskDirector.GetBusyCount().ToString()} (Queued: {TaskDirector.GetQueueCount().ToString()})");
            Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}");

            EditorTaskDirector.SetTickRate(EditorTaskDirector.DefaultTickRate);
            yield return m_WaitForOneSecond.While();

            Assert.IsTrue(TaskDirector.GetBusyCount() == 1,
                $"Expected 1, {TaskDirector.GetBusyCount().ToString()} (Queued: {TaskDirector.GetQueueCount().ToString()})");
            Assert.IsTrue(TaskDirector.GetQueueCount() == 0, $"Expected 0, {TaskDirector.GetQueueCount().ToString()}");
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

            yield return new EnterPlayMode();

            new CallbackTestTask(1).Enqueue();

            yield return m_WaitForOneSecond.While();

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        }

        // [UnityTest]
        // [Category(Core.TestCategory)]
        // public IEnumerator SetTickInPlayMode_False_NoTick()
        // {
        //     // Ensure that before we start the test that we're zeroed out.
        //     int busyCount = TaskDirector.GetBusyCount();
        //     int queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 0,
        //         $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     // Set tick in playmode
        //     EditorTaskDirector.SetTickRate(0.1f);
        //     EditorTaskDirector.SetTickInPlayMode(false);
        //
        //     yield return new EnterPlayMode();
        //
        //     new CallbackTestTask(1).Enqueue();
        //
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 1,
        //         $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     yield return m_WaitForOneSecond.While(); // It is ticking for some reason
        //
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 1,
        //         $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        // }

        class CallbackTestTask : TaskBase
        {
            readonly int m_Delay;
            public CallbackTestTask(int delay)
            {
                m_Delay = delay;
                m_BlockingModes = BlockingModeFlags.All;
            }

            public override void DoWork()
            {
                Thread.Sleep(m_Delay);
            }
        }
    }
}
