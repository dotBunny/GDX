using System;
using UnityEngine;
using System.Runtime.CompilerServices;
#if GDX_MATHEMATICS
using Unity.Mathematics;
#endif
using GDX.Collections.Pooling;

namespace GDX.Mathematics.Shapes
{
    /// <summary>
    /// Axis-Aligned Bounding Box
    /// </summary>
    public struct AABB
    {
        public Vector3 Min;
        public Vector3 Max;

        public Vector3 Extents
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Max - Min; }
        }

        public Vector3 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return (Max + Min) * 0.5f; }
        }

        public bool Intersects(in AABB other)
        {
            bool overlapsX = (Max.x >= other.Min.x) & (Min.x <= other.Max.x);
            bool overlapsY = (Max.y >= other.Min.y) & (Min.y <= other.Max.y);
            bool overlapsZ = (Max.z >= other.Min.z) & (Min.z <= other.Max.z);
            return overlapsX & overlapsY & overlapsZ;
        }
    }
}
