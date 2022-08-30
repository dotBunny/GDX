#if GDX_UNSAFE_COLLECTIONS
// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;
using Unity.Collections;

namespace GDX.Collections.Generic
{
    public unsafe class UnsafeSparseSetTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCount()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(4, Allocator.Temp);

            bool evaluate = mockList.Count == 0 && mockList.Length >= 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_CreateWithCountAndVersionArray()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(4, Allocator.Temp, out ulong* _);

            bool evaluate = mockList.Count == 0 && mockList.Length >= 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_IndicesReserved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(2, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray__IndicesReserved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(2, Allocator.Temp, out ulong* versionArray);
            versionArray[0] = 17;
            versionArray[1] = 5;

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int sparseIndex1, out int denseIndex1, versionArray, out ulong version1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && version0 == 17 && version1 == 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockData_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(1, Allocator.Temp);
            int initialLength = mockList.Length;
            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0);

            int tryCount = 0;
            bool expanded = false;
            int sparseIndex1 = 0;
            int denseIndex1 = 0;
            while (!expanded && tryCount < 1000)
            {
                expanded = mockList.AddWithExpandCheck(5, out sparseIndex1, out denseIndex1);
                tryCount++;
            }

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expanded && mockList.Length >= initialLength + 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataWithVersionArray_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(1, Allocator.Temp, out ulong* versionArray);
            int initialLength = mockList.Length;
            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0, ref versionArray);

            int tryCount = 0;
            bool expanded = false;
            int sparseIndex1 = 0;
            int denseIndex1 = 0;
            while (!expanded && tryCount < 1000)
            {
                expanded = mockList.AddWithExpandCheck(5, out sparseIndex1, out denseIndex1, ref versionArray);
                tryCount++;
            }

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expanded && mockList.Length >= initialLength + 5 && versionArray[mockList.Length - 1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(1, Allocator.Temp, out ulong* _);
            int initialLength = mockList.Length;
            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0);
            bool expanded = false;
            int tryCount = 0;

            int sparseIndex1 = 0;
            int denseIndex1 = 0;
            while (!expanded && tryCount < 1000)
            {
                expanded = mockList.AddWithExpandCheck(5, out sparseIndex1, out denseIndex1);
                tryCount++;
            }

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expanded && mockList.Length >= initialLength + 5;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexUnchecked_MockData_CorrectIndex()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(1, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex, out int denseIndex);

            int denseLookup = mockList.GetDenseIndexUnchecked(sparseIndex);

            bool evaluate = denseLookup == denseIndex;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithBoundsCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);

            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0);
            mockList.AddUnchecked(out int _, out int _);

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
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);

            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0, versionArray);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AddUnchecked(out int sparseIndex1, out int _, versionArray, out ulong version1);


            int badLookupIndexAfterReallocation = mockList.GetDenseIndexWithVersionCheck(sparseIndex0, version0, versionArray);

            int goodLookupIndexAfterReallocation = mockList.GetDenseIndexWithVersionCheck(sparseIndex1, version1, versionArray);

            bool evaluate = goodInitialLookup != -1 && badLookupIndexAfterFree == -1
                && badLookupIndexAfterReallocation == -1 && goodLookupIndexAfterReallocation != -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithBoundsAndVersionCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);

            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);

            int goodInitialLookup = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.RemoveUnchecked(sparseIndex0, versionArray, out int _, out int _);

            int badLookupIndexAfterFree = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            mockList.AddUnchecked(out int sparseIndex1, out int _, versionArray, out ulong version1);


            int badLookupIndexAfterReallocation = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex0, version0, versionArray);

            int goodLookupIndexAfterReallocation = mockList.GetDenseIndexWithBoundsAndVersionCheck(sparseIndex1, version1, versionArray);

            int outOfBoundsNegativeIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(-50, 0, versionArray);
            int outOfBoundsPositiveIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(50, 0, versionArray);
            int outOfCountBoundsIndex = mockList.GetDenseIndexWithBoundsAndVersionCheck(2, 0, versionArray);

            bool evaluate = goodInitialLookup != -1 && badLookupIndexAfterFree == -1
                && badLookupIndexAfterReallocation == -1 && goodLookupIndexAfterReallocation != -1
                && outOfBoundsNegativeIndex == -1 && outOfBoundsPositiveIndex == -1 && outOfCountBoundsIndex == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveWithBoundsCheck_MockData_ValidIndexRemovedInvalidIndicesNotRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int _);
            mockList.AddUnchecked(out int _, out int _);

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
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            int validIndexToRemove = sparseIndex0;

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
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            int validIndexToRemove = sparseIndex0;
            int indexToRemoveAfterFree = validIndexToRemove;
            int negativeIndex = -50;
            int indexPastLength = 50;

            bool removedNegative = mockList.RemoveWithBoundsAndVersionChecks(ref negativeIndex, version0, versionArray, out int negativeDataIndexToSwapFrom, out int negativeDataIndexToSwapTo);
            bool removedIndexPastLength = mockList.RemoveWithBoundsAndVersionChecks(ref indexPastLength, version0, versionArray, out int pastLengthDataIndexToSwapFrom, out int pastLengthDataIndexToSwapTo);
            bool removedValidIndex = mockList.RemoveWithBoundsAndVersionChecks(ref validIndexToRemove, version0, versionArray, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);
            bool removedAfterFree = mockList.RemoveWithBoundsAndVersionChecks(ref indexToRemoveAfterFree, version0, versionArray, out int afterFreeDataIndexToSwapFrom, out int afterFreeDataIndexToSwapTo);

            mockList.AddUnchecked(out int sparseIndex2, out int _, versionArray, out ulong version2);
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
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove, versionArray, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);

            bool evaluate = validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0 && mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataWithIndices_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int _);
            mockList.AddUnchecked(out int _, out int _);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove, out int validDataIndexToSwapFrom, out int validDataIndexToSwapTo);

            bool evaluate = validDataIndexToSwapFrom == 1 && validDataIndexToSwapTo == 0 && mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockData_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int sparseIndex0, out int _);
            mockList.AddUnchecked(out int _, out int _);

            int validIndexToRemove = sparseIndex0;

            mockList.RemoveUnchecked(validIndexToRemove);

            bool evaluate = mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockData_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int _, out int denseIndex0);
            mockList.AddUnchecked(out int _, out int _);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0);

            bool evaluate = mockList.Count == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithSwapIndex_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int _, out int denseIndex0);
            mockList.AddUnchecked(out int _, out int _);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, out int indexToSwapFrom);

            bool evaluate = mockList.Count == 1 && indexToSwapFrom == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithVersionArray_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, versionArray);

            bool evaluate = mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUncheckedFromDenseIndex_MockDataWithVersionArrayAndSwapIndex_ValidIndexRemoved()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int denseIndex0, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            mockList.RemoveUncheckedFromDenseIndex(denseIndex0, versionArray, out int indexToSwapFrom);

            bool evaluate = mockList.Count == 1 && versionArray[sparseIndex0] == version0 + 1 && indexToSwapFrom == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockData_ArraysCleared()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            mockList.AddUnchecked(out int _, out int _);
            mockList.AddUnchecked(out int _, out int _);

            mockList.Clear();

            bool evaluate = mockList.Count == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Clear_MockDataWithVersionArray_ArraysCleared()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong version0);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            mockList.Clear(versionArray);

            bool evaluate = mockList.Count == 0 && versionArray[sparseIndex0] == version0 + 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ClearWithVersionReset_MockData_ArraysClearedAndVersionReset()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.AddUnchecked(out int sparseIndex0, out int _, versionArray, out ulong _);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            mockList.ClearWithVersionArrayReset(versionArray);

            bool evaluate = mockList.Count == 0 && versionArray[sparseIndex0] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Expand_MockData_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            int initialCapacity = mockList.Length;
            mockList.Expand(3);

            bool evaluate = mockList.Length > initialCapacity + 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Expand_MockDataWithVersionArray_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            int initialCapacity = mockList.Length;
            mockList.Expand(3, ref versionArray);

            bool evaluate = mockList.Length > initialCapacity + 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockData_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);
            int initialLength = mockList.Length;
            mockList.AddUnchecked(out int _, out int _);
            mockList.Reserve(initialLength + 3);

            bool evaluate = mockList.Length >= initialLength + 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockDataWithVersionArray_Expanded()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            int initialLength = mockList.Length;
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);
            mockList.Reserve(initialLength + 3, ref versionArray);

            bool evaluate = mockList.Length >= initialLength + 3;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Dispose_MockDataWithVersionArray_Disposed()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp, out ulong* versionArray);
            mockList.DisposeVersionArray(ref versionArray);
            mockList.Dispose();

            bool evaluate = mockList.Data == null && versionArray == null;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void DenseAndSparseAccess_MockData_ArraysArePlacedCorrectly()
        {
            UnsafeSparseSet mockList = new UnsafeSparseSet(3, Allocator.Temp);

            int length = mockList.Length;
            int* sparse = mockList.SparseArray;
            int* dense = mockList.DenseArray;

            bool evaluate = dense == sparse + length;

            Assert.IsTrue(evaluate);
        }
    }
}
#endif