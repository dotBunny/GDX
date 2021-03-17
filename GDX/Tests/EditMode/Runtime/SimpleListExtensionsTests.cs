// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using GDX;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SimpleListExtensions" />
    ///     class.
    /// </summary>
    public class SimpleListExtensionsTests
    {
        private SimpleList<int> _mockData;

        [SetUp]
        public void Setup()
        {
            _mockData = new SimpleList<int>(5);
            _mockData.AddUnchecked(0);
            _mockData.AddUnchecked(1);
            _mockData.AddUnchecked(2);
            _mockData.AddUnchecked(3);
            _mockData.AddUnchecked(4);
        }

        [TearDown]
        public void TearDown()
        {
            _mockData.Clear();
            _mockData = default;
        }


        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheckUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem("test1");
            mockData.AddWithExpandCheckUniqueItem("test2");
            mockData.AddWithExpandCheckUniqueItem("test3");

            bool evaluate = mockData.AddWithExpandCheckUniqueItem("test1");

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheckUniqueItem_UniqueString_ReturnsTrue()
        {
            SimpleList<string> mockData = new SimpleList<string>(3);

            mockData.AddWithExpandCheckUniqueItem("test1");
            mockData.AddWithExpandCheckUniqueItem("test2");
            mockData.AddWithExpandCheckUniqueItem("test3");

            bool evaluate = mockData.AddWithExpandCheckUniqueItem("test4");

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheckUniqueValue_NonUniqueInteger_ReturnsFalse()
        {
            SimpleList<int> listOfIntegers = new SimpleList<int>(3);
            listOfIntegers.AddWithExpandCheckUniqueValue(1);
            listOfIntegers.AddWithExpandCheckUniqueValue(2);
            listOfIntegers.AddWithExpandCheckUniqueValue(3);

            bool evaluate = listOfIntegers.AddWithExpandCheckUniqueValue(1);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheckUniqueValue_UniqueInteger_ReturnsTrue()
        {
            SimpleList<int> listOfIntegers = new SimpleList<int>(3);
            listOfIntegers.AddWithExpandCheckUniqueValue(1);
            listOfIntegers.AddWithExpandCheckUniqueValue(2);
            listOfIntegers.AddWithExpandCheckUniqueValue(3);

            bool evaluate = listOfIntegers.AddWithExpandCheckUniqueValue(4);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueItem_NonUniqueString_ReturnsFalse()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            bool evaluate = listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueItem_UniqueStringWithRoom_NoException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.DoesNotThrow(() => { listOfStrings.AddUncheckedUniqueItem("test3"); });
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueItem_UniqueStringWithNoRoom_ThrowsException()
        {
            SimpleList<string> listOfStrings = new SimpleList<string>(2);
            listOfStrings.AddUncheckedUniqueItem("test1");
            listOfStrings.AddUncheckedUniqueItem("test2");

            Assert.Throws<IndexOutOfRangeException>(() => { listOfStrings.AddUncheckedUniqueItem("test3"); });
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueValue_NonUniqueInteger_ReturnsFalse()
        {
            SimpleList<int> listOfIntegers = new SimpleList<int>(3);
            listOfIntegers.AddUncheckedUniqueValue(1);
            listOfIntegers.AddUncheckedUniqueValue(2);

            bool evaluate = listOfIntegers.AddUncheckedUniqueValue(2);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueValue_UniqueIntegerWithRoom_NoException()
        {
            SimpleList<int> listOfIntegers = new SimpleList<int>(3);
            listOfIntegers.AddUncheckedUniqueValue(1);
            listOfIntegers.AddUncheckedUniqueValue(2);

            Assert.DoesNotThrow(() => { listOfIntegers.AddUncheckedUniqueValue(3); });
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUncheckedUniqueValue_UniqueIntegerWithNoRoom_ThrowsException()
        {
            SimpleList<int> listOfIntegers = new SimpleList<int>(2);
            listOfIntegers.AddUncheckedUniqueValue(1);
            listOfIntegers.AddUncheckedUniqueValue(2);

            Assert.Throws<IndexOutOfRangeException>(() => { listOfIntegers.AddUncheckedUniqueValue(3); });
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsItem_String_ReturnsTrue()
        {
            const string searchItem = "Hello";
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUnchecked(searchItem);
            listOfStrings.AddUnchecked("World");
            listOfStrings.AddUnchecked("!");

            bool evaluate = listOfStrings.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsItem_CircularBuffer_ReturnsTrue()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listItems = new SimpleList<CircularBuffer<int>>(5);

            // Build test rig
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(new CircularBuffer<int>(5));

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsValue_FirstValue_ReturnsTrue()
        {
            const int searchValue = 0;

            bool evaluate = _mockData.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsValue_LastValue_ReturnsTrue()
        {
            const int searchValue = 4;

            bool evaluate = _mockData.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsValue_MidValue_ReturnsTrue()
        {
            const int searchValue = 1;

            bool evaluate = _mockData.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsValue_MissingValue_ReturnsFalse()
        {
            const int searchValue = 12;

            bool evaluate = _mockData.ContainsValue(searchValue);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveFirstItem_MockData_RemovedItem()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listItems = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));

            listItems.RemoveFirstItem(searchItem);

            bool evaluate = listItems.Array[1] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveFirstValue_MockData_RemovedValue()
        {
            const int searchValue = 1;
            SimpleList<int> listValues = new SimpleList<int>(6);
            listValues.AddUnchecked(0);
            listValues.AddUnchecked(searchValue);
            listValues.AddUnchecked(2);
            listValues.AddUnchecked(3);
            listValues.AddUnchecked(4);
            listValues.AddUnchecked(searchValue);

            listValues.RemoveFirstValue(searchValue);

            bool evaluate = listValues.Array[1] == 2 && listValues.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveItems_MockData_RemovedItems()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listItems = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));

            listItems.RemoveItems(searchItem);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveLastItem_MockData_RemovedItem()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listItems = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(new CircularBuffer<int>(5));
            listItems.AddUnchecked(searchItem);
            listItems.AddUnchecked(new CircularBuffer<int>(5));

            listItems.RemoveLastItem(searchItem);

            bool evaluate = listItems.Array[4] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveLastValue_MockData_RemovedValue()
        {
            SimpleList<int> listValues = new SimpleList<int>(7);
            listValues.AddUnchecked(0);
            listValues.AddUnchecked(1);
            listValues.AddUnchecked(2);
            listValues.AddUnchecked(3);
            listValues.AddUnchecked(4);
            listValues.AddUnchecked(1);
            listValues.AddUnchecked(2);

            listValues.RemoveLastValue(1);

            bool evaluate = listValues.Array[5] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveValues_MockData_RemovedValue()
        {
            SimpleList<int> listValues = new SimpleList<int>(7);
            listValues.AddUnchecked(0);
            listValues.AddUnchecked(1);
            listValues.AddUnchecked(2);
            listValues.AddUnchecked(3);
            listValues.AddUnchecked(4);
            listValues.AddUnchecked(1);
            listValues.AddUnchecked(2);

            listValues.RemoveValues(1);

            bool evaluate = listValues.ContainsValue(1);

            Assert.IsFalse(evaluate);
        }
    }
}