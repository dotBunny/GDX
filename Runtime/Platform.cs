// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace GDX
{
    /// <summary>
    ///     A collection of platform related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Platform
    {
        /// <summary>
        ///     Validate that all directories are created for a given <paramref name="folderPath" />.
        /// </summary>
        /// <param name="folderPath">The path to process and validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureFolderHierarchyExists(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath))
            {
                Directory.CreateDirectory(folderPath);
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
        ///     Validate that the file path is writable, making the necessary folder structure and setting permissions.
        /// </summary>
        /// <param name="filePath">The absolute path to validate.</param>
        public static void EnsureFileWritable(this string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName != null)
            {
                string directoryPath = filePath.TrimEnd(fileName.ToCharArray());
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
            }
        }

        /// <summary>
        ///     Use our best attempt to remove a file at the designated <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The file path to remove forcefully.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForceDeleteFile(this string filePath)
        {
            if (File.Exists(filePath))
            {
                File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
                File.Delete(filePath);
            }
        }

        /// <summary>
        ///     Gets the current platforms hardware generation number?
        /// </summary>
        /// <remarks>Requires UnityEngine.CoreModule.dll to function correctly.</remarks>
        /// <returns>Returns 0 for base hardware, 1 for updates.</returns>
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
        /// Is it safe to write to the indicated <paramref name="filePath"/>?
        /// </summary>
        /// <param name="filePath">The file path to check if it can be written.</param>
        /// <returns>true/false if the path can be written too.</returns>
        public static bool IsFileWritable(this string filePath)
        {
            if (File.Exists(filePath))
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly ||
                    (attributes & FileAttributes.Offline) == FileAttributes.Offline)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Is the application focused?
        /// </summary>
        /// <remarks>
        ///     There are issues on some platforms with getting an accurate reading.
        /// </remarks>
        /// <returns>true/false if the application has focus.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        /// <summary>
        /// Is the application running in headless mode?.
        /// </summary>
        /// <remarks>Useful for detecting running a server.</remarks>
        /// <returns>true/false if the application is without an initialized graphics device.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHeadless()
        {
            return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
        }
    }
}