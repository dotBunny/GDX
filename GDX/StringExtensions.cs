// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using GDX.Collections.Generic;

namespace GDX
{
    /// <summary>
    ///     <see cref="string" /> Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    public static class StringExtensions
    {
        /// <summary>
        ///     The ASCII decimal value shift required to change the case of a letter.
        /// </summary>
        public const int AsciiCaseShift = 32;

        /// <summary>
        ///     The ASCII decimal value for a.
        /// </summary>
        public const int AsciiLowerCaseStart = 97;

        /// <summary>
        ///     The ASCII decimal value for lowercase z.
        /// </summary>
        public const int AsciiLowerCaseEnd = 122;

        /// <summary>
        ///     The ASCII decimal value for the number sign -.
        /// </summary>
        public const int AsciiNumberSign = 45;

        /// <summary>
        ///     The ASCII decimal value for the decimal (.).
        /// </summary>
        public const int AsciiNumberDecimal = 46;

        /// <summary>
        ///     The ASCII decimal value for the , separator.
        /// </summary>
        public const int AsciiNumberSeparator = 47;

        /// <summary>
        ///     The ASCII decimal value for 0.
        /// </summary>
        public const int AsciiNumberStart = 48;

        /// <summary>
        ///     The ASCII decimal value for 9.
        /// </summary>
        public const int AsciiNumberEnd = 57;

        /// <summary>
        ///     The ASCII decimal value for uppercase A.
        /// </summary>
        public const int AsciiUpperCaseStart = 65;

        /// <summary>
        ///     The ASCII  decimal value for uppercase Z.
        /// </summary>
        public const int AsciiUpperCaseEnd = 90;

        /// <summary>
        ///     The default encryption key used when none is provided to the encryption related extensions.
        /// </summary>
        /// <remarks>
        ///     You can change this at runtime during some sort of initialization pass to being something unique to your project,
        ///     but it is not absolutely necessary. This must be a multiple of 8 bytes.
        /// </remarks>
        public static byte[] EncryptionDefaultKey = Encoding.UTF8.GetBytes("Awesome!");

        /// <summary>
        ///     The IV (Initialization Vector) provided to the <see cref="DESCryptoServiceProvider" />.
        /// </summary>
        /// <remarks>
        ///     You can change this at runtime during some sort of initialization pass to being something unique to your project,
        ///     but it is not absolutely necessary. This must be a multiple of 8 bytes.
        /// </remarks>
        public static byte[] EncryptionInitializationVector = Encoding.UTF8.GetBytes("dotBunny");

        /// <summary>
        ///     Concatenate an array of strings into one unified string.
        /// </summary>
        /// <param name="pieces">An array of strings</param>
        /// <param name="delimiter">An optional string which to use between <paramref name="pieces" /> when combining.</param>
        /// <param name="trailingDelimiter">Should a trailing <paramref name="delimiter" /> be appended?</param>
        /// <returns>A concatenated <see cref="string" />.</returns>
        public static string Concatenate(this string[] pieces, string delimiter = null, bool trailingDelimiter = false)
        {
            StringBuilder builder = new StringBuilder();

            int count = pieces.Length;
            bool hasDelimiter = delimiter != null;
            int tail = count - 1;

            if (trailingDelimiter)
            {
                for (int i = 0; i < count; i++)
                {
                    builder.Append(pieces[i]);
                    if (hasDelimiter)
                    {
                        builder.Append(delimiter);
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    builder.Append(pieces[i]);
                    if (hasDelimiter && i != tail)
                    {
                        builder.Append(delimiter);
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        ///     Decrypt an encrypted <see cref="string" /> created by <see cref="Encrypt" />.
        /// </summary>
        /// <remarks>This will have quite a few allocations.</remarks>
        /// <param name="encryptedString">The encrypted <see cref="string" />.</param>
        /// <param name="encryptionKey">The key used to encrypt the <see cref="string" />.</param>
        /// <returns>The decrypted <see cref="string" />.</returns>
        public static string Decrypt(this string encryptedString, byte[] encryptionKey = null)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                Key = encryptionKey != null && encryptionKey.Length > 0 ? encryptionKey : EncryptionDefaultKey,
                IV = EncryptionInitializationVector
            };
            using MemoryStream stream = new MemoryStream(Convert.FromBase64String(encryptedString));
            using CryptoStream cs = new CryptoStream(stream, desProvider.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new StreamReader(cs, Encoding.UTF8);
            return sr.ReadToEnd();
        }

        /// <summary>
        ///     Encrypt a <see cref="string" /> utilizing a <see cref="DESCryptoServiceProvider" />.
        /// </summary>
        /// <remarks>This will have quite a few allocations.</remarks>
        /// <param name="decryptedString">The original <see cref="string" />.</param>
        /// <param name="encryptionKey">
        ///     The key to be used when encrypting the <see cref="string" />.  This must be a
        ///     multiple of 8 bytes.
        /// </param>
        /// <returns>The encrypted <see cref="string" />.</returns>
        public static string Encrypt(this string decryptedString, byte[] encryptionKey = null)
        {
            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                Key = encryptionKey != null && encryptionKey.Length > 0 ? encryptionKey : EncryptionDefaultKey,
                IV = EncryptionInitializationVector
            };
            using MemoryStream stream = new MemoryStream();
            using CryptoStream cs = new CryptoStream(stream, desProvider.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] data = Encoding.Default.GetBytes(decryptedString);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(stream.ToArray());
        }

        /// <summary>
        ///     Get the <see cref="string" /> after the first identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>
        ///     The content following the <paramref name="splitString" />, or <c>null</c> if none is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAfterFirst(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.IndexOf(splitString, 0, comparison);
            return splitIndex < 0 ? null : targetString.Substring(splitIndex + splitString.Length);
        }

        /// <summary>
        ///     Get the <see cref="string" /> after the last identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>
        ///     The content following the <paramref name="splitString" />, or <c>null</c> if none is found.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetAfterLast(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.LastIndexOf(splitString, targetString.Length - 1, comparison);
            return splitIndex < 0 ? null : targetString.Substring(splitIndex + splitString.Length);
        }

        /// <summary>
        ///     Get the <see cref="string" /> before the first identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>The content before the <paramref name="splitString" />, or <c>null</c> if none is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBeforeFirst(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.IndexOf(splitString, 0, comparison);
            return splitIndex < 0 ? null : targetString.Substring(0, splitIndex);
        }

        /// <summary>
        ///     Get the <see cref="string" /> before the last identified <paramref name="splitString" /> in
        ///     <paramref name="targetString" />.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" /> to look in.</param>
        /// <param name="splitString">The divider which the <paramref name="targetString" /> should be split on.</param>
        /// <param name="comparison">Specifies the culture, case, and sort rules to be used.</param>
        /// <returns>The content before the <paramref name="splitString" />, or <c>null</c> if none is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetBeforeLast(this string targetString, string splitString,
            StringComparison comparison = StringComparison.Ordinal)
        {
            int splitIndex = targetString.LastIndexOf(splitString, targetString.Length - 1, comparison);
            return splitIndex < 0 ? null : targetString.Substring(0, splitIndex);
        }

