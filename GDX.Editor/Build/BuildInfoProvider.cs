// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.Developer;
using UnityEditor;
using UnityEngine;
using TextGenerator = GDX.Developer.TextGenerator;

namespace GDX.Editor.Build
{
    /// <summary>
    ///     A set of tools to produce the content used for the <c>BuildInfo</c> generated file, as well as the surrounding
    ///     assembly definition, as well as an ability to reset the file.
    /// </summary>
    public static class BuildInfoProvider
    {
        /// <summary>
        ///     Create the content for the <c>BuildInfo</c> file based on provided information.
        /// </summary>
        /// <param name="forceDefaults">Should all default values be used instead?</param>
        /// <param name="internalDescription">An internally used description.</param>
        /// <returns>The files content.</returns>
        public static string GetContent(bool forceDefaults = false,
            string internalDescription = null)
        {
            TextGenerator code = new TextGenerator("    ", "{", "}");

            code.AppendLine($"namespace {Config.DeveloperBuildInfoNamespace}");
            code.PushIndent();
            code.AppendLine("/// <summary>");
            code.AppendLine("///     A collection of information providing further information as to the conditions present when the build was made.");
            code.AppendLine("/// </summary>");
            code.AppendLine("public static class BuildInfo");
            code.PushIndent();

            // BuildNumber
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The builds numerically incremented version.");
            code.AppendLine("/// </summary>");
            code.AppendLine("/// <remarks>");
            code.AppendLine("///     This may not be a shared build number across all build tasks.");
            code.AppendLine("/// </remarks>");
            code.ApplyIndent();
            code.Append("public const int BuildNumber = ");
            code.Append(!forceDefaults &&
                        CommandLineParser.Arguments.ContainsKey(Config.DeveloperBuildInfoBuildNumberArgument)
                ? CommandLineParser.Arguments[Config.DeveloperBuildInfoBuildNumberArgument]
                : "0");
            code.Append(";");
            code.NextLine();

            // Changelist
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The revision the workspace was at when the build was made.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const int Changelist = ");
            code.Append(!forceDefaults &&
                        CommandLineParser.Arguments.ContainsKey(Config.DeveloperBuildInfoBuildChangelistArgument)
                ? CommandLineParser.Arguments[Config.DeveloperBuildInfoBuildChangelistArgument]
                : "0");
            code.Append(";");
            code.NextLine();

            // BuildTask
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The specific build task used to create the build.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const string BuildTask = \"");
            code.Append(!forceDefaults &&
                        CommandLineParser.Arguments.ContainsKey(Config.DeveloperBuildInfoBuildTaskArgument)
                ? CommandLineParser.Arguments[Config.DeveloperBuildInfoBuildTaskArgument]
                : "N/A");
            code.Append("\";");
            code.NextLine();

            // Stream
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The version control stream which the build was built from.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const string Stream = \"");
            code.Append(!forceDefaults &&
                        CommandLineParser.Arguments.ContainsKey(Config.DeveloperBuildInfoBuildStreamArgument)
                ? CommandLineParser.Arguments[Config.DeveloperBuildInfoBuildStreamArgument]
                : "N/A");
            code.Append("\";");
            code.NextLine();

            // Build Description
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The passed in build description.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const string Description = \"");
            code.Append(!forceDefaults &&
                        CommandLineParser.Arguments.ContainsKey(
                            Config.DeveloperBuildInfoBuildDescriptionArgument)
                ? CommandLineParser.Arguments[Config.DeveloperBuildInfoBuildDescriptionArgument]
                : "N/A");
            code.Append("\";");
            code.NextLine();

            // Internal Description
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The internal description set through method invoke.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const string InternalDescription = \"");
            code.Append(!forceDefaults && !string.IsNullOrEmpty(internalDescription)
                ? internalDescription
                : "N/A");
            code.Append("\";");
            code.NextLine();

            // Timestamp
            code.AppendLine("/// <summary>");
            code.AppendLine("///     The date and time when the build was started.");
            code.AppendLine("/// </summary>");
            code.ApplyIndent();
            code.Append("public const string Timestamp = \"");
            code.Append(!forceDefaults
                ? DateTime.Now.ToString(Localization.Language.Default.GetTimestampFormat())
                : "N/A");
            code.Append("\";");
            code.NextLine();

            return code.ToString();
        }

        /// <summary>
        ///     Write default content to <c>BuildInfo</c> file.
        /// </summary>
        public static void WriteDefaultFile()
        {

            try
            {
                string path = Path.Combine(Application.dataPath, Config.DeveloperBuildInfoPath);
                Platform.EnsureFileFolderHierarchyExists(path);
                File.WriteAllText(path, GetContent(true));

                CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Trace.Output(Trace.TraceLevel.Warning, e);
            }
        }

        /// <summary>
        ///     Check if an assembly definition should be placed along side the written <c>BuildInfo</c> and write one.
        /// </summary>
        public static void CheckForAssemblyDefinition()
        {
            if (Config.DeveloperBuildInfoAssemblyDefinition)
            {
                return;
            }

            string assemblyDefinition = Path.Combine(
                Path.GetDirectoryName(Path.Combine(Application.dataPath, Config.DeveloperBuildInfoPath)) ??
                string.Empty,
                Config.DeveloperBuildInfoNamespace + ".asmdef");

            if (File.Exists(assemblyDefinition))
            {
                return;
            }

            TextGenerator asmDef = new TextGenerator("\t", "{", "}");
            asmDef.PushIndent();
            asmDef.AppendLine($"\"name\": \"{Config.DeveloperBuildInfoNamespace}\",");
            asmDef.AppendLine($"\"rootNamespace\": \"{Config.DeveloperBuildInfoNamespace}\",");
            asmDef.AppendLine("\"references\": [],");
            asmDef.AppendLine("\"includePlatforms\": [],");
            asmDef.AppendLine("\"excludePlatforms\": [],");
            asmDef.AppendLine("\"allowUnsafeCode\": [],");
            asmDef.AppendLine("\"overrideReferences\": [],");
            asmDef.AppendLine("\"precompiledReferences\": [],");
            asmDef.AppendLine("\"autoReferenced\": true,");
            asmDef.AppendLine("\"defineConstraints\": [],");
            asmDef.AppendLine("\"versionDefines\": [],");
            asmDef.AppendLine("\"noEngineReferences\": true");

            File.WriteAllText(assemblyDefinition, asmDef.ToString());
            AssetDatabase.ImportAsset("Assets/" +
                                      Path.GetDirectoryName(Config.DeveloperBuildInfoPath) + "/" +
                                      // ReSharper disable once StringLiteralTypo
                                      Config.DeveloperBuildInfoNamespace + ".asmdef");
        }
    }
}