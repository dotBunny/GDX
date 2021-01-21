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
    ///     A collection of unit tests to validate functionality of the <see cref="Vector2Extensions" /> class.
    /// </summary>
    public class Vector2ExtensionsTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void True_Approximately_Zero()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(0, 0);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Approximately_One()
        {
            Vector2 a = new Vector2(1, 1);
            Vector2 b = new Vector2(1, 1);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Approximately_FiveZerosOne()
        {
            Vector2 a = new Vector2(1.000001f, 1);
            Vector2 b = new Vector2(1, 1);
            Assert.IsTrue(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void False_Approximately_FiveZerosFive()
        {
            Vector2 a = new Vector2(1.000005f, 1);
            Vector2 b = new Vector2(1, 1);
            Assert.IsFalse(a.Approximately(b), "Expected a positive response.");
        }

        [Test]
        [Category("GDX.Tests")]
        public void True_Midpoint_Simple()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 1);
            Vector2 result = a.Midpoint(b);
            Vector2 expected = new Vector2(0.5f, 0.5f);
            Assert.IsTrue(expected == result, $"Expected {result} but got {result}");
        }
    }
}