// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Mathematics
{
    /// <summary>
    ///     The 32-bit MurmurHash3 (x86_32) by Austin Appleby: a fast, well mixed hash of integers and bytes.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Integer math only, so a value is the same on every platform, runtime and compiler: suitable for seeds
    ///         that must match across devices, unlike <see cref="object.GetHashCode" />.
    ///     </para>
    ///     <para>
    ///         Hashing words gives the same value as hashing their bytes in little-endian order, so
    ///         <c>Hash(a, b, seed)</c> equals <c>Hash(bytes of a then b, seed)</c>. Not a cryptographic hash.
    ///     </para>
    /// </remarks>
    public static class Murmur3
    {
        const uint k_C1 = 0xcc9e2d51u;
        const uint k_C2 = 0x1b873593u;

        /// <summary>
        ///     The MurmurHash3 finalizer: every bit of <paramref name="value" /> affects every bit of the result.
        /// </summary>
        /// <param name="value">The value to mix.</param>
        /// <returns>The mixed value. Zero mixes to zero.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Mix(uint value)
        {
            value ^= value >> 16;
            value *= 0x85ebca6bu;
            value ^= value >> 13;
            value *= 0xc2b2ae35u;
            value ^= value >> 16;
            return value;
        }

        /// <summary>
        ///     Hash one 32-bit word.
        /// </summary>
        /// <param name="a">The word.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of the word's four bytes.</returns>
        public static uint Hash(uint a, uint seed)
        {
            uint hash = Block(seed, a);
            return Mix(hash ^ 4u);
        }

        /// <summary>
        ///     Hash two 32-bit words, in order.
        /// </summary>
        /// <param name="a">The first word.</param>
        /// <param name="b">The second word.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of the words' eight bytes.</returns>
        public static uint Hash(uint a, uint b, uint seed)
        {
            uint hash = Block(seed, a);
            hash = Block(hash, b);
            return Mix(hash ^ 8u);
        }

        /// <summary>
        ///     Hash three 32-bit words, in order.
        /// </summary>
        /// <param name="a">The first word.</param>
        /// <param name="b">The second word.</param>
        /// <param name="c">The third word.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of the words' twelve bytes.</returns>
        public static uint Hash(uint a, uint b, uint c, uint seed)
        {
            uint hash = Block(seed, a);
            hash = Block(hash, b);
            hash = Block(hash, c);
            return Mix(hash ^ 12u);
        }

        /// <summary>
        ///     Hash four 32-bit words, in order.
        /// </summary>
        /// <param name="a">The first word.</param>
        /// <param name="b">The second word.</param>
        /// <param name="c">The third word.</param>
        /// <param name="d">The fourth word.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of the words' sixteen bytes.</returns>
        public static uint Hash(uint a, uint b, uint c, uint d, uint seed)
        {
            uint hash = Block(seed, a);
            hash = Block(hash, b);
            hash = Block(hash, c);
            hash = Block(hash, d);
            return Mix(hash ^ 16u);
        }

        /// <summary>
        ///     Hash an array of bytes.
        /// </summary>
        /// <param name="data">The bytes to hash.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of all of <paramref name="data" />.</returns>
        public static uint Hash(byte[] data, uint seed)
        {
            return Hash(data, 0, data.Length, seed);
        }

        /// <summary>
        ///     Hash a part of an array of bytes.
        /// </summary>
        /// <param name="data">The bytes.</param>
        /// <param name="offset">The index of the first byte to hash.</param>
        /// <param name="count">How many bytes to hash.</param>
        /// <param name="seed">The seed.</param>
        /// <returns>The hash of <paramref name="count" /> bytes from <paramref name="offset" />.</returns>
        public static uint Hash(byte[] data, int offset, int count, uint seed)
        {
            uint hash = seed;
            int blockEnd = offset + (count & ~3);
            int index = offset;
            while (index < blockEnd)
            {
                uint word = data[index] | ((uint)data[index + 1] << 8) | ((uint)data[index + 2] << 16) |
                            ((uint)data[index + 3] << 24);
                hash = Block(hash, word);
                index += 4;
            }

            uint tail = 0;
            switch (count & 3)
            {
                case 3:
                    tail ^= (uint)data[index + 2] << 16;
                    tail ^= (uint)data[index + 1] << 8;
                    tail ^= data[index];
                    hash ^= Scramble(tail);
                    break;
                case 2:
                    tail ^= (uint)data[index + 1] << 8;
                    tail ^= data[index];
                    hash ^= Scramble(tail);
                    break;
                case 1:
                    tail ^= data[index];
                    hash ^= Scramble(tail);
                    break;
            }

            return Mix(hash ^ (uint)count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Scramble(uint word)
        {
            word *= k_C1;
            word = (word << 15) | (word >> 17);
            return word * k_C2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static uint Block(uint hash, uint word)
        {
            hash ^= Scramble(word);
            hash = (hash << 13) | (hash >> 19);
            return hash * 5u + 0xe6546b64u;
        }
    }
}
