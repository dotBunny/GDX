// // Copyright (c) 2020-2022 dotBunny Inc.
// // dotBunny licenses this file to you under the BSL-1.0 license.
// // See the LICENSE file in the project root for more information.
//
// using System.Collections;
// using System.Threading;
// using GDX.Threading;
// using NUnit.Framework;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.TestTools;
//
//
// namespace GDX.Editor
// {
//     /// <summary>
//     ///     A collection of unit tests to validate functionality of the <see cref="EditorTaskDirector"/>.
//     /// </summary>
//     public class EditorTaskDirectorTests
//     {
//         EnterPlayModeOptions _previousOptions;
//         bool _previousToggle;
//         bool _tickInPlayMode;
//         double _tickRate;
//
//         [UnitySetUp]
//         public IEnumerator Setup()
//         {
//             _previousToggle = EditorSettings.enterPlayModeOptionsEnabled;
//             _previousOptions = EditorSettings.enterPlayModeOptions;
//             _tickInPlayMode = EditorTaskDirector.GetTickInPlayMode();
//             _tickRate = EditorTaskDirector.GetTickRate();
//
//             EditorSettings.enterPlayModeOptionsEnabled = true;
//             EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
//
//             // Wait for any outstanding to finish
//             yield return TaskDirector.WaitAsync().AsIEnumerator();;
//
//             yield return null;
//         }
//
//         [UnityTearDown]
//         public IEnumerator TearDown()
//         {
//             if (Application.isPlaying)
//             {
//                 yield return new ExitPlayMode();
//             }
//
//             yield return null;
//
//             EditorTaskDirector.SetTickInPlayMode(_tickInPlayMode);
//             EditorTaskDirector.SetTickRate(_tickRate);
//             EditorSettings.enterPlayModeOptionsEnabled = _previousToggle;
//             EditorSettings.enterPlayModeOptions = _previousOptions;
//
//             yield return TaskDirector.WaitAsync().AsIEnumerator();;
//         }
//
//         [UnityTest]
//         [Category(Core.TestCategory)]
//         public IEnumerator SetTickInPlayMode_False_NoTick()
//         {
//             // Validate we are starting right
//             Assert.IsTrue(TaskDirector.GetBusyCount() == 0, $"Expected 0, {TaskDirector.GetBusyCount().ToString()}");
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 0, $"Expected 0, {TaskDirector.GetQueueCount().ToString()}");
//
//             // Stop ticking outside temporarily
//             EditorTaskDirector.SetTickRate(0);
//             EditorTaskDirector.SetTickInPlayMode(false);
//
//             // This should add but do nothing
//             new CallbackTestTask().Enqueue();
//
//             Assert.IsTrue(TaskDirector.GetBusyCount() == 0, $"Expected 0, {TaskDirector.GetBusyCount().ToString()}");
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}");
//
//             // Going into playmode can reload the domain, we have turned it off for this test,
//             // otherwise things would disappear between loads
//             yield return new EnterPlayMode();
//
//             Assert.IsTrue(TaskDirector.GetBusyCount() == 0, $"Expected 0, {TaskDirector.GetBusyCount().ToString()}");
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}"); // task shouldnt be procesing yet
//
//             EditorTaskDirector.SetTickRate(EditorTaskDirector.DefaultTickRate);
//
//             Assert.IsTrue(TaskDirector.GetBusyCount() == 0, $"Expected 0, {TaskDirector.GetBusyCount().ToString()}");
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}");
//
//             yield return null;
//
//             Assert.IsTrue(TaskDirector.GetBusyCount() == 0, $"Expected 0, {TaskDirector.GetBusyCount().ToString()}");
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 1, $"Expected 1, {TaskDirector.GetQueueCount().ToString()}");
//
//             yield return new ExitPlayMode();
//
//             EditorTaskDirector.SetTickInPlayMode(true);
//
//             yield return new EnterPlayMode();
//
//             new CallbackTestTask().Enqueue();
//
//             yield return null;
//             yield return null;
//             Thread.Sleep(1000);
//             yield return null;
//             yield return null;
//
//
//             Assert.IsTrue(TaskDirector.GetQueueCount() == 0, $"Expected 0, {TaskDirector.GetQueueCount().ToString()}");
//
//             yield return new ExitPlayMode();
//
//             yield return null;
//         }
//
//         // SetTickRate
//         // EditorUpdate
//         // EditorUpdateCallback
//         class CallbackTestTask : TaskBase
//         {
//             public CallbackTestTask()
//             {
//                 m_BlockingModes = BlockingModeFlags.All;
//             }
//
//             public override void DoWork()
//             {
//                 Thread.Sleep(1);
//             }
//         }
//     }
// }
