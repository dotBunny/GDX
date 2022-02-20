// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Jobs.ParallelFor
{
    /// <summary>
    ///     Fills a <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" /> with a value in
    ///     parallel.
    /// </summary>
    [BurstCompile]
    // ReSharper disable once UnusedType.Global
    public struct IntegerBufferFillJob : IJobParallelFor
    {
        /// <summary>
        ///     <para>The <see cref="Unity.Collections.NativeArray{T}" /> which is going to be filled.</para>
        /// </summary>
        /// <remarks>Write-only.</remarks>
        [WriteOnly] public NativeArray<int> Buffer;

        /// <summary>
        ///     <para>The <see cref="System.Int32" /> value to fill the native array with.</para>
        /// </summary>
        /// <remarks>Read-only.</remarks>
        // ReSharper disable once UnassignedField.Global
        [ReadOnly] public int FillValue;

        /// <summary>
        /// Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            Buffer[index] = FillValue;
        }
    }
}