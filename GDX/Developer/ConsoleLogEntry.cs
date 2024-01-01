// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.Developer
{
    public struct ConsoleLogEntry : IComparable<ConsoleLogEntry>, IComparer<ConsoleLogEntry>
    {
        public int Frame;
        public string FrameCount;
        public LogType Level;
        public string Message;
        public string StackTrace;

        public ConsoleLogEntry(LogType type, string message, string stackTrace = null)
        {
            Frame = Time.frameCount;
            FrameCount = Frame.ToString().PadLeft(10, '0');
            Level = type;
            Message = message;
            StackTrace = stackTrace;
        }

        public int CompareTo(ConsoleLogEntry other)
        {

            if (Frame > other.Frame)
            {
                return 1;
            }

            if (Frame < other.Frame)
            {
                return -1;
            }

            return 0;
        }

        public int Compare(ConsoleLogEntry x, ConsoleLogEntry y)
        {
            if (x.Frame > y.Frame)
            {
                return 1;
            }

            if (x.Frame < y.Frame)
            {
                return -1;
            }

            return 0;
        }
    }
}