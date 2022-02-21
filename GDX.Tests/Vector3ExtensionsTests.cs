// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using UnityEngine;
using GDX;

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
        [Category(GDX.Core.TestCategory)]
        public void Approximately_FiveMillionths_ReturnsFalse()
        {
            Vector3 a = new Vector3(1.000005f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Approximately_One_ReturnsTrue()
        {
            Vector3 a = new Vector3(1, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Approximately_OneMillionth_ReturnsTrue()
        {
            Vector3 a = new Vector3(1.000001f, 1, 1);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Approximately_Zero_ReturnsTrue()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(0, 0, 0);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void DistanceSqr_MockData_ReturnsSame()
        {
            Vector3 a = new Vector3(0,20.5f, 16.5f);
            Vector3 b = new Vector3(18,17.5f, 92.5f);

            float originalDistance = Vector3.Distance(a, b);
            float original = Mathf.Round(originalDistance * originalDistance);
            float optimized = Mathf.Round(a.DistanceSqr(b));

            bool evaluate = Math.Abs(original - optimized) < GDX.Platform.FloatTolerance;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Midpoint_ZeroAndOne_ReturnsHalf()
        {
            Vector3 a = new Vector3(0, 0, 0);
            Vector3 b = new Vector3(1, 1, 1);

            bool evaluate = a.Midpoint(b) == new Vector3(0.5f, 0.5f, 0.5f);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
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
        [Category(GDX.Core.TestCategory)]
        public void NearestIndex_NullInput_ReturnsNegativeOne()
        {
            Vector3 a = Vector3.down;

            bool evaluate = a.NearestIndex(null) == -1;

            Assert.IsTrue(evaluate);
        }

         [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_NoSplit_ReturnsVector2Zero()
        {
            bool parse = "123".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_NoSecondSplit_ReturnsFalse()
        {
            bool parse = "2, 22".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_CharFirstSplit_ReturnsFalse()
        {
            bool parse = "c, 2, 2".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_CharSecondSplit_ReturnsFalse()
        {
            bool parse = "2, c, 2".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_CharThirdSplit_ReturnsFalse()
        {
            bool parse = "2, 2, c".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector3.zero;

            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_NoSpaces_ReturnsVector3()
        {
            bool parse = "3,2,1".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector3(3, 2, 1);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(GDX.Core.TestCategory)]
        public void TryParseVector3_SpacedValues_ReturnsVector3()
        {
            bool parse = "3, 2, 1".TryParseVector3(out Vector3 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector3(3, 2, 1);

            Assert.IsTrue(evaluate);
        }
    }
}