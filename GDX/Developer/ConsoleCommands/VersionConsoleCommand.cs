// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Logging;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class VersionConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            ManagedLog.Info(LogCategory.Default, "TODO: Version Information");
            return true;
        }

        /// <inheritdoc />
        public override ConsoleCommandLevel GetAccessLevel()
        {
            return ConsoleCommandLevel.Anonymous;
        }

        public override string GetKeyword()
        {
            return "version";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Display all versions of assemblies and libraries used by the project.";
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}