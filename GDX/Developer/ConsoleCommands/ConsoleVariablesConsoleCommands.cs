// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class ConsoleVariablesConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            TextGenerator textGenerator = new TextGenerator();
            List<string> sortedVariables = new List<string>(Console.GetVariableNamesCopy());
            sortedVariables.Sort();
            int variableCount = sortedVariables.Count;
            Console.ConsoleAccessLevel maxAccessLevel = Console.GetAccessLevel();
            for (int i = 0; i < variableCount; i++)
            {
                ConsoleVariableBase variable = Console.GetVariable(sortedVariables[i]);
                if (variable.GetAccessLevel() > maxAccessLevel)
                {
                    continue;
                }

                textGenerator.AppendLine(variable.GetName());
                textGenerator.AppendLine($"\t{variable.GetDescription()}");
                textGenerator.AppendLine($"\tCurrent: {variable.GetCurrentValueAsString()}");
                textGenerator.AppendLine($"\tDefault: {variable.GetDefaultValueAsString()}");
            }

            ManagedLog.Info(LogCategory.DEFAULT, textGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "cvars";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Return a list of all registered console variables.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}