// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A 2-dimensional array backed by a flat array.
    /// </summary>
    /// <remarks>Mimics multi-dimensional array format.</remarks>
    /// <typeparam name="T">Type of objects.</typeparam>
    [VisualScriptingCollection]
    public class Array2D<T>
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