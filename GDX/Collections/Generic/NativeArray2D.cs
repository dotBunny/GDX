// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Unity.Collections;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A 2-dimension <see cref="NativeArray{T}" /> backed array.
    /// </summary>
    /// <remarks>Use X (horizontal) and Y (vertical) arrangement.</remarks>
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
        /// <param name="rowCount">The number of rows (X).</param>
        /// <param name="columnCount">The number of columns (Y).</param>
        /// <param name="allocator">The <see cref="Unity.Collections.Allocator" /> type to use.</param>
        /// <param name="nativeArrayOptions">Should the memory be cleared on allocation?</param>
        public NativeArray2D(int rowCount, int columnCount, Allocator allocator,
            NativeArrayOptions nativeArrayOptions)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;
            Array = new NativeArray<T>(rowCount * columnCount, allocator, nativeArrayOptions);
        }

        /// <summary>
        ///     Get a typed object at a specific 2-dimensional index in <see cref="Array" />.
        /// </summary>
        /// <param name="x">The row/line number (vertical axis).</param>
        /// <param name="y">The column number (horizontal axis).</param>
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
        ///     Reverse the order of the columns in the backing <see cref="Array" />.
        /// </summary>
        public void ReverseColumns()
        {
            T temporaryStorage;

            int lastIndex = ColumnCount - 1;
            int middleIndex = ColumnCount / 2;

            for (int rowIndex = 0; rowIndex < RowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < middleIndex; columnIndex++)
                {
                    // Cache our indexes
                    int currentElementIndex = rowIndex * ColumnCount + columnIndex;
                    int swapElementIndex = rowIndex * ColumnCount + (lastIndex - columnIndex);

                    // Store the swap value
                    temporaryStorage = Array[currentElementIndex];

                    // Swap values
                    Array[currentElementIndex] = Array[swapElementIndex];
                    Array[swapElementIndex] = temporaryStorage;
                }
            }
        }

        /// <summary>
        ///     Reverse the order of the rows in the backing <see cref="Array" />.
        /// </summary>
        public void ReverseRows()
        {
            NativeArray<T> temporaryStorage =
                new NativeArray<T>(ColumnCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            int lastIndex = RowCount - 1;
            int middleIndex = RowCount / 2;

            for (int rowIndex = 0; rowIndex < middleIndex; rowIndex++)
            {
                // Save existing line
                NativeArray<T>.Copy(Array, rowIndex * ColumnCount, temporaryStorage, 0, ColumnCount);

                // Get line on other side of the flip and put in place
                NativeArray<T>.Copy(Array, (lastIndex - rowIndex) * ColumnCount, Array, rowIndex * ColumnCount,
                    ColumnCount);

                // Write other side content
                NativeArray<T>.Copy(temporaryStorage, 0, Array, (lastIndex - rowIndex) * ColumnCount, ColumnCount);
            }

            temporaryStorage.Dispose();
        }

        /// <summary>
        ///     Creates a copy of the internal array as a traditional multi-dimensional array.
        /// </summary>
        /// <remarks>Useful for scenarios where fills need to be done with [,] structured multi-dimensional arrays.</remarks>
        /// <returns>A new copy of the backing <see cref="Array" /> in multi-dimensional form.</returns>
        public T[,] ToMultiDimensionalArray()
        {
            T[,] returnArray = new T[RowCount, ColumnCount];
            for (int x = 0; x < RowCount; x++)
            {
                for (int y = 0; y < ColumnCount; y++)
                {
                    returnArray[x, y] = Array[x * ColumnCount + y];
                }
            }

            return returnArray;
        }
    }
}