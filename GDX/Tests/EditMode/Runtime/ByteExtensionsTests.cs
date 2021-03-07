// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        /// <summary>
        ///     Check if we get the correct hash code from an array of <see cref="byte" />s.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void GetStableHashCode_MockData_ReturnsValidCode()
        {
            byte[] testArray = Encoding.UTF8.GetBytes("Hello World");

            bool evaluate = (testArray.GetStableHashCode() == 1349791181);

            Assert.IsTrue(evaluate);
        }
    }
}