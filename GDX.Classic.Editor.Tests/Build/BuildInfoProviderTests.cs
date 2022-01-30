// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.Build;
using NUnit.Framework;

namespace Editor.Build
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="BuildInfoProvider" /> class.
    /// </summary>
    public class BuildInfoProviderTests
    {
        /// <summary>
        ///     Check if the default content is returned when asked for.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void GetContent_ForceDefaults_ReturnsDefaultContent()
        {
            string generateContent = BuildInfoProvider.GetContent(true);

            bool evaluate = generateContent.Contains(" public const int Changelist = 0;");

            Assert.IsTrue(evaluate);
        }
    }
}