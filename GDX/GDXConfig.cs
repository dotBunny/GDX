// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX
{
    /// <summary>
    ///     Project-wide configuration configurable for runtime override.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GDXConfig
    {
        /// <summary>
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        public string ConfigOutputPath = "Generated/GDXSettings.cs";

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        public bool DeveloperBuildInfoAssemblyDefinition = true;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildDescriptionArgument = "BUILD_DESC";

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildNumberArgument = "BUILD";

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildStreamArgument = "BUILD_STREAM";

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildTaskArgument = "BUILD_TASK";

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public bool DeveloperBuildInfoEnabled;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public string DeveloperBuildInfoNamespace = "Generated.Build";

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public string DeveloperBuildInfoPath = "Generated/Build/BuildInfo.cs";

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public string DeveloperCommandLineParserArgumentPrefix = "--";

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public string DeveloperCommandLineParserArgumentSplit = "=";

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public bool EnvironmentScriptingDefineSymbol;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        public Localization.Language LocalizationDefaultCulture = Localization.Language.English;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        public bool LocalizationSetDefaultCulture = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        public Trace.TraceLevel TraceDebugLevels = Trace.TraceLevel.Warning |
                                                   Trace.TraceLevel.Assertion |
                                                   Trace.TraceLevel.Error |
                                                   Trace.TraceLevel.Exception;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        public bool TraceDebugOutputToUnityConsole = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        public Trace.TraceLevel TraceDevelopmentLevels = Trace.TraceLevel.Log |
                                                         Trace.TraceLevel.Warning |
                                                         Trace.TraceLevel.Error |
                                                         Trace.TraceLevel.Exception |
                                                         Trace.TraceLevel.Assertion |
                                                         Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        public bool TraceDevelopmentOutputToUnityConsole = true;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        public Trace.TraceLevel TraceReleaseLevels = Trace.TraceLevel.Fatal;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public bool UpdateProviderCheckForUpdates = true;

        public GDXConfig()
        {
        }

        public GDXConfig(GDXConfig initialState)
        {
            ConfigOutputPath = initialState.ConfigOutputPath;
            UpdateProviderCheckForUpdates = initialState.UpdateProviderCheckForUpdates;
            DeveloperCommandLineParserArgumentPrefix = initialState.DeveloperCommandLineParserArgumentPrefix;
            DeveloperCommandLineParserArgumentSplit = initialState.DeveloperCommandLineParserArgumentSplit;
            DeveloperBuildInfoAssemblyDefinition = initialState.DeveloperBuildInfoAssemblyDefinition;
            DeveloperBuildInfoEnabled = initialState.DeveloperBuildInfoEnabled;
            DeveloperBuildInfoPath = initialState.DeveloperBuildInfoPath;
            DeveloperBuildInfoNamespace = initialState.DeveloperBuildInfoNamespace;
            DeveloperBuildInfoBuildNumberArgument = initialState.DeveloperBuildInfoBuildNumberArgument;
            DeveloperBuildInfoBuildDescriptionArgument = initialState.DeveloperBuildInfoBuildDescriptionArgument;
            DeveloperBuildInfoBuildChangelistArgument = initialState.DeveloperBuildInfoBuildChangelistArgument;
            DeveloperBuildInfoBuildTaskArgument = initialState.DeveloperBuildInfoBuildTaskArgument;
            DeveloperBuildInfoBuildStreamArgument = initialState.DeveloperBuildInfoBuildStreamArgument;
            EnvironmentScriptingDefineSymbol = initialState.EnvironmentScriptingDefineSymbol;
            TraceDevelopmentLevels = initialState.TraceDevelopmentLevels;
            TraceDebugLevels = initialState.TraceDebugLevels;
            TraceReleaseLevels = initialState.TraceReleaseLevels;
            TraceDevelopmentOutputToUnityConsole = initialState.TraceDevelopmentOutputToUnityConsole;
            TraceDebugOutputToUnityConsole = initialState.TraceDebugOutputToUnityConsole;
            LocalizationSetDefaultCulture = initialState.LocalizationSetDefaultCulture;
            LocalizationDefaultCulture = initialState.LocalizationDefaultCulture;
        }

        public bool Compare(GDXConfig rhs)
        {
            return UpdateProviderCheckForUpdates == rhs.UpdateProviderCheckForUpdates &&
                   DeveloperCommandLineParserArgumentPrefix == rhs.DeveloperCommandLineParserArgumentPrefix &&
                   DeveloperCommandLineParserArgumentSplit == rhs.DeveloperCommandLineParserArgumentSplit &&
                   DeveloperBuildInfoAssemblyDefinition == rhs.DeveloperBuildInfoAssemblyDefinition &&
                   DeveloperBuildInfoEnabled == rhs.DeveloperBuildInfoEnabled &&
                   DeveloperBuildInfoPath == rhs.DeveloperBuildInfoPath &&
                   DeveloperBuildInfoNamespace == rhs.DeveloperBuildInfoNamespace &&
                   DeveloperBuildInfoBuildNumberArgument == rhs.DeveloperBuildInfoBuildNumberArgument &&
                   DeveloperBuildInfoBuildDescriptionArgument == rhs.DeveloperBuildInfoBuildDescriptionArgument &&
                   DeveloperBuildInfoBuildChangelistArgument == rhs.DeveloperBuildInfoBuildChangelistArgument &&
                   DeveloperBuildInfoBuildTaskArgument == rhs.DeveloperBuildInfoBuildTaskArgument &&
                   DeveloperBuildInfoBuildStreamArgument == rhs.DeveloperBuildInfoBuildStreamArgument &&
                   EnvironmentScriptingDefineSymbol == rhs.EnvironmentScriptingDefineSymbol &&
                   TraceDevelopmentLevels == rhs.TraceDevelopmentLevels &&
                   TraceDebugLevels == rhs.TraceDebugLevels &&
                   TraceReleaseLevels == rhs.TraceReleaseLevels &&
                   TraceDevelopmentOutputToUnityConsole == rhs.TraceDevelopmentOutputToUnityConsole &&
                   TraceDebugOutputToUnityConsole == rhs.TraceDebugOutputToUnityConsole &&
                   LocalizationSetDefaultCulture == rhs.LocalizationSetDefaultCulture &&
                   LocalizationDefaultCulture == rhs.LocalizationDefaultCulture &&
                   ConfigOutputPath == rhs.ConfigOutputPath;
        }
    }
}