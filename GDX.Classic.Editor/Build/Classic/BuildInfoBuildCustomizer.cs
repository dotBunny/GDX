// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor.Build.Classic
{

#if GDX_PLATFORMS
    /// <summary>
    ///     <para>
    ///         A customizer for the <c>ClassicBuildPipeline</c> that handles the generation of the <c>BuildInfo</c>.
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
    public class BuildInfoBuildCustomizer : Unity.Build.Classic.ClassicBuildPipelineCustomizer
    {
        /// <summary>
        ///     Cache if the provider/customizer actually was effective.
        /// </summary>
        private bool _enabled = false;

        /// <summary>
        ///     Restore the default <c>BuildInfo</c> after a build process finishes.
        /// </summary>
        ~BuildInfoBuildCustomizer()
        {
            if (_enabled)
            {
                BuildInfoProvider.WriteDefaultFile();
            }
        }


        /// <summary>
        ///     Writes out <c>BuildInfo</c> prior to build.
        /// </summary>
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
                File.WriteAllText(path, BuildInfoProvider.GetContent(config, false, Context.BuildConfigurationName));

                BuildInfoProvider.CheckForAssemblyDefinition();
            }
            catch (Exception e)
            {
                Trace.Output(Trace.TraceLevel.Warning, e);
            }
        }
    }
#endif
}