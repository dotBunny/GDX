﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using GDX.Classic;
using GDX.Classic.Developer.Reports;
using NUnit.Framework;

namespace Runtime.Classic.Developer.Reports
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

            string outputPathA = GDX.Editor.Automation.GetTempFilePath("Output_MockData_StringBuilderSameAsStreamWriter-builderOutput-",".txt");
            string outputPathB = GDX.Editor.Automation.GetTempFilePath("Output_MockData_StringBuilderSameAsStreamWriter-writerOutput-",".txt");
            File.WriteAllText(outputPathA, builderOutput);
            File.WriteAllText(outputPathB, writerOutput);

            bool evaluate = builderOutput == writerOutput;

            Assert.IsTrue(evaluate, $"{builderOutput} != {writerOutput}");
        }
    }
}