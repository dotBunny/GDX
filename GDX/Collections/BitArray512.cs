// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 512-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray512[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitArray512
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
        ///     Fifth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 128-159</remarks>
        public int Bits4;

        /// <summary>
        ///     Sixth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 160-191</remarks>
        public int Bits5;

        /// <summary>
        ///     Seventh reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 192-223</remarks>
        public int Bits6;

        /// <summary>
        ///     Eighth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 224-255</remarks>
        public int Bits7;

        /// <summary>
        ///     Ninth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 256-287</remarks>
        public int Bits8;

        /// <summary>
        ///     Tenth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 288-319</remarks>
        public int Bits9;

        /// <summary>
        ///     Eleventh reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 320-351</remarks>
        public int Bits10;

        /// <summary>
        ///     Twelfth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 352-383</remarks>
        public int Bits11;

        /// <summary>
        ///     Thirteenth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 384-415</remarks>
        public int Bits12;

        /// <summary>
        ///     Fourteenth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 416-447</remarks>
        public int Bits13;

        /// <summary>
        ///     Fifteenth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 448-479</remarks>
        public int Bits14;

        /// <summary>
        ///     Sixteenth reserved <see cref="System.Int32" /> memory block.
        /// </summary>
        /// <remarks>Indices 480-511</remarks>
        public int Bits15;

        /// <summary>
        ///     Access bit in array.
        /// </summary>
        /// <param name="index">Target bit index.</param>
        public unsafe bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int intIndex = (index & 511) >> 5;
                int bitIndex = index & 31;
                int intContainingBits;

                fixed (BitArray512* array = &this)
                {
                    intContainingBits = ((int*)array)[intIndex];
                }

                return (intContainingBits & (1 << bitIndex)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int intIndex = (index & 511) >> 5;
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