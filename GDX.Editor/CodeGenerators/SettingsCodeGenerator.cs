// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace GDX.Editor.CodeGenerators
{
    public static class SettingsCodeGenerator
    {
        private const string k_CoreConfigPath = "Core.Config";
        public static string Build(GDXConfig lhs, GDXConfig rhs)
        {
            CodeGenerator code = new CodeGenerator("GDX", "Generated file of difference from default config.");

            // Start header
            code.AppendLine($"public class {Core.OverrideClass}");
            code.OpenBrace();
            code.AppendLine($"public static void {Core.OverrideMethod}()");
            code.OpenBrace();

            GetOverrideBoolean(code, "UpdateProviderCheckForUpdates",
                lhs.UpdateProviderCheckForUpdates, rhs.UpdateProviderCheckForUpdates);
            GetOverrideString(code, "DeveloperCommandLineParserArgumentPrefix",
                lhs.DeveloperCommandLineParserArgumentPrefix, rhs.DeveloperCommandLineParserArgumentPrefix);
            GetOverrideString(code, "DeveloperCommandLineParserArgumentSplit",
                lhs.DeveloperCommandLineParserArgumentSplit, rhs.DeveloperCommandLineParserArgumentSplit);
            GetOverrideBoolean(code, "DeveloperBuildInfoAssemblyDefinition",
                lhs.DeveloperBuildInfoAssemblyDefinition, rhs.DeveloperBuildInfoAssemblyDefinition);
            GetOverrideBoolean(code, "DeveloperBuildInfoEnabled",
                lhs.DeveloperBuildInfoEnabled, rhs.DeveloperBuildInfoEnabled);
            GetOverrideString(code, "DeveloperBuildInfoPath",
                lhs.DeveloperBuildInfoPath, rhs.DeveloperBuildInfoPath);
            GetOverrideString(code, "DeveloperBuildInfoNamespace",
                lhs.DeveloperBuildInfoNamespace, rhs.DeveloperBuildInfoNamespace);
            GetOverrideString(code, "DeveloperBuildInfoBuildNumberArgument",
                lhs.DeveloperBuildInfoBuildNumberArgument, rhs.DeveloperBuildInfoBuildNumberArgument);
            GetOverrideString(code, "DeveloperBuildInfoBuildDescriptionArgument",
                lhs.DeveloperBuildInfoBuildDescriptionArgument, rhs.DeveloperBuildInfoBuildDescriptionArgument);
            GetOverrideString(code, "DeveloperBuildInfoBuildChangelistArgument",
                lhs.DeveloperBuildInfoBuildChangelistArgument, rhs.DeveloperBuildInfoBuildChangelistArgument);
            GetOverrideString(code, "DeveloperBuildInfoBuildTaskArgument",
                lhs.DeveloperBuildInfoBuildTaskArgument, rhs.DeveloperBuildInfoBuildTaskArgument);
            GetOverrideString(code, "DeveloperBuildInfoBuildStreamArgument",
                lhs.DeveloperBuildInfoBuildStreamArgument, rhs.DeveloperBuildInfoBuildStreamArgument);
            GetOverrideBoolean(code, "EnvironmentScriptingDefineSymbol",
                lhs.EnvironmentScriptingDefineSymbol, rhs.EnvironmentScriptingDefineSymbol);

            GetOverrideTraceLevel(code, "TraceDevelopmentLevels",
                lhs.TraceDevelopmentLevels, rhs.TraceDevelopmentLevels);
            GetOverrideTraceLevel(code, "TraceDebugLevels",
                lhs.TraceDebugLevels, rhs.TraceDebugLevels);
            GetOverrideTraceLevel(code, "TraceReleaseLevels",
                lhs.TraceReleaseLevels, rhs.TraceReleaseLevels);

            GetOverrideBoolean(code, "TraceDevelopmentOutputToUnityConsole",
                lhs.TraceDevelopmentOutputToUnityConsole, rhs.TraceDevelopmentOutputToUnityConsole);
            GetOverrideBoolean(code, "TraceDebugOutputToUnityConsole",
                lhs.TraceDebugOutputToUnityConsole, rhs.TraceDebugOutputToUnityConsole);
            GetOverrideBoolean(code, "LocalizationSetDefaultCulture",
                lhs.LocalizationSetDefaultCulture, rhs.LocalizationSetDefaultCulture);
            OverrideLocalizationLanguage(code, "LocalizationDefaultCulture",
                lhs.LocalizationDefaultCulture, rhs.LocalizationDefaultCulture);

            return code.ToString();

        }

        private static void GetOverrideBoolean(CodeGenerator code, string member, bool lhs, bool rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine(rhs ? $"{k_CoreConfigPath}.{member} = true;" : $"{k_CoreConfigPath}.{member} = false;");
        }

        private static void GetOverrideString(CodeGenerator code, string member, string lhs, string rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = \"{rhs}\";");
        }

        private static void GetOverrideTraceLevel(CodeGenerator code, string member, Trace.TraceLevel lhs, Trace.TraceLevel rhs)
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

        private static void OverrideLocalizationLanguage(CodeGenerator code, string member, Localization.Language lhs, Localization.Language rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }
    }
}