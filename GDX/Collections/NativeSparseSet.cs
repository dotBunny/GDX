#if GDX_UNSAFE_COLLECTIONS
// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Burst;

namespace GDX.Collections
{
    /// <summary>
    ///     An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by
    ///     handle, as well as array-like iteration.
    /// </summary>
    [DebuggerDisplay("Count = {Count}, Length = {Length}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
    [DebuggerTypeProxy(typeof(NativeSparseSetDebugView))]
    [StructLayout(LayoutKind.Sequential)]
    [BurstCompatible(GenericTypeArguments = new[] { typeof(int) })]
    public unsafe struct NativeSparseSet : INativeDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        public UnsafeSparseSet* Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public AtomicSafetyHandle Safety;
        public int m_SafetyIndexHint;
        public static readonly SharedStatic<int> s_staticSafetyId = SharedStatic<int>.GetOrCreate<NativeSparseSet>();
#endif

        /// <summary>
        ///     Get the Sparse array as a NativeArray.
        /// </summary>
        public NativeArray<int> DenseArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
                return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(Data->DenseArray, Data->Length, Unity.Collections.Allocator.None);
            }
        }

        /// <summary>
        ///     Get the Dense array as a NativeArray.
        /// </summary>
        /// <remarks>
        ///     Its own indices are claimed and freed via a free-list.
        /// </remarks>
        public NativeArray<int> SparseArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
                return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(Data->SparseArray, Data->Length, Unity.Collections.Allocator.None);
            }

        }

        /// <summary>
        ///     Get the Sparse array as a NativeArray. Avoids checking the safety handle.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<int> GetDenseArrayWithoutSafetyHandleCheck()
        {
            return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(Data->DenseArray, Data->Length, Unity.Collections.Allocator.None);
        }

        /// <summary>
        ///     Get the Dense array as a NativeArray. Avoids checking the safety handle.
        /// </summary>
        /// <remarks>
        ///     Its own indices are claimed and freed via a free-list.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<int> GetSparseArrayWithoutSafetyHandleCheck()
        {
            return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(Data->SparseArray, Data->Length, Unity.Collections.Allocator.None);
        }

        /// <summary>
        ///     How many indices are being used currently?
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
                return Data->Count;
            }
        }

        /// <summary>
        ///     How many indices are being used currently? Avoids checking the safety handle.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCountWithoutSafetyHandleCheck()
        {
            return Data->Count;
        }

        /// <summary>
        ///     The first free (currently unused) index in the sparse array.
        /// </summary>
        public int FreeIndex
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
                return Data->FreeIndex;
            }
        }

        /// <summary>
        ///     The first free (currently unused) index in the sparse array. Avoids checking the safety handle.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFreeIndexWithoutSafetyHandleCheck()
        {
            return Data->FreeIndex;
        }

        /// <summary>
        ///     The current capacity of both the sparse and dense arrays.
        /// </summary>
        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
                return Data->Length;
            }
        }

        /// <summary>
        ///     The current capacity of both the sparse and dense arrays. Avoid checking the safety handle.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetLengthWithoutSafetyHandleCheck()
        {
            return Data->Length;
        }

        /// <summary>
        ///     The allocator used by the Sparse Set.
        /// </summary>
        public AllocatorManager.AllocatorHandle Allocator;

        /// <summary>
        ///     The allocator used by the Sparse Set. Avoid checking the safety handle.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AllocatorManager.AllocatorHandle GetAllocatorWithoutSafetyHandleCheck()
        {
            return Data->Allocator;
        }

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

        public void CheckExists()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckExistsAndThrow(Safety);
