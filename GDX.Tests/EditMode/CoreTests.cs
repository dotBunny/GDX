// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A simple unit test to validate the behaviour of the <see cref="Core"/>.
    /// </summary>
    public class CoreTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void InitializeOnMainThread_AlreadyInitialized_EarlyOut()
        {
            Core.InitializeOnMainThread();
        }
    }
}