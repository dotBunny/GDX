// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
#if GDX_MATHEMATICS
using Unity.Mathematics;
#endif

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
    public struct WELL1024a : IRandomProvider
    {
        /// <summary>
        ///     A copy of the original seed used to initialize the <see cref="WELL1024a" />.
        /// </summary>
        public readonly uint OriginalSeed;

        /// <summary>
        ///     The state array of the well.
        /// </summary>
        public readonly uint[] State;

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
            // Initialize
            Index = 0;
            State = new uint[32];
            State[0] = OriginalSeed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                State[i] = (69069u * State[i - 1]) & 4294967295u;
            }
        }

        /// <summary>
        ///     Creates a new pseudorandom number generator with the given <paramref name="seed" />.
        /// </summary>
        /// <param name="seed">A <see cref="uint" /> value to use as a seed.</param>
        public WELL1024a(uint seed)
        {
            OriginalSeed = seed;

            // Initialize
            Index = 0;
            State = new uint[32];
            State[0] = OriginalSeed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                State[i] = (69069u * State[i - 1]) & 4294967295u;
            }
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

            // Initialize
            Index = 0;
            State = new uint[32];
            State[0] = OriginalSeed & 4294967295u;
            for (int i = 1; i < 32; ++i)
            {
                State[i] = (69069u * State[i - 1]) & 4294967295u;
            }
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
        ///     Get a <see cref="WellState" /> for the <see cref="WELL1024a" />.
        /// </summary>
        /// <remarks>Useful to save and restore the state of the <see cref="WELL1024a" />.</remarks>
        /// <returns></returns>
        public WellState GetState()
        {
            return new WellState {Index = Index, State = State, Seed = OriginalSeed};
        }

        /// <inheritdoc cref="IRandomProvider.NextBoolean"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool NextBoolean(float chance = 0.5f)
        {
            return Sample() <= chance;
        }

        /// <inheritdoc cref="IRandomProvider.NextBytes"/>
        public void NextBytes(byte[] buffer)
        {
            int bufLen = buffer.Length;
            for (int idx = 0; idx < bufLen; ++idx)
            {
                buffer[idx] = (byte)NextInteger(0, 256);
            }
        }

        /// <inheritdoc cref="IRandomProvider.NextDouble"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble(double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            return Range.GetDouble(Sample(), minValue, maxValue);
        }

        /// <inheritdoc cref="IRandomProvider.NextInteger"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextInteger(int minValue = int.MinValue, int maxValue = Range.SafeIntegerMaxValue)
        {
            return Range.GetInteger(Sample(), minValue, maxValue + 1);
        }

        /// <inheritdoc cref="IRandomProvider.NextIntegerExclusive"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int NextIntegerExclusive(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            return Range.GetInteger(Sample(), minValue, maxValue);
        }

        /// <inheritdoc cref="IRandomProvider.NextSingle"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextSingle(float minValue = float.MinValue, float maxValue = float.MaxValue)
        {
            return Range.GetSingle(Sample(), minValue, maxValue);
        }

        /// <inheritdoc cref="IRandomProvider.NextUnsignedInteger"/>
        public uint NextUnsignedInteger(uint minValue = uint.MinValue, uint maxValue = Range.SafeUnsignedIntegerMaxValue)
        {
            return Range.GetUnsignedInteger(Sample(), minValue, maxValue + 1);
        }

        /// <inheritdoc cref="IRandomProvider.NextUnsignedIntegerExclusive"/>
        public uint NextUnsignedIntegerExclusive(uint minValue = uint.MinValue, uint maxValue = uint.MaxValue)
        {
            return Range.GetUnsignedInteger(Sample(), minValue, maxValue);
        }

        /// <inheritdoc cref="IRandomProvider.Sample"/>
        public double Sample()
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