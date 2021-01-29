// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX
{
    /// <summary>
    ///     <see cref="System.String" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     <i>Unit tests are found in GDX.Tests.EditMode, under Runtime.StringExtensionsTests.</i>
    /// </remarks>
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
        ///     The default encryption key used when none is provided to the encryption related extensions.
        /// </summary>
        /// <remarks>
        ///     You can change this at runtime during some sort of initialization pass to being something unique to your project,
        ///     but it is not absolutely necessary. This must be a multiple of 8 bytes.
        /// </remarks>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static byte[] EncryptionDefaultKey = Encoding.UTF8.GetBytes("Awesome!");

        /// <summary>
        ///     The IV (Initialization Vector) provided to the <see cref="DESCryptoServiceProvider" />.
        /// </summary>
        /// <remarks>
        ///     You can change this at runtime during some sort of initialization pass to being something unique to your project,
        ///     but it is not absolutely necessary. This must be a multiple of 8 bytes.
        /// </remarks>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static byte[] EncryptionInitializationVector = Encoding.UTF8.GetBytes("dotBunny");

        /// <summary>
        ///     Decrypt an encrypted <see cref="System.String" /> created by <see cref="Encrypt" />.
        /// </summary>
        /// <remarks>This will have quite a few allocations.</remarks>
        /// <param name="encryptedString">The encrypted <see cref="System.String" />.</param>
        /// <param name="encryptionKey">The key used to encrypt the <see cref="System.String" />.</param>
        /// <returns>The decrypted <see cref="System.String" />.</returns>
        public static string Decrypt(this string encryptedString, byte[] encryptionKey = null)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                Key = encryptionKey ?? EncryptionDefaultKey,
                IV = EncryptionInitializationVector
            };
#if UNITY_2020_2_OR_NEWER
            using MemoryStream stream = new MemoryStream(Convert.FromBase64String(encryptedString));
            using CryptoStream cs = new CryptoStream(stream, desProvider.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs, Encoding.UTF8);
#else
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(encryptedString)))
            {
                using (CryptoStream cs = new CryptoStream(stream, desProvider.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs, Encoding.UTF8))
                    {
#endif
            return sr.ReadToEnd();
#if !UNITY_2020_2_OR_NEWER
                    }
                }
            }
#endif
        }

        /// <summary>
        ///     Encrypt a <see cref="System.String" /> utilizing a <see cref="DESCryptoServiceProvider" />.
        /// </summary>
        /// <remarks>This will have quite a few allocations.</remarks>
        /// <param name="decryptedString">The original <see cref="System.String" />.</param>
        /// <param name="encryptionKey">
        ///     The key to be used when encrypting the <see cref="System.String" />.  This must be a
        ///     multiple of 8 bytes.
        /// </param>
        /// <returns>The encrypted <see cref="System.String" />.</returns>
        public static string Encrypt(this string decryptedString, byte[] encryptionKey = null)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                Key = encryptionKey ?? EncryptionDefaultKey,
                IV = EncryptionInitializationVector
            };
#if UNITY_2020_2_OR_NEWER
            using MemoryStream stream = new MemoryStream();
            using CryptoStream cs = new CryptoStream(stream, desProvider.CreateEncryptor(), CryptoStreamMode.Write);
#else
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(stream, desProvider.CreateEncryptor(),
                    CryptoStreamMode.Write))
                {
#endif
            byte[] data = Encoding.Default.GetBytes(decryptedString);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
#if !UNITY_2020_2_OR_NEWER
                }
            }
#endif
        }

        /// <summary>
        ///     Get the <see cref="System.String" /> after the first identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>
        ///     The content following the <paramref name="splitString" />, or <paramref name="targetString" /> if none is
        ///     found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAfterFirst(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.IndexOf(splitString, 0, comparison);
            return splitIndex < 0 ? targetString : targetString.Substring(splitIndex + splitString.Length);
        }

        /// <summary>
        ///     Get the <see cref="System.String" /> after the last identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>
        ///     The content following the <paramref name="splitString" />, or <paramref name="targetString" /> if none is
        ///     found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAfterLast(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.LastIndexOf(splitString, targetString.Length - 1, comparison);
            return splitIndex < 0 ? targetString : targetString.Substring(splitIndex + splitString.Length);
        }

        /// <summary>
        ///     Get the <see cref="System.String" /> before the first identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>The content before the <paramref name="splitString" />, or <paramref name="targetString" /> if none is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBeforeFirst(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.IndexOf(splitString, 0, comparison);
            return splitIndex < 0 ? targetString : targetString.Substring(0, splitIndex);
        }

        /// <summary>
        ///     Get the <see cref="System.String" /> before the last identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>The content before the <paramref name="splitString" />, or <paramref name="targetString" /> if none is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBeforeLast(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.LastIndexOf(splitString, targetString.Length - 1, comparison);
            return splitIndex < 0 ? targetString : targetString.Substring(0, splitIndex);
        }

        /// <summary>
        ///     <para>
        ///         Get the stable hash code value of <paramref name="targetString" /> (converted to an uppercase
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
        public static unsafe int GetStableUpperCaseHashCode(this string targetString)
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
        ///         Get the stable hash code value of <paramref name="targetString" /> (converted to an uppercase
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
        public static unsafe int GetStableLowerCaseHashCode(this string targetString)
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

        /// <summary>
        ///     Determine if there are any lowercase letters in the provided <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" />.</param>
        /// <returns>true/false if lowercase letters were found.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe bool HasLowerCase(this string targetString)
        {
            fixed (char* src = targetString)
            {
                int c;
                char* s = src;

                // Get character
                while ((c = s[0]) != 0)
                {
                    if (c >= AsciiLowerCaseStart && c <= AsciiLowerCaseEnd)
                    {
                        return true;
                    }

                    s += 1;
                }
            }

            return false;
        }

        /// <summary>
        ///     Determine if there are any uppercase letters in the provided <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" />.</param>
        /// <returns>true/false if uppercase letters were found.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe bool HasUpperCase(this string targetString)
        {
            fixed (char* src = targetString)
            {
                int c;
                char* s = src;

                // Get character
                while ((c = s[0]) != 0)
                {
                    if (c >= AsciiUpperCaseStart && c <= AsciiUpperCaseEnd)
                    {
                        return true;
                    }

                    s += 1;
                }
            }

            return false;
        }

        /// <summary>
        ///     Create a new string, splitting an existing string up based on camel case formatting.
        /// </summary>
        /// <param name="targetString">The target <see cref="System.String" />.</param>
        /// <param name="divider">The <see cref="System.String" /> to put in between the split <see cref="System.String" />.</param>
        /// <returns>A new <see cref="System.String" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SplitCamelCase(this string targetString, string divider = " ")
        {
            return Regex.Replace(targetString, "([A-Z])", $"{divider}$1", RegexOptions.None).Trim();
        }
    }
}