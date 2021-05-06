// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;

namespace GDX.Developer
{
    public class ResourcesDiffReport
    {
        public readonly LongDiff MonoHeapSize;
        public readonly LongDiff MonoUsedSize;
        public readonly IntegerDiff ObjectCount;
        public readonly LongDiff UnityGraphicsDriverAllocated;
        public readonly LongDiff UnityTotalAllocatedMemory;
        public readonly LongDiff UnityTotalReservedMemory;
        public readonly LongDiff UnityTotalUnusedReservedMemory;

        public readonly Dictionary<Type, LongDiff> KnownUsage = new SerializableDictionary<Type, LongDiff>();

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> AddedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> CommonObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> RemovedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public ResourcesDiffReport(ResourcesReport lhs, ResourcesReport rhs)
        {
            // Simple difference values
            ObjectCount = new IntegerDiff(lhs.ObjectCount, rhs.ObjectCount);
            MonoUsedSize = new LongDiff(lhs.MonoUsedSize, rhs.MonoUsedSize);
            MonoHeapSize = new LongDiff(lhs.MonoHeapSize, rhs.MonoHeapSize);
            UnityTotalAllocatedMemory = new LongDiff(lhs.UnityTotalAllocatedMemory, rhs.UnityTotalAllocatedMemory);
            UnityTotalReservedMemory = new LongDiff(lhs.UnityTotalReservedMemory, rhs.UnityTotalReservedMemory);
            UnityTotalUnusedReservedMemory = new LongDiff(lhs.UnityTotalUnusedReservedMemory, lhs.UnityTotalUnusedReservedMemory);
            UnityGraphicsDriverAllocated = new LongDiff(lhs.UnityGraphicsDriverAllocatedMemory, rhs.UnityGraphicsDriverAllocatedMemory);

            // Known Usage Changes
            foreach (var kvp in lhs.KnownUsage)
            {
                KnownUsage.Add(kvp.Key,
                    rhs.KnownUsage.ContainsKey(kvp.Key)
                        ? new LongDiff(kvp.Value, rhs.KnownUsage[kvp.Key])
                        : new LongDiff(kvp.Value, 0));
            }
            foreach (var kvp in rhs.KnownUsage)
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

        public struct IntegerDiff
        {
            public float Percentage;
            public int Change;

            public IntegerDiff(int lhs, int rhs)
            {
                Percentage = rhs / lhs;
                Change = rhs - lhs;
            }
        }

        public struct LongDiff
        {
            public float Percentage;
            public long Change;

            public LongDiff(long lhs, long rhs)
            {
                Percentage = rhs / lhs;
                Change = rhs - lhs;
            }
        }
    }
}