// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if GDX_JOBS && GDX_BURST
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

#elif GDX_JOBS
using Unity.Collections;
using Unity.Jobs;
#endif

#if GDX_JOBS
// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Jobs.ParallelFor
{
    /// <summary>
    ///     Swaps a <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" /> with a another
    ///     in parallel.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="IntegerBufferSwapJob" /> relies on the <see cref="IJobParallelFor" /> which
    ///         requires UnityEngine.CoreModule.dll.
    ///     </para>
    /// </remarks>
#if GDX_BURST
    [BurstCompile]
#endif
    public struct IntegerBufferSwapJob : IJobParallelFor
    {
        /// <summary>
        ///     The left-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" />.
        /// </summary>
        public NativeArray<int> A;

        /// <summary>
        ///     The right-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" />.
        /// </summary>
        public NativeArray<int> B;

        /// <summary>
        /// Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            int temp = A[index];
            A[index] = B[index];
            B[index] = temp;
        }
    }
}
#endif