// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using GDX.Collections.Generic;

namespace GDX
{
    /// <summary>
    ///     <see cref="GDX.Collections.Generic.SimpleList{T}" /> Based Extension Methods
    /// </summary>
    public static class SimpleListExtensions
    {
        /// <summary>
        ///     <para>Does <paramref name="targetSimpleList" /> contain <paramref name="targetItem" />?</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The <see cref="SimpleList{T}" /> to look in.</param>
        /// <param name="targetItem">The target class object to look for.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
                if (array[i] == targetItem)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetSimpleList" /> contain <paramref name="targetValue" />?</para>
        /// </summary>
        /// <param name="targetSimpleList">The <see cref="SimpleList{T}" /> to look in.</param>
        /// <param name="targetValue">The value to look for.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsValue<T>(ref this SimpleList<T> targetSimpleList, T targetValue) where T : struct
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
                if (array[i].Equals(targetValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes the first <paramref name="targetItem" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if an item was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveFirstItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
                // Skip to next if its not what we are looking for
                if (array[i] != targetItem)
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes the last <paramref name="targetItem" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if an item was removed.</returns>
        public static bool RemoveLastItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = length - 1; i >= 0; i--)
            {
                // Skip to next if its not what we are looking for
                if (array[i] != targetItem)
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes all <paramref name="targetItem" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetItem">The item to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if items were removed.</returns>
        public static bool RemoveAllItems<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;
            bool removedItems = false;

            for (int i = length - 1; i >= 0; i--)
            {
                // Skip to next if its not what we are looking for
                if (array[i] != targetItem)
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                removedItems = true;
            }

            return removedItems;
        }

        /// <summary>
        ///     <para>Removes all <paramref name="targetValue" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if values were removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveAllValues<T>(ref this SimpleList<T> targetSimpleList, T targetValue) where T : struct
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;
            bool removedValues = false;
            for (int i = length - 1; i >= 0; i--)
            {
                if (!array[i].Equals(targetValue))
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                removedValues = true;
            }

            return removedValues;
        }

        /// <summary>
        ///     <para>Removes the first <paramref name="targetValue" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Iterates positively starting at index 0.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if a value was removed.</returns>
        public static bool RemoveFirstValue<T>(ref this SimpleList<T> targetSimpleList, T targetValue) where T : struct
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;
            for (int i = 0; i < length; i++)
            {
                if (!array[i].Equals(targetValue))
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes the last <paramref name="targetValue" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Iterates negatively starting at the last index.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if a value was removed.</returns>
        public static bool RemoveLastValue<T>(ref this SimpleList<T> targetSimpleList, T targetValue) where T : struct
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;
            for (int i = length - 1; i >= 0; i--)
            {
                if (!array[i].Equals(targetValue))
                {
                    continue;
                }

                targetSimpleList.RemoveAt(i);
                return true;
            }

            return false;
        }
    }
}