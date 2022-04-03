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
        public void GetSegment_MockData_Matches()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);
            bool evaluate = splitString.GetSegment(0) == "hello" &&
                            splitString.GetSegment(1) == "my" &&
                            splitString.GetSegment(3) == "game" &&
                            splitString.GetSegment(2) == "favorite" &&
                            splitString.GetSegment(7) == "g";
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SplitOnNonAlphaNumericToLowerHashed_MockData_ValidHashCodes()
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(m_TestContent);


            bool evaluate = "Hello".GetStableLowerCaseHashCode() == splitString.GetSegmentHashCode(0) &&
                            "favorite".GetStableLowerCaseHashCode() == splitString.GetSegmentHashCode(2) &&
                            "G".GetStableLowerCaseHashCode() == splitString.GetSegmentHashCode(7);

            Assert.IsTrue(evaluate, $"{"Hello".GetStableLowerCaseHashCode()} == {splitString.GetSegmentHashCode(0)}");
        }
    }
}