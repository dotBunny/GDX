// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GDX.Developer.Reports.NUnit
{
    [ExcludeFromCodeCoverage]
    public class TestSuite
    {
        public readonly List<TestCase> TestCases = new List<TestCase>();

        public readonly List<TestSuite> TestSuites = new List<TestSuite>();
        public string Type { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ClassName { get; set; }
        public string RunState { get; set; }
        public int TestCaseCount { get; set; }
        public string Result { get; set; }
        public string Label { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public float Duration { get; set; }
        public int Total => Passed + Failed + Inconclusive + Skipped;
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Inconclusive { get; set; }
        public int Skipped { get; set; }
        public int Asserts { get; set; }
        public Properties Properties { get; set; }

        public void Process(string passedResult, string failedResult, string inconclusiveResult, string skippedResult)
        {
            Passed = 0;
            Failed = 0;
            Inconclusive = 0;
            Skipped = 0;

            foreach (TestCase t in TestCases)
            {
                if (t.Result == passedResult)
                {
                    Passed++;
                }
                else if (t.Result == failedResult)
                {
                    Failed++;
                }
                else if (t.Result == inconclusiveResult)
                {
                    Inconclusive++;
                }
                else if (t.Result == skippedResult)
                {
                    Skipped++;
                }
            }

            TestCaseCount = Total;

            // Update children
            foreach (TestSuite s in TestSuites)
            {
                s.Process(passedResult, failedResult, inconclusiveResult, skippedResult);
                Passed += s.Passed;
                Failed += s.Failed;
                Inconclusive += s.Inconclusive;
                Skipped += s.Skipped;
                TestCaseCount += Total;
            }
        }

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
    }
}