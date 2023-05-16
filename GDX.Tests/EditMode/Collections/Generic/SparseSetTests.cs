// Copyright (c) 2020-2023 dotBunny Inc.
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
            SparseSet mockList = new SparseSet(4, out ulong[] versionArray);

            bool evaluate = mockList.Count == 0 && mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4 && versionArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockData_IndicesReserved()
        {
            SparseSet mockList = new SparseSet(2);

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
            SparseSet mockList = new SparseSet(2, out ulong[] versionArray);
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
        public void AddUnchecked_MockDataOverflow_ThrowsException()
        {
            SparseSet mockList = new SparseSet(1);

            mockList.AddUnchecked(out int _, out int _);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(out int _, out int _); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataAndVersionArrayOverflow_ThrowsException()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] versionArray);

            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);

            Assert.Throws<IndexOutOfRangeException>(() => { mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _); });
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockData_Expanded()
        {
            SparseSet mockList = new SparseSet(1);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] _);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(1, out ulong[] _);

            bool expandedFirstTime = mockList.AddWithExpandCheck(5, out int sparseIndex0, out int denseIndex0);
            bool expandedSecondTime = mockList.AddWithExpandCheck(5, out int sparseIndex1, out int denseIndex1);

            bool evaluate = mockList.SparseArray[sparseIndex0] == denseIndex0 && mockList.DenseArray[denseIndex0] == sparseIndex0
                && mockList.SparseArray[sparseIndex1] == denseIndex1 && mockList.DenseArray[denseIndex1] == sparseIndex1
                && expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexUnchecked_MockData_CorrectIndex()
        {
            SparseSet mockList = new SparseSet(1);

            mockList.AddUnchecked(out int sparseIndex, out int denseIndex);

            int denseLookup = mockList.GetDenseIndexUnchecked(sparseIndex);

            bool evaluate = denseLookup == denseIndex;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDenseIndexWithBoundsCheck_MockData_CorrectIndexAndHandlesInvalidIndex()
        {
            SparseSet mockList = new SparseSet(3);

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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);

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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);

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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
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
            SparseSet mockList = new SparseSet(3);
            mockList.Expand(3);

            bool evaluate = mockList.SparseArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Expand_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
            mockList.Expand(3, ref versionArray);

            bool evaluate = mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6 && versionArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockData_Expanded()
        {
            SparseSet mockList = new SparseSet(3);
            mockList.AddUnchecked(out int _, out int _);
            mockList.Reserve(3);

            bool evaluate = mockList.SparseArray.Length == 4 && mockList.DenseArray.Length == 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Reserve_MockDataWithVersionArray_Expanded()
        {
            SparseSet mockList = new SparseSet(3, out ulong[] versionArray);
            mockList.AddUnchecked(out int _, out int _, versionArray, out ulong _);
            mockList.Expand(3, ref versionArray);

            bool evaluate = mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6 && versionArray.Length == 6;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric0_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray[0] == 0 && dataArray[1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric1_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric2_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric3_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            int[] dataArray3 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, 30, ref dataArray3, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, 31, ref dataArray3, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric4_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            int[] dataArray3 = new int[1];
            int[] dataArray4 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, 30, ref dataArray3, 40, ref dataArray4, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, 31, ref dataArray3, 41, ref dataArray4,  out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric5_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            int[] dataArray3 = new int[1];
            int[] dataArray4 = new int[1];
            int[] dataArray5 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, 30, ref dataArray3, 40, ref dataArray4, 50, ref dataArray5, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, 31, ref dataArray3, 41, ref dataArray4, 51, ref dataArray5,  out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric6_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            int[] dataArray3 = new int[1];
            int[] dataArray4 = new int[1];
            int[] dataArray5 = new int[1];
            int[] dataArray6 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, 30, ref dataArray3, 40, ref dataArray4, 50, ref dataArray5, 60, ref dataArray6, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, 31, ref dataArray3, 41, ref dataArray4, 51, ref dataArray5, 61, ref dataArray6, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51
                && dataArray6[0] == 60 && dataArray6[1] == 61;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddWithExpandCheck_MockDataGeneric7_Expanded()
        {
            SparseSet mockList = new SparseSet(1);
            int[] dataArray0 = new int[1];
            int[] dataArray1 = new int[1];
            int[] dataArray2 = new int[1];
            int[] dataArray3 = new int[1];
            int[] dataArray4 = new int[1];
            int[] dataArray5 = new int[1];
            int[] dataArray6 = new int[1];
            int[] dataArray7 = new int[1];
            bool expandedFirstTime = mockList.AddWithExpandCheck(0, ref dataArray0, 10, ref dataArray1, 20, ref dataArray2, 30, ref dataArray3, 40, ref dataArray4, 50, ref dataArray5, 60, ref dataArray6, 70, ref dataArray7, out int _, 5);
            bool expandedSecondTime = mockList.AddWithExpandCheck(1, ref dataArray0, 11, ref dataArray1, 21, ref dataArray2, 31, ref dataArray3, 41, ref dataArray4, 51, ref dataArray5, 61, ref dataArray6, 71, ref dataArray7, out int _, 5);

            bool evaluate = expandedFirstTime == false && expandedSecondTime && mockList.SparseArray.Length == 6 && mockList.DenseArray.Length == 6
                && dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51
                && dataArray6[0] == 60 && dataArray6[1] == 61
                && dataArray7[0] == 70 && dataArray7[1] == 71;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric0_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];

            mockList.AddUnchecked(0, dataArray0);
            mockList.AddUnchecked(1, dataArray0);

            bool evaluate = dataArray0[0] == 0 && dataArray0[1] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric1_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1);

            bool evaluate =
                dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric2_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric3_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric4_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric5_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric6_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];
            int[] dataArray6 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5, 60, dataArray6);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5, 61, dataArray6);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51
                && dataArray6[0] == 60 && dataArray6[1] == 61;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddUnchecked_MockDataGeneric7_Added()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];
            int[] dataArray6 = new int[2];
            int[] dataArray7 = new int[2];

            mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5, 60, dataArray6, 70, dataArray7);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5, 61, dataArray6, 71, dataArray7);

            bool evaluate =
                   dataArray0[0] == 0 && dataArray0[1] == 1
                && dataArray1[0] == 10 && dataArray1[1] == 11
                && dataArray2[0] == 20 && dataArray2[1] == 21
                && dataArray3[0] == 30 && dataArray3[1] == 31
                && dataArray4[0] == 40 && dataArray4[1] == 41
                && dataArray5[0] == 50 && dataArray5[1] == 51
                && dataArray6[0] == 60 && dataArray6[1] == 61
                && dataArray7[0] == 70 && dataArray7[1] == 71;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric0_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0);
            mockList.AddUnchecked(1, dataArray0);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric1_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric2_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11
                && dataArray2[1] == 0 && dataArray2[0] == 21;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric3_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2, dataArray3);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11
                && dataArray2[1] == 0 && dataArray2[0] == 21
                && dataArray3[1] == 0 && dataArray3[0] == 31;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric4_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2, dataArray3, dataArray4);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11
                && dataArray2[1] == 0 && dataArray2[0] == 21
                && dataArray3[1] == 0 && dataArray3[0] == 31
                && dataArray4[1] == 0 && dataArray4[0] == 41;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric5_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2, dataArray3, dataArray4, dataArray5);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11
                && dataArray2[1] == 0 && dataArray2[0] == 21
                && dataArray3[1] == 0 && dataArray3[0] == 31
                && dataArray4[1] == 0 && dataArray4[0] == 41
                && dataArray5[1] == 0 && dataArray5[0] == 51;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric6_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];
            int[] dataArray6 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5, 60, dataArray6);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5, 61, dataArray6);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2, dataArray3, dataArray4, dataArray5, dataArray6);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0 && dataArray0[0] == 1
                && dataArray1[1] == 0 && dataArray1[0] == 11
                && dataArray2[1] == 0 && dataArray2[0] == 21
                && dataArray3[1] == 0 && dataArray3[0] == 31
                && dataArray4[1] == 0 && dataArray4[0] == 41
                && dataArray5[1] == 0 && dataArray5[0] == 51
                && dataArray6[1] == 0 && dataArray6[0] == 61;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void RemoveUnchecked_MockDataGeneric7_Removed()
        {
            SparseSet mockList = new SparseSet(2);
            int[] dataArray0 = new int[2];
            int[] dataArray1 = new int[2];
            int[] dataArray2 = new int[2];
            int[] dataArray3 = new int[2];
            int[] dataArray4 = new int[2];
            int[] dataArray5 = new int[2];
            int[] dataArray6 = new int[2];
            int[] dataArray7 = new int[2];

            int sparseIndex0 = mockList.AddUnchecked(0, dataArray0, 10, dataArray1, 20, dataArray2, 30, dataArray3, 40, dataArray4, 50, dataArray5, 60, dataArray6, 70, dataArray7);
            mockList.AddUnchecked(1, dataArray0, 11, dataArray1, 21, dataArray2, 31, dataArray3, 41, dataArray4, 51, dataArray5, 61, dataArray6, 71, dataArray7);

            mockList.RemoveUnchecked(sparseIndex0, dataArray0, dataArray1, dataArray2, dataArray3, dataArray4, dataArray5, dataArray6, dataArray7);
            bool evaluate = mockList.Count == 1
                && dataArray0[1] == 0  && dataArray0[0] == 1
                && dataArray1[1] == 0  && dataArray1[0] == 11
                && dataArray2[1] == 0  && dataArray2[0] == 21
                && dataArray3[1] == 0  && dataArray3[0] == 31
                && dataArray4[1] == 0  && dataArray4[0] == 41
                && dataArray5[1] == 0  && dataArray5[0] == 51
                && dataArray6[1] == 0  && dataArray6[0] == 61
                && dataArray7[1] == 0  && dataArray7[0] == 71;

            Assert.IsTrue(evaluate);
        }
    }
}