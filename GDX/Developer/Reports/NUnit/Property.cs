// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace GDX.Developer.Reports.NUnit
{
    [ExcludeFromCodeCoverage]
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}