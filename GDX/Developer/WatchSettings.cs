// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Collections.Generic;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class WatchSettings
    {
        static StringKeyDictionary<bool> s_ParsedState = new StringKeyDictionary<bool>(50);

        static bool s_HasReadFile;

        public static void SaveToFile()
        {
            int count = WatchProvider.GetTotalCount();
            TextGenerator textGenerator = new TextGenerator();
            WatchProvider.WatchList watches = WatchProvider.GetWatchList();
            textGenerator.AppendLine("# Generated file of Watches states.");
            textGenerator.AppendLine("# All non supported content will be removed on save.");
            textGenerator.AppendLine("");

            for (int i = 0; i < count; i++)
            {
                textGenerator.AppendLine(watches.IsActive[i]
                    ? $"{watches.Identfiers[i]} 1"
                    : $"{watches.Identfiers[i]} 0");
            }

            try
            {
                File.WriteAllTextAsync(GetWatchesSaveFile(), textGenerator.ToString());
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static bool TryGetValue(string name, out bool foundValue)
        {
            if (!s_HasReadFile)
            {
                UpdateFromFile();
                s_HasReadFile = true;
            }

            if (s_ParsedState.ContainsKey(name))
            {
                foundValue = s_ParsedState[name];
                return true;
            }

            foundValue = false;
            return false;
        }

        public static void UpdateFromFile()
        {
            // Read in the settings from the cvars saved file
            string settingsFile = GetWatchesSaveFile();
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
                bool value = split[1].IsBooleanPositiveValue();
                if (!s_ParsedState.ContainsKey(split[0]))
                {
                    s_ParsedState.AddWithExpandCheck(split[0], value);
                }
                else
                {
                    s_ParsedState[split[0]] = value;
                }

                WatchBase watch = WatchProvider.GetWatch(split[0]);
                if (watch != null)
                {
                    WatchProvider.SetState(watch, value);
                }
            }
        }

        static string GetWatchesSaveFile()
        {
            return Path.Combine(Platform.GetOutputFolder(), "watches.gdx");
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}