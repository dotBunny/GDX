﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using GDX.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace GDX.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="InputProxy" /> class.
    /// </summary>
    public class InputProxyTests
    {
        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Keyboard_Synthesize_CompoundExtendedKeys()
        {
            InputProxy.KeyboardInput[] mockInputs = new[]
            {
                new InputProxy.KeyboardInput(InputProxy.KeyCode.Control, InputProxy.KeyboardFlag.KeyDown, 0,IntPtr.Zero),
                new InputProxy.KeyboardInput(InputProxy.KeyCode.Shift, InputProxy.KeyboardFlag.KeyDown, 0, IntPtr.Zero),
                new InputProxy.KeyboardInput(InputProxy.KeyCode.N, InputProxy.KeyboardFlag.KeyDown, 0, IntPtr.Zero),

                new InputProxy.KeyboardInput(InputProxy.KeyCode.N, InputProxy.KeyboardFlag.KeyUp, 0, IntPtr.Zero),
                new InputProxy.KeyboardInput(InputProxy.KeyCode.Shift, InputProxy.KeyboardFlag.KeyUp, 0, IntPtr.Zero),
                new InputProxy.KeyboardInput(InputProxy.KeyCode.Control, InputProxy.KeyboardFlag.KeyUp, 0, IntPtr.Zero)

            };

            // We're going to add an empty game object to a scene from teh shortcut
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            SceneManager.SetActiveScene(scene);
            int previousCount = scene.GetRootGameObjects().Length;
            yield return null;
            uint count = InputProxy.Synthesize(mockInputs);
            yield return null;
            int postCount = scene.GetRootGameObjects().Length;
            yield return SceneManager.UnloadSceneAsync(scene);

            Assert.IsTrue(mockInputs.Length == count, $"{mockInputs.Length.ToString()} == {count.ToString()}");
            Assert.IsTrue(previousCount != postCount, $"{previousCount.ToString()} != {postCount.ToString()}");
            Assert.IsTrue(postCount == (previousCount+1), $"{postCount.ToString()} == ({previousCount.ToString()}+1)");
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator Mouse_Synthesize_MoveClickMoveClick()
        {

           // EditorWindow window = Automation.GetWindow<SceneView>(false);

            InputProxy.MouseInput[] mockInputs = new[]
            {
                new InputProxy.MouseInput(400, 400, 0, InputProxy.MouseFlag.Move | InputProxy.MouseFlag.Absolute, 0, IntPtr.Zero)
            };

            EditorWindow customWindow = Automation.GetWindow<SceneView>(
                new Automation.EditorWindowSetup(
                    false, true, false, true, 400, 400, true));

            Assert.IsTrue(customWindow.hasFocus);


            customWindow.Close();

            // We're going to add an empty game object to a scene from teh shortcut
            // Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            // SceneManager.SetActiveScene(scene);
            // int previousCount = scene.GetRootGameObjects().Length;
            // yield return null;
            uint count = InputProxy.Synthesize(mockInputs);
            yield return null;
            // yield return null;
            // int postCount = scene.GetRootGameObjects().Length;
            // yield return SceneManager.UnloadSceneAsync(scene);
            //
            Assert.IsTrue(mockInputs.Length == count, $"{mockInputs.Length.ToString()} == {count.ToString()}");
            // Assert.IsTrue(previousCount != postCount, $"{previousCount.ToString()} != {postCount.ToString()}");
            // Assert.IsTrue(postCount == (previousCount+1), $"{postCount.ToString()} == ({previousCount.ToString()}+1)");
        }
    }
}