// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Mathematics
{
    /// <summary>
    ///     Integer division and remainder that round toward negative infinity, as grid and cell math needs.
    /// </summary>
    /// <remarks>
    ///     C#'s <c>/</c> and <c>%</c> round toward zero, so <c>-1 / 16</c> is <c>0</c> and <c>-1 % 16</c> is
    ///     <c>-1</c>: a coordinate just below zero lands in cell 0 with a negative offset. These put it in cell
    ///     <c>-1</c> at offset <c>15</c>. Integer only, so the same on every platform.
    /// </remarks>
    public static class IntegerMath
    {
        /// <summary>
        ///     Divide, rounding toward negative infinity.
        /// </summary>
        /// <param name="dividend">The value to divide.</param>
        /// <param name="divisor">The value to divide by; not zero.</param>
        /// <returns>The largest integer not greater than the exact quotient.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorDivide(int dividend, int divisor)
        {
            int quotient = dividend / divisor;
            if (dividend % divisor != 0 && (dividend < 0) != (divisor < 0))
            {
                quotient--;
            }

            return quotient;
        }

        /// <summary>
        ///     Divide, rounding toward negative infinity.
        /// </summary>
        /// <param name="dividend">The value to divide.</param>
        /// <param name="divisor">The value to divide by; not zero.</param>
        /// <returns>The largest integer not greater than the exact quotient.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FloorDivide(long dividend, long divisor)
        {
            long quotient = dividend / divisor;
            if (dividend % divisor != 0 && (dividend < 0) != (divisor < 0))
            {
                quotient--;
            }

            return quotient;
        }

        /// <summary>
        ///     The remainder of <see cref="FloorDivide(int, int)" />, which takes the sign of the divisor.
        /// </summary>
        /// <param name="dividend">The value to divide.</param>
        /// <param name="divisor">The value to divide by; not zero.</param>
        /// <returns>A value from 0 up to <paramref name="divisor" /> (exclusive) for a positive divisor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorModulo(int dividend, int divisor)
        {
            int remainder = dividend % divisor;
            if (remainder != 0 && (remainder < 0) != (divisor < 0))
            {
                remainder += divisor;
            }

            return remainder;
        }

        /// <summary>
        ///     The remainder of <see cref="FloorDivide(long, long)" />, which takes the sign of the divisor.
        /// </summary>
        /// <param name="dividend">The value to divide.</param>
        /// <param name="divisor">The value to divide by; not zero.</param>
        /// <returns>A value from 0 up to <paramref name="divisor" /> (exclusive) for a positive divisor.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long FloorModulo(long dividend, long divisor)
        {
            long remainder = dividend % divisor;
            if (remainder != 0 && (remainder < 0) != (divisor < 0))
            {
                remainder += divisor;
            }

            return remainder;
        }
    }
}
