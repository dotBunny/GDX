// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using GDX.Experimental.Logging;
using UnityEngine.SceneManagement;

namespace GDX.Experimental.ConsoleCommands
{
    public class SceneListConsoleCommand : ConsoleCommandBase
    {
        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            TextGenerator textGenerator = new TextGenerator();
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene.IsValid())
                {
                    textGenerator.AppendLine($"{i}\t\t{scene.name}");
                }
            }
            ManagedLog.Info(LogCategory.Default, textGenerator.ToString());
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "scene.list";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Displays a list of known scenes in the build.";
        }
    }
}