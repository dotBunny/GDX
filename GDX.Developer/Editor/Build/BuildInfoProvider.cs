// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
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
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}