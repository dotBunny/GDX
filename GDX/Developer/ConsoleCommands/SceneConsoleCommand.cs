// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Logging;
using UnityEngine.SceneManagement;

namespace GDX.Developer.ConsoleCommands
{
#if UNITY_2022_2_OR_NEWER
    public class SceneLoadConsoleCommand : ConsoleCommandBase
    {
        Scene m_TargetScene;
#if UNITY_EDITOR
        string m_EditorScenePath;
#endif

        /// <inheritdoc />
        public override bool Evaluate(float deltaTime)
        {
#if UNITY_EDITOR
            if (m_TargetScene.IsValid())
            {
                SceneManager.LoadSceneAsync(m_TargetScene.buildIndex, LoadSceneMode.Single);
            }
            else if(!string.IsNullOrEmpty(m_EditorScenePath))
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(m_EditorScenePath,
                    UnityEditor.SceneManagement.OpenSceneMode.Single);
            }
#else
            SceneManager.LoadSceneAsync(m_TargetScene.buildIndex, LoadSceneMode.Single);
#endif
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
            // Check our known scenes in the manager.
            // TODO: Build config output of list of scenes for this?
            SceneLoadConsoleCommand command =
                new SceneLoadConsoleCommand { m_TargetScene = SceneManager.GetSceneByName(context) };

            if (command.m_TargetScene.IsValid())
            {
                return command;
            }

            // Lets check if it was a numeric value
            if (int.TryParse(context, out int buildIndex))
            {
                command.m_TargetScene = SceneManager.GetSceneByBuildIndex(buildIndex);
                if (command.m_TargetScene.IsValid())
                {
                    return command;
                }
            }

#if UNITY_EDITOR
            // It has to be perfect, not partials
            string[] possibleGuids = UnityEditor.AssetDatabase.FindAssets($"t:SceneAsset {context}");
            int foundGuids = possibleGuids.Length;
            if (foundGuids > 0)
            {
                List<string> possiblePaths = new List<string>(foundGuids);
                for (int i = 0; i < foundGuids; i++)
                {
                    possiblePaths.Add(UnityEditor.AssetDatabase.GUIDToAssetPath(possibleGuids[i]));
                }
                possiblePaths.Sort();
                command.m_EditorScenePath = possiblePaths[0];
                return command;
            }
#endif


            ManagedLog.Warning(LogCategory.DEFAULT, $"Unable to find scene '{context}'.");
            return null;
        }

        /// <inheritdoc />
        public override string[] GetArgumentAutoCompleteSuggestions(string hint, string[] existingSet = null)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            SimpleList<string> potentialScenes = new SimpleList<string>(sceneCount);
            if (string.IsNullOrEmpty(hint))
            {
                for (int i = 0; i < sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(i);
                    if (scene.IsValid())
                    {
                        potentialScenes.AddUnchecked(scene.name);
                    }
                }
            }
            else
            {
                for (int i = 0; i < sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneByBuildIndex(i);
                    if (scene.IsValid() && scene.name.StartsWith(hint))
                    {
                        potentialScenes.AddUnchecked(scene.name);
                    }
                }
            }

            // TODO: Find in project? editor

            potentialScenes.Compact();
            return potentialScenes.Array;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}