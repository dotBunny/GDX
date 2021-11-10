// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Unity.Collections;

namespace Runtime.Jobs.ParallelFor
{
    public class ColorCompareJobTests
    {
        // [Test]
        // [Category("GDX.Tests")]
        // public void Compare_FiftyFifty_()
        // {
        //     //ProcessArguments_MockData_ContainsFlag()
        // }
        //
        // var screenshot = TestFramework.TakeScreenshot(Vector3.zero, Quaternion.identity);
        // var screenshotData = screenshot.GetPixelData<Color>(0);
        // var arrayLength = screenshotData.Length;
        // var percentages = new NativeArray<float>(arrayLength, Allocator.TempJob);
        //
        // var calcDifferencesJob = new ParallelColorCompareJob()
        // {
        //     A = screenshotData,
        //     B = screenshotData,
        //     Percentage = percentages
        // };
        //
        // var handle = calcDifferencesJob.Schedule(arrayLength, 64);
        // handle.Complete();
        //
        // float average = 0;
        //     for (var i = 0; i < arrayLength; i++)
        // {
        //     average += percentages[i];
        // }
        // average = average / arrayLength;
        //
        // screenshotData.Dispose();
        // percentages.Dispose();
        //
        // Assert.IsTrue(average == 1);
    }
}