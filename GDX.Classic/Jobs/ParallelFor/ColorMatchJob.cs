// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Classic.Jobs.ParallelFor
{
    /// <summary>
    ///     Determines if the <see cref="Color"/>s in the provided <see cref="Unity.Collections.NativeArray{T}" />s match each other in
    ///     parallel.
    /// </summary>
    [BurstCompile]
    // ReSharper disable once UnusedType.Global
    public struct ColorMatchJob : IJobParallelFor
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
        ///     Does the color match?
        /// </summary>
        [WriteOnly] public NativeArray<bool> Match;

        /// <summary>
        /// Executable work for the provided index.
        /// </summary>
        /// <param name="index">The index of the Parallel for loop at which to perform work.</param>
        public void Execute(int index)
        {
            if (A[index] != B[index])
            {
                Match[index] = false;
            }
            else
            {
                Match[index] = true;
            }
        }
    }
}