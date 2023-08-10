using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GDX.Editor.Build
{
    public static class BuildProvider
    {
        /// <summary>
        ///     Fires immediately when the provider has been told to execute a build definition.
        /// </summary>
        public static Action<BuildDefinition> OnExecute;

        public static Action<BuildDefinition, Exception> OnException;
        public static Action<BuildDefinition, BuildReport> OnReport;

        public static void Execute(BuildDefinition definition)
        {
            Debug.Log(definition.ToString());

            OnExecute?.Invoke(definition);

            // Lets do some platform specific work
            switch (definition.PlayerOptions.target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    if (!definition.ExecutableName.EndsWith(".exe"))
                    {
                        definition.ExecutableName = $"{definition.ExecutableName}.exe";
                    }

                    break;
                case BuildTarget.StandaloneOSX:
                    if (!definition.ExecutableName.EndsWith(".app"))
                    {
                        definition.ExecutableName = $"{definition.ExecutableName}.app";
                    }

                    break;
                default:
                    if (!definition.ExecutableName.EndsWith(".bin"))
                    {
                        definition.ExecutableName = $"{definition.ExecutableName}.bin";
                    }

                    break;
            }

            bool buildFailed = false;
            BuildReport buildReport = null;
            bool shouldRestoreScriptingImplementation = false;
            NamedBuildTarget buildTarget = NamedBuildTarget.FromBuildTargetGroup(definition.PlayerOptions.targetGroup);
            ScriptingImplementation previousScriptingImplementation = PlayerSettings.GetScriptingBackend(buildTarget);

            try
            {
                if (!Directory.Exists(definition.TargetFolder))
                {
                    Directory.CreateDirectory(definition.TargetFolder);
                }

                if (previousScriptingImplementation != definition.Backend)
                {
                    Debug.Log($"Switching backend to {definition.Backend}.");
                    PlayerSettings.SetScriptingBackend(buildTarget, definition.Backend);
                    shouldRestoreScriptingImplementation = true;
                }

                // Set locations
                EditorUserBuildSettings.SetBuildLocation(definition.PlayerOptions.target, definition.TargetFolder);

                definition.PlayerOptions.locationPathName =
                    Path.Combine(definition.TargetFolder, definition.ExecutableName);

                // Make sure everything is saved at this point
                AssetDatabase.SaveAssets();


                //// Special case for DOTS stuff that should land eventually a fix?
                //// NOTE: This method currently wipes extra scripting defines, feels like this could be handled better!!!
                //var instance = DotsGlobalSettings.Instance;
                //if (configuration.BuildOptions.subtarget == (int)StandaloneBuildSubtarget.Server)
                //{
                //    var provider = instance.ServerProvider ?? instance.ClientProvider;
                //    configuration.BuildOptions.extraScriptingDefines = provider.GetExtraScriptingDefines();
                //    configuration.BuildOptions.options |= provider.GetExtraBuildOptions();
                //}
                //else
                //{
                //    configuration.BuildOptions.extraScriptingDefines = instance.ClientProvider.GetExtraScriptingDefines();
                //    configuration.BuildOptions.options |= instance.ClientProvider.GetExtraBuildOptions();
                //}

                // Remove previous build entirely please, we do not want any sort of stale data
                if (Directory.Exists(definition.TargetFolder) && definition.ClearPreviousBuild)
                {
                    Debug.Log($"Remove previous build at {definition.TargetFolder}.");
                    Directory.Delete(definition.TargetFolder, true);
                    Directory.CreateDirectory(definition.TargetFolder);
                }

                // Execute classic pipeline
                buildReport = BuildPipeline.BuildPlayer(definition.PlayerOptions);
                OnReport?.Invoke(definition, buildReport);

                buildFailed = buildReport.summary.result != BuildResult.Succeeded;

                BuildEnvironment.SetParameter("Unity.Build.Status", buildFailed ? "1" : "0");
                BuildEnvironment.SetParameter("Unity.Build.TargetFolder", definition.TargetFolder);
                BuildEnvironment.SetParameter("Unity.Build.Location", definition.PlayerOptions.locationPathName);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                buildFailed = true;
                OnException?.Invoke(definition, ex);
                BuildEnvironment.SetParameter("Unity.Build.Status", "1");
            }
            finally
            {
                if (shouldRestoreScriptingImplementation && BuildEnvironment.ShouldRevertChanges())
                {
                    Debug.Log($"Restoring backend to {previousScriptingImplementation}.");
                    PlayerSettings.SetScriptingBackend(buildTarget, previousScriptingImplementation);
                    AssetDatabase.SaveAssets();
                }

                // Exit with error code
                if (Application.isBatchMode && buildFailed)
                {
                    EditorApplication.Exit(1);
                }
            }
        }
    }
}