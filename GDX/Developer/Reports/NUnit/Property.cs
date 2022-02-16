// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GDX.Developer.Reports.NUnit
{
    public class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }

        internal void AddToGenerator(TextGenerator generator)
        {
            generator.ApplyIndent();
            generator.Append("<property");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "name", Name);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "value", Value);
            generator.Append(" />");
            generator.NextLine();
        }
    }
}