// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
    public class WatchesConsoleCommand : ConsoleCommandBase
    {
        public override bool Evaluate(float deltaTime)
        {
			TextGenerator textGenerator = new TextGenerator();

            ManagedLog.Info(LogCategory.DEFAULT, textGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "watches";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Return a list of all registered watches.";
        }
    }
}