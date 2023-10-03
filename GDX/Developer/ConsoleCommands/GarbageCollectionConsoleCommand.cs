// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class GarbageCollectionConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            Memory.CleanUp();
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
            return "gc";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Force garbage collection be ran for managed memory.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}