// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Classic.Jobs.ParallelFor;
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
        [Ignore("Flaky")]
        public void CaptureEditorWindow_SceneView_SameTexture()
        {
            Texture2D screenshotA = Automation.CaptureEditorWindow<SceneView>();
            Texture2D screenshotB = Automation.CaptureEditorWindow<SceneView>();
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
                Assert.Fail("Screenshot array lengths differ.");
                return;
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