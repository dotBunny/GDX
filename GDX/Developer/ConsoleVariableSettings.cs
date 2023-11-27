// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Collections.Generic;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class ConsoleVariableSettings
    {
        static StringKeyDictionary<string> s_ParsedValues = new StringKeyDictionary<string>(50);

        static bool s_HasReadFile;

        public static void SaveToFile()
        {
            int count = Console.GetVariableCount();
            TextGenerator textGenerator = new TextGenerator();
            string[] knownVariables = Console.GetVariableNamesCopy();
            textGenerator.AppendLine("# Generated file of Console Variable (CVAR) settings.");
            textGenerator.AppendLine("# All non supported content will be removed on save.");
            textGenerator.AppendLine("");

            for (int i = 0; i < count; i++)
            {
                ConsoleVariableBase variable = Console.GetVariable(knownVariables[i]);
                if (variable.GetFlags().HasFlags(ConsoleVariableBase.ConsoleVariableFlags.Setting))
                {
                    textGenerator.AppendLine($"{variable.GetName()} {variable.GetCurrentValueAsString()}");
                }
            }

            try
            {
                File.WriteAllTextAsync(GetConsoleVariableSaveFile(), textGenerator.ToString());
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static bool TryGetValue(string name, out string foundValue)
        {
            if (!s_HasReadFile)
            {
                UpdateFromFile();
                s_HasReadFile = true;
            }

            if (s_ParsedValues.ContainsKey(name))
            {
                foundValue = s_ParsedValues[name];
                return true;
            }

            foundValue = null;
            return false;
        }

        public static void UpdateFromFile()
        {
            // Read in the settings from the cvars saved file
            string settingsFile = GetConsoleVariableSaveFile();
            if (!File.Exists(settingsFile))
            {
                return;
            }

            string[] lines = File.ReadAllLines(settingsFile);
            int lineCount = lines.Length;
            for (int i = 0; i < lineCount; i++)
            {
                string line = lines[i].Trim();
                if ( string.IsNullOrEmpty(line) || line[0] == '#') // Skip comments
                {
                    continue;
                }

                // Update settings cache
                string[] split = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (!s_ParsedValues.ContainsKey(split[0]))
                {
                    s_ParsedValues.AddWithExpandCheck(split[0], split[1]);
                }
                else
                {
                    s_ParsedValues[split[0]] = split[1];
                }

                // Update existing initialized
                if (Console.HasVariable(split[0]))
                {
                    Console.GetVariable(split[0]).SetValueFromString(split[1]);
                }
            }
        }

        static string GetConsoleVariableSaveFile()
        {
            return Path.Combine(Platform.GetOutputFolder(), "cvars.gdx");
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}