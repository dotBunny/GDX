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
    }
}