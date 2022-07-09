// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A 3-Dimensional <see cref="NativeArray{T}" /> backed array.
    /// </summary>
    /// <typeparam name="T">Type of objects.</typeparam>
    public struct NativeArray3D<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     The backing <see cref="NativeArray{T}" />.
        /// </summary>
        public NativeArray<T> Array;

        /// <summary>
        ///     The length of <see cref="Array" />.
        /// </summary>
        public readonly int Length;

        /// <summary>
        ///     The stride of each dimensional segment in <see cref="Array" />.
        /// </summary>
        public readonly int Width;

        /// <summary>
        ///     The stride of each dimensional segment in <see cref="Array" />.
        /// </summary>
        public readonly int Height;


        private readonly int BackStride;

        /// <summary>
        ///     The stride of each dimensional segment in <see cref="Array" />.
        /// </summary>
        public readonly int Depth;

        /// <summary>
        ///     Create a <see cref="NativeArray3D{T}" /> with a uniform dimensional length.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="width">X-axis length.</param>
        /// <param name="height">Y-axis length.</param>
        /// <param name="depth">Z-axis length.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeArray3D(int width, int height, int depth, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            Width = width;
            Height = height;
            Depth = depth;
            BackStride = Width * Depth;

            Length = width * height * depth;
            Array = new NativeArray<T>(Length, allocator, nativeArrayOptions);
        }

        /// <summary>
        ///     Access a specific location in the voxel.
        /// </summary>
        /// <param name="x">X location index.</param>
        /// <param name="y">Y location index.</param>
        /// <param name="z">Z location index.</param>
        public T this[int x, int y, int z]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[z * BackStride + y * Width + x];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[z * BackStride + y * Width + x] = value;
        }

       // Flat[x + WIDTH * (y + DEPTH * z)] = Original[x, y, z]

        /// <summary>
        ///     Access a specific location in the voxel.
        /// </summary>
        /// <param name="index">A 3-Dimensional index.</param>
        public T this[int3 index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[index.z * (BackStride) + index.y * Width + index.x];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[index.z * BackStride + index.y * Width + index.x] = value;
        }


        /// <summary>
        ///     Properly dispose of the <see cref="NativeUniformArray3D{T}" />.
        /// </summary>
        public void Dispose()
        {
            if (!Array.IsCreated)
            {
                return;
            }

            Array.Dispose();
            Array = default;
        }

        /// <summary>
        ///     Get the 3-Dimensional index of a flat array index.
        /// </summary>
        /// <param name="index">A flat array index.</param>
        /// <returns>A 3-Dimensional voxel index.</returns>
        public int3 GetFromIndex(int index)
        {
            int x = index % Width;
            int y =  (index - x)/Width % Width;
            int z = (index - x - Width * y) / BackStride;
            return new int3(x, y, z);
        }
    }
}