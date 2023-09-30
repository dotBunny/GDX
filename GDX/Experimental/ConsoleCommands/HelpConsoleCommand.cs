// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Developer;
using GDX.Experimental.Logging;

namespace GDX.Experimental.ConsoleCommands
{
    public class HelpConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            TextGenerator textGenerator = new();
            List<string> sortedCommands = new(DeveloperConsole.GetCommandKeywordsCopy());
            sortedCommands.Sort();
            int commandCount = sortedCommands.Count;
            ConsoleCommandLevel maxAccessLevel = DeveloperConsole.GetAccessLevel();
            for (int i = 0; i < commandCount; i++)
            {
                ConsoleCommandBase command = DeveloperConsole.GetCommand(sortedCommands[i]);

                // Dont show commands that are not usable.
                if (command.GetAccessLevel() > maxAccessLevel) continue;

                textGenerator.AppendLine(command.GetHelpUsage());
                textGenerator.Append($"\t{command.GetHelpMessage()}");
                if (command.IsEditorOnly())
                {
                    textGenerator.AppendLine("(Editor Only)");
                }
                else
                {
                    textGenerator.AppendLine("");
                }


            }

            ManagedLog.Info(LogCategory.Default, textGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override ConsoleCommandLevel GetAccessLevel()
        {
            return ConsoleCommandLevel.Anonymous;
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
}