// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class ConsoleAutoCompleteProvider
    {
        static int s_AutoCompleteOffset;
        static string[] s_AutoCompleteSuggestions;
        static string s_CurrentSuggestion;

        public static string GetCurrentSuggestion()
        {
            return s_CurrentSuggestion;
        }

        public static bool UpdateSuggestion(string inputString)
        {
            string[] split = inputString.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (split == null || split.Length == 0)
            {
                Reset();
                return false;
            }

            // Check if we have a command
            ConsoleCommandBase command = Console.GetCommand(split[0]);
            if (command != null)
            {
                s_AutoCompleteSuggestions =
                    command.GetArgumentAutoCompleteSuggestions(split.Length == 2 ? split[1] : null,
                        s_AutoCompleteSuggestions);

                if (s_AutoCompleteSuggestions == null || s_AutoCompleteSuggestions.Length == 0)
                {
                    Reset();
                }
                else
                {
                    s_CurrentSuggestion = $" {s_AutoCompleteSuggestions[s_AutoCompleteOffset]}";
                    s_AutoCompleteOffset++;
                    if (s_AutoCompleteOffset >= s_AutoCompleteSuggestions.Length)
                    {
                        s_AutoCompleteOffset = 0;
                    }
                }

                return true;
            }

            // Variable Search
            ConsoleVariableBase variable = Console.GetVariable(split[0]);
            if (variable != null)
            {
                s_AutoCompleteSuggestions =
                    variable.GetArgumentAutoCompleteSuggestions(split.Length == 2 ? split[1] : null,
                        s_AutoCompleteSuggestions);

                if (s_AutoCompleteSuggestions == null || s_AutoCompleteSuggestions.Length == 0)
                {
                    Reset();
                }
                else
                {
                    s_CurrentSuggestion = $" {s_AutoCompleteSuggestions[s_AutoCompleteOffset]}";
                    s_AutoCompleteOffset++;
                    if (s_AutoCompleteOffset >= s_AutoCompleteSuggestions.Length)
                    {
                        s_AutoCompleteOffset = 0;
                    }
                }

                return true;
            }

            // Wide Search
            s_AutoCompleteSuggestions = Console.GetCommandAutoCompleteSuggestions(split[0], s_AutoCompleteSuggestions);
            if (s_AutoCompleteSuggestions == null || s_AutoCompleteSuggestions.Length == 0)
            {
                s_CurrentSuggestion = string.Empty;
                s_AutoCompleteOffset = 0;
                return false;
            }

            // TODO : if you have a bad command and a space and a var it will get here and implode on auto
            s_CurrentSuggestion = s_AutoCompleteSuggestions[s_AutoCompleteOffset].Substring(inputString.Length);
            s_AutoCompleteOffset++;
            if (s_AutoCompleteOffset >= s_AutoCompleteSuggestions.Length)
            {
                s_AutoCompleteOffset = 0;
            }

            return true;
        }

        public static void Reset()
        {
            s_AutoCompleteOffset = 0;
            s_AutoCompleteSuggestions = null;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}