﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;
using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ByteExtensions" />
    ///     class.
    /// </summary>
    public class ByteExtensionsTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetStableHashCode_MockData_ReturnsValidCode()
        {
            byte[] testArray = Encoding.UTF8.GetBytes(TestLiterals.HelloWorld);

            bool evaluate = testArray.GetStableHashCode() == 1349791181;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSame_SameData_ReturnsTrue()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;

            bool evaluate = mockData.IsSame(StringExtensions.EncryptionDefaultKey);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSame_DifferentData_ReturnsFalse()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;
            byte[] differentData = StringExtensions.EncryptionInitializationVector;

            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSame_NullLeftHandSide_ReturnsFalse()
        {
            bool evaluate = ByteExtensions.IsSame(null,
                StringExtensions.EncryptionInitializationVector);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSame_NullRightHandSide_ReturnsFalse()
        {
            bool evaluate = StringExtensions.EncryptionDefaultKey.IsSame(null);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void IsSame_DifferentLengths_ReturnsFalse()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;
            byte[] differentData = new byte[9];

            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }
    }
}