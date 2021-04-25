// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;
using GDX;
using NUnit.Framework;
using UnityEngine.SocialPlatforms;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace Runtime
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Localization" />
    ///     class.
    /// </summary>
    public class LocalizationTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void GetHumanReadableFileSize_100_ReturnsValidString()
        {
            string mockString = Localization.GetHumanReadableFileSize(100);

            bool evaluate = mockString == "100 B";

            Assert.IsTrue(evaluate, mockString);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetHumanReadableFileSize_1024_ReturnsValidString()
        {
            string mockString = Localization.GetHumanReadableFileSize(1024);

            bool evaluate = mockString == "1 KB";

            Assert.IsTrue(evaluate, mockString);
        }

        [Test]
        [Category("GDX.Tests")]
        public void GetHumanReadableFileSize_1048576_ReturnsValidString()
        {
            string mockString = Localization.GetHumanReadableFileSize(1048576);

            bool evaluate = mockString == "1 MB";

            Assert.IsTrue(evaluate, mockString);
        }
    }
}