// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.Developer
{
    public struct ConsoleLogEntry : IComparable<ConsoleLogEntry>, IComparer<ConsoleLogEntry>
    {
        public DateTime Timestamp;
        public LogType Level;
        public string Message;
        public string StackTrace;

        public ConsoleLogEntry(LogType type, string message, string stackTrace = null)
        {
            Timestamp = DateTime.Now;
            Level = type;
            Message = message;
            StackTrace = stackTrace;
        }

        public int CompareTo(ConsoleLogEntry other)
        {

            if (Timestamp.Ticks > other.Timestamp.Ticks)
            {
                return 1;
            }

            if (Timestamp.Ticks < other.Timestamp.Ticks)
            {
                return -1;
            }

            return 0;
        }

        public int Compare(ConsoleLogEntry x, ConsoleLogEntry y)
        {
            if (x.Timestamp.Ticks > y.Timestamp.Ticks)
            {
                return 1;
            }

            if (x.Timestamp.Ticks < y.Timestamp.Ticks)
            {
                return -1;
            }

            return 0;
        }
    }
}