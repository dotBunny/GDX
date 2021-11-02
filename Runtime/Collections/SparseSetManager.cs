// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

public static class SparseSetManager
{
    /// <summary>
    ///     Create a <see cref="SparseSet" /> with an <paramref name="initialCapacity" />.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
    public static void InitSparseSet(int initialCapacity, out int[] denseArray, out int[] sparseArray, out int count, out int freeIndex)
    {
        denseArray = new int[initialCapacity];
        sparseArray = new int[initialCapacity];
        count = 0;
        freeIndex = 0;

        for (int i = 0; i < initialCapacity; i++)
        {
            denseArray[i] = -1;
            sparseArray[i] = i + 1;
        }
    }

    /// <summary>
    ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
    /// </summary>
    /// <param name="expandBy">How many indices to expand by.</param>
    /// <param name="sparseIndex">The sparse index allocated.</param>
    /// <param name="denseIndex">The dense index allocated.</param>
    /// <returns>True if the index pool expanded.</returns>
    public static bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, ref int[] sparseArray, ref int[] denseArray, ref int freeIndex, ref int count)
    {
        int indexToClaim = freeIndex;
        int currentCapacity = sparseArray.Length;
        bool needsExpansion = false;

        if (indexToClaim >= currentCapacity)
        {
            // We're out of space, the last free index points to nothing. Allocate more indices.
            needsExpansion = true;

            int newCapacity = currentCapacity + expandBy;

            int[] newSparseArray = new int[newCapacity];
            Array.Copy(sparseArray, 0, newSparseArray, 0, currentCapacity);
            sparseArray = newSparseArray;

            int[] newDenseArray = new int[newCapacity];
            Array.Copy(denseArray, 0, newDenseArray, 0, currentCapacity);
            denseArray = newDenseArray;

            for (int i = currentCapacity; i < newCapacity; i++)
            {
                sparseArray[i] = i + 1; // Build the free list chain.
                denseArray[i] = -1; // Set new dense indices as unclaimed.
            }
        }

        int nextFreeIndex = sparseArray[indexToClaim];
        denseArray[count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
        sparseArray[indexToClaim] = count; // Point our newly claimed sparse index at the dense index.
        denseIndex = count;

        ++count;
        freeIndex = nextFreeIndex; // Set the free list head for next time.

        sparseIndex = indexToClaim;
        return needsExpansion;
    }

    /// <summary>
    ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand.
    /// </summary>
    /// <param name="sparseIndex">The sparse index allocated.</param>
    /// <param name="denseIndex">The dense index allocated.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddUnchecked(out int sparseIndex, out int denseIndex, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int indexToClaim = freeIndex;
        int nextFreeIndex = sparseArray[indexToClaim];
        denseArray[count] = indexToClaim; // Point the next dense id at our newly claimed sparse index.
        sparseArray[indexToClaim] = count; // Point our newly claimed sparse index at the dense index.

        sparseIndex = indexToClaim;
        denseIndex = count;
        ++count;
        freeIndex = nextFreeIndex; // Set the free list head for next time.
    }

    /// <summary>
    ///     Gets the value of the sparse array at the given index without any data validation.
    /// </summary>
    /// <param name="sparseIndex">The index to check in the sparse array.</param>
    /// <returns>The dense index at the given sparse index.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDenseIndexUnchecked(int sparseIndex, int[] sparseArray)
    {
        return sparseArray[sparseIndex];
    }

    /// <summary>
    ///     Gets the value of the sparse array at the given index,
    ///     or -1 if the dense and sparse indices don't point to each other or if the dense index is outside the dense bounds.
    /// </summary>
    /// <param name="sparseIndex">The index in the sparse array to check against.</param>
    /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
    public static int GetDenseIndexWithBoundsCheck(int sparseIndex, int[] sparseArray, int[] denseArray, ref int count)
    {
        if (sparseIndex >= 0 && sparseIndex < sparseArray.Length)
        {
            int denseIndex = sparseArray[sparseIndex];

            if (denseIndex < count && denseIndex >= 0)
            {
                int sparseIndexAtDenseIndex = denseArray[denseIndex];

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
    public static int GetDenseIndexWithVersionCheck(int sparseIndex, ulong version, ulong[] versionArray, int[] sparseArray)
    {
        int denseIndex = sparseArray[sparseIndex];
        ulong versionAtSparseIndex = versionArray[sparseIndex];

        if (version == versionAtSparseIndex)
        {
            return denseIndex;
        }

        return -1;
    }

    /// <summary>
    ///     Gets the value of the sparse array at the given index,
    ///     or -1 if the given sparse index is invalid.
    /// </summary>
    /// <param name="sparseIndex">The index in the sparse array to check against.</param>
    /// <param name="version">The version number associated with the sparse index.</param>
    /// <param name="versionArray">The array containing the version number to check against.</param>
    /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
    public static int GetDenseIndexWithBoundsAndVersionCheck(int sparseIndex, ulong version, ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int count)
    {
        if (sparseIndex >= 0 && sparseIndex < sparseArray.Length)
        {
            int denseIndex = sparseArray[sparseIndex];
            ulong versionAtSparseIndex = versionArray[sparseIndex];

            if (versionAtSparseIndex == version && denseIndex < count && denseIndex >= 0)
            {
                int sparseIndexAtDenseIndex = denseArray[denseIndex];

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
    public static bool RemoveWithNullValueCheck(ref int sparseIndexToRemove, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        bool didRemove = false;
        if (sparseIndexToRemove >= 0 && sparseIndexToRemove < sparseArray.Length)
        {
            int denseIndexToRemove = sparseArray[sparseIndexToRemove];

            if (denseIndexToRemove >= 0 && denseIndexToRemove < count)
            {
                int sparseIndexAtDenseIndex = denseArray[denseIndexToRemove];
                int newLength = count - 1;
                int sparseIndexBeingSwapped = denseArray[newLength];

                if (denseIndexToRemove < count && sparseIndexAtDenseIndex == sparseIndexToRemove)
                {
                    didRemove = true;
                    // Swap the entry being removed with the last entry.
                    sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                    denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

                    // Clear the dense index, for debugging purposes
                    denseArray[newLength] = -1;

                    // Add the sparse index to the free list.
                    sparseArray[sparseIndexToRemove] = freeIndex;
                    freeIndex = sparseIndexToRemove;

                    count = newLength;
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
    public static bool RemoveWithBoundsAndVersionChecks(ref int sparseIndexToRemove, ulong version,
        ref ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        bool didRemove = false;
        if (sparseIndexToRemove >= 0 && sparseIndexToRemove < sparseArray.Length)
        {
            ulong sparseIndexVersion = versionArray[sparseIndexToRemove];
            int denseIndexToRemove = sparseArray[sparseIndexToRemove];

            if (sparseIndexVersion == version && denseIndexToRemove >= 0 && denseIndexToRemove < count)
            {
                int sparseIndexAtDenseIndex = denseArray[denseIndexToRemove];
                int newLength = count - 1;
                int sparseIndexBeingSwapped = denseArray[newLength];

                if (denseIndexToRemove < count && sparseIndexAtDenseIndex == sparseIndexToRemove)
                {
                    didRemove = true;
                    ++sparseIndexVersion;
                    versionArray[sparseIndexToRemove] = sparseIndexVersion;
                    // Swap the entry being removed with the last entry.
                    sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
                    denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

                    // Clear the dense index, for debugging purposes
                    denseArray[newLength] = -1;

                    // Add the sparse index to the free list.
                    sparseArray[sparseIndexToRemove] = freeIndex;
                    freeIndex = sparseIndexToRemove;

                    count = newLength;
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
    public static void RemoveUnchecked(int sparseIndexToRemove, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int denseIndexToRemove = sparseArray[sparseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        freeIndex = sparseIndexToRemove;

        count = newLength;
    }

    /// <summary>
    ///     Removes the associated sparse/dense index pair from active use and increments the version.
    /// </summary>
    /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
    /// <param name="versionArray">The array where version numbers to check against are stored.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveUnchecked(int sparseIndexToRemove, ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int denseIndexToRemove = sparseArray[sparseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
        freeIndex = sparseIndexToRemove;

        count = newLength;
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
    public static void RemoveUnchecked(int sparseIndexToRemove, out int indexToSwapTo, out int indexToSwapFrom, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int denseIndexToRemove = sparseArray[sparseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        freeIndex = sparseIndexToRemove;

        count = newLength;

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
    public static void RemoveUnchecked(int sparseIndexToRemove, ulong[] versionArray, out int indexToSwapTo,
        out int indexToSwapFrom, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int denseIndexToRemove = sparseArray[sparseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
        freeIndex = sparseIndexToRemove;

        count = newLength;

        indexToSwapTo = denseIndexToRemove;
        indexToSwapFrom = newLength;
    }

    /// <summary>
    ///     Removes the associated sparse/dense index pair from active use.
    /// </summary>
    /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
    public static void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int sparseIndexToRemove = denseArray[denseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        freeIndex = sparseIndexToRemove;

        count = newLength;
    }

    /// <summary>
    ///     Removes the associated sparse/dense index pair from active use.
    /// </summary>
    /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
    /// <param name="versionArray">The array where version numbers to check against are stored.</param>
    public static void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int sparseIndexToRemove = denseArray[denseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
        freeIndex = sparseIndexToRemove;

        count = newLength;
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
    public static void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, out int indexToSwapFrom, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int sparseIndexToRemove = denseArray[denseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        freeIndex = sparseIndexToRemove;

        count = newLength;

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
    public static void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ulong[] versionArray, out int indexToSwapFrom, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int sparseIndexToRemove = denseArray[denseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        // Swap the entry being removed with the last entry.
        sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
        denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

        // Clear the dense  index, for debugging purposes
        denseArray[newLength] = -1;

        // Add the sparse index to the free list.
        sparseArray[sparseIndexToRemove] = freeIndex;
        versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
        freeIndex = sparseIndexToRemove;

        count = newLength;

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
    public static bool TryRemoveWithVersionCheck(int sparseIndexToRemove, ulong version, ulong[] versionArray,
        out int indexToSwapTo, out int indexToSwapFrom, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int denseIndexToRemove = sparseArray[sparseIndexToRemove];
        ulong versionAtSparseIndex = versionArray[sparseIndexToRemove];

        indexToSwapFrom = -1;
        indexToSwapTo = -1;

        bool succeeded = versionAtSparseIndex == version;

        if (succeeded)
        {
            int newLength = count - 1;
            int sparseIndexBeingSwapped = denseArray[newLength];

            // Swap the entry being removed with the last entry.
            sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            denseArray[newLength] = -1;

            // Add the sparse index to the free list.
            sparseArray[sparseIndexToRemove] = freeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            freeIndex = sparseIndexToRemove;

            count = newLength;

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
    public static bool TryRemoveFromDenseIndexWithVersionCheck(int denseIndexToRemove, ulong version, ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int sparseIndexToRemove = denseArray[denseIndexToRemove];
        int newLength = count - 1;
        int sparseIndexBeingSwapped = denseArray[newLength];

        ulong versionAtSparseIndex = versionArray[sparseIndexToRemove];

        bool succeeded = version == versionAtSparseIndex;
        if (succeeded)
        {
            // Swap the entry being removed with the last entry.
            sparseArray[sparseIndexBeingSwapped] = denseIndexToRemove;
            denseArray[denseIndexToRemove] = sparseIndexBeingSwapped;

            // Clear the dense  index, for debugging purposes
            denseArray[newLength] = -1;

            // Add the sparse index to the free list.
            sparseArray[sparseIndexToRemove] = freeIndex;
            versionArray[sparseIndexToRemove] = versionArray[sparseIndexToRemove] + 1;
            freeIndex = sparseIndexToRemove;

            count = newLength;
        }

        return succeeded;
    }

    /// <summary>
    ///     Clear the dense and sparse arrays.
    /// </summary>
    public static void Clear(int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int capacity = sparseArray.Length;
        for (int i = 0; i < capacity; i++)
        {
            denseArray[i] = -1;
            sparseArray[i] = i + 1;
        }

        freeIndex = 0;
        count = 0;
    }

    /// <summary>
    ///     Clear the dense and sparse arrays.
    /// </summary>
    /// ///
    /// <param name="versionArray">Array containing version numbers to check against.</param>
    public static void Clear(ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int capacity = sparseArray.Length;
        for (int i = 0; i < capacity; i++)
        {
            denseArray[i] = -1;
            sparseArray[i] = i + 1;
            versionArray[i] = 0;
        }

        freeIndex = 0;
        count = 0;
    }

    /// <summary>
    ///     Reallocate the dense and sparse arrays with additional capacity.
    /// </summary>
    /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
    public static void Expand(int extraCapacity, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int currentCapacity = sparseArray.Length;
        int newCapacity = currentCapacity + extraCapacity;

        int[] newSparseArray = new int[newCapacity];
        Array.Copy(sparseArray, 0, newSparseArray, 0, currentCapacity);
        sparseArray = newSparseArray;

        int[] newDenseArray = new int[newCapacity];
        Array.Copy(denseArray, 0, newDenseArray, 0, currentCapacity);
        denseArray = newDenseArray;

        for (int i = currentCapacity; i < newCapacity; i++)
        {
            denseArray[i] = -1; // Set new dense indices as unclaimed.
            sparseArray[i] = i + 1; // Build the free list chain.
        }
    }

    /// <summary>
    ///     Reallocate the dense and sparse arrays with additional capacity.
    /// </summary>
    /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
    /// <param name="versionArray">Array containing version numbers to check against.</param>
    public static void Expand(int extraCapacity, ref ulong[] versionArray, int[] sparseArray, int[] denseArray, ref int freeIndex, ref int count)
    {
        int currentCapacity = sparseArray.Length;
        int newCapacity = currentCapacity + extraCapacity;

        int[] newSparseArray = new int[newCapacity];
        Array.Copy(sparseArray, 0, newSparseArray, 0, currentCapacity);
        sparseArray = newSparseArray;

        int[] newDenseArray = new int[newCapacity];
        Array.Copy(denseArray, 0, newDenseArray, 0, currentCapacity);
        denseArray = newDenseArray;

        ulong[] newVersionArray = new ulong[newCapacity];
        Array.Copy(versionArray, 0, newVersionArray, 0, currentCapacity);
        versionArray = newVersionArray;

        for (int i = currentCapacity; i < newCapacity; i++)
        {
            denseArray[i] = -1; // Set new dense indices as unclaimed.
            sparseArray[i] = i + 1; // Build the free list chain.
        }
    }
}
