// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Threading.Tasks;

namespace GDX.Threading
{
    /// <summary>
    ///     <see cref="Task" /> Based Extension Methods
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        ///     Wraps a <see cref="Task" /> for use in a coroutine.
        /// </summary>
        /// <remarks>Don't use coroutines.</remarks>
        /// <param name="task">An established <see cref="Task" />.</param>
        /// <returns>An <see cref="IEnumerator" /> for use with coroutines.</returns>
        public static IEnumerator AsIEnumerator(this Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}