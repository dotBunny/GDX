// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GDX.Editor.Build.Legacy
{
    /// <summary>
    ///     <para>
    ///         A <c>BuildInfo</c> process step for the legacy build pipeline in Unity.
    ///     </para>
    ///     <para>
    ///         During the build process a <c>BuildInfo</c> file will be generated containing information passed in
    ///         through commandline arguments (parsed by <see cref="GDX.Developer.CommandLineParser" />). These arguments and
    ///         their formats are configurable via the <see cref="GDXConfig" />.
    ///     </para>
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         After a build is finished, the <c>BuildInfo</c> will be reset to default values. This is intended to make sure
    ///         local builds have a specific marker.
    ///     </para>
    ///     <list type="table">
    ///         <listheader>
    ///             <term>Argument (Default Value)</term>
    ///             <description>Description</description>
    ///         </listheader>
    ///         <item>
    ///             <term>BUILD</term>
    ///             <description>The build number, accessible through <c>BuildInfo.BuildNumber</c>.</description>
    ///         </item>
    ///         <item>
    ///             <term>BUILD_DESC</term>
    ///             <description>
    ///                 A short description of the build (example: useful for identifying personal CI builds),
    ///                 accessible through <c>BuildInfo.Description</c>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>BUILD_CHANGELIST</term>
    ///             <description>
    ///                 The changelist which the build was made against, accessible through
    ///                 <c>BuildInfo.Changelist</c>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>BUILD_TASK</term>
    ///             <description>
    ///                 The name of the build script/task which was used to create the build (example:
    ///                 CODE-Main-Build-PS5-Development), accessible through <c>BuildInfo.BuildTask</c>.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <term>BUILD_STREAM</term>
    ///             <description>
    ///                 An indicator of what stream/branch that the build was build against from your chosen version
    ///                 control system, accessible through <c>BuildInfo.Stream</c>.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public class BuildInfoBuildProcessor :
#if !GDX_PLATFORMS
        IPreprocessBuildWithReport,
#endif
        IPostprocessBuildWithReport
    {
        /// <summary>
        ///     The priority for the processor to be executed, before defaults.
        /// </summary>
        /// <value>The numerical value used to sort callbacks, lowest to highest.</value>
        public int callbackOrder => -42;

        /// <summary>
        ///     Restores the default <c>BuildInfo</c> after a build process finishes.
        /// </summary>
        /// <param name="report">Build process reported information.</param>
        public void OnPostprocessBuild(BuildReport report)
        {
            GDXConfig config = GDXConfig.Get();
            if (config == null || !config.developerBuildInfoEnabled)
            {
                return;
            }

            BuildInfoProvider.WriteDefaultFile();
        }
#if !GDX_PLATFORMS
        /// <summary>
        ///     Writes out <c>BuildInfo</c> prior to build.
        /// </summary>
        /// <param name="report">Build process reported information.</param>
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
                File.WriteAllText(path, BuildInfoProvider.GetContent(config, false, "Legacy"));

                BuildInfoProvider.CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
#endif
    }
}