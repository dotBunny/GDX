// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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

        [Test]
        [Category("GDX.Tests")]
        public void AddUnchecked_MockData_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);

            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);

            bool evaluate = (mockList.Array[0] == 2 && mockList.Array[1] == 7);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddUnchecked_MockDataOverflow_ThrowsException()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddUnchecked(2);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(7); });
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheck_MockData_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7);

            bool evaluate = (mockList.Array[0] == 2 && mockList.Array[1] == 7);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddWithExpandCheck_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7, 10);

            bool evaluate = (mockList.Array[0] == 2 && mockList.Array[1] == 7 && mockList.Array.Length == 11);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Clear_MockData_DataCleared()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7);

            mockList.Clear();

            bool evaluate = (mockList.Array[1] == 0);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Insert_MockDataAtEnd_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.Insert(4, 2);

            bool evaluate = (mockList.Array[2] == 4);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Insert_MockDataAtStart_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.Insert(4, 0);

            bool evaluate = (mockList.Array[0] == 4);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Insert_MockDataInMiddle_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.Insert(4, 1);

            bool evaluate = (mockList.Array[1] == 4);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveAt_MockDataAtEnd_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(2);

            bool evaluate = (mockList.Count == 2 && mockList.Array[1] == 1);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveAt_MockDataAtStart_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(0);

            bool evaluate = (mockList.Count == 2 && mockList.Array[0] == 1);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveAt_MockDataInMiddle_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(1);

            bool evaluate = (mockList.Count == 2 && mockList.Array[1] == 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RemoveFromBack_MockData_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveFromBack();

            bool evaluate = (mockList.Count == 2 && mockList.Array[1] == 1);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Reserve_MockData_ArrayExpanded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.Reserve(10);

            bool evaluate = (mockList.Array.Length == 10);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Reverse_MockDataEven_ArrayReversed()
        {
            SimpleList<int> mockList = new SimpleList<int>(6);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(3);
            mockList.AddUnchecked(4);
            mockList.AddUnchecked(5);

            mockList.Reverse();

            bool evaluate = mockList.Array[0] == 5 &&
                            mockList.Array[1] == 4 &&
                            mockList.Array[2] == 3 &&
                            mockList.Array[3] == 2 &&
                            mockList.Array[4] == 1 &&
                            mockList.Array[5] == 0;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category("GDX.Tests")]
        public void Reverse_MockDataOdd_ArrayReversed()
        {
            SimpleList<int> mockList = new SimpleList<int>(5);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(3);
            mockList.AddUnchecked(4);

            mockList.Reverse();

            bool evaluate = mockList.Array[0] == 4 &&
                            mockList.Array[1] == 3 &&
                            mockList.Array[2] == 2 &&
                            mockList.Array[3] == 1 &&
                            mockList.Array[4] == 0;

            Assert.IsTrue(evaluate);
        }
        [Test]
        [Category("GDX.Tests")]
        public void Reverse_MockDataOddLimited_ArrayReversed()
        {
            SimpleList<int> mockList = new SimpleList<int>(5);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.Reverse();

            bool evaluate = mockList.Array[0] == 2 &&
                            mockList.Array[1] == 1 &&
                            mockList.Array[2] == 0;

            Assert.IsTrue(evaluate);
        }
    }
}