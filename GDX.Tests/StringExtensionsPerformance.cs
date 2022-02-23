// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_PERFORMANCETESTING
using NUnit.Framework;
using Unity.PerformanceTesting;

// ReSharper disable HeapView.ObjectAllocation, UnusedVariable

namespace GDX
{
    public class StringExtensionsPerformance
    {
        const int k_WarmupCount = 10;
        const int k_MeasurementCount = 20;
        const int k_IterationsPerMeasurement = 100;

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ToLower_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void GetStableLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ToUpper_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void IntTryParse()
        {
            Measure.Method(() =>
                {
                    int.TryParse("10", out int value);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int.TryParse("-12304912", out int value);
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void IsIntegerValue()
        {
            Measure.Method(() =>
                {
                    "10".IsIntegerValue();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    "-12304912".IsIntegerValue();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void GetStableUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }
    }
}
#endif