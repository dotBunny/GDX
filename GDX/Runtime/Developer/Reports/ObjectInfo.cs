// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEngine.Profiling;

namespace GDX.Developer.Reports
{
    public class ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.Reports.ObjectInfo, GDX";

        public long MemoryUsageBytes;
        public string Name;
        public System.Type Type;
        public uint CopyCount;

        public virtual void Populate(UnityEngine.Object targetObject)
        {
            CopyCount = 1;
            Name = targetObject.name;
            Type = targetObject.GetType();
            MemoryUsageBytes = Profiler.GetRuntimeMemorySizeLong(targetObject);
        }
    }
}