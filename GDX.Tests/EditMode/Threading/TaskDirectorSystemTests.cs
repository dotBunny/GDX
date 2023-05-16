// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using GDX.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace GDX.Threading
{
    public class TaskDirectorSystemTests
    {
        EnterPlayModeOptions m_PreviousOptions;
        bool m_PreviousToggle;
        bool m_PreviousEnvironmentEditorTaskDirector;
        bool m_PreviousEnvironmentTaskDirector;

        float m_PreviousTickRate;
        double m_PreviousEditorTickRate;
        int m_Counter;

        Scene m_TestScene;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Ensure we are not in a scene
            m_TestScene = TestFramework.ForceEmptyScene();

            // Cache previous settings we are bound to play with
            m_PreviousToggle = EditorSettings.enterPlayModeOptionsEnabled;
            m_PreviousOptions = EditorSettings.enterPlayModeOptions;
            m_PreviousEnvironmentTaskDirector = Config.TaskDirectorSystem;
            m_PreviousEnvironmentEditorTaskDirector = Config.EditorTaskDirectorSystem;
            m_PreviousEditorTickRate = EditorTaskDirectorSystem.GetTickRate();
            m_PreviousTickRate = TaskDirectorSystem.GetTickRate();
            m_Counter = 0;

            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload |
                                                  EnterPlayModeOptions.DisableSceneReload;

            EditorTaskDirectorSystem.SetTickRate(-1);
            Config.TaskDirectorSystem = true;

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

            Config.TaskDirectorSystem = m_PreviousEnvironmentTaskDirector;
            Config.EditorTaskDirectorSystem = m_PreviousEnvironmentEditorTaskDirector;
            TaskDirectorSystem.SetTickRate(m_PreviousTickRate);
            EditorTaskDirectorSystem.SetTickRate(m_PreviousEditorTickRate);

            EditorSettings.enterPlayModeOptionsEnabled = m_PreviousToggle;
            EditorSettings.enterPlayModeOptions = m_PreviousOptions;

            // Only unload if there is more then one
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(m_TestScene);
            }

            yield return TaskDirector.WaitAsync().AsIEnumerator();
        }

        void IncrementCounter(float delta)
        {
            m_Counter++;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Task_Complete_TicksDirector()
        {
            InstantTestTask task = new InstantTestTask();
            task.Enqueue();
            task.Complete();
            Assert.IsTrue(task.Finished);
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Initialize_DefaultTickRate()
        {
            Config.TaskDirectorSystem = true;
            TaskDirectorSystem.SetTickRate(-1);
            yield return new EnterPlayMode();
            yield return WaitFor.GetEnumerator(WaitFor.OneSecond);
            Assert.IsTrue(EditorApplication.isPlaying);
            Assert.IsTrue(Math.Abs(TaskDirectorSystem.GetTickRate() - Config.TaskDirectorSystemTickRate) < Platform.FloatTolerance);
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Offline_NoTick()
        {
            // Ensure that before we start the test that we're zeroed out.
            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            Config.TaskDirectorSystem = false;

            // Set tick in playmode
            yield return new EnterPlayMode();
            yield return WaitFor.GetEnumerator(WaitFor.OneSecond);
            Assert.IsTrue(EditorApplication.isPlaying);

            new CallbackTestTask(1).Enqueue();

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            yield return WaitFor.GetEnumerator(WaitFor.OneSecond);

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Online_Ticks()
        {
            // Ensure that before we start the test that we're zeroed out.
            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            Config.TaskDirectorSystem = true;
            TaskDirectorSystem.SetTickRate(0.1f);

            // Check that system is not present
            PlayerLoopSystem beforePlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsFalse(beforePlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));

            yield return new EnterPlayMode(false);

            // Check that system was added correctly
            PlayerLoopSystem duringPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsTrue(duringPlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));

            new CallbackTestTask(1).Enqueue();

            yield return WaitFor.GetEnumerator(WaitFor.OneSecond);

            // Check that we actually ticked and cleared
            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 0,
                $"Expected 0/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            yield return new ExitPlayMode();

            // Validate that we removed the system
            PlayerLoopSystem afterPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            Assert.IsFalse(afterPlayerLoop.GenerateSystemTree().ToString().Contains(nameof(TaskDirectorSystem)));
        }
        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Ticks_AtRate()
        {
            // Ensure that before we start the test that we're zeroed out.
            Config.TaskDirectorSystem = true;
            TaskDirectorSystem.SetTickRate(1.9f); // Need to be just under the 2 seconds to ensure we always tick right
            TaskDirectorSystem.ticked += IncrementCounter;

            yield return new EnterPlayMode(false);
            yield return WaitFor.GetEnumerator(WaitFor.TwoSeconds);
            yield return new ExitPlayMode();

            TaskDirectorSystem.ticked -= IncrementCounter;
            Assert.IsTrue(m_Counter == 1, m_Counter.ToString());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetTickRate_WhileOff_Warning()
        {
            Config.TaskDirectorSystem = false;
            TaskDirectorSystem.SetTickRate(1);
            LogAssert.Expect(LogType.Warning,
            "Tick rate set whilst TaskDirectorSystem has been configured off.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Invoke_PlayerLoopTick_NoTickInEditMode()
        {
            Reflection.InvokeStaticMethod("GDX.Threading.TaskDirectorSystem", "PlayerLoopTick", null,
                Reflection.PrivateStaticFlags);
            LogAssert.Expect(LogType.Warning,
                "Unable to tick Task Director from PlayerLoop outside of PlayMode.");
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

        class InstantTestTask : TaskBase
        {
            public bool Finished;

            /// <inheritdoc />
            public override void DoWork()
            {
                Finished = true;
            }
        }
    }
}