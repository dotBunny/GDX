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
        // ReSharper disable once MemberCanBePrivate.Global
        public const string SkippedString = "Skipped";
        public const string FailedString = "Failed";
        public const string PassedString = "Passed";

        readonly TestRun m_Results = new TestRun();

        public NUnitReport(string name = null, string fullName = null, string className = null )
        {
            m_Results.TestSuite.Name = name;
            m_Results.TestSuite.FullName = fullName;
            m_Results.TestSuite.ClassName = className;
        }
        public TestCase AddDurationResult(string name, float seconds, bool passed = true, string failMessage = null, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Duration = seconds,
                Result = passed ? PassedString : FailedString,
                Message = passed ? null : failMessage
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
        public TestCase AddSkippedTest(string name, string skipMessage = null, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Result = SkippedString,
                Message = skipMessage
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

        public void Process()
        {
            m_Results.Process();
        }

        /// <summary>
        ///     Builds and returns the NUnit formatted report.
        /// </summary>
        /// <remarks>Can result in a UTF-16 based XML document.</remarks>
        public override string ToString()
        {
            // We need to make sure it is a UTF-8 xml serializer as most parsers wont know what to do with UTF-16
            TextGenerator generator = new TextGenerator();
            m_Results.Process();
            AddToGenerator(generator, m_Results);
            return generator.ToString();
        }

        static void AddToGenerator(TextGenerator generator, Property property)
        {
            generator.ApplyIndent();
            generator.Append("<property");
            AddToGeneratorAttribute(generator, "name", property.Name);
            AddToGeneratorAttribute(generator, "value", property.Value);
            generator.Append(" />");
            generator.NextLine();
        }
        static void AddToGenerator(TextGenerator generator, Properties properties)
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
        static void AddToGenerator(TextGenerator generator, TestCase testCase)
        {
            generator.ApplyIndent();
            generator.Append($"<test-case");
            AddToGeneratorAttribute(generator, "id", testCase.Id);
            AddToGeneratorAttribute(generator, "name", testCase.Name);
            AddToGeneratorAttribute(generator, "fullname", testCase.FullName);
            AddToGeneratorAttribute(generator, "methodname", testCase.MethodName);
            AddToGeneratorAttribute(generator, "classname", testCase.ClassName);
            AddToGeneratorAttribute(generator, "runstate", testCase.RunState);
            AddToGeneratorAttribute(generator, "seed", testCase.Seed);
            AddToGeneratorAttribute(generator, "result", testCase.Result);
            AddToGeneratorAttribute(generator, "start-time", testCase.StartTime);
            AddToGeneratorAttribute(generator, "end-time", testCase.EndTime);
            AddToGeneratorAttribute(generator, "duration", testCase.Duration);
            AddToGeneratorAttribute(generator, "asserts", testCase.Asserts);
            AddToGeneratorAttribute(generator, "output", testCase.Output);

            if ((testCase.Properties != null && testCase.Properties.Property.Count > 0) ||
                testCase.Output != null)
            {
                generator.Append(">");
                generator.NextLine();
                generator.PushIndent();

                if (testCase.Output != null)
                {
                    generator.AppendLine($"<output><![CDATA[{testCase.Output}]]></output>");
                }

                if (testCase.Message != null)
                {
                    generator.AppendLine($"<message><![CDATA[{testCase.Output}]]></message>");
                }

                if (testCase.StackTrace != null)
                {
                    generator.AppendLine($"<stack-trace><![CDATA[{testCase.Output}]]></stack-trace>");
                }

                if (testCase.Properties != null && testCase.Properties.Property.Count > 0)
                {
                    AddToGenerator(generator, testCase.Properties);
                }

                generator.PopIndent();
                generator.AppendLine("</test-case>");
            }
            else
            {
                generator.Append(" />");
                generator.NextLine();
            }
        }
        static void AddToGenerator(TextGenerator generator, TestSuite testSuite)
        {
            generator.ApplyIndent();
            generator.Append($"<test-suite");
            AddToGeneratorAttribute(generator, "type", testSuite.Type);
            AddToGeneratorAttribute(generator, "id", testSuite.Id);
            AddToGeneratorAttribute(generator, "name", testSuite.Name);
            AddToGeneratorAttribute(generator, "fullname", testSuite.FullName);
            AddToGeneratorAttribute(generator, "classname", testSuite.ClassName);
            AddToGeneratorAttribute(generator, "runstate", testSuite.RunState);

            AddToGeneratorAttribute(generator, "testcasecount", testSuite.TestCaseCount);
            AddToGeneratorAttribute(generator, "result", testSuite.Result);
            AddToGeneratorAttribute(generator, "label", testSuite.Label);
            AddToGeneratorAttribute(generator, "start-time", testSuite.StartTime);
            AddToGeneratorAttribute(generator, "end-time", testSuite.EndTime);
            AddToGeneratorAttribute(generator, "duration", testSuite.Duration);

            AddToGeneratorAttribute(generator, "total", testSuite.Total);
            AddToGeneratorAttribute(generator, "passed", testSuite.Passed);
            AddToGeneratorAttribute(generator, "failed", testSuite.Failed);
            AddToGeneratorAttribute(generator, "inconclusive", testSuite.Inconclusive);
            AddToGeneratorAttribute(generator, "skipped", testSuite.Skipped);
            AddToGeneratorAttribute(generator, "asserts", testSuite.Asserts);

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
        static void AddToGenerator(TextGenerator generator, TestRun testRun)
        {
            generator.ApplyIndent();
            generator.Append($"<test-run");
            AddToGeneratorAttribute(generator, "id", testRun.Id);
            AddToGeneratorAttribute(generator, "testcasecount", testRun.TestCaseCount);
            AddToGeneratorAttribute(generator, "result", testRun.Result);
            AddToGeneratorAttribute(generator, "total", testRun.Total);
            AddToGeneratorAttribute(generator, "passed", testRun.Passed);
            AddToGeneratorAttribute(generator, "failed", testRun.Failed);
            AddToGeneratorAttribute(generator, "inconclusive", testRun.Inconclusive);
            AddToGeneratorAttribute(generator, "skipped", testRun.Skipped);
            AddToGeneratorAttribute(generator, "asserts", testRun.Asserts);
            AddToGeneratorAttribute(generator, "engine-version", testRun.EngineVersion);
            AddToGeneratorAttribute(generator, "clr-version", testRun.CLRVersion);
            AddToGeneratorAttribute(generator, "start-time", testRun.StartTime);
            AddToGeneratorAttribute(generator, "end-time", testRun.EndTime);
            AddToGeneratorAttribute(generator, "duration", testRun.Duration);
            generator.Append(">");
            generator.NextLine();
            generator.PushIndent();
            AddToGenerator(generator, testRun.TestSuite);
            generator.PopIndent();
            generator.AppendLine("</test-run>");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AddToGeneratorAttribute(TextGenerator generator, string key, string value)
        {
            if (value == default) return;
            generator.Append($" {key}=\"{value}\"");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AddToGeneratorAttribute(TextGenerator generator, string key, int value)
        {
            if (value == default) return;
            generator.Append($" {key}=\"{value}\"");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AddToGeneratorAttribute(TextGenerator generator, string key, float value)
        {
            if (value == 0f) return;
            generator.Append($" {key}=\"{value}\"");
        }
    }
}