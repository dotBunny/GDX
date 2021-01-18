// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX;
using GDX.Developer.Editor.Build;
using NUnit.Framework;
#if GDX_TESTFRAMEWORK_PERFORMANCETESTING

#endif

namespace Editor.Build
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="BuildInfoGenerator" /> class.
    /// </summary>
    public class BuildInfoGeneratorTests
    {
        /// <summary>
        ///     Check if the default content is returned when asked for.
        /// </summary>
        [Test]
        [Category("GDX.Tests")]
        public void True_GetContent_ForceDefaults()
        {
            string generateContent = BuildInfoGenerator.GetContent(GDXConfig.Get(), true);
            Assert.IsTrue(generateContent.Contains(" public const int Changelist = 0;"), "Expected to find 'public const int Changelist = 0;'");
        }
    }
}