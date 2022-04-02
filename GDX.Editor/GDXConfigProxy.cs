// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Editor
{
    public class TransientConfig
    {
        public string ConfigOutputPath = GDXConfig.ConfigOutputPath;
        public bool DeveloperBuildInfoAssemblyDefinition = GDXConfig.DeveloperBuildInfoAssemblyDefinition;
        public string DeveloperBuildInfoBuildChangelistArgument = GDXConfig.DeveloperBuildInfoBuildChangelistArgument;
        public string DeveloperBuildInfoBuildDescriptionArgument = GDXConfig.DeveloperBuildInfoBuildDescriptionArgument;
        public string DeveloperBuildInfoBuildNumberArgument = GDXConfig.DeveloperBuildInfoBuildNumberArgument;
        public string DeveloperBuildInfoBuildStreamArgument = GDXConfig.DeveloperBuildInfoBuildStreamArgument;
        public string DeveloperBuildInfoBuildTaskArgument = GDXConfig.DeveloperBuildInfoBuildTaskArgument;
        public bool DeveloperBuildInfoEnabled = GDXConfig.DeveloperBuildInfoEnabled;
        public string DeveloperBuildInfoNamespace = GDXConfig.DeveloperBuildInfoNamespace;
        public string DeveloperBuildInfoPath = GDXConfig.DeveloperBuildInfoPath;
        public string DeveloperCommandLineParserArgumentPrefix = GDXConfig.DeveloperCommandLineParserArgumentPrefix;
        public string DeveloperCommandLineParserArgumentSplit = GDXConfig.DeveloperCommandLineParserArgumentSplit;
        public bool EnvironmentScriptingDefineSymbol = GDXConfig.EnvironmentScriptingDefineSymbol;
        public Localization.Language LocalizationDefaultCulture = GDXConfig.LocalizationDefaultCulture;
        public bool LocalizationSetDefaultCulture = GDXConfig.LocalizationSetDefaultCulture;
        public string PlatformAutomationFolder = GDXConfig.PlatformAutomationFolder;
        public string PlatformCacheFolder = GDXConfig.PlatformCacheFolder;
        public Trace.TraceLevel TraceDebugLevels = GDXConfig.TraceDebugLevels;
        public bool TraceDebugOutputToUnityConsole = GDXConfig.TraceDebugOutputToUnityConsole;
        public Trace.TraceLevel TraceDevelopmentLevels = GDXConfig.TraceDevelopmentLevels;
        public bool TraceDevelopmentOutputToUnityConsole = GDXConfig.TraceDevelopmentOutputToUnityConsole;
        public Trace.TraceLevel TraceReleaseLevels = GDXConfig.TraceReleaseLevels;
        public bool UpdateProviderCheckForUpdates = GDXConfig.UpdateProviderCheckForUpdates;

        public bool HasChanges()
        {
            return UpdateProviderCheckForUpdates == GDXConfig.UpdateProviderCheckForUpdates &&
                   DeveloperCommandLineParserArgumentPrefix == GDXConfig.DeveloperCommandLineParserArgumentPrefix &&
                   DeveloperCommandLineParserArgumentSplit == GDXConfig.DeveloperCommandLineParserArgumentSplit &&
                   DeveloperBuildInfoAssemblyDefinition == GDXConfig.DeveloperBuildInfoAssemblyDefinition &&
                   DeveloperBuildInfoEnabled == GDXConfig.DeveloperBuildInfoEnabled &&
                   DeveloperBuildInfoPath == GDXConfig.DeveloperBuildInfoPath &&
                   DeveloperBuildInfoNamespace == GDXConfig.DeveloperBuildInfoNamespace &&
                   DeveloperBuildInfoBuildNumberArgument == GDXConfig.DeveloperBuildInfoBuildNumberArgument &&
                   DeveloperBuildInfoBuildDescriptionArgument == GDXConfig.DeveloperBuildInfoBuildDescriptionArgument &&
                   DeveloperBuildInfoBuildChangelistArgument == GDXConfig.DeveloperBuildInfoBuildChangelistArgument &&
                   DeveloperBuildInfoBuildTaskArgument == GDXConfig.DeveloperBuildInfoBuildTaskArgument &&
                   DeveloperBuildInfoBuildStreamArgument == GDXConfig.DeveloperBuildInfoBuildStreamArgument &&
                   EnvironmentScriptingDefineSymbol == GDXConfig.EnvironmentScriptingDefineSymbol &&
                   TraceDevelopmentLevels == GDXConfig.TraceDevelopmentLevels &&
                   TraceDebugLevels == GDXConfig.TraceDebugLevels &&
                   TraceReleaseLevels == GDXConfig.TraceReleaseLevels &&
                   TraceDevelopmentOutputToUnityConsole == GDXConfig.TraceDevelopmentOutputToUnityConsole &&
                   TraceDebugOutputToUnityConsole == GDXConfig.TraceDebugOutputToUnityConsole &&
                   LocalizationSetDefaultCulture == GDXConfig.LocalizationSetDefaultCulture &&
                   LocalizationDefaultCulture == GDXConfig.LocalizationDefaultCulture &&
                   ConfigOutputPath == GDXConfig.ConfigOutputPath &&
                   PlatformAutomationFolder == GDXConfig.PlatformAutomationFolder &&
                   PlatformCacheFolder == GDXConfig.PlatformCacheFolder;
        }
    }
}