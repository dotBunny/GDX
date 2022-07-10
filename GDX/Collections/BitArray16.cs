// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 16-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray16[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    [Serializable]
    public struct BitArray16
    {
        /// <summary>
        ///     First reserved <see cref="ushort" /> memory block (16-bits).
        /// </summary>
        /// <remarks>Indices 0-15</remarks>
        public ushort Bits0;

        /// <summary>
        ///     Create a new <see cref="BitArray16" /> based on provided <paramref name="bits0" />.
        /// </summary>
        /// <param name="bits0">An existing value to be used to create the backing data for a <see cref="BitArray16" /></param>
        public BitArray16(ushort bits0)
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
                if (value)
                {
                    Bits0 = (ushort)(Bits0 | (1 << index));
                }
                else
                {
                    Bits0 = (ushort)(Bits0 & ~(1 << index));
                }
            }
        }
    }
}