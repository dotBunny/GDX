// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Project-wide configuration which is available at runtime.
    /// </summary>
    /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
    // ReSharper disable once InconsistentNaming
    public class GDXConfig : ScriptableObject
    {
        /// <summary>
        ///     Resource path at runtime.
        /// </summary>
        public const string ResourcesPath = "GDX/GDXConfig.asset";

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        [InspectorLabel("Check For Updates")]
        public bool updateProviderCheckForUpdates = true;

        /// <summary>
        /// What should be used to denote arguments in the command line?
        /// </summary>
        [InspectorLabel("Argument Prefix")]
        public string developerCommandLineParserArgumentPrefix = "--";

        /// <summary>
        /// What should be used to split arguments from their values in the command line?
        /// </summary>
        [InspectorLabel("Argument Split")]
        public string developerCommandLineParserArgumentSplit = "=";

        /// <summary>
        /// Should the BuildInfo file be written during builds?
        /// </summary>
        [InspectorLabel("Output BuildInfo")]
        public bool developerBuildInfoEnabled = false;

        /// <summary>
        /// The path to output the BuildInfo file.
        /// </summary>
        [InspectorLabel("Output Path")]
        public string developerBuildInfoPath = "Generated/Build/BuildInfo.cs";

        /// <summary>
        /// The namespace where the BuildInfo should be placed.
        /// </summary>
        [InspectorLabel("Namespace")]
        public string developerBuildInfoNamespace = "Generated.Build";

        /// <summary>
        /// The argument key for the build number to be passed to the BuildInfoGenerator.
        /// </summary>
        [InspectorLabel("Number Argument")]
        public string developerBuildInfoBuildNumberArgument = "BUILD";

        /// <summary>
        /// The argument key for the build description to be passed to the BuildInfoGenerator.
        /// </summary>
        [InspectorLabel("Description Argument")]
        public string developerBuildInfoBuildDescriptionArgument = "BUILD_DESC";

        /// <summary>
        /// The argument key for the build's changelist to be passed to the BuildInfoGenerator.
        /// </summary>
        [InspectorLabel("Changelist Argument")]
        public string developerBuildInfoBuildChangelistArgument = "BUILD_CHANGELIST";

        /// <summary>
        /// The argument key for the build's task to be passed to the BuildInfoGenerator.
        /// </summary>
        [InspectorLabel("Task Argument")]
        public string developerBuildInfoBuildTaskArgument = "BUILD_TASK";

        /// <summary>
        /// The argument key for the build's stream to be passed to the BuildInfoGenerator.
        /// </summary>
        [InspectorLabel("Stream Argument")]
        public string developerBuildInfoBuildStreamArgument = "BUILD_STREAM";

        /// <summary>
        ///     Get a loaded instance of the <see cref="GDXConfig" /> from resources.
        /// </summary>
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        /// <returns>A instance of <see cref="GDXConfig" />.</returns>
        public static GDXConfig Get()
        {
            return Resources.Load<GDXConfig>(ResourcesPath);
        }
    }
}