#endif

            if (!IsCreated)
                throw new NullReferenceException("Sparse Set has not been allocated.");
        }

        public void CheckExistsAndCanWrite()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif

            if (!IsCreated)
                throw new NullReferenceException("Sparse Set has not been allocated.");
        }

        public void CheckExistsAndCanRead()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif

            if (!IsCreated)
                throw new NullReferenceException("Sparse Set has not been allocated.");
        }

        /// <summary>
        ///     Create a <see cref="NativeSparseSet" /> with an <paramref name="initialCapacity" />.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the sparse and dense int arrays.</param>
        /// <param name="allocator">The <see cref="AllocatorManager.AllocatorHandle" /> type to use.</param>
        public NativeSparseSet(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CollectionHelper.CheckAllocator(allocator.Handle);
            CheckInitialCapacity(initialCapacity);
            Safety = CollectionHelper.CreateSafetyHandle(allocator.Handle);
            CollectionHelper.SetStaticSafetyId<NativeSparseSet>(ref Safety, ref s_staticSafetyId.Data);
            m_SafetyIndexHint = (allocator.Handle).AddSafetyHandle(Safety);
#endif

            Allocator = allocator;
            Data = allocator.Allocate(default(UnsafeSparseSet), 1);
            ref UnsafeSparseSet dataAsRef = ref *Data;
            dataAsRef = new UnsafeSparseSet(initialCapacity, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.SetBumpSecondaryVersionOnScheduleWrite(Safety, true);
#endif
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckInitialCapacity(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new System.ArgumentOutOfRangeException(nameof(initialCapacity), "Capacity must be >= 0");
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <returns>True if the index pool expanded.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            return Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary. Avoids checking the safety handle.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <returns>True if the index pool expanded.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AddWithExpandCheckWithoutSafetyHandleCheck(int expandBy, out int sparseIndex, out int denseIndex)
        {
            return Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            bool returnValue = Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);

            if (returnValue)
                versionArray.Capacity = Data->Length;

            return returnValue;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary. Avoids checking the safety handle.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheckWithoutSafetyHandleCheck(int expandBy, out int sparseIndex, out int denseIndex, ref UnsafeList<ulong> versionArray)
        {
            bool returnValue = Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);

            if (returnValue)
                versionArray.Capacity = Data->Length;

            return returnValue;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <param name="version">The version number of the allocated sparse index at the time of allocation.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheck(int expandBy, out int sparseIndex, out int denseIndex, ref UnsafeList<ulong> versionArray, out ulong version)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            bool returnValue = Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);

            if (returnValue)
                versionArray.Capacity = Data->Length;

            version = versionArray[sparseIndex];

            return returnValue;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set and expands the arrays if necessary. Avoids checking the safety handle.
        /// </summary>
        /// <param name="expandBy">How many indices to expand by.</param>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        /// <param name="version">The version number of the allocated sparse index at the time of allocation.</param>
        /// <returns>True if the index pool expanded.</returns>
        public bool AddWithExpandCheckWithoutSafetyHandleCheck(int expandBy, out int sparseIndex, out int denseIndex, ref UnsafeList<ulong> versionArray, out ulong version)
        {
            bool returnValue = Data->AddWithExpandCheck(expandBy, out sparseIndex, out denseIndex);

            if (returnValue)
                versionArray.Capacity = Data->Length;

            version = versionArray[sparseIndex];

            return returnValue;
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUnchecked(out int sparseIndex, out int denseIndex)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->AddUnchecked(out sparseIndex, out denseIndex);
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUncheckedWithoutSafetyHandleCheck(out int sparseIndex, out int denseIndex)
        {
            Data->AddUnchecked(out sparseIndex, out denseIndex);
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <param name="version">Enables detection of use-after-free errors when using the sparse index as a reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUnchecked(out int sparseIndex, out int denseIndex, in UnsafeList<ulong> versionArray, out ulong version)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->AddUnchecked(out sparseIndex, out denseIndex);

            version = versionArray.Ptr[sparseIndex];
        }

        /// <summary>
        ///     Adds a sparse/dense index pair to the set without checking if the set needs to expand. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The sparse index allocated.</param>
        /// <param name="denseIndex">The dense index allocated.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <param name="version">Enables detection of use-after-free errors when using the sparse index as a reference.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddUncheckedWithoutSafetyHandleCheck(out int sparseIndex, out int denseIndex, in UnsafeList<ulong> versionArray, out ulong version)
        {
            Data->AddUnchecked(out sparseIndex, out denseIndex);

            version = versionArray.Ptr[sparseIndex];
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index without any data validation.
        /// </summary>
        /// <param name="sparseIndex">The index to check in the sparse array.</param>
        /// <returns>The dense index at the given sparse index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexUnchecked(int sparseIndex)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
            return Data->SparseArray[sparseIndex];
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index without any data validation. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The index to check in the sparse array.</param>
        /// <returns>The dense index at the given sparse index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexUncheckedWithoutSafetyHandleCheck(int sparseIndex)
        {
            return Data->SparseArray[sparseIndex];
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the dense and sparse indices don't point to each other or if the dense index is outside the dense bounds.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithBoundsCheck(int sparseIndex)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
            return Data->GetDenseIndexWithBoundsCheck(sparseIndex);
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the dense and sparse indices don't point to each other or if the dense index is outside the dense bounds. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithBoundsCheckWithoutSafetyHandleCheck(int sparseIndex)
        {
            return Data->GetDenseIndexWithBoundsCheck(sparseIndex);
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the version number does not match.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithVersionCheck(int sparseIndex, ulong version, in NativeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
            if (sparseIndex >= versionArray.Length)
                return -1;

            return Data->GetDenseIndexWithVersionCheck(sparseIndex, version, (ulong*)versionArray.GetUnsafeReadOnlyPtr());
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the version number does not match. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithVersionCheckWithoutSafetyHandleCheck(int sparseIndex, ulong version, in UnsafeList<ulong> versionArray)
        {
            if (sparseIndex >= versionArray.Length)
                return -1;

            return Data->GetDenseIndexWithVersionCheck(sparseIndex, version, versionArray.Ptr);
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the given sparse index is invalid.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithBoundsAndVersionCheck(int sparseIndex, ulong version, in NativeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(Safety);
#endif
            if (sparseIndex >= versionArray.Length)
                return -1;

            return Data->GetDenseIndexWithBoundsAndVersionCheck(sparseIndex, version, (ulong*)versionArray.GetUnsafeReadOnlyPtr());
        }

        /// <summary>
        ///     Gets the value of the sparse array at the given index,
        ///     or -1 if the given sparse index is invalid. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndex">The index in the sparse array to check against.</param>
        /// <param name="version">The version number associated with the sparse index.</param>
        /// <param name="versionArray">The array containing the version number to check against.</param>
        /// <returns>The dense index pointed to by the current sparse index, or -1 if invalid.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetDenseIndexWithBoundsAndVersionCheckWithoutSafetyHandleCheck(int sparseIndex, ulong version, in UnsafeList<ulong> versionArray)
        {
            if (sparseIndex >= versionArray.Length)
                return -1;

            return Data->GetDenseIndexWithBoundsAndVersionCheck(sparseIndex, version, versionArray.Ptr);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveWithBoundsCheck(ref int sparseIndexToRemove, out int dataIndexToSwapFrom, out int dataIndexToSwapTo)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            return Data->RemoveWithBoundsCheck(ref sparseIndexToRemove, out dataIndexToSwapFrom, out dataIndexToSwapTo);
        }

        /// <summary>
        ///     Removes the entry corresponding to the sparse index if the entry is within bounds and currently in use. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index corresponding to the entry to remove. Cleared to -1 in this operation.</param>
        /// <param name="dataIndexToSwapFrom">
        ///     Set the data array value at this index to default after swapping with the data array
        ///     value at indexToSwapTo.
        /// </param>
        /// <param name="dataIndexToSwapTo">Replace the data array value at this index with the data array value at indexToSwapFrom.</param>
        /// <returns>True if the index reference was valid, and thus removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveWithBoundsCheckWithoutSafetyHandleCheck(ref int sparseIndexToRemove, out int dataIndexToSwapFrom, out int dataIndexToSwapTo)
        {
            return Data->RemoveWithBoundsCheck(ref sparseIndexToRemove, out dataIndexToSwapFrom, out dataIndexToSwapTo);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveWithBoundsAndVersionChecks(ref int sparseIndexToRemove, ulong version, ref NativeList<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif

            if (sparseIndexToRemove >= versionArray.Length)
            {
                indexToSwapFrom = -1;
                indexToSwapTo = -1;
                return false;
            }

            return Data->RemoveWithBoundsAndVersionChecks(ref sparseIndexToRemove, version, (ulong*)versionArray.GetUnsafePtr(), out indexToSwapFrom, out indexToSwapTo);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use. Avoids checking the safety handle.
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveWithBoundsAndVersionChecksWithoutSafetyHandleCheck(ref int sparseIndexToRemove, ulong version, ref UnsafeList<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
            if (sparseIndexToRemove >= versionArray.Length)
            {
                indexToSwapFrom = -1;
                indexToSwapTo = -1;
                return false;
            }

            return Data->RemoveWithBoundsAndVersionChecks(ref sparseIndexToRemove, version, versionArray.Ptr, out indexToSwapFrom, out indexToSwapTo);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUnchecked(sparseIndexToRemove);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use. Avoids checking the safety handle.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUncheckedWithoutSafetyHandleCheck(int sparseIndexToRemove)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUnchecked(sparseIndexToRemove);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use and increments the version.
        /// </summary>
        /// <param name="sparseIndexToRemove">The sparse index to remove.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUnchecked(int sparseIndexToRemove, ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif

            Data->RemoveUnchecked(sparseIndexToRemove, versionArray.Ptr);
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
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUnchecked(sparseIndexToRemove, out indexToSwapFrom, out indexToSwapTo);
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
        public void RemoveUnchecked(int sparseIndexToRemove, ref UnsafeList<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUnchecked(sparseIndexToRemove, versionArray.Ptr, out indexToSwapFrom, out indexToSwapTo);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUncheckedFromDenseIndex(denseIndexToRemove);
        }

        /// <summary>
        ///     Removes the associated sparse/dense index pair from active use.
        /// </summary>
        /// <param name="denseIndexToRemove">The dense index associated with the sparse index to remove.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUncheckedFromDenseIndex(denseIndexToRemove, versionArray.Ptr);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, out int indexToSwapFrom)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            RemoveUncheckedFromDenseIndex(denseIndexToRemove, out indexToSwapFrom);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveUncheckedFromDenseIndex(int denseIndexToRemove, ref UnsafeList<ulong> versionArray, out int indexToSwapFrom)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->RemoveUncheckedFromDenseIndex(denseIndexToRemove, versionArray.Ptr, out indexToSwapFrom);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveWithVersionCheck(int sparseIndexToRemove, ulong version, ref UnsafeList<ulong> versionArray, out int indexToSwapFrom, out int indexToSwapTo)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            return Data->RemoveWithVersionCheck(sparseIndexToRemove, version, versionArray.Ptr, out indexToSwapFrom, out indexToSwapTo);
        }

        /// <summary>
        ///     Clear the dense and sparse arrays.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->Clear();
        }

        /// <summary>
        ///     Clear the dense and sparse arrays.
        /// </summary>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            if (versionArray.Length < Count)
                throw new ArgumentException("versionArray is smaller than the Sparse Set.");

            Data->Clear(versionArray.Ptr);
        }

        /// <summary>
        ///     Clear the dense and sparse arrays and reset the version array.
        ///     Note: Only clear the version array if you are sure there are no outstanding dependencies on version numbers.
        /// </summary>
        /// ///
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearWithVersionArrayReset(ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->ClearWithVersionArrayReset(versionArray.Ptr);
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int extraCapacity)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->Expand(extraCapacity);
        }

        /// <summary>
        ///     Reallocate the dense and sparse arrays with additional capacity.
        /// </summary>
        /// <param name="extraCapacity">How many indices to expand the dense and sparse arrays by.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int extraCapacity, ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->Expand(extraCapacity);

            versionArray.Capacity = Data->Length;
        }

        /// <summary>
        /// Reallocate the dense and sparse arrays with additional capacity if there are not at least <paramref name="numberToReserve"/> unused entries.
        /// </summary>
        /// <param name="numberToReserve">The number of unused entries to ensure capacity for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(int numberToReserve)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->Reserve(numberToReserve);
        }

        /// <summary>
        /// Reallocate the dense and sparse arrays with additional capacity if there are not at least <paramref name="numberToReserve"/> unused entries.
        /// </summary>
        /// <param name="numberToReserve">The number of unused entries to ensure capacity for.</param>
        /// <param name="versionArray">Enables detection of use-after-free errors when using sparse indices as references.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(int numberToReserve,  ref UnsafeList<ulong> versionArray)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(Safety);
#endif
            Data->Reserve(numberToReserve);
            versionArray.Capacity = Data->Length;
        }

        /// <summary>
        /// Disposes the memory of this Sparse Set.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CheckHandleMatches(Allocator.Handle);
            Allocator.TryRemoveSafetyHandle(Safety, m_SafetyIndexHint);
            CollectionHelper.DisposeSafetyHandle(ref Safety);
#endif

            CheckNull(Data);

            if (Data != null)
            {
                Data->Dispose();
                AllocatorManager.Free(Allocator, Data);
                Data = null;
                Allocator = Unity.Collections.Allocator.Invalid;
            }
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckHandleMatches(AllocatorManager.AllocatorHandle handle)
        {
            if(Data == null)
                throw new ArgumentOutOfRangeException($"Allocator handle {handle} can't match because container is not initialized.");
            if(Data->Allocator.Index != handle.Index)
                throw new ArgumentOutOfRangeException($"Allocator handle {handle} can't match because container handle index doesn't match.");
            if(Data->Allocator.Version != handle.Version)
                throw new ArgumentOutOfRangeException($"Allocator handle {handle} matches container handle index, but has different version.");
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        internal static void CheckNull(void* listData)
        {
            if (listData == null)
            {
                throw new Exception("UnsafeSparseSet has yet to be created or has been destroyed!");
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
                var jobHandle = new DisposeNativeSparseSetJob { UnsafeSparseSet = Data, Allocator = Allocator }.Schedule(inputDeps);

                Data = null;
                Allocator = AllocatorManager.Invalid;

                return jobHandle;
            }

            Data = null;
            Allocator = Unity.Collections.Allocator.Invalid;

            return inputDeps;
        }
    }

    [BurstCompile]
    internal unsafe struct DisposeNativeSparseSetJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public UnsafeSparseSet* UnsafeSparseSet;
        public AllocatorManager.AllocatorHandle Allocator;

        public void Execute()
        {
            UnsafeSparseSet->Dispose();
            Allocator.Free(UnsafeSparseSet, 1);
        }
    }

    sealed class NativeSparseSetDebugView
    {
        NativeSparseSet m_SparseSet;

        public NativeSparseSetDebugView(NativeSparseSet array)
        {
            m_SparseSet = array;
        }

        public SparseDenseIndexPair[] Items
        {
            get
            {
                SparseDenseIndexPair[] result = new SparseDenseIndexPair[m_SparseSet.Count];

                for (var i = 0; i < m_SparseSet.Count; ++i)
                {
                    result[i] = new SparseDenseIndexPair() { DenseIndex = i, SparseIndex = m_SparseSet.DenseArray[i] };
                }

                return result;
            }
        }
    }
}
#endif