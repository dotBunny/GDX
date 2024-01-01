// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using NUnit.Framework;

namespace GDX.Developer.Reports.Resource
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="ResourceReport"/> class.
    /// </summary>
    public class ResourceReportTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void CreateDivider_MockData_CorrectLength()
        {

            ResourceReportContext context = new ResourceReportContext();
            string mockData = ResourceReport.CreateDivider(context);

            bool evaluate = mockData.Length == context.CharacterWidth;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void CreateHeader_MockData_CorrectLength()
        {
            ResourceReportContext context = new ResourceReportContext();
            string mockData = ResourceReport.CreateHeader(context,"My Name");

            bool evaluate = mockData.Length == context.CharacterWidth;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void Output_MockData_StringBuilderSameAsStreamWriter()
        {
            ResourcesAuditReport report = ResourcesAuditReport.GetCommon();

            // String Builder
            StringBuilder builderString = new StringBuilder();
            report.Output(builderString);
            string builderOutput = builderString.ToString();

            // StreamWriter
            string writerOutput;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(memoryStream);
                report.Output(writer);
                writerOutput = Encoding.ASCII.GetString(memoryStream.ToArray());
            }

            bool evaluate = builderOutput == writerOutput;

            Assert.IsTrue(evaluate, $"{builderOutput} != {writerOutput}");
        }
    }
}