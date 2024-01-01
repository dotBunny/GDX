// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class WatchConsoleCommand : ConsoleCommandBase
    {
        string m_Identifier;
        bool m_HasState;
        bool m_DesiredState;

        public override bool Evaluate(float deltaTime)
        {
            if (m_HasState)
            {
                WatchProvider.SetState(WatchProvider.GetWatch(m_Identifier), m_DesiredState);
            }
            else
            {
                WatchProvider.ToggleState(WatchProvider.GetWatch(m_Identifier));
            }
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
            return "watch <identifier> [state]";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Toggles the enabled/disabled state of a watch based on its unique identifier, or sets a specific state. To disable polling globally of watches see the `watches.show` cvar.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                Debug.LogWarning("An identifier is required to find a watch.");
                return null;
            }

            string[] split = context.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!WatchProvider.HasWatch(split[0]))
            {
                Debug.LogWarning($"Unable to find watch '{context}'.");
                return null;
            }

            if (split.Length > 1)
            {
                return new WatchConsoleCommand
                {
                    m_Identifier = split[0], m_DesiredState =  split[1].IsBooleanPositiveValue(), m_HasState = true
                };
            }
            return new WatchConsoleCommand { m_Identifier = split[0] };
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}