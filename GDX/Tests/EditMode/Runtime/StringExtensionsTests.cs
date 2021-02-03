// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
using UnityEngine;
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
        public void True_IsBoolean_True()
        {
            int result = "True".GetStableLowerCaseHashCode();
          //  const string expected = "\"y_TEST_STR#$34343";
          Assert.Fail($"True: {result.ToString()}");
          //Assert.IsTrue(result == expected, $"Expected {expected} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_SplitCamelCase_CamelCase()
        {
            Assert.IsTrue("SomethingSomethingDarkSide".SplitCamelCase() == "Something Something Dark Side");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_SplitCamelCase_camelCase()
        {
            Assert.IsTrue("somethingSomethingDarkSide".SplitCamelCase() == "something Something Dark Side");
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_Encrypt_Decrypt_SimpleString()
        {
            string encrypted = SimpleTestString.Encrypt();
            Assert.IsTrue(encrypted.Decrypt() == SimpleTestString, "Expected strings to match.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_Encrypt_Decrypt_ComplexString()
        {
            string encrypted = ComplexTestString.Encrypt();
            Assert.IsTrue(encrypted.Decrypt() == ComplexTestString, "Expected strings to match.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetStableLowerCaseHashCode_ComplexString()
        {
            int oldHash = ComplexTestString.ToLower().GetHashCode();
            int newHash = ComplexTestString.GetStableLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetStableLowerCaseHashCode_LowerCase()
        {
            int oldHash = LowerCaseTestString.ToLower().GetHashCode();
            int newHash = LowerCaseTestString.GetStableLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetStableLowerCaseHashCode_Simple()
        {
            int oldHash = SimpleTestString.ToLower().GetHashCode();
            int newHash = SimpleTestString.GetStableLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToLowerGetHashCode_GetStableLowerCaseHashCode_UpperCase()
        {
            int oldHash = UpperCaseTestString.ToLower().GetHashCode();
            int newHash = UpperCaseTestString.GetStableLowerCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetStableUpperCaseHashCode_ComplexString()
        {
            int oldHash = ComplexTestString.ToUpper().GetHashCode();
            int newHash = ComplexTestString.GetStableUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetStableUpperCaseHashCode_LowerCase()
        {
            int oldHash = LowerCaseTestString.ToUpper().GetHashCode();
            int newHash = LowerCaseTestString.GetStableUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetStableUpperCaseHashCode_Simple()
        {
            int oldHash = SimpleTestString.ToUpper().GetHashCode();
            int newHash = SimpleTestString.GetStableUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_ToUpperGetHashCode_GetStableUpperCaseHashCode_UpperCase()
        {
            int oldHash = UpperCaseTestString.ToUpper().GetHashCode();
            int newHash = UpperCaseTestString.GetStableUpperCaseHashCode();
            Assert.IsTrue(oldHash == newHash, "Expected string hash codes to match. {0} vs {1}", oldHash, newHash);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_TryParseVector2_UnityFormat()
        {
            string sourceData = "1, 2";
            Vector2 successValue = new Vector2(1, 2);

            if (sourceData.TryParseVector2(out Vector2 parsedLocation))
            {
                Assert.IsTrue(successValue == parsedLocation, $"Expected {successValue}, found {parsedLocation}");
            }
            else
            {
                Assert.Fail($"Unable to parse provided source data ({sourceData}).");
            }
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_TryParseVector2_NoSpaces()
        {
            string sourceData = "1.5,2";
            Vector2 successValue = new Vector2(1.5f, 2);

            if (sourceData.TryParseVector2(out Vector2 parsedLocation))
            {
                Assert.IsTrue(successValue == parsedLocation, $"Expected {successValue}, found {parsedLocation}");
            }
            else
            {
                Assert.Fail($"Unable to parse provided source data ({sourceData}).");
            }
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_TryParseVector3_UnityFormat()
        {
            string sourceData = "3, 2, 1";
            Vector3 successValue = new Vector3(3, 2, 1);

            if (sourceData.TryParseVector3(out Vector3 parsedLocation))
            {
                Assert.IsTrue(successValue == parsedLocation, $"Expected {successValue}, found {parsedLocation}");
            }
            else
            {
                Assert.Fail($"Unable to parse provided source data ({sourceData}).");
            }
        }

        [Test]
        [Category("GDX.Tests")]
        public void Equal_TryParseVector3_NoSpaces()
        {
            string sourceData = "3,2,1";
            Vector3 successValue = new Vector3(3, 2, 1);

            if (sourceData.TryParseVector3(out Vector3 parsedLocation))
            {
                Assert.IsTrue(successValue == parsedLocation, $"Expected {successValue}, found {parsedLocation}");
            }
            else
            {
                Assert.Fail($"Unable to parse provided source data ({sourceData}).");
            }
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
        public void Measure_GetStableLowerCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.GetStableLowerCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.GetStableLowerCaseHashCode();
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
        public void Measure_GetStableUpperCaseHashCode()
        {
            Measure.Method(() =>
                {
                    int dummyValue = SimpleTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("Simple")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = LowerCaseTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("LowerCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = UpperCaseTestString.GetStableUpperCaseHashCode();
                })
                .WarmupCount(WarmupCount)
                .MeasurementCount(MeasurementCount)
                .IterationsPerMeasurement(IterationsPerMeasurement)
                .SampleGroup("UpperCase")
                .Run();
            Measure.Method(() =>
                {
                    int dummyValue = ComplexTestString.GetStableUpperCaseHashCode();
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