﻿// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Developer.Reports
{
    public class MeshObjectInfo : ObjectInfo
    {
        public int VertexCount;
        public int SubMeshCount;
        public int Triangles;
        public bool IsReadable;

        public override void Populate(UnityEngine.Object targetObject)
        {
            base.Populate(targetObject);
            Mesh meshAsset = (Mesh)targetObject;

            // Useful mesh information
            VertexCount = meshAsset.vertexCount;
            SubMeshCount = meshAsset.subMeshCount;
            Triangles = meshAsset.triangles.Length;
            IsReadable = meshAsset.isReadable;
        }
    }
}