// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace GDX.Developer
{
    public class WaitForMilliseconds
    {
        private readonly Stopwatch m_Stopwatch = new Stopwatch();
        private readonly float m_Duration;

        public WaitForMilliseconds(float milliseconds)
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