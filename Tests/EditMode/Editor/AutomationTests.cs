// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AutomationTests
    {
        /*
         * CaptureCamera
         * CaptureCameraToPNG
         * CaptureCamera
         * CaptureCameraToPNG
         */

        [Test]
        [Category("GDX.Tests")]
        public void CaptureEditorWindow_SceneView_ReturnsTexture()
        {
            Texture2D texture = Automation.CaptureEditorWindow<SceneView>();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureEditorWindowToPNG_SceneView_OutputsImage()
        {
            string outputPath = Automation.GetTempFilePath("CaptureEditorWindowToPNG_SceneView_OutputsImage-",".png");
            bool execute = Automation.CaptureEditorWindowToPNG<SceneView>(outputPath);
            bool evaluate = execute && File.Exists(outputPath);
#if !GDX_SAVE_TEST_OUTPUT
            File.Delete(outputPath);
#endif // !GDX_SAVE_TEST_OUTPUT
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureFocusedEditorWindow_ReturnsTexture()
        {
            Texture2D texture = Automation.CaptureFocusedEditorWindow();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureFocusedEditorWindowToPNG_OutputsImage()
        {
            string outputPath = Automation.GetTempFilePath("CaptureFocusedEditorWindowToPNG_OutputsImage-",".png");
            bool execute = Automation.CaptureGameViewWindowToPNG(outputPath);
            bool evaluate = execute && File.Exists(outputPath);
#if !GDX_SAVE_TEST_OUTPUT
            File.Delete(outputPath);
#endif
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureGameViewWindow_ReturnsTexture()
        {
            Texture2D texture = Automation.CaptureGameViewWindow();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureGameViewWindowToPNG_OutputsImage()
        {
            string outputPath = Automation.GetTempFilePath("CaptureGameViewWindowToPNG_OutputsImage-",".png");
            bool execute = Automation.CaptureGameViewWindowToPNG(outputPath);
            bool evaluate = execute && File.Exists(outputPath);
#if !GDX_SAVE_TEST_OUTPUT
            File.Delete(outputPath);
#endif
            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Tests")]
        public void ClearTempFolder_DontDelete_EmptyFolder()
        {
#if !GDX_SAVE_TEST_OUTPUT
            string path = Automation.GetTempFolder();
            File.WriteAllText(Path.Combine(path, "test.dat"), "ClearTempFolder_DontDelete_EmptyFolder");

            Automation.ClearTempFolder();
            DirectoryInfo di = new DirectoryInfo(path);

            bool evaluate = !(di.GetFiles().Length > 0 || di.GetDirectories().Length > 0);
            Assert.IsTrue(evaluate);
#else
            Assert.Ignore("Disabled due to GDX_SAVE_TEST_OUTPUT.");
#endif
        }

        // [Test]
        // [Category("GDX.Tests")]
        // public void GenerateProjectFiles_CreatesSolution()
        // {
        //     Automation.GenerateProjectFiles();
        // }

        // [Test]
        // [Category("GDX.Tests")]
        // public void GenerateProjectFiles_CreatesProjectFiles()
        // {
        //     Automation.GenerateProjectFiles();
        // }
        [Test]
        [Category("GDX.Tests")]
        public void GetTempFolder_EnsureExists_FolderExists()
        {
            string path = Automation.GetTempFolder();
            bool evaluate = Directory.Exists(path);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetWindow_SceneView_NotNull()
        {
            EditorWindow sceneView = Automation.GetWindow<SceneView>();
            bool evaluate = (sceneView != null);
            Assert.IsTrue(evaluate);
        }

        // [Test]
        // [Category("GDX.Tests")]
        // public void ResetEditor_ResetsLayout()
        // {
        //     //ProcessArguments_MockData_ContainsFlag()
        // }
    }
}