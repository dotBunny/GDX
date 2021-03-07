// dotBunny licenses this file to you under the MIT license.
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

        private const string SimpleIntegerTestString = "10";
        private const string ComplexIntegerTestString = "-12304912";

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_ToLower_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.SimpleTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.LowerCaseTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.UpperCaseTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.ComplexTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_GetStableLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.SimpleTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.LowerCaseTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.UpperCaseTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.ComplexTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_ToUpper_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.SimpleTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.LowerCaseTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.UpperCaseTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.ComplexTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_IntTryParse()
        {
            Measure.Method(() =>
                {
                    int.TryParse(SimpleIntegerTestString, out int value);
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int.TryParse(ComplexIntegerTestString, out int value);
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_IsIntegerValue()
        {
            Measure.Method(() =>
                {
                    SimpleIntegerTestString.IsIntegerValue();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    ComplexIntegerTestString.IsIntegerValue();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }

        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_GetStableUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.SimpleTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.LowerCaseTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.UpperCaseTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = StringExtensionsTests.ComplexTestString.GetStableUpperCaseHashCode();
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