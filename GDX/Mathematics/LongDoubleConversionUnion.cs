﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace GDX.Mathematics
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LongDoubleConversionUnion
    {
        [FieldOffset(0)] public long LongValue;
        [FieldOffset(0)] public double DoubleValue;
    }
}