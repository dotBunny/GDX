// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Threading;
using GDX.Developer;
using GDX.Threading;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        WaitForMilliseconds _waitForOneSecond = new WaitForMilliseconds(WaitForMilliseconds.OneSecond);
        EnterPlayModeOptions _previousOptions;
        bool _previousToggle;
        bool _previousTickInPlayMode;
        double _previousTickRate;
        Scene _testScene;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Ensure we are not in a scene
            _testScene = TestFramework.ForceEmptyScene();

            // Cache previous settings we are bound to play with
            _previousToggle = EditorSettings.enterPlayModeOptionsEnabled;
            _previousOptions = EditorSettings.enterPlayModeOptions;
            _previousTickInPlayMode = EditorTaskDirector.GetTickInPlayMode();
            _previousTickRate = EditorTaskDirector.GetTickRate();

            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;

            _waitForOneSecond.Reset();

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

            EditorTaskDirector.SetTickInPlayMode(_previousTickInPlayMode);
            EditorTaskDirector.SetTickRate(_previousTickRate);
            EditorSettings.enterPlayModeOptionsEnabled = _previousToggle;
            EditorSettings.enterPlayModeOptions = _previousOptions;

            // Only unload if there is more then one
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(_testScene);
            }

            yield return TaskDirector.WaitAsync().AsIEnumerator();
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator SetTickRate_QueuesOnZero_BusyWhenSet()
        {
            EditorTaskDirector.SetTickRate(0);
            new CallbackTestTask(WaitForMilliseconds.TenSeconds).Enqueue();

            Assert.IsTrue(TaskDirector.GetBusyCount() == 0,
                $"Expected 0, {TaskDirector.GetBusyCount().ToString()} (Queued: {TaskDirector.GetQueueCount().ToString()})");
            Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}");

            EditorTaskDirector.SetTickRate(EditorTaskDirector.DefaultTickRate);
            yield return _waitForOneSecond.While();

            Assert.IsTrue(TaskDirector.GetBusyCount() == 1,
                $"Expected 1, {TaskDirector.GetBusyCount().ToString()} (Queued: {TaskDirector.GetQueueCount().ToString()})");
            Assert.IsTrue(TaskDirector.GetQueueCount() == 0, $"Expected 0, {TaskDirector.GetQueueCount().ToString()}");
        }

        // [UnityTest]
        // [Category(Core.TestCategory)]
        // public IEnumerator SetTickInPlayMode_False_NoTick()
        // {
        //     // Ensure that before we start the test that we're zero'd out.
        //     int busyCount = TaskDirector.GetBusyCount();
        //     int queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 0,
        //         $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     // Stop ticking in general
        //     EditorTaskDirector.SetTickRate(0);
        //
        //     // No ticking in playmode yet either
        //     EditorTaskDirector.SetTickInPlayMode(false);
        //
        //     // This should add but do nothing
        //     new CallbackTestTask(WaitForMilliseconds.TenSeconds).Enqueue();
        //
        //     // Check if we have just 1 task queued since it is unable to run
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 1,
        //         $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     // Going into playmode can reload the domain, we have turned it off for this test,
        //     // otherwise things would disappear between loads
        //     yield return new EnterPlayMode();
        //
        //     // Still maintain that nothing has moved even through the entering playmode
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 1,
        //         $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     // Test setting the tick rate to something, wait enough that it could in theory tick
        //     EditorTaskDirector.SetTickRate(EditorTaskDirector.DefaultTickRate);
        //     yield return _waitForOneSecond.While();
        //     _waitForOneSecond.Reset();
        //
        //     // The task should remain still queued up because while ticking is enabled, in playmode has
        //     // not been enabled
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 1,
        //         $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     // Wait for the task to clear out in edit mode
        //     yield return new ExitPlayMode();
        //     yield return TaskDirector.WaitAsync().AsIEnumerator();
        //
        //     // Allow ticking in editor mode
        //     EditorTaskDirector.SetTickInPlayMode(true);
        //     yield return new EnterPlayMode();
        //
        //     // Add new task to the queue that should be processed
        //     new CallbackTestTask(WaitForMilliseconds.OneSecond).Enqueue();
        //
        //     // Wait for two seconds which should be enough to process the task and run it
        //     _waitForOneSecond.Reset();
        //     yield return _waitForOneSecond.While();
        //     _waitForOneSecond.Reset();
        //     yield return _waitForOneSecond.While();
        //
        //     busyCount = TaskDirector.GetBusyCount();
        //     queueCount = TaskDirector.GetQueueCount();
        //     Assert.IsTrue(
        //         busyCount == 0 && queueCount == 0,
        //         $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        //
        //     yield return new ExitPlayMode();
        //     yield return null;
        // }

        // SetTickRate
        // EditorUpdate
        // EditorUpdateCallback
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
