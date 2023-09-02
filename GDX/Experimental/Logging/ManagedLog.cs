// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
    ///     A managed-only categorized hierarchical logging solution.
    /// </summary>
    public static class ManagedLog
    {
        // TODO : buffer that flushes to disk log as well
        
        /// <summary>
        ///     A ring buffer structure of <see cref="LogEntry" />s.
        /// </summary>
        static ConcurrentCircularBuffer<LogEntry> s_Buffer = new ConcurrentCircularBuffer<LogEntry>(1000);

        /// <summary>
        ///     An index of labels per category.
        /// </summary>
        static IntKeyDictionary<string> s_CustomCategories = new IntKeyDictionary<string>(10);

        /// <summary>
        ///     A flag structure containing if a category should output to the system console.
        /// </summary>
        static BitArray64 s_EchoToConsole = new BitArray64(uint.MaxValue);

        /// <summary>
        ///     A flag structure containing if a category should output to the Unity log handler.
        /// </summary>
        static BitArray64 s_EchoToLogger = new BitArray64(uint.MaxValue);

        /// <summary>
        ///     The next identifier used when creating a new <see cref="LogEntry" />.
        /// </summary>
        static uint s_IdentifierHead;

        /// <summary>
        ///     Thread-safety management object.
        /// </summary>
        static readonly object k_Lock = new object();

        /// <summary>
        ///     Clears the internal buffer of <see cref="LogEntry" />s.
        /// </summary>
        public static void Clear()
        {
            s_Buffer.Clear();
        }

        /// <summary>
        ///     Registers a category identifier for logging purposes.
        /// </summary>
        /// <param name="categoryIdentifier">
        ///     The designated identifier, must fall within a valid range. See
        ///     <see cref="LogCategory.MinimumIdentifier" /> and <see cref="LogCategory.MaximumIdentifier" />
        /// </param>
        /// <param name="name">The category's name.</param>
        /// <param name="outputToUnity">Should this category output to Unity's log handler?</param>
        /// <param name="outputToConsole">Should this category output to the Console.</param>
        public static void RegisterCategory(int categoryIdentifier, string name, bool outputToUnity = true,
            bool outputToConsole = false)
        {
            if (categoryIdentifier <= LogCategory.MinimumIdentifier)
            {
                Error(LogCategory.GDX,
                    $"Unable to register category {categoryIdentifier.ToString()}:{name} as identifier below threshold {LogCategory.MinimumIdentifier.ToString()}.");
            }
            else if (categoryIdentifier > LogCategory.MaximumIdentifier)
            {
                Error(LogCategory.GDX,
                    $"Unable to register category {categoryIdentifier.ToString()}:{name} as identifier exceeds maximum allow categories (64).");
            }

            if (s_CustomCategories.ContainsKey(categoryIdentifier))
            {
                Error(LogCategory.GDX,
                    $"Unable to register category {categoryIdentifier.ToString()}:{name} as identifier already in use.");
            }
            else
            {
                s_CustomCategories.AddWithExpandCheck(categoryIdentifier, name);
                s_EchoToConsole[(byte)categoryIdentifier] = outputToConsole;
                s_EchoToLogger[(byte)categoryIdentifier] = outputToUnity;
            }
        }

        /// <summary>
        ///     Unregisters a category identifier.
        /// </summary>
        /// <param name="categoryIdentifier"></param>
        public static void UnregisterCategory(int categoryIdentifier)
        {
            if (categoryIdentifier <= LogCategory.MinimumIdentifier)
            {
                return;
            }

            s_CustomCategories.TryRemove(categoryIdentifier);
            s_EchoToConsole[(byte)categoryIdentifier] = true;
            s_EchoToLogger[(byte)categoryIdentifier] = true;
        }

        /// <summary>
        ///     Gets the name of a category.
        /// </summary>
        /// <param name="categoryIdentifier">The category to evaluate.</param>
        /// <returns>The label for the category.</returns>
        public static string GetCategoryLabel(int categoryIdentifier)
        {
            if (categoryIdentifier <= LogCategory.MinimumIdentifier)
            {
                switch (categoryIdentifier)
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
                        return LogCategory.UndefinedLabel;
                }
            }

            return s_CustomCategories.TryGetValue(categoryIdentifier, out string customCategory)
                ? customCategory
                : LogCategory.UndefinedLabel;
        }

        /// <summary>
        ///     Gets an array of <see cref="LogEntry" /> based on category.
        /// </summary>
        /// <param name="categoryIdentifier">The category identifier to filter by.</param>
        /// <returns>An array of filtered <see cref="LogEntry" />.</returns>
        public static LogEntry[] GetEntriesByCategory(int categoryIdentifier)
        {
            int count = s_Buffer.Count;
            SimpleList<LogEntry> entries = new SimpleList<LogEntry>(count);
            for (int i = 0; i < count; i++)
            {
                if (s_Buffer[i].Identifier == categoryIdentifier)
                {
                    entries.AddUnchecked(s_Buffer[i]);
                }
            }

            entries.Compact();
            return entries.Array;
        }

        /// <summary>
        ///     Gets an array of <see cref="LogEntry" /> based on parent identifier.
        /// </summary>
        /// <param name="parentIdentifier">The parent identifier to filter by.</param>
        /// <returns>An array of filtered <see cref="LogEntry" />.</returns>
        public static LogEntry[] GetEntriesByParent(int parentIdentifier)
        {
            int count = s_Buffer.Count;
            SimpleList<LogEntry> entries = new SimpleList<LogEntry>(count);
            for (int i = 0; i < count; i++)
            {
                if (s_Buffer[i].ParentIdentifier == parentIdentifier)
                {
                    entries.AddUnchecked(s_Buffer[i]);
                }
            }

            entries.Compact();
            return entries.Array;
        }

        /// <summary>
        ///     Gets an array of <see cref="LogEntry" /> based on contents.
        /// </summary>
        /// <param name="searchQuery">The text query to search for.</param>
        /// <returns>An array of filtered <see cref="LogEntry" />.</returns>
        public static LogEntry[] GetEntriesBySearch(string searchQuery)
        {
            int count = s_Buffer.Count;
            SimpleList<LogEntry> entries = new SimpleList<LogEntry>(count);
            for (int i = 0; i < count; i++)
            {
                if (s_Buffer[i].Message.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1 ||
                    s_Buffer[i].SourceFilePath.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    entries.AddUnchecked(s_Buffer[i]);
                }
            }

            entries.Compact();
            return entries.Array;
        }

        /// <summary>
        ///     Get the last <see cref="LogEntry" /> added to the backing buffer.
        /// </summary>
        /// <returns>The last log entry.</returns>
        public static LogEntry GetLastEntry()
        {
            return s_Buffer.GetBack();
        }

        /// <summary>
        ///     Sets the size of the internal <see cref="ConcurrentCircularBuffer{T}" /> based on a specific memory size
        ///     allocation.
        /// </summary>
        /// <param name="byteAllocation">The size in bytes to use for the internal buffer.</param>
        public static void SetBufferSizeByAllocation(long byteAllocation)
        {
            s_Buffer = new ConcurrentCircularBuffer<LogEntry>((int)(byteAllocation /
                                                                    Marshal.SizeOf(
                                                                        typeof(LogEntry))));
        }

        /// <summary>
        ///     Sets the size of the internal <see cref="ConcurrentCircularBuffer{T}" /> to be a specific number of entries.
        /// </summary>
        /// <param name="numberOfLogEntries">The number of <see cref="LogEntry" /> to size the buffer too.</param>
        public static void SetBufferSizeByCount(int numberOfLogEntries)
        {
            s_Buffer = new ConcurrentCircularBuffer<LogEntry>(numberOfLogEntries);
        }

        /// <summary>
        ///     Set if a category should output to the system console.
        /// </summary>
        /// <param name="categoryIdentifier">The unique category identifier.</param>
        /// <param name="output">Should the log entry be outputted to the console?</param>
        public static void SetConsoleOutput(int categoryIdentifier, bool output)
        {
            s_EchoToConsole[(byte)categoryIdentifier] = output;
        }

        /// <summary>
        ///     Set if a category should output to the unity logger.
        /// </summary>
        /// <param name="categoryIdentifier">The unique category identifier.</param>
        /// <param name="output">Should the log entry be outputted to the unity logger?</param>
        public static void SetUnityOutput(int categoryIdentifier, bool output)
        {
            s_EchoToLogger[(byte)categoryIdentifier] = output;
        }

        /// <summary>
        ///     Log a debug message.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Debug(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
                // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Debug, category, message,
                parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log a debug message with context.
        /// </summary>
        /// <remarks>The context-based versions of logging are slower.</remarks>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DebugWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
                // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Debug, category, message,
                parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log some information.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Info(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Info, category, message,
                parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(
                    LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier),
                    newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log some information with context.
        /// </summary>
        /// <remarks>The context-based versions of logging are slower.</remarks>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint InfoWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Info, category, message,
                parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(
                    LogType.Log,
                    GetCategoryLabel(newEntry.CategoryIdentifier),
                    newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log a warning.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Warning(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
                // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Warning, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Warning,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log a warning with context.
        /// </summary>
        /// <remarks>The context-based versions of logging are slower.</remarks>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint WarningWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Warning, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Warning,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an error.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Error(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Error, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an error with context.
        /// </summary>
        /// <remarks>The context-based versions of logging are slower.</remarks>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ErrorWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Error, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an exception.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="exception">The exception object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Exception(int category, Exception exception, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Exception, category,
                exception, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Exception,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an exception with context,
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="exception">The exception object.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ExceptionWithContext(int category,
                Exception exception,
                Object contextObject = null, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Exception, category,
                exception, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Exception,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an assertion failure.
        /// </summary>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Assertion(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Assertion, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Log an assertion failure with context.
        /// </summary>
        /// <remarks>The context-based versions of logging are slower.</remarks>
        /// <param name="category">The category identifier.</param>
        /// <param name="message">The <see cref="LogEntry" /> message.</param>
        /// <param name="contextObject">A Unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The optional parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AssertionWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Assertion, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Logs a fatal event and forces Unity to crash.
        /// </summary>
        /// <remarks>
        ///     The crash ensures that we stop a cascade of problems, allowing for back tracing. The context object based
        ///     methods should no be the preferred method of logging as they require a main-thread lock internally inside of Unity.
        /// </remarks>
        /// <param name="category">The category identifier to associate this event with.</param>
        /// <param name="message"></param>
        /// <param name="parentIdentifier">
        ///     The parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The newly created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Fatal(int category, string message, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Fatal, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            Utils.ForceCrash(ForcedCrashCategory.FatalError);

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Logs a fatal event and forces Unity to crash.
        /// </summary>
        /// <remarks>
        ///     The crash ensures that we stop a cascade of problems, allowing for back tracing. The context object based
        ///     methods should no be the preferred method of logging as they require a main-thread lock internally inside of Unity.
        /// </remarks>
        /// <param name="category">The category identifier to associate this event with.</param>
        /// <param name="message"></param>
        /// <param name="contextObject">A unity-based context object.</param>
        /// <param name="parentIdentifier">
        ///     The parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The newly created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint FatalWithContext(int category, string message, Object contextObject = null,
                uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Fatal, category,
                message, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Error,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput(), contextObject);
            }

            Utils.ForceCrash(ForcedCrashCategory.FatalError);

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Logs the current stack trace.
        /// </summary>
        /// <remarks>Pulled from <see cref="Environment.StackTrace " />.</remarks>
        /// <param name="category">The category identifier to associate this event with.</param>
        /// <param name="parentIdentifier">
        ///     The parent <see cref="LogEntry" />'s Identifier, if this event is meant to be
        ///     associated with another.
        /// </param>
        /// <returns>The newly created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Trace(int category = 0, uint parentIdentifier = 0,
                // ReSharper disable InvalidXmlDocComment
                [CallerMemberName] string memberName = "",
                [CallerFilePath] string sourceFilePath = "",
                [CallerLineNumber] int sourceLineNumber = 0)
            // ReSharper restore InvalidXmlDocComment
        {
            LogEntry newEntry = NewEntry(LogLevel.Trace, category,
                Environment.StackTrace, parentIdentifier, memberName, sourceFilePath, sourceLineNumber);

            if (s_EchoToConsole[(byte)category])
            {
                Console.WriteLine(newEntry.ToConsoleOutput());
            }

            if (s_EchoToLogger[(byte)category])
            {
                UnityEngine.Debug.unityLogger.Log(LogType.Assert,
                    GetCategoryLabel(newEntry.CategoryIdentifier), newEntry.ToUnityOutput());
            }

            return newEntry.Identifier;
        }

        /// <summary>
        ///     Creates and adds a new log entry to the <see cref="s_Buffer" />.
        /// </summary>
        /// <param name="level">The type/level of message being emitted.</param>
        /// <param name="category">The category of the message being emitted.</param>
        /// <param name="message">The content.</param>
        /// <param name="parentIdentifier">The parent <see cref="LogEntry" />'s Identifier.</param>
        /// <param name="memberName">The invoking member's name.</param>
        /// <param name="sourceFilePath">The source file where the member is found.</param>
        /// <param name="sourceLineNumber">The line number in the source file where the member is invoking the emit.</param>
        /// <returns>The newly created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LogEntry NewEntry(LogLevel level, int category, string message, uint parentIdentifier = 0u,
            string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            LogEntry newEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Identifier = NextIdentifier(),
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

        /// <summary>
        ///     Creates and adds a new log entry to the <see cref="s_Buffer" /> from a <see cref="System.Exception" />.
        /// </summary>
        /// <param name="level">The type/level of message being emitted.</param>
        /// <param name="category">The category of the message being emitted.</param>
        /// <param name="exception">The emitted exception.</param>
        /// <param name="parentIdentifier">The parent <see cref="LogEntry" />'s Identifier.</param>
        /// <param name="memberName">The invoking member's name.</param>
        /// <param name="sourceFilePath">The source file where the member is found.</param>
        /// <param name="sourceLineNumber">The line number in the source file where the member is invoking the emit.</param>
        /// <returns>The newly created <see cref="LogEntry" />'s Identifier.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static LogEntry NewEntry(LogLevel level, int category, Exception exception, uint parentIdentifier = 0u,
            string memberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
        {
            LogEntry newEntry = new LogEntry
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Identifier = NextIdentifier(),
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

        /// <summary>
        ///     Get the next identifier to use for a log entry
        /// </summary>
        /// <remarks>This will eventually roll over.</remarks>
        /// <returns>An unsigned integer representing a pseudo-unique log entry identifier.</returns>
        static uint NextIdentifier()
        {
            lock (k_Lock)
            {
                s_IdentifierHead++;

                // We need to ensure this never hits it in rollover
                if (s_IdentifierHead == 0)
                {
                    s_IdentifierHead++;
                }

                return ++s_IdentifierHead;
            }
        }

        class ManagedLogHandler : ILogHandler
        {
            /// <inheritdoc />
            public void LogFormat(LogType logType, Object context, string format, params object[] args)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public void LogException(Exception exception, Object context)
            {
                throw new NotImplementedException();
            }
        }
    }
}