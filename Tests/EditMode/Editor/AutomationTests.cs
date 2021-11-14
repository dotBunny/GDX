// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Jobs.ParallelFor;
using NUnit.Framework;
#if GDX_COLLECTIONS
using GDX;
using Unity.Collections;
#endif
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using Automation = GDX.Editor.Automation;

namespace Editor
{
    public class AutomationTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void CaptureEditorWindow_SceneView_ReturnsTexture()
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
            }
            Texture2D texture = Automation.CaptureEditorWindow<SceneView>();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureEditorWindow_SceneView_SameTexture()
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
                return;
            }
#if GDX_COLLECTIONS
            Texture2D screenshotA = Automation.CaptureEditorWindow<SceneView>();
            Texture2D screenshotB = Automation.CaptureEditorWindow<SceneView>();
            NativeArray<Color32> screenshotDataA = screenshotA.GetRawTextureData<Color32>();
            NativeArray<Color32> screenshotDataB = screenshotB.GetRawTextureData<Color32>();
#if GDX_SAVE_TEST_OUTPUT
            string outputPathA = Automation.GetTempFilePath("CaptureEditorWindow_SceneView_SameTextureA-",".png");
            string outputPathB = Automation.GetTempFilePath("CaptureEditorWindow_SceneView_SameTextureB-",".png");
            File.WriteAllBytes(outputPathA, screenshotA.EncodeToPNG());
            File.WriteAllBytes(outputPathB, screenshotB.EncodeToPNG());
#endif // GDX_SAVE_TEST_OUTPUT

            int arrayLengthA = screenshotDataA.Length;
            int arrayLengthB = screenshotDataB.Length;
            if (arrayLengthA != arrayLengthB)
            {
                Assert.Fail("Screenshot array lengths differ.");
                return;
            }
            NativeArray<float> percentages = new NativeArray<float>(arrayLengthA, Allocator.TempJob);

            Color32CompareJob calcDifferencesJob = new GDX.Jobs.ParallelFor.Color32CompareJob()
            {
                A = screenshotDataA,
                B = screenshotDataB,
                Percentage = percentages
            };

            JobHandle handle = calcDifferencesJob.Schedule(arrayLengthA, 256);
            handle.Complete();

            float average = 0f;
            for (int i = 0; i < arrayLengthA; i++)
            {
                average += percentages[i];
            }
            average /= arrayLengthA;

            // Cleanup arrays
            screenshotDataA.Dispose();
            screenshotDataB.Dispose();
            percentages.Dispose();

            Assert.IsTrue(Math.Abs(average - 1) < Platform.FloatTolerance, $"Similarity was {average}%.");
#else
            Assert.Ignore("Collections required.");
#endif
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureEditorWindowToPNG_SceneView_OutputsImage()
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
                return;
            }
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
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
                return;
            }
            Texture2D texture = Automation.CaptureFocusedEditorWindow();
            bool evaluate = (texture != null);
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CaptureFocusedEditorWindowToPNG_OutputsImage()
        {
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
                return;
            }
            string outputPath = Automation.GetTempFilePath("CaptureFocusedEditorWindowToPNG_OutputsImage-",".png");
            bool execute = Automation.CaptureFocusedEditorWindowToPNG(outputPath);
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
            if (Application.isBatchMode)
            {
                Assert.Ignore("Unable to run test through Batch Mode.");
                return;
            }
            EditorWindow sceneView = Automation.GetWindow<SceneView>();
            bool evaluate = (sceneView != null);
            if (sceneView != null)
            {
                sceneView.Close();
            }

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