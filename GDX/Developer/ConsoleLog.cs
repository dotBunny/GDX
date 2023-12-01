// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;
using System.Collections;
using GDX.Collections.Generic;

namespace GDX.Developer
{
    public class ConsoleLog : IList
    {
        public uint Version = 0;

        ConcurrentCircularBuffer<ConsoleLogEntry> m_LogHistory = new ConcurrentCircularBuffer<ConsoleLogEntry>(1000);

        public ConsoleLogEntry GetEntryAt(int index)
        {
            return m_LogHistory[index];
        }

        public ConsoleLogEntry GetLastEntry()
        {
            return m_LogHistory[Count - 1];
        }

        public ConsoleLog()
        {
            Application.logMessageReceivedThreaded += OnMessageReceived;
        }

        ~ConsoleLog()
        {
            Application.logMessageReceivedThreaded -= OnMessageReceived;
        }

        void OnMessageReceived(string message, string stacktrace, LogType type)
        {
            m_LogHistory.Add(new ConsoleLogEntry(type, message, stacktrace));
            Version++;
        }

        public IEnumerator GetEnumerator()
        {
            int count = m_LogHistory.Count;
            for (int i = 0; i < count; i++)
            {
                yield return m_LogHistory[i];
            }
        }

        public void CopyTo(Array array, int index)
        {
            // Immutable
        }

        public int Count => m_LogHistory.Count;

        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public int Add(object value)
        {
            return -1;
        }

        public void Clear()
        {
            m_LogHistory.Clear();
            Version++;
        }

        public bool Contains(object value)
        {
            if (value == null)
            {
                return false;
            }

            ConsoleLogEntry unboxed = (ConsoleLogEntry)value;
            int count = m_LogHistory.Count;
            for (int i = 0; i < count; i++)
            {
                ConsoleLogEntry currentEntry = m_LogHistory[i];
                if (currentEntry.CompareTo(unboxed) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public int IndexOf(object value)
        {
            if (value == null)
            {
                return -1;
            }

            ConsoleLogEntry unboxed = (ConsoleLogEntry)value;
            int count = m_LogHistory.Count;
            for (int i = 0; i < count; i++)
            {
                ConsoleLogEntry currentEntry = m_LogHistory[i];
                if (currentEntry.CompareTo(unboxed) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Insert(int index, object value)
        {
            // Immutable
        }

        public void Remove(object value)
        {
            // Immutable
        }

        public void RemoveAt(int index)
        {
            // Immutable
        }

        public bool IsFixedSize => false;
        public bool IsReadOnly => true;

        public object this[int index]
        {
            get => m_LogHistory[index];
            set
            {
            }
        }
    }
}