        /// <summary>
        ///     <para>
        ///         Get the stable hash code value of <paramref name="targetString" />.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function. It's value will be identical
        ///     to the value produced natively by processing a <see cref="string" /> with
        ///     <see cref="string.GetHashCode()" />, but with no allocations and no virtual calls.
        /// </remarks>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>A <see cref="int" /> value.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe int GetStableHashCode(this string targetString)
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
                    // Add to Hash #1
                    hash1 = ((hash1 << 5) + hash1) ^ c;

                    // Get our second character
                    c = s[1];

                    if (c == 0)
                    {
                        break;
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
        ///         <see cref="string" />).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function. It's value will be identical
        ///     to the value produced natively by processing a <see cref="string" /> with
        ///     <see cref="string.ToLower()" />.<see cref="string.GetHashCode()" />, but with no
        ///     allocations.
        /// </remarks>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>A <see cref="int" /> value.</returns>
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
                        c ^= AsciiCaseShift;
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
                        c ^= AsciiCaseShift;
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
        ///         <see cref="string" />).
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This loosely based on the Fowler–Noll–Vo (FNV) hash function. It's value will be identical
        ///     to the value produced natively by processing a <see cref="string" /> with
        ///     <see cref="string.ToUpper()" />.<see cref="string.GetHashCode()" />, but with no
        ///     allocations.
        /// </remarks>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>A <see cref="int" /> value.</returns>
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
                        c ^= AsciiCaseShift;
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
                        c ^= AsciiCaseShift;
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
        /// <param name="targetString">The target <see cref="string" />.</param>
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
        /// <param name="targetString">The target <see cref="string" />.</param>
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
        ///     Determine if the <paramref name="targetString" /> represents a boolean value arrangement.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>true/false if the <paramref name="targetString" /> can be evaluated as a boolean value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBooleanValue(this string targetString)
        {
            // Get an optimized hash value
            int hash = targetString.GetStableLowerCaseHashCode();

            // Check
            switch (hash)
            {
                case -971704226: // true
                case -1685090051: // false
                case 372029325: // 1
                case 372029326: // 0
                case -1273338385: // yes
                case 1496915069: // no
                case -1231968287: // on
                case -870054309: // off
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Determine if the <paramref name="targetString" /> represents a positive boolean value arrangement.
        /// </summary>
        /// <example>
        ///     Useful method when trying to parse data for branching.
        ///     <code>
        ///         if(data["set"].IsBooleanPositiveValue())
        ///         {
        ///             ShouldBlueBox();
        ///         }
        ///     </code>
        /// </example>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>true/false if the <paramref name="targetString" /> can be evaluated as a positive boolean value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsBooleanPositiveValue(this string targetString)
        {
            int hash = targetString.GetStableLowerCaseHashCode();
            switch (hash)
            {
                case -971704226: // true
                case 372029325: // 1
                case -1273338385: // yes
                case -1231968287: // on
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Determine if the <paramref name="targetString" /> is an <see cref="int" /> value.
        /// </summary>
        /// <remarks>
        ///     This method is meant for when you do not actually need the value returned, merely an evaluation if
        ///     the provided <paramref name="targetString" /> is an <see cref="int" />. This does not qualify
        ///     <see cref="float" /> values positively.
        /// </remarks>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>true/false if it contains an <see cref="int" />.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe bool IsIntegerValue(this string targetString)
        {
            fixed (char* src = targetString)
            {
                char* s = src;
                int c = s[0];

                // Nothing
                if (c == 0)
                {
                    return false;
                }

                // Check first character
                if (c != AsciiNumberSign && (c < AsciiNumberStart || c > AsciiNumberEnd))
                {
                    return false;
                }

                // Get character
                while ((c = s[1]) != 0)
                {
                    if (c < AsciiNumberStart || c > AsciiNumberEnd)
                    {
                        return false;
                    }

                    s += 1;
                }
            }

            return true;
        }

        /// <summary>
        ///     Is the <paramref name="targetString" /> a numeric value.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The following requirements must be met to be considered a valid number in this method:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 The first character may be an indicator of its sign, an explicit acceptance of <c>-</c> is made. If
        ///                 prefixed with <c>+</c>, the number will be found invalid.
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>A single decimal point <c>.</c> may be present in the <paramref name="targetString" />.</description>
        ///         </item>
        ///         <item>
        ///             <description>No alphabet characters are present in the <paramref name="targetString" />.</description>
        ///         </item>
        ///     </list>
        /// </remarks>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <returns>true/false if the <paramref name="targetString" /> qualifies as a numeric value.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe bool IsNumeric(this string targetString)
        {
            fixed (char* src = targetString)
            {
                char* s = src;
                int c = s[0];
                bool hasDecimal = false;

                // Nothing
                if (c == 0)
                {
                    return false;
                }

                // Check first character
                if (c != AsciiNumberSign && (c < AsciiNumberStart || c > AsciiNumberEnd))
                {
                    return false;
                }

                // Get character
                while ((c = s[1]) != 0)
                {
                    if (c < AsciiNumberStart || c > AsciiNumberEnd)
                    {
                        if (c == AsciiNumberDecimal && !hasDecimal)
                        {
                            hasDecimal = true;
                            s += 1;
                            continue;
                        }

                        return false;
                    }

                    s += 1;
                }
            }

            return true;
        }

        /// <summary>
        ///     Counts the number of times the needle (<paramref name="targetCharacter" />) appears in the haystack (
        ///     <paramref name="targetString" />).
        /// </summary>
        /// <remarks>Specifically created to avoid using LINQ and avoid an allocation.</remarks>
        /// <param name="targetString">The haystack.</param>
        /// <param name="targetCharacter">The needle.</param>
        /// <returns>The number of times <paramref name="targetCharacter" /> is found in <paramref name="targetString" />.</returns>
        [SecuritySafeCritical]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static unsafe int CountOccurence(this string targetString, char targetCharacter)
        {
            int counter = 0;
            fixed (char* src = targetString)
            {
                int c;
                char* s = src;
                while ((c = s[0]) != 0)
                {
                    if (c == targetCharacter)
                    {
                        counter++;
                    }

                    s += 1;
                }
            }

            return counter;
        }

        /// <summary>
        ///     Does the <paramref name="haystack" /> partially contain the <paramref name="needle" />?
        /// </summary>
        /// <param name="haystack">An array of <see cref="string" />s.</param>
        /// <param name="needle">The <see cref="string" /> that is being looked for.</param>
        /// <returns>true/false if found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PartialMatch(this string[] haystack, string needle)
        {
            if (haystack == null)
            {
                return false;
            }

            int count = haystack.Length;
            for (int i = 0; i < count; i++)
            {
                if (haystack[i].Contains(needle))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Does the <paramref name="haystack" /> partially contain the <paramref name="needle" />?
        /// </summary>
        /// <param name="haystack">A <see cref="SimpleList{T}" /> of <see cref="string" />s.</param>
        /// <param name="needle">The <see cref="string" /> that is being looked for.</param>
        /// <returns>true/false if found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PartialMatch(this SimpleList<string> haystack, string needle)
        {
            if (haystack.Count == 0)
            {
                return false;
            }

            int count = haystack.Count;
            for (int i = 0; i < count; i++)
            {
                if (haystack.Array[i].Contains(needle))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Create a new string, splitting an existing string up based on camel case formatting.
        /// </summary>
        /// <param name="targetString">The target <see cref="string" />.</param>
        /// <param name="divider">The <see cref="string" /> to put in between the split <see cref="string" />.</param>
        /// <returns>A new <see cref="string" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SplitCamelCase(this string targetString, string divider = " ")
        {
            return Regex.Replace(targetString, "([A-Z])", $"{divider}$1", RegexOptions.None).Trim();
        }

        /// <summary>
        ///     Remove non ASCII characters from a <see cref="string" />.
        /// </summary>
        /// <param name="targetString">The <see cref="string" /> to be cleaned.</param>
        /// <returns>A <see cref="string" /> without ASCII characters.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string StripNonAscii(this string targetString)
        {
            return Regex.Replace(targetString, @"[^\u0000-\u007F]+", string.Empty);
        }
    }
}