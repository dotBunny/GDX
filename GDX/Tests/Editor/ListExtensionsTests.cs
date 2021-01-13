// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Tests.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ListExtensions" />
    ///     class.
    /// </summary>
    public class ListExtensionsTests
    {
        #region ContainsItem

        /// <summary>
        ///     Check if we can find the item in a <see cref="List{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsItem_SecondValue()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listValues = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2),
                searchValue,
                new CircularBuffer<int>(4),
                new CircularBuffer<int>(15),
                searchValue
            };
            Assert.IsTrue(listValues.ContainsItem(searchValue),
                $"Expected positive response to looking for {searchValue} value.");
        }

        #endregion

        #region ContainsValue

        /// <summary>
        ///     Check if we can find the value in a <see cref="List{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsValue_SecondValue()
        {
            const int searchValue = 1;
            List<int> listValues = new List<int>
            {
                0,
                searchValue,
                2,
                3,
                searchValue
            };
            Assert.IsTrue(listValues.ContainsValue(searchValue),
                $"Expected positive response to looking for {searchValue} value.");
        }

        #endregion

        #region RemoveFirstItem

        /// <summary>
        ///     Check that removing the first item from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveFirstItem_Simple()
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
            Assert.IsTrue(listItems[1] != searchItem &&
                          listItems.ContainsItem(searchItem), "Item was expected to have been removed correctly.");
        }

        #endregion

        #region RemoveLastItem

        /// <summary>
        ///     Check that removing the last item from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveLastItem_Simple()
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
            Assert.IsTrue(listItems.Count == 4 &&
                          listItems[3] != searchItem &&
                          listItems.ContainsItem(searchItem), "Item was expected to have been removed correctly.");
        }

        #endregion

        #region RemoveFirstValue

        /// <summary>
        ///     Check that removing the first value from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveFirstValue_Simple()
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
            Assert.IsTrue(listItems[0] != searchValue &&
                          listItems.ContainsValue(searchValue), "Value was expected to have been removed correctly.");
        }

        #endregion

        #region RemoveItems

        /// <summary>
        ///     Check that removing all references to an item from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveItems_Simple()
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
            Assert.IsTrue(!listItems.ContainsItem(searchItem), "Item was expected to have been removed correctly.");
        }

        #endregion

        #region RemoveValues

        /// <summary>
        ///     Check that removing the all values from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveValues_Simple()
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
            Assert.IsTrue(!listItems.ContainsValue(searchValue), "Value was expected to be fully removed..");
        }

        #endregion

        #region RemoveItemSwap

        /// <summary>
        ///     Check that forced removal from the end of a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveItemSwap_Simple()
        {
            CircularBuffer<int> searchItem = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listItems = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), searchItem, new CircularBuffer<int>(4), new CircularBuffer<int>(15)
            };

            listItems.RemoveItemSwap(1);
            Assert.IsTrue(!listItems.ContainsItem(searchItem), "Item was expected to have been removed correctly.");
        }

        #endregion

        #region RemoveLastValue

        /// <summary>
        ///     Check that removing the last value from a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveLastValue_Simple()
        {
            const int searchValue = 9;

            List<int> listItems = new List<int>
            {
                2,
                searchValue,
                4,
                15,
                searchValue,
                98
            };
            listItems.RemoveLastValue(searchValue);
            Assert.IsTrue(listItems[4] != searchValue &&
                          listItems.ContainsValue(searchValue), "Value was expected to have been removed correctly.");
        }

        #endregion

        #region Shuffle

        /// <summary>
        ///     Check that shuffling a <see cref="List{T}" /> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_Shuffle_Simple()
        {
            List<int> listItemA = new List<int>
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
            List<int> listItemB = new List<int>
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

            listItemA.Shuffle();

            int listLength = listItemA.Count;
            int differentCount = listLength;
            for (int i = 0; i < listLength; i++)
            {
                if (listItemA[0] == listItemB[1])
                {
                    differentCount--;
                }
            }

            Assert.IsTrue(differentCount > 0, "List was not randomized.");
        }

        #endregion

        #region AddUniqueItem

        /// <summary>
        ///     Check if we can add a unique item to a <see cref="List{T}" /> for the first time.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_AddUniqueItem_FirstTime()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listValues = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), new CircularBuffer<int>(4), new CircularBuffer<int>(15)
            };
            Assert.IsTrue(listValues.AddUniqueItem(searchValue),
                "Expected to be able to add the new item.");
        }

        /// <summary>
        ///     Checks that we cant add a unique item a second time to a <see cref="List{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void False_AddUniqueItem_SecondTime()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(5);
            List<CircularBuffer<int>> listValues = new List<CircularBuffer<int>>
            {
                new CircularBuffer<int>(2), new CircularBuffer<int>(4), searchValue, new CircularBuffer<int>(15)
            };
            Assert.IsFalse(listValues.AddUniqueItem(searchValue),
                "Expected not to be able to add the duplicate item.");
        }

        #endregion

        #region AddUniqueValue

        /// <summary>
        ///     Check if we can add a unique value to a <see cref="List{T}" /> for the first time.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_AddUniqueValue_FirstTime()
        {
            const int searchValue = 1;
            List<int> listValues = new List<int> {0, 2, 3};
            Assert.IsTrue(listValues.AddUniqueValue(searchValue),
                $"Expected to be able to add {searchValue} value.");
        }

        /// <summary>
        ///     Checks that we cant add a unique value a second time to a <see cref="List{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void False_AddUniqueValue_SecondTime()
        {
            const int searchValue = 1;
            List<int> listValues = new List<int> {0, 2, 3};
            listValues.AddUniqueValue(searchValue);
            Assert.IsFalse(listValues.AddUniqueValue(searchValue),
                "Expected not to be able to add value.");
        }

        #endregion
    }
}