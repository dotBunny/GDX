// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;
using GDX.Collections.Generic;
using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="StringExtensions" /> class.
    /// </summary>
    public class StringExtensionsTests
    {
        byte[] m_PreviousEncryptionDefaultKey;
        byte[] m_PreviousEncryptionInitializationVector;

        [SetUp]
        public void Setup()
        {
            m_PreviousEncryptionDefaultKey = StringExtensions.EncryptionDefaultKey;
            m_PreviousEncryptionInitializationVector = StringExtensions.EncryptionInitializationVector;

            StringExtensions.EncryptionDefaultKey = Encoding.UTF8.GetBytes("Awesome!");
            StringExtensions.EncryptionInitializationVector = Encoding.UTF8.GetBytes("dotBunny");
        }

        [TearDown]
        public void TearDown()
        {
            StringExtensions.EncryptionDefaultKey = m_PreviousEncryptionDefaultKey;
            StringExtensions.EncryptionInitializationVector = m_PreviousEncryptionInitializationVector;
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetAfterFirst_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetAfterFirst("_M");

            bool evaluate = result == "\"y_TEST_STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetAfterLast_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetAfterLast("_");

            bool evaluate = result == "STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetBeforeFirst_MockData_ReturnsEmptyString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetBeforeFirst("_");

            bool evaluate = result == "";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetBeforeLast_MockData_ReturnsString()
        {
            string result = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetBeforeLast("_");

            bool evaluate = result == "_tH\\is_I!is_M\"y_TEST";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasUpperCase_LowerCaseString_ReturnsFalse()
        {
            bool evaluate = "this is all lowercase".HasUpperCase();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasLowerCase_UpperCaseString_ReturnsFalse()
        {
            bool evaluate = "THIS IS ALL UPPERCASE".HasLowerCase();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasUpperCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue("HelloWorld!".HasUpperCase());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void HasLowerCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue("HelloWorld!".HasLowerCase());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsBooleanPositiveValue_TrueString_ReturnsTrue()
        {
            bool evaluate = "true".IsBooleanPositiveValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsBooleanValue_OffString_ReturnsTrue()
        {
            bool evaluate = "off".IsBooleanValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsBooleanValue_BadString_ReturnsFalse()
        {
            bool evaluate = "off2".IsBooleanValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsBooleanPositiveValue_BadString_ReturnsFalse()
        {
            bool evaluate = "true2".IsBooleanPositiveValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsIntegerValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = "".IsIntegerValue();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsIntegerValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsIntegerValue();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsIntegerValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100222".IsIntegerValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsIntegerValue_BadString_ReturnsFalse()
        {
            bool evaluate = "bob".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsIntegerValue_MixedString_ReturnsFalse()
        {
            bool evaluate = "-100bob222".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsNumericValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsNumericValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = "".IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsNumericValue_Characters_ReturnsFalse()
        {
            bool evaluate = "c111".IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsNumericValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100.12123".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsNumericValue_TwoDecimals_ReturnsFalse()
        {
            bool evaluate = "-100..12123".IsNumeric();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SplitCamelCase_CamelCase_ReturnsString()
        {
            bool evaluate = "SomethingSomethingDarkSide".SplitCamelCase() == "Something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SplitCamelCase_camelCase_ReturnsString()
        {
            bool evaluate = "somethingSomethingDarkSide".SplitCamelCase() == "something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Encrypt_Decrypt_IsEqual()
        {
            const string k_SimpleString = "HelloWorld!";
            const string k_ComplexString = "_tH\\is_I!is_M\"y_TEST_STR#$34343";

            string encryptedSimple = k_SimpleString.Encrypt();
            string encryptedComplex = k_ComplexString.Encrypt();

            bool evaluate = encryptedSimple.Decrypt() == k_SimpleString &&
                            encryptedComplex.Decrypt() == k_ComplexString;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Encrypt_MockData_ReturnsString()
        {
            bool evaluate = "HelloWorld!".Encrypt() != "HelloWorld!";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetStableLowerCaseHashCode_ToLowerGetStableHashCode_IsEqual()
        {
            int oldSimpleHash = "HelloWorld!".ToLower().GetStableHashCode();
            int newSimpleHash = "HelloWorld!".GetStableLowerCaseHashCode();
            int oldLowerCaseHash = "this is all lowercase".ToLower().GetStableHashCode();
            int newLowerCaseHash = "this is all lowercase".GetStableLowerCaseHashCode();
            int oldUpperCaseHash = "THIS IS ALL UPPERCASE".ToLower().GetStableHashCode();
            int newUpperCaseHash = "THIS IS ALL UPPERCASE".GetStableLowerCaseHashCode();
            int oldComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToLower().GetStableHashCode();
            int newComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableLowerCaseHashCode();

            bool evaluate = oldSimpleHash == newSimpleHash &&
                            oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetStableUpperCaseHashCode_ToUpperGetStableHashCode_IsEqual()
        {
            int oldComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".ToUpper().GetStableHashCode();
            int newComplexHash = "_tH\\is_I!is_M\"y_TEST_STR#$34343".GetStableUpperCaseHashCode();
            int oldLowerCaseHash = "this is all lowercase".ToUpper().GetStableHashCode();
            int newLowerCaseHash = "this is all lowercase".GetStableUpperCaseHashCode();
            int oldSimpleHash = "HelloWorld!".ToUpper().GetStableHashCode();
            int newSimpleHash = "HelloWorld!".GetStableUpperCaseHashCode();
            int oldUpperCaseHash = "THIS IS ALL UPPERCASE".ToUpper().GetStableHashCode();
            int newUpperCaseHash = "THIS IS ALL UPPERCASE".GetStableUpperCaseHashCode();

            bool evaluate = oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldSimpleHash == newSimpleHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void CountOccurence_TwoMockData_ReturnsTwo()
        {
            string mockData = "this is a, count of sorts,";

            bool evaluate = mockData.CountOccurence(',') == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void CountOccurence_ZeroMockData_ReturnsTwo()
        {
            string mockData = "this is a, count of sorts,";

            bool evaluate = mockData.CountOccurence('!') == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Concatenate_Simple_NoSuffix()
        {
            string[] preString = new[] { "one", "two", "three" };
            string concat = preString.Concatenate(",");
            Assert.IsTrue(concat == "one,two,three", $"Unexpected results: {concat}");
        }
        [Test]
        [Category(Core.TestCategory)]
        public void Concatenate_Simple_Suffix()
        {
            string[] preString = new[] { "one", "two", "three" };
            string concat = preString.Concatenate(",", true);
            Assert.IsTrue(concat == "one,two,three,", $"Unexpected results: {concat}");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PartialMatch_StringArray_Found()
        {
            string[] preString = new[] { "one", "two", "three" };
            Assert.IsTrue(preString.PartialMatch("re"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PartialMatch_StringArray_NotFound()
        {
            string[] preString = new[] { "one", "two", "three" };
            Assert.IsFalse(preString.PartialMatch("happy"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PartialMatch_SimpleList_Found()
        {
            SimpleList<string> preString = new SimpleList<string>(new[] { "one", "two", "three" }) { Count = 3 };
            Assert.IsTrue(preString.PartialMatch("re"));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void PartialMatch_SimpleList_NotFound()
        {
            SimpleList<string> preString = new SimpleList<string>(new[] { "one", "two", "three" }) { Count = 3 };
            Assert.IsFalse(preString.PartialMatch("happy"));
        }
    }
}