// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A 2-dimension <see cref="NativeArray{T}" /> backed array.
    /// </summary>
    /// <remarks>
    ///     The <see cref="NativeArray2D{T}" /> is backed by a <see cref="Unity.Collections.NativeArray{T}" /> which requires
    ///     UnityEngine.CoreModule.dll.
    /// </remarks>
    /// <typeparam name="T">Type of objects.</typeparam>
    public struct NativeArray2D<T> : IDisposable where T : struct
    {
        /// <summary>
        ///     The backing <see cref="NativeArray{T}" />.
        /// </summary>
        public NativeArray<T> Array;

        /// <summary>
        ///     The length of each pseudo-array in the dataset.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int ColumnCount;

        /// <summary>
        ///     The number of pseudo-arrays created to support the dimensionality.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public int RowCount;

        /// <summary>
        ///     Create a <see cref="NativeArray2D{T}" />.
        /// </summary>
        /// <param name="columnCount">The number of columns (X).</param>
        /// <param name="rowCount">The number of rows (Y).</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeArray2D(int columnCount, int rowCount, Allocator allocator,
            NativeArrayOptions nativeArrayOptions)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;
            Array = new NativeArray<T>(rowCount * columnCount, allocator, nativeArrayOptions);
        }

        /// <summary>
        ///     Get a typed object at a specific 2-dimensional index in <see cref="Array" />.
        /// </summary>
        /// <param name="x">The column number (X).</param>
        /// <param name="y">The row number (Y).</param>
        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Array[x * ColumnCount + y];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Array[x * ColumnCount + y] = value;
        }

        /// <summary>
        ///     Add additional rows to the dataset.
        /// </summary>
        /// <param name="numberOfNewRows">The number of rows/arrays to add.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void AddRows(int numberOfNewRows, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int newArrayCount = RowCount + numberOfNewRows;

            NativeArray<T> newArray = new NativeArray<T>(newArrayCount * ColumnCount, allocator, nativeArrayOptions);
            Array.CopyTo(newArray);
            Array.Dispose();
            Array = newArray;
            RowCount = newArrayCount;
        }

        /// <summary>
        ///     Properly dispose of <see cref="Array" />.
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
        ///     Add additional columns to the dataset.
        /// </summary>
        /// <param name="numberOfNewColumns">The number of columns add.</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public void AddColumns(int numberOfNewColumns, Allocator allocator, NativeArrayOptions nativeArrayOptions)
        {
            int currentLengthOfArrays = ColumnCount;
            int newLengthOfArrays = currentLengthOfArrays + numberOfNewColumns;
            NativeArray<T> newArray = new NativeArray<T>(RowCount * newLengthOfArrays, allocator, nativeArrayOptions);

            for (int i = 0; i < RowCount; i++)
            for (int j = 0; j < currentLengthOfArrays; j++)
            {
                newArray[i * newLengthOfArrays + j] = Array[i * currentLengthOfArrays + j];
            }

            ColumnCount = newLengthOfArrays;
        }
    }
}