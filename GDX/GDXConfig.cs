// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{

        /// <summary>
        ///     Project-wide configuration configurable for runtime override.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public class GDXConfig
        {
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

            public GDXConfig()
            {

            }

            public GDXConfig(GDXConfig initialState)
            {
                updateProviderCheckForUpdates = initialState.updateProviderCheckForUpdates;
                developerCommandLineParserArgumentPrefix = initialState.developerCommandLineParserArgumentPrefix;
                developerCommandLineParserArgumentSplit = initialState.developerCommandLineParserArgumentSplit;
                developerBuildInfoAssemblyDefinition = initialState.developerBuildInfoAssemblyDefinition;
                developerBuildInfoEnabled = initialState.developerBuildInfoEnabled;
                developerBuildInfoPath = initialState.developerBuildInfoPath;
                developerBuildInfoNamespace = initialState.developerBuildInfoNamespace;
                developerBuildInfoBuildNumberArgument = initialState.developerBuildInfoBuildNumberArgument;
                developerBuildInfoBuildDescriptionArgument = initialState.developerBuildInfoBuildDescriptionArgument;
                developerBuildInfoBuildChangelistArgument = initialState.developerBuildInfoBuildChangelistArgument;
                developerBuildInfoBuildTaskArgument = initialState.developerBuildInfoBuildTaskArgument;
                developerBuildInfoBuildStreamArgument = initialState.developerBuildInfoBuildStreamArgument;
                environmentScriptingDefineSymbol = initialState.environmentScriptingDefineSymbol;
                traceDevelopmentLevels = initialState.traceDevelopmentLevels;
                traceDebugLevels = initialState.traceDebugLevels;
                traceReleaseLevels = initialState.traceReleaseLevels;
                traceDevelopmentOutputToUnityConsole = initialState.traceDevelopmentOutputToUnityConsole;
                traceDebugOutputToUnityConsole = initialState.traceDebugOutputToUnityConsole;
                localizationSetDefaultCulture = initialState.localizationSetDefaultCulture;
                localizationDefaultCulture = initialState.localizationDefaultCulture;
            }

            public bool Compare(GDXConfig rhs)
            {
                return updateProviderCheckForUpdates == rhs.updateProviderCheckForUpdates &&
                developerCommandLineParserArgumentPrefix == rhs.developerCommandLineParserArgumentPrefix &&
                developerCommandLineParserArgumentSplit == rhs.developerCommandLineParserArgumentSplit &&
                developerBuildInfoAssemblyDefinition == rhs.developerBuildInfoAssemblyDefinition &&
                developerBuildInfoEnabled == rhs.developerBuildInfoEnabled &&
                developerBuildInfoPath == rhs.developerBuildInfoPath &&
                developerBuildInfoNamespace == rhs.developerBuildInfoNamespace &&
                developerBuildInfoBuildNumberArgument == rhs.developerBuildInfoBuildNumberArgument &&
                developerBuildInfoBuildDescriptionArgument == rhs.developerBuildInfoBuildDescriptionArgument &&
                developerBuildInfoBuildChangelistArgument == rhs.developerBuildInfoBuildChangelistArgument &&
                developerBuildInfoBuildTaskArgument == rhs.developerBuildInfoBuildTaskArgument &&
                developerBuildInfoBuildStreamArgument == rhs.developerBuildInfoBuildStreamArgument &&
                environmentScriptingDefineSymbol == rhs.environmentScriptingDefineSymbol &&
                traceDevelopmentLevels == rhs.traceDevelopmentLevels &&
                traceDebugLevels == rhs.traceDebugLevels &&
                traceReleaseLevels == rhs.traceReleaseLevels &&
                traceDevelopmentOutputToUnityConsole == rhs.traceDevelopmentOutputToUnityConsole &&
                traceDebugOutputToUnityConsole == rhs.traceDebugOutputToUnityConsole &&
                localizationSetDefaultCulture == rhs.localizationSetDefaultCulture &&
                localizationDefaultCulture == rhs.localizationDefaultCulture;
            }

            public string GetGeneratedOverrideSource()
            {
                return null;
            }
        }
}