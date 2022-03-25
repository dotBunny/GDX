// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX
{
    /// <summary>
    ///     Array Based Extension Methods
    /// </summary>
    [VisualScriptingCompatible(2)]
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Set all elements in an array to the default values.
        /// </summary>
        /// <remarks>
        ///     This does not alter the <paramref name="targetArray"/>'s length.
        /// </remarks>
        /// <param name="targetArray">The array to be defaulted.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear<T>(this T[] targetArray)
        {
            Array.Clear(targetArray, 0, targetArray.Length);
        }

        /// <summary>
        ///     <para>Does <paramref name="targetArray" /> contain <paramref name="targetValue" />?</para>
        /// </summary>
        /// <param name="targetArray">The <see cref="System.Array" /> to look in.</param>
        /// <param name="targetValue">The target item to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Array" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsItem<T>(this T[] targetArray, T targetValue) where T : class
        {
            int count = targetArray.Length;
            for (int i = 0; i < count; i++)
            {
                if (targetArray[i] == targetValue)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetArray" /> contain <paramref name="targetItem" />?</para>
        /// </summary>
        /// <remarks>Ignores equality check and end up comparing object pointers.</remarks>
        /// <param name="targetArray">The <see cref="System.Array" /> to look in.</param>
        /// <param name="targetItem">The target item to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Array" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsReference<T>(this T[] targetArray, T targetItem) where T : class
        {
            int count = targetArray.Length;
            for (int i = 0; i < count; i++)
            {
#pragma warning disable
                // ReSharper disable All
                if ((System.Object)targetArray[i] == (System.Object)targetItem)
                {
                    return true;
                }
                // ReSharper restore All
#pragma warning restore
            }
            return false;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetArray" /> contain <paramref name="targetValue" />?</para>
        /// </summary>
        /// <param name="targetArray">The <see cref="System.Array" /> to look in.</param>
        /// <param name="targetValue">The target item to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Array" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsValue<T>(this T[] targetArray, T targetValue) where T : IEquatable<T>
        {
            int count = targetArray.Length;
            for (int i = 0; i < count; i++)
            {
                if (targetArray[i].Equals(targetValue))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Find the first index of <paramref name="targetItem" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <remarks>Ignores equality check and end up comparing object pointers.</remarks>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetItem">The object to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetItem" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int FirstIndexOfItem<T>(this T[] targetArray, T targetItem) where T : class
        {
            int length = targetArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (targetArray[i] == targetItem)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Find the first index of <paramref name="targetValue" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetValue">The value to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetValue" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int FirstIndexOfValue<T>(this T[] targetArray, T targetValue) where T : struct
        {
            int length = targetArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (targetArray[i].Equals(targetValue))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Find the last index of <paramref name="targetItem" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetItem">The object to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetItem" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int LastIndexOfItem<T>(this T[] targetArray, T targetItem) where T : class
        {
            int length = targetArray.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                if (targetArray[i] == targetItem)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Find the last index of <paramref name="targetValue" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetValue">The value to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetValue" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int LastIndexOfValue<T>(this T[] targetArray, T targetValue) where T : struct
        {
            int length = targetArray.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                if (targetArray[i].Equals(targetValue))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}