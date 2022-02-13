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
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly float _duration;

        public WaitForMilliseconds(float milliseconds)
        {
            _duration = milliseconds;
            _stopwatch.Restart();
        }

        public bool Wait()
        {
            if (_stopwatch.ElapsedMilliseconds >= _duration)
            {
                _stopwatch.Stop();
                return false;
            }

            return true;
        }

        public IEnumerator While()
        {
            while (_stopwatch.ElapsedMilliseconds < _duration)
            {
                yield return null;
            }

            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Restart();
        }
    }
}