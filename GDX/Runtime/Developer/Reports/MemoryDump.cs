// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using GDX.Collections.Generic;
using GDX.Developer.Reports;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace GDX.Developer.Reports
{
    // TODO: Add way to maybe writee out whats referencing?
    // instead of just counts? bool flag
    public static class MemoryDump
    {
        public static HeapState ManagedHeapSnapshot()
        {
            // Create our collection object, this is going to effect memory based on its size
            HeapState heapState = new HeapState();

            // TODO : Make types configurable from GDX config?
            // Sections
            heapState.QueryForType<RenderTexture, TextureObjectInfo>("RenderTexture");
            heapState.QueryForType<Texture3D, TextureObjectInfo>("Texture3D");

            heapState.QueryForType<Texture2D, TextureObjectInfo>("Texture2D");
            heapState.QueryForType<Texture2DArray, TextureObjectInfo>("Texture2D");

            heapState.QueryForType<Cubemap, TextureObjectInfo>("Cubemap");
            heapState.QueryForType<CubemapArray, TextureObjectInfo>("Cubemap");


            heapState.QueryForType<Material, ObjectInfo>("Material");
            heapState.QueryForType<Shader, ObjectInfo>("Shader");
            heapState.QueryForType<AnimationClip, ObjectInfo>("AnimationClip");

            return heapState;
        }
    }
}