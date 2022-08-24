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
            code.AppendLine($"public const int FormatVersion = {Config.FormatVersion};");
            code.AppendLine();
            code.AppendLine("[UnityEngine.Scripting.Preserve]");
            code.AppendLine($"public static void {Core.OverrideMethod}()");
            code.PushIndent();

            Type config = Reflection.GetType("GDX.Config");

            // Cache the field list so that we can quickly grab parts
            FieldInfo[] configFields = config.GetFields();

            // You can think of these as transfer functions, we have to be explicit, but I've created as much
            // boilerplate around it that its really easy.
            AddToGenerator(code, configFields, nameof(Config.ConfigOutputPath), rhs.ConfigOutputPath);

            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoAssemblyDefinition),
                rhs.DeveloperBuildInfoAssemblyDefinition);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoBuildChangelistArgument),
                rhs.DeveloperBuildInfoBuildChangelistArgument);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoBuildDescriptionArgument),
                rhs.DeveloperBuildInfoBuildDescriptionArgument);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoBuildNumberArgument),
                rhs.DeveloperBuildInfoBuildNumberArgument);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoBuildStreamArgument),
                rhs.DeveloperBuildInfoBuildStreamArgument);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoBuildTaskArgument),
                rhs.DeveloperBuildInfoBuildTaskArgument);
            AddToGenerator(code,configFields, nameof(Config.DeveloperBuildInfoEnabled),
                rhs.DeveloperBuildInfoEnabled);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoNamespace),
                rhs.DeveloperBuildInfoNamespace);
            AddToGenerator(code, configFields, nameof(Config.DeveloperBuildInfoPath),
                rhs.DeveloperBuildInfoPath);

            AddToGenerator(code, configFields, nameof(Config.DeveloperCommandLineParserArgumentPrefix),
                rhs.DeveloperCommandLineParserArgumentPrefix);
            AddToGenerator(code, configFields, nameof(Config.DeveloperCommandLineParserArgumentSplit),
                rhs.DeveloperCommandLineParserArgumentSplit);

            AddToGenerator(code, configFields, nameof(Config.EnvironmentAlwaysIncludeShaders),
                rhs.EnvironmentAlwaysIncludeShaders);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentScriptingDefineSymbol),
                rhs.EnvironmentScriptingDefineSymbol);


            AddToGenerator(code, configFields, nameof(Config.EnvironmentEditorTaskDirector),
                rhs.EnvironmentEditorTaskDirector);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentEditorTaskDirectorTickRate),
                rhs.EnvironmentEditorTaskDirectorTickRate);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentTaskDirector),
                rhs.EnvironmentTaskDirector);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentTaskDirectorTickRate),
                rhs.EnvironmentTaskDirectorTickRate);



            AddToGenerator(code, configFields, nameof(Config.LocalizationDefaultCulture),
                rhs.LocalizationDefaultCulture);
            AddToGenerator(code, configFields, nameof(Config.LocalizationSetDefaultCulture),
                rhs.LocalizationSetDefaultCulture);

            AddToGenerator(code, configFields, nameof(Config.PlatformAutomationFolder),
                rhs.PlatformAutomationFolder);
            AddToGenerator(code, configFields, nameof(Config.PlatformCacheFolder),
                rhs.PlatformCacheFolder);

            AddToGenerator(code, configFields, nameof(Config.TraceDebugLevels),
                rhs.TraceDebugLevels);
            AddToGenerator(code, configFields, nameof(Config.TraceDebugOutputToUnityConsole),
                rhs.TraceDebugOutputToUnityConsole);
            AddToGenerator(code, configFields, nameof(Config.TraceDevelopmentLevels),
                rhs.TraceDevelopmentLevels);
            AddToGenerator(code, configFields, nameof(Config.TraceDevelopmentOutputToUnityConsole),
                rhs.TraceDevelopmentOutputToUnityConsole);
            AddToGenerator(code, configFields, nameof(Config.TraceReleaseLevels),
                rhs.TraceReleaseLevels);

            AddToGenerator(code, configFields, nameof(Config.UpdateProviderCheckForUpdates),
                rhs.UpdateProviderCheckForUpdates);

            return code.ToString();
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, bool rhs)
        {
            bool lhs = OriginalValueAttribute.GetValue<bool>(GetFirstFieldInfoByNameContains(configFields, member));
            if (lhs == rhs) return;
            code.AppendLine(rhs ? $"{k_CoreConfigPath}.{member} = true;" : $"{k_CoreConfigPath}.{member} = false;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, double rhs)
        {
            double lhs = OriginalValueAttribute.GetValue<double>(GetFirstFieldInfoByNameContains(configFields, member));
            if (Math.Abs(lhs - rhs) < Platform.DoubleTolerance) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = {rhs}d;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, float rhs)
        {
            float lhs = OriginalValueAttribute.GetValue<float>(GetFirstFieldInfoByNameContains(configFields, member));
            if (Math.Abs(lhs - rhs) < Platform.DoubleTolerance) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = {rhs}f;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, string rhs)
        {
            string lhs = OriginalValueAttribute.GetValue<string>(GetFirstFieldInfoByNameContains(configFields, member));
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = \"{rhs}\";");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, Trace.TraceLevel rhs)
        {
            Trace.TraceLevel lhs = OriginalValueAttribute.GetValue<Trace.TraceLevel>(
                GetFirstFieldInfoByNameContains(configFields, member));

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

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, Localization.Language rhs)
        {
            Localization.Language lhs = OriginalValueAttribute.GetValue<Localization.Language>(
                GetFirstFieldInfoByNameContains(configFields, member));

            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }

        static FieldInfo GetFirstFieldInfoByNameContains(FieldInfo[] configFields, string name)
        {
            int count = configFields.Length;
            for (int i = 0; i < count; i++)
            {
                if (configFields[i].Name.Contains(name))
                {
                    return configFields[i];
                }
            }
            return null;
        }
    }
}