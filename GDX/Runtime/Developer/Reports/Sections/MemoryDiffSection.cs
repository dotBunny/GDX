// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Text;

namespace GDX.Developer.Reports.Sections
{
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

        public void Output(ReportContext context, StringBuilder builder, bool detailed = true)
        {
            builder.AppendLine(context.CreateKVP("Total Mono Heap",
                MonoHeapSize.GetSizeOutput(context)));
            builder.AppendLine(context.CreateKVP("Used Mono Heap",
                MonoUsedSize.GetSizeOutput(context)));

            if (detailed)
            {
                builder.AppendLine(context.CreateKVP("GFX Driver Allocated Memory",
                    UnityGraphicsDriverAllocated.GetSizeOutput(context)));
                builder.AppendLine(context.CreateKVP("Total Reserved Memory",
                    UnityTotalReservedMemory.GetSizeOutput(context)));
                builder.AppendLine(context.CreateKVP("Total Allocated Memory",
                    UnityTotalAllocatedMemory.GetSizeOutput(context)));
                builder.AppendLine(context.CreateKVP("Total Unused Reserved Memory",
                    UnityTotalUnusedReservedMemory.GetSizeOutput(context)));
            }
        }
    }
}