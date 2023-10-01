// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Mathematics.Random
{
    [VisualScriptingCompatible(4)]
    public interface IRandomProvider
    {
        /// <summary>
        ///     Returns a pseudorandom <see cref="System.Boolean" /> value based on chance, favoring false,
        ///     with a default 50% <paramref name="chance" />.
        /// </summary>
        /// <param name="chance">The 0-1 <see cref="float" /> percent chance of success.</param>
        /// <returns>A pseudorandom <see cref="System.Boolean" />.</returns>
        bool NextBoolean(float chance = 0.5f);

        /// <summary>
        ///     Fills a buffer with pseudorandom <see cref="System.Byte" />.
        /// </summary>
        /// <remarks>
        ///     The <paramref name="buffer" /> shouldn't be <see langword="null" />.
        /// </remarks>
        /// <param name="buffer">The buffer to fill.</param>
        void NextBytes(byte[] buffer);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="double" /> between <paramref name="minValue" /> and
        ///     less then <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value (inclusive).</param>
        /// <param name="maxValue">The highest possible value (exclusive).</param>
        /// <returns>A pseudorandom <see cref="double" />.</returns>
        double NextDouble(double minValue = 0d, double maxValue = 1d);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        ///     Never pass <see cref="int.MaxValue" /> to <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value, including itself.</param>
        /// <returns>A pseudorandom <see cref="int" />.</returns>
        int NextInteger(int minValue = 0, int maxValue = int.MaxValue);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />, excluding <paramref name="maxValue" /> itself.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value, excluding itself.</param>
        /// <returns>A pseudorandom <see cref="int" />.</returns>
        int NextIntegerExclusive(int minValue = 0, int maxValue = int.MaxValue);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="System.Single" /> between <paramref name="minValue" /> and
        ///     less then <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value (inclusive).</param>
        /// <param name="maxValue">The highest possible value (exclusive).</param>
        /// <returns>A pseudorandom <see cref="System.Single" />.</returns>
        float NextSingle(float minValue = 0f, float maxValue = 1f);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        ///     Never pass <see cref="uint.MaxValue" /> to <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value, including itself.</param>
        /// <returns>A pseudorandom <see cref="uint" />.</returns>
        uint NextUnsignedInteger(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />, excluding <paramref name="maxValue" /> itself.
        /// </summary>
        /// <remarks>
        ///     Distribution of values falls within a linear scale.
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value, excluding itself.</param>
        /// <returns>A pseudorandom <see cref="uint" />.</returns>
        uint NextUnsignedIntegerExclusive(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue);

        /// <summary>
        ///     Returns the next pseudorandom <see cref="double" /> value, between 0.0 and 1.0.
        /// </summary>
        /// <returns>A pseudorandom <see cref="double" /> floating point value.</returns>
        double Sample();
    }
}