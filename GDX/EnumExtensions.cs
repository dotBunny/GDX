// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX
{
    /// <summary>
    ///     Enumeration Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    public static class EnumExtensions
    {
        /// <summary>
        ///     Determine if the provide flags (<paramref name="needles" />) are found in the <paramref name="haystack" />.
        /// </summary>
        /// <param name="haystack">A predefined flag based enumeration.</param>
        /// <param name="needles">A set of flags to search for in the predefined enumeration.</param>
        /// <typeparam name="T">The enumeration's type.</typeparam>
        /// <returns>true if the needles are found in the haystack, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe bool HasFlags<T>(T* haystack, T* needles) where T : unmanaged, Enum
        {
            byte* flags = (byte*)haystack;
            byte* query = (byte*)needles;

            for (int i = 0; i < sizeof(T); i++)
            {
                if ((flags[i] & query[i]) != query[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Determine if the provide flags (<paramref name="needles" />) are found in the <paramref name="haystack" />.
        /// </summary>
        /// <remarks>Faster then <see cref="Enum.HasFlag" />.</remarks>
        /// <param name="haystack">A predefined flag based enumeration.</param>
        /// <param name="needles">A set of flags to search for in the predefined enumeration.</param>
        /// <typeparam name="T">The enumeration's type.</typeparam>
        /// <returns>true if the needles are found in the haystack, otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasFlags<T>(this T haystack, T needles) where T : unmanaged, Enum
        {
            return HasFlags(&haystack, &needles);
        }
    }
}