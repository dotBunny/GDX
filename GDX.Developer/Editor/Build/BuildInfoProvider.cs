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
                File.WriteAllText(path), BuildInfoGenerator.GetContent(config, false, Context.BuildConfigurationName));

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
                File.WriteAllText(path, BuildInfoGenerator.GetContent(config, false, "Legacy"));

                CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
#endif
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
                File.WriteAllText(path, BuildInfoGenerator.GetContent(config, true));

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