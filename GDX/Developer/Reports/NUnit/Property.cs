// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Xml.Serialization;

namespace GDX.Developer.Reports.NUnit
{
    [XmlRoot(ElementName = "property", IsNullable = true)]
    public class Property
    {
        [XmlAttribute(AttributeName = "name")] public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }
    }
}