using GDX.Developer.Reports.NUnit;
using System.IO;
using GDX.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace GDX.Developer.Reports
{
    public static class BuildVerificationReport
    {
        static NUnitReport s_Report = new NUnitReport("BVT", "Build Verification Test");
        static SimpleList<string> s_PanicMessages = new SimpleList<string>(2);

        public static string OutputReport(string outputPath)
        {
            if (s_PanicMessages.Count > 0)
            {
                s_Report.SetForceFail();
            }

            Platform.ForceDeleteFile(outputPath);
            File.WriteAllText(outputPath, s_Report.ToString());
            return s_Report.GetResult();
        }

        public static TestCase Assert(string identifier, bool condition, string failMessage = null, int duration = 0)
        {
            TestCase test = s_Report.AddDurationResult(identifier, duration, condition, failMessage);
            if (test.Result == NUnitReport.PassedString)
            {
                Debug.Log($"[BVT] {test.Name}: {test.Result}");
            }
            else
            {
                Debug.LogError($"[BVT] {test.Name}: {test.Result}, {test.Message}");
            }
            return test;
        }

        public static void Reset()
        {
            s_Report = new NUnitReport();
            s_PanicMessages.Clear();
        }

        public static TestCase Skip(string identifier, string skipMessage)
        {
            TestCase test = s_Report.AddSkippedTest(identifier, skipMessage);
            Debug.Log($"[BVT] {test.Name}: {test.Result}");
            return test;
        }

        public static void Panic(string panicMessage)
        {
            s_PanicMessages.AddWithExpandCheck(panicMessage);
            Debug.LogError($"[BVT] PANIC! {panicMessage}");
        }
    }
}