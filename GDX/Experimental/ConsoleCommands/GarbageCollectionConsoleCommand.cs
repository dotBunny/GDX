// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using GDX.Experimental.Logging;

namespace GDX.Experimental.ConsoleCommands
{
    public class GarbageCollectionConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            Memory.CleanUp();
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
            return "gc";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Force garbage collection be ran for managed memory.";
        }
    }
}