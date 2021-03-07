// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="StringExtensions" /> class.
    /// </summary>
    public class StringExtensionsTests
    {
        public const string ComplexTestString = "_tH\\is_I!is_M\"y_TEST_STR#$34343";
        public const string SimpleTestString = "HelloWorld!";
        public const string UpperCaseTestString = "THIS IS ALL UPPERCASE";
        public const string LowerCaseTestString = "this is all lowercase";

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
        public void True_IsBooleanPositiveValue_Simple()
        {
            Assert.IsTrue("true".IsBooleanPositiveValue(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_IsBooleanValue_Simple()
        {
            Assert.IsTrue("off".IsBooleanValue(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_IsBooleanValue_Simple()
        {
            Assert.IsFalse("off2".IsBooleanValue(), "Expected negative response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_IsBooleanPositiveValue_Simple()
        {
            Assert.IsFalse("true2".IsBooleanPositiveValue(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_IsIntegerValue_Simple()
        {
            Assert.IsTrue("1".IsIntegerValue(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_IsIntegerValue_Complex()
        {
            Assert.IsTrue("-100222".IsIntegerValue(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_IsIntegerValue_Simple()
        {
            Assert.IsFalse("bob".IsIntegerValue(), "Expected false response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_IsIntegerValue_Complex()
        {
            Assert.IsFalse("-100bob222".IsIntegerValue(), "Expected false response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_IsNumericValue_Simple()
        {
            Assert.IsTrue("1".IsNumeric(), "Expected positive response.");
        }
        [Test]
        [Category("GDX.Tests")]
        public void True_IsNumericValue_Complex()
        {
            Assert.IsTrue("-100.12123".IsNumeric(), "Expected positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_IsNumericValue_DoubleDecimal()
        {
            Assert.IsFalse("-100..12123".IsNumeric(), "Expected positive response.");
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

    }
}