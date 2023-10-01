// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.RuntimeContent;
using NUnit.Framework;

namespace GDX.RuntimeContent
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourcesProvider" />
    ///     class.
    /// </summary>
    public class ResourcesProviderTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void DottedLine_NotNull()
        {
            Assert.IsTrue(ResourceProvider.GetShaders().DottedLine != null);
        }
        [Test]
        [Category(Core.TestCategory)]
        public void UnlitColor_NotNull()
        {
            Assert.IsTrue(ResourceProvider.GetShaders().UnlitColor != null);
        }
    }
}