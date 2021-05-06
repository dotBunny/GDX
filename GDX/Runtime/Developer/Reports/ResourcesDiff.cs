// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Developer.Reports.Objects;
using GDX.Developer.Reports.Sections;

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

        public readonly MemoryDiffSection MemoryContext;

        public readonly IntegerDiff ObjectCount;

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> RemovedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public ResourcesDiff(ResourcesAudit lhs, ResourcesAudit rhs)
        {
            ObjectCount = new IntegerDiff(lhs.ObjectCount, rhs.ObjectCount);
            MemoryContext = MemoryDiffSection.Get(lhs.MemoryContext, rhs.MemoryContext);

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