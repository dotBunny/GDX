// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace GDX.Collections
{
    /// <summary>
    ///     An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by
    ///     handle, as well as array-like iteration.
    /// </summary>
    public struct NativeSparseSet
    {
        /// <summary>
        ///     Holds references to the sparse array for swapping indices.
        /// </summary>
        public NativeArray<int> DenseArray;

        /// <summary>
        ///     Holds references to dense array indices.
        /// </summary>
        /// <remarks>
        ///     Its own indices are claimed and freed via a free-list.
        /// </remarks>
        public NativeArray<int> SparseArray;

        /// <summary>
        ///     How many indices are being used currently?
        /// </summary>
        public int Count;

        /// <summary>
        ///     The first free (currently unused) index in the sparse array.
        /// </summary>
        public int FreeIndex;

        /// <summary>
        ///     Create a <see cref="NativeSparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeSparseSet(int initialCapacity, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            DenseArray = new NativeArray<int>(initialCapacity, allocator, nativeArrayOptions);
            SparseArray = new NativeArray<int>(initialCapacity, allocator, nativeArrayOptions);
            Count = 0;
            FreeIndex = 0;

            for (int i = 0; i < initialCapacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
            }
        }

        /// <summary>
        ///     Create a <see cref="NativeSparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeSparseSet(int initialCapacity, Allocator allocator, NativeArrayOptions nativeArrayOptions, out NativeArray<ulong> versionArray)
        {
            DenseArray = new NativeArray<int>(initialCapacity, allocator, nativeArrayOptions);
            SparseArray = new NativeArray<int>(initialCapacity, allocator, nativeArrayOptions);
            versionArray = new NativeArray<ulong>(initialCapacity, allocator, nativeArrayOptions);
            Count = 0;
            FreeIndex = 0;

            for (int i = 0; i < initialCapacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
                versionArray[i] = 1;
            }
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, Allocator allocator,
            NativeArrayOptions nativeArrayOptions)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                // We're out of space, the last free index points to nothing. Allocate more indices.
                needsExpansion = true;

                int newCapacity = currentCapacity + expandBy;

                NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
                newSparseArraySlice.CopyFrom(SparseArray);
                SparseArray.Dispose();
                SparseArray = newSparseArray;

                NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
                newDenseArraySlice.CopyFrom(DenseArray);
                DenseArray.Dispose();
                DenseArray = newDenseArray;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.
            denseIndex = Count;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            sparseIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, Allocator allocator,
            NativeArrayOptions nativeArrayOptions, ref NativeArray<ulong> versionArray)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                // We're out of space, the last free index points to nothing. Allocate more indices.
                needsExpansion = true;

                int newCapacity = currentCapacity + expandBy;

                NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
                newSparseArraySlice.CopyFrom(SparseArray);
                SparseArray.Dispose();
                SparseArray = newSparseArray;

                NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
                newDenseArraySlice.CopyFrom(DenseArray);
                DenseArray.Dispose();
                DenseArray = newDenseArray;

                NativeArray<ulong> newVersionArray = new NativeArray<ulong>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<ulong> newVersionArraySlice = new NativeSlice<ulong>(newVersionArray, 0, currentCapacity);
                newVersionArraySlice.CopyFrom(versionArray);
                versionArray.Dispose();
                versionArray = newVersionArray;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                    versionArray[i] = 1;
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.
            denseIndex = Count;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            sparseIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUnchecked(out int sparseIndex, out int denseIndex)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            sparseIndex = indexToClaim;
            denseIndex = Count;
            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <param name="version">Enables detection of use-after-free errors when using the sparse index as a reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUnchecked(out int sparseIndex, out int denseIndex, NativeArray<ulong> versionArray, out ulong version)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            version = versionArray[indexToClaim];
            sparseIndex = indexToClaim;
            denseIndex = Count;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index without any data validation.
        /// </summary>
        /// <param name="sparseIndex">The index to check in the sparse array.</param>
        /// <returns>The dense index at the given sparse index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexUnchecked(int sparseIndex)
        {
            return SparseArray[sparseIndex];
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the dense and sparse indices don't point to each other or if the dense index is outside the dense bounds.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        public int GetDenseIndexWithBoundsCheck(int sparseIndex)
        {
            if (sparseIndex >= 0 && sparseIndex < SparseArray.Length)
            {
                int denseIndex = SparseArray[sparseIndex];

                if (denseIndex < Count && denseIndex >= 0)
                {
                    int sparseIndexAtDenseIndex = DenseArray[denseIndex];

                    if (sparseIndex == sparseIndexAtDenseIndex)
                    {
                        return denseIndex;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the version number does not match.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        public int GetDenseIndexWithVersionCheck(int sparseIndex, ulong version, NativeArray<ulong> versionArray)
        {
            int denseIndex = SparseArray[sparseIndex];
            ulong versionAtSparseIndex = versionArray[sparseIndex];

            if (version == versionAtSparseIndex)
            {
                return denseIndex;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the given sparse index is invalid..
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        public int GetDenseIndexWithBoundsAndVersionCheck(int sparseIndex, ulong version,
            NativeArray<ulong> versionArray)
        {
            if (sparseIndex >= 0 && sparseIndex < SparseArray.Length)
            {
                int denseIndex = SparseArray[sparseIndex];
                ulong versionAtSparseIndex = versionArray[sparseIndex];

                if (versionAtSparseIndex == version && denseIndex < Count && denseIndex >= 0)
                {
                    int sparseIndexAtDenseIndex = DenseArray[denseIndex];

                    if (sparseIndex == sparseIndexAtDenseIndex)
                    {
                        return denseIndex;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        ///     Returns true if the element was successfully removed.
        ///     WARNING: Will not protect against accidentally removing twice if the index in question was recycled between Free calls.
        /// </summary>
        public bool RemoveWithBoundsCheck(ref int sparseIndexToRemove, out int dataIndexToSwapFrom, out int dataIndexToSwapTo)
        {
            dataIndexToSwapFrom = -1;
            dataIndexToSwapTo = -1;
            bool didRemove = false;
            if (sparseIndexToRemove >= 0 && sparseIndexToRemove < SparseArray.Length)
            {
                int denseIndexToRemove = SparseArray[sparseIndexToRemove];

                if (denseIndexToRemove >= 0 && denseIndexToRemove < Count)
                {
                    int sparseIndexAtDenseIndex = DenseArray[denseIndexToRemove];
                    int newLength = Count - 1;
                    int sparseIndexBeingSwapped = DenseArray[newLength];

                    if (denseIndexToRemove < Count && sparseIndexAtDenseIndex == sparseIndexToRemove)
                    {
                        didRemove = true;
                        // Swap the entry being removed with the last entry.
                        SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                        DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

                        dataIndexToSwapFrom = newLength;
                        dataIndexToSwapTo = denseIndexToRemove;

                        // Clear the dense index, for debugging purposes
                        DenseArray[newLength] = -1;

                        // Add the sparse index to the free list.
                        SparseArray[sparseIndexToRemove] = FreeIndex;
                        FreeIndex = sparseIndexToRemove;

                        Count = newLength;
                    }
                }
            }

            sparseIndexToRemove = -1;

            return didRemove;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        ///     calls.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="version">
        ///     The version number of the int used to access the sparse index. Used to guard against accessing
        ///     indices that have been removed and reused.
        /// </param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        /// <returns>True if the element was successfully removed.</returns>
        public bool RemoveWithBoundsAndVersionChecks(ref int sparseIndexToRemove, ulong version,
            NativeArray<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
            indexToSwapFrom = -1;
            indexToSwapTo = -1;
            bool didRemove = false;
            if (sparseIndexToRemove >= 0 && sparseIndexToRemove < SparseArray.Length)
            {
                ulong sparseIndexVersion = versionArray[sparseIndexToRemove];
                int denseIndexToRemove = SparseArray[sparseIndexToRemove];

                if (sparseIndexVersion == version && denseIndexToRemove >= 0 && denseIndexToRemove < Count)
                {
                    int sparseIndexAtDenseIndex = DenseArray[denseIndexToRemove];
                    int newLength = Count - 1;
                    int sparseIndexBeingSwapped = DenseArray[newLength];

                    if (denseIndexToRemove < Count && sparseIndexAtDenseIndex == sparseIndexToRemove)
                    {
                        didRemove = true;
                        versionArray[sparseIndexToRemove] = sparseIndexVersion + 1;
                        // Swap the entry being removed with the last entry.
                        SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                        DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

                        indexToSwapFrom = newLength;
                        indexToSwapTo = denseIndexToRemove;

                        // Clear the dense index, for debugging purposes
                        DenseArray[newLength] = -1;

                        // Add the sparse index to the free list.
                        SparseArray[sparseIndexToRemove] = FreeIndex;
                        FreeIndex = sparseIndexToRemove;

                        Count = newLength;
                    }
                }
            }

            sparseIndexToRemove = -1;

            return didRemove;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use and increments the version.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, NativeArray<ulong> versionArray)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        ///     Out parameters used to manage parallel data arrays.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, out int indexToSwapFrom, out int indexToSwapTo)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;

            indexToSwapTo = denseIndexToRemove;
            indexToSwapFrom = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use and increments the version.
        ///     Out parameters used to manage parallel data arrays.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, NativeArray<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;

            indexToSwapTo = denseIndexToRemove;
            indexToSwapFrom = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove)
        {
            int sparseIndexToRemove = DenseArray[denseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, NativeArray<ulong> versionArray)
        {
            int sparseIndexToRemove = DenseArray[denseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        ///     Out parameter used to manage parallel data arrays.
        /// </summary>
        /// <param name="denseIndexToRemove">The sparse index to remove.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at denseIndexToRemove.
        /// </param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, out int indexToSwapFrom)
        {
            int sparseIndexToRemove = DenseArray[denseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;

            indexToSwapFrom = newLength;
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        ///     Out parameter used to manage parallel data arrays.
        /// </summary>
        /// <param name="denseIndexToRemove">The sparse index to remove.</param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at denseIndexToRemove.
        /// </param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, NativeArray<ulong> versionArray, out int indexToSwapFrom)
        {
            int sparseIndexToRemove = DenseArray[denseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;

            indexToSwapFrom = newLength;
        }

        /// <summary>
        ///     Attempts to remove the associated sparse/dense index pair from active use and increments the version if successful.
        ///     Out parameters used to manage parallel data arrays.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="version">
        ///     The version number of the int used to access the sparse index. Used to guard against accessing
        ///     indices that have been removed and reused.
        /// </param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        /// <returns>True if the entry was valid and thus removed.</returns>
        public bool RemoveWithVersionCheck(int sparseIndexToRemove, ulong version, NativeArray<ulong> versionArray,
            out int indexToSwapFrom, out int indexToSwapTo)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            ulong versionAtSparseIndex = versionArray[sparseIndexToRemove];

            indexToSwapFrom = -1;
            indexToSwapTo = -1;

            bool succeeded = versionAtSparseIndex == version;

            if (succeeded)
            {
                int newLength = Count - 1;
                int sparseIndexBeingSwapped = DenseArray[newLength];

                // Swap the entry being removed with the last entry.
                SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

                // Clear the dense  index, for debugging purposes
                DenseArray[newLength] = -1;

                // Add the sparse index to the free list.
                SparseArray[sparseIndexToRemove] = FreeIndex;
                versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
                FreeIndex = sparseIndexToRemove;

                Count = newLength;

                indexToSwapTo = denseIndexToRemove;
                indexToSwapFrom = newLength;
            }

            return succeeded;
        }

        /// <summary>
        ///     Clear the dense and sparse arrays.
        /// </summary>
        public void Clear()
        {
            int capacity = SparseArray.Length;
            for (int i = 0; i < capacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
            }

            FreeIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Clear the dense and sparse arrays.
        /// </summary>
        /// <param name="versionArray">Array containing version numbers to check against.</param>
        public void Clear(NativeArray<ulong> versionArray)
        {
            int capacity = SparseArray.Length;
            for (int i = 0; i < capacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
                versionArray[i] = versionArray[i] + 1;
            }

            FreeIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Clear the dense and sparse arrays and reset the version array.
        ///     Note: Only clear the version array if you are sure there are no outstanding dependencies on version numbers.
        /// </summary>
        /// ///
        /// <param name="versionArray">Array containing version numbers to check against.</param>
        public void ClearWithVersionArrayReset(NativeArray<ulong> versionArray)
        {
            int capacity = SparseArray.Length;
            for (int i = 0; i < capacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
                versionArray[i] = 1;
            }

            FreeIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void Expand(int extraCapacity, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int currentCapacity = SparseArray.Length;
            int newCapacity = currentCapacity + extraCapacity;

            NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
            newSparseArraySlice.CopyFrom(SparseArray);
            SparseArray.Dispose();
            SparseArray = newSparseArray;

            NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
            newDenseArraySlice.CopyFrom(DenseArray);
            DenseArray.Dispose();
            DenseArray = newDenseArray;

            for (int i = currentCapacity; i < newCapacity; i++)
            {
                DenseArray[i] = -1; // Set new dense indices as unclaimed.
                SparseArray[i] = i + 1; // Build the free list chain.
            }
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        /// <param name="versionArray">Array containing version numbers to check against.</param>
        public void Expand(int extraCapacity, Allocator allocator, NativeArrayOptions nativeArrayOptions, ref NativeArray<ulong> versionArray)
        {
            int currentCapacity = SparseArray.Length;
            int newCapacity = currentCapacity + extraCapacity;

            NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
            newSparseArraySlice.CopyFrom(SparseArray);
            SparseArray.Dispose();
            SparseArray = newSparseArray;

            NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
            newDenseArraySlice.CopyFrom(DenseArray);
            DenseArray.Dispose();
            DenseArray = newDenseArray;

            NativeArray<ulong> newVersionArray = new NativeArray<ulong>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<ulong> newVersionArraySlice = new NativeSlice<ulong>(newVersionArray, 0, currentCapacity);
            newVersionArraySlice.CopyFrom(versionArray);
            versionArray.Dispose();
            versionArray = newVersionArray;

            for (int i = currentCapacity; i < newCapacity; i++)
            {
                DenseArray[i] = -1; // Set new dense indices as unclaimed.
                SparseArray[i] = i + 1; // Build the free list chain.
                versionArray[i] = 1;
            }
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        /// <param name="versionArray">Array containing version numbers to check against.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void Expand(int extraCapacity, ref NativeArray<ulong> versionArray, Allocator allocator,
            NativeArrayOptions nativeArrayOptions)
        {
            int currentCapacity = SparseArray.Length;
            int newCapacity = currentCapacity + extraCapacity;

            NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
            newSparseArraySlice.CopyFrom(SparseArray);
            SparseArray.Dispose();
            SparseArray = newSparseArray;

            NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
            newDenseArraySlice.CopyFrom(DenseArray);
            DenseArray.Dispose();
            DenseArray = newDenseArray;

            NativeArray<ulong> newVersionArray = new NativeArray<ulong>(newCapacity, allocator, nativeArrayOptions);
            NativeSlice<ulong> newVersionArraySlice = new NativeSlice<ulong>(newVersionArray, 0, currentCapacity);
            newVersionArraySlice.CopyFrom(versionArray);
            versionArray.Dispose();
            versionArray = newVersionArray;

            for (int i = currentCapacity; i < newCapacity; i++)
            {
                DenseArray[i] = -1; // Set new dense indices as unclaimed.
                SparseArray[i] = i + 1; // Build the free list chain.
                versionArray[i] = 1;
            }
        }

        public void Reserve(int numberToReserve, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int currentCapacity = SparseArray.Length;
            int currentCount = Count;
            int newCapacity = currentCount + numberToReserve;

            if (newCapacity > currentCapacity)
            {
                NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
                newSparseArraySlice.CopyFrom(SparseArray);
                SparseArray.Dispose();
                SparseArray = newSparseArray;

                NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
                newDenseArraySlice.CopyFrom(DenseArray);
                DenseArray.Dispose();
                DenseArray = newDenseArray;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                    SparseArray[i] = i + 1; // Build the free list chain.
                }
            }
        }

        public void Reserve(int numberToReserve, ref NativeArray<ulong> versionArray, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int currentCapacity = SparseArray.Length;
            int currentCount = Count;
            int newCapacity = currentCount + numberToReserve;

            if (newCapacity > currentCapacity)
            {
                NativeArray<int> newSparseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newSparseArraySlice = new NativeSlice<int>(newSparseArray, 0, currentCapacity);
                newSparseArraySlice.CopyFrom(SparseArray);
                SparseArray.Dispose();
                SparseArray = newSparseArray;

                NativeArray<int> newDenseArray = new NativeArray<int>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<int> newDenseArraySlice = new NativeSlice<int>(newDenseArray, 0, currentCapacity);
                newDenseArraySlice.CopyFrom(DenseArray);
                DenseArray.Dispose();
                DenseArray = newDenseArray;

                NativeArray<ulong> newVersionArray = new NativeArray<ulong>(newCapacity, allocator, nativeArrayOptions);
                NativeSlice<ulong> newVersionArraySlice = new NativeSlice<ulong>(newVersionArray, 0, currentCapacity);
                newVersionArraySlice.CopyFrom(versionArray);
                versionArray.Dispose();
                versionArray = newVersionArray;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                    SparseArray[i] = i + 1; // Build the free list chain.
                    versionArray[i] = 1;
                }
            }
        }
    }
}