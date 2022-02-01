// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using GDX.Collections.Generic;
using NUnit.Framework;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Array2DExtensions" />.
    ///     class.
    /// </summary>
    public class Array2DExtensionsTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Scale_MockFloatData_ScaledData()
        {
            int scaleFactor = 2;
            Array2D<float> mockArray = new Array2D<float>(2, 2) {[1, 0] = 0f, [1, 1] = 1f};
            Array2D<float> scaledArray = mockArray.Scale(scaleFactor);

            bool evaluate = scaledArray.RowCount == (mockArray.RowCount * scaleFactor) &&
                            scaledArray.ColumnCount == (mockArray.ColumnCount * scaleFactor) &&
                            scaledArray.Array[15] == 0.5625f &&
                            scaledArray.Array[10] == 0.25f &&
                            scaledArray.Array[6] == 0.125f;

            Assert.IsTrue(evaluate);
        }
    }
}