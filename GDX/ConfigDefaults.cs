// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX
{
    public static class ConfigDefaults
    {
/// <summary>
            ///     The asset database relative path of the GDX config override file.
            /// </summary>
            public const string ConfigOutputPath = "Generated/GDX.generated.cs";

            /// <summary>
            ///     Ensure that there is an assembly definition wrapping the generated content.
            /// </summary>
            public const bool DeveloperBuildInfoAssemblyDefinition = true;

            /// <summary>
            ///     The argument key for the build's changelist to be passed to the BuildInfoGenerator.
            /// </summary>
            public const string DeveloperBuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";

            /// <summary>
            ///     The argument key for the build description to be passed to the BuildInfoGenerator.
            /// </summary>
            public const string DeveloperBuildInfoBuildDescriptionArgument = "BUILD_DESC";

            /// <summary>
            ///     The argument key for the build number to be passed to the BuildInfoGenerator.
            /// </summary>
            public const string DeveloperBuildInfoBuildNumberArgument = "BUILD";

            /// <summary>
            ///     The argument key for the build's stream to be passed to the BuildInfoGenerator.
            /// </summary>
            public const string DeveloperBuildInfoBuildStreamArgument = "BUILD_STREAM";

            /// <summary>
            ///     The argument key for the build's task to be passed to the BuildInfoGenerator.
            /// </summary>
            public const string DeveloperBuildInfoBuildTaskArgument = "BUILD_TASK";

            /// <summary>
            ///     Should the BuildInfo file be written during builds?
            /// </summary>
            public const bool DeveloperBuildInfoEnabled = false;

            /// <summary>
            ///     The namespace where the BuildInfo should be placed.
            /// </summary>
            public const string DeveloperBuildInfoNamespace = "Generated.Build";

            /// <summary>
            ///     The path to output the BuildInfo file.
            /// </summary>
            public const string DeveloperBuildInfoPath = "Generated/Build/BuildInfo.cs";

            /// <summary>
            ///     What should be used to denote arguments in the command line?
            /// </summary>
            public const string DeveloperCommandLineParserArgumentPrefix = "--";

            /// <summary>
            ///     What should be used to split arguments from their values in the command line?
            /// </summary>
            public const string DeveloperCommandLineParserArgumentSplit = "=";

            /// <summary>
            ///     Should a GDX scripting define symbol be added to all target build groups.
            /// </summary>
            public const bool EnvironmentScriptingDefineSymbol = false;

            /// <summary>
            ///     The language to set the default thread culture too.
            /// </summary>
            public const Localization.Language LocalizationDefaultCulture = Localization.Language.English;

            /// <summary>
            ///     Should the default thread culture be set?
            /// </summary>
            public const bool LocalizationSetDefaultCulture = true;

            /// <summary>
            ///     The project relative path where automation should store its artifacts.
            /// </summary>
            public const string PlatformAutomationFolder = "GDX_Automation";

            /// <summary>
            ///     The project relative path to use as a cache.
            /// </summary>
            public const string PlatformCacheFolder = "GDX_Cache";

            /// <summary>
            ///     What is the level of traces which should be processed and logged by GDX in debug builds?
            /// </summary>
            public const Trace.TraceLevel TraceDebugLevels = Trace.TraceLevel.Warning |
                                                             Trace.TraceLevel.Assertion |
                                                             Trace.TraceLevel.Error |
                                                             Trace.TraceLevel.Exception;

            /// <summary>
            ///     Should GDX based traces output to the Unity console in debug builds?
            /// </summary>
            public const bool TraceDebugOutputToUnityConsole = true;

            /// <summary>
            ///     What is the level of traces which should be processed and logged by GDX in the editor or development builds?
            /// </summary>
            public const Trace.TraceLevel TraceDevelopmentLevels = Trace.TraceLevel.Log |
                                                                   Trace.TraceLevel.Warning |
                                                                   Trace.TraceLevel.Error |
                                                                   Trace.TraceLevel.Exception |
                                                                   Trace.TraceLevel.Assertion |
                                                                   Trace.TraceLevel.Fatal;

            /// <summary>
            ///     Should GDX based traces output to the Unity console in the editor or development builds?
            /// </summary>
            public const bool TraceDevelopmentOutputToUnityConsole = true;

            /// <summary>
            ///     What is the level of traces which should be processed and logged by GDX in release builds?
            /// </summary>
            public const Trace.TraceLevel TraceReleaseLevels = Trace.TraceLevel.Fatal;

            /// <summary>
            ///     Should GDX check for updates at editor time?
            /// </summary>
            public const bool UpdateProviderCheckForUpdates = true;
    }
}