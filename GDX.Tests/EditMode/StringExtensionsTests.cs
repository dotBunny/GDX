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
        public const string AllLowerCase = "this is all lowercase";
        public const string AllUpperCase = "THIS IS ALL UPPERCASE";
        public const string Complex = "_tH\\is_I!is_M\"y_TEST_STR#$34343";

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
        [Category(Literals.TestCategory)]
        public void GetAfterFirst_MockData_ReturnsString()
        {
            string result = StringExtensions.GetAfterFirst(Complex, "_M");

            bool evaluate = result == "\"y_TEST_STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetAfterLast_MockData_ReturnsString()
        {
            string result = StringExtensions.GetAfterLast(Complex, "_");

            bool evaluate = result == "STR#$34343";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetBeforeFirst_MockData_ReturnsEmptyString()
        {
            string result = StringExtensions.GetBeforeFirst(Complex, "_");

            bool evaluate = result == string.Empty;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetBeforeLast_MockData_ReturnsString()
        {
            string result = StringExtensions.GetBeforeLast(Complex, "_");

            bool evaluate = result == "_tH\\is_I!is_M\"y_TEST";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void HasUpperCase_LowerCaseString_ReturnsFalse()
        {
            Assert.IsFalse(StringExtensions.HasUpperCase(AllLowerCase));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void HasLowerCase_UpperCaseString_ReturnsFalse()
        {
            Assert.IsFalse(StringExtensions.HasLowerCase(AllUpperCase));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void HasUpperCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue(TestLiterals.HelloWorld.HasUpperCase());
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void HasLowerCase_MixedCaseString_ReturnsTrue()
        {
            Assert.IsTrue(TestLiterals.HelloWorld.HasLowerCase());
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsBooleanPositiveValue_TrueString_ReturnsTrue()
        {
            bool evaluate = TestLiterals.True.IsBooleanPositiveValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsBooleanValue_OffString_ReturnsTrue()
        {
            bool evaluate = TestLiterals.Off.IsBooleanValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsBooleanValue_BadString_ReturnsFalse()
        {
            bool evaluate = "off2".IsBooleanValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsBooleanPositiveValue_BadString_ReturnsFalse()
        {
            bool evaluate = "true2".IsBooleanPositiveValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsIntegerValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = string.Empty.IsIntegerValue();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsIntegerValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsIntegerValue();
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsIntegerValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100222".IsIntegerValue();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsIntegerValue_BadString_ReturnsFalse()
        {
            bool evaluate = "bob".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsIntegerValue_MixedString_ReturnsFalse()
        {
            bool evaluate = "-100bob222".IsIntegerValue();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsNumericValue_PositiveNumber_ReturnsTrue()
        {
            bool evaluate = "1".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsNumericValue_NullNumber_ReturnsFalse()
        {
            bool evaluate = string.Empty.IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsNumericValue_Characters_ReturnsFalse()
        {
            bool evaluate = "c111".IsNumeric();
            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsNumericValue_NegativeNumber_ReturnsTrue()
        {
            bool evaluate = "-100.12123".IsNumeric();

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void IsNumericValue_TwoDecimals_ReturnsFalse()
        {
            bool evaluate = "-100..12123".IsNumeric();

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SplitCamelCase_CamelCase_ReturnsString()
        {
            bool evaluate = "SomethingSomethingDarkSide".SplitCamelCase() == "Something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void SplitCamelCase_camelCase_ReturnsString()
        {
            bool evaluate = "somethingSomethingDarkSide".SplitCamelCase() == "something Something Dark Side";

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void Encrypt_Decrypt_IsEqual()
        {
            string encryptedSimple = TestLiterals.HelloWorld.Encrypt();
            string encryptedComplex = StringExtensions.Encrypt(Complex);

            bool evaluate = encryptedSimple.Decrypt() == TestLiterals.HelloWorld &&
                            encryptedComplex.Decrypt() == Complex;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void Encrypt_MockData_ReturnsString()
        {
            Assert.IsTrue(TestLiterals.HelloWorld.Encrypt() != TestLiterals.HelloWorld);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetStableLowerCaseHashCode_ToLowerGetStableHashCode_IsEqual()
        {
            int oldSimpleHash = TestLiterals.HelloWorld.ToLower().GetStableHashCode();
            int newSimpleHash = TestLiterals.HelloWorld.GetStableLowerCaseHashCode();
            int oldLowerCaseHash = StringExtensions.GetStableHashCode(AllLowerCase.ToLower());
            int newLowerCaseHash = StringExtensions.GetStableLowerCaseHashCode(AllLowerCase);
            int oldUpperCaseHash = StringExtensions.GetStableHashCode(AllUpperCase.ToLower());
            int newUpperCaseHash = StringExtensions.GetStableLowerCaseHashCode(AllUpperCase);
            int oldComplexHash = StringExtensions.GetStableHashCode(Complex.ToLower());
            int newComplexHash = StringExtensions.GetStableLowerCaseHashCode(Complex);

            bool evaluate = oldSimpleHash == newSimpleHash &&
                            oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void GetStableUpperCaseHashCode_ToUpperGetStableHashCode_IsEqual()
        {
            int oldComplexHash = StringExtensions.GetStableHashCode(Complex.ToUpper());
            int newComplexHash = StringExtensions.GetStableUpperCaseHashCode(Complex);
            int oldLowerCaseHash = StringExtensions.GetStableHashCode(AllLowerCase.ToUpper());
            int newLowerCaseHash = StringExtensions.GetStableUpperCaseHashCode(AllLowerCase);
            int oldSimpleHash = TestLiterals.HelloWorld.ToUpper().GetStableHashCode();
            int newSimpleHash = TestLiterals.HelloWorld.GetStableUpperCaseHashCode();
            int oldUpperCaseHash = StringExtensions.GetStableHashCode(AllUpperCase.ToUpper());
            int newUpperCaseHash = StringExtensions.GetStableUpperCaseHashCode(AllUpperCase);

            bool evaluate = oldComplexHash == newComplexHash &&
                            oldLowerCaseHash == newLowerCaseHash &&
                            oldSimpleHash == newSimpleHash &&
                            oldUpperCaseHash == newUpperCaseHash;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void CountOccurence_TwoMockData_ReturnsTwo()
        {
            string mockData = "this is a, count of sorts,";

            bool evaluate = mockData.CountOccurence(',') == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void CountOccurence_ZeroMockData_ReturnsTwo()
        {
            string mockData = "this is a, count of sorts,";

            bool evaluate = mockData.CountOccurence('!') == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void Concatenate_Simple_NoSuffix()
        {
            string[] preString = new[] { "one", "two", "three" };
            string concat = preString.Concatenate(",");
            Assert.IsTrue(concat == "one,two,three");
        }
        [Test]
        [Category(Literals.TestCategory)]
        public void Concatenate_Simple_Suffix()
        {
            string[] preString = new[] { "one", "two", "three" };
            string concat = preString.Concatenate(",", true);
            Assert.IsTrue(concat == "one,two,three,");
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_StringArray_Found()
        {
            string[] preString = new[] { "one", "two", "three" };
            Assert.IsTrue(preString.PartialMatch("re"));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_StringArray_NotFound()
        {
            string[] preString = new[] { "one", "two", "three" };
            Assert.IsFalse(preString.PartialMatch("happy"));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_NullStringArray_NotFound()
        {
            Assert.IsFalse(StringExtensions.PartialMatch(null, "happy"));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_SimpleList_Found()
        {
            SimpleList<string> preString = new SimpleList<string>(new[] { "one", "two", "three" }) { Count = 3 };
            Assert.IsTrue(preString.PartialMatch("re"));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_SimpleList_NotFound()
        {
            SimpleList<string> preString = new SimpleList<string>(new[] { "one", "two", "three" }) { Count = 3 };
            Assert.IsFalse(preString.PartialMatch("happy"));
        }

        [Test]
        [Category(Literals.TestCategory)]
        public void PartialMatch_EmptySimpleList_NotFound()
        {
            SimpleList<string> preString = new SimpleList<string>();
            Assert.IsFalse(preString.PartialMatch("happy"));
        }
    }
}