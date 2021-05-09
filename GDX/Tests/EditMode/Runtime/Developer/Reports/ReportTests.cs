// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using GDX.Developer.Reports;
using NUnit.Framework;

namespace Runtime.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="Report"/> class.
    /// </summary>
    public class ReportTests
    {
        [Test]
        [Category("GDX.Tests")]
        public void Output_MockData_StringBuilderSameAsStreamWriter()
        {
            var report = ResourcesAudit.GetCommon();

            // String Builder
            StringBuilder builderString = new StringBuilder();
            report.Output(builderString);
            string builderOutput = builderString.ToString();

            // StreamWriter
            string writerOutput = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(memoryStream);
                report.Output(writer);
                writerOutput = Encoding.ASCII.GetString(memoryStream.ToArray());
            }

            bool evaluate = builderOutput == writerOutput;

            Assert.IsTrue(evaluate);
        }
    }
}