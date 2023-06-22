// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace GDX
{
    public static class Serializable
    {
        const string k_Invalid = "Invalid";
        static readonly int k_InvalidHashCode = k_Invalid.GetStableHashCode();
        const string k_String = "String";
        static readonly int k_StringHashCode = k_String.GetStableHashCode();
        const string k_Char = "Character";
        static readonly int k_CharHashCode = k_Char.GetStableHashCode();
        const string k_Bool = "Boolean";
        static readonly int k_BoolHashCode = k_Bool.GetStableHashCode();
        const string k_SByte = "Signed Byte";
        static readonly int k_SByteHashCode = k_SByte.GetStableHashCode();
        const string k_Byte = "Unsigned Byte";
        static readonly int k_ByteHashCode = k_Byte.GetStableHashCode();
        const string k_Short = "Short";
        static readonly int k_ShortHashCode = k_Short.GetStableHashCode();
        const string k_UShort = "Unsigned Short";
        static readonly int k_UShortHashCode = k_UShort.GetStableHashCode();
        const string k_Int = "Integer";
        static readonly int k_IntHashCode = k_Int.GetStableHashCode();
        const string k_UInt = "UInt";
        static readonly int k_UIntHashCode = k_UInt.GetStableHashCode();
        const string k_Long = "Long";
        static readonly int k_LongHashCode = k_Long.GetStableHashCode();
        const string k_ULong = "Unsigned Long";
        static readonly int k_ULongHashCode = k_ULong.GetStableHashCode();
        const string k_Float = "Float";
        static readonly int k_FloatHashCode = k_Float.GetStableHashCode();
        const string k_Double = "Double";
        static readonly int k_DoubleHashCode = k_Double.GetStableHashCode();
        const string k_Vector2 = "Vector2";
        static readonly int k_Vector2HashCode = k_Vector2.GetStableHashCode();
        const string k_Vector3 = "Vector3";
        static readonly int k_Vector3HashCode = k_Vector3.GetStableHashCode();
        const string k_Vector4 = "Vector4";
        static readonly int k_Vector4HashCode = k_Vector4.GetStableHashCode();
        const string k_Vector2Int = "Vector2 (Integer)";
        static readonly int k_Vector2IntHashCode = k_Vector2Int.GetStableHashCode();
        const string k_Vector3Int = "Vector3 (Integer)";
        static readonly int k_Vector3IntHashCode = k_Vector3Int.GetStableHashCode();
        const string k_Quaternion = "Quaternion";
        static readonly int k_QuaternionHashCode = k_Quaternion.GetStableHashCode();
        const string k_Rect = "Rectangle";
        static readonly int k_RectHashCode = k_Rect.GetStableHashCode();
        const string k_RectInt = "Rectangle (Integer)";
        static readonly int k_RectIntHashCode = k_RectInt.GetStableHashCode();
        const string k_Color = "Color";
        static readonly int k_ColorHashCode = k_Color.GetStableHashCode();
        const string k_LayerMask = "Layer (Mask)";
        static readonly int k_LayerMaskHashCode = k_LayerMask.GetStableHashCode();
        const string k_Bounds = "Bounds";
        static readonly int k_BoundsHashCode = k_Bounds.GetStableHashCode();
        const string k_BoundsInt = "Bounds (Integer)";
        static readonly int k_BoundsIntHashCode = k_BoundsInt.GetStableHashCode();
        const string k_Hash128 = "Hash128";
        static readonly int k_Hash128HashCode = k_Hash128.GetStableHashCode();
        const string k_Gradient = "Gradient";
        static readonly int k_GradientHashCode = k_Gradient.GetStableHashCode();
        const string k_AnimationCurve = "AnimationCurve";
        static readonly int k_AnimationCurveHashCode = k_AnimationCurve.GetStableHashCode();
        const string k_Object = "Object";
        static readonly int k_ObjectHashCode = k_Object.GetStableHashCode();
        const string k_EnumInt = "Enum (Integer)";
        static readonly int k_EnumIntHashCode = k_EnumInt.GetStableHashCode();

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
            Object,
            EnumInt
        }

        public const int SerializableTypesCount = 30;

        public static int GetHashCode(this SerializableTypes serializableTypes)
        {
            switch (serializableTypes)
            {
                case SerializableTypes.Invalid:
                    return k_InvalidHashCode;
                case SerializableTypes.String:
                    return k_StringHashCode;
                case SerializableTypes.Char:
                    return k_CharHashCode;
                case SerializableTypes.Bool:
                    return k_BoolHashCode;
                case SerializableTypes.SByte:
                    return k_SByteHashCode;
                case SerializableTypes.Byte:
                    return k_ByteHashCode;
                case SerializableTypes.Short:
                    return k_ShortHashCode;
                case SerializableTypes.UShort:
                    return k_UShortHashCode;
                case SerializableTypes.Int:
                    return k_IntHashCode;
                case SerializableTypes.UInt:
                    return k_UIntHashCode;
                case SerializableTypes.Long:
                    return k_LongHashCode;
                case SerializableTypes.ULong:
                    return k_ULongHashCode;
                case SerializableTypes.Float:
                    return k_FloatHashCode;
                case SerializableTypes.Double:
                    return k_DoubleHashCode;
                case SerializableTypes.Vector2:
                    return k_Vector2HashCode;
                case SerializableTypes.Vector3:
                    return k_Vector3HashCode;
                case SerializableTypes.Vector4:
                    return k_Vector4HashCode;
                case SerializableTypes.Vector2Int:
                    return k_Vector2IntHashCode;
                case SerializableTypes.Vector3Int:
                    return k_Vector3IntHashCode;
                case SerializableTypes.Quaternion:
                    return k_QuaternionHashCode;
                case SerializableTypes.Rect:
                    return k_RectHashCode;
                case SerializableTypes.RectInt:
                    return k_RectIntHashCode;
                case SerializableTypes.Color:
                    return k_ColorHashCode;
                case SerializableTypes.LayerMask:
                    return k_LayerMaskHashCode;
                case SerializableTypes.Bounds:
                    return k_BoundsHashCode;
                case SerializableTypes.BoundsInt:
                    return k_BoundsIntHashCode;
                case SerializableTypes.Hash128:
                    return k_Hash128HashCode;
                case SerializableTypes.Gradient:
                    return k_GradientHashCode;
                case SerializableTypes.AnimationCurve:
                    return k_AnimationCurveHashCode;
                case SerializableTypes.Object:
                    return k_ObjectHashCode;
                case SerializableTypes.EnumInt:
                    return k_EnumIntHashCode;
            }
            return k_InvalidHashCode;
        }

        public static string GetLabel(this SerializableTypes serializableType)
        {
            switch (serializableType)
            {
                case SerializableTypes.Invalid:
                    return k_Invalid;
                case SerializableTypes.String:
                    return k_String;
                case SerializableTypes.Char:
                    return k_Char;
                case SerializableTypes.Bool:
                    return k_Bool;
                case SerializableTypes.SByte:
                    return k_SByte;
                case SerializableTypes.Byte:
                    return k_Byte;
                case SerializableTypes.Short:
                    return k_Short;
                case SerializableTypes.UShort:
                    return k_UShort;
                case SerializableTypes.Int:
                    return k_Int;
                case SerializableTypes.UInt:
                    return k_UInt;
                case SerializableTypes.Long:
                    return k_Long;
                case SerializableTypes.ULong:
                    return k_ULong;
                case SerializableTypes.Float:
                    return k_Float;
                case SerializableTypes.Double:
                    return k_Double;
                case SerializableTypes.Vector2:
                    return k_Vector2;
                case SerializableTypes.Vector3:
                    return k_Vector3;
                case SerializableTypes.Vector4:
                    return k_Vector4;
                case SerializableTypes.Vector2Int:
                    return k_Vector2Int;
                case SerializableTypes.Vector3Int:
                    return k_Vector3Int;
                case SerializableTypes.Quaternion:
                    return k_Quaternion;
                case SerializableTypes.Rect:
                    return k_Rect;
                case SerializableTypes.RectInt:
                    return k_RectInt;
                case SerializableTypes.Color:
                    return k_Color;
                case SerializableTypes.LayerMask:
                    return k_LayerMask;
                case SerializableTypes.Bounds:
                    return k_Bounds;
                case SerializableTypes.BoundsInt:
                    return k_BoundsInt;
                case SerializableTypes.Hash128:
                    return k_Hash128;
                case SerializableTypes.Gradient:
                    return k_Gradient;
                case SerializableTypes.AnimationCurve:
                    return k_AnimationCurve;
                case SerializableTypes.Object:
                    return k_Object;
                case SerializableTypes.EnumInt:
                    return k_EnumInt;

            }
            return k_Invalid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetLabelFromTypeValue(int typeValue)
        {
            return GetLabel((SerializableTypes)typeValue);
        }

        public static SerializableTypes GetTypeFromHashCode(int hashCode)
        {
            if (hashCode == k_StringHashCode)
            {
                return SerializableTypes.String;
            }

            if (hashCode == k_CharHashCode)
            {
                return SerializableTypes.Char;
            }

            if (hashCode == k_BoolHashCode)
            {
                return SerializableTypes.Bool;
            }

            if (hashCode == k_SByteHashCode)
            {
                return SerializableTypes.SByte;
            }

            if (hashCode == k_ByteHashCode)
            {
                return SerializableTypes.Byte;
            }

            if (hashCode == k_ShortHashCode)
            {
                return SerializableTypes.Short;
            }

            if (hashCode == k_UShortHashCode)
            {
                return SerializableTypes.UShort;
            }

            if (hashCode == k_IntHashCode)
            {
                return SerializableTypes.Int;
            }

            if (hashCode == k_UIntHashCode)
            {
                return SerializableTypes.UInt;
            }

            if (hashCode == k_LongHashCode)
            {
                return SerializableTypes.Long;
            }

            if (hashCode == k_ULongHashCode)
            {
                return SerializableTypes.ULong;
            }

            if (hashCode == k_FloatHashCode)
            {
                return SerializableTypes.Float;
            }

            if (hashCode == k_DoubleHashCode)
            {
                return SerializableTypes.Double;
            }

            if (hashCode == k_Vector2HashCode)
            {
                return SerializableTypes.Vector2;
            }

            if (hashCode == k_Vector3HashCode)
            {
                return SerializableTypes.Vector3;
            }

            if (hashCode == k_Vector4HashCode)
            {
                return SerializableTypes.Vector4;
            }

            if (hashCode == k_Vector2IntHashCode)
            {
                return SerializableTypes.Vector2Int;
            }

            if (hashCode == k_Vector3IntHashCode)
            {
                return SerializableTypes.Vector3Int;
            }

            if (hashCode == k_QuaternionHashCode)
            {
                return SerializableTypes.Quaternion;
            }

            if (hashCode == k_RectHashCode)
            {
                return SerializableTypes.Rect;
            }

            if (hashCode == k_RectIntHashCode)
            {
                return SerializableTypes.RectInt;
            }

            if (hashCode == k_ColorHashCode)
            {
                return SerializableTypes.Color;
            }

            if (hashCode == k_LayerMaskHashCode)
            {
                return SerializableTypes.LayerMask;
            }

            if (hashCode == k_BoundsHashCode)
            {
                return SerializableTypes.Bounds;
            }

            if (hashCode == k_BoundsIntHashCode)
            {
                return SerializableTypes.BoundsInt;
            }

            if (hashCode == k_Hash128HashCode)
            {
                return SerializableTypes.Hash128;
            }

            if (hashCode == k_GradientHashCode)
            {
                return SerializableTypes.Gradient;
            }

            if (hashCode == k_AnimationCurveHashCode)
            {
                return SerializableTypes.AnimationCurve;
            }

            if (hashCode == k_ObjectHashCode)
            {
                return SerializableTypes.Object;
            }

            if (hashCode == k_EnumIntHashCode)
            {
                return SerializableTypes.EnumInt;
            }

            return SerializableTypes.Invalid;
        }

        public static SerializableTypes GetTypeFromLabel(string label)
        {
            return GetTypeFromHashCode(label.GetStableHashCode());
        }

        public static bool IsReferenceType(this SerializableTypes type)
        {
            switch (type)
            {
                case SerializableTypes.Gradient:
                case SerializableTypes.AnimationCurve:
                case SerializableTypes.Object:
                    return true;
                default:
                    return false;
            }
        }

    }
}