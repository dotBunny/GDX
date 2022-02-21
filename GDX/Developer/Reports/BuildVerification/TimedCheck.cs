using System.Diagnostics;

namespace GDX.Developer.Reports.BuildVerification
{
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
            BVT.Assert(Identifier, condition, failMessage, Duration);
        }
    }
}