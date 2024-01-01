// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Localization" />
    ///     class.
    /// </summary>
    public class LocalizationTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void GetHumanReadableFileSize_100_ReturnsValidString()
        {
            Assert.IsTrue(Localization.GetHumanReadableFileSize(100) == $"100 {Localization.ByteSizes[0]}");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetHumanReadableFileSize_1024_ReturnsValidString()
        {
            Assert.IsTrue(Localization.GetHumanReadableFileSize(1024) == $"1 {Localization.ByteSizes[1]}");
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetHumanReadableFileSize_1048576_ReturnsValidString()
        {
            Assert.IsTrue(Localization.GetHumanReadableFileSize(1048576) == $"1 {Localization.ByteSizes[2]}");
        }
    }
}