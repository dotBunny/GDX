using GDX.Developer.Reports.NUnit;
using System.IO;
using GDX.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace GDX.Developer.Reports
{
    // ReSharper disable once InconsistentNaming
    public static class BVT
    {
        static NUnitReport m_Report = new NUnitReport("BVT", "Build Verification Test");
        private static SimpleList<string> m_PanicMessages = new SimpleList<string>(2);

        public static string OutputReport(string outputPath)
        {
            Platform.ForceDeleteFile(outputPath);
            File.WriteAllText(outputPath, m_Report.ToString());
            return m_Report.GetResult();
        }

        public static TestCase Assert(string identifier, bool condition, string failMessage = null, int duration = 0)
        {
            TestCase test = m_Report.AddDurationResult(identifier, duration, condition, failMessage);
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
            m_Report = new NUnitReport();
            m_PanicMessages.Clear();
        }
        
        public static TestCase Skip(string identifier, string skipMessage)
        {
            TestCase test = m_Report.AddSkippedTest(identifier, skipMessage);
            Debug.Log($"[BVT] {test.Name}: {test.Result}");
            return test;
        }
        
        public static void Panic(string panicMessage)
        {
            m_PanicMessages.AddWithExpandCheck(panicMessage);
            Debug.LogError($"[BVT] PANIC! {panicMessage}");   
        }
    }
}