// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using NUnit.Framework;

namespace GDX.Developer.Reports
{
    /// <summary>
    ///     A collection of unit tests to validate functionality of the <see cref="NUnitReport"/> class.
    /// </summary>
    public class NUnitReportTests
    {
        [Test]
        [Category(Core.TestCategory)]
        public void Constructor_Create()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            Assert.IsTrue(createdSuite.Name == TestLiterals.Foo);
            Assert.IsTrue(createdSuite.FullName == TestLiterals.HelloWorld);
            Assert.IsTrue(createdSuite.ClassName == TestLiterals.Bar);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddDuration_PassNoFailNoSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddDurationResult(TestLiterals.Seed, 41f, true, null, null);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(Math.Abs(createdSuite.TestCases[0].Duration - 41f) < Platform.FloatTolerance);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.PassedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddDuration_PassNoFailWithSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddDurationResult(TestLiterals.Seed, 41f, true, null, createdSuite);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(Math.Abs(createdSuite.TestCases[0].Duration - 41f) < Platform.FloatTolerance);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.PassedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddDuration_FailNoFailNoSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddDurationResult(TestLiterals.Seed, 41f, false, null, null);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(Math.Abs(createdSuite.TestCases[0].Duration - 41f) < Platform.FloatTolerance);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.FailedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddDuration_FailWithFailNoSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddDurationResult(TestLiterals.Seed, 41f, false, TestLiterals.HelloWorld, null);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(Math.Abs(createdSuite.TestCases[0].Duration - 41f) < Platform.FloatTolerance);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.FailedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == TestLiterals.HelloWorld);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSkippedTest_NoSkipNoSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddSkippedTest(TestLiterals.Seed);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(createdSuite.TestCases[0].Duration == 0f);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.SkippedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == null);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSkippedTest_SkipNoSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddSkippedTest(TestLiterals.Seed, TestLiterals.HelloWorld);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(createdSuite.TestCases[0].Duration == 0f);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.SkippedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == TestLiterals.HelloWorld);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void AddSkippedTest_SkipSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddSkippedTest(TestLiterals.Seed, TestLiterals.HelloWorld, createdSuite);

            Assert.IsTrue(createdSuite.TestCases[0].Name == TestLiterals.Seed);
            Assert.IsTrue(createdSuite.TestCases[0].Duration == 0f);
            Assert.IsTrue(createdSuite.TestCases[0].Result == NUnitReport.SkippedString);
            Assert.IsTrue(createdSuite.TestCases[0].Message == TestLiterals.HelloWorld);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void SetForceFail_Fails()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            report.SetForceFail();

            Assert.IsTrue(report.GetResult() == NUnitReport.FailedString);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetResultCount_NoTestSuites()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            // Have to process whats there
            report.Process();

            Assert.IsTrue(report.GetResultCount() == 0);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void GetResultCount_TestSuite()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            NUnit.TestSuite createdSuite = report.GetRootSuite();

            report.AddSkippedTest(TestLiterals.Seed, TestLiterals.HelloWorld, createdSuite);
            report.Process();

            Assert.IsTrue(report.GetResultCount() == 1);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ToString_Fail_ExpectedOutput()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            report.AddSkippedTest(TestLiterals.Seed, TestLiterals.HelloWorld);
            report.AddDurationResult(TestLiterals.Foo, 41f, false, TestLiterals.HelloWorld);
            report.AddDurationResult(TestLiterals.HelloWorld, 41f);


            StringBuilder expectedOutputBuilder = new StringBuilder();
            expectedOutputBuilder.AppendLine($"<test-run testcasecount=\"3\" result=\"Failed\" total=\"3\" passed=\"1\" failed=\"1\" skipped=\"1\" engine-version=\"{UnityEngine.Application.unityVersion}\">");
            expectedOutputBuilder.AppendLine("\t<test-suite name=\"foo\" fullname=\"Hello World\" classname=\"bar\" testcasecount=\"3\" total=\"3\" passed=\"1\" failed=\"1\" skipped=\"1\">");
            expectedOutputBuilder.AppendLine("\t\t<test-case name=\"TestSeed\" result=\"Skipped\" />");
            expectedOutputBuilder.AppendLine("\t\t<test-case name=\"foo\" result=\"Failed\" duration=\"41\" />");
            expectedOutputBuilder.AppendLine("\t\t<test-case name=\"Hello World\" result=\"Passed\" duration=\"41\" />");
            expectedOutputBuilder.AppendLine("\t</test-suite>");
            expectedOutputBuilder.AppendLine("</test-run>");

            string reportOutput = report.ToString();
            string expectedOutput = expectedOutputBuilder.ToString().TrimEnd();

            Assert.IsTrue(expectedOutput == reportOutput);
        }

        [Test]
        [Category(Core.TestCategory)]
        public void ToString_Pass_ExpectedOutput()
        {
            NUnitReport report = new NUnitReport(
                TestLiterals.Foo,
                TestLiterals.HelloWorld,
                TestLiterals.Bar);

            report.AddDurationResult(TestLiterals.HelloWorld, 41f);


            StringBuilder expectedOutputBuilder = new StringBuilder();
            expectedOutputBuilder.AppendLine($"<test-run testcasecount=\"1\" result=\"Passed\" total=\"1\" passed=\"1\" engine-version=\"{UnityEngine.Application.unityVersion}\">");
            expectedOutputBuilder.AppendLine("\t<test-suite name=\"foo\" fullname=\"Hello World\" classname=\"bar\" testcasecount=\"1\" total=\"1\" passed=\"1\">");
            expectedOutputBuilder.AppendLine("\t\t<test-case name=\"Hello World\" result=\"Passed\" duration=\"41\" />");
            expectedOutputBuilder.AppendLine("\t</test-suite>");
            expectedOutputBuilder.AppendLine("</test-run>");

            string reportOutput = report.ToString();
            string expectedOutput = expectedOutputBuilder.ToString().TrimEnd();

            Assert.IsTrue(expectedOutput == reportOutput);
        }
    }
}