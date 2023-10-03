// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class HelpConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            TextGenerator textGenerator = new TextGenerator();
            List<string> sortedCommands = new List<string>(Console.GetCommandKeywordsCopy());
            sortedCommands.Sort();
            int commandCount = sortedCommands.Count;
            Console.ConsoleAccessLevel maxAccessLevel = Console.GetAccessLevel();
            for (int i = 0; i < commandCount; i++)
            {
                ConsoleCommandBase command = Console.GetCommand(sortedCommands[i]);

                // Dont show commands that are not usable.
                if (command.GetAccessLevel() > maxAccessLevel)
                {
                    continue;
                }

                textGenerator.AppendLine(command.GetHelpUsage());
                textGenerator.Append($"\t{command.GetHelpMessage()}");
                if (command.IsEditorOnly())
                {
                    textGenerator.AppendLine("(Editor Only)");
                }
                else
                {
                    textGenerator.AppendLine();
                }
            }

            ManagedLog.Info(LogCategory.Default, textGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override Console.ConsoleAccessLevel GetAccessLevel()
        {
            return Console.ConsoleAccessLevel.Anonymous;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "help";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Display a list of all known commands and their usage.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}