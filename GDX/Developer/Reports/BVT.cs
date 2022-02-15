using GDX.Developer.Reports.NUnit;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace GDX.Developer.Reports
{
    // ReSharper disable once InconsistentNaming
    public static class BVT
    {
        static readonly NUnitReport m_Report = new NUnitReport("BVT");
        
        public static string OutputReport(string outputPath)
        {
            m_Report.Stage(0, "GDX Build");
            Platform.ForceDeleteFile(outputPath);
            m_Report.Save(outputPath);
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
        
        static void AddTestCaseResult(string identifier, bool passed, int duration = 0, string output = null)
        {
            TestCase test = m_Report.AddDurationResult(identifier, duration, passed, output);
            if (passed)
            {
                Debug.Log($"[BVT] PASS {test.Name}: {test.Result}");
            }
            else
            {
                Debug.LogError($"[BVT] FAIL {test.Name}: {test.Output}");    
            }
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