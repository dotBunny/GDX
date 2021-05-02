// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer
{
    public class ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.ObjectInfo,GDX";
        public uint CopyCount;

        public long MemoryUsageBytes;
        public string Name;
        public TransientReference Reference;
        public long TotalMemoryUsageBytes;
        public Type Type;

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