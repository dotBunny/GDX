// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Developer.ConsoleCommands;
using GDX.Logging;
using UnityEditor;
using UnityEngine;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    // TODO: Hint system?
    // TOOD: CVars
    // TODO: Echo

    public static class Console
    {
        static ConsoleCommandBase.ConsoleCommandLevel s_AccessLevel = ConsoleCommandBase.ConsoleCommandLevel.Developer;
        static CircularBuffer<string> s_CommandHistory = new CircularBuffer<string>(50);
        static readonly Queue<ConsoleCommandBase> s_CommandBuffer = new Queue<ConsoleCommandBase>(50);

        static StringKeyDictionary<ConsoleCommandBase>
            s_KnownCommands = new StringKeyDictionary<ConsoleCommandBase>(50);

        // We keep a list of the commands handy because of how frequently we need to iterate over them
        static readonly List<string> k_KnownCommandsList = new List<string>(50);

        public static int PreviousCommandCount => s_CommandHistory.Count;

        public static void RegisterCommand(ConsoleCommandBase command)
        {
            string keyword = command.GetKeyword();
            if (!s_KnownCommands.ContainsKey(keyword))
            {
                s_KnownCommands.AddWithExpandCheck(keyword, command);
                k_KnownCommandsList.Add(keyword);
            }
        }

        public static void UnregisterCommand(ConsoleCommandBase command)
        {
            string keyword = command.GetKeyword();

            k_KnownCommandsList.Remove(keyword);
            s_KnownCommands.TryRemove(command.GetKeyword());
        }

        public static ConsoleCommandBase.ConsoleCommandLevel GetAccessLevel()
        {
            return s_AccessLevel;
        }

        public static string GetAutoComplete(string hint, int offset)
        {
            // TODO: Auto?
            return null;
        }

        public static ConsoleCommandBase GetCommand(string keyword)
        {
            return s_KnownCommands.ContainsKey(keyword) ? s_KnownCommands[keyword] : null;
        }

        public static string[] GetCommandKeywordsCopy()
        {
            return k_KnownCommandsList.ToArray();
        }

        public static List<string>.Enumerator GetCommandKeywordsEnumerator()
        {
            return k_KnownCommandsList.GetEnumerator();
        }

        public static string GetPreviousCommand(int offset = 0)
        {
            int count = s_CommandHistory.Count;
            if (count == 0)
            {
                return string.Empty;
            }

            if (offset >= count)
            {
                return s_CommandHistory[count - 1];
            }

            return s_CommandHistory[count - 1 - offset];
        }

        public static bool QueueCommand(string commandString)
        {
            string safeCommand = commandString.Trim();
            if (string.IsNullOrEmpty(safeCommand))
            {
                return false;
            }

            // Add to history even if bad
            s_CommandHistory.Add(safeCommand);

            // We do not split passed the command as to ensure the execution cost is passed down to the actual command
            // if subsequent args need to be processed.
            string[] split = safeCommand.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            ConsoleCommandBase command = GetCommand(split[0]);
            if (command != null)
            {
                if (command.GetAccessLevel() > s_AccessLevel)
                {
                    ManagedLog.Warning(LogCategory.Default,
                        $"Invalid Command: {split[0]} - A higher access level is required.");
                    return false;
                }

                ConsoleCommandBase workUnit = command.GetInstance(split.Length > 1 ? split[1].Trim() : null);
                if (workUnit == null)
                {
                    ManagedLog.Warning(LogCategory.Default,
                        $"Invalid Command: {split[0]} - Unable to generate work unit.");
                    return false;
                }

                s_CommandBuffer.Enqueue(workUnit);
                return true;
            }

            ManagedLog.Warning(LogCategory.Default, $"Invalid Command: {split[0]}");
            return false;
        }

        public static void SetAccessLevel(ConsoleCommandBase.ConsoleCommandLevel level)
        {
            s_AccessLevel = level;
        }

        public static void Tick(float deltaTime)
        {
            while (s_CommandBuffer.Count > 0)
            {
                ConsoleCommandBase command = s_CommandBuffer.Peek();

                // We evaluate inside of a try-catch to capture the exception and flush the command buffer.
                try
                {
                    if (!command.Evaluate(deltaTime))
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    uint parentMessageID = ManagedLog.Error(LogCategory.Default,
                        "An exception occured while processing the console command buffer. It has been flushed.");
                    ManagedLog.Exception(LogCategory.Default, e, parentMessageID);
                    s_CommandBuffer.Clear();
                    break;
                }

                s_CommandBuffer.Dequeue();
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
            // We preregister a bunch of commands to be optimal
            RegisterCommand(new HelpConsoleCommand());
            RegisterCommand(new QuitConsoleCommand());
            RegisterCommand(new ExecConsoleCommand());
            RegisterCommand(new PlayerLoopConsoleCommand());
            RegisterCommand(new VersionConsoleCommand());
            RegisterCommand(new WaitConsoleCommand());
            RegisterCommand(new SceneListConsoleCommand());
            RegisterCommand(new SceneLoadConsoleCommand());
            RegisterCommand(new SceneWaitConsoleCommand());
            RegisterCommand(new InputKeyPressConsoleCommand());
            RegisterCommand(new GarbageCollectionConsoleCommand());
            RegisterCommand(new BuildVerificationTestConsoleCommand());

            // We are going to look at the arguments
            if (CommandLineParser.Arguments.ContainsKey("exec"))
            {
                // We want to make sure that we have everything loading and ready before we actually execute a script
                QueueCommand("scene.wait");
                QueueCommand($"exec {CommandLineParser.Arguments["exec"]}");
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}