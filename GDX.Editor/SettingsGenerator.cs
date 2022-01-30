// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace GDX.Editor
{
    public static class SettingsGenerator
    {
        private const string CoreConfigPath = "Core.Config";
        public static string Build(GDXConfig lhs, GDXConfig rhs)
        {
            StringBuilder fileContent = new StringBuilder();

            // Start header
            fileContent.AppendLine("// Generated file of difference from default config.");
            fileContent.AppendLine("namespace GDX");
            fileContent.AppendLine("{");
            fileContent.AppendLine("\tpublic static class GDXSettings");
            fileContent.AppendLine("\t{");
            fileContent.AppendLine("#if UNITY_EDITOR");
            fileContent.AppendLine("\t\t[UnityEditor.InitializeOnLoadMethod]");
            fileContent.AppendLine("#endif");
            fileContent.AppendLine("\t\t[UnityEngine.RuntimeInitializeOnLoadMethod]");
            fileContent.AppendLine("\t\tpublic static void Init()");
            fileContent.AppendLine("\t\t{");

            GetOverrideBoolean(fileContent, "updateProviderCheckForUpdates",
                lhs.updateProviderCheckForUpdates, rhs.updateProviderCheckForUpdates);
            GetOverrideString(fileContent, "developerCommandLineParserArgumentPrefix",
                lhs.developerCommandLineParserArgumentPrefix, rhs.developerCommandLineParserArgumentPrefix);
            GetOverrideString(fileContent, "developerCommandLineParserArgumentSplit",
                lhs.developerCommandLineParserArgumentSplit, rhs.developerCommandLineParserArgumentSplit);
            GetOverrideBoolean(fileContent, "developerBuildInfoAssemblyDefinition",
                lhs.developerBuildInfoAssemblyDefinition, rhs.developerBuildInfoAssemblyDefinition);
            GetOverrideBoolean(fileContent, "developerBuildInfoEnabled",
                lhs.developerBuildInfoEnabled, rhs.developerBuildInfoEnabled);
            GetOverrideString(fileContent, "developerBuildInfoPath",
                lhs.developerBuildInfoPath, rhs.developerBuildInfoPath);
            GetOverrideString(fileContent, "developerBuildInfoNamespace",
                lhs.developerBuildInfoNamespace, rhs.developerBuildInfoNamespace);
            GetOverrideString(fileContent, "developerBuildInfoBuildNumberArgument",
                lhs.developerBuildInfoBuildNumberArgument, rhs.developerBuildInfoBuildNumberArgument);
            GetOverrideString(fileContent, "developerBuildInfoBuildDescriptionArgument",
                lhs.developerBuildInfoBuildDescriptionArgument, rhs.developerBuildInfoBuildDescriptionArgument);
            GetOverrideString(fileContent, "developerBuildInfoBuildChangelistArgument",
                lhs.developerBuildInfoBuildChangelistArgument, rhs.developerBuildInfoBuildChangelistArgument);
            GetOverrideString(fileContent, "developerBuildInfoBuildTaskArgument",
                lhs.developerBuildInfoBuildTaskArgument, rhs.developerBuildInfoBuildTaskArgument);
            GetOverrideString(fileContent, "developerBuildInfoBuildStreamArgument",
                lhs.developerBuildInfoBuildStreamArgument, rhs.developerBuildInfoBuildStreamArgument);
            GetOverrideBoolean(fileContent, "environmentScriptingDefineSymbol",
                lhs.environmentScriptingDefineSymbol, rhs.environmentScriptingDefineSymbol);

            GetOverrideTraceLevel(fileContent, "traceDevelopmentLevels",
                lhs.traceDevelopmentLevels, rhs.traceDevelopmentLevels);
            GetOverrideTraceLevel(fileContent, "traceDebugLevels",
                lhs.traceDebugLevels, rhs.traceDebugLevels);
            GetOverrideTraceLevel(fileContent, "traceReleaseLevels",
                lhs.traceReleaseLevels, rhs.traceReleaseLevels);

            GetOverrideBoolean(fileContent, "traceDevelopmentOutputToUnityConsole",
                lhs.traceDevelopmentOutputToUnityConsole, rhs.traceDevelopmentOutputToUnityConsole);
            GetOverrideBoolean(fileContent, "traceDebugOutputToUnityConsole",
                lhs.traceDebugOutputToUnityConsole, rhs.traceDebugOutputToUnityConsole);
            GetOverrideBoolean(fileContent, "localizationSetDefaultCulture",
                lhs.localizationSetDefaultCulture, rhs.localizationSetDefaultCulture);
            OverrideLocalizationLanguage(fileContent, "localizationDefaultCulture",
                lhs.localizationDefaultCulture, rhs.localizationDefaultCulture);

            // Finish file
            fileContent.AppendLine("\t\t}");
            fileContent.AppendLine("\t}");
            fileContent.AppendLine("}");
            return fileContent.ToString();

        }

        private static void GetOverrideBoolean(StringBuilder builder, string member, bool lhs, bool rhs)
        {
            if (lhs == rhs) return;
            builder.AppendLine(rhs ? $"\t\t\t{CoreConfigPath}.{member} = true;" : $"\t\t\t{CoreConfigPath}.{member} = false;");
        }

        private static void GetOverrideString(StringBuilder builder, string member, string lhs, string rhs)
        {
            if (lhs == rhs) return;
            builder.AppendLine($"\t\t\t{CoreConfigPath}.{member} = \"{rhs}\";");
        }

        private static void GetOverrideTraceLevel(StringBuilder builder, string member, Trace.TraceLevel lhs, Trace.TraceLevel rhs)
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
            builder.AppendLine($"\t\t\t{CoreConfigPath}.{member} = {masks.Substring(0,masks.Length - 2)};");
        }

        private static void OverrideLocalizationLanguage(StringBuilder builder, string member, Localization.Language lhs, Localization.Language rhs)
        {
            if (lhs == rhs) return;
            builder.AppendLine($"\t\t\t{CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }
    }
}