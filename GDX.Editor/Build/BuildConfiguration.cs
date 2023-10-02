using System;
using System.Collections.Generic;
using System.IO;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Build
{
    [CreateAssetMenu(fileName = "BC_BuildConfig", menuName = "GDX/Build Configuration")]
    public class BuildConfiguration : ScriptableObject
    {
        [Serializable]
        public class FolderMapping
        {
            public string ProjectRelativePath;
            public string BuildRelativePath;
        }

        public string GetResolvedExecutableName()
        {
            List<BuildConfiguration> visitedConfigurations = new List<BuildConfiguration>();
            if (string.IsNullOrEmpty(ExecutableName))
            {
                string executableName = GetExecutableNameWithDependencies(visitedConfigurations);
                visitedConfigurations.Clear();
                return executableName;
            }

            return ExecutableName;
        }

        public string[] GetResolvedScenePaths()
        {
            List<BuildConfiguration> visitedConfigurations = new List<BuildConfiguration>();
            string[] scenes = GetScenesWithDependencies(visitedConfigurations);
            visitedConfigurations.Clear();
            return scenes;
        }

        public FolderMapping[] GetResolvedOverlayFolders()
        {
            List<BuildConfiguration> visitedConfigurations = new List<BuildConfiguration>();
            FolderMapping[] mappings = GetOverlayFoldersWithDependencies(visitedConfigurations);
            visitedConfigurations.Clear();
            return mappings;
        }

        public string GetResolvedOutputPath()
        {
            List<BuildConfiguration> visitedConfigurations = new List<BuildConfiguration>();
            if (string.IsNullOrEmpty(OutputFolder))
            {
                string outputFolder = GetOutputFolderWithDependencies(visitedConfigurations);
                visitedConfigurations.Clear();
                return outputFolder;
            }

            return OutputFolder;
        }

        public string[] GetResolvedScriptingDefines()
        {
            List<BuildConfiguration> visitedConfigurations = new List<BuildConfiguration>();
            string[] defines = GetScriptingDefinesWithDependencies(visitedConfigurations);
            visitedConfigurations.Clear();
            return defines;
        }

        string[] GetScenesFromAssets()
        {
            int sceneCount = Scenes.Length;
            List<string> paths = new List<string>(sceneCount);
            for (int i = 0; i < sceneCount; i++)
            {
                SceneAsset asset = Scenes[i];
                if (asset != null)
                {
                    string path = AssetDatabase.GetAssetPath(asset);
                    if (!paths.Contains(path))
                    {
                        paths.Add(path);
                    }
                }
            }

            return paths.ToArray();
        }

        string[] GetScenesFromFolders()
        {
            // Validate the set scene folders
            int folderCount = SceneFolders.Length;
            SimpleList<string> validSceneFolders = new SimpleList<string>(folderCount);
            for (int i = 0; i < folderCount; i++)
            {
                string folder = SceneFolders[i];
                if (AssetDatabase.IsValidFolder(folder))
                {
                    validSceneFolders.AddUnchecked(folder);
                }
            }

            // Grab scenes in the folder order, these scenes are sorted based on order and folder names under.
            int validFolderCount = validSceneFolders.Count;
            List<string> paths = new List<string>();
            for (int i = 0; i < validFolderCount; i++)
            {
                // The problem is that this gets all scenes, even "subscenes" which are treated as normal scenes in this case
                // thus we need to do some creative filtering down the line
                string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { validSceneFolders.Array[i] });
                int guidsCount = guids.Length;
                List<string> folderPaths = new List<string>();
                for (int j = 0; j < guidsCount; j++)
                {
                    string actualPath = AssetDatabase.GUIDToAssetPath(guids[j]);
                    if (!string.IsNullOrEmpty(actualPath))
                    {
                        folderPaths.Add(actualPath);
                    }
                }

                // Sort by folder entry (dont sort the whole thing)
                folderPaths.Sort();

                if (IgnoreSubsceneFolders)
                {
                    // Typical folder structure naming
                    int folderPathsCount = folderPaths.Count;
                    for (int j = folderPathsCount - 1; j >= 0; j--)
                    {
                        string workingPath = folderPaths[j];
                        int workingPathLength = workingPath.Length;
                        string fileName = Path.GetFileName(workingPath);
                        int fileNameLength = fileName.Length;

                        if (folderPaths.Contains(
                                $"{workingPath.Substring(0, workingPathLength - fileNameLength - 1)}.unity"))
                        {
                            folderPaths.RemoveAt(j);
                        }
                    }
                }

                paths.AddRange(folderPaths.ToArray());
                folderPaths.Clear();
            }

            return paths.ToArray();
        }

        string[] GetScenesWithDependencies(List<BuildConfiguration> visitedConfigurations)
        {
            List<string> returnedScenes = new List<string>();

            int parentCount = Dependencies.Length;
            for (int i = 0; i < parentCount; i++)
            {
                BuildConfiguration dependency = Dependencies[i];
                if (!visitedConfigurations.Contains(dependency))
                {
                    visitedConfigurations.Add(dependency);
                    returnedScenes.AddUniqueRange(dependency.GetScenesWithDependencies(visitedConfigurations));
                }
            }

            returnedScenes.AddUniqueRange(GetScenesFromAssets());
            returnedScenes.AddUniqueRange(GetScenesFromFolders());


            return returnedScenes.ToArray();
        }

        FolderMapping[] GetOverlayFoldersWithDependencies(List<BuildConfiguration> visitedConfigurations)
        {
            List<FolderMapping> returnedScenes = new List<FolderMapping>();

            int parentCount = Dependencies.Length;
            for (int i = 0; i < parentCount; i++)
            {
                BuildConfiguration dependency = Dependencies[i];
                if (!visitedConfigurations.Contains(dependency))
                {
                    visitedConfigurations.Add(dependency);
                    returnedScenes.AddUniqueRange(dependency.GetOverlayFoldersWithDependencies(visitedConfigurations));
                }
            }

            if (OverlayFolders != null && OverlayFolders.Length > 0)
            {
                returnedScenes.AddUniqueRange(OverlayFolders);
            }

            // Clean up using dirty method
            List<string> found = new List<string>();
            for (int i = returnedScenes.Count - 1; i >= 0; i--)
            {
                if (found.Contains(returnedScenes[i].ProjectRelativePath))
                {
                    returnedScenes.RemoveAt(i);
                    continue;
                }
                found.Add(returnedScenes[i].ProjectRelativePath);
            }

            return returnedScenes.ToArray();
        }

        string[] GetScriptingDefinesWithDependencies(List<BuildConfiguration> visitedConfigurations)
        {
            List<string> returnedDefines = new List<string>();

            int parentCount = Dependencies.Length;
            for (int i = 0; i < parentCount; i++)
            {
                BuildConfiguration dependency = Dependencies[i];
                if (!visitedConfigurations.Contains(dependency))
                {
                    visitedConfigurations.Add(dependency);
                    returnedDefines.AddUniqueRange(
                        dependency.GetScriptingDefinesWithDependencies(visitedConfigurations));
                }
            }

            returnedDefines.AddUniqueRange(AdditionalDefines);

            return returnedDefines.ToArray();
        }

        string GetOutputFolderWithDependencies(List<BuildConfiguration> visitedConfigurations)
        {
            string returnedFolder = null;

            int parentCount = Dependencies.Length;
            for (int i = parentCount - 1; i >= 0; i--)
            {
                BuildConfiguration dependency = Dependencies[i];
                if (!visitedConfigurations.Contains(dependency))
                {
                    visitedConfigurations.Add(dependency);
                    string dependencyOutputFolder = dependency.GetOutputFolderWithDependencies(visitedConfigurations);
                    if (!string.IsNullOrEmpty(dependencyOutputFolder))
                    {
                        returnedFolder = dependencyOutputFolder;
                        break;
                    }
                }
            }

            if (returnedFolder == null && !string.IsNullOrEmpty(OutputFolder))
            {
                returnedFolder = OutputFolder;
            }

            return returnedFolder;
        }

        string GetExecutableNameWithDependencies(List<BuildConfiguration> visitedConfigurations)
        {
            string returnedExecutableName = null;

            int parentCount = Dependencies.Length;
            for (int i = parentCount - 1; i >= 0; i--)
            {
                BuildConfiguration dependency = Dependencies[i];
                if (!visitedConfigurations.Contains(dependency))
                {
                    visitedConfigurations.Add(dependency);
                    string dependencyExecutableName =
                        dependency.GetExecutableNameWithDependencies(visitedConfigurations);
                    if (!string.IsNullOrEmpty(dependencyExecutableName))
                    {
                        returnedExecutableName = dependencyExecutableName;
                        break;
                    }
                }
            }

            if (returnedExecutableName == null && !string.IsNullOrEmpty(ExecutableName))
            {
                returnedExecutableName = ExecutableName;
            }

            return returnedExecutableName;
        }
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        [Header("Based On")] public BuildConfiguration[] Dependencies;

        [Header("Output")] public string OutputFolder;

        public string ExecutableName;

        [Header("Content")] public SceneAsset[] Scenes;

        public string[] SceneFolders;

        [Tooltip("This will attempt to ignore anything under a folder which matches a previously added Scene.")]
        public bool IgnoreSubsceneFolders = true;

        [Header("Compilation")] public string[] AdditionalDefines;

        [Tooltip("Overlay content relative to Application.dataPath, on top of the build executable.")]
        [Header("Extras")] public FolderMapping[] OverlayFolders;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
    }
}