// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable ConvertToConstant.Global

using GDX.Logging;

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
        public const int FormatVersion = 2;

        const string k_ConfigOutputPath = "Generated/GDX.generated.cs";
        const bool k_BuildInfo = false;
        const bool k_BuildInfoAssemblyDefinition = true;
        const string k_BuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";
        const string k_BuildInfoBuildDescriptionArgument = "BUILD_DESC";
        const string k_BuildInfoBuildNumberArgument = "BUILD";
        const string k_BuildInfoBuildStreamArgument = "BUILD_STREAM";
        const string k_BuildInfoBuildTaskArgument = "BUILD_TASK";
        const string k_BuildInfoNamespace = "Generated.Build";
        const string k_BuildInfoOutputPath = "Generated/Build/BuildInfo.cs";
        const string k_CommandLineParserArgumentPrefix = "--";
        const string k_CommandLineParserArgumentSplit = "=";
        const bool k_EditorTaskDirectorSystem = true;
        const double k_EditorTaskDirectorSystemTickRate = 0.5d;
        const bool k_EnvironmentAlwaysIncludeShaders = true;
        const bool k_EnvironmentDeveloperConsole = true;
        const bool k_EnvironmentManagedLog = true;
        const bool k_EnvironmentAutoCaptureUnityLogs = true;
        const bool k_EnvironmentScriptingDefineSymbol = false;
        const bool k_EnvironmentToolsMenu = false;
        const Localization.Language k_LocalizationDefaultCulture = Localization.Language.English;
        const bool k_LocalizationSetDefaultCulture = true;
        const string k_PlatformAutomationFolder = "GDX_Automation";
        const string k_PlatformCacheFolder = "GDX_Cache";
        const bool k_TaskDirectorSystem = false;
        const float k_TaskDirectorSystemTickRate = 0.1f;
        const bool k_UpdateProviderCheckForUpdates = true;

        /// <summary>
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        [OriginalValue(k_ConfigOutputPath)] public static string ConfigOutputPath = k_ConfigOutputPath;

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        [OriginalValue(k_BuildInfo)] public static bool BuildInfo = k_BuildInfo;

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        [OriginalValue(k_BuildInfoAssemblyDefinition)]
        public static bool BuildInfoAssemblyDefinition = k_BuildInfoAssemblyDefinition;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildChangelistArgument)]
        public static string BuildInfoBuildChangelistArgument = k_BuildInfoBuildChangelistArgument;

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildDescriptionArgument)]
        public static string BuildInfoBuildDescriptionArgument = k_BuildInfoBuildDescriptionArgument;

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildNumberArgument)]
        public static string BuildInfoBuildNumberArgument = k_BuildInfoBuildNumberArgument;

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildStreamArgument)]
        public static string BuildInfoBuildStreamArgument = k_BuildInfoBuildStreamArgument;

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        [OriginalValue(k_BuildInfoBuildTaskArgument)]
        public static string BuildInfoBuildTaskArgument = k_BuildInfoBuildTaskArgument;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        [OriginalValue(k_BuildInfoNamespace)] public static string BuildInfoNamespace = k_BuildInfoNamespace;

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        [OriginalValue(k_BuildInfoOutputPath)] public static string BuildInfoOutputPath = k_BuildInfoOutputPath;

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        [OriginalValue(k_CommandLineParserArgumentPrefix)]
        public static string CommandLineParserArgumentPrefix =
            k_CommandLineParserArgumentPrefix;

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        [OriginalValue(k_CommandLineParserArgumentSplit)]
        public static string CommandLineParserArgumentSplit = k_CommandLineParserArgumentSplit;

        /// <summary>
        ///     Should the Editor Task Director tick the Task Director.
        /// </summary>
        [OriginalValue(k_EditorTaskDirectorSystem)]
        public static bool EditorTaskDirectorSystem = k_EditorTaskDirectorSystem;

        /// <summary>
        ///     How often should the editor task director tick trigger the task director to tick?
        /// </summary>
        [OriginalValue(k_EditorTaskDirectorSystemTickRate)]
        public static double EditorTaskDirectorSystemTickRate = k_EditorTaskDirectorSystemTickRate;

        /// <summary>
        ///     Should the GDX tools menu be added in the editor?
        /// </summary>
        [OriginalValue(k_EnvironmentDeveloperConsole)]
        public static bool EnvironmentDeveloperConsole = k_EnvironmentDeveloperConsole;

        /// <summary>
        ///     Should the GDX utilized the <see cref="ManagedLog" />.
        /// </summary>
        [OriginalValue(k_EnvironmentManagedLog)]
        public static bool EnvironmentManagedLog = k_EnvironmentManagedLog;

        /// <summary>
        ///     Should the <see cref="ManagedLog" /> automatically capture the Unity logs?
        /// </summary>
        [OriginalValue(k_EnvironmentAutoCaptureUnityLogs)]
        public static bool EnvironmentAutoCaptureUnityLogs = k_EnvironmentAutoCaptureUnityLogs;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        [OriginalValue(k_EnvironmentScriptingDefineSymbol)]
        public static bool EnvironmentScriptingDefineSymbol = k_EnvironmentScriptingDefineSymbol;

        /// <summary>
        ///     Should the GDX tools menu be added in the editor?
        /// </summary>
        [OriginalValue(k_EnvironmentToolsMenu)]
        public static bool EnvironmentToolsMenu = k_EnvironmentToolsMenu;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        [OriginalValue(k_LocalizationDefaultCulture)]
        public static Localization.Language LocalizationDefaultCulture = k_LocalizationDefaultCulture;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        [OriginalValue(k_LocalizationSetDefaultCulture)]
        public static bool LocalizationSetDefaultCulture = k_LocalizationSetDefaultCulture;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        [OriginalValue(k_PlatformAutomationFolder)]
        public static string PlatformAutomationFolder = k_PlatformAutomationFolder;

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        [OriginalValue(k_PlatformCacheFolder)] public static string PlatformCacheFolder = k_PlatformCacheFolder;

        /// <summary>
        ///     Should the Task Director System be added to the player loop during playmode.
        /// </summary>
        [OriginalValue(k_TaskDirectorSystem)] public static bool TaskDirectorSystem = k_TaskDirectorSystem;

        /// <summary>
        ///     How often should the task director tick in playmode?
        /// </summary>
        [OriginalValue(k_TaskDirectorSystemTickRate)]
        public static float TaskDirectorSystemTickRate = k_TaskDirectorSystemTickRate;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        [OriginalValue(k_UpdateProviderCheckForUpdates)]
        public static bool UpdateProviderCheckForUpdates = true;
    }
}