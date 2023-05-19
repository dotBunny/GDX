// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX
{
    public static class Serializable
    {
        public const string Invalid = "Invalid";
        public const string String = "String";
        public const string Char = "Character";
        public const string Bool = "Boolean";
        public const string SByte = "Byte";
        public const string Byte = "Unsigned Byte";
        public const string Short = "Short";
        public const string UShort = "Unsigned Short";
        public const string Int = "Integer";
        public const string UInt = "UInt";
        public const string Long = "Long";
        public const string ULong = "Unsigned Long";
        public const string Float = "Float";
        public const string Double = "Double";
        public const string Vector2 = "Vector2";
        public const string Vector3 = "Vector3";
        public const string Vector4 = "Vector4";
        public const string Vector2Int = "Vector2 (Integer)";
        public const string Vector3Int = "Vector3 (Integer)";
        public const string Quaternion = "Quaternion";
        public const string Rect = "Rectangle";
        public const string RectInt = "Rectangle (Integer)";
        public const string Color = "Color";
        public const string LayerMask = "Layer (Mask)";
        public const string Bounds = "Bounds";
        public const string BoundsInt = "Bounds (Integer)";
        public const string Hash128 = "Hash128";
        public const string Gradient = "Gradient";
        public const string AnimationCurve = "AnimationCurve";
        public const string Object = "Object";

        public enum SerializableTypes
        {
            Invalid = -1,
            String,
            Char,
            Bool,
            SByte,
            Byte,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            Vector2,
            Vector3,
            Vector4,
            Vector2Int,
            Vector3Int,
            Quaternion,
            Rect,
            RectInt,
            Color,
            LayerMask,
            Bounds,
            BoundsInt,
            Hash128,
            Gradient,
            AnimationCurve,
            Object
        }
        public const int SerializableTypesCount = 29;

        public static string GetLabel(this SerializableTypes serializableType)
        {
            switch (serializableType)
            {
                case SerializableTypes.Invalid:
                    return Invalid;
                case SerializableTypes.String:
                    return String;
                case SerializableTypes.Char:
                    return Char;
                case SerializableTypes.Bool:
                    return Bool;
                case SerializableTypes.SByte:
                    return SByte;
                case SerializableTypes.Byte:
                    return Byte;
                case SerializableTypes.Short:
                    return Short;
                case SerializableTypes.UShort:
                    return UShort;
                case SerializableTypes.Int:
                    return Int;
                case SerializableTypes.UInt:
                    return UInt;
                case SerializableTypes.Long:
                    return Long;
                case SerializableTypes.ULong:
                    return ULong;
                case SerializableTypes.Float:
                    return Float;
                case SerializableTypes.Double:
                    return Double;
                case SerializableTypes.Vector2:
                    return Vector2;
                case SerializableTypes.Vector3:
                    return Vector3;
                case SerializableTypes.Vector4:
                    return Vector4;
                case SerializableTypes.Vector2Int:
                    return Vector2Int;
                case SerializableTypes.Vector3Int:
                    return Vector3Int;
                case SerializableTypes.Quaternion:
                    return Quaternion;
                case SerializableTypes.Rect:
                    return Rect;
                case SerializableTypes.RectInt:
                    return RectInt;
                case SerializableTypes.Color:
                    return Color;
                case SerializableTypes.LayerMask:
                    return LayerMask;
                case SerializableTypes.Bounds:
                    return Bounds;
                case SerializableTypes.BoundsInt:
                    return BoundsInt;
                case SerializableTypes.Hash128:
                    return Hash128;
                case SerializableTypes.Gradient:
                    return Gradient;
                case SerializableTypes.AnimationCurve:
                    return AnimationCurve;
                case SerializableTypes.Object:
                    return Object;
            }
            return Invalid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializableTypesLabel(int typeValue)
        {
            return GetLabel((SerializableTypes)typeValue);
        }

    }
}