// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Xml.Serialization;

namespace GDX.Developer.Reports.NUnit
{
    [XmlRoot(ElementName = "test-case")]
    public class TestCase
    {
        [XmlAttribute(AttributeName = "id")] public int Id { get; set; }

        [XmlAttribute(AttributeName = "name")] public string Name { get; set; }

        [XmlAttribute(AttributeName = "fullname")]
        public string FullName { get; set; }

        [XmlAttribute(AttributeName = "methodname")]
        public string MethodName { get; set; }

        [XmlAttribute(AttributeName = "classname")]
        public string ClassName { get; set; }

        [XmlAttribute(AttributeName = "runstate")]
        public string RunState { get; set; }

        [XmlAttribute(AttributeName = "seed")] public string Seed { get; set; }

        [XmlAttribute(AttributeName = "result")]
        public string Result { get; set; }

        [XmlAttribute(AttributeName = "start-time")]
        public string StartTime { get; set; }

        [XmlAttribute(AttributeName = "end-time")]
        public string EndTime { get; set; }

        [XmlAttribute(AttributeName = "duration")]
        public string Duration { get; set; }

        [XmlAttribute(AttributeName = "asserts")]
        public int Asserts { get; set; }

        [XmlElement(ElementName = "properties", IsNullable = true)]
        public Properties Properties { get; set; }

        [XmlElement(ElementName = "output")] public string Output { get; set; }

        public string GetCategory()
        {
            string foundProperty = "Default";
            foreach (Property p in Properties.Property)
            {
                if (p.Name == "Category" && p.Value != "Performance")
                {
                    foundProperty = p.Value;
                }
            }

            return foundProperty;
        }
    }

}