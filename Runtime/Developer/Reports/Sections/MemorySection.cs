// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;
using UnityEngine.Profiling;

namespace GDX.Developer.Reports.Sections
{
    public readonly struct MemorySection
    {
        /// <summary>
        ///     The size of the Mono heap when the <see cref="ResourcesAudit" /> was created.
        /// </summary>
        /// <remarks>This is cached so that the <see cref="ResourcesAudit" /> does not effect this value.</remarks>
        public readonly long MonoHeapSize;

        /// <summary>
        ///     The amount of the Mono heap used when the <see cref="ResourcesAudit" /> was created.
        /// </summary>
        /// <remarks>This is cached so that the <see cref="ResourcesAudit" /> does not effect this value.</remarks>
        public readonly long MonoUsedSize;

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

        public MemorySection(long monoHeapSize, long monoUsedSize, long unityTotalAllocatedMemory,
            long unityTotalReservedMemory, long unityTotalUnusedReservedMemory, long unityGraphicsDriverAllocatedMemory)
        {
            MonoHeapSize = monoHeapSize;
            MonoUsedSize = monoUsedSize;
            UnityTotalAllocatedMemory = unityTotalAllocatedMemory;
            UnityTotalReservedMemory = unityTotalReservedMemory;
            UnityTotalUnusedReservedMemory = unityTotalUnusedReservedMemory;
            UnityGraphicsDriverAllocatedMemory = unityGraphicsDriverAllocatedMemory;
        }
        public static MemorySection Get()
        {
            return new MemorySection(Profiler.GetMonoHeapSizeLong(), Profiler.GetMonoUsedSizeLong(),
                Profiler.GetTotalAllocatedMemoryLong(), Profiler.GetTotalReservedMemoryLong(),
                Profiler.GetTotalUnusedReservedMemoryLong(), Profiler.GetAllocatedMemoryForGraphicsDriver());
        }

        public void Output(ReportContext context, StringBuilder builder,
            bool detailed = true)
        {
            builder.AppendLine(context.CreateKVP("Total Mono Heap",
                Localization.GetHumanReadableFileSize(MonoHeapSize)));
            builder.AppendLine(context.CreateKVP("Used Mono Heap",
                Localization.GetHumanReadableFileSize(MonoUsedSize)));

            if (detailed)
            {
                builder.AppendLine(context.CreateKVP("GFX Driver Allocated Memory",
                    Localization.GetHumanReadableFileSize(UnityGraphicsDriverAllocatedMemory)));
                builder.AppendLine(context.CreateKVP("Total Reserved Memory",
                    Localization.GetHumanReadableFileSize(UnityTotalReservedMemory)));
                builder.AppendLine(context.CreateKVP("Total Allocated Memory",
                    Localization.GetHumanReadableFileSize(UnityTotalAllocatedMemory)));
                builder.AppendLine(context.CreateKVP("Total Unused Reserved Memory",
                    Localization.GetHumanReadableFileSize(UnityTotalUnusedReservedMemory)));
            }
        }
    }

}