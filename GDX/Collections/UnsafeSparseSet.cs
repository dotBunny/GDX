#if GDX_UNSAFE_COLLECTIONS
// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Burst;

namespace GDX.Collections
{
    /// <summary>
    ///     An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by
    ///     handle, as well as array-like iteration.
    /// </summary>
    [DebuggerDisplay("Count = {Count}, Length = {Length}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
    [DebuggerTypeProxy(typeof(UnsafeSparseSetDebugView))]
    [StructLayout(LayoutKind.Sequential)]
    [BurstCompatible(GenericTypeArguments = new[] { typeof(int) })]
    public unsafe struct UnsafeSparseSet : INativeDisposable
    {
        public const int MinimumCapacity = JobsUtility.CacheLineSize / (sizeof(int) * 2);

        [NativeDisableUnsafePtrRestriction]
        public void* Data;
        /// <summary>
        ///     Holds references to the sparse array for swapping indices.
        /// </summary>
        public int* DenseArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int*)Data + Length;
        }

        /// <summary>
        ///     Holds references to dense array indices.
        /// </summary>
        /// <remarks>
        ///     Its own indices are claimed and freed via a free-list.
        /// </remarks>
        public int* SparseArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (int*)Data;
        }

        /// <summary>
        ///     How many indices are being used currently?
        /// </summary>
        public int Count;

        /// <summary>
        ///     The first free (currently unused) index in the sparse array.
        /// </summary>
        public int FreeIndex;

        /// <summary>
        ///     The current capacity of both the sparse and dense arrays.
        /// </summary>
        public int Length;

        public AllocatorManager.AllocatorHandle Allocator;

