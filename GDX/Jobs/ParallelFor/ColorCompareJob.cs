// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace GDX.Jobs.ParallelFor
{
    [BurstCompile]
    public struct ColorCompareJob : IJobParallelFor
    {
        /// <summary>
        ///     The left-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="byte" />.
        /// </summary>
        [ReadOnly] public NativeArray<Color> A;

        /// <summary>
        ///     The right-hand side <see cref="Unity.Collections.NativeArray{T}" /> typed as <see cref="byte" />.
        /// </summary>
        [ReadOnly] public NativeArray<Color> B;

        /// <summary>
        ///     The percent difference between the two values.
        /// </summary>
        [WriteOnly] public NativeArray<float> Percentage;

        /// <summary>
        ///     Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            if (A[index] == B[index])
            {
                Percentage[index] = 1f;
            }
            else
            {
                float rDiff = A[index].r - B[index].r;
                float gDiff = A[index].g - B[index].g;
                float bDiff = A[index].b - B[index].b;
                float aDiff = A[index].a - B[index].a;

                Percentage[index] = 1f - (rDiff + gDiff + bDiff + aDiff) / 4f;
            }
        }
    }
}