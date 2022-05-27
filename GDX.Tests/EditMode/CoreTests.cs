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
        const string k_CoreFullName = "GDX.Core";
        const string k_CoreFieldName = "s_InitializedMainThread";

        [Test]
        [Category(Core.TestCategory)]
        public void InitializeOnMainThread_AlreadyInitialized_EarlyOut()
        {
            Type core = Reflection.GetType(k_CoreFullName);
            Assert.IsNotNull(core);

            Reflection.SetFieldValue(null, core, k_CoreFieldName, true,
                Reflection.PrivateStaticFlags);
            Assert.IsTrue(Reflection.TryGetFieldValue(null, core, k_CoreFieldName,
                out bool _, Reflection.PrivateStaticFlags));


            Core.InitializeOnMainThread();
        }

        [Test]
        [Category(Core.TestCategory)]
        public void InitializeOnMainThread_NotAlreadyInitialized_Executes()
        {
            Type core = Reflection.GetType(k_CoreFullName);
            Assert.IsNotNull(core);

            Reflection.SetFieldValue(null, core, k_CoreFieldName, false,
                Reflection.PrivateStaticFlags);

            Assert.IsTrue(Reflection.TryGetFieldValue(null, core, k_CoreFieldName,
                out bool returnValue, Reflection.PrivateStaticFlags));
            Assert.IsFalse(returnValue);

            Core.InitializeOnMainThread();
        }
    }
}