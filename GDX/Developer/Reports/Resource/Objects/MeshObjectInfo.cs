﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;

namespace GDX.Developer.Reports.Resource.Objects
{
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    public sealed class MeshObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Resource.Objects.MeshObjectInfo,GDX";
        public bool IsReadable;
        public int SubMeshCount;
        public int Triangles;

        public int VertexCount;

        /// <summary>
        ///     Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new MeshObjectInfo
            {
                CopyCount = CopyCount,
                MemoryUsage = MemoryUsage,
                Name = Name,
                Reference = Reference,
                TotalMemoryUsage = TotalMemoryUsage,
                Type = Type,
                IsReadable = IsReadable,
                SubMeshCount = SubMeshCount,
                Triangles = Triangles
            };
        }

        public override void Populate(Object targetObject, TransientReference reference = null)
        {
            base.Populate(targetObject, reference);
            Mesh meshAsset = (Mesh)targetObject;

            // Useful mesh information
            VertexCount = meshAsset.vertexCount;
            SubMeshCount = meshAsset.subMeshCount;
            Triangles = meshAsset.triangles.Length;
            IsReadable = meshAsset.isReadable;
        }

        public override string GetDetailedInformation(int maximumWidth)
        {
            return maximumWidth < 40
                ? $"{(IsReadable ? "RW" : "")} (V:{VertexCount.ToString()}, SM:{SubMeshCount.ToString()})"
                : $"{(IsReadable ? "RW" : "")} (Verts:{VertexCount.ToString()}, SubMeshes:{SubMeshCount.ToString()})";
        }
    }
}
#endif // !UNITY_DOTSRUNTIME