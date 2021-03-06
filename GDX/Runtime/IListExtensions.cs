﻿// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     <see cref="System.Collections.Generic.IList{T}" /> Based Extension Methods
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        ///     Add an item to a <see cref="System.Collections.Generic.IList{T}" />, but only if it is not already contained.
        /// </summary>
        /// <param name="targetList">The <see cref="System.Collections.Generic.IList{T}" /> to add too.</param>
        /// <param name="targetItem">The target object to add..</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if this operation was able to add the item.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUniqueItem<T>(this IList<T> targetList, T targetItem) where T : class
        {
            if (targetList.ContainsItem(targetItem))
            {
                return false;
            }

            targetList.Add(targetItem);
            return true;

        }

        /// <summary>
        ///     Add a value to a <see cref="System.Collections.Generic.IList{T}" />, but only if it is not already contained.
        /// </summary>
        /// <param name="targetList">The <see cref="System.Collections.Generic.IList{T}" /> to add too.</param>
        /// <param name="targetValue">The target value to add..</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if this operation was able to add the value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUniqueValue<T>(this IList<T> targetList, T targetValue) where T : struct
        {
            if (targetList.ContainsValue(targetValue))
            {
                return false;
            }

            targetList.Add(targetValue);
            return true;

        }

        /// <summary>
        ///     <para>Does <paramref name="targetList" /> contain <paramref name="targetItem" />?</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetList">The <see cref="System.Collections.Generic.IList{T}" /> to look in.</param>
        /// <param name="targetItem">The target object to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsItem<T>(this IList<T> targetList, T targetItem) where T : class
        {
            int length = targetList.Count;
            for (int i = 0; i < length; i++)
            {
                if (targetList[i] == targetItem)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetList" /> contain <paramref name="targetValue" />?</para>
        /// </summary>
        /// <remarks>
        ///     WARNING: This will box the <paramref name="targetValue" />. You should probably write your own explicit
        ///     implementation.
        /// </remarks>
        /// <param name="targetList">The <see cref="System.Collections.Generic.IList{T}" /> to look in.</param>
        /// <param name="targetValue">The value to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsValue<T>(this IList<T> targetList, object targetValue)
            where T : struct
        {
            int length = targetList.Count;
            for (int i = 0; i < length; i++)
            {
                if (targetList[i].Equals(targetValue))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes the first <paramref name="targetItem" /> from the provided <paramref name="targetList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if the item was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveFirstItem<T>(this IList<T> targetList, T targetItem) where T : class
        {
            int length = targetList.Count;
            for (int i = 0; i < length; i++)
            {
                if (targetList[i] != targetItem)
                {
                    continue;
                }

                targetList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the first <paramref name="targetValue" /> from the provided <paramref name="targetList" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         WARNING: This will box the <paramref name="targetValue" />. You should probably write your own explicit
        ///         implementation.
        ///     </para>
        /// </remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if a value was removed.</returns>
        public static bool RemoveFirstValue<T>(this IList<T> targetList, object targetValue)
            where T : struct
        {
            int length = targetList.Count;
            for (int i = 0; i < length; i++)
            {
                if (!targetList[i].Equals(targetValue))
                {
                    continue;
                }

                targetList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes all <paramref name="targetItem" /> from the provided <paramref name="targetList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if the item was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveItems<T>(this IList<T> targetList, T targetItem) where T : class
        {
            int length = targetList.Count;
            bool removedItem = false;
            for (int i = length - 1; i >= 0; i--)
            {
                if (targetList[i] != targetItem)
                {
                    continue;
                }

                targetList.RemoveAt(i);
                removedItem = true;
            }

            return removedItem;
        }

        /// <summary>
        ///     Removes all <paramref name="targetValue" /> from the provided <paramref name="targetList" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         WARNING: This will box the <paramref name="targetValue" />. You should probably write your own explicit
        ///         implementation.
        ///     </para>
        /// </remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if values were removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveValues<T>(this IList<T> targetList, object targetValue) where T : struct
        {
            int length = targetList.Count;
            bool removedValues = false;
            for (int i = length - 1; i >= 0; i--)
            {
                if (!targetList[i].Equals(targetValue))
                {
                    continue;
                }

                targetList.RemoveAt(i);
                removedValues = true;
            }

            return removedValues;
        }

        /// <summary>
        ///     <para>
        ///         Replaces the object found at the provided <paramref name="index" /> with the last object in
        ///         <paramref name="targetList" />, then removes the last item from the <paramref name="targetList" />.
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     This make sure that you are always removing from the end of a
        ///     <see cref="System.Collections.Generic.IList{T}" />.
        /// </remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="index">The index of the item to remove.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveItemSwap<T>(this IList<T> targetList, int index)
        {
            int lastIndex = targetList.Count - 1;
            targetList[index] = targetList[lastIndex];
            targetList.RemoveAt(lastIndex);
        }


        /// <summary>
        ///     <para>Removes the last <paramref name="targetItem" /> from the provided <paramref name="targetList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if the item was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveLastItem<T>(this IList<T> targetList, T targetItem) where T : class
        {
            int length = targetList.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                if (targetList[i] != targetItem)
                {
                    continue;
                }

                targetList.RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the last <paramref name="targetValue" /> from the provided <paramref name="targetList" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         WARNING: This will box the <paramref name="targetValue" />. You should probably write your own explicit
        ///         implementation.
        ///     </para>
        /// </remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <param name="targetValue">The value to remove from the <paramref name="targetList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        /// <returns>true/false if a value was removed.</returns>
        public static bool RemoveLastValue<T>(this IList<T> targetList, object targetValue)
            where T : struct
        {
            int length = targetList.Count;
            for (int i = length - 1; i >= 0; i--)
            {
                if (!targetList[i].Equals(targetValue))
                {
                    continue;
                }

                targetList.RemoveAt(i);
                return true;
            }

            return false;
        }


        /// <summary>
        ///     Shuffle the items in the <paramref name="targetList" />.
        /// </summary>
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        /// <param name="targetList">The target <see cref="System.Collections.Generic.IList{T}" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.IList{T}" />.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shuffle<T>(this IList<T> targetList)
        {
            int length = targetList.Count;
            for (int i = 0; i < length; i++)
            {
                T t = targetList[i];
                int r = Random.Range(i, length);
                targetList[i] = targetList[r];
                targetList[r] = t;
            }
        }
    }
}