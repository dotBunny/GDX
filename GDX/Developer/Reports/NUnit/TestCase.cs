// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace GDX.Developer.Reports.NUnit
{
    [ExcludeFromCodeCoverage]
    public class TestCase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string MethodName { get; set; }
        public string ClassName { get; set; }
        public string RunState { get; set; }
        public string Seed { get; set; }
        public string Result { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public float Duration { get; set; }
        public int Asserts { get; set; }
        public string Message { get; set; }
        public string Output { get; set; }
        public Properties Properties { get; set; }

        public string StackTrace { get; set; }

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