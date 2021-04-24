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

        /// <summary>
        ///     Prepare the <see cref="WELL1024a" /> with the provided seed.
        /// </summary>
        /// <param name="seed">A <see cref="uint" /> value.</param>
        private void Initialize(uint seed)
        {
            _state[0] = seed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                _state[i] = (69069u * _state[i - 1]) & 4294967295u;
            }
        }

        /// <summary>
        ///     Returns the next pseudorandom <see cref="double" /> value .
        /// </summary>
        /// <returns>A pseudorandom <see cref="double" /> floating point value.</returns>
        public double Next()
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
        ///     Returns a pseudorandom <see cref="System.Boolean" /> value based on chance (<c>0</c>-<c>1</c> roll),
        ///     favoring false.
        /// </summary>
        /// <param name="chance">The 0-1 percent chance of success.</param>
        /// <returns>A pseudorandom <see cref="System.Boolean" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBias(float chance)
        {
            // No chance at all
            if (chance == 0f)
            {
                return false;
            }

            // If it's inclusive we nailed it
            return Next() <= chance;
        }

        /// <summary>
        ///     Returns a pseudorandom <see cref="System.Boolean" />.
        /// </summary>
        /// <returns>A <see cref="System.Boolean" /> value of either true or false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBoolean()
        {
            return NextBias(0.5f);
        }

        /// <summary>
        ///     Fills a buffer with pseudorandom <see cref="System.Byte" />.
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
        ///     Returns the next pseudorandom <see cref="double" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudorandom <see cref="double" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble(double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            return Range.GetDouble(Next(), minValue, maxValue);
        }

        /// <summary>
        ///     Returns the next pseudorandom <see cref="int" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudorandom <see cref="int" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInteger(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return Range.GetInteger(Next(), minValue, maxValue);
        }

        /// <summary>
        ///     Returns the next pseudorandom <see cref="System.Single" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudorandom <see cref="System.Single" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSingle(float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return Range.GetSingle(Next(), minValue, maxValue);
        }

        /// <summary>
        ///     Returns the next pseudorandom <see cref="uint" /> between <paramref name="minValue" /> and
        ///     <paramref name="maxValue" />.
        /// </summary>
        /// <remarks>
        ///     <paramref name="minValue" /> should not be greater then <paramref name="maxValue" />.
        /// </remarks>
        /// <param name="minValue">The lowest possible value.</param>
        /// <param name="maxValue">The highest possible value.</param>
        /// <returns>A pseudorandom <see cref="uint" />.</returns>
        public uint NextUnsignedInteger(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            return Range.GetUnsignedInteger(Next(), minValue, maxValue);
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