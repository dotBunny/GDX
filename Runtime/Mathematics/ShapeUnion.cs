// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Mathematics.Shapes;

namespace GDX.Mathematics
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit)]
    public struct ShapeUnion
    {
        [System.Runtime.InteropServices.FieldOffset(0)]
        public OBB obb;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public Capsule capsule;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public Sphere sphere;
        [System.Runtime.InteropServices.FieldOffset(40)]
        public int type;
    }
}