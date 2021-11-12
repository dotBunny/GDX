// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if GDX_BURST
using Unity.Burst;
#endif
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GDX.Jobs.ParallelFor
{
#if GDX_BURST
    [BurstCompile]
#endif
    public struct Color32CompareJob : IJobParallelFor
    {
        /// <summary>
        ///     The left-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="byte" />.
        /// </summary>
        [ReadOnly] public NativeArray<Color32> A;

        /// <summary>
        ///     The right-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="byte" />.
        /// </summary>
        [ReadOnly] public NativeArray<Color32> B;

        /// <summary>
        ///     The percent difference between the two values.
        /// </summary>
        [WriteOnly] public NativeArray<float> Percentage;

        /// <summary>
        /// Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            if (A[index].r == B[index].r &&
                A[index].g == B[index].g &&
                A[index].b == B[index].b &&
                A[index].a == B[index].a)
            {
                Percentage[index] = 1f;
            }
            else
            {
                Percentage[index] = ((A[index].r / B[index].r) + (A[index].g / B[index].g) + (A[index].b / B[index].b) +
                                     (A[index].a / B[index].a)) / 4f;
            }
        }
    }
}