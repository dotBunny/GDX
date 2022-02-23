// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX.Mathematics
{
    public static class FibonacciHash
    {
        /// <summary>
        /// Takes a 32-bit length equal to a power of two,
        /// and returns how many spaces another 32-bit int would need to shift in order to be a valid index within an array of that length.
        /// </summary>
        /// <param name="pow2Length">A 32-bit int equal to a power of two.</param>
        /// <returns>How many spaces a 32-bit int would need to shift in order to be a valid index within <paramref name="pow2Length" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetRightShiftFromPow2Length(int pow2Length)
        {
            LongDoubleConversionUnion u;
            u.doubleValue = 0.0;
            u.longValue = 0x4330000000000000L + pow2Length;
            u.doubleValue -= 4503599627370496.0;
            int index = (int)(u.longValue >> 52) - 0x3FF;

            return (byte)(32 - index);
        }

        /// <summary>
        /// Takes the <paramref name="hash" /> and multiplies it by 2^32 divided by the golden ratio,
        /// then right shifts it by <paramref name="shift" /> to fit within a given power-of-two size.
        /// </summary>
        /// <param name="hash">The key to find an index for.</param>
        /// <param name="shift">How far to right shift in order to fit within a given power-of-two size.</param>
        /// <returns>The index to store the <paramref name="hash" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndexFromHash(int hash, byte shift)
        {
            int fibonacci = unchecked((int)(unchecked((uint)hash) * 2654435769));
            return fibonacci >> shift;
        }

        /// <summary>
        /// Takes the <paramref name="hash" /> and finds an index within the provided <paramref name="pow2Length" /> range with Fibonacci hashing.
        /// </summary>
        /// <param name="hash">The hash to find an index for.</param>
        /// <param name="pow2Length">The power-of-two array length to find an index within.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetIndexFromHash(int hash, int pow2Length)
        {
            LongDoubleConversionUnion u;
            u.doubleValue = 0.0;
            u.longValue = 0x4330000000000000L + pow2Length;
            u.doubleValue -= 4503599627370496.0;
            int index = (int)(u.longValue >> 52) - 0x3FF;

            int shift = 32 - index;

            int fibonacci = unchecked((int)(unchecked((uint)hash) * 2654435769));
            return fibonacci >> shift;
        }
    }
}

