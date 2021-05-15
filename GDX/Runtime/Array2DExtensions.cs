// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
#if GDX_MATHEMATICS
using Unity.Mathematics;
#else
using UnityEngine;
#endif

namespace GDX
{
    /// <summary>
    ///     Array2D Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(12)]
    public static class Array2DExtensions
    {
        /// <summary>
        ///     Generate an array scaled by bilinear interpolation.
        /// </summary>
        /// <remarks>Works with <see cref="double"/> values.</remarks>
        /// <param name="inputArray">The existing <see cref="Array2D{T}"/> to scale.</param>
        /// <param name="scaleFactor">The multiple to scale by.</param>
        public static Array2D<double> Scale(this Array2D<double> inputArray, int scaleFactor = 2)
        {
            int originalRowsMinusOne = inputArray.RowCount - 1;
            int originalColsMinusOne = inputArray.ColumnCount - 1;

            int newRows = inputArray.RowCount * scaleFactor;
            int newCols = inputArray.ColumnCount * scaleFactor;

            Array2D<double> returnArray = new Array2D<double>(inputArray.RowCount * scaleFactor,
                inputArray.ColumnCount * scaleFactor);

            for (int x = 0; x < newCols; x++)
            for (int y = 0; y < newRows; y++)
            {
                double gx = (double)x / newCols * originalColsMinusOne;
                double gy = (double)y / newCols * originalRowsMinusOne;
                int gxi = (int)gx;
                int gyi = (int)gy;

                double c00 = inputArray[gxi, gyi];
                double c10 = inputArray[gxi + 1, gyi];
                double c01 = inputArray[gxi, gyi + 1];
                double c11 = inputArray[gxi + 1, gyi + 1];

                double tx = gx - gxi;
                double ty = gy - gyi;
#if GDX_MATHEMATICS
                returnArray[x, y] = math.lerp(math.lerp(c00, c10, tx), math.lerp(c01, c11, tx), ty);
#else
                returnArray[x, y] = Mathf.Lerp(Mathf.Lerp(c00, c10, tx), Mathf.Lerp(c01, c11, tx), ty);
#endif
            }

            return returnArray;
        }

        /// <summary>
        ///     Generate an array scaled by bilinear interpolation.
        /// </summary>
        /// <remarks>Works with <see cref="float"/> values.</remarks>
        /// <param name="inputArray">The existing <see cref="Array2D{T}"/> to scale.</param>
        /// <param name="scaleFactor">The multiple to scale by.</param>
        public static Array2D<float> Scale(this Array2D<float> inputArray, int scaleFactor = 2)
        {
            int originalRowsMinusOne = inputArray.RowCount - 1;
            int originalColsMinusOne = inputArray.ColumnCount - 1;

            int newRows = inputArray.RowCount * scaleFactor;
            int newCols = inputArray.ColumnCount * scaleFactor;

            Array2D<float> returnArray = new Array2D<float>(inputArray.RowCount * scaleFactor,
                inputArray.ColumnCount * scaleFactor);

            for (int x = 0; x < newCols; x++)
            for (int y = 0; y < newRows; y++)
            {
                float gx = (float)x / newCols * originalColsMinusOne;
                float gy = (float)y / newCols * originalRowsMinusOne;
                int gxi = (int)gx;
                int gyi = (int)gy;

                float c00 = inputArray[gxi, gyi];
                float c10 = inputArray[gxi + 1, gyi];
                float c01 = inputArray[gxi, gyi + 1];
                float c11 = inputArray[gxi + 1, gyi + 1];

                float tx = gx - gxi;
                float ty = gy - gyi;
#if GDX_MATHEMATICS
                returnArray[x, y] = math.lerp(math.lerp(c00, c10, tx), math.lerp(c01, c11, tx), ty);
#else
                returnArray[x, y] = Mathf.Lerp(Mathf.Lerp(c00, c10, tx), Mathf.Lerp(c01, c11, tx), ty);
#endif
            }

            return returnArray;
        }

        /// <summary>
        ///     Generate an array scaled by bilinear interpolation.
        /// </summary>
        /// <remarks>Works with <see cref="int"/> values.</remarks>
        /// <param name="inputArray">The existing <see cref="Array2D{T}"/> to scale.</param>
        /// <param name="scaleFactor">The multiple to scale by.</param>
        public static Array2D<int> Scale(this Array2D<int> inputArray, int scaleFactor = 2)
        {
            int originalRowsMinusOne = inputArray.RowCount - 1;
            int originalColsMinusOne = inputArray.ColumnCount - 1;

            int newRows = inputArray.RowCount * scaleFactor;
            int newCols = inputArray.ColumnCount * scaleFactor;

            Array2D<int> returnArray = new Array2D<int>(inputArray.RowCount * scaleFactor,
                inputArray.ColumnCount * scaleFactor);

            for (int x = 0; x < newCols; x++)
            for (int y = 0; y < newRows; y++)
            {
                int gx = x / newCols * originalColsMinusOne;
                int gy = y / newCols * originalRowsMinusOne;
                int gxi = gx;
                int gyi = gy;

                int c00 = inputArray[gxi, gyi];
                int c10 = inputArray[gxi + 1, gyi];
                int c01 = inputArray[gxi, gyi + 1];
                int c11 = inputArray[gxi + 1, gyi + 1];

                int tx = gx - gxi;
                int ty = gy - gyi;

#if GDX_MATHEMATICS
                returnArray[x, y] = (int)math.lerp(math.lerp(c00, c10, tx), math.lerp(c01, c11, tx), ty);
#else
                returnArray[x, y] = (int)Mathf.Lerp(Mathf.Lerp(c00, c10, tx), Mathf.Lerp(c01, c11, tx), ty);
#endif
            }

            return returnArray;
        }
    }
}