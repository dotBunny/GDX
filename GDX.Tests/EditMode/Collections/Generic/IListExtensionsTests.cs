// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using NUnit.Framework;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="IListExtensions" />
    ///     class.
    /// </summary>
    public class ListExtensionsTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void AddUniqueItem_UniqueItem_ReturnsTrue()
        {
            object searchValue = new object();
            List<object> listValues = new List<object>
            {
                new object(), new object(), new object()
            };

            bool evaluate = listValues.AddUniqueItem(searchValue);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUniqueItem_NonUniqueItem_ReturnsFalse()
        {
            object searchValue = new object();
            List<object> listValues = new List<object>
            {
                new object(), new object(), searchValue, new object()
            };

            bool evaluate = listValues.AddUniqueItem(searchValue);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsItem_MockData_FindsItem()
        {
            object mockObject = new object();
            List<object> mockData = new List<object>
            {
                new object(),
                mockObject,
                new object(),
                new object(),
                mockObject
            };

            bool evaluate = mockData.ContainsItem(mockObject);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ContainsItem_StringArray_FindsItem()
        {
            string[] mockObject = new string[] {TestLiterals.Foo, TestLiterals.Bar, TestLiterals.Seed};
            bool evaluate = ArrayExtensions.ContainsItem(mockObject, TestLiterals.Bar);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveFirstItem_MockData_RemovedItem()
        {
            object searchItem = new object();
            List<object> listItems = new List<object>
            {
                new object(),
                searchItem,
                new object(),
                new object(),
                searchItem
            };

            listItems.RemoveFirstItem(searchItem);

            bool evaluate = listItems[1] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveFirstItem_BadMockData_ReturnsFalse()
        {
            object searchItem = new object();
            object mockData = new object();
            List<object> listItems = new List<object>
            {
                new object(),
                searchItem,
                new object(),
                new object(),
                searchItem
            };

            bool evaluate = listItems.RemoveFirstItem(mockData);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveItemSwap_MockData_RemovedItem()
        {
            object searchItem = new object();
            List<object> listItems = new List<object>
            {
                new object(), searchItem, new object(), new object()
            };

            listItems.RemoveItemSwap(1);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveLastItem_MockData_RemovedItems()
        {
            object searchItem = new object();
            List<object> listItems = new List<object>
            {
                new object(),
                searchItem,
                new object(),
                new object(),
                searchItem
            };

            listItems.RemoveLastItem(searchItem);

            bool evaluate = listItems.Count == 4 &&
                            listItems[3] != searchItem &&
                            listItems.ContainsItem(searchItem);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveLastItem_BadMockData_ReturnsFalse()
        {
            object searchItem = new object();
            List<object> listItems = new List<object>
            {
                new object(),
                new object(),
                new object()
            };

            bool evaluate = listItems.RemoveLastItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveItems_MockData_RemovedItems()
        {
            object searchItem = new object();
            List<object> listItems = new List<object>
            {
                new object(),
                searchItem,
                new object(),
                new object(),
                searchItem
            };

            listItems.RemoveItems(searchItem);

            bool evaluate = listItems.ContainsItem(searchItem);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
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