// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using GDX.Collections;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using Object = UnityEngine.Object;

namespace GDX.Experimental.Logging
{
    /// <summary>
    ///     A logging system that lives in managed-space.
    /// </summary>
    public static class ManagedLog
    {
        static BitArray64 s_EchoToUnityBuiltIn = new BitArray64(uint.MaxValue);
        static BitArray64 s_EchoToConsoleBuiltIn = new BitArray64(uint.MaxValue);

        static readonly object m_Lock = new object();
        static uint s_TicketHead;

        // TODO ? categories to echo?
        // TODO ? categories to console
        static IntKeyDictionary<string> s_CustomCategories = new IntKeyDictionary<string>(10);
        static ConcurrentCircularBuffer<LogEntry> s_Buffer = new ConcurrentCircularBuffer<LogEntry>(1000);

        public static void Clear()
        {
            s_Buffer.Clear();
        }
        public static void RegisterCategory(int identifier, string name, bool outputToUnity = true, bool outputToConsole = false)
        {
            if (identifier <= LogCategory.k_CategoryThreshold)
            {
                Error(LogCategory.GDX, $"Unable to register category {identifier.ToString()}:{name} as identifier below threshold {LogCategory.k_CategoryThreshold.ToString()}.");
            }
            else if (identifier >= 64)
            {
                Error(LogCategory.GDX, $"Unable to register category {identifier.ToString()}:{name} as identifier exceeds maximum allow categories (64).");
            }

            if (s_CustomCategories.ContainsKey(identifier))
            {
                Error(LogCategory.GDX, $"Unable to register category {identifier.ToString()}:{name} as identifier already in use.");
            }
            else
            {
                s_CustomCategories.AddWithExpandCheck(identifier, name);
                s_EchoToConsoleBuiltIn[(byte)identifier] = outputToConsole;
                s_EchoToUnityBuiltIn[(byte)identifier] = outputToUnity;
            }
        }

        public static void UnregisterCategory(int identifier)
        {
            if (identifier <= LogCategory.k_CategoryThreshold) return;
            s_CustomCategories.TryRemove(identifier);
            s_EchoToConsoleBuiltIn[(byte)identifier] = true;
            s_EchoToUnityBuiltIn[(byte)identifier] = true;
        }

        public static string GetCategoryLabel(int category)
        {
            if (category <= LogCategory.k_CategoryThreshold)
            {
                switch (category)
                {
                    case LogCategory.GDX:
                        return "GDX";
                    case LogCategory.Default:
                        return "Default";
                    case LogCategory.Platform:
                        return "Platform";
                    case LogCategory.Unity:
                        return "Unity";
                    case LogCategory.Input:
                        return "Input";
                    case LogCategory.Gameplay:
                        return "Gameplay";
                    case LogCategory.UI:
                        return "UI";
                    case LogCategory.Test:
                        return "Test";
                    default:
                        return "Undefined";
                }
            }

            return s_CustomCategories.TryGetValue(category, out string customCategory) ? customCategory : "Undefined";
        }

        public static void SetConsoleOutput(int identifier, bool output)
        {
            s_EchoToConsoleBuiltIn[(byte)identifier] = output;
        }

        public static LogEntry GetLastEntry()
        {
            return s_Buffer.GetBack();
        }
        public static void SetUnityOutput(int identifier, bool output)
        {
            s_EchoToUnityBuiltIn[(byte)identifier] = output;
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
        public static uint Debug(int category, object debugObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Debug, category, debugObject.ToString(),
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DebugWithContext(int category, object debugObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Debug, category, debugObject.ToString(),
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Info(int category, object infoObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Info, category, infoObject.ToString(),
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(
                    LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier),
                    newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint InfoWithContext(int category, object infoObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Info, category, infoObject.ToString(),
                associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(
                    LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier),
                    newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Warning(int category, object warningObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Warning, category,
                warningObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Warning,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WarningWithContext(int category, object warningObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Warning, category,
                warningObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Warning,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Error(int category, object errorObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Error, category,
                errorObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ErrorWithContext(int category, object errorObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Error, category,
                errorObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Exception(int category, Exception exceptionObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Exception, category,
                exceptionObject, associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Exception,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExceptionWithContext(int category,
            Exception exceptionObject,
            Object contextObject = null, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Exception, category,
                exceptionObject, associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Exception,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Assertion(int category, object assertionObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Assertion, category,
                assertionObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AssertionWithContext(int category, object assertionObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Assertion, category,
                assertionObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Fatal(int category, object fatalObject, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Fatal, category,
                fatalObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            Utils.ForceCrash(ForcedCrashCategory.FatalError);

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FatalWithContext(int category, object fatalObject, Object contextObject = null,
            uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Fatal, category,
                fatalObject.ToString(), associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToUnityBuiltIn[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            Utils.ForceCrash(ForcedCrashCategory.FatalError);

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Trace(int category = 0, uint associativeIdentifier = 0,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogEntry newEntry = NewEntry(LogLevel.Trace, category,
                Environment.StackTrace, associativeIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsoleBuiltIn[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            return newEntry.Identifier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LogEntry NewEntry(LogLevel level, int category, string message, uint parentIdentifier = 0u,
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
        static LogEntry NewEntry(LogLevel level, int category, Exception exception, uint parentIdentifier = 0u,
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
    }
}