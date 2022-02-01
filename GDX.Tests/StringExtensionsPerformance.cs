// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_PERFORMANCETESTING
using GDX;
using NUnit.Framework;
using Unity.PerformanceTesting;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    public class StringExtensionsPerformance
    {
        private const int WarmupCount = 10;
        private const int MeasurementCount = 20;
        private const int IterationsPerMeasurement = 100;

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void ToLower_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void GetStableLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void ToUpper_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void IntTryParse()
        {
            Measure.Method(() =>
                {
                    int.TryParse("10", out int value);
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int.TryParse("-12304912", out int value);
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void IsIntegerValue()
        {
            Measure.Method(() =>
                {
                    "10".IsIntegerValue();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    "-12304912".IsIntegerValue();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category(GDX.Core.PerformanceCategory)]
        public void GetStableUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = "HelloWorld!".GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "this is all lowercase".GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "THIS IS ALL UPPERCASE".GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }
    }
}
#endif