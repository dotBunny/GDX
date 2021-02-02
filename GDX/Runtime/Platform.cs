// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     A collection of platform related helper utilities.
    /// </summary>
    public static class Platform
    {
        /// <summary>
        ///     Validate that all directories are created for a given <paramref name="folderPath" />.
        /// </summary>
        /// <param name="folderPath">The path to process and validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureFolderHierarchyExists(string folderPath)
        {
            string targetDirectory = Path.GetDirectoryName(folderPath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
        }

        /// <summary>
        ///     Validate that all parent directories are created for a given <paramref name="filePath" />.
        /// </summary>
        /// <param name="filePath">The path to process and validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureFileFolderHierarchyExists(string filePath)
        {
            string targetDirectory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
        }

        /// <summary>
        ///     Gets the current platforms hardware generation number?
        /// </summary>
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        /// <returns>true/false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        ///     Is the application focused?
        /// </summary>
        /// <remarks>
        ///     There are issues on some platforms with getting an accurate reading.
        /// </remarks>
        /// <returns>true/false if the application has focus.</returns>
        public static bool IsFocused()
        {
#if UNITY_XBOXONE && !UNITY_EDITOR
            return !XboxOnePLM.AmConstrained();
#elif UNITY_PS4 && !UNITY_EDITOR
            return true;
#else
            return Application.isFocused;
#endif
        }
    }
}