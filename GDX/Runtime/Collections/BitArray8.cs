// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 8-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray8[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    public struct BitArray8
    {
        /// <summary>
        ///     First reserved <see cref="byte" /> memory block (8-bits).
        /// </summary>
        /// <remarks>Indices 0-7</remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        public byte Bits0;

        /// <summary>
        ///     Create a new <see cref="BitArray8" /> based on provided <paramref name="bits0" />.
        /// </summary>
        /// <param name="bits0">An existing value to be used to create the backing data for a <see cref="BitArray8" /></param>
        public BitArray8(byte bits0)
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
                    Bits0 = (byte)(Bits0 | (1 << index));
                }
                else
                {
                    Bits0 = (byte)(Bits0 & ~(1 << index));
                }
            }
        }
    }
}