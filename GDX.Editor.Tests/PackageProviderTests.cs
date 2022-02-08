// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor;
using NUnit.Framework;

namespace Editor
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="PackageProvider" /> class.
    /// </summary>
    public class PackageProviderTests
    {
        [Test]
        [Category(GDX.Core.TestCategory)]
        public void Get_DefaultPackage_NotNull()
        {
            PackageProvider localPackage = new PackageProvider();
            Assert.IsTrue(localPackage != null);
        }
    }
}