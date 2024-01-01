// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class WatchAllConsoleCommand : ConsoleCommandBase
    {
        bool m_DesiredState;

        public override bool Evaluate(float deltaTime)
        {
            if (m_DesiredState)
            {
                WatchProvider.SetAllEnabled();
            }
            else
            {
                WatchProvider.SetAllDisabled();
            }
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "watch.all";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "watch.all <state>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Sets the state of all known watches.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                UnityEngine.Debug.LogWarning($"A state must be given to apply to all watches.");
                return null;
            }

            return new WatchAllConsoleCommand()
            {
                m_DesiredState =  context.IsBooleanPositiveValue()
            };
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}