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
            code.AppendLine("// ReSharper disable All");
            code.AppendLine("#pragma warning disable");
            code.AppendLine("namespace GDX");
            code.PushIndent();
            code.AppendLine($"public class {Core.OverrideClass}");
            code.PushIndent();
            code.AppendLine("[UnityEngine.Scripting.Preserve]");
            code.AppendLine($"public static void {Core.OverrideMethod}()");
            code.PushIndent();

            AddToGenerator(code, "ConfigOutputPath",
                ConfigDefaults.ConfigOutputPath, rhs.ConfigOutputPath);
            AddToGenerator(code, "UpdateProviderCheckForUpdates",
                ConfigDefaults.UpdateProviderCheckForUpdates, rhs.UpdateProviderCheckForUpdates);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentPrefix",
                ConfigDefaults.DeveloperCommandLineParserArgumentPrefix, rhs.DeveloperCommandLineParserArgumentPrefix);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentSplit",
                ConfigDefaults.DeveloperCommandLineParserArgumentSplit, rhs.DeveloperCommandLineParserArgumentSplit);
            AddToGenerator(code, "DeveloperBuildInfoAssemblyDefinition",
                ConfigDefaults.DeveloperBuildInfoAssemblyDefinition, rhs.DeveloperBuildInfoAssemblyDefinition);
            AddToGenerator(code, "DeveloperBuildInfoEnabled",
                ConfigDefaults.DeveloperBuildInfoEnabled, rhs.DeveloperBuildInfoEnabled);
            AddToGenerator(code, "DeveloperBuildInfoPath",
                ConfigDefaults.DeveloperBuildInfoPath, rhs.DeveloperBuildInfoPath);
            AddToGenerator(code, "DeveloperBuildInfoNamespace",
                ConfigDefaults.DeveloperBuildInfoNamespace, rhs.DeveloperBuildInfoNamespace);
            AddToGenerator(code, "DeveloperBuildInfoBuildNumberArgument",
                ConfigDefaults.DeveloperBuildInfoBuildNumberArgument, rhs.DeveloperBuildInfoBuildNumberArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildDescriptionArgument",
                ConfigDefaults.DeveloperBuildInfoBuildDescriptionArgument, rhs.DeveloperBuildInfoBuildDescriptionArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildChangelistArgument",
                ConfigDefaults.DeveloperBuildInfoBuildChangelistArgument, rhs.DeveloperBuildInfoBuildChangelistArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildTaskArgument",
                ConfigDefaults.DeveloperBuildInfoBuildTaskArgument, rhs.DeveloperBuildInfoBuildTaskArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildStreamArgument",
                ConfigDefaults.DeveloperBuildInfoBuildStreamArgument, rhs.DeveloperBuildInfoBuildStreamArgument);
            AddToGenerator(code, "EnvironmentScriptingDefineSymbol",
                ConfigDefaults.EnvironmentScriptingDefineSymbol, rhs.EnvironmentScriptingDefineSymbol);

            AddToGenerator(code, "PlatformAutomationFolder",
                ConfigDefaults.PlatformAutomationFolder, rhs.PlatformAutomationFolder);
            AddToGenerator(code, "PlatformCacheFolder",
                ConfigDefaults.PlatformCacheFolder, rhs.PlatformCacheFolder);

            AddToGenerator(code, "TraceDevelopmentLevels",
                ConfigDefaults.TraceDevelopmentLevels, rhs.TraceDevelopmentLevels);
            AddToGenerator(code, "TraceDebugLevels",
                ConfigDefaults.TraceDebugLevels, rhs.TraceDebugLevels);
            AddToGenerator(code, "TraceReleaseLevels",
                ConfigDefaults.TraceReleaseLevels, rhs.TraceReleaseLevels);

            AddToGenerator(code, "TraceDevelopmentOutputToUnityConsole",
                ConfigDefaults.TraceDevelopmentOutputToUnityConsole, rhs.TraceDevelopmentOutputToUnityConsole);
            AddToGenerator(code, "TraceDebugOutputToUnityConsole",
                ConfigDefaults.TraceDebugOutputToUnityConsole, rhs.TraceDebugOutputToUnityConsole);
            AddToGenerator(code, "LocalizationSetDefaultCulture",
                ConfigDefaults.LocalizationSetDefaultCulture, rhs.LocalizationSetDefaultCulture);
            AddToGenerator(code, "LocalizationDefaultCulture",
                ConfigDefaults.LocalizationDefaultCulture, rhs.LocalizationDefaultCulture);

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