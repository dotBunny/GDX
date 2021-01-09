// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;

namespace GDX.Tests.Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SimpleListExtensions" />
    ///     class.
    /// </summary>
    public class SimpleListExtensionsTests
    {
        private SimpleList<int> _listValues;

        [SetUp]
        public void Setup()
        {
            _listValues = new SimpleList<int>(5);
            _listValues.AddUnchecked(0);
            _listValues.AddUnchecked(1);
            _listValues.AddUnchecked(2);
            _listValues.AddUnchecked(3);
            _listValues.AddUnchecked(4);
        }

        [TearDown]
        public void TearDown()
        {
            _listValues.Clear();
            _listValues = default;
        }


        #region ContainsItem

        /// <summary>
        ///     Check if we can find a <see cref="string" /> in a SimpleList, while its not the actual object, this validates
        ///     the .NET magic for strings.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsItem_String()
        {
            const string searchValue = "Hello";
            SimpleList<string> listOfStrings = new SimpleList<string>(3);
            listOfStrings.AddUnchecked(searchValue);
            listOfStrings.AddUnchecked("World");
            listOfStrings.AddUnchecked("!");

            Assert.IsTrue(listOfStrings.ContainsItem(searchValue),
                $"Expected positive response to looking for {searchValue} item.");
        }

        /// <summary>
        ///     Check if a class object can be correctly found inside of a <see cref="SimpleList{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsItem_CircularBuffer()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listOfCircularBuffers = new SimpleList<CircularBuffer<int>>(5);

            // Build test rig
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));

            Assert.IsTrue(listOfCircularBuffers.ContainsItem(searchValue),
                "Expected positive response to looking for target circular buffer.");
        }

        #endregion

        #region ContainsValue

        /// <summary>
        ///     Check if we can find the first struct/value in a <see cref="SimpleList{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsValue_FirstValue()
        {
            const int searchValue = 0;
            Assert.IsTrue(_listValues.ContainsValue(searchValue),
                $"Expected positive response to looking for {searchValue} value.");
        }

        /// <summary>
        ///     Check if we cant find a value in <see cref="SimpleList{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void False_ContainsValue_MissingValue()
        {
            const int searchValue = 12;
            Assert.IsFalse(_listValues.ContainsValue(searchValue),
                $"Expected negative response to looking for {searchValue} value.");
        }


        /// <summary>
        ///     Check if we can find the last struct/value in a <see cref="SimpleList{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsValue_LastValue()
        {
            const int searchValue = 4;
            Assert.IsTrue(_listValues.ContainsValue(searchValue),
                $"Expected positive response to looking for {searchValue} value.");
        }

        /// <summary>
        ///     Check if we can find a struct/value in amongst a <see cref="SimpleList{T}" />.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_ContainsValue_MidValue()
        {
            const int searchValue = 1;
            Assert.IsTrue(_listValues.ContainsValue(searchValue),
                $"Expected positive response to looking for {searchValue} value.");
        }

        #endregion

        #region RemoveFirstItem

        /// <summary>
        /// Check that removing the first item from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveFirstItem_Simple()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listOfCircularBuffers = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));

            listOfCircularBuffers.RemoveFirstItem(searchValue);
            Assert.IsTrue(listOfCircularBuffers.Array[1] != searchValue &&
                          listOfCircularBuffers.ContainsItem(searchValue), $"Item was expected to have been removed correctly.");

        }

        #endregion

        #region RemoveFirstValue

        /// <summary>
        /// Check that removing the first value from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveFirstValue_Simple()
        {
            const int targetValue = 1;
            SimpleList<int> listValues = new SimpleList<int>(6);
            listValues.AddUnchecked(0);
            listValues.AddUnchecked(targetValue);
            listValues.AddUnchecked(2);
            listValues.AddUnchecked(3);
            listValues.AddUnchecked(4);
            listValues.AddUnchecked(targetValue);

            listValues.RemoveFirstValue(targetValue);
            Assert.IsTrue(listValues.Array[1] == 2 && listValues.ContainsValue(targetValue), $"A value of 2 was expected.");
        }

        #endregion

        #region RemoveItems
        /// <summary>
        /// Check that removing the all references to an item from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveItems_Simple()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listOfCircularBuffers = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));

            listOfCircularBuffers.RemoveItems(searchValue);
            Assert.IsTrue(!listOfCircularBuffers.ContainsItem(searchValue), $"Items were expected to have been removed correctly.");

        }
        #endregion

        #region RemoveLastItem
        /// <summary>
        /// Check that removing the last item from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveLastItem_Simple()
        {
            CircularBuffer<int> searchValue = new CircularBuffer<int>(3, new[] {0, 1, 2});
            SimpleList<CircularBuffer<int>> listOfCircularBuffers = new SimpleList<CircularBuffer<int>>(6);

            // Build test rig
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));
            listOfCircularBuffers.AddUnchecked(searchValue);
            listOfCircularBuffers.AddUnchecked(new CircularBuffer<int>(5));

            listOfCircularBuffers.RemoveLastItem(searchValue);
            Assert.IsTrue(listOfCircularBuffers.Array[4] != searchValue &&
                          listOfCircularBuffers.ContainsItem(searchValue), $"Item was expected to have been removed correctly.");

        }
        #endregion

        #region RemoveLastValue

        /// <summary>
        /// Check that removing the last value from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveLastValue_Simple()
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
            Assert.IsTrue(listValues.Array[5] == 2, $"A value of 2 was expected.");
        }

        #endregion

        #region RemoveValues

        /// <summary>
        /// Check that removing all of a specific value from a <see cref="SimpleList{T}"/> works correctly.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_RemoveValues_Simple()
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

            Assert.IsTrue(!listValues.ContainsValue(1), $"No 1 values were expected.");
        }

        #endregion
    }
}