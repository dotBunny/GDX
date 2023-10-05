// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.ConsoleCommands;
using GDX.Developer.ConsoleVariables;
using GDX.Logging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    // TODO: Hint system?
    // TOOD: CVars
    // TODO: Watches

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
        static StringKeyDictionary<string> s_ConsoleVariablesSettings = new StringKeyDictionary<string>(50);
        // We keep a list of the commands handy because of how frequently we need to iterate over them
        static readonly List<string> k_KnownCommandsList = new List<string>(50);
        static readonly List<string> k_KnownVariablesList = new List<string>(50);
        static bool s_HasReadSettingsFile;

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

        public static void RegisterVariable(ConsoleVariableBase variable)
        {
            string name = variable.GetName();
            if (!s_ConsoleVariables.ContainsKey(name))
            {
                s_ConsoleVariables.AddWithExpandCheck(name, variable);
                k_KnownVariablesList.Add(name);
            }
        }

        public static void UnregisterVariable(ConsoleVariableBase variable)
        {
            UnregisterVariable(variable.GetName());
        }

        public static void UnregisterVariable(string name)
        {
            k_KnownVariablesList.Remove(name);
            s_ConsoleVariables.TryRemove(name);
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

        public static string GetAutoComplete(string hint, int offset)
        {
            // TODO: Auto?
            return null;
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
                    ManagedLog.Warning(LogCategory.DEFAULT,
                        $"Invalid Command: {split[0]} - A higher access level is required.");
                    return false;
                }

                ConsoleCommandBase workUnit = command.GetInstance(split.Length > 1 ? split[1].Trim() : null);
                if (workUnit == null)
                {
                    ManagedLog.Warning(LogCategory.DEFAULT,
                        $"Invalid Command: {split[0]} - Unable to generate work unit.");
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
                        ManagedLog.Warning(LogCategory.DEFAULT,
                            $"Unable to set {split[0]} - A higher access level is required.");
                        return false;
                    }

                    variable.SetValueFromString(split[1].Trim());
                    ManagedLog.Info(LogCategory.DEFAULT,
                        $"{variable.GetName()} is now '{variable.GetCurrentValueAsString()}'");
                }
                else // Echo
                {
                    ManagedLog.Info(LogCategory.DEFAULT,
                        $"{variable.GetName()} is '{variable.GetCurrentValueAsString()}' [{variable.GetDefaultValueAsString()}])");
                }

                return true;
            }

            ManagedLog.Warning(LogCategory.DEFAULT, $"Invalid Command: {split[0]}");
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

        static string GetConsoleVariableSaveFile()
        {
            return System.IO.Path.Combine(Platform.GetOutputFolder(), "settings.gdx");
        }

        public static void SaveConsoleVariables()
        {
            int count = k_KnownVariablesList.Count;
            GDX.Developer.TextGenerator textGenerator = new TextGenerator();
            for (int i = 0; i < count; i++)
            {
                ConsoleVariableBase variable = s_ConsoleVariables[k_KnownVariablesList[i]];
                if (variable.GetFlags().HasFlags(ConsoleVariableBase.ConsoleVariableFlags.Setting))
                {

                }
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
            RegisterCommand(new ConsoleVariablesConsoleCommands());

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

            UpdateFromSettingsFile();
        }



        public static bool TryGetVariableSettingsValue(string name, out string foundValue)
        {
            if (!s_HasReadSettingsFile)
            {
                UpdateFromSettingsFile();
                s_HasReadSettingsFile = true;
            }

            if (s_ConsoleVariablesSettings.ContainsKey(name))
            {
                foundValue = s_ConsoleVariablesSettings[name];
                return true;
            }

            foundValue = null;
            return false;
        }
        static void UpdateFromSettingsFile()
        {
            // Read in the settings from the cvars saved file
            string settingsFile = GetConsoleVariableSaveFile();
            if (File.Exists(settingsFile))
            {
                string[] lines = File.ReadAllLines(settingsFile);
                int lineCount = lines.Length;
                for (int i = 0; i < lineCount; i++)
                {
                    string line = lines[i].Trim();
                    if (line[0] == '#') // Skip comments
                    {
                        continue;
                    }

                    string[] split = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    if (!s_ConsoleVariablesSettings.ContainsKey(split[0]))
                    {
                        s_ConsoleVariablesSettings.AddWithExpandCheck(split[0], split[1]);
                    }
                    else
                    {
                        s_ConsoleVariablesSettings[split[0]] = split[1];
                    }
                }
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}