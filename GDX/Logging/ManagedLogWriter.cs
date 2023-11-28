using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GDX.Logging
{
    public class ManagedLogWriter : IDisposable
    {
        readonly ConcurrentBag<LogEntry> m_Buffer = new ConcurrentBag<LogEntry>();
        readonly ConcurrentBag<LogEntry> m_BackBuffer = new ConcurrentBag<LogEntry>();

        readonly string m_Path;
        readonly CancellationTokenSource m_CancellationToken;
        readonly int m_MillisecondDelay;

        bool m_Flushing = false;

        public ManagedLogWriter(string filename, int millisecondDelay = 5000)
        {
            m_Path = Path.Combine(Platform.GetOutputFolder(), filename);
            m_MillisecondDelay = millisecondDelay;

            File.WriteAllText(m_Path, "");
            m_CancellationToken = new CancellationTokenSource();
            Thread thread = new Thread(() =>
            {
                Write(m_CancellationToken.Token);
            });
            thread.Start();
        }

        ~ManagedLogWriter()
        {
            Dispose();
        }

        public void Flush()
        {
            // m_Flushing = true;
            //
            // File.AppendAllLines(m_Path,m_Buffer.);
            // int count = m_Buffer.Count;
            // for (int i = 0; i < count; i++)
            // {
            //     File.AppendAllLinesAsync()
            // }
            // m_Buffer.GetEnumerator()
            //
            // File.AppendAllLines(m_Path, );
            // m_Buffer.Clear();
            //
            // m_Flushing = false;
            //
            // if (m_BackBuffer.Count > 0)
            // {
            //     m_Buffer.Add(m_BackBuffer.GetEnumerator())
            // }


        }

        public void AddEntry(LogEntry entry)
        {
            if (m_Flushing)
            {
                m_BackBuffer.Add(entry);
            }
            else
            {
                m_Buffer.Add(entry);
            }
        }

        void Write(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    Task.Delay(m_MillisecondDelay, token);
                    Flush();
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Flush();
            m_CancellationToken?.Cancel();
            m_CancellationToken?.Dispose();
        }
    }
}