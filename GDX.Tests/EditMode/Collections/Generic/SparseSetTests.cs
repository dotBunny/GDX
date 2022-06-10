// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace GDX.Collections.Generic
{
    public class SparseSetTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCount()
        {
            SparseSet mockList = new SparseSet(4);

            bool evaluate = mockList.Count == 0 && mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCountAndVersionArray()
        {
            ulong[] versionArray;
            SparseSet mockList = new SparseSet(4, out versionArray);

            bool evaluate = mockList.Count == 0 && mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4 && versionArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_IndicesReserved()
        {
            SparseSet mockList = new SparseSet(2);

            int sparseIndex0, sparseIndex1, denseIndex0, denseIndex1;
            mockList.AllocEntryNoExpand(out sparseIndex0, out denseIndex0);
            mockList.AllocEntryNoExpand(out sparseIndex1, out denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray__IndicesReserved()
        {
            ulong[] versionArray;
            SparseSet mockList = new SparseSet(2, out versionArray);
            versionArray[0] = 17;
            versionArray[1] = 5;

            int sparseIndex0, sparseIndex1, denseIndex0, denseIndex1;
            mockList.AllocEntryNoExpand(out sparseIndex0, out denseIndex0, versionArray, out ulong version0);
            mockList.AllocEntryNoExpand(out sparseIndex1, out denseIndex1, versionArray, out ulong version1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && version0 == 17 && version1 == 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AllocEntryNoExpand_MockDataOverflow_ThrowsException()
        {
            SparseSet mockList = new SparseSet(1);

            mockList.AllocEntryNoExpand(out int sparseIndex, out int denseIndex);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AllocEntryNoExpand(out sparseIndex, out denseIndex); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AllocEntryNoExpand_MockDataAndVersionArrayOverflow_ThrowsException()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] versionArray);

            mockList.AllocEntryNoExpand(out int sparseIndex, out int denseIndex, versionArray, out ulong version);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AllocEntryNoExpand(out sparseIndex, out denseIndex, versionArray, out version); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AllocEntryExpandIfNeeded_MockData_Expanded()
        {
            SparseSet mockList = new SparseSet(1);

            bool expandedFirstTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AllocEntryExpandIfNeeded_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] versionArray);

            bool expandedFirstTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] versionArray);

            bool expandedFirstTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AllocEntryExpandIfNeeded(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        public void GetDenseIndexUnchecked_MockData_CorrectIndex()
        {
            SparseSet mockList = new SparseSet(1);

            mockList.AllocEntryNoExpand(out int sparseIndex, out int denseIndex);

            int denseLookup = mockList.GetDenseIndexUnchecked(sparseIndex);

            bool evaluate = denseLookup == denseIndex;

            Assert.IsTrue(evaluate);
        }

        public void GetDenseIndexWithBoundsCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            SparseSet mockList = new SparseSet(3);

            mockList.AllocEntryNoExpand(out int sparseIndex0, out int denseIndex0);
            mockList.AllocEntryNoExpand(out int sparseIndex1, out int denseIndex1);

            int denseLookupCorrect = mockList.GetDenseIndexWithBoundsCheck(sparseIndex0);
            int denseLookupNegative = mockList.GetDenseIndexWithBoundsCheck(-50);
            int denseLookupGreaterThanLength = mockList.GetDenseIndexWithBoundsCheck(3);
            int denseLookupGreaterThanCount = mockList.GetDenseIndexWithBoundsCheck(2);

            bool evaluate = denseIndex0 == denseLookupCorrect && denseLookupNegative == -1
                && denseLookupGreaterThanLength == -1 && denseLookupGreaterThanCount == -1;

            Assert.IsTrue(evaluate);
        }

        public void GetDenseIndexWithVersionCheck_MockData_CorrectIndexAndHandlesVersion()
        {
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);

            mockList.AllocEntryNoExpand(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AllocEntryNoExpand(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);


            int badLookupIndexAfterRealloc = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            int goodLookupIndexAfterRealloc = mockList.GetDenseIndexWithVersionCheck(sparseIndex1, version1, versionArray);

            bool evaluate = goodInitialLookup != -1 && badLookupIndexAfterFree == -1
                && badLookupIndexAfterRealloc == -1 && goodLookupIndexAfterRealloc != -1;

            Assert.IsTrue(evaluate);
        }

        public void GetDenseIndexWithBoundsAndVersionCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);

            mockList.AllocEntryNoExpand(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AllocEntryNoExpand(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);


            int badLookupIndexAfterRealloc = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            int goodLookupIndexAfterRealloc = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex1, version1, versionArray);

            int outOfBoundsNegativeIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(-50, 0, versionArray);
            int outOfBoundsPositiveIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(50, 0, versionArray);
            int outOfCountBoundsIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(2, 0, versionArray);

            bool evaluate = goodInitialLookup != -1 && badLookupIndexAfterFree == -1
                && badLookupIndexAfterRealloc == -1 && goodLookupIndexAfterRealloc != -1
                && outOfBoundsNegativeIndex == -1 && outOfBoundsPositiveIndex == -1 && outOfCountBoundsIndex == -1;

            Assert.IsTrue(evaluate);
        }
    }
}