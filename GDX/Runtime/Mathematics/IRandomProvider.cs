namespace GDX.Mathematics
{
    public interface IRandomProvider
    {
        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Int32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Int32" /> value.</returns>
        int Next();

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Int32" /> up to <paramref name="maxValue" />.
        /// </summary>
        /// <param name="maxValue">The maximum value of the pseudo-random number to create.</param>
        /// <returns>
        ///     A pseudo-random <see cref="Int32" /> value which is at most <paramref name="maxValue" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When <paramref name="maxValue" /> &lt; 0.
        /// </exception>
        int Next(int maxValue);

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Int32" /> at least <paramref name="minValue" />
        ///     and up to <paramref name="maxValue" />.
        /// </summary>
        /// <param name="minValue">The minimum value of the pseudo-random number to create.</param>
        /// <param name="maxValue">The maximum value of the pseudo-random number to create.</param>
        /// <returns>
        ///     A pseudo-random <see cref="System.Int32" /> value which is at least <paramref name="minValue" /> and at
        ///     most <paramref name="maxValue" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <c><paramref name="minValue" /> &gt;= <paramref name="maxValue" /></c>.
        /// </exception>
        int Next(int minValue, int maxValue);

        /// <summary>
        ///     Returns a pseudo random <see cref="System.Boolean" />. value based on chance (<code>0</code>-<code>1</code> roll),
        ///     if the result is included.
        /// </summary>
        /// <param name="chance">The 0-1 percent chance of success.</param>
        /// <returns>A pseudo random <see cref="System.Boolean" />.</returns>
        bool NextBias(float chance);

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="System.Boolean" /> value of either true or false.</returns>
        bool NextBoolean();

        /// <summary>
        ///     Fills a buffer with pseudo-random <see cref="System.Byte" />.
        /// </summary>
        /// <param name="buffer">The buffer to fill.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <c><paramref name="buffer" /> == <see langword="null" /></c>.
        /// </exception>
        void NextBytes(byte[] buffer);

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Double" /> value.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Double" /> floating point value.</returns>
        double NextDouble();

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Double" /> number greater than or equal to zero, and
        ///     either strictly less than one, or less than or equal to one,
        ///     depending on the value of the given parameter.
        /// </summary>
        /// <param name="includeOne">
        ///     If <see langword="true" />, the pseudo-random <see cref="System.Double" /> number returned will be
        ///     less than or equal to one; otherwise, the pseudo-random number returned will
        ///     be strictly less than one.
        /// </param>
        /// <returns>
        ///     If <paramref name="includeOne" /> is <see langword="true" />, this method returns a
        ///     <see cref="System.Double" />-precision pseudo-random number greater than or equal to zero, and less
        ///     than or equal to one. If <paramref name="includeOne" /> is <see langword="false" />, this method
        ///     returns a <see cref="System.Double" />-precision pseudo-random number greater than or equal to zero and
        ///     strictly less than one.
        /// </returns>
        double NextDouble(bool includeOne);

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Double" /> number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Double" /> number greater than 0.0 and less than 1.0.</returns>
        double NextDoublePositive();

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Single" /> number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Single" />-precision floating point number greater than or equal to 0.0,
        ///     and less than 1.0.
        /// </returns>
        float NextSingle();

        /// <summary>
        ///     Generate a random <see cref="System.Single" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudo random <see cref="System.Single" />.</returns>
        float NextSingle(float minValue, float maxValue);

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Single" /> number greater than or equal to zero,
        ///     and either strictly less than one, or less than or equal to one, depending on the value of the
        ///     given boolean parameter.
        /// </summary>
        /// <param name="includeOne">
        ///     If <see langword="true" />, the pseudo-random number returned will be
        ///     less than or equal to one; otherwise, the pseudo-random number returned will
        ///     be strictly less than one.
        /// </param>
        /// <returns>
        ///     If <paramref name="includeOne" /> is <see langword="true" />, this method returns a
        ///     <see cref="System.Single" />-precision pseudo-random number greater than or equal to zero, and less
        ///     than or equal to one. If <paramref name="includeOne" /> is <see langword="false" />,
        ///     this method returns a single-precision pseudo-random number greater than or equal to zero and
        ///     strictly less than one.
        /// </returns>
        float NextSingle(bool includeOne);

        /// <summary>
        ///     Returns a pseudo-random positive <see cref="System.Single" /> number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A pseudo-random number greater than 0.0 and less than 1.0.</returns>
        float NextSinglePositive();

        /// <summary>
        ///     Generates a new pseudo-random <see cref="System.UInt32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.UInt32" />.</returns>
        uint NextUnsignedInteger();

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.UInt32" /> up to <paramref name="maxValue" />.
        /// </summary>
        /// <param name="maxValue">
        ///     The maximum value of the pseudo-random number to create.
        /// </param>
        /// <returns>
        ///     A pseudo-random <see cref="System.UInt32" /> value which is at most <paramref name="maxValue" />.
        /// </returns>
        uint NextUnsignedInteger(uint maxValue);

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.UInt32" /> at least
        ///     <paramref name="minValue" /> and up to <paramref name="maxValue" />.
        /// </summary>
        /// <param name="minValue">The minimum value of the pseudo-random number to create.</param>
        /// <param name="maxValue">The maximum value of the pseudo-random number to create.</param>
        /// <returns>
        ///     A pseudo-random <see cref="System.UInt32" /> value which is at least
        ///     <paramref name="minValue" /> and at most <paramref name="maxValue" />.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If <c><paramref name="minValue" /> &gt;= <paramref name="maxValue" /></c>.
        /// </exception>
        uint NextUnsignedInteger(uint minValue, uint maxValue);
    }
}