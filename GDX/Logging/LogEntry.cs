// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GDX.Collections.Generic;

namespace GDX.Logging
{
    public struct LogEntry : IComparable<LogEntry>, IComparer<LogEntry>
    {
        public DateTime Timestamp;
        public LogLevel Level;
        public ulong Tick;
        public uint Identifier;
        public uint ParentIdentifier;
        public int CategoryIdentifier;
        public string Message;
        public string MemberName;
        public string SourceFilePath;
        public int SourceLineNumber;

        public string ToConsoleOutput()
        {
            return
                $"[{ManagedLog.GetCategoryLabel(CategoryIdentifier)}::{LogLevelToLabel(Level)}] {Message}\n\t@ {SourceFilePath}:{SourceLineNumber.ToString()}";
        }

        public string ToUnityOutput()
        {
            return $"{Message}\n\t@ {SourceFilePath}:{SourceLineNumber.ToString()}";
        }

        public string GetLevelLabel()
        {
            return LogLevelToLabel(Level);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string LogLevelToLabel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "Trace";
                case LogLevel.Debug:
                    return "Debug";
                case LogLevel.Info:
                    return "Info";
                case LogLevel.Warning:
                    return "Warning";
                case LogLevel.Error:
                    return "Error";
                case LogLevel.Exception:
                    return "Exception";
                case LogLevel.Assertion:
                    return "Assertion";
                case LogLevel.Fatal:
                    return "Fatal";
            }

            return string.Empty;
        }


        public int CompareTo(LogEntry other)
        {
            if (Identifier > other.Identifier)
            {
                return 1;
            }

            if (Identifier < other.Identifier)
            {
                return -1;
            }

            return 0;
        }

        public int Compare(LogEntry x, LogEntry y)
        {
            if (x.Identifier > y.Identifier)
            {
                return 1;
            }

            if (x.Identifier < y.Identifier)
            {
                return -1;
            }

            return 0;
        }
    }
}