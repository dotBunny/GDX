// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.CompilerServices;
using GDX.Mathematics.Random;

// ReSharper disable UnusedMember.Global

namespace GDX
{
    /// <summary>
    ///     A collection of platform related helper utilities.
    /// </summary>
    [VisualScriptingCompatible(8)]
    public static class Platform
    {
        public const float ImageCompareTolerance = 0.99f;
        public const float FloatTolerance = 0.000001f;
        public const double DoubleTolerance = 0.000001d;
        public const string SafeCharacterPool = "abcdefghijklmnopqrstuvwxyz";
        public const int CharacterPoolLength = 25;
        public const int CharacterPoolLengthExclusive = 24;
        /// <summary>
        ///     A filename safe version of the timestamp format.
        /// </summary>
        public const string FilenameTimestampFormat = "yyyyMMdd_HHmmss";

        private static string s_outputFolder;

        public static char GetRandomSafeCharacter(IRandomProvider random)
        {
            return SafeCharacterPool[random.NextInteger(0, CharacterPoolLengthExclusive)];
        }

        /// <summary>
        ///     Returns a runtime writable folder.
        /// </summary>
        /// <returns>The full path to a writable folder at runtime.</returns>
        public static string GetOutputFolder()
        {
            if (s_outputFolder == null)
            {
#if UNITY_EDITOR
                s_outputFolder = Path.Combine(UnityEngine.Application.dataPath, "..", "GDX");
#elif UNITY_DOTSRUNTIME
                s_outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "GDX");
#else
                s_outputFolder = UnityEngine.Application.persistentDataPath;

                // TODO: Add console safe folders for dev?
                // Maybe throw exception on release builds?
#endif
                EnsureFolderHierarchyExists(s_outputFolder);
            }

            return s_outputFolder;


        }

        /// <summary>
        ///     Validate that all directories are created for a given <paramref name="folderPath" />.
        /// </summary>
        /// <param name="folderPath">The path to process and validate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EnsureFolderHierarchyExists(string folderPath)
        {
            if (!string.IsNullOrEmpty(folderPath) && !Directory.Exists(folderPath))
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
            EnsureFolderHierarchyExists(targetDirectory);
        }

        /// <summary>
        ///     Validate that the file path is writable, making the necessary folder structure and setting permissions.
        /// </summary>
        /// <param name="filePath">The absolute path to validate.</param>
        public static void EnsureFileWritable(string filePath)
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
        public static void ForceDeleteFile(string filePath)
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
        public static bool IsFileWritable(string filePath)
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
    }
}