// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.Profiling;

namespace GDX.Developer
{
    public class ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.ObjectInfo,GDX";

        public long MemoryUsageBytes;
        public long TotalMemoryUsageBytes;
        public string Name;
        public System.Type Type;
        public uint CopyCount;
        public TransientReference Reference;

        public virtual void Populate(Object targetObject)
        {
            CopyCount = 1;
            Name = targetObject.name;
            Type = targetObject.GetType();
            MemoryUsageBytes = Profiler.GetRuntimeMemorySizeLong(targetObject);
            TotalMemoryUsageBytes = MemoryUsageBytes;
            Reference = new TransientReference(targetObject);
        }

        public virtual void Populate(Object targetObject, TransientReference reference)
        {
            CopyCount = 1;
            Name = targetObject.name;
            Type = targetObject.GetType();
            MemoryUsageBytes = Profiler.GetRuntimeMemorySizeLong(targetObject);
            TotalMemoryUsageBytes = MemoryUsageBytes;
            Reference = reference;
        }
    }
}