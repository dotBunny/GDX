// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
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

            Type config = Reflection.GetType("GDX.Config");

            // Cache the field list so that we can quickly grab parts
            FieldInfo[] configFields = config.GetFields();

            OriginalValueAttribute.GetValue<string>(configFields[2]);

            // The below indexes correspond to the field order of GDX.Config, if any changes happen there
            // for order, these need to be updated. Opted to use reflection in this spot because it has
            // very little performance impact and it avoided having to add another assembly visible tag.

            AddToGenerator(code, "ConfigOutputPath",
                OriginalValueAttribute.GetValue<string>(configFields[0]),
                rhs.ConfigOutputPath);
            AddToGenerator(code, "DeveloperBuildInfoAssemblyDefinition",
                OriginalValueAttribute.GetValue<bool>(configFields[1]),
                rhs.DeveloperBuildInfoAssemblyDefinition);
            AddToGenerator(code, "DeveloperBuildInfoBuildChangelistArgument",
                OriginalValueAttribute.GetValue<string>(configFields[2]),
                rhs.DeveloperBuildInfoBuildChangelistArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildDescriptionArgument",
                OriginalValueAttribute.GetValue<string>(configFields[3]),
                rhs.DeveloperBuildInfoBuildDescriptionArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildNumberArgument",
                OriginalValueAttribute.GetValue<string>(configFields[4]),
                rhs.DeveloperBuildInfoBuildNumberArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildStreamArgument",
                OriginalValueAttribute.GetValue<string>(configFields[5]),
                rhs.DeveloperBuildInfoBuildStreamArgument);
            AddToGenerator(code, "DeveloperBuildInfoBuildTaskArgument",
                OriginalValueAttribute.GetValue<string>(configFields[6]),
                rhs.DeveloperBuildInfoBuildTaskArgument);
            AddToGenerator(code, "DeveloperBuildInfoEnabled",
                OriginalValueAttribute.GetValue<bool>(configFields[7]),
                rhs.DeveloperBuildInfoEnabled);
            AddToGenerator(code, "DeveloperBuildInfoNamespace",
                OriginalValueAttribute.GetValue<string>(configFields[8]),
                rhs.DeveloperBuildInfoNamespace);
            AddToGenerator(code, "DeveloperBuildInfoPath",
                OriginalValueAttribute.GetValue<string>(configFields[9]),
                rhs.DeveloperBuildInfoPath);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentPrefix",
                OriginalValueAttribute.GetValue<string>(configFields[10]),
                rhs.DeveloperCommandLineParserArgumentPrefix);
            AddToGenerator(code, "DeveloperCommandLineParserArgumentSplit",
                OriginalValueAttribute.GetValue<string>(configFields[11]),
                rhs.DeveloperCommandLineParserArgumentSplit);
            AddToGenerator(code, "EnvironmentScriptingDefineSymbol",
                OriginalValueAttribute.GetValue<bool>(configFields[12]),
                rhs.EnvironmentScriptingDefineSymbol);
            AddToGenerator(code, "EnvironmentAlwaysIncludeShaders",
                OriginalValueAttribute.GetValue<bool>(configFields[13]),
                rhs.EnvironmentAlwaysIncludeShaders);
            AddToGenerator(code, "LocalizationDefaultCulture",
                OriginalValueAttribute.GetValue<Localization.Language>(configFields[14]),
                rhs.LocalizationDefaultCulture);
            AddToGenerator(code, "LocalizationSetDefaultCulture",
                OriginalValueAttribute.GetValue<bool>(configFields[15]),
                rhs.LocalizationSetDefaultCulture);
            AddToGenerator(code, "PlatformAutomationFolder",
                OriginalValueAttribute.GetValue<string>(configFields[16]),
                rhs.PlatformAutomationFolder);
            AddToGenerator(code, "PlatformCacheFolder",
                OriginalValueAttribute.GetValue<string>(configFields[17]),
                rhs.PlatformCacheFolder);
            AddToGenerator(code, "TraceDebugLevels",
                OriginalValueAttribute.GetValue<Trace.TraceLevel>(configFields[18]),
                rhs.TraceDebugLevels);
            AddToGenerator(code, "TraceDebugOutputToUnityConsole",
                OriginalValueAttribute.GetValue<bool>(configFields[19]),
                rhs.TraceDebugOutputToUnityConsole);
            AddToGenerator(code, "TraceDevelopmentLevels",
                OriginalValueAttribute.GetValue<Trace.TraceLevel>(configFields[20]),
                rhs.TraceDevelopmentLevels);
            AddToGenerator(code, "TraceDevelopmentOutputToUnityConsole",
                OriginalValueAttribute.GetValue<bool>(configFields[21]),
                rhs.TraceDevelopmentOutputToUnityConsole);
            AddToGenerator(code, "TraceReleaseLevels",
                OriginalValueAttribute.GetValue<Trace.TraceLevel>(configFields[22]),
                rhs.TraceReleaseLevels);
            AddToGenerator(code, "UpdateProviderCheckForUpdates",
                OriginalValueAttribute.GetValue<bool>(configFields[23]),
                rhs.UpdateProviderCheckForUpdates);

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