// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Vector2Extensions" /> class.
    /// </summary>
    public class Vector2ExtensionsTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Approximately_FiveMillionths_ReturnsFalse()
        {
            Vector2 a = new Vector2(1.000005f, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsFalse(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Approximately_One_ReturnsTrue()
        {
            Vector2 a = new Vector2(1, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Approximately_OneMillionth_ReturnsTrue()
        {
            Vector2 a = new Vector2(1.000001f, 1);
            Vector2 b = new Vector2(1, 1);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Approximately_Zero_ReturnsTrue()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(0, 0);

            bool evaluate = a.Approximately(b);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Midpoint_ZeroAndOne_ReturnsHalf()
        {
            Vector2 a = new Vector2(0, 0);
            Vector2 b = new Vector2(1, 1);
            Vector2 expected = new Vector2(0.5f, 0.5f);

            bool evaluate = a.Midpoint(b) == expected;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
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
        [Category(Core.TestCategory)]
        public void NearestIndex_NullInput_ReturnsNegativeOne()
        {
            Vector2 a = Vector2.down;

            bool evaluate = a.NearestIndex(null) == -1;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Slope_ZeroXValue_ReturnsZero()
        {
            Vector2 mockData = new Vector2(0, 10);

            bool evaluate = mockData.Slope() == 0;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Slope_MockData_ReturnsSlope()
        {
            Vector2 mockData = new Vector2(5, 10);

            bool evaluate = Math.Abs(mockData.Slope() - 2f) < Platform.FloatTolerance;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryParseVector2_NoSplit_ReturnsFalse()
        {
            bool parse = "12".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryParseVector2_NoSpaces_ReturnsVector2()
        {
            bool parse = "1.5,2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector2(1.5f, 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryParseVector2_SpacedValues_ReturnsVector2()
        {
            bool parse = "1, 2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = parse && parsedLocation == new Vector2(1, 2);

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryParseVector2_CharFirstSplit_ReturnsFalse()
        {
            bool parse = "c, 2".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void TryParseVector2_CharSecondSplit_ReturnsFalse()
        {
            bool parse = "1, c".TryParseVector2(out Vector2 parsedLocation);

            bool evaluate = !parse && parsedLocation == Vector2.zero;

            Assert.IsTrue(evaluate);
        }


    }
}