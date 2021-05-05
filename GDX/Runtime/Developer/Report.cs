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
    public abstract class Report
    {
        /// <summary>
        ///     The name of the scene that was known to the <see cref="UnityEngine.SceneManagement" /> as being the active scene
        ///     when this <see cref="ResourcesReport" /> was created.
        /// </summary>
        public string ActiveScene;

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

        protected Report()
        {
            ActiveScene = SceneManager.GetActiveScene().name;
            Platform = Application.platform;
            MonoUsedSize = Profiler.GetMonoUsedSizeLong();
            MonoHeapSize = Profiler.GetMonoHeapSizeLong();
            Created = DateTime.Now;

            UnityTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            UnityTotalReservedMemory = Profiler.GetTotalReservedMemoryLong();
            UnityGraphicsDriverAllocatedMemory = Profiler.GetAllocatedMemoryForGraphicsDriver();
            UnityTotalUnusedReservedMemory = Profiler.GetTotalUnusedReservedMemoryLong();
        }

        /// <summary>
        ///     Output the report format as an array of <see cref="string" />.
        /// </summary>
        /// <remarks>It is usually best to provide a <see cref="StringBuilder" /> or <see cref="StreamWriter" /> instead.</remarks>
        /// <param name="context"></param>
        /// <returns>A generated report as an array of <see cref="string" />.</returns>
        public string[] Output(ReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            return Output(builder, context)
                ? builder.ToString().Split(ReportBuilder.NewLineSplit, StringSplitOptions.None)
                : null;
        }

        /// <summary>
        ///     Output the report format utilizing the provided <paramref name="writer" />, optionally limiting the
        ///     write buffers by <paramref name="bufferSize" />.
        /// </summary>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <param name="writer">A <see cref="StreamWriter"/> instance to use for output.</param>
        /// <param name="bufferSize">The write buffer size.</param>
        /// <returns>true/false if the report was successfully written to the provided <paramref name="writer" />.</returns>
        public bool Output(StreamWriter writer, int bufferSize = 1024, ReportContext context = null)
        {
            StringBuilder builder = new StringBuilder();
            if (!Output(builder, context))
            {
                return false;
            }

            int contentLength = builder.Length;
            char[] content = new char[bufferSize];
            int leftOvers = (contentLength % bufferSize);
            int writeCount = (contentLength - leftOvers) / bufferSize;

            // Fixed sized writes
            for (int i = 0; i < writeCount; i++)
            {
                builder.CopyTo(i * bufferSize, content, 0, bufferSize);
                writer.Write(content, 0, bufferSize);
            }

            if (leftOvers > 0)
            {
                builder.CopyTo(writeCount * bufferSize, content, 0, leftOvers);
                writer.Write(content, 0, leftOvers);
            }

            writer.Flush();
            return true;
        }

        /// <summary>
        ///     Output the report format utilizing the provided <paramref name="builder" />.
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> to use when generating the report.</param>
        /// <param name="context">Contextual information regarding the generation of the report.</param>
        /// <returns>true/false if report was added to <paramref name="builder"/>.</returns>
        public abstract bool Output(StringBuilder builder, ReportContext context = null);
    }
}