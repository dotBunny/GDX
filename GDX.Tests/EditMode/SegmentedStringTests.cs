// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SegmentedString" /> class.
    /// </summary>
    public class SegmentedStringTests
    {
        readonly string m_TestContent = "Hello my; favorite .game is NOT happy G";

        [Test]
        [Category(Core.TestCategory)]
        public void AsCharArray_MockData_ReturnsAll()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            char[] data = splitString.AsCharArray();
            Assert.IsTrue(data != null && data.Length == 39);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AsCharArray_MockDataSpecific_ReturnsTarget()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            Assert.IsTrue(splitString.AsCharArray(0)[2] == 'l');
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AsString_MockData_ReturnsLowered()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            string data = splitString.AsString();
            Assert.IsTrue(data != null && data == m_TestContent.ToLower());
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetCount_MockData_Correct()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLower(m_TestContent);
            Assert.IsTrue(splitString.GetCount() == 8);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetHashCode_MockData_Correct()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            int hash = splitString.GetHashCode();
            Assert.IsTrue(hash == -316307395, $"Expected -316307395, but found {hash.ToString()}.");
        }
        [Test]
        [Category(Core.TestCategory)]
        public void GetOffset_MockData_Valid()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            int offset = splitString.GetOffset(1);
            Assert.IsTrue(offset ==6, $"Expected 6, but found {offset.ToString()}.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetSegmentLength_MockData_CorrectCount()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            int length = splitString.GetSegmentLength(1);
            Assert.IsTrue(length ==2, $"Expected 2, but found {length.ToString()}.");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetSegment_MockData_Matches()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            bool evaluate = splitString.AsString(0) == "hello" &&
                            splitString.AsString(1) == "my" &&
                            splitString.AsString(3) == "game" &&
                            splitString.AsString(2) == "favorite" &&
                            splitString.AsString(7) == "g";
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SplitOnNonAlphaNumericToLowerHashed_MockData_ValidHashCodes()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);


            bool evaluate = "Hello".GetStableLowerCaseHashCode() == splitString.GetHashCode(0) &&
                            "favorite".GetStableLowerCaseHashCode() == splitString.GetHashCode(2) &&
                            "G".GetStableLowerCaseHashCode() == splitString.GetHashCode(7);

            Assert.IsTrue(evaluate,
                $"{"Hello".GetStableLowerCaseHashCode().ToString()} == {splitString.GetHashCode(0).ToString()}");
        }
    }
}