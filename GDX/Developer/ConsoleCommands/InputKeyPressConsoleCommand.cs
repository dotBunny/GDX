// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class InputKeyPressConsoleCommand : ConsoleCommandBase
    {
        InputProxy.KeyCode m_KeyCode;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            InputProxy.KeyPress(m_KeyCode);
            return true;
        }

        /// <inheritdoc />
        public override ConsoleCommandLevel GetAccessLevel()
        {
            return ConsoleCommandLevel.Developer;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "input.keypress";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "input.keypress <name>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return
                "Synthesize the down and up event for a designated keyboard key. See https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes for valid names.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            InputKeyPressConsoleCommand command =
                new InputKeyPressConsoleCommand { m_KeyCode = InputProxy.GetKeyCode(context) };
            if (command.m_KeyCode != InputProxy.KeyCode.Invalid)
            {
                return command;
            }

            ManagedLog.Warning(LogCategory.Default, $"Unable to parse key from '{context}'.");
            return null;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}