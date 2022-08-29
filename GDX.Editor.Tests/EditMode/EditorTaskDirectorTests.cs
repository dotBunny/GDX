// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Threading;
using GDX.Developer;
using GDX.Threading;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="EditorTaskDirectorSystem"/>.
    /// </summary>
    public class EditorTaskDirectorTests
    {
        bool m_PreviousEditorTaskDirectorSystem;
        double m_PreviousEditorTickRate;
        Scene m_TestScene;

        [UnitySetUp]
        public IEnumerator Setup()
        {
            // Ensure we are not in a scene
            m_TestScene = TestFramework.ForceEmptyScene();

            // Cache previous settings we are bound to play with
            m_PreviousEditorTaskDirectorSystem = Config.EditorTaskDirectorSystem;
            m_PreviousEditorTickRate = EditorTaskDirectorSystem.GetTickRate();

            Config.EditorTaskDirectorSystem = true;

            // Wait for any outstanding to finish
            yield return TaskDirector.WaitAsync().AsIEnumerator();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            // Restore tick rate
            Config.EditorTaskDirectorSystem = m_PreviousEditorTaskDirectorSystem;
            EditorTaskDirectorSystem.SetTickRate(m_PreviousEditorTickRate);

            // Only unload if there is more then one
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(m_TestScene);
            }

            yield return TaskDirector.WaitAsync().AsIEnumerator();
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator SetTickRate_QueuesOnLessThanZero_BusyWhenSet()
        {

            EditorTaskDirectorSystem.SetTickRate(-1);
            new CallbackTestTask(WaitFor.TwoSeconds).Enqueue();

            int busyCount = TaskDirector.GetBusyCount();
            int queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 0 && queueCount == 1,
                $"Expected 0/1 - Found {busyCount.ToString()}/{queueCount.ToString()}");

            EditorTaskDirectorSystem.SetTickRate(0.1f);

            yield return WaitFor.While(WaitFor.OneSecond);

            busyCount = TaskDirector.GetBusyCount();
            queueCount = TaskDirector.GetQueueCount();
            Assert.IsTrue(
                busyCount == 1 && queueCount == 0,
                $"Expected 1/0 - Found {busyCount.ToString()}/{queueCount.ToString()}");
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
