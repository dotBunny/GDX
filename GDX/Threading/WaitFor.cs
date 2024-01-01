// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GDX.Threading
{
    /// <summary>
    ///     Some useful wait for methods to control program flow.
    /// </summary>
    public static class WaitFor
    {
        /// <summary>
        ///     One second worth of milliseconds.
        /// </summary>
        public const int OneSecond = 1000;

        /// <summary>
        ///     Two seconds worth of milliseconds.
        /// </summary>
        public const int TwoSeconds = 2000;

        /// <summary>
        ///     Five seconds worth of milliseconds.
        /// </summary>
        public const int FiveSeconds = 5000;

        /// <summary>
        ///     Ten seconds worth of milliseconds.
        /// </summary>
        public const int TenSeconds = 10000;

        /// <summary>
        ///     Thirty seconds worth of milliseconds.
        /// </summary>
        public const int ThirtySeconds = 30000;

        /// <summary>
        ///     One minute worth of milliseconds.
        /// </summary>
        public const int OneMinute = 60000;

        /// <summary>
        ///     Ten minutes worth of milliseconds.
        /// </summary>
        public const int TenMinutes = 600000;

        /// <summary>
        ///     Wait using an <see cref="IEnumerator" />.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait for.</param>
        /// <returns>Yields null values.</returns>
        public static IEnumerator GetEnumerator(int milliseconds)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < milliseconds)
            {
                yield return null;
            }

            stopwatch.Stop();
        }

        /// <summary>
        ///     Wait asynchronously.
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to wait for.</param>
        public static async Task GetTask(int milliseconds)
        {
            await Task.Run(() =>
            {
                Task.Delay(milliseconds).Wait();
            });
        }
    }
}