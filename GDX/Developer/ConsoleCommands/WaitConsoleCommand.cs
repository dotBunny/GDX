// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2021_3_OR_NEWER
    public class WaitConsoleCommand : ConsoleCommandBase
    {
        float m_WaitTime;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            m_WaitTime -= deltaTime;
            return !(m_WaitTime > 0);
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "wait";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "wait [seconds]";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Waits for a given period of time, till the next update if nothing is provided.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            WaitConsoleCommand command = new WaitConsoleCommand();
            float.TryParse(context, out float result);
            command.m_WaitTime = result;
            return command;
        }
    }
#endif // UNITY_2021_3_OR_NEWER
}