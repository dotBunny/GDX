using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Build
{
    public class BuildDefinition
    {
        public readonly bool ClearPreviousBuild = true;
        public readonly string TargetFolder;
        public ScriptingImplementation Backend;
        public string ExecutableName;
        public BuildPlayerOptions PlayerOptions;

        /// <remarks>Only supported in Unity 2021+</remarks>
        public BuildConfiguration.FolderMapping[] Mappings;

        public BuildDefinition(string folderName, string executableName)
        {
            string buildFolder = BuildEnvironment.GetBuildFolder();
            if (buildFolder != null)
            {
                TargetFolder = buildFolder;
            }
            else
            {
                if (!string.IsNullOrEmpty(folderName))
                {
                    TargetFolder = Path.Combine(BuildEnvironment.GetDefaultBuildsFolder(), folderName);
                }
            }

            if (string.IsNullOrEmpty(TargetFolder))
            {
                TargetFolder = Path.Combine(BuildEnvironment.GetDefaultBuildsFolder(), Application.productName);
            }

            ExecutableName = executableName;
            PlayerOptions = new BuildPlayerOptions { scenes = Array.Empty<string>() };
        }

        public BuildDefinition(BuildConfiguration buildConfig)
        {
            if (buildConfig == null)
            {
                throw new FileNotFoundException(
                    "The build configuration was not found in the asset database. Please check your spelling.");
            }

            Debug.Log($"[BuildDefinition] Using BuildConfiguration: {buildConfig.name}");

            string buildFolder = BuildEnvironment.GetBuildFolder();
            if (buildFolder != null)
            {
                TargetFolder = buildFolder;
            }
            else
            {
                string folder = buildConfig.GetResolvedOutputPath();
                if (!string.IsNullOrEmpty(folder))
                {
                    TargetFolder = Path.Combine(BuildEnvironment.GetDefaultBuildsFolder(), folder);
                }
            }

            if (string.IsNullOrEmpty(TargetFolder))
            {
                TargetFolder = Path.Combine(BuildEnvironment.GetDefaultBuildsFolder(), Application.productName);
            }

            ExecutableName = buildConfig.GetResolvedExecutableName();
            PlayerOptions = new BuildPlayerOptions
            {
                scenes = buildConfig.GetResolvedScenePaths(),
                extraScriptingDefines = buildConfig.GetResolvedScriptingDefines()
            };

            Mappings = buildConfig.GetResolvedOverlayFolders();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Build Definition for {TargetFolder}/{ExecutableName}");
            sb.AppendLine("Scenes");
            for (int i = 0; i < PlayerOptions.scenes.Length; i++)
            {
                sb.AppendLine(PlayerOptions.scenes[i]);
            }

            return sb.ToString();
        }
    }
}