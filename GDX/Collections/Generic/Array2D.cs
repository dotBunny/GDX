// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

// TODO: Change to struct + unit tests

// ReSharper disable MemberCanBePrivate.Global
namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A 2-dimensional array backed by a flat array.
    /// </summary>
    /// <remarks>Mimics multi-dimensional array format.</remarks>
    /// <typeparam name="T">Type of objects.</typeparam>
    [VisualScriptingCompatible(1)]
    public struct Array2D<T>
    {
        /// <summary>
        ///     The backing flat array.
        /// </summary>
        public T[] Array;

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
        ///     Create a <see cref="Array2D{T}" />.
        /// </summary>
        /// <param name="rowCount">The number of rows (X).</param>
        /// <param name="columnCount">The number of columns (Y).</param>
        public Array2D(int rowCount, int columnCount)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;
            Array = new T[rowCount * columnCount];
        }

        /// <summary>
        ///     Create a <see cref="Array2D{T}" /> providing an existing <paramref name="arrayToUse" />.
        /// </summary>
        /// <param name="rowCount">The number of rows (X).</param>
        /// <param name="columnCount">The number of columns (Y).</param>
        /// <param name="arrayToUse">An existing array to use in the <see cref="Array2D{T}" />.</param>
        public Array2D(int rowCount, int columnCount, T[] arrayToUse)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;
            Array = arrayToUse;
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
        public void AddRows(int numberOfNewRows)
        {
            int newArrayCount = RowCount + numberOfNewRows;
            T[] newArray = new T[newArrayCount * ColumnCount];
            for (int i = 0; i < Array.Length; i++)
            {
                newArray[i] = Array[i];
            }

            Array = newArray;
            RowCount = newArrayCount;
        }

        /// <summary>
        ///     Add additional columns to the dataset.
        /// </summary>
        /// <param name="numberOfNewColumns">The number of columns add.</param>
        public void AddColumns(int numberOfNewColumns)
        {
            int currentLengthOfArrays = ColumnCount;
            int newLengthOfArrays = currentLengthOfArrays + numberOfNewColumns;
            T[] newArray = new T[RowCount * newLengthOfArrays];

            for (int i = 0; i < RowCount; i++)
            for (int j = 0; j < currentLengthOfArrays; j++)
            {
                newArray[i * newLengthOfArrays + j] = Array[i * currentLengthOfArrays + j];
            }

            Array = newArray;
            ColumnCount = newLengthOfArrays;
        }

        /// <summary>
        /// Get the column index of the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">A valid index contained within <see cref="Array"/>.</param>
        /// <returns>The column index.</returns>
        public int GetColumnIndex(int index)
        {
            int leftOvers = index % ColumnCount;
            return leftOvers;
        }

        /// <summary>
        /// Get the row index of the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">A valid index contained within <see cref="Array"/>.</param>
        /// <returns>The row index.</returns>
        public int GetRowIndex(int index)
        {
            int leftOvers = index % ColumnCount;
            return (index - leftOvers) / ColumnCount;
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
            T[] temporaryStorage = new T[ColumnCount];

            int lastIndex = RowCount - 1;
            int middleIndex = RowCount / 2;

            for (int rowIndex = 0; rowIndex < middleIndex; rowIndex++)
            {
                // Save existing line
                System.Array.Copy(Array, rowIndex * ColumnCount, temporaryStorage, 0, ColumnCount);

                // Get line on other side of the flip and put in place
                System.Array.Copy(Array, (lastIndex - rowIndex) * ColumnCount, Array, rowIndex * ColumnCount,
                    ColumnCount);

                // Write other side content
                System.Array.Copy(temporaryStorage, 0, Array, (lastIndex - rowIndex) * ColumnCount, ColumnCount);
            }
        }

        /// <summary>
        /// Rotate internal dataset clockwise.
        /// </summary>
        public void RotateClockwise()
        {
            // TODO: There should be a way to do this without making a transient array.

            // Make our new array
            T[] newArray = new T[Array.Length];

            int newColumnCount = RowCount;
            int runningIndex = 0;

            // Transpose values to new array
            for (int column = 0; column < ColumnCount; column++)
            {
                for (int row = RowCount - 1; row >= 0; row--)
                {
                    newArray[runningIndex] = Array[row * ColumnCount + column];
                    runningIndex++;
                }
            }

            // Assign Data
            RowCount = ColumnCount;
            ColumnCount = newColumnCount;
            Array = newArray;
        }

        /// <summary>
        /// Rotate internal dataset counter-clockwise.
        /// </summary>
        public void RotateCounterClockwise()
        {
            // TODO: There should be a way to do this without making a transient array.

            // Make our new array
            T[] newArray = new T[Array.Length];

            int newColumnCount = RowCount;
            int runningIndex = 0;

            // Transpose values to new array
            for (int column = ColumnCount - 1; column >= 0; column--)
            {
                for (int row = 0; row < RowCount; row++)
                {
                    newArray[runningIndex] = Array[row * ColumnCount + column];
                    runningIndex++;
                }
            }

            // Assign Data
            RowCount = ColumnCount;
            ColumnCount = newColumnCount;
            Array = newArray;
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