using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace GDX.Mathematics.Random
{
    // ReSharper disable CommentTypo
    /// <summary>
    ///     Generates pseudorandom values based on the WELL1024a algorithm.
    /// </summary>
    /// <remarks>
    ///     Primarily based on the work of <a href="http://lomont.org/papers/2008/Lomont_PRNG_2008.pdf">Chris Lomont</a>,
    ///     accessed on 2021-04-23.
    ///     Additional understanding from
    ///     <a href="http://www.iro.umontreal.ca/~lecuyer/myftp/papers/lfsr04.pdf">Francois Panneton and Pierre L`Ecuyer</a>,
    ///     accessed on 2021-04-23.
    /// </remarks>
    // ReSharper restore CommentTypo
    [VisualScriptingType]
    // ReSharper disable once InconsistentNaming
    public class WELL1024a
    {
        /// <summary>
        ///     A copy of the original seed used to initialize the <see cref="WELL1024a" />.
        /// </summary>
        public readonly uint OriginalSeed;

        /// <summary>
        ///     The state array of the well.
        /// </summary>
        public readonly uint[] State = new uint[32];

        /// <summary>
        ///     The current index of use for the well array.
        /// </summary>
        /// <remarks>CAUTION! Changing this will alter the understanding of the data.</remarks>
        public uint Index;

        /// <summary>
        ///     Creates a new pseudorandom number generator with the given <paramref name="seed" />.
        /// </summary>
        /// <remarks>
        ///     The <paramref name="seed" /> will have its sign stripped and stored as such in
        ///     <see cref="OriginalSeed" />.
        /// </remarks>
        /// <param name="seed">A <see cref="int" /> value to use as a seed.</param>
        public WELL1024a(int seed)
        {
#if GDX_MATHEMATICS
            OriginalSeed = (uint)math.abs(seed);
#else
            OriginalSeed = (uint)UnityEngine.Mathf.Abs(seed);
#endif
            Initialize(OriginalSeed);
        }

        /// <summary>
        ///     Creates a new pseudorandom number generator with the given <paramref name="seed" />.
        /// </summary>
        /// <param name="seed">A <see cref="uint" /> value to use as a seed.</param>
        public WELL1024a(uint seed)
        {
            OriginalSeed = seed;
            Initialize(OriginalSeed);
        }

        /// <summary>
        ///     Creates a new pseudorandom number generator with the given <paramref name="seed" />.
        /// </summary>
        /// <remarks>
        ///     The created hashcode will have its sign stripped and stored as such in
        ///     <see cref="OriginalSeed" />.
        /// </remarks>
        /// <param name="seed">A <see cref="string" /> to create a <see cref="uint" /> seed from.</param>
        /// <param name="forceUpperCase">
        ///     Should the generated hashcode used as the seed be generated from an uppercase version of
        ///     the <paramref name="seed" />.
        /// </param>
        public WELL1024a(string seed, bool forceUpperCase = true)
        {
            if (forceUpperCase)
            {
#if GDX_MATHEMATICS
                OriginalSeed = (uint)math.abs(seed.GetStableUpperCaseHashCode());
#else
                OriginalSeed = (uint)UnityEngine.Mathf.Abs(seed.GetStableUpperCaseHashCode());
#endif
            }
            else
            {
#if GDX_MATHEMATICS
                OriginalSeed = (uint)math.abs(seed.GetHashCode());
#else
                OriginalSeed = (uint)UnityEngine.Mathf.Abs(seed.GetHashCode());
#endif
            }

            Initialize(OriginalSeed);
        }

        /// <summary>
        ///     Create a pseudorandom number generator from a <paramref name="restoreState" />.
        /// </summary>
        /// <param name="restoreState">A saved <see cref="WELL1024a" /> state.</param>
        public WELL1024a(WellState restoreState)
        {
            OriginalSeed = restoreState.Seed;
            Index = restoreState.Index;
            State = restoreState.State;
        }

        /// <summary>
        ///     Prepare the <see cref="WELL1024a" /> with the provided seed.
        /// </summary>
        /// <param name="seed">A <see cref="uint" /> value.</param>
        private void Initialize(uint seed)
        {
            State[0] = seed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                State[i] = (69069u * State[i - 1]) & 4294967295u;
            }
        }

        /// <summary>
        ///     Get a <see cref="WellState" /> for the <see cref="WELL1024a" />.
        /// </summary>
        /// <remarks>Useful to save and restore the state of the <see cref="WELL1024a" />.</remarks>
        /// <returns></returns>
        public WellState GetState()
        {
            return new WellState {Index = Index, State = State, Seed = OriginalSeed};
        }

        /// <summary>
        ///     Returns the next pseudorandom <see cref="double" /> value .
        /// </summary>
        /// <returns>A pseudorandom <see cref="double" /> floating point value.</returns>
        public double Next()
        {
            uint a = State[(Index + 3u) & 31u];
            uint z1 = State[Index] ^ a ^ (a >> 8);
            uint b = State[(Index + 24u) & 31u];
            uint c = State[(Index + 10u) & 31u];
            uint z2 = b ^ (b << 19) ^ c ^ (c << 14);

            State[Index] = z1 ^ z2;
            uint d = State[(Index + 31u) & 31u];
            State[(Index + 31u) & 31u] = d ^ (d << 11) ^ z1 ^ (z1 << 7) ^ z2 ^ (z2 << 13);
            Index = (Index + 31u) & 31u;

            return State[Index] * 2.32830643653869628906e-10d;
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
        /// <remarks>
        ///     The <paramref name="buffer" /> shouldn't be <see lanwgword="null" />.
        /// </remarks>
        /// <param name="buffer">The buffer to fill.</param>
        public void NextBytes(byte[] buffer)
        {
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
        [Serializable]
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