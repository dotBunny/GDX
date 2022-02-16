// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using GDX.Developer.Reports.NUnit;

// ReSharper disable UnusedMember.Global

namespace GDX.Developer.Reports
{
    // ReSharper disable once UnusedType.Global
    public class NUnitReport
    {
        public const string FailedString = "Failed";
        public const string PassedString = "Passed";

        private readonly TestRun m_Results = new TestRun();

        public NUnitReport(string suiteName = null)
        {
            m_Results.TestSuite.Name = suiteName;
        }

        public TestCase AddDurationResult(string name, float seconds, bool passed = true, string output = null, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Duration = seconds,
                Result = passed ? PassedString : FailedString,
                Output = output
            };
            if (testSuite != null)
            {
                testSuite.TestCases.Add(testCase);
            }
            else
            {
                m_Results.TestSuite.TestCases.Add(testCase);
            }
            return testCase;
        }

        public string GetResult()
        {
            return m_Results.Result;
        }

        public TestSuite GetRootSuite()
        {
            return m_Results.TestSuite;
        }

        public void Stage(int uniqueIdentifier = 0, string testSuiteLabel = "Test Suite", int panicMessages = 0)
        {
            m_Results.Id = uniqueIdentifier;
            m_Results.Process();
            m_Results.TestSuite.Label = testSuiteLabel;
        }

        /// <inheritdoc />
        /// <remarks>Can result in a UTF-16 based XML document.</remarks>
        public override string ToString()
        {
            // We need to make sure it is a UTF-8 xml serializer as most parsers wont know what to do with UTF-16
            TextGenerator generator = new TextGenerator();
            AddToGenerator(generator, m_Results);
            return generator.ToString();
        }
        
        
        
        
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AddToGeneratorKeyValuePair(TextGenerator generator, string key, string value)
        {
            if (value == default) return;
            generator.Append($" {key}=\"{value}\"");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AddToGeneratorKeyValuePair(TextGenerator generator, string key, int value)
        {
            if (value == default) return;
            generator.Append($" {key}=\"{value}\"");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void AddToGeneratorKeyValuePair(TextGenerator generator, string key, float value)
        {
            if (value == 0f) return;
            generator.Append($" {key}=\"{value}\"");
        }

        internal void AddToGenerator(TextGenerator generator, NUnit.Property property)
        {
            generator.ApplyIndent();
            generator.Append("<property");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "name", property.Name);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "value", property.Value);
            generator.Append(" />");
            generator.NextLine();
        }
        internal void AddToGenerator(TextGenerator generator, NUnit.Properties properties)
        {
            generator.AppendLine("<properties>");
            generator.PushIndent();
            int count = properties.Property.Count;
            for (int i = 0; i < count; i++)
            {
                AddToGenerator(generator, properties.Property[i]);
            }
            generator.PopIndent();
            generator.AppendLine("</properties>");
        }
        internal void AddToGenerator(TextGenerator generator, NUnit.TestCase testCase)
        {
            generator.ApplyIndent();
            generator.Append($"<test-case");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "id", testCase.Id);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "name", testCase.Name);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "fullname", testCase.FullName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "methodname", testCase.MethodName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "classname", testCase.ClassName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "runstate", testCase.RunState);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "seed", testCase.Seed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "result", testCase.Result);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "start-time", testCase.StartTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "end-time", testCase.EndTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "duration", testCase.Duration);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "asserts", testCase.Asserts);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "output", testCase.Output);

            if (testCase.Properties != null && testCase.Properties.Property.Count > 0)
            {
                generator.Append(">");
                generator.NextLine();
                generator.PushIndent();
                AddToGenerator(generator, testCase.Properties);
                generator.PopIndent();
                generator.AppendLine("</test-case>");
            }
            else
            {
                generator.Append(" />");   
                generator.NextLine();
            }
        }
        
        internal void AddToGenerator(TextGenerator generator, NUnit.TestRun testRun)
        {
            generator.ApplyIndent();
            generator.Append($"<test-run");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "id", testRun.Id);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "testcasecount", testRun.TestCaseCount);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "result", testRun.Result);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "total", testRun.Total);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "passed", testRun.Passed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "failed", testRun.Failed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "inconclusive", testRun.Inconclusive);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "skipped", testRun.Skipped);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "asserts", testRun.Asserts);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "engine-version", testRun.EngineVersion);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "clr-version", testRun.CLRVersion);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "start-time", testRun.StartTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "end-time", testRun.EndTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "duration", testRun.Duration);
            generator.Append(">");
            generator.NextLine();
            generator.PushIndent();
            AddToGenerator(generator, testRun.TestSuite);
            generator.PopIndent();
            generator.AppendLine("</test-run>");
        }
        
        internal void AddToGenerator(TextGenerator generator, NUnit.TestSuite testSuite)
        {
            generator.ApplyIndent();
            generator.Append($"<test-suite");
            NUnitReport.AddToGeneratorKeyValuePair(generator, "type", testSuite.Type);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "id", testSuite.Id);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "name", testSuite.Name);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "fullname", testSuite.FullName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "classname", testSuite.ClassName);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "runstate", testSuite.RunState);
            
            NUnitReport.AddToGeneratorKeyValuePair(generator, "testcasecount", testSuite.TestCaseCount);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "result", testSuite.Result);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "label", testSuite.Label);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "start-time", testSuite.StartTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "end-time", testSuite.EndTime);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "duration", testSuite.Duration);
            
            NUnitReport.AddToGeneratorKeyValuePair(generator, "total", testSuite.Total);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "passed", testSuite.Passed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "failed", testSuite.Failed);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "inconclusive", testSuite.Inconclusive);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "skipped", testSuite.Skipped);
            NUnitReport.AddToGeneratorKeyValuePair(generator, "asserts", testSuite.Asserts);

            generator.Append(">");
            generator.NextLine();
            generator.PushIndent();
            if (testSuite.Properties != null)
            {
                AddToGenerator(generator, testSuite.Properties);
                
            }

            foreach (TestCase t in testSuite.TestCases)
            {
                AddToGenerator(generator, t);
            }

            foreach (TestSuite s in testSuite.TestSuites)
            {
                AddToGenerator(generator, s);
            }
            generator.PopIndent();
            generator.AppendLine("</test-suite>");
        }
    }
}