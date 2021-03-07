// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="IListExtensions" />
    ///     class.
    /// </summary>
    public class ListExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void AddUniqueItem_UniqueItem_ReturnsTrue()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listValues = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), new CircularBuffer<int>(4), new CircularBuffer<int>(15)
            };

            bool evaluate = listValues.AddUniqueItem(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUniqueItem_NonUniqueItem_ReturnsFalse()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listValues = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), new CircularBuffer<int>(4), searchValue, new CircularBuffer<int>(15)
            };

            bool evaluate = listValues.AddUniqueItem(searchValue);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUniqueValue_UniqueValue_ReturnsTrue()
        {
            const int searchValue = 1;
            List<int> listValues = new List<int> {0, 2, 3};

            bool evaluate = listValues.AddUniqueValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUniqueValue_NonUniqueValue_ReturnsFalse()
        {
            const int searchValue = 1;
            List<int> listValues = new List<int> {0, 2, 3};

            listValues.AddUniqueValue(searchValue);
            bool evaluate = listValues.AddUniqueValue(searchValue);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsItem_MockData_ReturnsIndex()
        {
            CircularBuffer<int> mockObject = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> mockData = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2),
                mockObject,
                new CircularBuffer<int>(4),
                new CircularBuffer<int>(15),
                mockObject
            };

            bool evaluate = mockData.ContainsItem(mockObject);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ContainsValue_MockData_ReturnsIndex()
        {
            const int searchValue = 1;
            List<int> mockValues = new List<int>
            {
                0,
                searchValue,
                2,
                3,
                searchValue
            };

            bool evaluate = mockValues.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveFirstItem_MockData_RemovedItem()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listItems = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2),
                searchItem,
                new CircularBuffer<int>(4),
                new CircularBuffer<int>(15),
                searchItem
            };

            listItems.RemoveFirstItem(searchItem);

            bool evaluate = listItems[1] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveFirstValue_MockData_RemovedItem()
        {
            const int searchValue = 9;

            List<int> listItems = new List<int>
            {
                2,
                searchValue,
                4,
                15,
                searchValue
            };
            listItems.RemoveFirstValue(searchValue);

            bool evaluate = listItems[0] != searchValue &&
                            listItems.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveItemSwap_MockData_RemovedItem()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listItems = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), searchItem, new CircularBuffer<int>(4), new CircularBuffer<int>(15)
            };

            listItems.RemoveItemSwap(1);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveLastItem_MockData_RemovedItems()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listItems = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2),
                searchItem,
                new CircularBuffer<int>(4),
                new CircularBuffer<int>(15),
                searchItem
            };

            listItems.RemoveLastItem(searchItem);

            bool evaluate = listItems.Count == 4 &&
                            listItems[3] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveItems_MockData_RemovedItems()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listItems = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2),
                searchItem,
                new CircularBuffer<int>(4),
                new CircularBuffer<int>(15),
                searchItem
            };

            listItems.RemoveItems(searchItem);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveValues_MockData_RemovedValues()
        {
            const int searchValue = 9;

            List<int> listItems = new List<int>
            {
                2,
                searchValue,
                4,
                15,
                searchValue,
                9
            };

            listItems.RemoveValues(searchValue);

            bool evaluate = listItems.ContainsValue(searchValue);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveLastValue_MockData_RemovedValue()
        {
            const int searchValue = 9;
            List<int> mockData = new List<int>
            {
                2,
                searchValue,
                4,
                15,
                searchValue,
                98
            };
            mockData.RemoveLastValue(searchValue);

            bool evaluate = mockData[4] != searchValue &&
                            mockData.ContainsValue(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Shuffle_MockData_HasDifferences()
        {
            List<int> mockData = new List<int>
            {
                2,
                3,
                4,
                15,
                2,
                98,
                109,
                2,
                29,
                99,
                123,
                911,
                69,
                2,
                3,
                4,
                15,
                2,
                98,
                109,
                2,
                29,
                99,
                123,
                911,
                69
            };
            List<int> listItemB = new List<int>(mockData);
            mockData.Shuffle();

            int listLength = mockData.Count;
            int differentCount = listLength;
            for (int i = 0; i < listLength; i++)
            {
                if (mockData[i] == listItemB[i])
                {
                    differentCount--;
                }
            }

            bool evaluate = differentCount > 0;

            Assert.IsTrue(evaluate);
        }
    }
}