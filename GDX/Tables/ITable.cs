// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GDX.Tables
{
    public interface ITable
    {
        public struct ColumnEntry
        {
            public string Name;
            public int Id;
            public ColumnType Type;
        }

        public enum ColumnType
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
            Count
        }



        ulong GetDataVersion();

        string GetString(int row, int columnID);
        bool GetBool(int row, int columnID);
        char GetChar(int row, int columnID);
        sbyte GetSByte(int row, int columnID);
        byte GetByte(int row, int columnID);
        short GetShort(int row, int columnID);
        ushort GetUShort(int row, int columnID);
        int GetInt(int row, int columnID);
        uint GetUInt(int row, int columnID);
        long GetLong(int row, int columnID);
        ulong GetULong(int row, int columnID);
        float GetFloat(int row, int columnID);
        double GetDouble(int row, int columnID);
        Vector2 GetVector2(int row, int columnID);
        Vector3 GetVector3(int row, int columnID);
        Vector4 GetVector4(int row, int columnID);
        Vector2Int GetVector2Int(int row, int columnID);
        Vector3Int GetVector3Int(int row, int columnID);
        Quaternion GetQuaternion(int row, int columnID);
        Rect GetRect(int row, int columnID);
        RectInt GetRectInt(int row, int columnID);
        Color GetColor(int row, int columnID);
        LayerMask GetLayerMask(int row, int columnID);
        Bounds GetBounds(int row, int columnID);
        BoundsInt GetBoundsInt(int row, int columnID);
        Hash128 GetHash128(int row, int columnID);
        Gradient GetGradient(int row, int columnID);
        AnimationCurve GetAnimationCurve(int row, int columnID);
        Object GetObject(int row, int columnID);

        ulong SetString(int row, int columnID, string value);
        ulong SetBool(int row, int columnID, bool value);
        ulong SetChar(int row, int columnID, char value);
        ulong SetSByte(int row, int columnID, sbyte value);
        ulong SetByte(int row, int columnID, byte value);
        ulong SetShort(int row, int columnID, short value);
        ulong SetUShort(int row, int columnID, ushort value);
        ulong SetInt(int row, int columnID, int value);
        ulong SetUInt(int row, int columnID, uint value);
        ulong SetLong(int row, int columnID, long value);
        ulong SetULong(int row, int columnID, ulong value);
        ulong SetFloat(int row, int columnID, float value);
        ulong SetDouble(int row, int columnID, double value);
        ulong SetVector2(int row, int columnID, Vector2 value);
        ulong SetVector3(int row, int columnID, Vector3 value);
        ulong SetVector4(int row, int columnID, Vector4 value);
        ulong SetVector2Int(int row, int columnID, Vector2Int value);
        ulong SetVector3Int(int row, int columnID, Vector3Int value);
        ulong SetQuaternion(int row, int columnID, Quaternion value);
        ulong SetRect(int row, int columnID, Rect value);
        ulong SetRectInt(int row, int columnID, RectInt value);
        ulong SetColor(int row, int columnID, Color value);
        ulong SetLayerMask(int row, int columnID, LayerMask value);
        ulong SetBounds(int row, int columnID, Bounds value);
        ulong SetBoundsInt(int row, int columnID, BoundsInt value);
        ulong SetHash128(int row, int columnID, Hash128 value);
        ulong SetGradient(int row, int columnID, Gradient value);
        ulong SetAnimationCurve(int row, int columnID, AnimationCurve value);
        ulong SetObject(int row, int columnID, Object value);
    }
}