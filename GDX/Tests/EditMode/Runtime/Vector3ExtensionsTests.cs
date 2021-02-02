// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using NUnit.Framework;
using UnityEngine;
#if GDX_PERFORMANCETESTING

#endif

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
        public void True_Approximately_Zero()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(0, 0, 0);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Approximately_One()
        {
            Vector3 a = new Vector3(1, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Approximately_FiveZerosOne()
        {
            Vector3 a = new Vector3(1.000001f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_Approximately_FiveZerosFive()
        {
            Vector3 a = new Vector3(1.000005f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);
            Assert.IsFalse(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Midpoint_Simple()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(1, 1, 1);
            Vector3 result = a.Midpoint(b);
            Vector3 expected = new Vector3(0.5f, 0.5f, 0.5f);
            Assert.IsTrue(expected == result, $"Expected {result} but got {result}");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Nearest_NoData()
        {
            Vector3 a = Vector3.down;
            Assert.IsTrue(a.NearestIndex(null) == -1, "Expected a return of -1 for null data");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Nearest_Simple()
        {
            Vector3 target = Vector3.one;

            Vector3[] searchData = {
                new Vector3(0,0,0),
                new Vector3(1.1f, 1.1f, 1.1f),
                new Vector3(2,2, 0),
                new Vector3(3,3, 1),
                new Vector3(3,1, 0)
            };

            int nearestIndex =  target.NearestIndex(searchData);
            Assert.IsTrue(nearestIndex == 1, $"Expected index of 1, found {nearestIndex}.");
        }
    }
}