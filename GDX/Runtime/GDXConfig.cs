// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX
{
    /// <summary>
    /// Project-wide configuration which is available at runtime.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GDXConfig : ScriptableObject
    {
        /// <summary>
        /// Resource path at runtime.
        /// </summary>
        public const string ResourcesPath = "Resources/GDX.asset";

        /// <summary>
        /// Should GDX check for updates at editor time?
        /// </summary>
        public bool checkForUpdates;

        public static GDXConfig Get()
        {
            return Resources.Load<GDXConfig>(ResourcesPath);
        }

    }
}