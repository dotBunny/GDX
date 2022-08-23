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
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        [OriginalValue(k_ConfigOutputPathDefault)]
        public static string ConfigOutputPath = k_ConfigOutputPathDefault;
        const string k_ConfigOutputPathDefault = "Generated/GDX.generated.cs";

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoAssemblyDefinitionDefault)]
        public static bool DeveloperBuildInfoAssemblyDefinition = k_DeveloperBuildInfoAssemblyDefinitionDefault;
        const bool k_DeveloperBuildInfoAssemblyDefinitionDefault = true;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoBuildChangelistArgumentDefault)]
        public static string DeveloperBuildInfoBuildChangelistArgument =
            k_DeveloperBuildInfoBuildChangelistArgumentDefault;
        const string k_DeveloperBuildInfoBuildChangelistArgumentDefault = "BUILD_CHANGELIST";

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoBuildDescriptionArgumentDefault)]
        public static string DeveloperBuildInfoBuildDescriptionArgument =
            k_DeveloperBuildInfoBuildDescriptionArgumentDefault;
        const string k_DeveloperBuildInfoBuildDescriptionArgumentDefault = "BUILD_DESC";

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoBuildNumberArgumentDefault)]
        public static string DeveloperBuildInfoBuildNumberArgument = k_DeveloperBuildInfoBuildNumberArgumentDefault;
        const string k_DeveloperBuildInfoBuildNumberArgumentDefault = "BUILD";

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoBuildStreamArgumentDefault)]
        public static string DeveloperBuildInfoBuildStreamArgument = k_DeveloperBuildInfoBuildStreamArgumentDefault;
        const string k_DeveloperBuildInfoBuildStreamArgumentDefault = "BUILD_STREAM";

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoBuildTaskArgumentDefault)]
        public static string DeveloperBuildInfoBuildTaskArgument = k_DeveloperBuildInfoBuildTaskArgumentDefault;
        const string k_DeveloperBuildInfoBuildTaskArgumentDefault = "BUILD_TASK";

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoEnabledDefault)]
        public static bool DeveloperBuildInfoEnabled = k_DeveloperBuildInfoEnabledDefault;
        const bool k_DeveloperBuildInfoEnabledDefault = false;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoNamespaceDefault)]
        public static string DeveloperBuildInfoNamespace = k_DeveloperBuildInfoNamespaceDefault;
        const string k_DeveloperBuildInfoNamespaceDefault = "Generated.Build";

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        [OriginalValue(k_DeveloperBuildInfoPathDefault)]
        public static string DeveloperBuildInfoPath = k_DeveloperBuildInfoPathDefault;
        const string k_DeveloperBuildInfoPathDefault = "Generated/Build/BuildInfo.cs";

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        [OriginalValue(k_DeveloperCommandLineParserArgumentPrefixDefault)]
        public static string DeveloperCommandLineParserArgumentPrefix =
            k_DeveloperCommandLineParserArgumentPrefixDefault;
        const string k_DeveloperCommandLineParserArgumentPrefixDefault = "--";

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        [OriginalValue(k_DeveloperCommandLineParserArgumentSplitDefault)]
        public static string DeveloperCommandLineParserArgumentSplit = k_DeveloperCommandLineParserArgumentSplitDefault;
        const string k_DeveloperCommandLineParserArgumentSplitDefault = "=";

        /// <summary>
        ///     Should GDX make sure that it's shaders are always included in builds.
        /// </summary>
        [OriginalValue(k_EnvironmentAlwaysIncludeShadersDefault)]
        public static bool EnvironmentAlwaysIncludeShaders = k_EnvironmentAlwaysIncludeShadersDefault;
        const bool k_EnvironmentAlwaysIncludeShadersDefault = true;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        [OriginalValue(k_EnvironmentScriptingDefineSymbolDefault)]
        public static bool EnvironmentScriptingDefineSymbol = k_EnvironmentScriptingDefineSymbolDefault;
        const bool k_EnvironmentScriptingDefineSymbolDefault = false;

        /// <summary>
        ///     Should the Editor Task Director tick the Task Director.
        /// </summary>
        [OriginalValue(k_EnvironmentEditorTaskDirectorDefault)]
        public static bool EnvironmentEditorTaskDirector = k_EnvironmentEditorTaskDirectorDefault;
        const bool k_EnvironmentEditorTaskDirectorDefault = true;

        /// <summary>
        ///     Should the editor task director also tick in play mode?
        /// </summary>
        [OriginalValue(k_EnvironmentEditorTaskDirectorTickInPlayModeDefault)]
        public static bool EnvironmentEditorTaskDirectorTickInPlayMode = k_EnvironmentEditorTaskDirectorTickInPlayModeDefault;
        const bool k_EnvironmentEditorTaskDirectorTickInPlayModeDefault = false;

        /// <summary>
        ///     How often should the editor task director tick?
        /// </summary>
        [OriginalValue(k_EnvironmentEditorTaskDirectorTickRateDefault)]
        public static double EnvironmentEditorTaskDirectorTickRate = k_EnvironmentEditorTaskDirectorTickRateDefault;
        const double k_EnvironmentEditorTaskDirectorTickRateDefault = 0.5d;

        /// <summary>
        ///     Should the Task Director System be added to the player loop during playmode.
        /// </summary>
        [OriginalValue(k_EnvironmentTaskDirectorDefault)]
        public static bool EnvironmentTaskDirector = k_EnvironmentTaskDirectorDefault;
        const bool k_EnvironmentTaskDirectorDefault = true;

        /// <summary>
        ///     How often should the task director tick in playmode?
        /// </summary>
        [OriginalValue(k_EnvironmentTaskDirectorTickRateDefault)]
        public static float EnvironmentTaskDirectorTickRate = k_EnvironmentTaskDirectorTickRateDefault;
        const float k_EnvironmentTaskDirectorTickRateDefault = 0.1f;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        [OriginalValue(k_LocalizationDefaultCultureDefault)]
        public static Localization.Language LocalizationDefaultCulture = k_LocalizationDefaultCultureDefault;
        const Localization.Language k_LocalizationDefaultCultureDefault = Localization.Language.English;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        [OriginalValue(k_LocalizationSetDefaultCultureDefault)]
        public static bool LocalizationSetDefaultCulture = k_LocalizationSetDefaultCultureDefault;
        const bool k_LocalizationSetDefaultCultureDefault = true;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        [OriginalValue(k_PlatformAutomationFolderDefault)]
        public static string PlatformAutomationFolder = k_PlatformAutomationFolderDefault;
        const string k_PlatformAutomationFolderDefault = "GDX_Automation";

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        [OriginalValue(k_PlatformCacheFolderDefault)]
        public static string PlatformCacheFolder = k_PlatformCacheFolderDefault;
        const string k_PlatformCacheFolderDefault = "GDX_Cache";

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        [OriginalValue(k_TraceDebugLevelsDefault)]
        public static Trace.TraceLevel TraceDebugLevels = k_TraceDebugLevelsDefault;
        const Trace.TraceLevel k_TraceDebugLevelsDefault = Trace.TraceLevel.Warning |
                                                           Trace.TraceLevel.Assertion |
                                                           Trace.TraceLevel.Error |
                                                           Trace.TraceLevel.Exception;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        [OriginalValue(k_TraceDebugOutputToUnityConsoleDefault)]
        public static bool TraceDebugOutputToUnityConsole = k_TraceDebugOutputToUnityConsoleDefault;
        const bool k_TraceDebugOutputToUnityConsoleDefault = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        [OriginalValue(k_TraceDevelopmentLevelsDefault)]
        public static Trace.TraceLevel TraceDevelopmentLevels = k_TraceDevelopmentLevelsDefault;
        const Trace.TraceLevel k_TraceDevelopmentLevelsDefault = Trace.TraceLevel.Log |
                                                                 Trace.TraceLevel.Warning |
                                                                 Trace.TraceLevel.Error |
                                                                 Trace.TraceLevel.Exception |
                                                                 Trace.TraceLevel.Assertion |
                                                                 Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        [OriginalValue(k_TraceDevelopmentOutputToUnityConsoleDefault)]
        public static bool TraceDevelopmentOutputToUnityConsole = k_TraceDevelopmentOutputToUnityConsoleDefault;
        const bool k_TraceDevelopmentOutputToUnityConsoleDefault = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        [OriginalValue(k_TraceReleaseLevelsDefault)]
        public static Trace.TraceLevel TraceReleaseLevels = k_TraceReleaseLevelsDefault;
        const Trace.TraceLevel k_TraceReleaseLevelsDefault = Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        [OriginalValue(k_UpdateProviderCheckForUpdates)]
        public static bool UpdateProviderCheckForUpdates = true;
        const bool k_UpdateProviderCheckForUpdates = true;
    }
}