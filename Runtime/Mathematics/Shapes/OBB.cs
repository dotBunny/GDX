// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Mathematics.Shapes
{
    /// <summary>
    /// Oriented Bounding Box
    /// </summary>
    public struct OBB
    {
        public Quaternion Rotation;
        public Vector3 Position;
        public Vector3 Size;
    }
}