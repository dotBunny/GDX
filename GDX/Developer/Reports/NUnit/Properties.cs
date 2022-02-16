// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GDX.Developer.Reports.NUnit
{
    public class Properties
    {
        public List<Property> Property { get; set; }

        internal void AddToGenerator(TextGenerator generator)
        {
            generator.AppendLine("<properties>");
            generator.PushIndent();
            int count = Property.Count;
            for (int i = 0; i < count; i++)
            {
                Property[i].AddToGenerator(generator);
            }
            generator.PopIndent();
            generator.AppendLine("</properties>");
        }
    }
}
// TODO make data data, then the add as becomes as NUNIT format