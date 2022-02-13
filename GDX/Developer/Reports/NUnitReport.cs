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
        private const string k_FailedString = "Failed";
        private const string k_PassedString = "Passed";

        private readonly TestRun m_Results = new TestRun();

        public TestCase AddDurationResult(string name, int seconds, bool passed = true, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Duration = seconds.ToString(),
                Result = passed ? k_PassedString : k_FailedString
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