        /// <summary>
        /// Whether this Sparse Set has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this list has been allocated (and not yet deallocated).</value>
        public bool IsCreated { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return Data != null; } }

        /// <summary>
        /// Whether the Sparse Set is empty.
        /// </summary>
        /// <value>True if the Sparse Set is empty or has not been constructed.</value>
        public bool IsEmpty { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return !IsCreated || Length == 0; } }

        /// <summary>
        ///     Create an <see cref="UnsafeSparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        /// <param name="allocator">The <see cref="AllocatorManager.AllocatorHandle" /> type to use.</param>
        public UnsafeSparseSet(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
        {
            initialCapacity = initialCapacity > MinimumCapacity ? initialCapacity : MinimumCapacity;
            --initialCapacity;
            initialCapacity |= initialCapacity >> 1;
            initialCapacity |= initialCapacity >> 2;
            initialCapacity |= initialCapacity >> 4;
            initialCapacity |= initialCapacity >> 8;
            initialCapacity |= initialCapacity >> 16;
            ++initialCapacity;

            Data = allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, initialCapacity * 2);
            Count = 0;
            FreeIndex = 0;
            Length = initialCapacity;
            Allocator = allocator;

            for (int i = 0; i < initialCapacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
            }
        }

        /// <summary>
        ///     Create an <see cref="UnsafeSparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public UnsafeSparseSet(int initialCapacity, AllocatorManager.AllocatorHandle allocator, out ulong* versionArray)
        {
            initialCapacity = initialCapacity > MinimumCapacity ? initialCapacity : MinimumCapacity;
            --initialCapacity;
            initialCapacity |= initialCapacity >> 1;
            initialCapacity |= initialCapacity >> 2;
            initialCapacity |= initialCapacity >> 4;
            initialCapacity |= initialCapacity >> 8;
            initialCapacity |= initialCapacity >> 16;
            ++initialCapacity;

            Data = allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, initialCapacity * 2);
            Count = 0;
            FreeIndex = 0;
            Length = initialCapacity;
            Allocator = allocator;
            versionArray = (ulong*)allocator.Allocate(sizeof(ulong), JobsUtility.CacheLineSize, initialCapacity);

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
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                // We're out of space, the last free index points to nothing. Allocate more indices.
                needsExpansion = true;

                int newCapacity = currentCapacity + expandBy;

                newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
                --newCapacity;
                newCapacity |= newCapacity >> 1;
                newCapacity |= newCapacity >> 2;
                newCapacity |= newCapacity >> 4;
                newCapacity |= newCapacity >> 8;
                newCapacity |= newCapacity >> 16;
                ++newCapacity;

                void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

                UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
                int* newSparseArray = (int*)newData;
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newSparseArray[i] = i + 1;
                }

                int* newDenseArray = newSparseArray + newCapacity;
                UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newDenseArray[i] = -1;
                }

                Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);

                Data = newData;
                Length = newCapacity;
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, ref ulong* versionArray)
        {
            int indexToClaim = FreeIndex;
            int currentCapacity = Length;
            bool needsExpansion = false;

            if (indexToClaim >= currentCapacity)
            {
                // We're out of space, the last free index points to nothing. Allocate more indices.
                needsExpansion = true;

                int newCapacity = currentCapacity + expandBy;

                newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
                --newCapacity;
                newCapacity |= newCapacity >> 1;
                newCapacity |= newCapacity >> 2;
                newCapacity |= newCapacity >> 4;
                newCapacity |= newCapacity >> 8;
                newCapacity |= newCapacity >> 16;
                ++newCapacity;

                void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

                UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
                int* newSparseArray = (int*)newData;
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newSparseArray[i] = i + 1;
                }

                int* newDenseArray = newSparseArray + newCapacity;
                UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newDenseArray[i] = -1;
                }

                ulong* newVersionArray = (ulong*)Allocator.Allocate(sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
                UnsafeUtility.MemCpy(newVersionArray, versionArray, sizeof(ulong) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newVersionArray[i] = 1;
                }

                Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);
                Allocator.Free(versionArray, sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
                versionArray = newVersionArray;

                Data = newData;
                Length = newCapacity;
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
        public void AddUnchecked(out int sparseIndex, out int denseIndex, ulong* versionArray, out ulong version)
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
            if (sparseIndex >= 0 && sparseIndex < Length)
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
        public int GetDenseIndexWithVersionCheck(int sparseIndex, ulong version, ulong* versionArray)
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
        public int GetDenseIndexWithBoundsAndVersionCheck(int sparseIndex, ulong version, ulong* versionArray)
        {
            if (sparseIndex >= 0 && sparseIndex < Length)
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
        ///     Removes the entry corresponding to the sparse index if the entry is within bounds and currently in use.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index corresponding to the entry to remove. Cleared to -1 in this operation.</param>
        /// <param name="dataIndexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        /// <param name="dataIndexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <returns>True if the index reference was valid, and thus removed.</returns>
        public bool RemoveWithBoundsCheck(ref int sparseIndexToRemove, out int dataIndexToSwapFrom, out int dataIndexToSwapTo)
        {
            dataIndexToSwapFrom = -1;
            dataIndexToSwapTo = -1;
            bool didRemove = false;
            if (sparseIndexToRemove >= 0 && sparseIndexToRemove < Length)
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
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <returns>True if the element was successfully removed.</returns>
        public bool RemoveWithBoundsAndVersionChecks(ref int sparseIndexToRemove, ulong version, ulong* versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
            indexToSwapFrom = -1;
            indexToSwapTo = -1;
            bool didRemove = false;
            if (sparseIndexToRemove >= 0 && sparseIndexToRemove < Length)
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, ulong* versionArray)
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
            versionArray[sparseIndexToRemove] += 1;
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, ulong* versionArray, out int indexToSwapFrom, out int indexToSwapTo)
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
            versionArray[sparseIndexToRemove] += 1;
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong* versionArray)
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
            versionArray[sparseIndexToRemove] += 1;
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at denseIndexToRemove.
        /// </param>
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong* versionArray, out int indexToSwapFrom)
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
            versionArray[sparseIndexToRemove] += 1;
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
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <param name="indexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <param name="indexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        /// <returns>True if the entry was valid and thus removed.</returns>
        public bool RemoveWithVersionCheck(int sparseIndexToRemove, ulong version, ulong* versionArray, out int indexToSwapFrom, out int indexToSwapTo)
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
                versionArray[sparseIndexToRemove] += 1;
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
            int capacity = Length;
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
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public void Clear(ulong* versionArray)
        {
            int capacity = Length;
            for (int i = 0; i < capacity; i++)
            {
                DenseArray[i] = -1;
                SparseArray[i] = i + 1;
                versionArray[i] += 1;
            }

            FreeIndex = 0;
            Count = 0;
        }

        /// <summary>
        ///     Clear the dense and sparse arrays and reset the version array.
        ///     Note: Only clear the version array if you are sure there are no outstanding dependencies on version numbers.
        /// </summary>
        /// ///
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public void ClearWithVersionArrayReset(ulong* versionArray)
        {
            int capacity = Length;
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
        public void Expand(int extraCapacity)
        {
            int currentCapacity = Length;
            int newCapacity = currentCapacity + extraCapacity;

            newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
            --newCapacity;
            newCapacity |= newCapacity >> 1;
            newCapacity |= newCapacity >> 2;
            newCapacity |= newCapacity >> 4;
            newCapacity |= newCapacity >> 8;
            newCapacity |= newCapacity >> 16;
            ++newCapacity;

            void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

            UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
            int* newSparseArray = (int*)newData;
            for (int i = currentCapacity; i < newCapacity; i++)
            {
                newSparseArray[i] = i + 1;
            }

            int* newDenseArray = newSparseArray + newCapacity;
            UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
            for (int i = currentCapacity; i < newCapacity; i++)
            {
                newDenseArray[i] = -1;
            }

            Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);

            Data = newData;
            Length = newCapacity;
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public void Expand(int extraCapacity, ref ulong* versionArray)
        {
            int currentCapacity = Length;
            int newCapacity = currentCapacity + extraCapacity;

            newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
            --newCapacity;
            newCapacity |= newCapacity >> 1;
            newCapacity |= newCapacity >> 2;
            newCapacity |= newCapacity >> 4;
            newCapacity |= newCapacity >> 8;
            newCapacity |= newCapacity >> 16;
            ++newCapacity;

            void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

            UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
            int* newSparseArray = (int*)newData;
            for (int i = currentCapacity; i < newCapacity; i++)
            {
                newSparseArray[i] = i + 1;
            }

            int* newDenseArray = newSparseArray + newCapacity;
            UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
            for (int i = currentCapacity; i < newCapacity; i++)
            {
                newDenseArray[i] = -1;
            }

            ulong* newVersionArray = (ulong*)Allocator.Allocate(sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
            UnsafeUtility.MemCpy(newVersionArray, versionArray, sizeof(ulong) * currentCapacity);
            for (int i = currentCapacity; i < newCapacity; i++)
            {
                newVersionArray[i] = 1;
            }

            Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);
            Allocator.Free(versionArray, sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
            versionArray = newVersionArray;

            Data = newData;
            Length = newCapacity;
        }

        /// <summary>
        /// Reallocate the dense and sparse arrays with additional capacity if there are not at least <paramref name="numberToReserve"/> unused entries.
        /// </summary>
        /// <param name="numberToReserve">The number of unused entries to ensure capacity for.</param>
        public void Reserve(int numberToReserve)
        {
            int currentCapacity = Length;
            int currentCount = Count;
            int newCapacity = currentCount + numberToReserve;

            if (newCapacity > currentCapacity)
            {
                newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
                --newCapacity;
                newCapacity |= newCapacity >> 1;
                newCapacity |= newCapacity >> 2;
                newCapacity |= newCapacity >> 4;
                newCapacity |= newCapacity >> 8;
                newCapacity |= newCapacity >> 16;
                ++newCapacity;

                void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

                UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
                int* newSparseArray = (int*)newData;
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newSparseArray[i] = i + 1;
                }

                int* newDenseArray = newSparseArray + newCapacity;
                UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newDenseArray[i] = -1;
                }

                Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);

                Data = newData;
                Length = newCapacity;
            }
        }

        /// <summary>
        /// Reallocate the dense and sparse arrays with additional capacity if there are not at least <paramref name="numberToReserve"/> unused entries.
        /// </summary>
        /// <param name="numberToReserve">The number of unused entries to ensure capacity for.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        public void Reserve(int numberToReserve,  ref ulong* versionArray)
        {
            int currentCapacity = Length;
            int currentCount = Count;
            int newCapacity = currentCount + numberToReserve;

            if (newCapacity > currentCapacity)
            {
                newCapacity = newCapacity > MinimumCapacity ? newCapacity : MinimumCapacity;
                --newCapacity;
                newCapacity |= newCapacity >> 1;
                newCapacity |= newCapacity >> 2;
                newCapacity |= newCapacity >> 4;
                newCapacity |= newCapacity >> 8;
                newCapacity |= newCapacity >> 16;
                ++newCapacity;

                void* newData = Allocator.Allocate(sizeof(int), JobsUtility.CacheLineSize, newCapacity * 2);

                UnsafeUtility.MemCpy(newData, Data, sizeof(int) * currentCapacity);
                int* newSparseArray = (int*)newData;
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newSparseArray[i] = i + 1;
                }

                int* newDenseArray = newSparseArray + newCapacity;
                UnsafeUtility.MemCpy(newDenseArray, DenseArray, sizeof(int) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newDenseArray[i] = -1;
                }

                ulong* newVersionArray = (ulong*)Allocator.Allocate(sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
                UnsafeUtility.MemCpy(newVersionArray, versionArray, sizeof(ulong) * currentCapacity);
                for (int i = currentCapacity; i < newCapacity; i++)
                {
                    newVersionArray[i] = 1;
                }

                Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, currentCapacity * 2);
                Allocator.Free(versionArray, sizeof(ulong), JobsUtility.CacheLineSize, currentCapacity);
                versionArray = newVersionArray;

                Data = newData;
                Length = newCapacity;
            }
        }

        /// <summary>
        /// Disposes the memory of this Sparse Set.
        /// </summary>
        public void Dispose()
        {
            if (Data != null)
            {
                Allocator.Free(Data, sizeof(int), JobsUtility.CacheLineSize, Length * 2);
                Data = null;
                Length = 0;
                Count = 0;
                FreeIndex = 0;
                Allocator = Unity.Collections.Allocator.Invalid;
            }
        }

        /// <summary>
        /// Creates and schedules a job that disposes the memory of this Sparse Set.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and frees the memory of this Sparse Set.</returns>
        [NotBurstCompatible /* This is not burst compatible because of IJob's use of a static IntPtr. Should switch to IJobBurstSchedulable in the future */]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            if (CollectionHelper.ShouldDeallocate(Allocator))
            {
                var jobHandle = new DisposeUnsafeSparseSetJob { Ptr = Data, Capacity = Length, Allocator = Allocator }.Schedule(inputDeps);

                Data = null;
                Length = 0;
                Count = 0;
                FreeIndex = 0;
                Allocator = Unity.Collections.Allocator.Invalid;

                return jobHandle;
            }

            Data = null;
            Length = 0;
            Count = 0;
            FreeIndex = 0;
            Allocator = Unity.Collections.Allocator.Invalid;

            return inputDeps;
        }

        /// <summary>
        /// Creates and schedules a job that disposes the memory of this Sparse Set.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and frees the memory of this Sparse Set.</returns>
        [NotBurstCompatible /* This is not burst compatible because of IJob's use of a static IntPtr. Should switch to IJobBurstSchedulable in the future */]
        public JobHandle Dispose(JobHandle inputDeps, ref ulong* versionArray)
        {
            if (CollectionHelper.ShouldDeallocate(Allocator))
            {
                var jobHandle = new DisposeUnsafeSparseSetAndVersionArrayJob { Ptr = Data, VersionArrayPtr = versionArray, Capacity = Length, Allocator = Allocator }.Schedule(inputDeps);

                Data = null;
                Length = 0;
                Count = 0;
                FreeIndex = 0;
                Allocator = Unity.Collections.Allocator.Invalid;
                versionArray = null;

                return jobHandle;
            }

            Data = null;
            Length = 0;
            Count = 0;
            FreeIndex = 0;
            Allocator = Unity.Collections.Allocator.Invalid;
            versionArray = null;

            return inputDeps;
        }

        /// <summary>
        /// Disposes the memory of the version array for this Sparse Set.
        /// </summary>
        /// <param name="versionArray">The pointer of the versionArray to dispose of.</param>
        public void DisposeVersionArray(ref ulong* versionArray)
        {
            Allocator.Free(versionArray, sizeof(ulong), JobsUtility.CacheLineSize, Length);
            versionArray = null;
        }

        /// <summary>
        /// Creates and schedules a job that disposes the memory of this Sparse Set.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and disposes the memory of this Sparse Set.</returns>
        [NotBurstCompatible /* This is not burst compatible because of IJob's use of a static IntPtr. Should switch to IJobBurstSchedulable in the future */]
        public JobHandle DisposeVersionArray(JobHandle inputDeps, ref ulong* versionArray)
        {
            if (CollectionHelper.ShouldDeallocate(Allocator))
            {
                var jobHandle = new DisposeUnsafeVersionArrayJob { Ptr = versionArray, Capacity = Length, Allocator = Allocator }.Schedule(inputDeps);

                versionArray = null;

                return jobHandle;
            }

            versionArray = null;

            return inputDeps;
        }
    }

    [BurstCompile]
    internal unsafe struct DisposeUnsafeSparseSetJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public void* Ptr;
        public int Capacity;
        public AllocatorManager.AllocatorHandle Allocator;

        public void Execute()
        {
            Allocator.Free(Ptr, UnsafeUtility.SizeOf<int>(), JobsUtility.CacheLineSize, Capacity * 2);
        }
    }

    [BurstCompile]
    internal unsafe struct DisposeUnsafeVersionArrayJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public void* Ptr;
        public int Capacity;
        public AllocatorManager.AllocatorHandle Allocator;

        public void Execute()
        {
            Allocator.Free(Ptr, UnsafeUtility.SizeOf<ulong>(), JobsUtility.CacheLineSize, Capacity);
        }
    }

    [BurstCompile]
    internal unsafe struct DisposeUnsafeSparseSetAndVersionArrayJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public void* Ptr;
        [NativeDisableUnsafePtrRestriction]
        public void* VersionArrayPtr;
        public int Capacity;
        public AllocatorManager.AllocatorHandle Allocator;

        public void Execute()
        {
            Allocator.Free(Ptr, UnsafeUtility.SizeOf<int>(), JobsUtility.CacheLineSize, Capacity * 2);
            Allocator.Free(VersionArrayPtr, UnsafeUtility.SizeOf<ulong>(), JobsUtility.CacheLineSize, Capacity);
        }
    }

    public struct SparseDenseIndexPair
    {
        public int SparseIndex;
        public int DenseIndex;
    }

    internal sealed class UnsafeSparseSetDebugView
    {
        UnsafeSparseSet Data;

        public UnsafeSparseSetDebugView(UnsafeSparseSet data)
        {
            Data = data;
        }

        public unsafe SparseDenseIndexPair[] Items
        {
            get
            {
                SparseDenseIndexPair[] result = new SparseDenseIndexPair[Data.Count];

                for (var i = 0; i < Data.Count; ++i)
                {
                    result[i] = new SparseDenseIndexPair() { DenseIndex = i, SparseIndex = Data.DenseArray[i] };
                }

                return result;
            }
        }
    }
}
#endif // GDX_UNSAFE_COLLECTIONS