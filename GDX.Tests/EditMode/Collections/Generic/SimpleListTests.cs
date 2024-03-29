// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="SimpleList{T}" />.
    /// </summary>
    public class SimpleListTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCount()
        {
            SimpleList<int> mockList = new SimpleList<int>(4);

            bool evaluate = mockList.Count == 0 && mockList.Array is { Length: 4 };

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithExisting_FillsList()
        {
            int[] mockArray = {0, 1, 2, 3};

            SimpleList<int> mockList = new SimpleList<int>(mockArray, mockArray.Length);

            bool evaluate = mockList.Count == 4 && mockList.Array[2] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithExistingNoCount_FillsListNoCount()
        {
            int[] mockArray = {0, 1, 2, 3};

            SimpleList<int> mockList = new SimpleList<int>(mockArray);

            bool evaluate = mockList.Count == 0 && mockList.Array[2] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);

            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);

            bool evaluate = mockList.Array[0] == 2 && mockList.Array[1] == 7;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataOverflow_ThrowsException()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddUnchecked(2);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(7); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockData_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7);

            bool evaluate = mockList.Array[0] == 2 && mockList.Array[1] == 7;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7, 10);

            bool evaluate = mockList.Array[0] == 2 && mockList.Array[1] == 7 && mockList.Array.Length == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddExpandNoClear_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandNoClear(2, pool);
            mockList.AddExpandNoClear(7, pool);
            mockList.AddExpandNoClear(3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            mockList.Array[2] == 3 &&
                            mockList.Array[3] == 5 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddExpandClearOld_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[0] = 0;
            poolMinimums[1] = 1;
            poolMaximums[0] = 1;
            poolMaximums[1] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            mockList.AddExpandClearOld(2, pool);
            mockList.AddExpandClearOld(7, pool);

            bool evaluate = mockList.Array.Length == 2 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            initialBackingArray[0] == 0 &&
                            pool.ArrayPools[0].Count == 1 &&
                            pool.ArrayPools[1].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddExpandClearNew_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandClearNew(2, pool);
            mockList.AddExpandClearNew(7, pool);
            mockList.AddExpandClearNew(3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            mockList.Array[2] == 3 &&
                            mockList.Array[3] == 0 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddExpandClearBoth_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandClearBoth(2, pool);
            mockList.AddExpandClearBoth(7, pool);
            mockList.AddExpandClearBoth(3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            mockList.Array[2] == 3 &&
                            mockList.Array[3] == 0 &&
                            initialBackingArray[0] == 0 &&
                            initialBackingArray[1] == 0 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertExpandNoClear_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandNoClear(2, pool);
            mockList.AddExpandNoClear(7, pool);
            mockList.InsertExpandNoClear(1, 3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 3 &&
                            mockList.Array[2] == 7 &&
                            mockList.Array[3] == 5 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertExpandClearOld_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[0] = 0;
            poolMinimums[1] = 1;
            poolMaximums[0] = 1;
            poolMaximums[1] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            mockList.AddExpandClearOld(2, pool);
            mockList.InsertExpandClearOld(0, 7, pool);

            bool evaluate = mockList.Array.Length == 2 &&
                            mockList.Array[0] == 7 &&
                            mockList.Array[1] == 2 &&
                            initialBackingArray[0] == 0 &&
                            pool.ArrayPools[0].Count == 1 &&
                            pool.ArrayPools[1].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertExpandClearNew_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandClearNew(2, pool);
            mockList.AddExpandClearNew(7, pool);
            mockList.InsertExpandClearNew(1, 3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 3 &&
                            mockList.Array[2] == 7 &&
                            mockList.Array[3] == 0 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertExpandClearBoth_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddExpandClearBoth(2, pool);
            mockList.AddExpandClearBoth(7, pool);
            mockList.InsertExpandClearBoth(1, 3, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 3 &&
                            mockList.Array[2] == 7 &&
                            mockList.Array[3] == 0 &&
                            initialBackingArray[0] == 0 &&
                            initialBackingArray[1] == 0 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockData_DataCleared()
        {
            SimpleList<int> mockList = new SimpleList<int>(1);

            mockList.AddWithExpandCheck(2);
            mockList.AddWithExpandCheck(7);

            mockList.Clear();

            bool evaluate = mockList.Array[1] == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertUnchecked_MockDataAtEnd_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(4);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertUnchecked(2, 4);

            bool evaluate = mockList.Array[2] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertUnchecked_MockDataAtStart_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(4);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertUnchecked(0, 4);

            bool evaluate = mockList.Array[0] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertUnchecked_MockDataInMiddle_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(4);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertUnchecked(1, 4);

            bool evaluate = mockList.Array[1] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataAtEnd_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(2, 4);

            bool evaluate = mockList.Array[2] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataAtStart_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(0, 4);

            bool evaluate = mockList.Array[0] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataInMiddle_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(1, 4);

            bool evaluate = mockList.Array[1] == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataAtEndAndCustomCount_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(2, 4, 5);

            bool evaluate = mockList.Array[2] == 4 && mockList.Array.Length == 8;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataAtStartAndCustomCount_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(0, 4, 5);

            bool evaluate = mockList.Array[0] == 4 && mockList.Array.Length == 8;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InsertWithExpandCheck_MockDataInMiddleAndCustomCount_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.InsertWithExpandCheck(1, 4, 5);

            bool evaluate = mockList.Array[1] == 4 && mockList.Array.Length == 8;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAt_MockDataAtEnd_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(2);

            bool evaluate = mockList.Count == 2 && mockList.Array[1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAt_MockDataAtStart_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(0);

            bool evaluate = mockList.Count == 2 && mockList.Array[0] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAt_MockDataInMiddle_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAt(1);

            bool evaluate = mockList.Count == 2 && mockList.Array[1] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAtSwapBack_MockDataAtEnd_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAtSwapBack(2);

            bool evaluate = mockList.Count == 2 && mockList.Array[1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAtSwapBack_MockDataAtStart_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAtSwapBack(0);

            bool evaluate = mockList.Count == 2 && mockList.Array[0] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAtSwapBack_MockDataInMiddle_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveAtSwapBack(1);

            bool evaluate = mockList.Count == 2 && mockList.Array[1] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveFromBack_MockData_ValueRemoved()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.AddUnchecked(0);
            mockList.AddUnchecked(1);
            mockList.AddUnchecked(2);

            mockList.RemoveFromBack();

            bool evaluate = mockList.Count == 2 && mockList.Array[1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockData_ArrayExpanded()
        {
            SimpleList<int> mockList = new SimpleList<int>(3);

            mockList.Reserve(10);

            bool evaluate = mockList.Array.Length == 10;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReserveNoClear_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(4);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[2] = 0;
            poolMinimums[3] = 1;
            poolMaximums[2] = 1;
            poolMaximums[3] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[3].Pool[0];
            replacementArrayOnExpand[5] = 5;
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);
            mockList.AddUnchecked(3);
            mockList.ReserveNoClear(2, pool);

            bool evaluate = mockList.Array.Length == 8 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            mockList.Array[2] == 3 &&
                            mockList.Array[5] == 5 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[2].Count == 1 &&
                            pool.ArrayPools[3].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReserveClearOld_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);
            mockList.ReserveClearOld(1, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            initialBackingArray[0] == 0 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReserveClearNew_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);
            mockList.ReserveClearNew(1, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            initialBackingArray[0] == 2 &&
                            initialBackingArray[1] == 7 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReserveClearBoth_MockDataWithExpansion_ValueAdded()
        {
            SimpleList<int> mockList = new SimpleList<int>(2);
            int[] initialBackingArray = mockList.Array;
            int[] poolMinimums = new int[32];
            int[] poolMaximums = new int[32];
            poolMinimums[1] = 0;
            poolMinimums[2] = 1;
            poolMaximums[1] = 1;
            poolMaximums[2] = 1;

            Pooling.ArrayPool<int> pool = new Pooling.ArrayPool<int>(poolMinimums, poolMaximums);
            int[] replacementArrayOnExpand = pool.ArrayPools[2].Pool[0];
            replacementArrayOnExpand[3] = 5;
            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);
            mockList.ReserveClearBoth(1, pool);

            bool evaluate = mockList.Array.Length == 4 &&
                            mockList.Array[0] == 2 &&
                            mockList.Array[1] == 7 &&
                            initialBackingArray[0] == 0 &&
                            initialBackingArray[1] == 0 &&
                            pool.ArrayPools[1].Count == 1 &&
                            pool.ArrayPools[2].Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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