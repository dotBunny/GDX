// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using GDX;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ByteExtensions" />
    ///     class.
    /// </summary>
    public class ByteExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void GetStableHashCode_MockData_ReturnsValidCode()
        {
            byte[] testArray = Encoding.UTF8.GetBytes("Hello World");

            bool evaluate = (testArray.GetStableHashCode() == 1349791181);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsSame_SameData_ReturnsTrue()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;

            bool evaluate = mockData.IsSame(StringExtensions.EncryptionDefaultKey);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsSame_DifferentData_ReturnsFalse()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;
            byte[] differentData = StringExtensions.EncryptionInitializationVector;

            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsSame_NullLeftHandSide_ReturnsFalse()
        {
            byte[] mockData = null;
            byte[] differentData = StringExtensions.EncryptionInitializationVector;

            // ReSharper disable once ExpressionIsAlwaysNull
            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsSame_NullRightHandSide_ReturnsFalse()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;
            byte[] differentData = null;

            // ReSharper disable once ExpressionIsAlwaysNull
            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void IsSame_DifferentLengths_ReturnsFalse()
        {
            byte[] mockData = StringExtensions.EncryptionDefaultKey;
            byte[] differentData = new byte[9];

            bool evaluate = mockData.IsSame(differentData);

            Assert.IsFalse(evaluate);
        }
    }
}