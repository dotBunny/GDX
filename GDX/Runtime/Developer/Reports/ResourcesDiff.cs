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

            // Known Objects - Start on right hand side
            foreach (var kvpType in lhs.KnownObjects)
            {
                // Lets just add something ahead to be safe
                if (!AddedObjects.ContainsKey(kvpType.Key))
                {
                    AddedObjects.Add(kvpType.Key, new Dictionary<TransientReference, ObjectInfo>());
                }
                if (!RemovedObjects.ContainsKey(kvpType.Key))
                {
                    RemovedObjects.Add(kvpType.Key, new Dictionary<TransientReference, ObjectInfo>());
                }
                if (!CommonObjects.ContainsKey(kvpType.Key))
                {
                    CommonObjects.Add(kvpType.Key, new Dictionary<TransientReference, ObjectInfo>());
                }

                // RHS Dictionary?
                Dictionary<TransientReference, ObjectInfo> rhsKnownObjects = null;
                if (rhs.KnownObjects.ContainsKey(kvpType.Key))
                {
                    rhsKnownObjects = rhs.KnownObjects[kvpType.Key];
                }

                // Iterate over everything in the left hand side
                foreach (var knownObject in kvpType.Value)
                {
                    // None of this type in the right, means removed
                    if (rhsKnownObjects == null)
                    {
                        RemovedObjects[kvpType.Key].Add(knownObject.Key, knownObject.Value);
                    }
                    else
                    {
                        if (rhsKnownObjects.ContainsKey(knownObject.Key))
                        {
                            ObjectInfo clone = rhsKnownObjects[knownObject.Key].Clone();
                            clone.CopyCount = clone.CopyCount - knownObject.Value.CopyCount;

                            CommonObjects[kvpType.Key].Add(knownObject.Key, clone);
                        }
                        else
                        {
                            RemovedObjects[kvpType.Key].Add(knownObject.Key, knownObject.Value);
                        }

                    }
                }
            }

            // Iterate over rhs for added
            foreach (var kvpType in rhs.KnownObjects)
            {
                if (!lhs.KnownObjects.ContainsKey(kvpType.Key))
                {
                    AddedObjects.Add(kvpType.Key, new Dictionary<TransientReference, ObjectInfo>());
                    // Iterate over everything in the left hand side
                    foreach (var knownObject in kvpType.Value)
                    {
                        AddedObjects[kvpType.Key].Add(knownObject.Key, knownObject.Value);
                    }
                }
            }

            List<Type> removeType = new List<Type>();
            foreach (var kvpType in AddedObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }
            foreach (var type in removeType)
            {
                AddedObjects.Remove(type);
            }
            removeType.Clear();
            foreach (var kvpType in RemovedObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }
            foreach (var type in removeType)
            {
                RemovedObjects.Remove(type);
            }
            removeType.Clear();
            foreach (var kvpType in CommonObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }
            foreach (var type in removeType)
            {
                CommonObjects.Remove(type);
            }
        }
    }
}