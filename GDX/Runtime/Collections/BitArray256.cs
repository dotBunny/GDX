// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GDX.Collections
{
    /// <summary>
    ///     A 256-bit array.
    /// </summary>
    /// <example>
    ///     Useful for packing a bunch of data with known indices tightly.
    ///     <code>
    ///         if(myBitArray256[1])
    ///         {
    ///             BeAwesome();
    ///         }
    ///     </code>
    /// </example>
    [StructLayout(LayoutKind.Sequential)]
    public struct BitArray256
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable FieldCanBeMadeReadOnly.Global

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

        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore FieldCanBeMadeReadOnly.Global

        /// <summary>
        ///     Access bit in array.
        /// </summary>
        /// <param name="index">Target bit index.</param>
        public unsafe bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                int intIndex = (index & 255) >> 5;
                int bitIndex = index & 31;
                int intContainingBits;

                fixed (BitArray256* array = &this)
                {
                    intContainingBits = ((int*)array)[intIndex];
                }

                return (intContainingBits & (1 << bitIndex)) != 0;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                int intIndex = (index & 255) >> 5;
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