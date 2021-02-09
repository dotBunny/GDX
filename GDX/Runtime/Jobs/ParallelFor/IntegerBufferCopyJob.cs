// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Unity.Collections;
using Unity.Jobs;
#if GDX_BURST
using Unity.Burst;
#endif

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Jobs.ParallelFor
{
    /// <summary>
    ///     Copy one <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" /> to another in
    ///     parallel.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="IntegerBufferCopyJob" /> relies on the <see cref="IJobParallelFor" /> which
    ///         requires UnityEngine.CoreModule.dll.
    ///     </para>
    /// </remarks>
#if GDX_BURST
    [BurstCompile]
#endif
    public struct IntegerBufferCopyJob : IJobParallelFor
    {
        /// <summary>
        ///     <para>The destination <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" />.</para>
        /// </summary>
        /// <remarks>Write-only.</remarks>
        [WriteOnly] public NativeArray<int> Destination;

        /// <summary>
        ///     <para>The source <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" />.</para>
        /// </summary>
        /// <remarks>Read-only.</remarks>
        [ReadOnly] public NativeArray<int> Source;

        /// <summary>
        /// Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            Destination[index] = Source[index];
        }
    }
}