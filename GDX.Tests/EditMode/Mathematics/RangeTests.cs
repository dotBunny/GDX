﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using NUnit.Framework;

namespace GDX.Mathematics
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the Range helper.
    /// </summary>
    public class RangeTests
    {

        [Test]
        [Category(Core.TestCategory)]
        public void GetDouble_Bottom_Inclusive()
        {
            double data = Range.GetDouble(0, 0, 9);
            bool evaluate = data == 0;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDouble_Top_Inclusive()
        {
            double data = Range.GetDouble(1, 0, 9);
            bool evaluate = Math.Abs(data - 9) < Platform.FloatTolerance;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetDouble_Middle_Inclusive()
        {
            double data = Range.GetDouble(0.5d, 0, 9);
            bool evaluate = Math.Abs(data - 4.5d) < Platform.FloatTolerance;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetInteger_Bottom_Inclusive()
        {
            int data = Range.GetInteger(0, -1, 9);
            bool evaluate = data == -1;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetInteger_Top_Inclusive()
        {
            int data = Range.GetInteger(1, 0, 9);
            bool evaluate = data == 9;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetInteger_Middle_Inclusive()
        {
            int data = Range.GetInteger(0.5f, 0, 9);
            bool evaluate = data == 4;
            Assert.IsTrue(evaluate);
        }


        [Test]
        [Category(Core.TestCategory)]
        public void GetSingle_Bottom_Inclusive()
        {
            float data = Range.GetSingle(0, 0, 9);
            bool evaluate = data == 0;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetSingle_Top_Inclusive()
        {
            float data = Range.GetSingle(1, 0, 9);
            bool evaluate = Math.Abs(data - 9) < Platform.FloatTolerance;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetSingle_Middle_Inclusive()
        {
            float data = Range.GetSingle(0.5f, 0, 9);
            bool evaluate = Math.Abs(data - 4.5f) < Platform.FloatTolerance;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetUnsignedInteger_Bottom_Inclusive()
        {
            uint data = Range.GetUnsignedInteger(0, 0, 9);
            bool evaluate = data == 0;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetUnsignedInteger_Top_Inclusive()
        {
            uint data = Range.GetUnsignedInteger(1, 0, 9);
            bool evaluate = data == 9;
            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetUnsignedInteger_Middle_Inclusive()
        {
            uint data = Range.GetUnsignedInteger(0.5f, 0, 9);
            bool evaluate = data == 4;
            Assert.IsTrue(evaluate);
        }
    }
}