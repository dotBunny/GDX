﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GDX.Classic.Jobs.ParallelFor;
using GDX.Editor;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

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

        [UnityTest]
        [Category(GDX.Core.TestCategory)]
        public IEnumerator CaptureEditorWindow_SceneView_SameTexture()
        {
            EditorWindow sceneViewA = Automation.GetWindow<SceneView>();
            yield return new GDX.Developer.WaitForMilliseconds(500).While();
            Texture2D screenshotA = Automation.CaptureEditorWindow(sceneViewA);
            sceneViewA.Close();

            EditorWindow sceneViewB = Automation.GetWindow<SceneView>();
            yield return new GDX.Developer.WaitForMilliseconds(500).While();
            Texture2D screenshotB = Automation.CaptureEditorWindow(sceneViewB);
            sceneViewB.Close();

            NativeArray<Color32> screenshotDataA = screenshotA.GetRawTextureData<Color32>();
            NativeArray<Color32> screenshotDataB = screenshotB.GetRawTextureData<Color32>();
            string outputPathA = Automation.GetTempFilePath("CaptureEditorWindow_SceneView_SameTextureA-",".png");
            string outputPathB = Automation.GetTempFilePath("CaptureEditorWindow_SceneView_SameTextureB-",".png");
            File.WriteAllBytes(outputPathA, screenshotA.EncodeToPNG());
            File.WriteAllBytes(outputPathB, screenshotB.EncodeToPNG());

            int arrayLengthA = screenshotDataA.Length;
            int arrayLengthB = screenshotDataB.Length;
            if (arrayLengthA != arrayLengthB)
            {
                Assert.Fail($"Screenshot array lengths differ ({arrayLengthA} vs {arrayLengthB}).");
                yield break;
            }
            NativeArray<float> percentages = new NativeArray<float>(arrayLengthA, Allocator.TempJob);

            Color32CompareJob calcDifferencesJob = new GDX.Classic.Jobs.ParallelFor.Color32CompareJob()
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

            Assert.IsTrue(Math.Abs(average - 1) < GDX.Platform.FloatTolerance, $"Similarity was {average}%.");
        }
    }
}