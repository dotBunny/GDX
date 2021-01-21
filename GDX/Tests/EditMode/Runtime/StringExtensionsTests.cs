// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
#if GDX_PERFORMANCETESTING
using Unity.PerformanceTesting;

#endif

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="StringExtensions" /> class.
    /// </summary>
    public class StringExtensionsTests
    {
        private const int WarmupCount = 10;
        private const int MeasurementCount = 20;
        private const int IterationsPerMeasurement = 100;

        private const string ComplexTestString = "_tH\\is_I!is_M\"y_TEST_STR#$34343";
        private const string SimpleTestString = "HelloWorld!";
        private const string UpperCaseTestString = "THIS IS ALL UPPERCASE";
        private const string LowerCaseTestString = "this is all lowercase";


        [Test]
        [Category("GDX.Tests")]
        public void True_GetAfterFirst_Simple()
        {
            string result = ComplexTestString.GetAfterFirst("_M");
            const string expected = "\"y_TEST_STR#$34343";

            Assert.IsTrue(result == expected, $"Expected {expected} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_GetAfterLast_Simple()
        {
            string result = ComplexTestString.GetAfterLast("_");
            const string expected = "STR#$34343";

            Assert.IsTrue(result == expected, $"Expected {expected} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_GetBeforeFirst_Simple()
        {
            string result = ComplexTestString.GetBeforeFirst("_");
            const string expected = "";

            Assert.IsTrue(result == expected, $"Expected {expected} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_GetBeforeLast_Simple()
        {
            string result = ComplexTestString.GetBeforeLast("_");
            const string expected = "_tH\\is_I!is_M\"y_TEST";

            Assert.IsTrue(result == expected, $"Expected {expected} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_HasUpperCase_Simple()
        {
            Assert.IsFalse(LowerCaseTestString.HasUpperCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_HasLowerCase_Simple()
        {
            Assert.IsFalse(UpperCaseTestString.HasLowerCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_HasUpperCase_Simple()
        {
            Assert.IsTrue(SimpleTestString.HasUpperCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_HasLowerCase_Simple()
        {
            Assert.IsTrue(SimpleTestString.HasLowerCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetLowerCaseHashCode_ComplexString()
        {
            int oldHash = ComplexTestString.ToLower().GetHashCode();
            int newHash = ComplexTestString.GetLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetLowerCaseHashCode_LowerCase()
        {
            int oldHash = LowerCaseTestString.ToLower().GetHashCode();
            int newHash = LowerCaseTestString.GetLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetLowerCaseHashCode_Simple()
        {
            int oldHash = SimpleTestString.ToLower().GetHashCode();
            int newHash = SimpleTestString.GetLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetLowerCaseHashCode_UpperCase()
        {
            int oldHash = UpperCaseTestString.ToLower().GetHashCode();
            int newHash = UpperCaseTestString.GetLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetUpperCaseHashCode_ComplexString()
        {
            int oldHash = ComplexTestString.ToUpper().GetHashCode();
            int newHash = ComplexTestString.GetUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetUpperCaseHashCode_LowerCase()
        {
            int oldHash = LowerCaseTestString.ToUpper().GetHashCode();
            int newHash = LowerCaseTestString.GetUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetUpperCaseHashCode_Simple()
        {
            int oldHash = SimpleTestString.ToUpper().GetHashCode();
            int newHash = SimpleTestString.GetUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetUpperCaseHashCode_UpperCase()
        {
            int oldHash = UpperCaseTestString.ToUpper().GetHashCode();
            int newHash = UpperCaseTestString.GetUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

#if GDX_PERFORMANCETESTING
        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_ToLower_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.ToLower().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.ToLower().GetHashCode();
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
        public void Measure_GetLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.GetLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.GetLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.GetLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.GetLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }
#endif

#if GDX_PERFORMANCETESTING
        [Test]
        [Performance]
        [Category("GDX.Performance")]
        public void Measure_ToUpper_GetHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.ToUpper().GetHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.ToUpper().GetHashCode();
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
        public void Measure_GetUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.GetUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.GetUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.GetUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.GetUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Complex")
                .Run();
        }
#endif
    }
}