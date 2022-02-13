// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace GDX.Editor.CodeGenerators
{
    public static class SettingsCodeGenerator
    {
        private const string CoreConfigPath = "Core.Config";
        public static string Build(GDXConfig lhs, GDXConfig rhs)
        {
            CodeGenerator code = new CodeGenerator("GDX", "Generated file of difference from default config.");

            // Start header
            code.AppendLine("public class GDXSettings");
            code.OpenBrace();

            code.AppendLine("#if UNITY_EDITOR");
            code.AppendLine("[UnityEditor.InitializeOnLoadMethod]");
            code.AppendLine("#endif");
            code.AppendLine("[UnityEngine.RuntimeInitializeOnLoadMethod(UnityEngine.RuntimeInitializeLoadType.AfterAssembliesLoaded)]");
            code.AppendLine("public static void Init()");
            code.OpenBrace();

            GetOverrideBoolean(code, "updateProviderCheckForUpdates",
                lhs.updateProviderCheckForUpdates, rhs.updateProviderCheckForUpdates);
            GetOverrideString(code, "developerCommandLineParserArgumentPrefix",
                lhs.developerCommandLineParserArgumentPrefix, rhs.developerCommandLineParserArgumentPrefix);
            GetOverrideString(code, "developerCommandLineParserArgumentSplit",
                lhs.developerCommandLineParserArgumentSplit, rhs.developerCommandLineParserArgumentSplit);
            GetOverrideBoolean(code, "developerBuildInfoAssemblyDefinition",
                lhs.developerBuildInfoAssemblyDefinition, rhs.developerBuildInfoAssemblyDefinition);
            GetOverrideBoolean(code, "developerBuildInfoEnabled",
                lhs.developerBuildInfoEnabled, rhs.developerBuildInfoEnabled);
            GetOverrideString(code, "developerBuildInfoPath",
                lhs.developerBuildInfoPath, rhs.developerBuildInfoPath);
            GetOverrideString(code, "developerBuildInfoNamespace",
                lhs.developerBuildInfoNamespace, rhs.developerBuildInfoNamespace);
            GetOverrideString(code, "developerBuildInfoBuildNumberArgument",
                lhs.developerBuildInfoBuildNumberArgument, rhs.developerBuildInfoBuildNumberArgument);
            GetOverrideString(code, "developerBuildInfoBuildDescriptionArgument",
                lhs.developerBuildInfoBuildDescriptionArgument, rhs.developerBuildInfoBuildDescriptionArgument);
            GetOverrideString(code, "developerBuildInfoBuildChangelistArgument",
                lhs.developerBuildInfoBuildChangelistArgument, rhs.developerBuildInfoBuildChangelistArgument);
            GetOverrideString(code, "developerBuildInfoBuildTaskArgument",
                lhs.developerBuildInfoBuildTaskArgument, rhs.developerBuildInfoBuildTaskArgument);
            GetOverrideString(code, "developerBuildInfoBuildStreamArgument",
                lhs.developerBuildInfoBuildStreamArgument, rhs.developerBuildInfoBuildStreamArgument);
            GetOverrideBoolean(code, "environmentScriptingDefineSymbol",
                lhs.environmentScriptingDefineSymbol, rhs.environmentScriptingDefineSymbol);

            GetOverrideTraceLevel(code, "traceDevelopmentLevels",
                lhs.traceDevelopmentLevels, rhs.traceDevelopmentLevels);
            GetOverrideTraceLevel(code, "traceDebugLevels",
                lhs.traceDebugLevels, rhs.traceDebugLevels);
            GetOverrideTraceLevel(code, "traceReleaseLevels",
                lhs.traceReleaseLevels, rhs.traceReleaseLevels);

            GetOverrideBoolean(code, "traceDevelopmentOutputToUnityConsole",
                lhs.traceDevelopmentOutputToUnityConsole, rhs.traceDevelopmentOutputToUnityConsole);
            GetOverrideBoolean(code, "traceDebugOutputToUnityConsole",
                lhs.traceDebugOutputToUnityConsole, rhs.traceDebugOutputToUnityConsole);
            GetOverrideBoolean(code, "localizationSetDefaultCulture",
                lhs.localizationSetDefaultCulture, rhs.localizationSetDefaultCulture);
            OverrideLocalizationLanguage(code, "localizationDefaultCulture",
                lhs.localizationDefaultCulture, rhs.localizationDefaultCulture);

            return code.ToString();

        }

        private static void GetOverrideBoolean(CodeGenerator code, string member, bool lhs, bool rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine(rhs ? $"{CoreConfigPath}.{member} = true;" : $"{CoreConfigPath}.{member} = false;");
        }

        private static void GetOverrideString(CodeGenerator code, string member, string lhs, string rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{CoreConfigPath}.{member} = \"{rhs}\";");
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
            code.AppendLine($"{CoreConfigPath}.{member} = {masks.Substring(0,masks.Length - 2)};");
        }

        private static void OverrideLocalizationLanguage(CodeGenerator code, string member, Localization.Language lhs, Localization.Language rhs)
        {
            if (lhs == rhs) return;
            code.AppendLine($"{CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }
    }
}