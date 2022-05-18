// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace GDX.Editor
{
    public static class ConfigGenerator
    {
        const string k_CoreConfigPath = "GDX.Config";
        public static string Build(TransientConfig rhs)
        {
            Developer.TextGenerator code = new Developer.TextGenerator("    ", "{", "}");

            // Start header
            code.AppendLine("// Generated file of difference from default config.");
            code.AppendLine("#pragma warning disable");
            code.AppendLine("// ReSharper disable All");
            code.AppendLine("namespace GDX");
            code.PushIndent();
            code.AppendLine($"public class {Core.OverrideClass}");
            code.PushIndent();
            code.AppendLine("[UnityEngine.Scripting.Preserve]");
            code.AppendLine($"public static void {Core.OverrideMethod}()");
            code.PushIndent();

            AddToGenerator(code, "ConfigOutputPath", Config.k_ConfigOutputPathDefault, rhs.ConfigOutputPath);
            AddToGenerator(code, "DeveloperBuildInfoAssemblyDefinition",
                Config.k_DeveloperBuildInfoAssemblyDefinitionDefault, rhs.DeveloperBuildInfoAssemblyDefinition);
            AddToGenerator(code, "DeveloperBuildInfoBuildChangelistArgument",
                Config.k_DeveloperBuildInfoBuildChangelistArgumentDefault, rhs.DeveloperBuildInfoBuildChangelistArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildDescriptionArgument",
                Config.k_DeveloperBuildInfoBuildDescriptionArgumentDefault, rhs.DeveloperBuildInfoBuildDescriptionArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildNumberArgument",
                Config.k_DeveloperBuildInfoBuildNumberArgumentDefault, rhs.DeveloperBuildInfoBuildNumberArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildStreamArgument",
                Config.k_DeveloperBuildInfoBuildStreamArgumentDefault, rhs.DeveloperBuildInfoBuildStreamArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildTaskArgument",
                Config.k_DeveloperBuildInfoBuildTaskArgumentDefault, rhs.DeveloperBuildInfoBuildTaskArgument);
            AddToGenerator(code, "DeveloperBuildInfoEnabled",
                Config.k_DeveloperBuildInfoEnabledDefault, rhs.DeveloperBuildInfoEnabled);
            AddToGenerator(code, "DeveloperBuildInfoNamespace",
                Config.k_DeveloperBuildInfoNamespaceDefault, rhs.DeveloperBuildInfoNamespace);
            AddToGenerator(code, "DeveloperBuildInfoPath",
                Config.k_DeveloperBuildInfoPathDefault, rhs.DeveloperBuildInfoPath);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentPrefix",
                Config.k_DeveloperCommandLineParserArgumentPrefixDefault, rhs.DeveloperCommandLineParserArgumentPrefix);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentSplit",
                Config.k_DeveloperCommandLineParserArgumentSplitDefault, rhs.DeveloperCommandLineParserArgumentSplit);
            AddToGenerator(code, "EnvironmentScriptingDefineSymbol",
                Config.k_EnvironmentScriptingDefineSymbolDefault, rhs.EnvironmentScriptingDefineSymbol);
            AddToGenerator(code, "LocalizationDefaultCulture",
                Config.k_LocalizationDefaultCultureDefault, rhs.LocalizationDefaultCulture);
            AddToGenerator(code, "LocalizationSetDefaultCulture",
                Config.k_LocalizationSetDefaultCultureDefault, rhs.LocalizationSetDefaultCulture);
            AddToGenerator(code, "PlatformAutomationFolder",
                Config.k_PlatformAutomationFolderDefault, rhs.PlatformAutomationFolder);
            AddToGenerator(code, "PlatformCacheFolder", Config.k_PlatformCacheFolderDefault, rhs.PlatformCacheFolder);
            AddToGenerator(code, "TraceDebugLevels", Config.k_TraceDebugLevelsDefault, rhs.TraceDebugLevels);
            AddToGenerator(code, "TraceDebugOutputToUnityConsole",
                Config.k_TraceDebugOutputToUnityConsoleDefault, rhs.TraceDebugOutputToUnityConsole);
            AddToGenerator(code, "TraceDevelopmentLevels",
                Config.k_TraceDevelopmentLevelsDefault, rhs.TraceDevelopmentLevels);
            AddToGenerator(code, "TraceDevelopmentOutputToUnityConsole",
                Config.k_TraceDevelopmentOutputToUnityConsoleDefault, rhs.TraceDevelopmentOutputToUnityConsole);
            AddToGenerator(code, "TraceReleaseLevels", Config.k_TraceReleaseLevelsDefault, rhs.TraceReleaseLevels);
            AddToGenerator(code, "UpdateProviderCheckForUpdates",
                Config.k_UpdateProviderCheckForUpdatesDefault, rhs.UpdateProviderCheckForUpdates);

            return code.ToString();
        }

        static void AddToGenerator(Developer.TextGenerator code, string member, bool lhs, bool rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine(rhs ? $"{k_CoreConfigPath}.{member} = true;" : $"{k_CoreConfigPath}.{member} = false;");
        }

        static void AddToGenerator(Developer.TextGenerator code, string member, string lhs, string rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = \"{rhs}\";");
        }

        static void AddToGenerator(Developer.TextGenerator code, string member, Trace.TraceLevel lhs, Trace.TraceLevel rhs)
        {
            if (lhs == rhs) return;

            StringBuilder maskBuilder = new StringBuilder();
            if (rhs.HasFlags(Trace.TraceLevel.Info))
            {
                maskBuilder.Append(" Trace.TraceLevel.Info |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Log))
            {
                maskBuilder.Append(" Trace.TraceLevel.Log |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Warning))
            {
                maskBuilder.Append(" Trace.TraceLevel.Warning |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Error))
            {
                maskBuilder.Append(" Trace.TraceLevel.Error |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Exception))
            {
                maskBuilder.Append(" Trace.TraceLevel.Exception |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Assertion))
            {
                maskBuilder.Append(" Trace.TraceLevel.Assertion |");
            }
            if (rhs.HasFlags(Trace.TraceLevel.Fatal))
            {
                maskBuilder.Append(" Trace.TraceLevel.Fatal |");
            }

            string masks = maskBuilder.ToString().Trim();
            code.AppendLine($"{k_CoreConfigPath}.{member} = {masks.Substring(0,masks.Length - 2)};");
        }

        static void AddToGenerator(Developer.TextGenerator code, string member, Localization.Language lhs, Localization.Language rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }
    }
}