using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GDX.Collections.Generic;

namespace GDX.Logging
{
    public class ManagedLogWriter : IDisposable
    {
        ExpandingArray<byte> m_Buffer = new ExpandingArray<byte>(2048);
        ExpandingArray<byte> m_DoubleBuffer = new ExpandingArray<byte>(2048);


        readonly CancellationTokenSource m_CancellationToken;
        readonly FileStream m_FileStream;

        bool m_Switch = false;

        readonly object m_LockBuffer = new object();
        readonly object m_LockDoubleBuffer = new object();

        readonly int m_MillisecondDelay;

        public static string GetDefaultLogPath()
        {
            return Path.Combine(Platform.GetOutputFolder(),
                $"ManagedLog_{DateTime.Now.ToString(Platform.FilenameTimestampFormat)}.log");
        }

        public ManagedLogWriter(string filePath, int millisecondDelay = 5000)
        {
            m_MillisecondDelay = millisecondDelay;
            m_FileStream = new FileStream(filePath, FileMode.Create);
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
            if (m_Switch)
            {
                m_Switch = false;
                lock (m_LockDoubleBuffer)
                {
                    if (m_DoubleBuffer.HasData())
                    {
                        m_FileStream.Write(m_DoubleBuffer.GetReadOnlySpan());
                        m_DoubleBuffer.Clear();
                    }
                }
            }
            else
            {
                m_Switch = true;
                lock (m_LockBuffer)
                {
                    if (m_Buffer.HasData())
                    {
                        m_FileStream.Write(m_Buffer.GetReadOnlySpan());
                        m_Buffer.Clear();
                    }
                }
            }
        }

        public void RecordEntry(LogEntry entry)
        {
            byte[] data = Encoding.UTF8.GetBytes(
                $"<{entry.Timestamp.ToString(Platform.TimestampFormat)}> [{ManagedLog.GetCategoryLabel(entry.CategoryIdentifier)}::{LogEntry.LogLevelToLabel(entry.Level)}] {entry.Message}\n\t@ {entry.SourceFilePath}:{entry.SourceLineNumber.ToString()}\n\r");

            if (m_Switch)
            {
                lock (m_LockDoubleBuffer)
                {
                    m_DoubleBuffer.AddRange(data);
                }
            }
            else
            {
                lock (m_LockBuffer)
                {
                    m_Buffer.AddRange(data);
                }
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
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            m_CancellationToken?.Cancel();
            m_CancellationToken?.Dispose();

            Flush();
            Flush();

            m_FileStream.DisposeAsync();
        }
    }
}
