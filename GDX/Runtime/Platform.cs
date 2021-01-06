// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace GDX
{
    /// <summary>
    ///     A collection of platform related helper utilities.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        ///     Gets the current platforms hardware generation number?
        /// </summary>
        /// <remarks>This can be used to determine enhanced/upgraded status.</remarks>
        /// <returns>true/false</returns>
        public static int GetHardwareGeneration()
        {
#if UNITY_XBOXONE && !UNITY_EDITOR
            if (Hardware.version == HardwareVersion.XboxOneX_Devkit ||
                Hardware.version == HardwareVersion.XboxOneX_Retail)
            {
                return 1;
            }
            return 0;
#elif UNITY_PS4 && !UNITY_EDITOR
            return 1;
#else
            return 0;
#endif
        }

        /// <summary>
        /// Validate that all parent directories are created for a given <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to process and validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureFolderHierarchyExists(string path)
        {
            string targetDirectory = System.IO.Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                System.IO.Directory.CreateDirectory(targetDirectory);
            }
        }
    }
}