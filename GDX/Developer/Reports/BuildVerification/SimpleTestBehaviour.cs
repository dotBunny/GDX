using System;
using System.Diagnostics;
using GDX.Developer.Reports.NUnit;
using UnityEngine;

namespace GDX.Developer.Reports.BuildVerification
{
    public abstract class SimpleTestBehaviour : MonoBehaviour, ITestBehaviour
    {
        int m_frameWait = 5;
        string m_StartTime;
        Stopwatch m_Timer;

        void Start()
        {
            TestRunner.AddTest(this);
        }

        /// <summary>
        ///     Run the test in Unity's late update
        /// </summary>
#pragma warning disable IDE0051
        // ReSharper disable UnusedMember.Local
        void Update()
        {
            // Handle frame delay
            if (m_frameWait > 0)
            {
                m_frameWait--;
                return;
            }

            if (m_frameWait < 0)
            {
                return;
            }

            m_frameWait--;

            m_StartTime = DateTime.Now.ToString(Localization.UtcTimestampFormat);
            m_Timer = new Stopwatch();
            m_Timer.Restart();

            TestCase testCase = RunTest();

            m_Timer.Stop();
            testCase.Duration = m_Timer.ElapsedMilliseconds / 1000f;
            testCase.EndTime = DateTime.Now.ToString(Localization.UtcTimestampFormat);
            testCase.StartTime = m_StartTime;

            TestRunner.RemoveTest(this);
        }

        // ReSharper restore UnusedMember.Local
#pragma warning restore IDE0051

        public abstract TestCase Check();
        public abstract string GetIdentifier();

        /// <inheritdoc />
        public virtual void Setup()
        {
        }

        /// <inheritdoc />
        public virtual void TearDown()
        {
        }

        TestCase RunTest()
        {
            try
            {
                Setup();
                TestCase testCase = Check();
                TearDown();
                return testCase;
            }
            catch (Exception e)
            {
                TestCase testCase = BuildVerificationReport.Assert(GetIdentifier(), false, e.Message);
                testCase.StackTrace = e.StackTrace;
                TearDown();
                return testCase;
            }
        }
    }
}