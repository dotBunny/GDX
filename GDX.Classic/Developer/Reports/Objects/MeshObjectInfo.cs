// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace GDX.Classic.Developer.Reports.Objects
{
    public sealed class MeshObjectInfo : ObjectInfo
    {
        public new const string TypeDefinition = "GDX.Developer.Reports.Objects.MeshObjectInfo,GDX";
        public bool IsReadable;
        public int SubMeshCount;
        public int Triangles;

        /// <summary>
        /// Create a clone of this object.
        /// </summary>
        /// <returns></returns>
        public override ObjectInfo Clone()
        {
            return new MeshObjectInfo()
            {
                CopyCount = this.CopyCount,
                MemoryUsage = this.MemoryUsage,
                Name = this.Name,
                Reference = this.Reference,
                TotalMemoryUsage = this.TotalMemoryUsage,
                Type = this.Type,
                IsReadable =  this.IsReadable,
                SubMeshCount = this.SubMeshCount,
                Triangles =  this.Triangles
            };
        }

        public int VertexCount;

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