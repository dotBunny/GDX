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
        public static string ConfigOutputPath = ConfigDefaults.ConfigOutputPath;

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        public static bool DeveloperBuildInfoAssemblyDefinition = ConfigDefaults.DeveloperBuildInfoAssemblyDefinition;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildChangelistArgument =
            ConfigDefaults.DeveloperBuildInfoBuildChangelistArgument;

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildDescriptionArgument =
            ConfigDefaults.DeveloperBuildInfoBuildDescriptionArgument;

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildNumberArgument =
            ConfigDefaults.DeveloperBuildInfoBuildNumberArgument;

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildStreamArgument =
            ConfigDefaults.DeveloperBuildInfoBuildStreamArgument;

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public static string DeveloperBuildInfoBuildTaskArgument = ConfigDefaults.DeveloperBuildInfoBuildTaskArgument;

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public static bool DeveloperBuildInfoEnabled = ConfigDefaults.DeveloperBuildInfoEnabled;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public static string DeveloperBuildInfoNamespace = ConfigDefaults.DeveloperBuildInfoNamespace;

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public static string DeveloperBuildInfoPath = ConfigDefaults.DeveloperBuildInfoPath;

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public static string DeveloperCommandLineParserArgumentPrefix =
            ConfigDefaults.DeveloperCommandLineParserArgumentPrefix;

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public static string DeveloperCommandLineParserArgumentSplit =
            ConfigDefaults.DeveloperCommandLineParserArgumentSplit;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public static bool EnvironmentScriptingDefineSymbol = ConfigDefaults.EnvironmentScriptingDefineSymbol;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        public static Localization.Language LocalizationDefaultCulture = ConfigDefaults.LocalizationDefaultCulture;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        public static bool LocalizationSetDefaultCulture = ConfigDefaults.LocalizationSetDefaultCulture;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        public static string PlatformAutomationFolder = ConfigDefaults.PlatformAutomationFolder;

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        public static string PlatformCacheFolder = ConfigDefaults.PlatformCacheFolder;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        public static Trace.TraceLevel TraceDebugLevels = ConfigDefaults.TraceDebugLevels;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        public static bool TraceDebugOutputToUnityConsole = ConfigDefaults.TraceDebugOutputToUnityConsole;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        public static Trace.TraceLevel TraceDevelopmentLevels = ConfigDefaults.TraceDevelopmentLevels;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        public static bool TraceDevelopmentOutputToUnityConsole = ConfigDefaults.TraceDevelopmentOutputToUnityConsole;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        public static Trace.TraceLevel TraceReleaseLevels = ConfigDefaults.TraceReleaseLevels;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public static bool UpdateProviderCheckForUpdates = ConfigDefaults.UpdateProviderCheckForUpdates;
    }
}