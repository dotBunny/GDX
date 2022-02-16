// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Xml.Serialization;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GDX.Developer.Reports.NUnit
{
    public class TestRun
    {
        public int Id { get; set; }
        public int TestCaseCount { get; set; }
        public string Result { get; set; } = "Incomplete";
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Inconclusive { get; set; }
        public int Skipped { get; set; }
        public int Asserts { get; set; }
        public string EngineVersion { get; set; }

        [XmlAttribute(AttributeName = "clr-version")]
        public string CLRVersion { get; set; }

        [XmlAttribute(AttributeName = "start-time")]
        public string StartTime { get; set; }

        [XmlAttribute(AttributeName = "end-time")]
        public string EndTime { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public float Duration { get; set; }

        [XmlElement(ElementName = "test-suite")]
        public TestSuite TestSuite { get; set; } = new TestSuite();

        internal void AddToGenerator(TextGenerator generator)
        {
            generator.ApplyIndent();
            generator.Append($"<test-run");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "id", Id);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "testcasecount", TestCaseCount);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "result", Result);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "total", Total);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "passed", Passed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "failed", Failed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "inconclusive", Inconclusive);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "skipped", Skipped);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "asserts", Asserts);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "engine-version", EngineVersion);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "clr-version", CLRVersion);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "start-time", StartTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "end-time", EndTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "duration", Duration);
            generator.Append(">");
            generator.NextLine();
            generator.PushIndent();
            TestSuite.AddToGenerator(generator);
            generator.PopIndent();
            generator.AppendLine("</test-run>");
        }
    }
}