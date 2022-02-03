// Copyright (c) 2020-2022 dotBunny Inc.
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
        [Category(GDX.Core.TestCategory)]
        public void Clear_MockData_HasDefaultValues()
        {
            int[] mockArray = { 0, 1, 2, 3 };

            mockArray.Clear();

            bool evaluate = mockArray[0] == default &&
                            mockArray[1] == default &&
                            mockArray[2] == default &&
                            mockArray[3] == default;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void FirstIndexOfItem_MockDataObject_ReturnsIndex()
        {
            object mockSearchObject = new object();
            object[] mockArray = new object[4];
            mockArray[0] = new object();
            mockArray[1] = mockSearchObject;
            mockArray[2] = new object();
            mockArray[3] = mockSearchObject;

            bool evaluate = mockArray.FirstIndexOfItem(mockSearchObject) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void FirstIndexOfItem_NoMockDataObject_ReturnsIndex()
        {
            object mockSearchObject = new object();
            object[] mockArray = new object[4];
            object noObject = new object();
            mockArray[0] = new object();
            mockArray[1] = mockSearchObject;
            mockArray[2] = new object();
            mockArray[3] = mockSearchObject;

            bool evaluate = mockArray.FirstIndexOfItem(noObject) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void FirstIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.FirstIndexOfValue(1) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void FirstIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.FirstIndexOfValue(22) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void LastIndexOfItem_MockDataObject_ReturnsIndex()
        {
            object mockSearchObject = new object();
            object[] mockArray = new object[4];
            mockArray[0] = new object();
            mockArray[1] = mockSearchObject;
            mockArray[2] = new object();
            mockArray[3] = mockSearchObject;

            bool evaluate = mockArray.LastIndexOfItem(mockSearchObject) == 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void LastIndexOfItem_NoMockDataObject_ReturnsNegativeOne()
        {
            object mockSearchBuffer = new object();
            object[] mockArray = new object[4];
            object noObject = new object();
            mockArray[0] = new object();
            mockArray[1] = mockSearchBuffer;
            mockArray[2] = new object();
            mockArray[3] = mockSearchBuffer;

            bool evaluate = mockArray.LastIndexOfItem(noObject) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void LastIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.LastIndexOfValue(1) == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void LastIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.LastIndexOfValue(12) == -1;

            Assert.IsTrue(evaluate);
        }
    }
}