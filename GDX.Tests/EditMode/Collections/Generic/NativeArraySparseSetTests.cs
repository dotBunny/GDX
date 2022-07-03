// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using Unity.Collections;

namespace GDX.Collections.Generic
{
    public class NativeArraySparseSetTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCount()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(4, Allocator.Temp);

            bool evaluate = mockList.Count == 0 && mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCountAndVersionArray()
        {
            NativeArray<ulong> versionArray;
            NativeArraySparseSet mockList = new NativeArraySparseSet(4, Allocator.Temp, out versionArray);

            bool evaluate = mockList.Count == 0 && mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4 && versionArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_IndicesReserved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(2, Allocator.Temp);

            int sparseIndex0, sparseIndex1, denseIndex0, denseIndex1;
            mockList.AddUnchecked(out sparseIndex0, out denseIndex0);
            mockList.AddUnchecked(out sparseIndex1, out denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray__IndicesReserved()
        {
            NativeArray<ulong> versionArray;
            NativeArraySparseSet mockList = new NativeArraySparseSet(2, Allocator.Temp, out versionArray);
            versionArray[0] = 17;
            versionArray[1] = 5;

            int sparseIndex0, sparseIndex1, denseIndex0, denseIndex1;
            mockList.AddUnchecked(out sparseIndex0, out denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out sparseIndex1, out denseIndex1, versionArray, out ulong version1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && version0 == 17 && version1 == 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataOverflow_ThrowsException()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex, out int denseIndex);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(out sparseIndex, out denseIndex); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataAndVersionArrayOverflow_ThrowsException()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp, out NativeArray<ulong> versionArray);

            mockList.AddUnchecked(out int sparseIndex, out int denseIndex, versionArray, out ulong version);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(out sparseIndex, out denseIndex, versionArray, out version); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockData_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0, Allocator.Temp);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1, Allocator.Temp);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataWithVersionArray_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp, out NativeArray<ulong> versionArray);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0, Allocator.Temp, ref versionArray);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1, Allocator.Temp, ref versionArray);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && versionArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp, out NativeArray<ulong> versionArray);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0, Allocator.Temp);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1, Allocator.Temp);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime == true && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexUnchecked_MockData_CorrectIndex()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(1, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex, out int denseIndex);

            int denseLookup = mockList.GetDenseIndexUnchecked(sparseIndex);

            bool evaluate = denseLookup == denseIndex;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithBoundsCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            int denseLookupCorrect = mockList.GetDenseIndexWithBoundsCheck(sparseIndex0);
            int denseLookupNegative = mockList.GetDenseIndexWithBoundsCheck(-50);
            int denseLookupGreaterThanLength = mockList.GetDenseIndexWithBoundsCheck(3);
            int denseLookupGreaterThanCount = mockList.GetDenseIndexWithBoundsCheck(2);

            bool evaluate = denseIndex0 == denseLookupCorrect && denseLookupNegative == -1
                && denseLookupGreaterThanLength == -1 && denseLookupGreaterThanCount == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithVersionCheck_MockData_CorrectIndexAndHandlesVersion()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0, versionArray);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);


            int badLookupIndexAfterRealloc = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            int goodLookupIndexAfterRealloc = mockList.GetDenseIndexWithVersionCheck(sparseIndex1, version1, versionArray);

            bool evaluate = goodInitialLookup != -1 && badLookupIndexAfterFree == -1
                && badLookupIndexAfterRealloc == -1 && goodLookupIndexAfterRealloc != -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithBoundsAndVersionCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0, versionArray, out int indexToSwapFrom, out int indexToSwapTo);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);


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

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveWithBoundsCheck_MockData_ValidIndexRemovedInvalidIndicesNotRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            int validIndexToRemove = sparseIndex0;
            int indexToRemoveAfterFree = validIndexToRemove;
            int negativeIndex = -50;
            int indexPastLength = 50;

            bool removedNegative = mockList.RemoveWithBoundsCheck(ref negativeIndex, out int negativeDataIndexToSwapFrom, out int negativeDataIndexToSwapTo);
            bool removedIndexPastLength = mockList.RemoveWithBoundsCheck(ref indexPastLength, out int pastLengthDataIndexToSwapFrom, out int pastLengthDataIndexToSwapTo);
            bool removedValidIndex = mockList.RemoveWithBoundsCheck(ref validIndexToRemove, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);
            bool removedAfterFree = mockList.RemoveWithBoundsCheck(ref indexToRemoveAfterFree, out int afterFreeDataIndexToSwapFrom, out int afterFreeDataIndexToSwapTo);

            bool evaluate = removedValidIndex && !removedNegative && !removedIndexPastLength && !removedAfterFree
                && validIndexToRemove == -1 && indexToRemoveAfterFree == -1 && negativeIndex == -1 && indexPastLength == -1
                && negativeDataIndexToSwapFrom == -1 && negativeDataIndexToSwapTo == -1
                && pastLengthDataIndexToSwapFrom == -1 && pastLengthDataIndexToSwapTo == -1
                && afterFreeDataIndexToSwapFrom == -1 && afterFreeDataIndexToSwapTo == -1
                && validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveWithVersionCheck_MockData_ValidIndexRemovedInvalidIndicesNotRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            int validIndexToRemove = sparseIndex0;
            int indexToRemoveAfterFree = validIndexToRemove;

            bool removedValidIndex = mockList.RemoveWithVersionCheck(validIndexToRemove, version0, versionArray, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);
            bool removedValidAfterFree = mockList.RemoveWithVersionCheck(validIndexToRemove, version0, versionArray, out int invalidDataIndexToSwapFrom, out int invalidDataIndexToSwapTo);

            bool evaluate = removedValidIndex && !removedValidAfterFree
               && invalidDataIndexToSwapFrom == -1 && invalidDataIndexToSwapTo == -1
               && validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveWithBoundsAndVersionChecks_MockData_ValidIndexRemovedInvalidIndicesNotRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            int validIndexToRemove = sparseIndex0;
            int indexToRemoveAfterFree = validIndexToRemove;
            int negativeIndex = -50;
            int indexPastLength = 50;

            bool removedNegative = mockList.RemoveWithBoundsAndVersionChecks(ref negativeIndex, version0, versionArray, out int negativeDataIndexToSwapFrom, out int negativeDataIndexToSwapTo);
            bool removedIndexPastLength = mockList.RemoveWithBoundsAndVersionChecks(ref indexPastLength, version0, versionArray, out int pastLengthDataIndexToSwapFrom, out int pastLengthDataIndexToSwapTo);
            bool removedValidIndex = mockList.RemoveWithBoundsAndVersionChecks(ref validIndexToRemove, version0, versionArray, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);
            bool removedAfterFree = mockList.RemoveWithBoundsAndVersionChecks(ref indexToRemoveAfterFree, version0, versionArray, out int afterFreeDataIndexToSwapFrom, out int afterFreeDataIndexToSwapTo);

            mockList.AddUnchecked(out int sparseIndex2, out int denseIndex2, versionArray, out ulong version2);
            bool removedAfterReallocate = mockList.RemoveWithBoundsAndVersionChecks(ref validIndexToRemove, version0, versionArray, out int afterReallocateDataIndexToSwapFrom, out int afterReallocateDataIndexToSwapTo);
            bool removedValidAfterReallocate = mockList.RemoveWithBoundsAndVersionChecks(ref sparseIndex2, version2, versionArray, out int validAfterReallocateDataIndexToSwapFrom, out int validAfterReallocateDataIndexToSwapTo);

            bool evaluate = removedValidIndex && !removedNegative && !removedIndexPastLength && !removedAfterFree && !removedAfterReallocate && removedValidAfterReallocate
                && validIndexToRemove == -1 && indexToRemoveAfterFree == -1 && negativeIndex == -1 && indexPastLength == -1
                && negativeDataIndexToSwapFrom == -1 && negativeDataIndexToSwapTo == -1
                && pastLengthDataIndexToSwapFrom == -1 && pastLengthDataIndexToSwapTo == -1
                && afterFreeDataIndexToSwapFrom == -1 && afterFreeDataIndexToSwapTo == -1
                && afterReallocateDataIndexToSwapFrom == -1 && afterReallocateDataIndexToSwapTo == -1
                && validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0
                && validAfterReallocateDataIndexToSwapFrom == 1 && validAfterReallocateDataIndexToSwapTo == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataWithIndicesAndVersionArray_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove, versionArray, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);

            bool evaluate = validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0 && mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataWithIndices_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);

            bool evaluate = validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0 && mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockData_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove);

            bool evaluate = mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockData_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0);

            bool evaluate = mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithSwapIndex_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, out int indexToSwapFrom);

            bool evaluate = mockList.Count == 1 && indexToSwapFrom == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithVersionArray_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, versionArray);

            bool evaluate = mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithVersionArrayAndSwapIndex_ValidIndexRemoved()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, versionArray, out int indexToSwapFrom);

            bool evaluate = mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1 && indexToSwapFrom == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockData_ArraysCleared()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            mockList.Clear();

            bool evaluate = mockList.Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockDataWithVersionArray_ArraysCleared()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            mockList.Clear(versionArray);

            bool evaluate = mockList.Count == 0 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ClearWithVersionReset_MockData_ArraysClearedAndVersionReset()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            mockList.ClearWithVersionArrayReset(versionArray);

            bool evaluate = mockList.Count == 0 && versionArray[sparseIndex0] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Expand_MockData_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.Expand(3, Allocator.Temp);

            bool evaluate = mockList.SparseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Expand_MockDataWithVersionArray_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.Expand(3, Allocator.Temp, ref versionArray);

            bool evaluate = mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6 && versionArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockData_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex, out int denseIndex);
            mockList.Reserve(3, Allocator.Temp);

            bool evaluate = mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockDataWithVersionArray_Expanded()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.AddUnchecked(out int sparseIndex, out int denseIndex, versionArray, out ulong version);
            mockList.Reserve(3, Allocator.Temp, ref versionArray);

            bool evaluate = mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4 && versionArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Dispose_MockData_Disposed()
        {
            NativeArraySparseSet mockList = new NativeArraySparseSet(3, Allocator.Temp, out NativeArray<ulong> versionArray);
            mockList.Dispose();

            bool evaluate = !mockList.SparseArray.IsCreated && !mockList.DenseArray.IsCreated;

            Assert.IsTrue(evaluate);
        }
    }
}