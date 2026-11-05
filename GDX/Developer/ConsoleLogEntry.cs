// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GDX.Developer
{
    public struct ConsoleLogEntry : IComparable<ConsoleLogEntry>, IComparer<ConsoleLogEntry>
    {
        /// <summary>
        ///     The frame of the last entry made on the main thread.
        /// </summary>
        /// <remarks>
        ///     Unity's frame count can only be read on the main thread, and log messages arrive on whichever thread
        ///     logged them (<see cref="Application.logMessageReceivedThreaded" />): an entry made on another thread
        ///     takes this frame, so it still sorts among the main thread's.
        /// </remarks>
        static int s_LastFrame;

        public int Frame;
        public string FrameCount;
        public LogType Level;
        public string Message;
        public string StackTrace;

        public ConsoleLogEntry(LogType type, string message, string stackTrace = null)
        {
            if (Thread.CurrentThread.ManagedThreadId == Core.MainThreadID)
            {
                s_LastFrame = Time.frameCount;
            }

            Frame = s_LastFrame;
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