// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GDX.Threading
{
    public class WaitWhileTests
    {
        readonly Stopwatch m_Stopwatch = new Stopwatch();

        bool LessThan20Milliseconds()
        {
            return m_Stopwatch.ElapsedMilliseconds < 20;
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator WaitWhile_LessThanOne()
        {
            m_Stopwatch.Restart();
            yield return WaitWhile.WaitAsync(LessThan20Milliseconds).AsIEnumerator();
            m_Stopwatch.Stop();
            Assert.IsTrue(m_Stopwatch.ElapsedMilliseconds >= 20);
        }


        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator While_OneSecond_Elapsed()
        {
            m_Stopwatch.Restart();
            yield return WaitWhile.While(LessThan20Milliseconds);
            m_Stopwatch.Stop();
            Assert.IsTrue(m_Stopwatch.ElapsedMilliseconds >= 20);
        }
    }
}