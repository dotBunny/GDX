// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Developer
{
    public class ResourcesDiffReport
    {
        public int ObjectCount;
        public float ObjectCountChange;
        public long MonoUsedSize;
        public float MonoUsedSizeChange;
        public long MonoHeapSize;
        public float MonoHeapSizeChange;
        public long UnityTotalAllocatedMemory;
        public float UnityTotalAllocatedMemoryChange;
        public long UnityTotalReservedMemory;
        public float UnityTotalReservedMemoryChange;
        public long UnityGraphicsDriverAllocatedMemory;
        public float UnityGraphicsDriverAllocatedMemoryChange;
        public long UnityTotalUnusedReservedMemory;
        public float UnityTotalUnusedReservedMemoryChange;

        public static ResourcesDiffReport Generate(ResourcesReport lhs, ResourcesReport rhs)
        {
            ResourcesDiffReport returnReport = new ResourcesDiffReport();

            // Simple difference values
            returnReport.ObjectCount = rhs.ObjectCount - lhs.ObjectCount;
            returnReport.ObjectCountChange = rhs.ObjectCount / lhs.ObjectCount;
            returnReport.MonoUsedSize = rhs.MonoUsedSize - lhs.MonoUsedSize;
            returnReport.MonoUsedSizeChange = rhs.MonoUsedSize / lhs.MonoUsedSize;
            returnReport.MonoHeapSize = rhs.MonoHeapSize - lhs.MonoHeapSize;
            returnReport.MonoHeapSizeChange = rhs.MonoHeapSize / lhs.MonoHeapSize;
            returnReport.UnityTotalAllocatedMemory = rhs.UnityTotalAllocatedMemory - lhs.UnityTotalAllocatedMemory;
            returnReport.UnityTotalAllocatedMemoryChange = rhs.UnityTotalAllocatedMemory / lhs.UnityTotalAllocatedMemory;
            returnReport.UnityTotalReservedMemory = rhs.UnityTotalReservedMemory - lhs.UnityTotalReservedMemory;
            returnReport.UnityTotalReservedMemoryChange = rhs.UnityTotalReservedMemory / lhs.UnityTotalReservedMemory;
            returnReport.UnityGraphicsDriverAllocatedMemory =
                rhs.UnityGraphicsDriverAllocatedMemory - lhs.UnityGraphicsDriverAllocatedMemory;
            returnReport.UnityGraphicsDriverAllocatedMemoryChange =
                rhs.UnityGraphicsDriverAllocatedMemory / lhs.UnityGraphicsDriverAllocatedMemory;
            returnReport.UnityTotalUnusedReservedMemory =
                rhs.UnityTotalUnusedReservedMemory - lhs.UnityTotalUnusedReservedMemory;
            returnReport.UnityTotalUnusedReservedMemoryChange =
                rhs.UnityTotalUnusedReservedMemory / lhs.UnityTotalUnusedReservedMemory;

            // Known Usage Changes

            //
            // // Calculate Known Usage Changes
            // foreach (var kvp in lhs.KnownUsage)
            // {
            //     if (rhs.KnownUsage.ContainsKey(kvp.Key))
            //     {
            //         returnReport.KnownUsage.Add(kvp.Key, rhs.KnownUsage[kvp.Key] - kvp.Value);
            //     }
            //     else
            //     {
            //         returnReport.KnownUsage.Add(kvp.Key, -kvp.Value);
            //     }
            // }
            // foreach (var kvp in rhs.KnownUsage)
            // {
            //     if (!lhs.KnownUsage.ContainsKey(kvp.Key))
            //     {
            //         returnReport.KnownUsage.Add(kvp.Key, kvp.Value);
            //     }
            // }
            //
            // // Figure out Known Objects
            // foreach (var kvpType in lhs.KnownObjects)
            // {
            //
            // }

            return returnReport;
        }
    }
}