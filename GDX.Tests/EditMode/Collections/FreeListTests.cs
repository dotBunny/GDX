// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace GDX.Collections.Generic
{
	public class FreeListTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void FreeList_CreateWithCount()
        {
            FreeList mockList = new FreeList(4);

            bool evaluate = mockList.Count == 0 && mockList.Indices.Length == 4 ;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_ValueAdded()
        {
            FreeList mockList = new FreeList(2);

            mockList.AddUnchecked(2);
            mockList.AddUnchecked(7);

            bool evaluate = (mockList.Indices[0] == 2 && mockList.Indices[1] == 7);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataOverflow_ThrowsException()
        {
            FreeList mockList = new FreeList(1);

            mockList.AddUnchecked(2);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(7); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockData_ValueAdded()
        {
            FreeList mockList = new FreeList(1);

            mockList.AddWithExpandCheck(2, out int allocatedIndex0);
            mockList.AddWithExpandCheck(7, out int allocatedIndex1);

            bool evaluate = (mockList.Indices[allocatedIndex0] == 2 && mockList.Indices[allocatedIndex1] == 7);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataWithExpansion_ValueAdded()
        {
            FreeList mockList = new FreeList(1);

            mockList.AddWithExpandCheck(2, out int allocatedIndex0);
            mockList.AddWithExpandCheck(7, out int allocatedIndex1, 10);

            bool evaluate = (mockList.Indices[allocatedIndex0] == 2 && mockList.Indices[allocatedIndex1] == 7 && mockList.Indices.Length == 11);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockData_DataCleared()
        {
            FreeList mockList = new FreeList(1);

            mockList.AddWithExpandCheck(2, out int _);
            mockList.AddWithExpandCheck(7, out int _);

            mockList.Clear();

            bool evaluate = (mockList.Indices[1] == 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveAt_MockData_ValueRemoved()
        {
            FreeList mockList = new FreeList(3);

            mockList.AddUnchecked(7);
            mockList.AddUnchecked(8);
            mockList.AddUnchecked(9);

            mockList.RemoveAt(1);

            bool evaluate = (mockList.Count == 2 && mockList.Indices[1] == 3 && mockList.CurrentFreeIndex == 1 && mockList.Indices[2] == 9);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetAndRemoveAt_MockData_ValueRemoved()
        {
            FreeList mockList = new FreeList(3);

            mockList.AddUnchecked(7);
            mockList.AddUnchecked(8);
            mockList.AddUnchecked(9);

            int valueStored = mockList.GetAndRemoveAt(1);

            Assert.IsTrue(mockList.Count == 2);
            Assert.IsTrue(mockList.Indices[1] == 3);
            Assert.IsTrue(mockList.CurrentFreeIndex == 1);
            Assert.IsTrue(mockList.Indices[2] == 9);
            Assert.IsTrue(valueStored == 8);
        }
    }
}