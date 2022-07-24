// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using System;
using NUnit.Framework;
using UnityEditor;

namespace GDX.Rendering
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ShaderProvider" />
    ///     class.
    /// </summary>
    public class ShaderProviderTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void DottedLine_NotNull()
        {
            Assert.IsTrue(ShaderProvider.DottedLine != null);
        }
        [Test]
        [Category(Core.TestCategory)]
        public void UnlitColor_NotNull()
        {
            Assert.IsTrue(ShaderProvider.UnlitColor != null);
        }
    }
}