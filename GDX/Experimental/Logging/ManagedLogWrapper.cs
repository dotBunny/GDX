// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

namespace GDX.Experimental.Logging
{
    public class ManagedLogWrapper : IList
    {
        public IEnumerator GetEnumerator()
        {
            int count = ManagedLog.GetEntryCount();
            for (int i = 0; i < count; i++)
            {
                yield return ManagedLog.GetEntryAt(i);
            }
        }

        public void CopyTo(Array array, int index)
        {
            // Immutable
        }

        public int Count
        {
            get
            {
                return ManagedLog.GetEntryCount();
            }
        }

        public bool IsSynchronized { get { return false; } }
        public object SyncRoot { get { return this; } }

        public int Add(object value)
        {
            return -1;
        }

        public void Clear()
        {
            // Immutable
        }

        public bool Contains(object value)
        {
            if (value == null) return false;
            LogEntry unboxed = (LogEntry)value;
            int count = ManagedLog.GetEntryCount();
            for (int i = 0; i < count; i++)
            {
                LogEntry currentEntry = ManagedLog.GetEntryAt(i);
                if (currentEntry.CompareTo(unboxed) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public int IndexOf(object value)
        {
            if (value == null) return -1;
            LogEntry unboxed = (LogEntry)value;
            int count = ManagedLog.GetEntryCount();
            for (int i = 0; i < count; i++)
            {
                LogEntry currentEntry = ManagedLog.GetEntryAt(i);
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

        public bool IsFixedSize { get { return false ; } }
        public bool IsReadOnly { get { return true; } }

        public object this[int index]
        {
            get
            {
                return ManagedLog.GetEntryAt(index);
            }
            set
            {

            }
        }
    }
}