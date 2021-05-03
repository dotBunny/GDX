// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using GDX.Developer;
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

            ReportContext context = new ReportContext();
            string mockData = context.CreateDivider();

            bool evaluate = mockData.Length == context.CharacterWidth;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void CreateHeader_MockData_CorrectLength()
        {
            ReportContext context = new ReportContext();
            string mockData = context.CreateHeader("My Name");

            bool evaluate = mockData.Length == context.CharacterWidth;

            Assert.IsTrue(evaluate);
        }

        [Test]
        [Category("GDX.Tests")]
        public void Output_MockData_StringBuilderSameAsStreamWriter()
        {
            var report = ResourcesReport.GetCommon();

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

            string pathA = System.IO.Path.Combine(UnityEngine.Application.dataPath, "builder.log");
            string pathB = System.IO.Path.Combine(UnityEngine.Application.dataPath, "writer.log");
            File.WriteAllText(pathA, builderOutput);
            File.WriteAllText(pathB, writerOutput);

            bool evaluate = builderOutput == writerOutput;

            Assert.IsTrue(evaluate);
        }
    }
}