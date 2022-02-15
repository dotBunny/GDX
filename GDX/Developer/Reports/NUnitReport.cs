// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
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

        public NUnitReport(string suiteName = null)
        {
            m_Results.TestSuite.Name = suiteName;
        }

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

        public TestSuite GetRootSuite()
        {
            return m_Results.TestSuite;
        }

        public void Stage(int uniqueIdentifier = 0, string testSuiteLabel = "Test Suite", int panicMessages = 0)
        {
            m_Results.Id = uniqueIdentifier;

            m_Results.Failed = m_Results.TestSuite.GetFailCount();
            m_Results.Passed = m_Results.TestSuite.GetPassCount();
            m_Results.Asserts = panicMessages;
            
            m_Results.TestCaseCount = m_Results.Failed + m_Results.Passed;
            m_Results.Total = m_Results.Failed + m_Results.Passed;

            m_Results.Result = (m_Results.Failed == 0 && m_Results.Asserts == 0) ? PassedString : FailedString;
            m_Results.TestSuite.Label = testSuiteLabel;
        }

        /// <summary>
        ///     Saves the report to a UTF-8 formatted XML document.
        /// </summary>
        /// <param name="filePath">The absolute path where to save the content of the report.</param>
        public void Save(string filePath)
        {
            XmlSerializer reportSerializer = new XmlSerializer(m_Results.GetType());
            using StreamWriter outputFile = new StreamWriter(filePath);
            reportSerializer.Serialize(outputFile, m_Results);
        }

        /// <inheritdoc />
        /// <remarks>Can result in a UTF-16 based XML document.</remarks>
        public override string ToString()
        {
            // We need to make sure it is a UTF-8 xml serializer as most parsers wont know what to do with UTF-16
            XmlSerializer reportSerializer = new XmlSerializer(m_Results.GetType());
            using StringWriter textWriter = new StringWriter();
            reportSerializer.Serialize(textWriter, m_Results);
            return textWriter.ToString();
        }
    }
}