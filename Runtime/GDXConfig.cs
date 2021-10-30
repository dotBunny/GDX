﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace GDX
{
    /// <summary>
    ///     Project-wide configuration which is available at runtime.
    /// </summary>
    /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
    // ReSharper disable once InconsistentNaming
    [HideFromDocFX]
    public class GDXConfig : ScriptableObject
    {
        /// <summary>
        ///     A runtime only instance of <see cref="GDXConfig" />.
        /// </summary>
        private static GDXConfig s_instance = null;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public bool updateProviderCheckForUpdates = true;

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public string developerCommandLineParserArgumentPrefix = "--";

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public string developerCommandLineParserArgumentSplit = "=";

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        public bool developerBuildInfoAssemblyDefinition = true;

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public bool developerBuildInfoEnabled;

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public string developerBuildInfoPath = "Generated/Build/BuildInfo.cs";

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public string developerBuildInfoNamespace = "Generated.Build";

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public string developerBuildInfoBuildNumberArgument = "BUILD";

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public string developerBuildInfoBuildDescriptionArgument = "BUILD_DESC";

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public string developerBuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public string developerBuildInfoBuildTaskArgument = "BUILD_TASK";

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public string developerBuildInfoBuildStreamArgument = "BUILD_STREAM";

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public bool environmentScriptingDefineSymbol;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        public Trace.TraceLevel traceDevelopmentLevels = Trace.TraceLevel.Log |
                                                         Trace.TraceLevel.Warning |
                                                         Trace.TraceLevel.Error |
                                                         Trace.TraceLevel.Exception |
                                                         Trace.TraceLevel.Assertion |
                                                         Trace.TraceLevel.Fatal;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        public Trace.TraceLevel traceDebugLevels = Trace.TraceLevel.Warning |
                                                   Trace.TraceLevel.Assertion |
                                                   Trace.TraceLevel.Error |
                                                   Trace.TraceLevel.Exception;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        public Trace.TraceLevel traceReleaseLevels = Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        public bool traceDevelopmentOutputToUnityConsole = true;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        public bool traceDebugOutputToUnityConsole = true;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        public bool localizationSetDefaultCulture = true;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        public Localization.Language localizationDefaultCulture = Localization.Language.English;

        /// <summary>
        ///     Get a loaded instance of the <see cref="GDXConfig" /> from resources.
        /// </summary>
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        /// <returns>A instance of <see cref="GDXConfig" />.</returns>
        public static GDXConfig Get()
        {
#if UNITY_EDITOR
            if (s_instance != null)
            {
                return s_instance;
            }

            // Load from disk
            s_instance = AssetDatabase.LoadAssetAtPath<GDXConfig>("Assets/Resources/GDX/GDXConfig.asset");
            if (s_instance != null)
            {
                return s_instance;
            }

            // Looks like we need to make one
            s_instance = CreateInstance<GDXConfig>();

            // Ensure the folder structure is in place before we manually make the asset
            Platform.EnsureFileFolderHierarchyExists(
                System.IO.Path.Combine(Application.dataPath, "Resources/GDX/GDXConfig.asset"));

            // Create and save the asset
            AssetDatabase.CreateAsset(s_instance, "Assets/Resources/GDX/GDXConfig.asset");

            // Send it back!
            return s_instance;
#else
            if (s_instance == null)
            {
                s_instance = Resources.Load<GDXConfig>("GDX/GDXConfig");
            }
            return s_instance;
#endif
        }
    }
}