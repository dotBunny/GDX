// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

// ReSharper disable UnusedMember.Global

namespace GDX.Classic.Developer
{
    // ReSharper disable once UnusedType.Global
    public class Profiling
    {
        /// <summary>
        ///     The prefix to use with all capture files.
        /// </summary>
        private const string MemoryCaptureFilePrefix = "MemCap-";

        private const int MemoryCapturesToKeep = 10;

        private const int ProfilesToKeep = 10;
        /// <summary>
        ///     The prefix to use with all binary profile files.
        /// </summary>
        private const string ProfileFilePrefix = "Profile-";

        private const CaptureFlags AllCaptureFlags = CaptureFlags.ManagedObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces;

        public static void TakeMemorySnapshot(string prefix = null, Action<string, bool> finishCallback = null, CaptureFlags captureFlags = AllCaptureFlags, bool manageCaptures = true)
        {
            string outputFolder = GDX.Platform.GetOutputFolder();
            if (manageCaptures)
            {
                string[] files = Directory.GetFiles(outputFolder, prefix == null ? $"{MemoryCaptureFilePrefix}*" : $"{MemoryCaptureFilePrefix}{prefix}-*", SearchOption.TopDirectoryOnly);
                int filesToRemove = files.Length - (MemoryCapturesToKeep - 1);

                if (filesToRemove > 0)
                {
                    List<string> fileList = new List<string>(files.Length);
                    fileList.AddRange(files);
                    fileList.Sort();

                    for (int i = 0; i < filesToRemove; i++)
                    {
                        GDX.Platform.ForceDeleteFile(fileList[i]);
                    }
                }
            }

            string path = Path.Combine(outputFolder, prefix != null ? $"{MemoryCaptureFilePrefix}{prefix}-{DateTime.Now:GDX.Platform.FilenameTimestampFormat}.snap" :
                $"{MemoryCaptureFilePrefix}{DateTime.Now:GDX.Platform.FilenameTimestampFormat}.raw");
            MemoryProfiler.TakeSnapshot(path, finishCallback, captureFlags);
            Trace.Output(Trace.TraceLevel.Info, $"[MemorySnapshot] {path}");
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

            string outputFolder = GDX.Platform.GetOutputFolder();
            if (manageProfiles)
            {
                string[] files = Directory.GetFiles(outputFolder, prefix == null ? $"{ProfileFilePrefix}*" : $"{ProfileFilePrefix}{prefix}-*", SearchOption.TopDirectoryOnly);
                // If we have 15 files, and our max is 15, we need to remove just one.
                int filesToRemove = files.Length - (ProfilesToKeep - 1);

                if (filesToRemove > 0)
                {
                    List<string> fileList = new List<string>(files.Length);
                    fileList.AddRange(files);
                    fileList.Sort();

                    for (int i = 0; i < filesToRemove; i++)
                    {
                        GDX.Platform.ForceDeleteFile(fileList[i]);
                    }
                }
            }

            string path = Path.Combine(outputFolder, prefix != null ? $"{ProfileFilePrefix}{prefix}-{GDX.Platform.FilenameTimestampFormat}.raw" : $"{ProfileFilePrefix}{GDX.Platform.FilenameTimestampFormat}.raw");
            Trace.Output(Trace.TraceLevel.Info, $"[Profiling Started] {path}");
            Profiler.logFile = path;
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;
        }

        /// <summary>
        ///     Finalize a profiling session during an import.
        /// </summary>
        public static void StopProfiling()
        {
            Trace.Output(Trace.TraceLevel.Info, $"[Profiling Stopped] {Profiler.logFile}");
            Profiler.enabled = false;
            Profiler.logFile = "";
        }
    }
}