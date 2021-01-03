// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace dotBunny.Hydrogen.Collections.Extensions
{
    /// <summary>
    ///     Array Based Extension Methods
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Find the index of <paramref name="targetItem" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetItem">The object to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetItem" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int IndexOf<T>(this T[] targetArray, T targetItem)
        {
            int length = targetArray.Length;
            for (int i = 0; i < length; i++)
            {
                // ReSharper disable once HeapView.PossibleBoxingAllocation
                if (targetArray[i].Equals(targetItem))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Clear an array and set its length to 0.
        /// </summary>
        /// <param name="targetArray">The array to be cleared.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        public static void Clear<T>(this T[] targetArray)
        {
            Array.Clear(targetArray, 0, targetArray.Length);
        }
    }
}