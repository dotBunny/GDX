// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Vector3Extensions" /> class.
    /// </summary>
    public class Vector3ExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Approximately_FiveMillionths_ReturnsFalse()
        {
            Vector3 a = new Vector3(1.000005f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_One_ReturnsTrue()
        {
            Vector3 a = new Vector3(1, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_OneMillionth_ReturnsTrue()
        {
            Vector3 a = new Vector3(1.000001f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_Zero_ReturnsTrue()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(0, 0, 0);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Midpoint_ZeroAndOne_ReturnsHalf()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Midpoint(b) == new Vector3(0.5f, 0.5f, 0.5f);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NearestIndex_MockData_ReturnsClosest()
        {
            Vector3 target = Vector3.one;
            Vector3[] mockData =
            {
                new Vector3(0, 0, 0), new Vector3(1.1f, 1.1f, 1.1f), new Vector3(2, 2, 0), new Vector3(3, 3, 1),
                new Vector3(3, 1, 0)
            };

            bool evaluate = target.NearestIndex(mockData) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NearestIndex_NullInput_ReturnsNegativeOne()
        {
            Vector3 a = Vector3.down;

            bool evaluate = a.NearestIndex(null) == -1;

            Assert.IsTrue(evaluate);
        }
    }
}