// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Tests.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of arrays.
    ///     class.
    /// </summary>
    public class ArrayExtensionsTests
    {
        /// <summary>
        ///     Check if the <see cref="ArrayExtensions.Clear{T}" /> method correctly defaults all elements in an array.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_Clear_Simple()
        {
            int[] clearArray = {0, 1, 2, 3};
            clearArray.Clear();

            Assert.IsTrue(clearArray[0] == default &&
                          clearArray[1] == default &&
                          clearArray[2] == default &&
                          clearArray[3] == default, "Expected all elements in the array to be default.");
        }

        /// <summary>
        ///     Check if the <see cref="ArrayExtensions.FirstIndexOfItem{T}" /> method correctly finds the first index.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_FirstIndexOfItem_Simple()
        {
            CircularBuffer<int> searchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] testArray = new CircularBuffer<int>[4];
            testArray[0] = new CircularBuffer<int>(1);
            testArray[1] = searchBuffer;
            testArray[2] = new CircularBuffer<int>(2);
            testArray[3] = searchBuffer;
            Assert.IsTrue(testArray.FirstIndexOfItem(searchBuffer) == 1, "Expected an index of 1.");
        }

        /// <summary>
        ///     Check if the <see cref="ArrayExtensions.FirstIndexOfValue{T}" /> method correctly finds the first index.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_FirstIndexOfValue_Simple()
        {
            int[] testArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};
            Assert.IsTrue(testArray.FirstIndexOfValue(1) == 1, "Expected an index of 1.");
        }

        /// <summary>
        ///     Check if the <see cref="ArrayExtensions.LastIndexOfItem{T}" /> method correctly finds the last index.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_LastIndexOfItem_Simple()
        {
            CircularBuffer<int> searchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] testArray = new CircularBuffer<int>[4];
            testArray[0] = new CircularBuffer<int>(1);
            testArray[1] = searchBuffer;
            testArray[2] = new CircularBuffer<int>(2);
            testArray[3] = searchBuffer;

            Assert.IsTrue(testArray.LastIndexOfItem(searchBuffer) == 3, "Expected an index of 3.");
        }

        /// <summary>
        ///     Check if the <see cref="ArrayExtensions.LastIndexOfValue{T}" /> method correctly finds the last index.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_LastIndexOfValue_Simple()
        {
            int[] testArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};
            Assert.IsTrue(testArray.LastIndexOfValue(1) == 11, "Expected an index of 11.");
        }
    }
}