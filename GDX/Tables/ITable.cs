// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables
{
    public interface ITable
    {
        int AddColumn(Serializable.SerializableTypes columnType, string columnName, int insertAt = -1);
        void RemoveColumn(Serializable.SerializableTypes columnType, int removeAt = -1);
        int AddRow(string rowName = null, int insertAt = -1);
        void RemoveRow(int removeAt);

        ulong GetDataVersion();

        /// <summary>
        ///     Returns the number of columns in the <see cref="ITable" />.
        /// </summary>
        /// <returns>A count of columns.</returns>
        int GetColumnCount();

        /// <summary>
        ///     Returns the number of rows in the <see cref="ITable" />.
        /// </summary>
        /// <returns>A count of rows.</returns>
        int GetRowCount();

        RowDescription[] GetAllRowDescriptions();
        RowDescription GetRowDescription(string name);
        RowDescription GetRowDescription(int order);
        /// <summary>
        /// Returns an ordered array of all columns descriptions
        /// </summary>
        /// <returns></returns>
        ColumnDescription[] GetAllColumnDescriptions();
        ColumnDescription GetColumnDescription(string name);
        ColumnDescription GetColumnDescription(int order);

        string GetString(int row, int column);
        bool GetBool(int row, int column);
        char GetChar(int row, int column);
        sbyte GetSByte(int row, int column);
        byte GetByte(int row, int column);
        short GetShort(int row, int column);
        ushort GetUShort(int row, int column);
        int GetInt(int row, int column);
        uint GetUInt(int row, int column);
        long GetLong(int row, int column);
        ulong GetULong(int row, int column);
        float GetFloat(int row, int column);
        double GetDouble(int row, int column);
        Vector2 GetVector2(int row, int column);
        Vector3 GetVector3(int row, int column);
        Vector4 GetVector4(int row, int column);
        Vector2Int GetVector2Int(int row, int column);
        Vector3Int GetVector3Int(int row, int column);
        Quaternion GetQuaternion(int row, int column);
        Rect GetRect(int row, int column);
        RectInt GetRectInt(int row, int column);
        Color GetColor(int row, int column);
        LayerMask GetLayerMask(int row, int column);
        Bounds GetBounds(int row, int column);
        BoundsInt GetBoundsInt(int row, int column);
        Hash128 GetHash128(int row, int column);
        Gradient GetGradient(int row, int column);
        AnimationCurve GetAnimationCurve(int row, int column);
        Object GetObject(int row, int column);

        ulong SetString(int row, int column, string value);
        ulong SetBool(int row, int column, bool value);
        ulong SetChar(int row, int column, char value);
        ulong SetSByte(int row, int column, sbyte value);
        ulong SetByte(int row, int column, byte value);
        ulong SetShort(int row, int column, short value);
        ulong SetUShort(int row, int column, ushort value);
        ulong SetInt(int row, int column, int value);
        ulong SetUInt(int row, int column, uint value);
        ulong SetLong(int row, int column, long value);
        ulong SetULong(int row, int column, ulong value);
        ulong SetFloat(int row, int column, float value);
        ulong SetDouble(int row, int column, double value);
        ulong SetVector2(int row, int column, Vector2 value);
        ulong SetVector3(int row, int column, Vector3 value);
        ulong SetVector4(int row, int column, Vector4 value);
        ulong SetVector2Int(int row, int column, Vector2Int value);
        ulong SetVector3Int(int row, int column, Vector3Int value);
        ulong SetQuaternion(int row, int column, Quaternion value);
        ulong SetRect(int row, int column, Rect value);
        ulong SetRectInt(int row, int column, RectInt value);
        ulong SetColor(int row, int column, Color value);
        ulong SetLayerMask(int row, int column, LayerMask value);
        ulong SetBounds(int row, int column, Bounds value);
        ulong SetBoundsInt(int row, int column, BoundsInt value);
        ulong SetHash128(int row, int column, Hash128 value);
        ulong SetGradient(int row, int column, Gradient value);
        ulong SetAnimationCurve(int row, int column, AnimationCurve value);
        ulong SetObject(int row, int column, Object value);

        public struct ColumnDescription
        {
            public string Name;
            public int Index;
            public Serializable.SerializableTypes Type;
        }

        public struct RowDescription
        {
            public string Name;
            public int Index;
        }
    }
}