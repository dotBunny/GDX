// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GDX.Threading
{
    public class WaitForTests
    {
        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator WaitAsync_OneSecond_Elapsed()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            yield return WaitFor.WaitAsync(WaitFor.OneSecond).AsIEnumerator();
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= WaitFor.OneSecond);
        }

        [UnityTest]
        [Category(Core.TestCategory)]
        public IEnumerator While_OneSecond_Elapsed()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();
            yield return WaitFor.While(WaitFor.OneSecond);
            stopwatch.Stop();
            Assert.IsTrue(stopwatch.ElapsedMilliseconds >= WaitFor.OneSecond);
        }
    }
}