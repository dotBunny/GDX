// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading.Tasks;

namespace GDX.Threading
{
    /// <summary>
    ///     Some useful wait while methods to control program flow.
    /// </summary>
    public static class WaitWhile
    {
        /// <summary>
        ///     Wait using an <see cref="IEnumerator" /> while the <paramref name="conditional" /> is true.
        /// </summary>
        /// <param name="conditional">A function evaluated to determine if the wait continues.</param>
        /// <returns>Yields null values.</returns>
        public static IEnumerator GetEnumerator(Func<bool> conditional)
        {
            while (conditional())
            {
                yield return null;
            }
        }

        /// <summary>
        ///     Wait asynchronously while the <paramref name="conditional" /> is true.
        /// </summary>
        /// <param name="conditional">A function evaluated to determine if the wait continues.</param>
        public static async Task GetTask(Func<bool> conditional)
        {
            await Task.Run(() =>
            {
                while (conditional())
                {
                    Task.Delay(1).Wait();
                }
            });
        }
    }
}