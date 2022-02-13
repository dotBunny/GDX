// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedMember.Global

namespace GDX.Collections
{
    /// <summary>
    ///     An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by
    ///     handle, as well as array-like iteration.
    /// </summary>
    public struct SparseSet
    {
        // ReSharper disable MemberCanBePrivate.Global

        /// <summary>
        ///     Holds references to the sparse array for swapping indices.
        /// </summary>
        public int[] DenseArray;

        /// <summary>
        ///     Holds references to dense array indices.
        /// </summary>
        /// <remarks>
        ///     Its own indices are claimed and freed via a free-list.
        /// </remarks>
        public int[] SparseArray;

        /// <summary>
        ///     How many indices are being used currently?
        /// </summary>
        public int Count;

        /// <summary>
        ///     The first free (currently unused) index in the sparse array.
        /// </summary>
        public int FreeIndex;

        // ReSharper enable MemberCanBePrivate.Global

        /// <summary>
        ///     Create a <see cref="SparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        public SparseSet(int initialCapacity)
        {
            DenseArray = new int[initialCapacity];
            SparseArray = new int[initialCapacity];
            Count = 0;
            FreeIndex = 0;

            for (int i = 0; i < initialCapacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
            }
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                // We're out of space, the last free index points to nothing. Allocate more indices.
                needsExpansion = true;

                int newCapacity = currentCapacity + expandBy;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
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
        public int GetDenseIndexWithVersionCheck(int sparseIndex, ulong version, ulong[] versionArray)
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
        public int GetDenseIndexWithBoundsAndVersionCheck(int sparseIndex, ulong version, ulong[] versionArray)
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
        ///     WARNING: Will not protect against accidentally removing twice if the index in question was recycled between Remove
        ///     calls.
        /// </summary>
        public bool RemoveWithNullValueCheck(ref int sparseIndexToRemove)
        {
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
            ref ulong[] versionArray)
        {
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
                        ++sparseIndexVersion;
                        versionArray[sparseIndexToRemove] = sparseIndexVersion;
                        // Swap the entry being removed with the last entry.
                        SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                        DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

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
        public void RemoveUnchecked(int sparseIndexToRemove, ulong[] versionArray)
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
        public void RemoveUnchecked(int sparseIndexToRemove, out int indexToSwapTo, out int indexToSwapFrom)
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
        public void RemoveUnchecked(int sparseIndexToRemove, ulong[] versionArray, out int indexToSwapTo,
            out int indexToSwapFrom)
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
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong[] versionArray)
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
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong[] versionArray, out int indexToSwapFrom)
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
        /// <returns>Whether or not the remove attempt succeeded.</returns>
        public bool TryRemoveWithVersionCheck(int sparseIndexToRemove, ulong version, ulong[] versionArray,
            out int indexToSwapTo, out int indexToSwapFrom)
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
        ///     Attempts to remove the associated sparse/dense index pair from active use and increments the version if successful.
        /// </summary>
        /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
        /// <param name="version">The array where version numbers to check against are stored.</param>
        /// <param name="versionArray">The array where version numbers to check against are stored.</param>
        /// <returns>Whether or not the remove attempt succeeded.</returns>
        public bool TryRemoveFromDenseIndexWithVersionCheck(int denseIndexToRemove, ulong version, ulong[] versionArray)
        {
            int sparseIndexToRemove = DenseArray[denseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            ulong versionAtSparseIndex = versionArray[sparseIndexToRemove];

            bool succeeded = version == versionAtSparseIndex;
            if (succeeded)
            {
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
        /// ///
        /// <param name="versionArray">Array containing version numbers to check against.</param>
        public void Clear(ulong[] versionArray)
        {
            int capacity = SparseArray.Length;
            for (int i = 0; i < capacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
                versionArray[i] = 0;
            }

            FreeIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        public void Expand(int extraCapacity)
        {
            int currentCapacity = SparseArray.Length;
            int newCapacity = currentCapacity + extraCapacity;

            int[] newSparseArray = new int[newCapacity];
            Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
            SparseArray = newSparseArray;

            int[] newDenseArray = new int[newCapacity];
            Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
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
        public void Expand(int extraCapacity, ref ulong[] versionArray)
        {
            int currentCapacity = SparseArray.Length;
            int newCapacity = currentCapacity + extraCapacity;

            int[] newSparseArray = new int[newCapacity];
            Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
            SparseArray = newSparseArray;

            int[] newDenseArray = new int[newCapacity];
            Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
            DenseArray = newDenseArray;

            ulong[] newVersionArray = new ulong[newCapacity];
            Array.Copy(versionArray, 0, newVersionArray, 0, currentCapacity);
            versionArray = newVersionArray;

            for (int i = currentCapacity; i < newCapacity; i++)
            {
                DenseArray[i] = -1; // Set new dense indices as unclaimed.
                SparseArray[i] = i + 1; // Build the free list chain.
            }
        }

        public void Reserve(int numberToReserve)
        {
            int currentCapacity = SparseArray.Length;
            int currentCount = Count;
            int newCount = currentCount + numberToReserve;

            if (newCount > currentCapacity)
            {
                int[] newSparseArray = new int[newCount];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCount];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                for (int i = currentCapacity; i < newCount; i++)
                {
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                    SparseArray[i] = i + 1; // Build the free list chain.
                }
            }
        }

        public void Reserve(int numberToReserve, ref ulong[] versionArray)
        {
            int currentCapacity = SparseArray.Length;
            int currentCount = Count;
            int newCount = currentCount + numberToReserve;

            if (newCount > currentCapacity)
            {
                int[] newSparseArray = new int[newCount];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCount];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                ulong[] newVersionArray = new ulong[newCount];
                Array.Copy(versionArray, 0, newVersionArray, 0, currentCapacity);
                versionArray = newVersionArray;

                for (int i = currentCapacity; i < newCount; i++)
                {
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                    SparseArray[i] = i + 1; // Build the free list chain.
                }
            }
        }

        #region Parallel array method duplicates

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0>(T0 obj0, ref T0[] array0, out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1, out int lookupIndex,
            int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1, T2 obj2,
            ref T2[] array2,
            out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2, T3>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1, T2 obj2,
            ref T2[] array2, T3 obj3, ref T3[] array3, out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                T3[] newArray3 = new T3[newCapacity];
                Array.Copy(array3, 0, newArray3, 0, currentCapacity);
                array3 = newArray3;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2, T3, T4>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1, T2 obj2,
            ref T2[] array2, T3 obj3, ref T3[] array3, T4 obj4, ref T4[] array4, out int lookupIndex,
            int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                T3[] newArray3 = new T3[newCapacity];
                Array.Copy(array3, 0, newArray3, 0, currentCapacity);
                array3 = newArray3;

                T4[] newArray4 = new T4[newCapacity];
                Array.Copy(array4, 0, newArray4, 0, currentCapacity);
                array4 = newArray4;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2, T3, T4, T5>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1,
            T2 obj2,
            ref T2[] array2, T3 obj3, ref T3[] array3, T4 obj4, ref T4[] array4, T5 obj5, ref T5[] array5,
            out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                T3[] newArray3 = new T3[newCapacity];
                Array.Copy(array3, 0, newArray3, 0, currentCapacity);
                array3 = newArray3;

                T4[] newArray4 = new T4[newCapacity];
                Array.Copy(array4, 0, newArray4, 0, currentCapacity);
                array4 = newArray4;

                T5[] newArray5 = new T5[newCapacity];
                Array.Copy(array5, 0, newArray5, 0, currentCapacity);
                array5 = newArray5;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2, T3, T4, T5, T6>(T0 obj0, ref T0[] array0, T1 obj1, ref T1[] array1,
            T2 obj2,
            ref T2[] array2, T3 obj3, ref T3[] array3, T4 obj4, ref T4[] array4, T5 obj5, ref T5[] array5, T6 obj6,
            ref T6[] array6, out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                T3[] newArray3 = new T3[newCapacity];
                Array.Copy(array3, 0, newArray3, 0, currentCapacity);
                array3 = newArray3;

                T4[] newArray4 = new T4[newCapacity];
                Array.Copy(array4, 0, newArray4, 0, currentCapacity);
                array4 = newArray4;

                T5[] newArray5 = new T5[newCapacity];
                Array.Copy(array5, 0, newArray5, 0, currentCapacity);
                array5 = newArray5;

                T6[] newArray6 = new T6[newCapacity];
                Array.Copy(array6, 0, newArray6, 0, currentCapacity);
                array6 = newArray6;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;
            array6[Count] = obj6;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck<T0, T1, T2, T3, T4, T5, T6, T7>(T0 obj0, ref T0[] array0, T1 obj1,
            ref T1[] array1, T2 obj2,
            ref T2[] array2, T3 obj3, ref T3[] array3, T4 obj4, ref T4[] array4, T5 obj5, ref T5[] array5, T6 obj6,
            ref T6[] array6, T7 obj7, ref T7[] array7, out int lookupIndex, int howMuchToExpand = 16)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = SparseArray.Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                needsExpansion = true;
                // We're out of space, the last free index points to nothing. Allocate more indices.
                int newCapacity = currentCapacity + howMuchToExpand;

                int[] newSparseArray = new int[newCapacity];
                Array.Copy(SparseArray, 0, newSparseArray, 0, currentCapacity);
                SparseArray = newSparseArray;

                int[] newDenseArray = new int[newCapacity];
                Array.Copy(DenseArray, 0, newDenseArray, 0, currentCapacity);
                DenseArray = newDenseArray;

                T0[] newArray0 = new T0[newCapacity];
                Array.Copy(array0, 0, newArray0, 0, currentCapacity);
                array0 = newArray0;

                T1[] newArray1 = new T1[newCapacity];
                Array.Copy(array1, 0, newArray1, 0, currentCapacity);
                array1 = newArray1;

                T2[] newArray2 = new T2[newCapacity];
                Array.Copy(array2, 0, newArray2, 0, currentCapacity);
                array2 = newArray2;

                T3[] newArray3 = new T3[newCapacity];
                Array.Copy(array3, 0, newArray3, 0, currentCapacity);
                array3 = newArray3;

                T4[] newArray4 = new T4[newCapacity];
                Array.Copy(array4, 0, newArray4, 0, currentCapacity);
                array4 = newArray4;

                T5[] newArray5 = new T5[newCapacity];
                Array.Copy(array5, 0, newArray5, 0, currentCapacity);
                array5 = newArray5;

                T6[] newArray6 = new T6[newCapacity];
                Array.Copy(array6, 0, newArray6, 0, currentCapacity);
                array6 = newArray6;

                T7[] newArray7 = new T7[newCapacity];
                Array.Copy(array7, 0, newArray7, 0, currentCapacity);
                array7 = newArray7;

                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    SparseArray[i] = i + 1; // Build the free list chain.
                    DenseArray[i] = -1; // Set new dense indices as unclaimed.
                }
            }

            int nextFreeIndex = SparseArray[indexToClaim];

            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;
            array6[Count] = obj6;
            array7[Count] = obj7;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            lookupIndex = indexToClaim;
            return needsExpansion;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0>(T0 obj0, T0[] array0)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1>(T0 obj0, T0[] array0, T1 obj1, T1[] array1)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2, T2[] array2)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2, T3>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2, T2[] array2,
            T3 obj3,
            T3[] array3)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2, T3, T4>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2, T2[] array2,
            T3 obj3, T3[] array3, T4 obj4, T4[] array4)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2, T3, T4, T5>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2,
            T2[] array2,
            T3 obj3, T3[] array3, T4 obj4, T4[] array4, T5 obj5, T5[] array5)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2, T3, T4, T5, T6>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2,
            T2[] array2,
            T3 obj3, T3[] array3, T4 obj4, T4[] array4, T5 obj5, T5[] array5, T6 obj6, T6[] array6)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;
            array6[Count] = obj6;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        /// <summary>
        ///     Adds to the set without checking if the set needs to expand.
        /// </summary>
        /// <returns>The sparse index allocated</returns>
        public int AddUnchecked<T0, T1, T2, T3, T4, T5, T6, T7>(T0 obj0, T0[] array0, T1 obj1, T1[] array1, T2 obj2,
            T2[] array2, T3 obj3, T3[] array3, T4 obj4, T4[] array4, T5 obj5, T5[] array5, T6 obj6, T6[] array6,
            T7 obj7, T7[] array7)
        {
            int indexToClaim = FreeIndex;
            int nextFreeIndex = SparseArray[indexToClaim];
            DenseArray[Count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
            SparseArray[indexToClaim] = Count; // Point our newly claimed sparse index at the dense index.

            array0[Count] = obj0;
            array1[Count] = obj1;
            array2[Count] = obj2;
            array3[Count] = obj3;
            array4[Count] = obj4;
            array5[Count] = obj5;
            array6[Count] = obj6;
            array7[Count] = obj7;

            ++Count;
            FreeIndex = nextFreeIndex; // Set the free list head for next time.

            return indexToClaim;
        }

        public void RemoveUnchecked<T0>(int sparseIndexToRemove, T0[] array0)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1>(int sparseIndexToRemove, T0[] array0, T1[] array1)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2>(int sparseIndexToRemove, T0[] array0, T1[] array1, T2[] array2)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2, T3>(int sparseIndexToRemove, T0[] array0, T1[] array1, T2[] array2,
            T3[] array3)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];
            array3[denseIndexToRemove] = array3[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);
            array3[newLength] = default(T3);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2, T3, T4>(int sparseIndexToRemove, T0[] array0, T1[] array1, T2[] array2,
            T3[] array3, T4[] array4)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];
            array3[denseIndexToRemove] = array3[newLength];
            array4[denseIndexToRemove] = array4[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);
            array3[newLength] = default(T3);
            array4[newLength] = default(T4);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2, T3, T4, T5>(int sparseIndexToRemove, T0[] array0, T1[] array1,
            T2[] array2,
            T3[] array3, T4[] array4, T5[] array5)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];
            array3[denseIndexToRemove] = array3[newLength];
            array4[denseIndexToRemove] = array4[newLength];
            array5[denseIndexToRemove] = array5[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);
            array3[newLength] = default(T3);
            array4[newLength] = default(T4);
            array5[newLength] = default(T5);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2, T3, T4, T5, T6>(int sparseIndexToRemove, T0[] array0, T1[] array1,
            T2[] array2, T3[] array3, T4[] array4, T5[] array5, T6[] array6)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];
            array3[denseIndexToRemove] = array3[newLength];
            array4[denseIndexToRemove] = array4[newLength];
            array5[denseIndexToRemove] = array5[newLength];
            array6[denseIndexToRemove] = array6[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);
            array3[newLength] = default(T3);
            array4[newLength] = default(T4);
            array5[newLength] = default(T5);
            array6[newLength] = default(T6);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        public void RemoveUnchecked<T0, T1, T2, T3, T4, T5, T6, T7>(int sparseIndexToRemove, T0[] array0, T1[] array1,
            T2[] array2, T3[] array3, T4[] array4, T5[] array5, T6[] array6, T7[] array7)
        {
            int denseIndexToRemove = SparseArray[sparseIndexToRemove];
            int newLength = Count - 1;
            int sparseIndexBeingSwapped = DenseArray[newLength];

            // Swap the entry being removed with the last entry.
            SparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            DenseArray[denseIndexToRemove] = sparseIndexBeingSwapped;
            array0[denseIndexToRemove] = array0[newLength];
            array1[denseIndexToRemove] = array1[newLength];
            array2[denseIndexToRemove] = array2[newLength];
            array3[denseIndexToRemove] = array3[newLength];
            array4[denseIndexToRemove] = array4[newLength];
            array5[denseIndexToRemove] = array5[newLength];
            array6[denseIndexToRemove] = array6[newLength];
            array7[denseIndexToRemove] = array7[newLength];

            // Clear the dense  index, for debugging purposes
            DenseArray[newLength] = -1;
            array0[newLength] = default(T0);
            array1[newLength] = default(T1);
            array2[newLength] = default(T2);
            array3[newLength] = default(T3);
            array4[newLength] = default(T4);
            array5[newLength] = default(T5);
            array6[newLength] = default(T6);
            array7[newLength] = default(T7);

            // Add the sparse index to the free list.
            SparseArray[sparseIndexToRemove] = FreeIndex;
            FreeIndex = sparseIndexToRemove;

            Count = newLength;
        }

        #endregion
    }
}