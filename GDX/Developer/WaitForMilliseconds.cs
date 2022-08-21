// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GDX.Developer
{
    public class WaitForMilliseconds
    {
        public const int OneSecond = 1000;
        public const int TwoSeconds = 2000;
        public const int FiveSeconds = 5000;
        public const int TenSeconds = 10000;
        public const int ThirtySeconds = 30000;
        public const int OneMinute = 60000;
        public const int TenMinutes = 600000;

        readonly Stopwatch m_Stopwatch = new Stopwatch();
        readonly int m_Duration;

        public WaitForMilliseconds(int milliseconds)
        {
            m_Duration = milliseconds;
            m_Stopwatch.Restart();
        }

        public bool Wait()
        {
            if (m_Stopwatch.ElapsedMilliseconds >= m_Duration)
            {
                m_Stopwatch.Stop();
                return false;
            }

            return true;
        }

        public async Task WaitAsync()
        {
            await Task.Run(() =>
            {
                Task.Delay(m_Duration).Wait();
            });
        }

        public IEnumerator While()
        {
            while (m_Stopwatch.ElapsedMilliseconds < m_Duration)
            {
                yield return null;
            }

            m_Stopwatch.Stop();
        }

        public void Reset()
        {
            m_Stopwatch.Restart();
        }
    }
}