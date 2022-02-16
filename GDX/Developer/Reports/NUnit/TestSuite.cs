// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GDX.Developer.Reports.NUnit
{
    public class TestSuite
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ClassName { get; set; }
        public string RunState { get; set; }
        public string TestCaseCount { get; set; }
        public string Result { get; set; }
        public string Label { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Duration { get; set; }
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Inconclusive { get; set; }
        public int Skipped { get; set; }
        public int Asserts { get; set; }
        public Properties Properties { get; set; }
        public List<TestCase> TestCases { get; set; } = new List<TestCase>();
        public List<TestSuite> TestSuites { get; set;  }= new List<TestSuite>();

        public int GetPassCount()
        {
            int passCount = 0;
            foreach (TestCase testCase in TestCases)
            {
                if (testCase.Result == NUnitReport.PassedString)
                {
                    passCount++;
                }
            }
            foreach (TestSuite testSuite in TestSuites)
            {
                passCount += testSuite.GetPassCount();
            }
            return passCount;
        }

        public int GetFailCount()
        {
            int failCount = 0;
            foreach (TestCase testCase in TestCases)
            {
                if (testCase.Result == NUnitReport.FailedString)
                {
                    failCount++;
                }
            }
            foreach (TestSuite testSuite in TestSuites)
            {
                failCount += testSuite.GetPassCount();
            }
            return failCount;
        }

        internal void AddToGenerator(TextGenerator generator)
        {
            generator.ApplyIndent();
            generator.Append($"<test-suite");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "type", Type);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "id", Id);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "name", Name);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "fullname", FullName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "classname", ClassName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "runstate", RunState);
            
            NUnitReport.AddToGeneratorKeyValuePair(generator, "testcasecount", TestCaseCount);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "result", Result);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "label", Label);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "start-time", StartTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "end-time", EndTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "duration", Duration);
            
            NUnitReport.AddToGeneratorKeyValuePair(generator, "total", Total);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "passed", Passed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "failed", Failed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "inconclusive", Inconclusive);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "skipped", Skipped);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "asserts", Asserts);

            generator.Append(">");
            generator.NextLine();
            generator.PushIndent();
            if (Properties != null)
            {
                Properties.AddToGenerator(generator);
            }

            foreach (TestCase t in TestCases)
            {
                t.AddToGenerator(generator);
            }

            foreach (TestSuite s in TestSuites)
            {
                s.AddToGenerator(generator);
            }
            generator.PopIndent();
            generator.AppendLine("</test-suite>");
        }
    }

}