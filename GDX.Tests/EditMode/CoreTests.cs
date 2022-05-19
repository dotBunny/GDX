// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
            Type core = Reflection.GetType("GDX.Core");
            Assert.IsNotNull(core);

            Reflection.SetFieldValue(null, core, "s_InitializedMainThread", true,
                Reflection.PrivateStaticFlags);
            Assert.IsTrue(Reflection.GetFieldValue<bool>(null, core, "s_InitializedMainThread",
                Reflection.PrivateStaticFlags));


            Core.InitializeOnMainThread();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InitializeOnMainThread_NotAlreadyInitialized_Executes()
        {
            Type core = Reflection.GetType("GDX.Core");
            Assert.IsNotNull(core);

            Reflection.SetFieldValue(null, core, "s_InitializedMainThread", false,
                Reflection.PrivateStaticFlags);

            Assert.IsFalse(Reflection.GetFieldValue<bool>(null, core, "s_InitializedMainThread",
                Reflection.PrivateStaticFlags));

            Core.InitializeOnMainThread();
        }
    }
}