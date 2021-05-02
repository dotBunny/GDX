// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.ObjectInfos
{
    public sealed class MeshObjectInfo : ObjectInfo
    {
        public const string TypeDefinition = "GDX.Developer.ObjectInfos.MeshObjectInfo,GDX";
        public bool IsReadable;
        public int SubMeshCount;
        public int Triangles;

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

        public override string GetDetailedInformation()
        {
            return $"Readable: {IsReadable.ToString()}, Vertices: {VertexCount.ToString()}, SubMeshes: {SubMeshCount.ToString()}";
        }
    }
}