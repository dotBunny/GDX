// dotBunny licenses this file to you under the MIT license.
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
        /// <summary>
        ///     Returns the <see cref="double" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="double" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(double percent, double minValue = double.MinValue,
            double maxValue = double.MaxValue)
        {
            double range = (maxValue - minValue);
            double rangeMult = percent * range;
            double returnVa = rangeMult + minValue;

            return returnVa;
            //return percent * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="double" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="double" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetDouble(float percent, double minValue = double.MinValue,
            double maxValue = double.MaxValue)
        {
            return percent * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>The <see cref="int" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInteger(double percent, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return (int)(percent * (maxValue - minValue)) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>The <see cref="int" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInteger(float percent, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return (int)(percent * (maxValue - minValue)) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="float" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="float" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSingle(double percent, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return (float)(percent * (maxValue - minValue) + minValue);
        }

        /// <summary>
        ///     Returns the <see cref="float" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="float" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetSingle(float percent, float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return percent * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="uint" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUnsignedInteger(double percent, uint minValue = uint.MinValue,
            uint maxValue = uint.MaxValue)
        {
            return (uint)(percent * (maxValue - minValue)) + minValue;
        }

        /// <summary>
        ///     Returns the <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> range at <paramref name="percent" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A <see cref="uint" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUnsignedInteger(float percent, uint minValue = uint.MinValue,
            uint maxValue = uint.MaxValue)
        {
            return (uint)(percent * (maxValue - minValue)) + minValue;
        }
    }
}