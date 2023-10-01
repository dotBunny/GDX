// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2021_3_OR_NEWER
    public class SceneWaitConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            return !SceneExtensions.IsSceneManagerBusy();
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "scene.wait";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Waits for all scene actions to stabilize (loading/unloading), and all enabled.";
        }
    }
#endif // UNITY_2021_3_OR_NEWER
}