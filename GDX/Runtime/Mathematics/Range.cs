// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX.Mathematics
{
    /// <summary>
    /// Some simple logic to pick a value from a range.
    /// </summary>
    public static class Range
    {
        public const int SafeIntegerMaxValue = int.MaxValue - 1;
        public const uint SafeUnsignedIntegerMaxValue = uint.MaxValue - 1;

        /// <summary>
        ///     Returns the <see cref="double" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="double" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(double percent, double minValue = double.MinValue,
            double maxValue = double.MaxValue)
        {
            return  maxValue * percent + minValue * (1d - percent);
        }

        /// <summary>
        ///     Returns the <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>The <see cref="int" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInteger(double percent, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return (int)(maxValue * percent + minValue * (1d - percent));
        }

        /// <summary>
        ///     Returns the <see cref="float" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="float" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSingle(double percent, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return (float)(maxValue * percent + minValue * (1d - percent));
        }

        /// <summary>
        ///     Returns the <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="uint" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUnsignedInteger(double percent, uint minValue = uint.MinValue,
            uint maxValue = uint.MaxValue)
        {
            return (uint)(maxValue * percent + minValue * (1d - percent));
        }
    }
}