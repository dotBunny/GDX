// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GDX.Collections;
using GDX.Collections.Generic;
using UnityEngine.Diagnostics;
using Object = UnityEngine.Object;

namespace GDX.Experimental
{
    /// <summary>
    ///     A logging system that lives in managed-space.
    /// </summary>
    public static class ManagedLog
    {
        public enum LogEntryLevel
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
            Exception,
            Assertion,
            Fatal
        }

        public const int GDX = -1;
        public const int Default = 0;
        public const int Platform = 1;
        public const int Unity = 2;
        public const int Input = 3;
        public const int Gameplay = 4;
        public const int Test = 5;
        public const int UI = 10;

        // Use bitfield for internals?
        static BitArray32 s_EchoToUnityBuiltIn = new BitArray32();
        static BitArray32 s_EchoToConsoleBuiltIn = new BitArray32();

        const int k_CategoryThreshold = 32;
        static readonly object m_Lock = new object();
        static uint s_TicketHead;

        static readonly bool s_EchoToUnity = true;

        static readonly bool s_EchoToConsole = false;


        // TODO ? categories to echo?
        // TODO ? categories to console
        static IntKeyDictionary<string> s_CustomCategories = new IntKeyDictionary<string>(10);
        static ConcurrentCircularBuffer<LogEntry> s_Buffer = new ConcurrentCircularBuffer<LogEntry>(1000);

        public static string LogEntryLevelToLabel(this LogEntryLevel level)
        {
            switch (level)
            {
                case LogEntryLevel.Trace:
                    return "Trace";
                case LogEntryLevel.Debug:
                    return "Debug";
                case LogEntryLevel.Info:
                    return "Info";
                case LogEntryLevel.Warning:
                    return "Warning";
                case LogEntryLevel.Error:
                    return "Error";
                case LogEntryLevel.Exception:
                    return "Exception";
                case LogEntryLevel.Assertion:
                    return "Assertion";
                case LogEntryLevel.Fatal:
                    return "Fatal";
            }

            return string.Empty;
        }

        static uint NextTicket()
        {
            lock (m_Lock)
            {
                s_TicketHead++;

                // We need to ensure this never hits it in rollover
                if (s_TicketHead == 0)
                {
                    s_TicketHead++;
                }

                return ++s_TicketHead;
            }
        }

        public static void RegisterCategory(int identifier, string name)
        {
            if (identifier <= k_CategoryThreshold)
            {
                Error(
                    $"Unable to register category {identifier.ToString()}:{name} as identifier below threshold {k_CategoryThreshold}.",
                    GDX);
            }

            if (s_CustomCategories.ContainsKey(identifier))
            {
                Error(
                    $"Unable to register category {identifier.ToString()}:{name} as identifier already in use.",
                    GDX);
            }
            else
            {
                s_CustomCategories.AddWithExpandCheck(identifier, name);
            }

        }

        public static string GetCategoryLabel(int category)
        {
            if (category <= k_CategoryThreshold)
            {
                switch (category)
                {
                    case GDX:
                        return "GDX";
                    case Default:
                        return "Default";
                    case Platform:
                        return "Platform";
                    case Unity:
                        return "Unity";
                    case Input:
                        return "Input";
                    case Gameplay:
                        return "Gameplay";
                    case UI:
                        return "UI";
                    case Test:
                        return "Test";
                    default:
                        return "Undefined";
                }
            }

            return s_CustomCategories.TryGetValue(category, out string customCategory) ? customCategory : "Undefined";
        }

        public static void SetBufferSize(long byteAllocation)
        {
            s_Buffer = new ConcurrentCircularBuffer<LogEntry>((int)(byteAllocation /
                                                                    Marshal.SizeOf(
                                                                        typeof(LogEntry))));
        }

        public static void SetBufferCount(int numberOfLogEntries)
        {
            s_Buffer = new ConcurrentCircularBuffer<LogEntry>(numberOfLogEntries);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LogEntry NewEntry(LogEntryLevel level, string message, int category = 0, uint parentIdentifier = 0u,
            string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            LogEntry newEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Identifier = NextTicket(),
                CategoryIdentifier = category,
                ParentIdentifier = parentIdentifier,
                Message = message,
                MemberName = memberName,
                SourceFilePath = sourceFilePath,
                SourceLineNumber = sourceLineNumber
            };
            s_Buffer.Add(newEntry);
            return newEntry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LogEntry NewEntry(LogEntryLevel level, Exception exception, int category = 0, uint parentIdentifier = 0u,
            string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            LogEntry newEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Identifier = NextTicket(),
                CategoryIdentifier = category,
                ParentIdentifier = parentIdentifier,
                Message = exception.Message,
                MemberName = memberName,
                SourceFilePath = sourceFilePath,
                SourceLineNumber = sourceLineNumber
            };
            s_Buffer.Add(newEntry);
            return newEntry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Debug(object debugObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Debug, debugObject.ToString(), category, associativeIdentifier,
                memberName, sourceFilePath, sourceLineNumber);


            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.Log(newEntry.ToUnityOutput());
            }


            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DebugWithContext(object debugObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Debug, debugObject.ToString(), category, associativeIdentifier,
                memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.Log(newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Info(object infoObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Info, infoObject.ToString(), category, associativeIdentifier,
                memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.Log(newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint InfoWithContext(object infoObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Info, infoObject.ToString(), category, associativeIdentifier,
                memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.Log(newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Warning(object warningObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Warning, warningObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogWarning(newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WarningWithContext(object warningObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Warning, warningObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogWarning(newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Error(object errorObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Error, errorObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogError(newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ErrorWithContext(object errorObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Error, errorObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogError(newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Exception(Exception exceptionObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Exception, exceptionObject, category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogException(exceptionObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExceptionWithContext(Exception exceptionObject, int category = 0,
            Object contextObject = null, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Exception, exceptionObject, category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogException(exceptionObject, contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Assertion(object assertionObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Assertion, assertionObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogAssertion(newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AssertionWithContext(object assertionObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Assertion, assertionObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogAssertion(newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Fatal(object fatalObject, int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Fatal, fatalObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogError(newEntry.ToUnityOutput());
                Utils.ForceCrash(ForcedCrashCategory.FatalError);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FatalWithContext(object fatalObject, int category = 0, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Fatal, fatalObject.ToString(), category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnity)
            {
                UnityEngine.Debug.LogError(newEntry.ToUnityOutput(), contextObject);
                Utils.ForceCrash(ForcedCrashCategory.FatalError);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Trace(int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogEntryLevel.Trace, Environment.StackTrace, category,
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole)
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            return newEntry.Identifier;
        }
        // TODO: By level? or by category?


        public struct LogEntry
        {
            public DateTime Timestamp;
            public LogEntryLevel Level;
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
                    $"[{GetCategoryLabel(CategoryIdentifier)}::{Level.LogEntryLevelToLabel()}] {Message}\n\t@ {SourceFilePath}:{SourceLineNumber.ToString()}";
            }

            public string ToUnityOutput()
            {
                return  $"[{GetCategoryLabel(CategoryIdentifier)}] {Message}\n@ {SourceFilePath}:{SourceLineNumber.ToString()}";
            }
        }
    }
}