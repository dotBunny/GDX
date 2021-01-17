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
    ///     Fills a <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="System.Int32" /> with a value in
    ///     parallel.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="IntegerBufferFillJob" /> relies on the <see cref="IJobParallelFor" /> which
    ///         requires UnityEngine.CoreModule.dll.
    ///     </para>
    /// </remarks>
#if GDX_BURST
    [BurstCompile]
#endif
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

        /// <inheritdoc />
        public void Execute(int index)
        {
            Buffer[index] = FillValue;
        }
    }
}
#endif