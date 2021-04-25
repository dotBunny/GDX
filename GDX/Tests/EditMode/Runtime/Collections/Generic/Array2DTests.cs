// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Array2D{T}" />.
    /// </summary>
    public class Array2DTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void AddColumns_MockData_ArrayResized()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 3);
            int pre = mockArray.Array.Length;

            mockArray.AddColumns(1);

            bool evaluate = mockArray.ColumnCount == 4 &&
                            mockArray.Array.Length == pre + 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void AddRows_MockData_ArrayResized()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4);
            int pre = mockArray.Array.Length;

            mockArray.AddRows(1);

            bool evaluate = mockArray.RowCount == 3 &&
                            mockArray.Array.Length == pre + 4;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ReverseColumns_MockDataEven_ValuesReversed()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4) {[1, 0] = 0, [1, 1] = 1, [1, 2] = 2, [1, 3] = 3};


            mockArray.ReverseColumns();

            bool evaluate = mockArray[1, 0] == 3 &&
                            mockArray[1, 1] == 2 &&
                            mockArray[1, 2] == 1 &&
                            mockArray[1, 3] == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ReverseColumns_MockDataOdd_ValuesReversed()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 5)
            {
                [1, 0] = 0,
                [1, 1] = 1,
                [1, 2] = 2,
                [1, 3] = 3,
                [1, 4] = 4
            };


            mockArray.ReverseColumns();

            bool evaluate = mockArray[1, 0] == 4 &&
                            mockArray[1, 1] == 3 &&
                            mockArray[1, 2] == 2 &&
                            mockArray[1, 3] == 1 &&
                            mockArray[1, 4] == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ReverseRows_MockDataEven_ValuesReversed()
        {
            Array2D<int> mockArray = new Array2D<int>(6, 2)
            {
                [0, 0] = 0,
                [1, 0] = 1,
                [2, 0] = 2,
                [3, 0] = 3,
                [4, 0] = 4,
                [5, 0] = 5
            };

            mockArray.ReverseRows();

            bool evaluate = mockArray[0, 0] == 5 &&
                            mockArray[1, 0] == 4 &&
                            mockArray[2, 0] == 3 &&
                            mockArray[3, 0] == 2 &&
                            mockArray[4, 0] == 1 &&
                            mockArray[5, 0] == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ReverseRows_MockDataOdd_ValuesReversed()
        {
            Array2D<int> mockArray = new Array2D<int>(5, 2)
            {
                [0, 0] = 0,
                [1, 0] = 1,
                [2, 0] = 2,
                [3, 0] = 3,
                [4, 0] = 4
            };

            mockArray.ReverseRows();

            bool evaluate = mockArray[0, 0] == 4 &&
                            mockArray[1, 0] == 3 &&
                            mockArray[2, 0] == 2 &&
                            mockArray[3, 0] == 1 &&
                            mockArray[4, 0] == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RotateClockwise_MockDataOdd_ValuesRotated()
        {
            Array2D<int> mockArray = new Array2D<int>(5, 2)
            {
                [0, 0] = 0,
                [1, 0] = 1,
                [2, 0] = 2,
                [3, 0] = 3,
                [4, 0] = 4,

                [0, 1] = 5,
                [1, 1] = 6,
                [2, 1] = 7,
                [3, 1] = 8,
                [4, 1] = 9
            };

            mockArray.RotateClockwise();

            bool evaluate = mockArray[0, 4] == 0 && mockArray[1, 2] == 7;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void RotateCounterClockwise_MockDataOdd_ValuesRotated()
        {
            Array2D<int> mockArray = new Array2D<int>(5, 2)
            {
                [0, 0] = 0,
                [1, 0] = 1,
                [2, 0] = 2,
                [3, 0] = 3,
                [4, 0] = 4,

                [0, 1] = 5,
                [1, 1] = 6,
                [2, 1] = 7,
                [3, 1] = 8,
                [4, 1] = 9
            };

            mockArray.RotateCounterClockwise();

            bool evaluate = mockArray[0, 4] == 9 && mockArray[1, 2] == 2;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void ToMultiDimensionalArray_MockData_MatchingValues()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 2) {[0, 0] = 0, [1, 0] = 1};

            int[,] mockConvert = mockArray.ToMultiDimensionalArray();

            bool evaluate = mockConvert[0, 0] == 0 &&
                            mockConvert[1, 0] == 1 &&
                            mockConvert[0, 1] == 0 &&
                            mockConvert[1, 1] == 0;

            Assert.IsTrue(evaluate);
        }
    }
}