// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

namespace GDX.Developer
{
    public class Report
    {
        /// <summary>
        ///     The name of the scene that was known to the <see cref="UnityEngine.SceneManagement" /> as being the active scene
        ///     when this <see cref="ResourcesReport" /> was created.
        /// </summary>
        public readonly string ActiveScene;

        /// <summary>
        ///     The time of creation for the <see cref="ResourcesReport" />.
        /// </summary>
        public readonly DateTime Created;

        /// <summary>
        ///     The size of the Mono heap when the <see cref="ResourcesReport" /> was created.
        /// </summary>
        /// <remarks>This is cached so that the <see cref="ResourcesReport" /> does not effect this value.</remarks>
        public readonly long MonoHeapSize;

        /// <summary>
        ///     The amount of the Mono heap used when the <see cref="ResourcesReport" /> was created.
        /// </summary>
        /// <remarks>This is cached so that the <see cref="ResourcesReport" /> does not effect this value.</remarks>
        public readonly long MonoUsedSize;

        /// <summary>
        ///     The platform that the <see cref="ResourcesReport" /> was created on.
        /// </summary>
        public readonly RuntimePlatform Platform;

        /// <summary>
        ///     Unity's allocated native memory for the graphics driver (in bytes).
        /// </summary>
        public readonly long UnityGraphicsDriverAllocatedMemory;

        /// <summary>
        ///     Unity's total allocated memory (in bytes).
        /// </summary>
        public readonly long UnityTotalAllocatedMemory;

        /// <summary>
        ///     Unity's total reserved memory (in bytes).
        /// </summary>
        public readonly long UnityTotalReservedMemory;

        /// <summary>
        ///     Unity's total unused reserved memory (in bytes).
        /// </summary>
        public readonly long UnityTotalUnusedReservedMemory;

        /// <summary>
        ///     Unity's used portion of the heap (in bytes).
        /// </summary>
        public readonly long UnityUsedHeapSize;

        public Report()
        {
            ActiveScene = SceneManager.GetActiveScene().name;
            Platform = Application.platform;
            MonoUsedSize = Profiler.GetMonoUsedSizeLong();
            MonoHeapSize = Profiler.GetMonoHeapSizeLong();
            Created = DateTime.Now;

            UnityUsedHeapSize = Profiler.usedHeapSizeLong;
            UnityTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            UnityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong();
            UnityGraphicsDriverAllocatedMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
            UnityTotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong();
        }

        /// <summary>
        ///     Output the report format as an array of <see cref="string" />.
        /// </summary>
        /// <remarks>It is usually best to provide a <see cref="StringBuilder" /> or <see cref="StreamWriter" /> instead.</remarks>
        /// <returns>A generated report as an array of <see cref="string" />.</returns>
        public string[] Output()
        {
            StringBuilder builder = new StringBuilder();
            return Output(ref builder)
                ? builder.ToString().Split(ReportBuilder.NewLineSplit, StringSplitOptions.None)
                : null;
        }

        /// <summary>
        ///     Output the report format utilizing the provided <paramref name="writer" />, optionally limiting the
        ///     write buffers by <paramref name="bufferSize" />.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="bufferSize"></param>
        /// <returns>true/false if the report was successfully written to the provided <paramref name="writer" />.</returns>
        public bool Output(ref StreamWriter writer, int bufferSize = -1)
        {
            StringBuilder builder = new StringBuilder();
            if (Output(ref builder))
            {
                //writer.Write();
            }
            return false;
        }

        public virtual bool Output(ref StringBuilder builder)
        {
            return false;
        }
    }
}