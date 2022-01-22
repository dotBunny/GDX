// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using GDX.Developer;
using UnityEditor;
using UnityEngine;

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
        /// <param name="config">A <see cref="Config" /> used to determine many of the keys for information.</param>
        /// <param name="forceDefaults">Should all default values be used instead?</param>
        /// <param name="internalDescription">An internally used description.</param>
        /// <returns>The files content.</returns>
        public static string GetContent(bool forceDefaults = false,
            string internalDescription = null)
        {
            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("namespace ");
            fileContent.Append(Core.Config.developerBuildInfoNamespace);
            fileContent.AppendLine();

            fileContent.AppendLine("{");

            fileContent.AppendLine("    /// <summary>");
            fileContent.AppendLine(
                "    ///     A collection of information providing further information as to the conditions present when the build was made.");
            fileContent.AppendLine("    /// </summary>");
            fileContent.AppendLine("    public static class BuildInfo");
            fileContent.AppendLine("    {");

            // BuildNumber
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The builds numerically incremented version.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.AppendLine("        /// <remarks>");
            fileContent.AppendLine("        ///     This may not be a shared build number across all build tasks.");
            fileContent.AppendLine("        /// </remarks>");
            fileContent.Append("        public const int BuildNumber = ");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(Core.Config.developerBuildInfoBuildNumberArgument)
                ? CommandLineParser.Arguments[Core.Config.developerBuildInfoBuildNumberArgument]
                : "0");
            fileContent.AppendLine(";");

            // Changelist
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The revision the workspace was at when the build was made.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const int Changelist = ");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(Core.Config.developerBuildInfoBuildChangelistArgument)
                ? CommandLineParser.Arguments[Core.Config.developerBuildInfoBuildChangelistArgument]
                : "0");
            fileContent.AppendLine(";");


            // BuildTask
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The specific build task used to create the build.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string BuildTask = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(Core.Config.developerBuildInfoBuildTaskArgument)
                ? CommandLineParser.Arguments[Core.Config.developerBuildInfoBuildTaskArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Stream
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The version control stream which the build was built from.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Stream = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(Core.Config.developerBuildInfoBuildStreamArgument)
                ? CommandLineParser.Arguments[Core.Config.developerBuildInfoBuildStreamArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Build Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The passed in build description.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Description = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(
                                   Core.Config.developerBuildInfoBuildDescriptionArgument)
                ? CommandLineParser.Arguments[Core.Config.developerBuildInfoBuildDescriptionArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Internal Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The internal description set through method invoke.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string InternalDescription = \"");
            fileContent.Append(!forceDefaults && !string.IsNullOrEmpty(internalDescription)
                ? internalDescription
                : "N/A");
            fileContent.AppendLine("\";");

            // Timestamp
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The date and time when the build was started.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Timestamp = \"");
            fileContent.Append(!forceDefaults
                ? DateTime.Now.ToString(Localization.Language.Default.GetTimestampFormat())
                : "N/A");
            fileContent.AppendLine("\";");

            fileContent.AppendLine("\t}");

            fileContent.AppendLine("}");

            return fileContent.ToString();
        }

        /// <summary>
        ///     Write default content to <c>BuildInfo</c> file.
        /// </summary>
        public static void WriteDefaultFile()
        {

            try
            {
                string path = Path.Combine(Application.dataPath, Core.Config.developerBuildInfoPath);
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
            if (Core.Config.developerBuildInfoAssemblyDefinition)
            {
                return;
            }

            string assemblyDefinition = Path.Combine(
                Path.GetDirectoryName(Path.Combine(Application.dataPath, Core.Config.developerBuildInfoPath)) ??
                string.Empty,
                Core.Config.developerBuildInfoNamespace + ".asmdef");

            if (File.Exists(assemblyDefinition))
            {
                return;
            }

            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.AppendLine("{");
            fileBuilder.Append("\t\"name\": \"");
            fileBuilder.Append(Core.Config.developerBuildInfoNamespace);
            fileBuilder.AppendLine("\",");
            fileBuilder.Append("\t\"rootNamespace\": \"");
            fileBuilder.Append(Core.Config.developerBuildInfoNamespace);
            fileBuilder.AppendLine("\",");
            fileBuilder.AppendLine("\t\"references\": [],");
            fileBuilder.AppendLine("\t\"includePlatforms\": [],");
            fileBuilder.AppendLine("\t\"excludePlatforms\": [],");
            fileBuilder.AppendLine("\t\"allowUnsafeCode\": false,");
            fileBuilder.AppendLine("\t\"overrideReferences\": false,");
            fileBuilder.AppendLine("\t\"precompiledReferences\": [],");
            fileBuilder.AppendLine("\t\"autoReferenced\": true,");
            fileBuilder.AppendLine("\t\"defineConstraints\": [],");
            fileBuilder.AppendLine("\t\"versionDefines\": [],");
            fileBuilder.AppendLine("\t\"noEngineReferences\": true");
            fileBuilder.AppendLine("}");

            File.WriteAllText(assemblyDefinition, fileBuilder.ToString());
            AssetDatabase.ImportAsset("Assets/" +
                                      Path.GetDirectoryName(Core.Config.developerBuildInfoPath) + "/" +
                                      Core.Config.developerBuildInfoNamespace + ".asmdef");
        }
    }
}