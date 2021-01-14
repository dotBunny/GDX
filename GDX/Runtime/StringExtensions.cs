// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ConstrainedExecution;
using System.Security;

namespace GDX
{
    /// <summary>
    ///     <see cref="System.String" /> Based Extension Methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     The ASCII character code for uppercase A.
        /// </summary>
        private const int AsciiUpperCaseStart = 65;

        /// <summary>
        ///     The ASCII  character code for uppercase Z.
        /// </summary>
        private const int AsciiUpperCaseEnd = 90;

        /// <summary>
        ///     The ASCII character code for lowercase a.
        /// </summary>
        private const int AsciiLowerCaseStart = 97;

        /// <summary>
        ///     The ASCII character code for lowercase z.
        /// </summary>
        private const int AsciiLowerCaseEnd = 122;

        /// <summary>
        ///     The ASCII character code shift value required to change the case of a letter.
        /// </summary>
        private const int CaseShift = 32;

        /// <summary>
        ///     <para>
        ///         Get the hash code value of <paramref name="targetString" /> (converted to an uppercase
        ///         <see cref="System.String" />).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function. It's value will be identical
        ///     to the value produced natively by processing a <see cref="System.String" /> with
        ///     <see cref="System.String.ToUpper()" />.<see cref="System.String.GetHashCode()" />, but with no
        ///     allocations.
        /// </remarks>
        /// <param name="targetString">The target <see cref="System.String" />.</param>
        /// <returns>A <see cref="System.Int32" /> value.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe int GetUpperCaseHashCode(this string targetString)
        {
            fixed (char* src = targetString)
            {
                int hash1 = 5381;
                int hash2 = hash1;
                int c;
                char* s = src;

                // Get character
                while ((c = s[0]) != 0)
                {
                    // Check character value and shift it if necessary (32)
                    if (c >= AsciiLowerCaseStart && c <= AsciiLowerCaseEnd)
                    {
                        c ^= CaseShift;
                    }

                    // Add to Hash #1
                    hash1 = ((hash1 << 5) + hash1) ^ c;

                    // Get our second character
                    c = s[1];

                    if (c == 0)
                    {
                        break;
                    }

                    // Check character value and shift it if necessary (32)
                    if (c >= AsciiLowerCaseStart && c <= AsciiLowerCaseEnd)
                    {
                        c ^= CaseShift;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ c;
                    s += 2;
                }

                return hash1 + hash2 * 1566083941;
            }
        }

        /// <summary>
        ///     <para>
        ///         Get the hash code value of <paramref name="targetString" /> (converted to an uppercase
        ///         <see cref="System.String" />).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function. It's value will be identical
        ///     to the value produced natively by processing a <see cref="System.String" /> with
        ///     <see cref="System.String.ToLower()" />.<see cref="System.String.GetHashCode()" />, but with no
        ///     allocations.
        /// </remarks>
        /// <param name="targetString">The target <see cref="System.String" />.</param>
        /// <returns>A <see cref="System.Int32" /> value.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe int GetLowerCaseHashCode(this string targetString)
        {
            fixed (char* src = targetString)
            {
                int hash1 = 5381;
                int hash2 = hash1;
                int c;
                char* s = src;

                // Get character
                while ((c = s[0]) != 0)
                {
                    // Check character value and shift it if necessary (32)
                    if (c >= AsciiUpperCaseStart && c <= AsciiUpperCaseEnd)
                    {
                        c ^= CaseShift;
                    }

                    // Add to Hash #1
                    hash1 = ((hash1 << 5) + hash1) ^ c;

                    // Get our second character
                    c = s[1];

                    if (c == 0)
                    {
                        break;
                    }

                    // Check character value and shift it if necessary (32)
                    if (c >= AsciiUpperCaseStart && c <= AsciiUpperCaseEnd)
                    {
                        c ^= CaseShift;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ c;
                    s += 2;
                }

                return hash1 + hash2 * 1566083941;
            }
        }
    }
}