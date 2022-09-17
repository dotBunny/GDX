// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

namespace GDX
{
    /// <summary>
    ///     Project-wide configuration configurable for runtime override.
    /// </summary>
    /// <remarks>
    ///     Field order matters to the Config Generator.
    /// </remarks>
    public static class Config
    {
        /// <summary>
        ///     A reference number to what version the config was generated based off of.
        /// </summary>
        public const int FormatVersion = 1;

        /// <summary>
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        [OriginalValue(k_ConfigOutputPath)]
        public static string ConfigOutputPath = k_ConfigOutputPath;
        const string k_ConfigOutputPath = "Generated/GDX.generated.cs";

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        [OriginalValue(k_BuildInfo)]
        public static bool BuildInfo = k_BuildInfo;
        const bool k_BuildInfo = false;

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        [OriginalValue(k_BuildInfoAssemblyDefinition)]
        public static bool BuildInfoAssemblyDefinition = k_BuildInfoAssemblyDefinition;
        const bool k_BuildInfoAssemblyDefinition = true;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildChangelistArgument)]
        public static string BuildInfoBuildChangelistArgument = k_BuildInfoBuildChangelistArgument;
        const string k_BuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildDescriptionArgument)]
        public static string BuildInfoBuildDescriptionArgument = k_BuildInfoBuildDescriptionArgument;
        const string k_BuildInfoBuildDescriptionArgument = "BUILD_DESC";

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildNumberArgument)]
        public static string BuildInfoBuildNumberArgument = k_BuildInfoBuildNumberArgument;
        const string k_BuildInfoBuildNumberArgument = "BUILD";

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildStreamArgument)]
        public static string BuildInfoBuildStreamArgument = k_BuildInfoBuildStreamArgument;
        const string k_BuildInfoBuildStreamArgument = "BUILD_STREAM";

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildTaskArgument)]
        public static string BuildInfoBuildTaskArgument = k_BuildInfoBuildTaskArgument;
        const string k_BuildInfoBuildTaskArgument = "BUILD_TASK";

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        [OriginalValue(k_BuildInfoNamespace)]
        public static string BuildInfoNamespace = k_BuildInfoNamespace;
        const string k_BuildInfoNamespace = "Generated.Build";

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        [OriginalValue(k_BuildInfoOutputPath)]
        public static string BuildInfoOutputPath = k_BuildInfoOutputPath;
        const string k_BuildInfoOutputPath = "Generated/Build/BuildInfo.cs";

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        [OriginalValue(k_CommandLineParserArgumentPrefix)]
        public static string CommandLineParserArgumentPrefix =
            k_CommandLineParserArgumentPrefix;
        const string k_CommandLineParserArgumentPrefix = "--";

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        [OriginalValue(k_CommandLineParserArgumentSplit)]
        public static string CommandLineParserArgumentSplit = k_CommandLineParserArgumentSplit;
        const string k_CommandLineParserArgumentSplit = "=";

        /// <summary>
        ///     Should the Editor Task Director tick the Task Director.
        /// </summary>
        [OriginalValue(k_EditorTaskDirectorSystem)]
        public static bool EditorTaskDirectorSystem = k_EditorTaskDirectorSystem;
        const bool k_EditorTaskDirectorSystem = true;

        /// <summary>
        ///     How often should the editor task director tick trigger the task director to tick?
        /// </summary>
        [OriginalValue(k_EditorTaskDirectorSystemTickRate)]
        public static double EditorTaskDirectorSystemTickRate = k_EditorTaskDirectorSystemTickRate;
        const double k_EditorTaskDirectorSystemTickRate = 0.5d;

        /// <summary>
        ///     Should GDX make sure that it's shaders are always included in builds.
        /// </summary>
        [OriginalValue(k_EnvironmentAlwaysIncludeShaders)]
        public static bool EnvironmentAlwaysIncludeShaders = k_EnvironmentAlwaysIncludeShaders;
        const bool k_EnvironmentAlwaysIncludeShaders = true;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        [OriginalValue(k_EnvironmentScriptingDefineSymbol)]
        public static bool EnvironmentScriptingDefineSymbol = k_EnvironmentScriptingDefineSymbol;
        const bool k_EnvironmentScriptingDefineSymbol = false;

        /// <summary>
        ///     Should the GDX tools menu be added in the editor?
        /// </summary>
        [OriginalValue(k_EnvironmentToolsMenu)]
        public static bool EnvironmentToolsMenu = k_EnvironmentToolsMenu;
        const bool k_EnvironmentToolsMenu = false;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        [OriginalValue(k_LocalizationDefaultCulture)]
        public static Localization.Language LocalizationDefaultCulture = k_LocalizationDefaultCulture;
        const Localization.Language k_LocalizationDefaultCulture = Localization.Language.English;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        [OriginalValue(k_LocalizationSetDefaultCulture)]
        public static bool LocalizationSetDefaultCulture = k_LocalizationSetDefaultCulture;
        const bool k_LocalizationSetDefaultCulture = true;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        [OriginalValue(k_PlatformAutomationFolder)]
        public static string PlatformAutomationFolder = k_PlatformAutomationFolder;
        const string k_PlatformAutomationFolder = "GDX_Automation";

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        [OriginalValue(k_PlatformCacheFolder)]
        public static string PlatformCacheFolder = k_PlatformCacheFolder;
        const string k_PlatformCacheFolder = "GDX_Cache";

        /// <summary>
        ///     Should the Task Director System be added to the player loop during playmode.
        /// </summary>
        [OriginalValue(k_TaskDirectorSystem)]
        public static bool TaskDirectorSystem = k_TaskDirectorSystem;
        const bool k_TaskDirectorSystem = false;

        /// <summary>
        ///     How often should the task director tick in playmode?
        /// </summary>
        [OriginalValue(k_TaskDirectorSystemTickRate)]
        public static float TaskDirectorSystemTickRate = k_TaskDirectorSystemTickRate;
        const float k_TaskDirectorSystemTickRate = 0.1f;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        [OriginalValue(k_TraceDebugLevels)]
        public static Trace.TraceLevel TraceDebugLevels = k_TraceDebugLevels;
        const Trace.TraceLevel k_TraceDebugLevels = Trace.TraceLevel.Warning |
                                                           Trace.TraceLevel.Assertion |
                                                           Trace.TraceLevel.Error |
                                                           Trace.TraceLevel.Exception;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        [OriginalValue(k_TraceDebugOutputToUnityConsole)]
        public static bool TraceDebugOutputToUnityConsole = k_TraceDebugOutputToUnityConsole;
        const bool k_TraceDebugOutputToUnityConsole = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        [OriginalValue(k_TraceDevelopmentLevels)]
        public static Trace.TraceLevel TraceDevelopmentLevels = k_TraceDevelopmentLevels;
        const Trace.TraceLevel k_TraceDevelopmentLevels = Trace.TraceLevel.Log |
                                                                 Trace.TraceLevel.Warning |
                                                                 Trace.TraceLevel.Error |
                                                                 Trace.TraceLevel.Exception |
                                                                 Trace.TraceLevel.Assertion |
                                                                 Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        [OriginalValue(k_TraceDevelopmentOutputToUnityConsole)]
        public static bool TraceDevelopmentOutputToUnityConsole = k_TraceDevelopmentOutputToUnityConsole;
        const bool k_TraceDevelopmentOutputToUnityConsole = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        [OriginalValue(k_TraceReleaseLevels)]
        public static Trace.TraceLevel TraceReleaseLevels = k_TraceReleaseLevels;
        const Trace.TraceLevel k_TraceReleaseLevels = Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        [OriginalValue(k_UpdateProviderCheckForUpdates)]
        public static bool UpdateProviderCheckForUpdates = true;
        const bool k_UpdateProviderCheckForUpdates = true;
    }
}