// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using GDX.Collections.Generic;

namespace GDX
{
    /// <summary>
    ///     <see cref="GDX.Collections.Generic.SimpleList{T}" /> Based Extension Methods
    /// </summary>
    /// <remarks>
    ///     Methods found in this extensions class are less performant then the included methods in
    ///     <see cref="GDX.Collections.Generic.SimpleList{T}" />. They are seperated out to clearly delineate this
    ///     performance regression.
    /// </remarks>
    public static class SimpleListExtensions
    {
        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" /> without checking the internal size,
        ///     making sure that the item is not already contained in the <see cref="SimpleList{T}" />.
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" /> to add to.</param>
        /// <param name="targetItem">The target class object to add.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if the operation was able to add the item successfully.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUncheckedUniqueItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem)
            where T : class
        {
            if (targetSimpleList.ContainsItem(targetItem))
            {
                return false;
            }

            targetSimpleList.AddUnchecked(targetItem);
            return true;
        }

        /// <summary>
        ///     Add an object reference to the <see cref="SimpleList{T}" /> without checking the internal size,
        ///     making sure that the reference is not already contained in the <see cref="SimpleList{T}" />.
        ///     Does not prevent addition of different objects for which Equals returns true.
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" /> to add to.</param>
        /// <param name="targetReference">The target class object to add.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if the operation was able to add the reference successfully.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddUncheckedUniqueReference<T>(ref this SimpleList<T> targetSimpleList, T targetReference)
            where T : class
        {
            if (targetSimpleList.ContainsReference(targetReference))
            {
                return false;
            }

            targetSimpleList.AddUnchecked(targetReference);
            return true;
        }

        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" /> with checking the internal size (expanding as necessary),
        ///     making sure that the item is not already contained in the <see cref="SimpleList{T}" />.
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" /> to add to.</param>
        /// <param name="targetItem">The target class object to add.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if the operation was able to add the item successfully.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddWithExpandCheckUniqueItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem)
            where T : class
        {
            if (targetSimpleList.ContainsItem(targetItem))
            {
                return false;
            }

            targetSimpleList.AddWithExpandCheck(targetItem);
            return true;
        }

        /// <summary>
        ///     Add an item to the <see cref="SimpleList{T}" /> with checking the internal size (expanding as necessary),
        ///     making sure that the item is not already contained in the <see cref="SimpleList{T}" />.
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" /> to add to.</param>
        /// <param name="targetItem">The target class object to add.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if the operation was able to add the item successfully.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddWithExpandCheckUniqueValue<T>(ref this SimpleList<T> targetSimpleList, T targetItem)
            where T : IEquatable<T>
        {
            int count = targetSimpleList.Count;
            for (int i = 0; i < count; i++)
            {
                if (targetSimpleList.Array[i].Equals(targetItem))
                {
                    return false;
                }
            }

            targetSimpleList.AddWithExpandCheck(targetItem);
            return true;
        }

        /// <summary>
        ///     Add an object reference to the <see cref="SimpleList{T}" /> with checking the internal size (expanding as necessary),
        ///     making sure that the reference is not already contained in the <see cref="SimpleList{T}" />.
        ///     Does not prevent addition of different objects for which Equals returns true.
        /// </summary>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" /> to add to.</param>
        /// <param name="targetReference">The target class object to add.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if the operation was able to add the reference successfully.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddWithExpandCheckUniqueReference<T>(ref this SimpleList<T> targetSimpleList, T targetReference)
            where T : class
        {
            if (targetSimpleList.ContainsReference(targetReference))
            {
                return false;
            }

            targetSimpleList.AddWithExpandCheck(targetReference);
            return true;
        }

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
        ///     <para>Does <paramref name="targetSimpleList" /> contain <paramref name="targetItem" />?</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The <see cref="SimpleList{T}" /> to look in.</param>
        /// <param name="targetItem">The target class object to look for.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsValue<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : IEquatable<T>
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
                if (array[i].Equals(targetItem))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     <para>Does <paramref name="targetSimpleList" /> contain <paramref name="targetItem" />?</para>
        /// </summary>
        /// <remarks>Ignores equality check and end up comparing object pointers.</remarks>
        /// <param name="targetSimpleList">The <see cref="SimpleList{T}" /> to look in.</param>
        /// <param name="targetItem">The target class object to look for.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsReference<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
#pragma warning disable
                // ReSharper disable All
                if ((System.Object)array[i] == (System.Object)targetItem)
                {
                    return true;
                }
                // ReSharper restore All
#pragma warning restore
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
        ///     <para>Removes the first <paramref name="targetReference" /> from the provided <paramref name="targetSimpleList" />.</para>
        ///     Only removes direct object references, i.e. equivalent strings at different memory addresses would not be removed.
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetReference">The target object to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if an object reference was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveFirstReference<T>(ref this SimpleList<T> targetSimpleList, T targetReference) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
#pragma warning disable
                // ReSharper disable All
                if ((System.Object)array[i] == (System.Object)targetReference)
                {
                    targetSimpleList.RemoveAt(i);
                    return true;
                }
                // ReSharper restore All
#pragma warning restore
            }

            return false;
        }

        /// <summary>
        ///     <para>Removes all <paramref name="targetItem" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>

        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetItem">The item to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if items were removed.</returns>
        public static bool RemoveItems<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
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
        ///     <para>Removes all instances of references to <paramref name="targetReference" /> from the provided <paramref name="targetSimpleList" />.</para>
        ///     Only removes direct object references, i.e. equivalent strings at different memory addresses would not be removed.
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>

        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetReference">The object reference to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if any references were removed.</returns>
        public static bool RemoveReferences<T>(ref this SimpleList<T> targetSimpleList, T targetReference) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;
            bool removedItems = false;

            for (int i = length - 1; i >= 0; i--)
            {
#pragma warning disable
                // ReSharper disable All
                if ((System.Object)array[i] == (System.Object)targetReference)
                {
                    targetSimpleList.RemoveAt(i);
                    removedItems = true;
                }
                // ReSharper restore All
#pragma warning restore
            }

            return removedItems;
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
        ///     <para>Removes the last reference to <paramref name="targetReference" /> from the provided <paramref name="targetSimpleList" />.</para>
        ///     Only removes direct object references, i.e. equivalent strings at different memory addresses would not be removed.
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The target <see cref="SimpleList{T}" />.</param>
        /// <param name="targetReference">The target object reference to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="SimpleList{T}" />.</typeparam>
        /// <returns>true/false if an object reference was removed.</returns>
        public static bool RemoveLastReference<T>(ref this SimpleList<T> targetSimpleList, T targetReference) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = length - 1; i >= 0; i--)
            {
#pragma warning disable
                // ReSharper disable All
                if ((System.Object)array[i] == (System.Object)targetReference)
                {
                    targetSimpleList.RemoveAt(i);
                    return true;
                }
                // ReSharper restore All
#pragma warning restore
            }

            return false;
        }
    }
}