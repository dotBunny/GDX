// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.Reports.NUnit
{
    public class TestRun
    {
        public int Id { get; set; }
        public int TestCaseCount { get; set; }
        public string Result { get; private set; } = "Incomplete";
        public int Total { get; private set; }
        public int Passed { get; private set; }
        public int Failed { get; private set; }
        public int Inconclusive { get; private set; }
        public int Skipped { get; private set; }
        public int Asserts { get; private set; }
        public string EngineVersion { get; set; }
#pragma warning disable once IDE1006
        // ReSharper disable once InconsistentNaming
        public string CLRVersion { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public float Duration { get; set; }
        public TestSuite TestSuite { get; } = new TestSuite();

        public void Process(string passedResult = "Passed", string failedResult = "Failed",
            string inconclusiveResult = "Inconclusive", string skippedResult = "Skipped")
        {
#if !UNITY_DOTSRUNTIME
            EngineVersion = UnityEngine.Application.unityVersion;
#endif

            // Process the underlying suite of tests
            TestSuite.Process(passedResult, failedResult, inconclusiveResult, skippedResult);

            Passed = TestSuite.Passed;
            Failed = TestSuite.Failed;
            Inconclusive = TestSuite.Inconclusive;
            Skipped = TestSuite.Skipped;
            Total = Passed + Failed + Inconclusive + Skipped;

            Asserts = TestSuite.Asserts;
            Result = Failed > 0 || Total <= 0 ? failedResult : passedResult;
        }
    }
}