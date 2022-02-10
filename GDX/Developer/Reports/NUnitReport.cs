// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Xml.Serialization;
using GDX.Developer.Reports.NUnit;

namespace GDX.Developer.Reports
{
    public class NUnitReport
    {
        public const string FailedString = "Failed";
        public const string PassedString = "Passed";

        public TestRun Results = new TestRun();

        public TestCase AddDurationResult(string name, int seconds, bool passed = true, TestSuite testSuite = null)
        {
            TestCase testCase = new TestCase
            {
                Name = name,
                Duration = seconds.ToString(),
                Result = passed ? PassedString : FailedString
            };
            if (testSuite != null)
            {
                testSuite.TestCases.Add(testCase);
            }
            else
            {
                Results.TestSuite.TestCases.Add(testCase);
            }
            return testCase;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            XmlSerializer reportSerializer = new XmlSerializer(Results.GetType());
            using StringWriter textWriter = new StringWriter();
            reportSerializer.Serialize(textWriter, Results);
            return textWriter.ToString();
        }
    }
}