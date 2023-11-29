// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.ConsoleCommands;
using GDX.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class Console
    {

        public enum ConsoleAccessLevel
        {
            Anonymous = -1,
            User = 0,
            Superuser = 1,
            Developer = 2
        }

        static ConsoleAccessLevel s_AccessLevel = ConsoleAccessLevel.Developer;
        static CircularBuffer<string> s_CommandHistory = new CircularBuffer<string>(50);
        static readonly Queue<ConsoleCommandBase> k_CommandBuffer = new Queue<ConsoleCommandBase>(50);
        static StringKeyDictionary<ConsoleCommandBase>
            s_KnownCommands = new StringKeyDictionary<ConsoleCommandBase>(50);
        static StringKeyDictionary<ConsoleVariableBase> s_ConsoleVariables =
            new StringKeyDictionary<ConsoleVariableBase>(
                50);

        static readonly List<string> k_KnownCommandsList = new List<string>(50);
        static readonly List<string> k_KnownVariablesList = new List<string>(50);
        static CircularBuffer<LogEntry> s_LogHistory = new CircularBuffer<LogEntry>(1000);
        static int s_HintCacheLength = 0;

        public static int PreviousCommandCount => s_CommandHistory.Count;
        public static int CommandBufferCount => k_CommandBuffer.Count;


        static SimpleWatch s_BufferCountWatch = new SimpleWatch("console.buffer", "Buffered Commands",
            () => CommandBufferCount.ToString(), false);

        public static void RegisterCommand(ConsoleCommandBase command)
        {
            string keyword = command.GetKeyword();
            if (!s_KnownCommands.ContainsKey(keyword))
            {
                s_KnownCommands.AddWithExpandCheck(keyword, command);
                k_KnownCommandsList.Add(keyword);
                s_HintCacheLength++;
            }
        }

        public static void RegisterVariable(ConsoleVariableBase variable)
        {
            string name = variable.GetName();
            if (s_ConsoleVariables.ContainsKey(name))
            {
                return;
            }

            s_ConsoleVariables.AddWithExpandCheck(name, variable);
            k_KnownVariablesList.Add(name);
            s_HintCacheLength++;
        }

        public static void UnregisterVariable(ConsoleVariableBase variable)
        {
            UnregisterVariable(variable.GetName());
        }

        public static void UnregisterVariable(string name)
        {
            k_KnownVariablesList.Remove(name);
            s_ConsoleVariables.TryRemove(name);
            s_HintCacheLength--;
        }

        public static bool HasVariable(string name)
        {
            return k_KnownVariablesList.Contains(name);
        }

        public static int GetVariableCount()
        {
            return k_KnownVariablesList.Count;
        }

        public static void UnregisterCommand(ConsoleCommandBase command)
        {
            UnregisterCommand(command.GetKeyword());
        }

        public static void UnregisterCommand(string keyword)
        {
            k_KnownCommandsList.Remove(keyword);
            s_KnownCommands.TryRemove(keyword);
        }

        public static ConsoleAccessLevel GetAccessLevel()
        {
            return s_AccessLevel;
        }


        static string[] FilterStartsWith(string filter, string[] set)
        {
            int count = set.Length;
            SimpleList<string> possible = new SimpleList<string>(count);
            for (int i = 0; i < count; i++)
            {
                string evalString = set[i];
                if (evalString.StartsWith(filter))
                {
                    possible.AddUnchecked(evalString);
                }
            }
            possible.Compact();
            return possible.Array;
        }

        static string[] FilterContains(string filter, string[] set)
        {
            int count = set.Length;
            SimpleList<string> possible = new SimpleList<string>(count);
            for (int i = 0; i < count; i++)
            {
                string evalString = set[i];
                if (evalString.Contains(filter))
                {
                    possible.AddUnchecked(evalString);
                }
            }
            possible.Compact();
            return possible.Array;
        }


        public static string[] GetCommandAutoCompleteSuggestions(string hint, string[] existingSet = null)
        {
            if (existingSet != null)
            {
                return FilterStartsWith(hint, existingSet);
            }

            SimpleList<string> possibleCommands = new SimpleList<string>(s_HintCacheLength);
            int commandCount = k_KnownCommandsList.Count;
            for (int i = 0; i < commandCount; i++)
            {
                string evalString = k_KnownCommandsList[i];
                if (evalString.StartsWith(hint))
                {
                    possibleCommands.AddUnchecked(evalString);
                }
            }

            int variableCount = k_KnownVariablesList.Count;
            for (int i = 0; i < variableCount; i++)
            {
                string evalString = k_KnownVariablesList[i];
                if (evalString.StartsWith(hint))
                {
                    possibleCommands.AddUnchecked(evalString);
                }
            }
            possibleCommands.Compact();
            return possibleCommands.Array;
        }

        public static ConsoleCommandBase GetCommand(string keyword)
        {
            return s_KnownCommands.ContainsKey(keyword) ? s_KnownCommands[keyword] : null;
        }

        public static ConsoleVariableBase GetVariable(string name)
        {
            return s_ConsoleVariables.ContainsKey(name) ? s_ConsoleVariables[name] : null;
        }

        public static string[] GetCommandKeywordsCopy()
        {
            return k_KnownCommandsList.ToArray();
        }

        public static string[] GetVariableNamesCopy()
        {
            return k_KnownVariablesList.ToArray();
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

            // Check if its a valid command
            ConsoleCommandBase command = GetCommand(split[0]);
            if (command != null)
            {
                if (command.GetAccessLevel() > s_AccessLevel)
                {
                    Debug.LogWarning($"Invalid Command: {split[0]} - A higher access level is required.");
                    return false;
                }

                ConsoleCommandBase workUnit = command.GetInstance(split.Length > 1 ? split[1].Trim() : null);
                if (workUnit == null)
                {
                    Debug.LogWarning($"Invalid Command: {split[0]} - Unable to generate work unit.");
                    return false;
                }

                k_CommandBuffer.Enqueue(workUnit);
                return true;
            }

            // Check for console variables
            ConsoleVariableBase variable = GetVariable(split[0]);
            if (variable != null)
            {
                if (split.Length > 1) // Set Value
                {
                    if (variable.GetAccessLevel() > s_AccessLevel)
                    {
                        Debug.LogWarning($"Unable to set {split[0]} - A higher access level is required.");
                        return false;
                    }

                    variable.SetValueFromString(split[1].Trim());
                    Debug.Log($"{variable.GetName()} is now '{variable.GetCurrentValueAsString()}'");
                }
                else // Echo
                {
                    Debug.Log($"{variable.GetName()} is '{variable.GetCurrentValueAsString()}' [{variable.GetDefaultValueAsString()}]");
                }

                return true;
            }

            Debug.LogWarning($"Invalid Command: {split[0]}");
            return false;
        }

        public static void SetAccessLevel(ConsoleAccessLevel level)
        {
            s_AccessLevel = level;
        }

        public static void Tick(float deltaTime)
        {
            while (k_CommandBuffer.Count > 0)
            {
                ConsoleCommandBase command = k_CommandBuffer.Peek();

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
                    uint parentMessageID = ManagedLog.Error(LogCategory.DEFAULT,
                        "An exception occured while processing the console command buffer. It has been flushed.");
                    ManagedLog.Exception(LogCategory.DEFAULT, e, parentMessageID);
                    k_CommandBuffer.Clear();
                    break;
                }

                k_CommandBuffer.Dequeue();
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
            RegisterCommand(new ShowConsoleCommand());
            RegisterCommand(new HideConsoleCommand());
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
            RegisterCommand(new ConsoleVariablesConsoleCommand());
            RegisterCommand(new WatchConsoleCommand());
            RegisterCommand(new WatchListConsoleCommand());
            RegisterCommand(new ScreenCaptureConsoleCommand());

            // We are going to look at the arguments
            if (CommandLineParser.Arguments.ContainsKey("exec"))
            {
                // We want to make sure that we have everything loading and ready before we actually execute a script
                QueueCommand("scene.wait");
                QueueCommand($"exec {CommandLineParser.Arguments["exec"]}");
            }

            // Register for remote connection from editor response
            PlayerConnection.instance.Register(ConsoleCommandBase.PlayerConnectionGuid,
                args => QueueCommand(Encoding.UTF8.GetString(args.data)));

            ConsoleVariableSettings.UpdateFromFile();
            WatchSettings.UpdateFromFile();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}