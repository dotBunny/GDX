// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Xml.Serialization;

namespace GDX.Developer.Reports.NUnit
{
    [XmlRoot(ElementName = "test-run")]
    public class TestRun
    {
        [XmlAttribute(AttributeName = "id")] 
        public int Id { get; set; }

        [XmlAttribute(AttributeName = "testcasecount")]
        public int TestCaseCount { get; set; }

        [XmlAttribute(AttributeName = "result")]
        public string Result { get; set; } = "Incomplete";

        [XmlAttribute(AttributeName = "total")]
        public int Total { get; set; }

        [XmlAttribute(AttributeName = "passed")]
        public int Passed { get; set; }

        [XmlAttribute(AttributeName = "failed")]
        public int Failed { get; set; }

        [XmlAttribute(AttributeName = "inconclusive")]
        public int Inconclusive { get; set; }

        [XmlAttribute(AttributeName = "skipped")]
        public int Skipped { get; set; }

        [XmlAttribute(AttributeName = "asserts")]
        public int Asserts { get; set; }

        [XmlAttribute(AttributeName = "engine-version")]
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
    }
}