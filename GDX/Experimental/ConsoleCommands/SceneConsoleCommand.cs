// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX;
using GDX.Experimental.Logging;
using UnityEngine.SceneManagement;

namespace GDX.Experimental.ConsoleCommands
{
    public class SceneLoadConsoleCommand : ConsoleCommandBase
    {
        Scene m_TargetScene;

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
            SceneManager.LoadSceneAsync(m_TargetScene.buildIndex);
            return true;
        }

        /// <inheritdoc />
        public override string GetKeyword()
        {
            return "scene";
        }

        /// <inheritdoc />
        public override string GetHelpUsage()
        {
            return "scene <scene name> OR <scene index>";
        }

        /// <inheritdoc />
        public override string GetHelpMessage()
        {
            return "Loads a given scene asynchronously.";
        }

        /// <inheritdoc />
        public override ConsoleCommandBase GetInstance(string context)
        {
            SceneLoadConsoleCommand command =
                new SceneLoadConsoleCommand { m_TargetScene = SceneManager.GetSceneByName(context) };

            if (command.m_TargetScene.IsValid())
            {
                return command;
            }

            if (int.TryParse(context, out int buildIndex))
            {
                command.m_TargetScene = SceneManager.GetSceneByBuildIndex(buildIndex);
                if (command.m_TargetScene.IsValid())
                {
                    return command;
                }
            }

            ManagedLog.Warning(LogCategory.Default, $"Unable to find scene '{context}'.");
            return null;
        }
    }
}