// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using NUnit.Framework;

namespace Runtime.Developer
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Report"/> class.
    /// </summary>
    public class ReportTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void CreateDivider_MockData_CorrectLength()
        {

            string mockData = GDX.Developer.Report.CreateDivider();

            bool evaluate = mockData.Length == GDX.Developer.Report.CharacterWidth;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CreateHeader_MockData_CorrectLength()
        {

            string mockData = GDX.Developer.Report.CreateHeader("My Name");

            bool evaluate = mockData.Length == GDX.Developer.Report.CharacterWidth;

            Assert.IsTrue(evaluate);
        }
    }
}