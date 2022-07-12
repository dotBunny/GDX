// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A uniform three-dimensional array.
    /// </summary>
    /// <typeparam name="T">Type of objects.</typeparam>
    public struct UniformArray3D<T> : IDisposable
    {
        /// <summary>
        ///     The backing <see cref="Array" />.
        /// </summary>
        public T[] Array;

        /// <summary>
        ///     The length of <see cref="Array" />.
        /// </summary>
        public readonly int Length;

        /// <summary>
        ///     The stride of each dimensional segment in <see cref="Array" />.
        /// </summary>
        public readonly int Stride;

        /// <summary>
        ///     Stores a cached copy of the stride squared.
        /// </summary>
        public readonly int StrideSquared;

        /// <summary>
        ///     Create a <see cref="UniformArray3D{T}" /> with a uniform dimensional length.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="stride">X length, Y length and Z length will all be set to this value.</param>
        public UniformArray3D(int stride)
        {
            Stride = stride;
            StrideSquared = stride * stride;
            Length = StrideSquared * stride;

            Array = new T[Length];
        }

        /// <summary>
        ///     Access a specific location in the voxel.
        /// </summary>
        /// <remarks>x + WIDTH * (y + DEPTH * z)</remarks>
        /// <param name="x">X location index.</param>
        /// <param name="y">Y location index.</param>
        /// <param name="z">Z location index.</param>
        public T this[int x, int y, int z]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[z * StrideSquared + y * Stride + x];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[z * StrideSquared + y * Stride + x] = value;
        }


        /// <summary>
        ///     Access a specific location in the voxel.
        /// </summary>
        /// <param name="index">A three-dimensional index.</param>
        public T this[int3 index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[index.z * StrideSquared + index.y * Stride + index.x];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[index.z * StrideSquared + index.y * Stride + index.x] = value;
        }

        /// <summary>
        ///     Properly dispose of the <see cref="UniformArray3D{T}" />.
        /// </summary>
        public void Dispose()
        {
            Array = default;
        }

        /// <summary>
        ///     Get the three-dimensional index of a flat array index.
        /// </summary>
        /// <param name="index">A flat array index.</param>
        /// <returns>A three-dimensional voxel index.</returns>
        public int3 GetFromIndex(int index)
        {
            int x = index % Stride;
            int y =  (index - x)/Stride % Stride;
            int z = (index - x - Stride * y) / StrideSquared;
            return new int3(x, y, z);
        }

        /// <summary>
        ///     Get the three-dimensional index of a flat array index.
        /// </summary>
        /// <param name="index">A flat array index.</param>
        /// <param name="stride">The predetermined length of an axis.</param>
        /// <param name="strideSquared">The squared value of <paramref name="stride"/>.</param>
        /// <returns>A three-dimensional voxel index.</returns>
        public static int3 GetFromIndex(int index, int stride, int strideSquared)
        {
            int x = index % stride;
            int y =  (index - x)/stride % stride;
            int z = (index - x - stride * y) / strideSquared;
            return new int3(x, y, z);
        }
    }
}