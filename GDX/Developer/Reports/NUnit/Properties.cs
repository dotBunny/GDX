// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Xml.Serialization;
using System.Collections.Generic;

namespace GDX.Developer.Reports.NUnit
{
    [XmlRoot(ElementName = "properties", IsNullable = true)]
    public class Properties
    {
        [XmlElement(ElementName = "property")] public List<Property> Property { get; set; }
    }
}