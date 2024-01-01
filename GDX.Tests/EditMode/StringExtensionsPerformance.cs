// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_PERFORMANCETESTING
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace GDX
{
    public class StringExtensionsPerformance
    {
        const int k_WarmupCount = 10;
        const int k_MeasurementCount = 20;
        const int k_IterationsPerMeasurement = 100;

        static readonly SampleGroup k_ComplexSample = new SampleGroup("Complex");
        static readonly SampleGroup k_SimpleSample = new SampleGroup("Simple");
        static readonly SampleGroup k_UpperCaseSample = new SampleGroup("UpperCase");
        static readonly SampleGroup k_LowerCaseSample = new SampleGroup("LowerCase");

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ToLower_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int unused = TestLiterals.HelloWorld.ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_SimpleSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllLowerCase.ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LowerCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllUpperCase.ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_UpperCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.Complex.ToLower().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_ComplexSample)
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void GetStableLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int unused = TestLiterals.HelloWorld.GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_SimpleSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllLowerCase.GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LowerCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllUpperCase.GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_UpperCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.Complex.GetStableLowerCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_ComplexSample)
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void ToUpper_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int unused = TestLiterals.HelloWorld.ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_SimpleSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllLowerCase.ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LowerCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllUpperCase.ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_UpperCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.Complex.ToUpper().GetHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_ComplexSample)
                .Run();
        }

        [Test]
        [Performance]
        [Category(Core.PerformanceCategory)]
        public void GetStableUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int unused = TestLiterals.HelloWorld.GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_SimpleSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllLowerCase.GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_LowerCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.AllUpperCase.GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_UpperCaseSample)
                .Run();
            Measure.Method(() =>
                {
                    int unused = StringExtensionsTests.Complex.GetStableUpperCaseHashCode();
                })
                .WarmupCount(k_WarmupCount)
                .MeasurementCount(k_MeasurementCount)
                .IterationsPerMeasurement(k_IterationsPerMeasurement).SampleGroup(k_ComplexSample)
                .Run();
        }
    }
}
#endif // GDX_PERFORMANCETESTING