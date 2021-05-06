// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Developer.Reports.Elements;
using GDX.Developer.Reports.Objects;

namespace GDX.Developer.Reports
{
    public class ResourcesDiff
    {
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> AddedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> CommonObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, LongDiff> KnownUsage =
            new SerializableDictionary<Type, LongDiff>();

        public readonly LongDiff MonoHeapSize;
        public readonly LongDiff MonoUsedSize;
        public readonly IntegerDiff ObjectCount;

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> RemovedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly LongDiff UnityGraphicsDriverAllocated;
        public readonly LongDiff UnityTotalAllocatedMemory;
        public readonly LongDiff UnityTotalReservedMemory;
        public readonly LongDiff UnityTotalUnusedReservedMemory;

        public ResourcesDiff(ResourcesAudit lhs, ResourcesAudit rhs)
        {
            // Simple difference values
            ObjectCount = new IntegerDiff(lhs.ObjectCount, rhs.ObjectCount);
            MonoUsedSize = new LongDiff(lhs.MemoryContext.MonoUsedSize, rhs.MemoryContext.MonoUsedSize);
            MonoHeapSize = new LongDiff(lhs.MemoryContext.MonoHeapSize, rhs.MemoryContext.MonoHeapSize);
            UnityTotalAllocatedMemory =
                new LongDiff(lhs.MemoryContext.UnityTotalAllocatedMemory, rhs.MemoryContext.UnityTotalAllocatedMemory);
            UnityTotalReservedMemory =
                new LongDiff(lhs.MemoryContext.UnityTotalReservedMemory, rhs.MemoryContext.UnityTotalReservedMemory);
            UnityTotalUnusedReservedMemory = new LongDiff(lhs.MemoryContext.UnityTotalUnusedReservedMemory,
                lhs.MemoryContext.UnityTotalUnusedReservedMemory);
            UnityGraphicsDriverAllocated = new LongDiff(lhs.MemoryContext.UnityGraphicsDriverAllocatedMemory,
                rhs.MemoryContext.UnityGraphicsDriverAllocatedMemory);

            // Known Usage Changes
            foreach (KeyValuePair<Type, long> kvp in lhs.KnownUsage)
            {
                KnownUsage.Add(kvp.Key,
                    rhs.KnownUsage.ContainsKey(kvp.Key)
                        ? new LongDiff(kvp.Value, rhs.KnownUsage[kvp.Key])
                        : new LongDiff(kvp.Value, 0));
            }
            foreach (KeyValuePair<Type, long> kvp in rhs.KnownUsage)
            {
                if (!lhs.KnownUsage.ContainsKey(kvp.Key))
                {
                    KnownUsage.Add(kvp.Key, new LongDiff(0, kvp.Value));
                }
            }


            // Known Objects
            // foreach (var kvpType in lhs.KnownObjects)
            // {
            //
            // }
        }
    }
}