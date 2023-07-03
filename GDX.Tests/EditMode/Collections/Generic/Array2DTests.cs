// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Array2D{T}" />.
    /// </summary>
    public class Array2DTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void AddColumns_MockData_ArrayResized()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 3);
            int pre = mockArray.Array.Length;

            mockArray.AddColumns(1);

            Assert.That(mockArray.ColumnCount, Is.EqualTo(4));
            Assert.That(mockArray.Array.Length, Is.EqualTo(pre+2));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddRows_MockData_ArrayResized()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4);
            int pre = mockArray.Array.Length;

            mockArray.AddRows(1);

            Assert.That(mockArray.RowCount, Is.EqualTo(3));
            Assert.That(mockArray.Array.Length, Is.EqualTo(pre+4));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetColumnIndex_MockData_ReturnsIndex()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4) {[1, 0] = 0, [1, 1] = 1, [1, 2] = 2, [1, 3] = 3};

            Assert.That(mockArray.GetColumnIndex(5), Is.EqualTo(1));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetRowIndex_MockData_ReturnsIndex()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4) {[1, 0] = 0, [1, 1] = 1, [1, 2] = 2, [1, 3] = 3};

            Assert.That(mockArray.GetRowIndex(5), Is.EqualTo(1));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ReverseColumns_MockDataEven_ValuesReversed()
        {
            Array2D<int> mockArray = new Array2D<int>(2, 4) {[1, 0] = 0, [1, 1] = 1, [1, 2] = 2, [1, 3] = 3};
            mockArray.ReverseColumns();

            Assert.That(mockArray[1, 0], Is.EqualTo(3));
            Assert.That(mockArray[1, 1], Is.EqualTo(2));
            Assert.That(mockArray[1, 2], Is.EqualTo(1));
            Assert.That(mockArray[1, 3], Is.Zero);
        }

        [Test]
        [Category(Core.TestCategory)]
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

            Assert.That(mockArray[1, 0], Is.EqualTo(4));
            Assert.That(mockArray[1, 1], Is.EqualTo(3));
            Assert.That(mockArray[1, 2], Is.EqualTo(2));

            Assert.That(mockArray[1, 3], Is.EqualTo(1));
            Assert.That(mockArray[1, 4], Is.EqualTo(0));
        }

        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
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