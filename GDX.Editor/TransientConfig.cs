// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Editor
{
    /// <summary>
    ///     A configuration that is operated on when in Project Settings.
    /// </summary>
    /// <remarks>A non-static copy of <see cref="GDX.Config"/>.</remarks>
    public class TransientConfig
    {
        /// <summary>
        ///     The asset database relative path of the GDX config override file.
        /// </summary>
        public string ConfigOutputPath;

        /// <summary>
        ///     Ensure that there is an assembly definition wrapping the generated content.
        /// </summary>
        public bool DeveloperBuildInfoAssemblyDefinition;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildChangelistArgument;

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildDescriptionArgument;

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildNumberArgument;

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildStreamArgument;

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public string DeveloperBuildInfoBuildTaskArgument;

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public bool DeveloperBuildInfoEnabled;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public string DeveloperBuildInfoNamespace;

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public string DeveloperBuildInfoPath;

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public string DeveloperCommandLineParserArgumentPrefix;

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public string DeveloperCommandLineParserArgumentSplit;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public bool EnvironmentScriptingDefineSymbol;

        /// <summary>
        ///     Should GDX make sure that it's shaders are always included in builds.
        /// </summary>
        public bool EnvironmentAlwaysIncludeShaders;

        /// <summary>
        ///     The language to set the default thread culture too.
        /// </summary>
        public Localization.Language LocalizationDefaultCulture;

        /// <summary>
        ///     Should the default thread culture be set?
        /// </summary>
        public bool LocalizationSetDefaultCulture;

        /// <summary>
        ///     The project relative path where automation should store its artifacts.
        /// </summary>
        public string PlatformAutomationFolder;

        /// <summary>
        ///     The project relative path to use as a cache.
        /// </summary>
        public string PlatformCacheFolder;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in debug builds?
        /// </summary>
        public Trace.TraceLevel TraceDebugLevels;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in debug builds?
        /// </summary>
        public bool TraceDebugOutputToUnityConsole;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
        /// </summary>
        public Trace.TraceLevel TraceDevelopmentLevels;

        /// <summary>
        ///     Should GDX based traces output to the Unity console in the editor or development builds?
        /// </summary>
        public bool TraceDevelopmentOutputToUnityConsole;

        /// <summary>
        ///     What is the level of traces which should be processed and logged by GDX in release builds?
        /// </summary>
        public Trace.TraceLevel TraceReleaseLevels;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public bool UpdateProviderCheckForUpdates;

        public TransientConfig()
        {
            ConfigOutputPath = Config.ConfigOutputPath;
            DeveloperBuildInfoAssemblyDefinition = Config.DeveloperBuildInfoAssemblyDefinition;
            DeveloperBuildInfoBuildChangelistArgument = Config.DeveloperBuildInfoBuildChangelistArgument;
            DeveloperBuildInfoBuildDescriptionArgument = Config.DeveloperBuildInfoBuildDescriptionArgument;
            DeveloperBuildInfoBuildNumberArgument = Config.DeveloperBuildInfoBuildNumberArgument;
            DeveloperBuildInfoBuildStreamArgument = Config.DeveloperBuildInfoBuildStreamArgument;
            DeveloperBuildInfoBuildTaskArgument = Config.DeveloperBuildInfoBuildTaskArgument;
            DeveloperBuildInfoEnabled = Config.DeveloperBuildInfoEnabled;
            DeveloperBuildInfoNamespace = Config.DeveloperBuildInfoNamespace;
            DeveloperBuildInfoPath = Config.DeveloperBuildInfoPath;
            DeveloperCommandLineParserArgumentPrefix = Config.DeveloperCommandLineParserArgumentPrefix;
            DeveloperCommandLineParserArgumentSplit = Config.DeveloperCommandLineParserArgumentSplit;
            EnvironmentScriptingDefineSymbol = Config.EnvironmentScriptingDefineSymbol;
            EnvironmentAlwaysIncludeShaders = Config.EnvironmentAlwaysIncludeShaders;
            LocalizationDefaultCulture = Config.LocalizationDefaultCulture;
            LocalizationSetDefaultCulture = Config.LocalizationSetDefaultCulture;
            PlatformAutomationFolder = Config.PlatformAutomationFolder;
            PlatformCacheFolder = Config.PlatformCacheFolder;
            TraceDebugLevels = Config.TraceDebugLevels;
            TraceDebugOutputToUnityConsole = Config.TraceDebugOutputToUnityConsole;
            TraceDevelopmentLevels = Config.TraceDevelopmentLevels;
            TraceDevelopmentOutputToUnityConsole = Config.TraceDevelopmentOutputToUnityConsole;
            TraceReleaseLevels = Config.TraceReleaseLevels;
            UpdateProviderCheckForUpdates = Config.UpdateProviderCheckForUpdates;
        }

        public bool HasChanges()
        {
            return UpdateProviderCheckForUpdates == Config.UpdateProviderCheckForUpdates &&
                   DeveloperCommandLineParserArgumentPrefix == Config.DeveloperCommandLineParserArgumentPrefix &&
                   DeveloperCommandLineParserArgumentSplit == Config.DeveloperCommandLineParserArgumentSplit &&
                   DeveloperBuildInfoAssemblyDefinition == Config.DeveloperBuildInfoAssemblyDefinition &&
                   DeveloperBuildInfoEnabled == Config.DeveloperBuildInfoEnabled &&
                   DeveloperBuildInfoPath == Config.DeveloperBuildInfoPath &&
                   DeveloperBuildInfoNamespace == Config.DeveloperBuildInfoNamespace &&
                   DeveloperBuildInfoBuildNumberArgument == Config.DeveloperBuildInfoBuildNumberArgument &&
                   DeveloperBuildInfoBuildDescriptionArgument == Config.DeveloperBuildInfoBuildDescriptionArgument &&
                   DeveloperBuildInfoBuildChangelistArgument == Config.DeveloperBuildInfoBuildChangelistArgument &&
                   DeveloperBuildInfoBuildTaskArgument == Config.DeveloperBuildInfoBuildTaskArgument &&
                   DeveloperBuildInfoBuildStreamArgument == Config.DeveloperBuildInfoBuildStreamArgument &&
                   EnvironmentScriptingDefineSymbol == Config.EnvironmentScriptingDefineSymbol &&
                   EnvironmentAlwaysIncludeShaders == Config.EnvironmentAlwaysIncludeShaders &&
                   TraceDevelopmentLevels == Config.TraceDevelopmentLevels &&
                   TraceDebugLevels == Config.TraceDebugLevels &&
                   TraceReleaseLevels == Config.TraceReleaseLevels &&
                   TraceDevelopmentOutputToUnityConsole == Config.TraceDevelopmentOutputToUnityConsole &&
                   TraceDebugOutputToUnityConsole == Config.TraceDebugOutputToUnityConsole &&
                   LocalizationSetDefaultCulture == Config.LocalizationSetDefaultCulture &&
                   LocalizationDefaultCulture == Config.LocalizationDefaultCulture &&
                   ConfigOutputPath == Config.ConfigOutputPath &&
                   PlatformAutomationFolder == Config.PlatformAutomationFolder &&
                   PlatformCacheFolder == Config.PlatformCacheFolder;
        }
    }
}