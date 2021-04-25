// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of arrays.
    ///     class.
    /// </summary>
    public class ArrayExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Clear_MockData_HasDefaultValues()
        {
            int[] mockArray = {0, 1, 2, 3};

            mockArray.Clear();

            bool evaluate = mockArray[0] == default &&
                            mockArray[1] == default &&
                            mockArray[2] == default &&
                            mockArray[3] == default;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void FirstIndexOfItem_MockDataObject_ReturnsIndex()
        {
            CircularBuffer<int> mockSearchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] mockArray = new CircularBuffer<int>[4];
            mockArray[0] = new CircularBuffer<int>(1);
            mockArray[1] = mockSearchBuffer;
            mockArray[2] = new CircularBuffer<int>(2);
            mockArray[3] = mockSearchBuffer;

            bool evaluate = mockArray.FirstIndexOfItem(mockSearchBuffer) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void FirstIndexOfItem_NoMockDataObject_ReturnsIndex()
        {
            CircularBuffer<int> mockSearchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] mockArray = new CircularBuffer<int>[4];
            CircularBuffer<int> noObject = new CircularBuffer<int>(1, new[] {0});
            mockArray[0] = new CircularBuffer<int>(1);
            mockArray[1] = mockSearchBuffer;
            mockArray[2] = new CircularBuffer<int>(2);
            mockArray[3] = mockSearchBuffer;

            bool evaluate = mockArray.FirstIndexOfItem(noObject) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void FirstIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};

            bool evaluate = mockArray.FirstIndexOfValue(1) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void FirstIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};

            bool evaluate = mockArray.FirstIndexOfValue(22) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void LastIndexOfItem_MockDataObject_ReturnsIndex()
        {
            CircularBuffer<int> mockSearchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] mockArray = new CircularBuffer<int>[4];
            mockArray[0] = new CircularBuffer<int>(1);
            mockArray[1] = mockSearchBuffer;
            mockArray[2] = new CircularBuffer<int>(2);
            mockArray[3] = mockSearchBuffer;

            bool evaluate = mockArray.LastIndexOfItem(mockSearchBuffer) == 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void LastIndexOfItem_NoMockDataObject_ReturnsNegativeOne()
        {
            CircularBuffer<int> mockSearchBuffer = new CircularBuffer<int>(2, new[] {0, 1});
            CircularBuffer<int>[] mockArray = new CircularBuffer<int>[4];
            CircularBuffer<int> noObject = new CircularBuffer<int>(1, new[] {0});
            mockArray[0] = new CircularBuffer<int>(1);
            mockArray[1] = mockSearchBuffer;
            mockArray[2] = new CircularBuffer<int>(2);
            mockArray[3] = mockSearchBuffer;

            bool evaluate = mockArray.LastIndexOfItem(noObject) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void LastIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};

            bool evaluate = mockArray.LastIndexOfValue(1) == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void LastIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2};

            bool evaluate = mockArray.LastIndexOfValue(12) == -1;

            Assert.IsTrue(evaluate);
        }
    }
}