// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Mathematics.Random
{
    /// <summary>
    ///     Generates pseudo-random value using the Mersenne Twister algorithm.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Adapted from the work of Akihilo Kramot (Takel), originally based on a
    ///         <a href="http://www.math.sci.hiroshima-u.ac.jp/m-mat/MT/MT2002/CODES/mt19937ar.c">
    ///             C-program for MT19937
    ///         </a>
    ///         by Takuji Nishimura, considering the suggestions by Topher Cooper and Marc Rieffel in July-Aug. 1997.
    ///         See the
    ///         <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">
    ///             Mersenne Twister Homepage
    ///         </a>
    ///         for details on the algorithm.
    ///     </para>
    /// </remarks>
    [VisualScripting(VisualScriptingAttribute.Category.Types)]
    public class MersenneTwister : System.Random
    {
        /// <summary>
        ///     The degree of recurrence.
        /// </summary>
        /// <remarks>
        ///     Size of byte pool.
        /// </remarks>
        private const int N = 624;

        /// <summary>
        ///     The middle word value.
        /// </summary>
        /// <remarks>
        ///     Period second parameter.
        /// </remarks>
        private const int M = 397;

        /// <summary>
        ///     The most significant w-r bits.
        /// </summary>
        private const uint UpperMask = 0x80000000;

        /// <summary>
        ///     Least significant r bits.
        /// </summary>
        private const uint LowerMask = 0x7fffffff;

        /// <summary>
        ///     A magic inducing value.
        /// </summary>
        /// <remarks>
        ///     A value of 9007199254740991.0 is the maximum <see cref="System.Double" /> value which the 53
        ///     significant can hold when the exponent is 0.
        /// </remarks>
        private const double FiftyThreeBitsOf1S = 9007199254740991.0;

        /// <summary>
        ///     Inverse of <see cref="FiftyThreeBitsOf1S" />.
        /// </summary>
        /// <remarks>
        ///     Multiply by inverse to (vainly?) try to avoid a division.
        /// </remarks>
        private const double Inverse53BitsOf1S = 1.0 / FiftyThreeBitsOf1S;

        /// <summary>
        ///     1 plus <see cref="FiftyThreeBitsOf1S" />.
        /// </summary>
        private const double OnePlus53BitsOf1S = FiftyThreeBitsOf1S + 1;

        /// <summary>
        ///     Inverse of <see cref="OnePlus53BitsOf1S" />.
        /// </summary>
        private const double InverseOnePlus53BitsOf1S = 1.0 / OnePlus53BitsOf1S;

        /// <summary>
        ///     Magnitude lookup.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly uint[] s_mag01 = {0x0, 0x9908b0df};

        /// <summary>
        ///     The array for the state vector of the twister.
        /// </summary>
        /// <remarks>Also known as it's byte pool.</remarks>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private readonly uint[] _mersenneTwisterState = new uint[N];

        /// <summary>
        ///     The current index in the array for the state of the twister.
        /// </summary>
        private short _mersenneTwisterIndex;

        /// <summary>
        ///     Creates a new pseudo-random number generator with the given <paramref name="seed" />.
        /// </summary>
        /// <param name="seed">A <see cref="System.Int32" /> value to use as a seed.</param>
        public MersenneTwister(int seed)
        {
            Initialize((uint)seed);
        }

        /// <summary>
        ///     Creates a new pseudo-random number generator with a default seed.
        /// </summary>
        /// <remarks>
        ///     <c>new <see cref="System.Random" />().<see cref="System.Random.Next()" /></c>
        ///     is used for the seed.
        /// </remarks>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        public MersenneTwister() : this(new System.Random().Next())
        {
        }

        /// <summary>
        ///     Creates a pseudo-random number generator initialized with the given array.
        /// </summary>
        /// <param name="initKey">The array for <see cref="System.Int32" /> initializing keys.</param>
        public MersenneTwister(int[] initKey)
        {
            if (initKey == null)
            {
                throw new ArgumentNullException(nameof(initKey));
            }

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            uint[] initArray = new uint[initKey.Length];

            int i = 0;
            for (; i < initKey.Length; ++i)
            {
                initArray[i] = (uint)initKey[i];
            }

            InitializeByKeys(initArray);
        }

        /// <summary>
        ///     Generates a new pseudo-random <see cref="System.UInt32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.UInt32" />.</returns>
        private uint GenerateUnsignedInteger()
        {
            uint y;
            if (_mersenneTwisterIndex >= N)
            {
                short kk = 0;

                for (; kk < N - M; ++kk)
                {
                    y = (_mersenneTwisterState[kk] & UpperMask) | (_mersenneTwisterState[kk + 1] & LowerMask);
                    _mersenneTwisterState[kk] = _mersenneTwisterState[kk + M] ^ (y >> 1) ^ s_mag01[y & 0x1];
                }

                for (; kk < N - 1; ++kk)
                {
                    y = (_mersenneTwisterState[kk] & UpperMask) | (_mersenneTwisterState[kk + 1] & LowerMask);
                    _mersenneTwisterState[kk] = _mersenneTwisterState[kk + (M - N)] ^ (y >> 1) ^ s_mag01[y & 0x1];
                }

                y = (_mersenneTwisterState[N - 1] & UpperMask) | (_mersenneTwisterState[0] & LowerMask);
                _mersenneTwisterState[N - 1] = _mersenneTwisterState[M - 1] ^ (y >> 1) ^ s_mag01[y & 0x1];

                _mersenneTwisterIndex = 0;
            }

            y = _mersenneTwisterState[_mersenneTwisterIndex++];

            // Apply bit shifts (aka Tempering)
            y ^= y >> 11;
            y ^= (y << 7) & 0x9d2c5680;
            y ^= (y << 15) & 0xefc60000;
            y ^= y >> 18;

            return y;
        }

        private void Initialize(uint seed)
        {
            _mersenneTwisterState[0] = seed & 0xffffffffU;

            for (_mersenneTwisterIndex = 1; _mersenneTwisterIndex < N; _mersenneTwisterIndex++)
            {
                _mersenneTwisterState[_mersenneTwisterIndex] =
                    (uint)(1812433253U * (_mersenneTwisterState[_mersenneTwisterIndex - 1] ^
                                          (_mersenneTwisterState[_mersenneTwisterIndex - 1] >> 30)) +
                           _mersenneTwisterIndex);

                // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
                // In the previous versions, MSBs of the seed affect
                // only MSBs of the array _mt[].
                // 2002/01/09 modified by Makoto Matsumoto
                _mersenneTwisterState[_mersenneTwisterIndex] &= 0xffffffffU;
                // for >32 bit machines
            }
        }

        private void InitializeByKeys(uint[] keys)
        {
            // Default initializer
            Initialize(19650218U);

            int keyLength = keys.Length;
            int i = 1;
            int j = 0;
            int k = N > keyLength ? N : keyLength;

            for (; k > 0; k--)
            {
                // Non-linear
                _mersenneTwisterState[i] =
                    (uint)((_mersenneTwisterState[i] ^
                            ((_mersenneTwisterState[i - 1] ^ (_mersenneTwisterState[i - 1] >> 30)) * 1664525U)) +
                           keys[j] + j);

                // For WORDSIZE > 32 machines
                _mersenneTwisterState[i] &= 0xffffffffU;
                i++;
                j++;
                if (i >= N)
                {
                    _mersenneTwisterState[0] = _mersenneTwisterState[N - 1];
                    i = 1;
                }

                if (j >= keyLength)
                {
                    j = 0;
                }
            }

            for (k = N - 1; k > 0; k--)
            {
                // Non-linear
                _mersenneTwisterState[i] = (uint)((_mersenneTwisterState[i] ^
                                                   ((_mersenneTwisterState[i - 1] ^
                                                     (_mersenneTwisterState[i - 1] >> 30)) * 1566083941U)) -
                                                  i);
                // For WORDSIZE > 32 machines
                _mersenneTwisterState[i] &= 0xffffffffU;
                i++;

                if (i < N)
                {
                    continue;
                }

                _mersenneTwisterState[0] = _mersenneTwisterState[N - 1];
                i = 1;
            }

            // MSB is 1; assuring non-zero initial array
            _mersenneTwisterState[0] = 0x80000000U;
        }

        private double Compute53BitRandom(double translate, double scale)
        {
            ulong a = (ulong)GenerateUnsignedInteger() >> 5;
            ulong b = (ulong)GenerateUnsignedInteger() >> 6;
            return (a * 67108864.0 + b + translate) * scale;
        }

        #region Random Value Getters

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.UInt32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.UInt32" /> value.</returns>
        public virtual uint NextUInt()
        {
            return GenerateUnsignedInteger();
        }

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.UInt32" /> up to <paramref name="maxValue" />.
        /// </summary>
        /// <param name="maxValue">
        ///     The maximum value of the pseudo-random number to create.
        /// </param>
        /// <returns>
        ///     A pseudo-random <see cref="System.UInt32" /> value which is at most <paramref name="maxValue" />.
        /// </returns>
        public virtual uint NextUInt(uint maxValue)
        {
            return (uint)(GenerateUnsignedInteger() / ((double)uint.MaxValue / maxValue));
        }

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
        public virtual uint NextUInt(uint minValue, uint maxValue) /* throws ArgumentOutOfRangeException */
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (uint)(GenerateUnsignedInteger() / ((double)uint.MaxValue / (maxValue - minValue)) + minValue);
        }

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Int32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Int32" /> value.</returns>
        public override int Next()
        {
            return Next(int.MaxValue);
        }

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
        public override int Next(int maxValue)
        {
            if (maxValue > 1)
            {
                return (int)(NextDouble() * maxValue);
            }

            if (maxValue < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return 0;
        }

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
        public override int Next(int minValue, int maxValue)
        {
            if (maxValue <= minValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (maxValue == minValue)
            {
                return minValue;
            }

            return Next(maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Fills a buffer with pseudo-random <see cref="System.Byte" />.
        /// </summary>
        /// <param name="buffer">The buffer to fill.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <c><paramref name="buffer" /> == <see langword="null" /></c>.
        /// </exception>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            int bufLen = buffer.Length;

            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)Next(256);
            }
        }

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Double" /> value.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Double" /> floating point value.</returns>
        /// <remarks>
        ///     <para>
        ///         There are two common ways to create a double floating point using MT19937:
        ///         using <see cref="GenerateUnsignedInteger" /> and dividing by 0xFFFFFFFF + 1,
        ///         or else generating two double words and shifting the first by 26 bits and
        ///         adding the second.
        ///     </para>
        ///     <para>
        ///         In a newer measurement of the randomness of MT19937 published in the
        ///         journal "Monte Carlo Methods and Applications, Vol. 12, No. 5-6, pp. 385 – 393 (2006)"
        ///         entitled "A Repetition Test for Pseudo-Random Number Generators",
        ///         it was found that the 32-bit version of generating a double fails at the 95%
        ///         confidence level when measuring for expected repetitions of a particular
        ///         number in a sequence of numbers generated by the algorithm.
        ///     </para>
        ///     <para>
        ///         Due to this, the 53-bit method is implemented here and the 32-bit method
        ///         of generating a double is not. If, for some reason,
        ///         the 32-bit method is needed, it can be generated by the following:
        ///         <code>
        ///             (System.Double)NextUInt32() / ((UInt64)UInt32.MaxValue + 1);
        ///         </code>
        ///     </para>
        /// </remarks>
        public override double NextDouble()
        {
            return Compute53BitRandom(0, InverseOnePlus53BitsOf1S);
        }

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
        public double NextDouble(bool includeOne)
        {
            return includeOne ? Compute53BitRandom(0, Inverse53BitsOf1S) : NextDouble();
        }

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Double" /> number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Double" /> number greater than 0.0 and less than 1.0.</returns>
        public double NextDoublePositive()
        {
            return Compute53BitRandom(0.5, Inverse53BitsOf1S);
        }

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Single" /> number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Single" />-precision floating point number greater than or equal to 0.0,
        ///     and less than 1.0.
        /// </returns>
        public float NextSingle()
        {
            return (float)NextDouble();
        }

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
        public float NextSingle(bool includeOne)
        {
            return (float)NextDouble(includeOne);
        }

        /// <summary>
        ///     Returns a pseudo-random positive <see cref="System.Single" /> number greater than 0.0 and less than 1.0.
        /// </summary>
        /// <returns>A pseudo-random number greater than 0.0 and less than 1.0.</returns>
        public float NextSinglePositive()
        {
            return (float)NextDoublePositive();
        }

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="System.Boolean" /> value of either true or false.</returns>
        public bool NextBoolean()
        {
            return Next(1) == 1;
        }

        #endregion
    }
}