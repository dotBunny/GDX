// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 64-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray64[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    public struct BitArray64
    {
        /// <summary>
        ///     First reserved <see cref="long" /> memory block (64-bits).
        /// </summary>
        /// <remarks>Indices 0-63</remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        public long Bits0;

        /// <summary>
        ///     Create a new <see cref="BitArray64" /> based on provided <paramref name="bits0" />.
        /// </summary>
        /// <param name="bits0">An existing value to be used to create the backing data for a <see cref="BitArray64" /></param>
        public BitArray64(long bits0)
        {
            Bits0 = bits0;
        }

        /// <summary>
        ///     Access bit in array.
        /// </summary>
        /// <param name="index">Target bit index.</param>
        public bool this[byte index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (Bits0 & (1 << index)) != 0;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                long negativeValue = value ? -1 : 0;
                Bits0 ^= (negativeValue ^ Bits0) & (1L << index);
            }
        }
    }
}