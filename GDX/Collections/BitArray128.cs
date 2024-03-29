// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 128-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray128[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitArray128
    {
        /// <summary>
        ///     First reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 0-31</remarks>
        public int Bits0;

        /// <summary>
        ///     Second reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 32-63</remarks>
        public int Bits1;

        /// <summary>
        ///     Third reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 64-95</remarks>
        public int Bits2;

        /// <summary>
        ///     Fourth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 96-127</remarks>
        public int Bits3;

        /// <summary>
        ///     Access bit in array.
        /// </summary>
        /// <param name="index">Target bit index.</param>
        public unsafe bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int intIndex = (index & 127) >> 5;
                int bitIndex = index & 31;
                int intContainingBits;
                fixed (BitArray128* array = &this) { intContainingBits = ((int*)array)[intIndex]; }

                return (intContainingBits & (1 << bitIndex)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int intIndex = (index & 127) >> 5;
                int bitIndex = index & 31;
                int negativeVal = value ? -1 : 0;
                fixed (int* array = &Bits0)
                {
                    array[intIndex] ^= (negativeVal ^ array[intIndex]) & (1 << bitIndex);
                }
            }
        }
    }
}