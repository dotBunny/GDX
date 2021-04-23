using System;
using System.Runtime.CompilerServices;

namespace GDX.Mathematics.Random
{
    // http://lomont.org/papers/2008/Lomont_PRNG_2008.pdf
    // http://www.iro.umontreal.ca/~panneton/WELLRNG.html
    // http://www.iro.umontreal.ca/~panneton/well/WELL1024a.c

    // ReSharper disable once InconsistentNaming
    public class WELL1024a
    {
        private readonly uint[] _state = new uint[32];

        /// <summary>
        ///     A copy of the original seed used to initialize the <see cref="WELL1024a" />.
        /// </summary>
        public readonly uint OriginalSeed;

        private uint _index;

        public WELL1024a(int seed)
        {
            OriginalSeed = (uint)seed;
            Initialize(OriginalSeed);
        }

        public WELL1024a(uint seed)
        {
            OriginalSeed = seed;
            Initialize(OriginalSeed);
        }

        public WELL1024a(string seed, bool forceUpperCase = true)
        {
            if (forceUpperCase)
            {
                OriginalSeed = (uint)seed.GetStableUpperCaseHashCode();
            }
            else
            {
                OriginalSeed = (uint)seed.GetHashCode();
            }

            Initialize(OriginalSeed);
        }

        public WELL1024a(WellState restoreState)
        {
            OriginalSeed = restoreState.Seed;
            _index = restoreState.Index;
            _state = restoreState.State;
        }

        private void Initialize(uint seed)
        {
            _state[0] = seed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                _state[i] = (69069u * _state[i - 1]) & 4294967295u;
            }
        }

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Int32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Int32" /> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInteger()
        {
            throw new NotImplementedException();
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInteger(int minValue, int maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (maxValue == minValue)
            {
                return minValue;
            }

            return NextInteger() * (maxValue - minValue) + minValue;
        }

        /// <summary>
        ///     Returns a pseudo random <see cref="System.Boolean" />. value based on chance (<c>0</c>-<c>1</c> roll),
        ///     favoring false.
        /// </summary>
        /// <param name="chance">The 0-1 percent chance of success.</param>
        /// <returns>A pseudo random <see cref="System.Boolean" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBias(float chance)
        {
            // No chance at all
            if (chance == 0f)
            {
                return false;
            }

            // Roll the dice
            float pseudoRandomValue = NextSingle(0.0f, 1.0f);

            // If it's inclusive we nailed it
            return pseudoRandomValue <= chance;
        }

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="System.Boolean" /> value of either true or false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBoolean()
        {
            return NextBias(0.5f);
        }

        /// <summary>
        ///     Fills a buffer with pseudo-random <see cref="System.Byte" />.
        /// </summary>
        /// <param name="buffer">The buffer to fill.</param>
        /// <exception cref="ArgumentNullException">
        ///     If <c><paramref name="buffer" /> == <see langword="null" /></c>.
        /// </exception>
        public void NextBytes(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException();
            }

            int bufLen = buffer.Length;

            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)NextInteger(0, 256);
            }
        }

        /// <summary>
        ///     Returns the next pseudo-random <see cref="System.Double" /> value.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.Double" /> floating point value.</returns>
        public double NextDouble()
        {
            uint a = _state[(_index + 3u) & 31u];
            uint z1 = _state[_index] ^ a ^ (a >> 8);
            uint b = _state[(_index + 24u) & 31u];
            uint c = _state[(_index + 10u) & 31u];
            uint z2 = b ^ (b << 19) ^ c ^ (c << 14);

            _state[_index] = z1 ^ z2;
            uint d = _state[(_index + 31u) & 31u];
            _state[(_index + 31u) & 31u] = d ^ (d << 11) ^ z1 ^ (z1 << 7) ^ z2 ^ (z2 << 13);
            _index = (_index + 31u) & 31u;

            return _state[_index] * 2.32830643653869628906e-10d;
        }

        /// <summary>
        ///     Returns a pseudo-random <see cref="System.Single" /> number between 0.0 and 1.0.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.Single" />-precision floating point number greater than or equal to 0.0,
        ///     and less than 1.0.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSingle()
        {
            return (float)NextDouble();
        }

        /// <summary>
        ///     Generate a random <see cref="System.Single" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" /> .
        /// </summary>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudo random <see cref="System.Single" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSingle(float minValue, float maxValue)
        {
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (maxValue == minValue)
            {
                return minValue;
            }

            return (float)(NextDouble() * (maxValue - minValue) + minValue);
        }

        /// <summary>
        ///     Generates a new pseudo-random <see cref="System.UInt32" />.
        /// </summary>
        /// <returns>A pseudo-random <see cref="System.UInt32" />.</returns>
        public uint NextUnsignedInteger()
        {
            throw new NotImplementedException();
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
        public uint NextUnsignedInteger(uint minValue, uint maxValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     A complete state of <see cref="WELL1024a" />.
        /// </summary>
        public struct WellState
        {
            /// <summary>
            ///     The seed used to originally create the <see cref="WELL1024a" />.
            /// </summary>
            public uint Seed;

            /// <summary>
            ///     The internal state index.
            /// </summary>
            public uint Index;

            /// <summary>
            ///     The internal state array.
            /// </summary>
            public uint[] State;
        }
    }
}