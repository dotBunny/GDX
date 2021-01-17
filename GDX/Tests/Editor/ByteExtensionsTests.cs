// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Tests.Editor
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
        public void True_GetValueHashCode_Simple()
        {
            byte[] testArray = Encoding.UTF8.GetBytes("Hello World");
            int hashCode = testArray.GetValueHashCode();
            Assert.IsTrue(hashCode == 1349791181, $"Expected value of 1349791181 instead got {hashCode}.");
        }
    }
}