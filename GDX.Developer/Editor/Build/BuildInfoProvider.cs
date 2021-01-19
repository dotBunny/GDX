// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Developer.Editor.Build
{
#if GDX_PLATFORMS
    /// <summary>
    ///     A wrapper for the scriptable build system (platform package based), to create the BuildInfo
    ///     file during the build process, and clean it up afterwards.
    /// </summary>
    internal class BuildInfoProvider : ClassicBuildPipelineCustomizer
    {
        /// <summary>
        ///     Cache if the provider/customizer actually was effective.
        /// </summary>
        private bool _enabled = false;

        /// <summary>
        ///     Reverse the changes to the BuildInfo generated file.
        /// </summary>
        ~BuildInfoProvider()
        {
            if (_enabled)
            {
                WriteDefaultFile();
            }
        }

        /// <inheritdoc />
        public override void OnBeforeBuild()
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null || !config.developerBuildInfoEnabled)
            {
                return;
            }

            // Cache for destructor
            _enabled = true;

            try
            {
                string path = Path.Combine(Application.dataPath, config.developerBuildInfoPath);
                Platform.EnsureFileFolderHierarchyExists(path);
                File.WriteAllText(path), GetContent(config, false, Context.BuildConfigurationName));

                CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
#else
    /// <summary>
    ///     A wrapper for the legacy build system (non platform package based), to create the BuildInfo
    ///     file during the build process, and clean it up afterwards.
    /// </summary>
    internal class BuildInfoProvider : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        /// <inheritdoc />
        public int callbackOrder { get; }

        /// <inheritdoc />
        public void OnPostprocessBuild(BuildReport report)
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null || !config.developerBuildInfoEnabled)
            {
                return;
            }

            WriteDefaultFile();
        }

        /// <inheritdoc />
        public void OnPreprocessBuild(BuildReport report)
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null || !config.developerBuildInfoEnabled)
            {
                return;
            }

            try
            {
                string path = Path.Combine(Application.dataPath, config.developerBuildInfoPath);
                Platform.EnsureFileFolderHierarchyExists(path);
                File.WriteAllText(path, GetContent(config, false, "Legacy"));

                CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
#endif

        /// <summary>
        /// Create the content for the BuildInfo file based on provided information.
        /// </summary>
        /// <param name="config">A <see cref="GDXConfig"/> used to determine many of the keys for information.</param>
        /// <param name="forceDefaults">Should all default values be used instead?</param>
        /// <param name="internalDescription">An internally used description.</param>
        /// <returns>The files content.</returns>
        public static string GetContent(GDXConfig config, bool forceDefaults = false,
            string internalDescription = Strings.Null)
        {
            // Force the parse because this isn't a runtime thing
            if (!forceDefaults)
            {
                CommandLineParser.ParseArguments();
            }

            StringBuilder fileContent = new StringBuilder();

            fileContent.Append("namespace ");
            fileContent.Append(config.developerBuildInfoNamespace);
            fileContent.AppendLine();

            fileContent.AppendLine("{");

            fileContent.AppendLine("    /// <summary>");
            fileContent.AppendLine("    ///     A collection of information providing further information as to the conditions present when the build was made.");
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
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildNumberArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildNumberArgument]
                : "0");
            fileContent.AppendLine(";");

            // Changelist
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The revision the workspace was at when the build was made.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const int Changelist = ");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildChangelistArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildChangelistArgument]
                : "0");
            fileContent.AppendLine(";");


            // BuildTask
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The specific build task used to create the build.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string BuildTask = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildTaskArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildTaskArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Stream
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The version control stream which the build was built from.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Stream = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(config.developerBuildInfoBuildStreamArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildStreamArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Build Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The passed in build description.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string Description = \"");
            fileContent.Append(!forceDefaults &&
                               CommandLineParser.Arguments.ContainsKey(
                                   config.developerBuildInfoBuildDescriptionArgument)
                ? CommandLineParser.Arguments[config.developerBuildInfoBuildDescriptionArgument]
                : "N/A");
            fileContent.AppendLine("\";");

            // Internal Description
            fileContent.AppendLine("        /// <summary>");
            fileContent.AppendLine("        ///     The internal description set through method invoke.");
            fileContent.AppendLine("        /// </summary>");
            fileContent.Append("        public const string InternalDescription = \"");
            fileContent.Append(!forceDefaults && !string.IsNullOrEmpty(internalDescription)
                ? internalDescription : "N/A");
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
        ///     Write default content to BuildInfo file.
        /// </summary>
        public static void WriteDefaultFile()
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null) return;

            try
            {
                string path = Path.Combine(Application.dataPath, config.developerBuildInfoPath);
                Platform.EnsureFileFolderHierarchyExists(path);
                File.WriteAllText(path, GetContent(config, true));

                CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        /// <summary>
        ///     Check if an assembly definition should be placed along side the written BuildInfo and write one.
        /// </summary>
        public static void CheckForAssemblyDefinition()
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null || !config.developerBuildInfoAssemblyDefinition) return;

            string assemblyDefinition = Path.Combine(
                Path.GetDirectoryName(Path.Combine(Application.dataPath, config.developerBuildInfoPath)) ?? string.Empty,
                config.developerBuildInfoNamespace + ".asmdef");

            if (File.Exists(assemblyDefinition))
            {
                return;
            }

            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.AppendLine("{");
            fileBuilder.Append("\t\"name\": \"");
            fileBuilder.Append(config.developerBuildInfoNamespace);
            fileBuilder.AppendLine("\",");
            fileBuilder.Append("\t\"rootNamespace\": \"");
            fileBuilder.Append(config.developerBuildInfoNamespace);
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
                                      Path.GetDirectoryName(config.developerBuildInfoPath) + "/" +
                                      config.developerBuildInfoNamespace + ".asmdef" );

        }
    }
}