// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GDX.Jobs.ParallelFor
{
    public class Color32CompareJobTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Compare_WhiteTexture_Same()
        {
            NativeArray<Color32> whiteData = Texture2D.whiteTexture.GetRawTextureData<Color32>();
            int arrayLength = whiteData.Length;
            NativeArray<float> percentages = new NativeArray<float>(arrayLength, Allocator.TempJob);
            Color32CompareJob calcDifferencesJob = new Color32CompareJob()
            {
                A = whiteData,
                B = whiteData,
                Percentage = percentages
            };

            JobHandle handle = calcDifferencesJob.Schedule(arrayLength, 256);
            handle.Complete();

            float average = 0f;
            for (int i = 0; i < arrayLength; i++)
            {
                average += percentages[i];
            }
            average /= arrayLength;

            // Cleanup arrays
            whiteData.Dispose();
            percentages.Dispose();

            bool evaluate = (Math.Abs(average - 1) < GDX.Platform.FloatTolerance);

            Assert.IsTrue(evaluate, $"White texture similarity was {average}%.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Compare_WhiteBlack_Different()
        {
            NativeArray<Color32> whiteData = Texture2D.whiteTexture.GetRawTextureData<Color32>();
            NativeArray<Color32> blackData = Texture2D.blackTexture.GetRawTextureData<Color32>();
            int arrayLength = whiteData.Length;
            NativeArray<float> percentages = new NativeArray<float>(arrayLength, Allocator.TempJob);
            Color32CompareJob calcDifferencesJob = new Color32CompareJob()
            {
                A = whiteData,
                B = blackData,
                Percentage = percentages
            };

            JobHandle handle = calcDifferencesJob.Schedule(arrayLength, 256);
            handle.Complete();

            float average = 0f;
            for (int i = 0; i < arrayLength; i++)
            {
                average += percentages[i];
            }
            average /= arrayLength;

            // Cleanup arrays
            whiteData.Dispose();
            blackData.Dispose();
            percentages.Dispose();

            bool evaluate = (average == 0f);

            Assert.IsTrue(evaluate, $"White texture similarity was {average}%.");
        }
    }
}