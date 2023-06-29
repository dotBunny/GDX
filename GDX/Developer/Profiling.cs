// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.IO;
using GDX.Experimental;
using GDX.Experimental.Logging;
#if UNITY_2022_2_OR_NEWER
using Unity.Profiling.Memory;
#else
using UnityEngine.Profiling.Memory.Experimental;
#endif // UNITY_2022_2_OR_NEWER

namespace GDX.Developer
{
    /// <summary>
    ///     A set of functionality useful for creating profiling data to reason about the performance of an application.
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public class Profiling
    {
        /// <summary>
        ///     The prefix to use with all capture files.
        /// </summary>
        const string k_MemoryCaptureFilePrefix = "MemCap-";
        /// <summary>
        ///     The number of memory captures to keep in the output folder.
        /// </summary>
        const int k_MemoryCapturesToKeep = 10;
        /// <summary>
        ///     The number of profile captures to keep in the output folder.
        /// </summary>
        const int k_ProfilesToKeep = 10;
        /// <summary>
        ///     The prefix to use with all binary profile files.
        /// </summary>
        const string k_ProfileFilePrefix = "Profile-";

        /// <summary>
        ///     The default flags (all) used when capturing memory.
        /// </summary>
        const CaptureFlags k_AllCaptureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces;

        /// <summary>
        ///     Take a memory snapshot and save it to <see cref="Platform.GetOutputFolder"/>.
        /// </summary>
        /// <param name="prefix">Override the default prefix <see cref="k_MemoryCaptureFilePrefix"/>.</param>
        /// <param name="finishCallback">Optional callback action once the memory capture has been made.</param>
        /// <param name="captureFlags">Override of the memory capture flags, defaults to <see cref="k_AllCaptureFlags"/>.</param>
        /// <param name="manageCaptures">Should the number of captures found in the output folder be managed?</param>
        public static void TakeMemorySnapshot(string prefix = null, Action<string, bool> finishCallback = null, CaptureFlags captureFlags = k_AllCaptureFlags, bool manageCaptures = true)
        {
            string outputFolder = Platform.GetOutputFolder();
            if (manageCaptures)
            {
                string[] files = Directory.GetFiles(outputFolder, prefix == null ? $"{k_MemoryCaptureFilePrefix}*" : $"{k_MemoryCaptureFilePrefix}{prefix}-*", SearchOption.TopDirectoryOnly);
                int filesToRemove = files.Length - (k_MemoryCapturesToKeep - 1);

                if (filesToRemove > 0)
                {
                    List<string> fileList = new List<string>(files.Length);
                    fileList.AddRange(files);
                    fileList.Sort();

                    for (int i = 0; i < filesToRemove; i++)
                    {
                        Platform.ForceDeleteFile(fileList[i]);
                    }
                }
            }

            string path = Path.Combine(outputFolder, prefix != null ? $"{k_MemoryCaptureFilePrefix}{prefix}-{DateTime.Now:GDX.Platform.FilenameTimestampFormat}.snap" :
                $"{k_MemoryCaptureFilePrefix}{DateTime.Now:GDX.Platform.FilenameTimestampFormat}.raw");
            MemoryProfiler.TakeSnapshot(path, finishCallback, captureFlags);
            ManagedLog.Info(LogCategory.GDX, $"[MemorySnapshot] {path}");
        }

        /// <summary>
        ///     Setup a profiling session used during an import. This will create a binary file when finished profiling.
        /// </summary>
        /// <param name="prefix">Optional descriptor for profile run used in filename.</param>
        /// <param name="manageProfiles">Should the number of profiles be managed.</param>
        public static void StartProfiling(string prefix = null, bool manageProfiles = true)
        {
            // Make sure it is off
            Profiler.enabled = false;

            string outputFolder = Platform.GetOutputFolder();
            if (manageProfiles)
            {
                string[] files = Directory.GetFiles(outputFolder, prefix == null ? $"{k_ProfileFilePrefix}*" : $"{k_ProfileFilePrefix}{prefix}-*", SearchOption.TopDirectoryOnly);
                // If we have 15 files, and our max is 15, we need to remove just one.
                int filesToRemove = files.Length - (k_ProfilesToKeep - 1);

                if (filesToRemove > 0)
                {
                    List<string> fileList = new List<string>(files.Length);
                    fileList.AddRange(files);
                    fileList.Sort();

                    for (int i = 0; i < filesToRemove; i++)
                    {
                        Platform.ForceDeleteFile(fileList[i]);
                    }
                }
            }

            string path = Path.Combine(outputFolder, prefix != null ? $"{k_ProfileFilePrefix}{prefix}-{Platform.FilenameTimestampFormat}.raw" : $"{k_ProfileFilePrefix}{Platform.FilenameTimestampFormat}.raw");
            ManagedLog.Info(LogCategory.GDX, $"[Profiling Started] {path}");
            Profiler.logFile = path;
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;
        }

        /// <summary>
        ///     Finalize a profiling session during an import.
        /// </summary>
        public static void StopProfiling()
        {
            ManagedLog.Info(LogCategory.GDX, $"[Profiling Stopped] {Profiler.logFile}");
            Profiler.enabled = false;
            Profiler.logFile = "";
        }
    }
}
#endif // !UNITY_DOTSRUNTIME