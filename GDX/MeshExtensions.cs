// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using UnityEngine;
using Unity.Mathematics;

namespace GDX
{
    /// <summary>
    ///     <see cref="UnityEngine.Mesh" /> Based Extension Methods
    /// </summary>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [VisualScriptingCompatible(2)]
    public static class MeshExtensions
    {
        /// <summary>
        ///     Determine the volume of a given mesh.
        /// </summary>
        /// <remarks>
        ///     Based off of https://n-e-r-v-o-u-s.com/blog/?p=4415.
        /// </remarks>
        /// <param name="targetMesh">The mesh to evaluate for its volume.</param>
        /// <returns>The meshes volume.</returns>
        public static float CalculateVolume(this Mesh targetMesh)
        {
            float volume = 0;
            Vector3[] vertices = targetMesh.vertices;
            int[] triangles = targetMesh.triangles;
            int triangleLength = targetMesh.triangles.Length;

            for (int i = 0; i < triangleLength; i += 3)
            {
                float3 p1 = vertices[triangles[i + 0]];
                float3 p2 = vertices[triangles[i + 1]];
                float3 p3 = vertices[triangles[i + 2]];
                volume += math.dot(math.cross(p1, p2), p3);
            }
            return math.abs(volume / 6.0f);
        }
    }
}
#endif