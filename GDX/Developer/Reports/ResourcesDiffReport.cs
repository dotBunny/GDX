// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System;
using System.Collections.Generic;
using System.Text;
using GDX.Developer.Reports.Resource;
using GDX.Developer.Reports.Resource.Objects;
using GDX.Developer.Reports.Resource.Sections;

namespace GDX.Developer.Reports
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public class ResourcesDiffReport : ResourceReport
    {
        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> AddedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> CommonObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public readonly Dictionary<Type, LongDiff> KnownUsage =
            new Dictionary<Type, LongDiff>();

        public readonly MemoryDiffSection MemoryContext;

        public readonly IntegerDiff ObjectCount;

        public readonly Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> RemovedObjects =
            new Dictionary<Type, Dictionary<TransientReference, ObjectInfo>>();

        public ResourcesDiffReport(ResourcesAuditReport lhs, ResourcesAuditReport rhs)
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
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> kvpType in lhs.KnownObjects)
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
                foreach (KeyValuePair<TransientReference, ObjectInfo> knownObject in kvpType.Value)
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
                            clone.CopyCount -= knownObject.Value.CopyCount;

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
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> kvpType in rhs.KnownObjects)
            {
                if (!lhs.KnownObjects.ContainsKey(kvpType.Key))
                {
                    AddedObjects.Add(kvpType.Key, new Dictionary<TransientReference, ObjectInfo>());
                    // Iterate over everything in the left hand side
                    foreach (KeyValuePair<TransientReference, ObjectInfo> knownObject in kvpType.Value)
                    {
                        AddedObjects[kvpType.Key].Add(knownObject.Key, knownObject.Value);
                    }
                }
            }

            List<Type> removeType = new List<Type>();
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> kvpType in AddedObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }

            foreach (Type type in removeType)
            {
                AddedObjects.Remove(type);
            }

            removeType.Clear();
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> kvpType in RemovedObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }

            foreach (Type type in removeType)
            {
                RemovedObjects.Remove(type);
            }

            removeType.Clear();
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> kvpType in CommonObjects)
            {
                if (kvpType.Value.Count == 0)
                {
                    removeType.Add(kvpType.Key);
                }
            }

            foreach (Type type in removeType)
            {
                CommonObjects.Remove(type);
            }
        }

        /// <inheritdoc />
        public override bool Output(StringBuilder builder, ResourceReportContext context = null)
        {
            // We need to make the context if its not provided
            context ??= new ResourceReportContext();

            // Create header
            builder.AppendLine(CreateHeader(context, "START: Resources Diff Report"));

            // Custom header information
            builder.AppendLine(CreateKeyValuePair(context, "Total Objects", ObjectCount.GetOutput(context)));
            builder.AppendLine();
            foreach (KeyValuePair<Type, LongDiff> usage in KnownUsage)
            {
                builder.AppendLine(
                    CreateKeyValuePair(context, usage.Key.ToString(), usage.Value.GetSizeOutput(context)));
            }

            builder.AppendLine();

            // Add memory information
            MemoryContext.Output(context, builder);
            builder.AppendLine();

            builder.AppendLine(CreateHeader(context, "Added Objects"));
            OutputObjectInfos(builder, context, AddedObjects);

            builder.AppendLine(CreateHeader(context, "Common Objects"));
            OutputObjectInfos(builder, context, CommonObjects);

            builder.AppendLine(CreateHeader(context, "Removed Objects"));
            OutputObjectInfos(builder, context, RemovedObjects);

            // Footer
            builder.AppendLine(CreateHeader(context, "END: Resources Diff Report"));

            return true;
        }

        static void OutputObjectInfos(StringBuilder builder, ResourceReportContext context,
            Dictionary<Type, Dictionary<TransientReference, ObjectInfo>> targetObjects)
        {
            foreach (KeyValuePair<Type, Dictionary<TransientReference, ObjectInfo>> target in targetObjects)
            {
                int count = target.Value.Count;

                builder.AppendLine(CreateHeader(context, $"{target.Key} [{count.ToString()}] ", '-'));

                // Sort the known objects based on size as that's the most useful context to have them listed
                List<ObjectInfo> newList = new List<ObjectInfo>(count);
                foreach (KeyValuePair<TransientReference, ObjectInfo> objectInfo in target.Value)
                {
                    newList.Add(objectInfo.Value);
                }

                newList.Sort();

                // Output each item
                for (int i = 0; i < count; i++)
                {
                    newList[i].Output(context, builder);
                }

                builder.AppendLine();
            }
        }
    }
}
#endif // !UNITY_DOTSRUNTIME