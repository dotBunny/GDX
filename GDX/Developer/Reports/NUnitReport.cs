// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Xml.Serialization;
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

        public TestCase AddDurationResult(string name, int seconds, bool passed = true, string output = null, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Duration = seconds.ToString(),
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

        public void Stage(int uniqueIdentifier = 0, string testSuiteLabel = "Test Suite")
        {
            m_Results.Id = uniqueIdentifier;

            m_Results.Failed = m_Results.TestSuite.GetFailCount();
            m_Results.Passed = m_Results.TestSuite.GetFailCount();
            m_Results.TestCaseCount = m_Results.Failed + m_Results.Passed;
            m_Results.Total = m_Results.Failed + m_Results.Passed;

            m_Results.Result = m_Results.Failed == 0 ? PassedString : FailedString;
            m_Results.TestSuite.Label = testSuiteLabel;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            XmlSerializer reportSerializer = new XmlSerializer(m_Results.GetType());
            using StringWriter textWriter = new StringWriter();
            reportSerializer.Serialize(textWriter, m_Results);
            return textWriter.ToString();
        }
    }
}