// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace GDX.Jobs.ParallelFor
{
    /// <summary>
    ///     Swaps a <see cref="NativeArray{T}" /> typed as <see cref="int" /> with a another
    ///     in parallel.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="IntegerBufferSwapJob" /> relies on the <see cref="IJobParallelFor" /> which
    ///         requires UnityEngine.CoreModule.dll.
    ///     </para>
    /// </remarks>
    [BurstCompile]
    public struct IntegerBufferSwapJob : IJobParallelFor
    {
        /// <summary>
        ///     The left-hand side <see cref="NativeArray{T}" /> typed as <see cref="int" />.
        /// </summary>
        public NativeArray<int> A;

        /// <summary>
        ///     The right-hand side <see cref="NativeArray{T}" /> typed as <see cref="int" />.
        /// </summary>
        public NativeArray<int> B;

        /// <summary>
        ///     Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            (A[index], B[index]) = (B[index], A[index]);
        }
    }
}