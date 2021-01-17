// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{
    /// <summary>
    ///     Array Based Extension Methods
    /// </summary>
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
        public static void Clear<T>(this T[] targetArray)
        {
            Array.Clear(targetArray, 0, targetArray.Length);
        }

        /// <summary>
        ///     Find the first index of <paramref name="targetItem" /> in <paramref name="targetArray" />.
        /// </summary>
        /// <param name="targetArray">The array which to look in.</param>
        /// <param name="targetItem">The object to be found.</param>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <returns>The index of <paramref name="targetItem" /> in <paramref name="targetArray" />, or -1 if not found.</returns>
        public static int FirstIndexOfItem<T>(this T[] targetArray, T targetItem) where T : class
        {
            int length = targetArray.Length;
            for (int i = 0; i < length; i++)
            {
                // ReSharper disable once HeapView.PossibleBoxingAllocation
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
                // ReSharper disable once HeapView.PossibleBoxingAllocation
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
                // ReSharper disable once HeapView.PossibleBoxingAllocation
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
                // ReSharper disable once HeapView.PossibleBoxingAllocation
                if (targetArray[i].Equals(targetValue))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}