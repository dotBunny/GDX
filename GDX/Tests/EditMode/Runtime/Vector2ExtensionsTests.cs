// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation
// ReSharper disable UnusedVariable

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Vector2Extensions" /> class.
    /// </summary>
    public class Vector2ExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Approximately_FiveMillionths_ReturnsFalse()
        {
            Vector2 a = new Vector2(1.000005f, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_One_ReturnsTrue()
        {
            Vector2 a = new Vector2(1, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_OneMillionth_ReturnsTrue()
        {
            Vector2 a = new Vector2(1.000001f, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Approximately_Zero_ReturnsTrue()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(0, 0);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Midpoint_ZeroAndOne_ReturnsHalf()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 1);
            Vector2 expected = new Vector2(0.5f, 0.5f);

            bool evaluate = a.Midpoint(b) == expected;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NearestIndex_MockData_ReturnsClosest()
        {
            Vector2 target = Vector2.one;
            Vector2[] mockData = {
                new Vector2(0,0),
                new Vector2(1.1f, 1.1f),
                new Vector2(2,2),
                new Vector2(3,3),
                new Vector2(3,1)
            };

            bool evaluate = target.NearestIndex(mockData) == 1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void NearestIndex_NullInput_ReturnsNegativeOne()
        {
            Vector2 a = Vector2.down;

            bool evaluate = a.NearestIndex(null) == -1;

            Assert.IsTrue(evaluate);
        }


    }
}