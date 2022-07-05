// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System.Text;

namespace GDX.Developer.Reports.Resource.Sections
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public readonly struct MemoryDiffSection
    {
        public readonly LongDiff MonoHeapSize;
        public readonly LongDiff MonoUsedSize;
        public readonly LongDiff UnityGraphicsDriverAllocated;
        public readonly LongDiff UnityTotalAllocatedMemory;
        public readonly LongDiff UnityTotalReservedMemory;
        public readonly LongDiff UnityTotalUnusedReservedMemory;

        public MemoryDiffSection(MemorySection lhs, MemorySection rhs)
        {
            MonoUsedSize = new LongDiff(lhs.MonoUsedSize, rhs.MonoUsedSize);
            MonoHeapSize = new LongDiff(lhs.MonoHeapSize, rhs.MonoHeapSize);
            UnityTotalAllocatedMemory =
                new LongDiff(lhs.UnityTotalAllocatedMemory, rhs.UnityTotalAllocatedMemory);
            UnityTotalReservedMemory =
                new LongDiff(lhs.UnityTotalReservedMemory, rhs.UnityTotalReservedMemory);
            UnityTotalUnusedReservedMemory = new LongDiff(lhs.UnityTotalUnusedReservedMemory,
                rhs.UnityTotalUnusedReservedMemory);
            UnityGraphicsDriverAllocated = new LongDiff(lhs.UnityGraphicsDriverAllocatedMemory,
                rhs.UnityGraphicsDriverAllocatedMemory);
        }

        public static MemoryDiffSection Get(MemorySection lhs, MemorySection rhs)
        {
            return new MemoryDiffSection(lhs, rhs);
        }

        public void Output(ResourceReportContext context, StringBuilder builder, bool detailed = true)
        {
            builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"Total Mono Heap",
                MonoHeapSize.GetSizeOutput(context)));
            builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"Used Mono Heap",
                MonoUsedSize.GetSizeOutput(context)));

            if (detailed)
            {
                builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"GFX Driver Allocated Memory",
                    UnityGraphicsDriverAllocated.GetSizeOutput(context)));
                builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"Total Reserved Memory",
                    UnityTotalReservedMemory.GetSizeOutput(context)));
                builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"Total Allocated Memory",
                    UnityTotalAllocatedMemory.GetSizeOutput(context)));
                builder.AppendLine(ResourceReport.CreateKeyValuePair(context,"Total Unused Reserved Memory",
                    UnityTotalUnusedReservedMemory.GetSizeOutput(context)));
            }
        }
    }
}
#endif