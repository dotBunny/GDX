// Copyright (c) 2020-2023 dotBunny Inc.
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

            AddToGenerator(code,configFields, nameof(Config.BuildInfo),
                rhs.BuildInfo);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoAssemblyDefinition),
                rhs.BuildInfoAssemblyDefinition);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoBuildChangelistArgument),
                rhs.BuildInfoBuildChangelistArgument);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoBuildDescriptionArgument),
                rhs.BuildInfoBuildDescriptionArgument);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoBuildNumberArgument),
                rhs.BuildInfoBuildNumberArgument);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoBuildStreamArgument),
                rhs.BuildInfoBuildStreamArgument);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoBuildTaskArgument),
                rhs.BuildInfoBuildTaskArgument);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoNamespace),
                rhs.BuildInfoNamespace);
            AddToGenerator(code, configFields, nameof(Config.BuildInfoOutputPath),
                rhs.BuildInfoOutputPath);

            AddToGenerator(code, configFields, nameof(Config.CommandLineParserArgumentPrefix),
                rhs.CommandLineParserArgumentPrefix);
            AddToGenerator(code, configFields, nameof(Config.CommandLineParserArgumentSplit),
                rhs.CommandLineParserArgumentSplit);

            AddToGenerator(code, configFields, nameof(Config.EditorTaskDirectorSystem),
                rhs.EditorTaskDirectorSystem);
            AddToGenerator(code, configFields, nameof(Config.EditorTaskDirectorSystemTickRate),
                rhs.EditorTaskDirectorSystemTickRate);

            AddToGenerator(code, configFields, nameof(Config.EnvironmentAlwaysIncludeShaders),
                rhs.EnvironmentAlwaysIncludeShaders);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentScriptingDefineSymbol),
                rhs.EnvironmentScriptingDefineSymbol);
            AddToGenerator(code, configFields, nameof(Config.EnvironmentToolsMenu),
                rhs.EnvironmentToolsMenu);

            AddToGenerator(code, configFields, nameof(Config.TaskDirectorSystem),
                rhs.TaskDirectorSystem);
            AddToGenerator(code, configFields, nameof(Config.TaskDirectorSystemTickRate),
                rhs.TaskDirectorSystemTickRate);

            AddToGenerator(code, configFields, nameof(Config.LocalizationDefaultCulture),
                rhs.LocalizationDefaultCulture);
            AddToGenerator(code, configFields, nameof(Config.LocalizationSetDefaultCulture),
                rhs.LocalizationSetDefaultCulture);

            AddToGenerator(code, configFields, nameof(Config.PlatformAutomationFolder),
                rhs.PlatformAutomationFolder);
            AddToGenerator(code, configFields, nameof(Config.PlatformCacheFolder),
                rhs.PlatformCacheFolder);

            // AddToGenerator(code, configFields, nameof(Config.TraceDebugLevels),
            //     rhs.TraceDebugLevels);
            // AddToGenerator(code, configFields, nameof(Config.TraceDebugOutputToUnityConsole),
            //     rhs.TraceDebugOutputToUnityConsole);
            // AddToGenerator(code, configFields, nameof(Config.TraceDevelopmentLevels),
            //     rhs.TraceDevelopmentLevels);
            // AddToGenerator(code, configFields, nameof(Config.TraceDevelopmentOutputToUnityConsole),
            //     rhs.TraceDevelopmentOutputToUnityConsole);
            // AddToGenerator(code, configFields, nameof(Config.TraceReleaseLevels),
            //     rhs.TraceReleaseLevels);

            AddToGenerator(code, configFields, nameof(Config.UpdateProviderCheckForUpdates),
                rhs.UpdateProviderCheckForUpdates);

            return code.ToString();
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, bool rhs)
        {
            bool lhs = OriginalValueAttribute.GetValue<bool>(GetFirstFieldInfoByName(configFields, member));
            if (lhs == rhs) return;
            code.AppendLine(rhs ? $"{k_CoreConfigPath}.{member} = true;" : $"{k_CoreConfigPath}.{member} = false;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, double rhs)
        {
            double lhs = OriginalValueAttribute.GetValue<double>(GetFirstFieldInfoByName(configFields, member));
            if (Math.Abs(lhs - rhs) < Platform.DoubleTolerance) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = {rhs}d;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, float rhs)
        {
            float lhs = OriginalValueAttribute.GetValue<float>(GetFirstFieldInfoByName(configFields, member));
            if (Math.Abs(lhs - rhs) < Platform.DoubleTolerance) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = {rhs}f;");
        }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, string rhs)
        {
            string lhs = OriginalValueAttribute.GetValue<string>(GetFirstFieldInfoByName(configFields, member));
            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = \"{rhs}\";");
        }

        // static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, Trace.TraceLevel rhs)
        // {
        //     Trace.TraceLevel lhs = OriginalValueAttribute.GetValue<Trace.TraceLevel>(
        //         GetFirstFieldInfoByName(configFields, member));
        //
        //     if (lhs == rhs) return;
        //
        //     StringBuilder maskBuilder = new StringBuilder();
        //     if (rhs.HasFlags(Trace.TraceLevel.Info))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Info |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Log))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Log |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Warning))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Warning |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Error))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Error |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Exception))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Exception |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Assertion))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Assertion |");
        //     }
        //     if (rhs.HasFlags(Trace.TraceLevel.Fatal))
        //     {
        //         maskBuilder.Append(" Trace.TraceLevel.Fatal |");
        //     }
        //
        //     string masks = maskBuilder.ToString().Trim();
        //     code.AppendLine($"{k_CoreConfigPath}.{member} = {masks.Substring(0,masks.Length - 2)};");
        // }

        static void AddToGenerator(Developer.TextGenerator code, FieldInfo[] configFields, string member, Localization.Language rhs)
        {
            Localization.Language lhs = OriginalValueAttribute.GetValue<Localization.Language>(
                GetFirstFieldInfoByName(configFields, member));

            if (lhs == rhs) return;
            code.AppendLine($"{k_CoreConfigPath}.{member} = Localization.Language.{rhs.ToString()};");
        }

        static FieldInfo GetFirstFieldInfoByName(FieldInfo[] configFields, string name)
        {
            int count = configFields.Length;
            for (int i = 0; i < count; i++)
            {
                if (configFields[i].Name == name)
                {
                    return configFields[i];
                }
            }
            return null;
        }
    }
}