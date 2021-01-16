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