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
    public static class Config
    {
        /// <summary>
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        public static string ConfigOutputPath = k_ConfigOutputPathDefault;
        internal const string k_ConfigOutputPathDefault = "Generated/GDX.generated.cs";

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        public static bool DeveloperBuildInfoAssemblyDefinition = k_DeveloperBuildInfoAssemblyDefinitionDefault;
        internal const bool k_DeveloperBuildInfoAssemblyDefinitionDefault = true;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildChangelistArgument =
            k_DeveloperBuildInfoBuildChangelistArgumentDefault;
        internal const string k_DeveloperBuildInfoBuildChangelistArgumentDefault = "BUILD_CHANGELIST";

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildDescriptionArgument =
            k_DeveloperBuildInfoBuildDescriptionArgumentDefault;
        internal const string k_DeveloperBuildInfoBuildDescriptionArgumentDefault = "BUILD_DESC";

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildNumberArgument = k_DeveloperBuildInfoBuildNumberArgumentDefault;
        internal const string k_DeveloperBuildInfoBuildNumberArgumentDefault = "BUILD";

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildStreamArgument = k_DeveloperBuildInfoBuildStreamArgumentDefault;
        internal const string k_DeveloperBuildInfoBuildStreamArgumentDefault = "BUILD_STREAM";

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildTaskArgument = k_DeveloperBuildInfoBuildTaskArgumentDefault;
        internal const string k_DeveloperBuildInfoBuildTaskArgumentDefault = "BUILD_TASK";

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public static bool DeveloperBuildInfoEnabled = k_DeveloperBuildInfoEnabledDefault;
        internal const bool k_DeveloperBuildInfoEnabledDefault = false;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public static string DeveloperBuildInfoNamespace = k_DeveloperBuildInfoNamespaceDefault;
        internal const string k_DeveloperBuildInfoNamespaceDefault = "Generated.Build";

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public static string DeveloperBuildInfoPath = k_DeveloperBuildInfoPathDefault;
        internal const string k_DeveloperBuildInfoPathDefault = "Generated/Build/BuildInfo.cs";

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public static string DeveloperCommandLineParserArgumentPrefix =
            k_DeveloperCommandLineParserArgumentPrefixDefault;
        internal const string k_DeveloperCommandLineParserArgumentPrefixDefault = "--";

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public static string DeveloperCommandLineParserArgumentSplit = k_DeveloperCommandLineParserArgumentSplitDefault;
        internal const string k_DeveloperCommandLineParserArgumentSplitDefault = "=";

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public static bool EnvironmentScriptingDefineSymbol = k_EnvironmentScriptingDefineSymbolDefault;
        internal const bool k_EnvironmentScriptingDefineSymbolDefault = false;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        public static Localization.Language LocalizationDefaultCulture = k_LocalizationDefaultCultureDefault;
        internal const Localization.Language k_LocalizationDefaultCultureDefault = Localization.Language.English;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        public static bool LocalizationSetDefaultCulture = k_LocalizationSetDefaultCultureDefault;
        internal const bool k_LocalizationSetDefaultCultureDefault = true;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        public static string PlatformAutomationFolder = k_PlatformAutomationFolderDefault;
        internal const string k_PlatformAutomationFolderDefault = "GDX_Automation";

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        public static string PlatformCacheFolder = k_PlatformCacheFolderDefault;
        internal const string k_PlatformCacheFolderDefault = "GDX_Cache";

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        public static Trace.TraceLevel TraceDebugLevels = k_TraceDebugLevelsDefault;
        internal const Trace.TraceLevel k_TraceDebugLevelsDefault = Trace.TraceLevel.Warning |
                                                           Trace.TraceLevel.Assertion |
                                                           Trace.TraceLevel.Error |
                                                           Trace.TraceLevel.Exception;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        public static bool TraceDebugOutputToUnityConsole = k_TraceDebugOutputToUnityConsoleDefault;
        internal const bool k_TraceDebugOutputToUnityConsoleDefault = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        public static Trace.TraceLevel TraceDevelopmentLevels = k_TraceDevelopmentLevelsDefault;
        internal const Trace.TraceLevel k_TraceDevelopmentLevelsDefault = Trace.TraceLevel.Log |
                                                                 Trace.TraceLevel.Warning |
                                                                 Trace.TraceLevel.Error |
                                                                 Trace.TraceLevel.Exception |
                                                                 Trace.TraceLevel.Assertion |
                                                                 Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        public static bool TraceDevelopmentOutputToUnityConsole = k_TraceDevelopmentOutputToUnityConsoleDefault;
        internal const bool k_TraceDevelopmentOutputToUnityConsoleDefault = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        public static Trace.TraceLevel TraceReleaseLevels = k_TraceReleaseLevelsDefault;
        internal const Trace.TraceLevel k_TraceReleaseLevelsDefault = Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public static bool UpdateProviderCheckForUpdates = k_UpdateProviderCheckForUpdatesDefault;
        internal const bool k_UpdateProviderCheckForUpdatesDefault = true;
    }
}