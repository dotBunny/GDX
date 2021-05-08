﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

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
                lhs.UnityTotalUnusedReservedMemory);
            UnityGraphicsDriverAllocated = new LongDiff(lhs.UnityGraphicsDriverAllocatedMemory,
                rhs.UnityGraphicsDriverAllocatedMemory);
        }

        public static MemoryDiffSection Get(MemorySection lhs, MemorySection rhs)
        {
            return new MemoryDiffSection(lhs, rhs);
        }

    }
}