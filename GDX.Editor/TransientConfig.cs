// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

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
        public bool BuildInfoAssemblyDefinition;

        /// <summary>
        ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        public string BuildInfoBuildChangelistArgument;

        /// <summary>
        ///     The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        public string BuildInfoBuildDescriptionArgument;

        /// <summary>
        ///     The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        public string BuildInfoBuildNumberArgument;

        /// <summary>
        ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        public string BuildInfoBuildStreamArgument;

        /// <summary>
        ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        public string BuildInfoBuildTaskArgument;

        /// <summary>
        ///     Should the BuildInfo file be written during builds?
        /// </summary>
        public bool BuildInfo;

        /// <summary>
        ///     The namespace where the BuildInfo should be placed.
        /// </summary>
        public string BuildInfoNamespace;

        /// <summary>
        ///     The path to output the BuildInfo file.
        /// </summary>
        public string BuildInfoOutputPath;

        /// <summary>
        ///     What should be used to denote arguments in the command line?
        /// </summary>
        public string CommandLineParserArgumentPrefix;

        /// <summary>
        ///     What should be used to split arguments from their values in the command line?
        /// </summary>
        public string CommandLineParserArgumentSplit;

        /// <summary>
        ///     Should the Editor Task Director tick the Task Director.
        /// </summary>
        public bool EditorTaskDirectorSystem;

        /// <summary>
        ///     How often should the editor task director tick trigger the task director to tick?
        /// </summary>
        public double EditorTaskDirectorSystemTickRate;

        /// <summary>
        ///     Should GDX make sure that it's shaders are always included in builds.
        /// </summary>
        public bool EnvironmentAlwaysIncludeShaders;

        /// <summary>
        ///     Should a GDX scripting define symbol be added to all target build groups.
        /// </summary>
        public bool EnvironmentScriptingDefineSymbol;

        /// <summary>
        ///     Should the GDX tools menu be added in the editor?
        /// </summary>
        public bool EnvironmentToolsMenu;

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
        ///     Should the Task Director System be added to the player loop during playmode.
        /// </summary>
        public bool TaskDirectorSystem;

        /// <summary>
        ///     How often should the task director tick in playmode?
        /// </summary>
        public float TaskDirectorSystemTickRate;

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public bool UpdateProviderCheckForUpdates;

        public TransientConfig()
        {
            ConfigOutputPath = Config.ConfigOutputPath;
            BuildInfoAssemblyDefinition = Config.BuildInfoAssemblyDefinition;
            BuildInfoBuildChangelistArgument = Config.BuildInfoBuildChangelistArgument;
            BuildInfoBuildDescriptionArgument = Config.BuildInfoBuildDescriptionArgument;
            BuildInfoBuildNumberArgument = Config.BuildInfoBuildNumberArgument;
            BuildInfoBuildStreamArgument = Config.BuildInfoBuildStreamArgument;
            BuildInfoBuildTaskArgument = Config.BuildInfoBuildTaskArgument;
            BuildInfo = Config.BuildInfo;
            BuildInfoNamespace = Config.BuildInfoNamespace;
            BuildInfoOutputPath = Config.BuildInfoOutputPath;
            CommandLineParserArgumentPrefix = Config.CommandLineParserArgumentPrefix;
            CommandLineParserArgumentSplit = Config.CommandLineParserArgumentSplit;
            EnvironmentAlwaysIncludeShaders = Config.EnvironmentAlwaysIncludeShaders;
            EditorTaskDirectorSystem = Config.EditorTaskDirectorSystem;
            EditorTaskDirectorSystemTickRate = Config.EditorTaskDirectorSystemTickRate;
            EnvironmentScriptingDefineSymbol = Config.EnvironmentScriptingDefineSymbol;
            EnvironmentToolsMenu = Config.EnvironmentToolsMenu;
            TaskDirectorSystem = Config.TaskDirectorSystem;
            TaskDirectorSystemTickRate = Config.TaskDirectorSystemTickRate;
            LocalizationDefaultCulture = Config.LocalizationDefaultCulture;
            LocalizationSetDefaultCulture = Config.LocalizationSetDefaultCulture;
            PlatformAutomationFolder = Config.PlatformAutomationFolder;
            PlatformCacheFolder = Config.PlatformCacheFolder;
            UpdateProviderCheckForUpdates = Config.UpdateProviderCheckForUpdates;
        }

        public bool HasChanges()
        {
            return UpdateProviderCheckForUpdates == Config.UpdateProviderCheckForUpdates &&
                   CommandLineParserArgumentPrefix == Config.CommandLineParserArgumentPrefix &&
                   CommandLineParserArgumentSplit == Config.CommandLineParserArgumentSplit &&
                   BuildInfoAssemblyDefinition == Config.BuildInfoAssemblyDefinition &&
                   BuildInfo == Config.BuildInfo &&
                   BuildInfoOutputPath == Config.BuildInfoOutputPath &&
                   BuildInfoNamespace == Config.BuildInfoNamespace &&
                   BuildInfoBuildNumberArgument == Config.BuildInfoBuildNumberArgument &&
                   BuildInfoBuildDescriptionArgument == Config.BuildInfoBuildDescriptionArgument &&
                   BuildInfoBuildChangelistArgument == Config.BuildInfoBuildChangelistArgument &&
                   BuildInfoBuildTaskArgument == Config.BuildInfoBuildTaskArgument &&
                   BuildInfoBuildStreamArgument == Config.BuildInfoBuildStreamArgument &&
                   EnvironmentScriptingDefineSymbol == Config.EnvironmentScriptingDefineSymbol &&
                   EnvironmentToolsMenu == Config.EnvironmentToolsMenu &&
                   EnvironmentAlwaysIncludeShaders == Config.EnvironmentAlwaysIncludeShaders &&
                   TaskDirectorSystem == Config.TaskDirectorSystem &&
                   Math.Abs(TaskDirectorSystemTickRate - Config.TaskDirectorSystemTickRate) < Platform.FloatTolerance &&
                   EditorTaskDirectorSystem == Config.EditorTaskDirectorSystem &&
                   Math.Abs(EditorTaskDirectorSystemTickRate - Config.EditorTaskDirectorSystemTickRate) < Platform.DoubleTolerance &&
                   LocalizationSetDefaultCulture == Config.LocalizationSetDefaultCulture &&
                   LocalizationDefaultCulture == Config.LocalizationDefaultCulture &&
                   ConfigOutputPath == Config.ConfigOutputPath &&
                   PlatformAutomationFolder == Config.PlatformAutomationFolder &&
                   PlatformCacheFolder == Config.PlatformCacheFolder;
        }
    }
}