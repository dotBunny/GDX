// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Editor;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AutomationTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void CaptureEditorWindow_SceneView_ReturnsTexture()
        {
            Texture2D texture = Automation.CaptureEditorWindow<SceneView>();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void CaptureEditorWindowToPNG_SceneView_OutputsImage()
        {
            string outputPath = Automation.GetTempFilePath("CaptureEditorWindowToPNG_SceneView_OutputsImage-",".png");
            bool execute = Automation.CaptureEditorWindowToPNG<SceneView>(outputPath);
            bool evaluate = execute && File.Exists(outputPath);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void CaptureFocusedEditorWindow_ReturnsTexture()
        {
            Automation.GetGameView().Focus();
            Texture2D texture = Automation.CaptureFocusedEditorWindow();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void CaptureFocusedEditorWindowToPNG_OutputsImage()
        {
            Automation.GetGameView().Focus();
            string outputPath = Automation.GetTempFilePath("CaptureFocusedEditorWindowToPNG_OutputsImage-",".png");
            bool execute = Automation.CaptureFocusedEditorWindowToPNG(outputPath);
            bool evaluate = execute && File.Exists(outputPath);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetTempFolder_EnsureExists_FolderExists()
        {
            string path = Automation.GetTempFolder();
            bool evaluate = Directory.Exists(path);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void GetWindow_SceneView_NotNull()
        {
            EditorWindow sceneView = Automation.GetWindow<SceneView>();
            bool evaluate = (sceneView != null);
            if (sceneView != null)
            {
                sceneView.Close();
            }

            Assert.IsTrue(evaluate);
        }
    }
}