using GDX.Developer.Reports.NUnit;
using System.Diagnostics;
using System.IO;
using GDX.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace GDX.Developer.Reports
{
    // ReSharper disable once InconsistentNaming
    public static class BVT
    {
        static readonly NUnitReport m_Report = new NUnitReport("BVT");
        private static SimpleList<string> m_PanicMessages = new SimpleList<string>(2);

        public static string OutputReport(string outputPath)
        {
            m_Report.Stage(0, "Build Verification Test", m_PanicMessages.Count);
            Platform.ForceDeleteFile(outputPath);
            File.WriteAllText(outputPath, m_Report.ToString());
            return m_Report.GetResult();
        }

        public static void Assert(string identifier, bool condition, string failMessage = null)
        {
            if (!condition)
            {
                AddTestCaseResult(identifier, false, 0, failMessage);
            }
            else
            {
                AddTestCaseResult(identifier, true);
            }
        }

        public static void Panic(string panicMessage)
        {
            m_PanicMessages.AddWithExpandCheck(panicMessage);
            Debug.LogError($"[BVT] PANIC! {panicMessage}");   
        }
        
        static TestCase AddTestCaseResult(string identifier, bool passed, int duration = 0, string output = null)
        {
            TestCase test = m_Report.AddDurationResult(identifier, duration, passed, output);
            if (passed)
            {
                Debug.Log($"[BVT] {test.Name}: {test.Result}");
            }
            else
            {
                Debug.LogError($"[BVT] {test.Name}: {test.Result}, {test.Output}");    
            }
            return test;
        }

        public class TimedCheck
        {
            public int Duration => (int)(m_Timer.ElapsedMilliseconds / 1000f);
            public string Identifier { get; }
            public string Output { get; private set; }

            public bool Passed { get; private set; } = true;

            readonly Stopwatch m_Timer;
        
            public TimedCheck(string testIdentifier)
            {
                Identifier = testIdentifier;
                m_Timer = new Stopwatch();
                m_Timer.Restart();
            }

            public void Assert(bool condition, string failMessage = null)
            {
                m_Timer.Stop();

                if (!condition)
                {
                    Output = failMessage;
                    Passed = false;
                }

                AddTestCaseResult(Identifier, Passed, Duration, Output);
            }
        }
    }
}