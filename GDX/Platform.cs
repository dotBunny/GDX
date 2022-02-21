﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime.CompilerServices;
using GDX.Mathematics.Random;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

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

        static string s_OutputFolder;

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

        public static System.Reflection.Assembly[] GetLoadedAssemblies()
        {
            //TODO: We might need to shim this based on platform compilation, investigate
            return System.AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        ///     Returns a runtime writable folder.
        /// </summary>
        /// <returns>The full path to a writable folder at runtime.</returns>
        public static string GetOutputFolder(string folderName = "GDX")
        {
            if (s_OutputFolder != null) return s_OutputFolder;
            
#if UNITY_EDITOR
            s_OutputFolder = Path.Combine(UnityEngine.Application.dataPath, "..", folderName);
#elif UNITY_DOTSRUNTIME
                s_OutputFolder = Path.Combine(Directory.GetCurrentDirectory(), folderName);
#else
                s_OutputFolder = Path.Combine(UnityEngine.Application.persistentDataPath, folderName);

                // TODO: Add console safe folders for dev?
                // Maybe throw exception on release builds?
#endif
            EnsureFolderHierarchyExists(s_OutputFolder);

            return s_OutputFolder;


        }

        public static char GetRandomSafeCharacter(IRandomProvider random)
        {
            return SafeCharacterPool[random.NextInteger(0, CharacterPoolLengthExclusive)];
        }

#if !UNITY_DOTSRUNTIME
        /// <summary>
        ///     Is the application focused?
        /// </summary>
        /// <remarks>
        ///     There are issues on some platforms with getting an accurate reading.
        /// </remarks>
        /// <returns>true/false if the application has focus.</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
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
#endif // !UNITY_DOTSRUNTIME 

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
        
#if !UNITY_DOTSRUNTIME
        /// <summary>
        /// Is the application running in headless mode?.
        /// </summary>
        /// <remarks>Useful for detecting running a server.</remarks>
        /// <returns>true/false if the application is without an initialized graphics device.</returns>
        /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHeadless()
        {
            return SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null;
        }
#endif // !UNITY_DOTSRUNTIME   
    }
}