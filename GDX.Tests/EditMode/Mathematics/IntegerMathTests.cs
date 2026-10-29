// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Mathematics
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of <see cref="IntegerMath" />.
    /// </summary>
    public class IntegerMathTests
    {
        [Test]
        [Category(Core.TestCategory)]
        [TestCase(0, 16, 0, 0)]
        [TestCase(15, 16, 0, 15)]
        [TestCase(16, 16, 1, 0)]
        [TestCase(-1, 16, -1, 15)]
        [TestCase(-16, 16, -1, 0)]
        [TestCase(-17, 16, -2, 15)]
        [TestCase(7, -2, -4, -1)]
        [TestCase(-7, -2, 3, -1)]
        [TestCase(int.MaxValue, 3, 715827882, 1)]
        [TestCase(int.MinValue, 3, -715827883, 1)]
        public void FloorDivideModulo_Int_RoundsDown(int dividend, int divisor, int quotient, int remainder)
        {
            Assert.AreEqual(quotient, IntegerMath.FloorDivide(dividend, divisor));
            Assert.AreEqual(remainder, IntegerMath.FloorModulo(dividend, divisor));
            Assert.AreEqual(dividend, quotient * divisor + remainder);
        }

        [Test]
        [Category(Core.TestCategory)]
        [TestCase(-1L, 16L, -1L, 15L)]
        [TestCase(-17L, 16L, -2L, 15L)]
        [TestCase(long.MinValue, 7L, -1317624576693539402L, 6L)]
        public void FloorDivideModulo_Long_RoundsDown(long dividend, long divisor, long quotient, long remainder)
        {
            Assert.AreEqual(quotient, IntegerMath.FloorDivide(dividend, divisor));
            Assert.AreEqual(remainder, IntegerMath.FloorModulo(dividend, divisor));
        }

        [Test]
        [Category(Core.TestCategory)]
        public void FloorDivideModulo_Range_AgreesWithEachOther()
        {
            for (int dividend = -100; dividend <= 100; dividend++)
            {
                for (int divisor = -9; divisor <= 9; divisor++)
                {
                    if (divisor == 0)
                    {
                        continue;
                    }

                    int quotient = IntegerMath.FloorDivide(dividend, divisor);
                    int remainder = IntegerMath.FloorModulo(dividend, divisor);
                    Assert.AreEqual(dividend, quotient * divisor + remainder, $"{dividend} / {divisor}");
                    bool inRange = divisor > 0
                        ? remainder >= 0 && remainder < divisor
                        : remainder <= 0 && remainder > divisor;
                    Assert.IsTrue(inRange, $"{dividend} % {divisor}");
                }
            }
        }
    }
}
