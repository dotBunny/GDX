// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Project-wide configuration which is available at runtime.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GDXConfig : ScriptableObject
    {
        /// <summary>
        ///     Resource path at runtime.
        /// </summary>
        public const string ResourcesPath = "Resources/GDX/GDXConfig.asset";

        /// <summary>
        ///     Should GDX check for updates at editor time?
        /// </summary>
        public bool checkForUpdates = true;

        /// <summary>
        ///     Get a loaded instance of the <see cref="GDXConfig" /> from resources.
        /// </summary>
        /// <returns>A instance of <see cref="GDXConfig" />.</returns>
        public static GDXConfig Get()
        {
            return Resources.Load<GDXConfig>(ResourcesPath);
        }
    }
}