// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX.Experimental.Logging
{
    public struct LogEntry
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
            return  $"{Message}\n\t@ {SourceFilePath}:{SourceLineNumber.ToString()}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string LogLevelToLabel(LogLevel level)
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
    }
}