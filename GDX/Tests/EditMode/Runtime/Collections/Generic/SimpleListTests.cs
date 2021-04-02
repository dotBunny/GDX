// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SimpleList{T}" />.
    /// </summary>
    public class SimpleListTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Constructor_CreateWithExisting_FillsList()
        {
            int[] mockArray = {0, 1, 2, 3};

            SimpleList<int> mockList = new SimpleList<int>(mockArray, mockArray.Length);

            bool evaluate = mockList.Count == 4 && mockList.Array[2] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Constructor_CreateWithExistingNoCount_FillsListNoCount()
        {
            int[] mockArray = {0, 1, 2, 3};

            SimpleList<int> mockList = new SimpleList<int>(mockArray);

            bool evaluate = mockList.Count == 0 && mockList.Array[2] == 2;

            Assert.IsTrue(evaluate);
        }
    }
}