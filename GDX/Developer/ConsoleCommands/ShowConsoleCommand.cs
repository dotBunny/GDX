// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class ShowConsoleCommand : ConsoleCommandBase
    {
        public static Action ShowConsole;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            ShowConsole?.Invoke();
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
            return "show";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Shows the developer console.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}