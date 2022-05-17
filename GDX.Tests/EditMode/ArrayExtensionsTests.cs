// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of arrays.
    /// </summary>
    public class ArrayExtensionsTests
    {
        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
        public void ContainsItem_MockData_HasValue()
        {

            string[] mockArray = { "hello", "jello", "baby", "Init" };

            bool evaluate = mockArray.ContainsItem(Core.OverrideMethod);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsItem_MockData_NoValue()
        {
            string[] mockArray = { "hello", "jello", "baby" };

            bool evaluate = mockArray.ContainsItem("bob");

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsReference_MockData_HasReference()
        {
            string checkObject = "object";

            string[] mockArray = { "hello", "jello", "baby", checkObject };

            bool evaluate = mockArray.ContainsReference(checkObject);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsReference_MockData_NoReference()
        {
            string checkObject = "object";

            string[] mockArray = { "hello", "jello", "baby" };

            bool evaluate = mockArray.ContainsReference(checkObject);

            Assert.IsFalse(evaluate);
        }


        [Test]
        [Category(Core.TestCategory)]
        public void ContainsValue_MockData_HasValue()
        {
            int[] mockArray = { 0, 1, 2, 3 };

            bool evaluate = mockArray.ContainsValue(3);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsValue_MockData_NoValue()
        {
            int[] mockArray = { 0, 1, 2, 3 };

            bool evaluate = mockArray.ContainsValue(5);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
        public void FirstIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.FirstIndexOfValue(1) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void FirstIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.FirstIndexOfValue(22) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
        public void LastIndexOfValue_MockDataValue_ReturnsIndex()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.LastIndexOfValue(1) == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void LastIndexOfValue_NoMockDataValue_ReturnsNegativeOne()
        {
            int[] mockArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2 };

            bool evaluate = mockArray.LastIndexOfValue(12) == -1;

            Assert.IsTrue(evaluate);
        }
    }
}