// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class WatchConsoleCommand : ConsoleCommandBase
    {
        string m_Identifier;

        public override bool Evaluate(float deltaTime)
        {
            WatchBase watch = WatchProvider.GetWatch(m_Identifier);
            if (watch == null) return false;

            WatchProvider.ToggleState(watch);
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "watch";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "watch [identifier]";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Toggles the enabled/disabled state of a watch based on its unique identifier.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                ManagedLog.Warning(LogCategory.DEFAULT, $"An identifier is required to find a watch.");
            }

            return new WatchConsoleCommand { m_Identifier = context };
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}