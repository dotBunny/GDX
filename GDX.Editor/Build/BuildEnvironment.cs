using System;
using System.IO;
using GDX.Developer;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Build
{
    public static class BuildEnvironment
    {
        const string k_BuildConfigArgument = "BUILD-CONFIG";
        const string k_BuildConfigFolderArgument = "BUILD-CONFIG-FOLDER";
        const string k_BuildFolderArgument = "BUILD-FOLDER";
        const string k_BuildSetParamsArgument = "BUILD-PARAMS";
        const string k_BuildNoRevertArgument = "BUILD-NO-REVERT";

        public static string GetBuildFolder()
        {
            return CommandLineParser.Arguments.TryGetValue(k_BuildFolderArgument, out string argument)
                ? Path.GetFullPath(argument)
                : null;
        }

        public static string[] GetBuildConfigFolder()
        {
            return CommandLineParser.Arguments.TryGetValue(k_BuildConfigFolderArgument, out string argument)
                ? argument.Split(',')
                : null;
        }

        public static bool ShouldRevertChanges()
        {
            return CommandLineParser.Arguments.ContainsKey(k_BuildNoRevertArgument);
        }

        public static BuildConfiguration GetBuildConfiguration(string name)
        {
            string[] configGuids = Array.Empty<string>();


            // Command args will override the passed in name to search for, but will fall back if it is not found
            if (CommandLineParser.Arguments.ContainsKey(k_BuildConfigArgument))
            {
                configGuids = AssetDatabase.FindAssets(
                    $"t:BuildConfiguration {CommandLineParser.Arguments[k_BuildConfigArgument]}",
                    GetBuildConfigFolder());
            }

            // If we dont have one at this point use the provided name
            if (configGuids.Length == 0)
            {
                configGuids = AssetDatabase.FindAssets($"t:BuildConfiguration {name}", GetBuildConfigFolder());
            }

            // If we have nothing, let the people know.
            if (configGuids.Length == 0)
            {
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<BuildConfiguration>(AssetDatabase.GUIDToAssetPath(configGuids[0]));
        }

        public static string GetDefaultBuildsFolder()
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Builds"));
        }

        public static void SetParameter(string name, string value)
        {
            if (CommandLineParser.Arguments.ContainsKey(k_BuildSetParamsArgument) && !string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"##teamcity[setParameter name='{name}' value='{value}']");
            }
        }
    }
}