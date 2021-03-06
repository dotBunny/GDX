﻿// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
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
        private byte[] _previousEncryptionDefaultKey;
        private byte[] _previousEncryptionInitializationVector;

        [SetUp]
        public void Setup()
        {
            _previousEncryptionDefaultKey = StringExtensions.EncryptionDefaultKey;
            _previousEncryptionInitializationVector = StringExtensions.EncryptionInitializationVector;

            StringExtensions.EncryptionDefaultKey = Encoding.UTF8.GetBytes("Awesome!");
            StringExtensions.EncryptionInitializationVector = Encoding.UTF8.GetBytes("dotBunny");
        }

        [TearDown]
        public void TearDown()
        {
            StringExtensions.EncryptionDefaultKey = _previousEncryptionDefaultKey;
            StringExtensions.EncryptionInitializationVector = _previousEncryptionInitializationVector;
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetAfterFirst_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetAfterFirst("_M");

            bool evaluate = result == "\"y_TEST_STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetAfterLast_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetAfterLast("_");

            bool evaluate = result == "STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetBeforeFirst_MockData_ReturnsEmptyString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetBeforeFirst("_");

            bool evaluate = result == "";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetBeforeLast_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetBeforeLast("_");

            bool evaluate = result == "_tH\\is_I!is_M\"y_TEST";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void HasUpperCase_LowerCaseString_ReturnsFalse()
        {
            bool evaluate = "this is all lowercase".HasUpperCase();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void HasLowerCase_UpperCaseString_ReturnsFalse()
        {
            bool evaluate = "THIS IS ALL UPPERCASE".HasLowerCase();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void HasUpperCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue("HelloWorld!".HasUpperCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void HasLowerCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue("HelloWorld!".HasLowerCase());
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsBooleanPositiveValue_TrueString_ReturnsTrue()
        {
            bool evaluate = "true".IsBooleanPositiveValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsBooleanValue_OffString_ReturnsTrue()
        {
            bool evaluate = "off".IsBooleanValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsBooleanValue_BadString_ReturnsFalse()
        {
            bool evaluate = "off2".IsBooleanValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsBooleanPositiveValue_BadString_ReturnsFalse()
        {
            bool evaluate = "true2".IsBooleanPositiveValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsIntegerValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = "".IsIntegerValue();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsIntegerValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsIntegerValue();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsIntegerValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100222".IsIntegerValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsIntegerValue_BadString_ReturnsFalse()
        {
            bool evaluate = "bob".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsIntegerValue_MixedString_ReturnsFalse()
        {
            bool evaluate = "-100bob222".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsNumericValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsNumericValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = "".IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsNumericValue_Characters_ReturnsFalse()
        {
            bool evaluate = "c111".IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsNumericValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100.12123".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsNumericValue_TwoDecimals_ReturnsFalse()
        {
            bool evaluate = "-100..12123".IsNumeric();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void SplitCamelCase_CamelCase_ReturnsString()
        {
            bool evaluate = "SomethingSomethingDarkSide".SplitCamelCase() == "Something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void SplitCamelCase_camelCase_ReturnsString()
        {
            bool evaluate = "somethingSomethingDarkSide".SplitCamelCase() == "something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Encrypt_Decrypt_IsEqual()
        {
            const string simpleString = "HelloWorld!";
            const string complexString = "_tH\\is_I!is_M\"y_TEST_STR#$34343";

            string encryptedSimple = simpleString.Encrypt();
            string encryptedComplex = complexString.Encrypt();

            bool evaluate = encryptedSimple.Decrypt() == simpleString &&
                            encryptedComplex.Decrypt() == complexString;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Encrypt_MockData_ReturnsString()
        {
            bool evaluate = "HelloWorld!".Encrypt() != "HelloWorld!";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetStableLowerCaseHashCode_ToLowerGetHashCode_IsEqual()
        {
            int oldSimpleHash = "HelloWorld!".ToLower().GetHashCode();
            int newSimpleHash = "HelloWorld!".GetStableLowerCaseHashCode();
            int oldLowerCaseHash = "this is all lowercase".ToLower().GetHashCode();
            int newLowerCaseHash = "this is all lowercase".GetStableLowerCaseHashCode();
            int oldUpperCaseHash = "THIS IS ALL UPPERCASE".ToLower().GetHashCode();
            int newUpperCaseHash = "THIS IS ALL UPPERCASE".GetStableLowerCaseHashCode();
            int oldComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToLower().GetHashCode();
            int newComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableLowerCaseHashCode();

            bool evaluate = oldSimpleHash == newSimpleHash &&
                            oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetStableUpperCaseHashCode_ToUpperGetHashCode_IsEqual()
        {
            int oldComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToUpper().GetHashCode();
            int newComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableUpperCaseHashCode();
            int oldLowerCaseHash = "this is all lowercase".ToUpper().GetHashCode();
            int newLowerCaseHash = "this is all lowercase".GetStableUpperCaseHashCode();
            int oldSimpleHash = "HelloWorld!".ToUpper().GetHashCode();
            int newSimpleHash = "HelloWorld!".GetStableUpperCaseHashCode();
            int oldUpperCaseHash = "THIS IS ALL UPPERCASE".ToUpper().GetHashCode();
            int newUpperCaseHash = "THIS IS ALL UPPERCASE".GetStableUpperCaseHashCode();

            bool evaluate = oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldSimpleHash == newSimpleHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector2_NoSplit_ReturnsFalse()
        {
            bool parse = "12".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector2_NoSpaces_ReturnsVector2()
        {
            bool parse = "1.5,2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector2(1.5f, 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector2_SpacedValues_ReturnsVector2()
        {
            bool parse = "1, 2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector2(1, 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector2_CharFirstSplit_ReturnsFalse()
        {
            bool parse = "c, 2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector2_CharSecondSplit_ReturnsFalse()
        {
            bool parse = "1, c".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_NoSplit_ReturnsVector2Zero()
        {
            bool parse = "123".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_NoSecondSplit_ReturnsFalse()
        {
            bool parse = "2, 22".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_CharFirstSplit_ReturnsFalse()
        {
            bool parse = "c, 2, 2".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_CharSecondSplit_ReturnsFalse()
        {
            bool parse = "2, c, 2".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_CharThirdSplit_ReturnsFalse()
        {
            bool parse = "2, 2, c".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_NoSpaces_ReturnsVector3()
        {
            bool parse = "3,2,1".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector3(3, 2, 1);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void TryParseVector3_SpacedValues_ReturnsVector3()
        {
            bool parse = "3, 2, 1".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector3(3, 2, 1);

            Assert.IsTrue(evaluate);
        }
    }
}