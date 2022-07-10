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
    ///     A three-dimensional <see cref="NativeArray{T}" /> backed array.
    /// </summary>
    /// <typeparam name="T">Type of objects.</typeparam>
    public struct NativeArray3D<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     The backing <see cref="NativeArray{T}" />.
        /// </summary>
        public NativeArray<T> Array;

        /// <summary>
        ///     The stride of the z-axis segment in <see cref="Array" />.
        /// </summary>
        public readonly int Depth;

        /// <summary>
        ///     The total length of a single <see cref="Depth"/>.
        /// </summary>
        public readonly int DepthLength;

        /// <summary>
        ///     The stride of the y-axis segment in <see cref="Array" />.
        /// </summary>
        public readonly int Height;

        /// <summary>
        ///     The length of <see cref="Array" />.
        /// </summary>
        public readonly int Length;

        /// <summary>
        ///     The stride of the x-axis segment in <see cref="Array" />.
        /// </summary>
        public readonly int Width;

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
            DepthLength = width * height;
            Depth = depth;
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
            get => Array[x + Width * (y + Height * z)];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[x + Width * (y + Height * z)] = value;
        }

        /// <summary>
        ///     Access a specific location in the voxel.
        /// </summary>
        /// <param name="index">A three-dimensional index.</param>
        public T this[int3 index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[index.x + Width * (index.y + Height * index.z)];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[index.x + Width * (index.y + Height * index.z)] = value;
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
        ///     Get the three-dimensional index of a flat array index.
        /// </summary>
        /// <param name="index">A flat array index.</param>
        /// <returns>A three-dimensional voxel index.</returns>
        public int3 GetFromIndex(int index)
        {
            int x = index % Width;
            int y =  (index - x) / Width % Height;
            int z = index / (DepthLength);

            return new int3(x, y, z);
        }

        public int GetFromIndex(int3 index)
        {
            return index.x + Width * (index.y + Height * index.z);
        }
    }
}