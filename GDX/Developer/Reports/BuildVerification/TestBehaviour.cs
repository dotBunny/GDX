using System;
using System.Diagnostics;
using GDX.Developer.Reports.NUnit;
using UnityEngine;

namespace GDX.Developer.Reports.BuildVerification
{
    public abstract class TestBehaviour : MonoBehaviour
    {
        protected abstract TestCase Check();
        protected abstract string GetIdentifier();

        string m_StartTime;
        Stopwatch m_Timer;

        /// <summary>
        ///     Unity's Start event.
        /// </summary>
#pragma warning disable IDE0051
        // ReSharper disable UnusedMember.Local        
        void Start()
        {
            m_StartTime = DateTime.Now.ToString(Localization.UtcTimestampFormat);
            m_Timer = new Stopwatch();
            m_Timer.Restart();

            TestCase testCase = RunTest();

            m_Timer.Stop();
            testCase.Duration = (m_Timer.ElapsedMilliseconds / 1000f);
            testCase.EndTime = DateTime.Now.ToString(Localization.UtcTimestampFormat);
            testCase.StartTime = m_StartTime;
        }
        // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051

        TestCase RunTest()
        {
            try
            {
                TestCase testCase = Check();
                return testCase;
            }
            catch (Exception e)
            {
                TestCase testCase = BuildVerificationReport.Assert(GetIdentifier(), false, e.Message);
                testCase.StackTrace = e.StackTrace;
                return testCase;
            }
        }
    }
}