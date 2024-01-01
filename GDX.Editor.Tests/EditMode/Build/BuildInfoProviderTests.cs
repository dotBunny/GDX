// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace GDX.Editor.Build
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
        [Category(Core.TestCategory)]
        public void GetContent_ForceDefaults_ReturnsDefaultContent()
        {
            string generateContent = BuildInfoProvider.GetContent(true);

            bool evaluate = generateContent.Contains(" public const int Changelist = 0;");

            Assert.IsTrue(evaluate);
        }
    }
}