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
        /// <param name="targetSimpleList">The <see cref="System.Collections.Generic.List{T}" /> to look in.</param>
        /// <param name="targetItem">The target object to look for.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.List{T}" />.</typeparam>
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
        ///     <para>Removes <paramref name="targetItem" /> from the provided <paramref name="targetSimpleList" />.</para>
        /// </summary>
        /// <remarks>Avoids using <see cref="System.Collections.Generic.EqualityComparer{T}" />.</remarks>
        /// <param name="targetSimpleList">The target <see cref="System.Collections.Generic.List{T}" />.</param>
        /// <param name="targetItem">The target object to remove from the <paramref name="targetSimpleList" />.</param>
        /// <typeparam name="T">The type of the <see cref="System.Collections.Generic.List{T}" />.</typeparam>
        /// <returns>true/false if the item was removed.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveItem<T>(ref this SimpleList<T> targetSimpleList, T targetItem) where T : class
        {
            int length = targetSimpleList.Count;
            T[] array = targetSimpleList.Array;

            for (int i = 0; i < length; i++)
            {
                if (array[i] != targetItem)
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