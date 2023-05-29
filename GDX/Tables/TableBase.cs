// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables
{
    public abstract class TableBase : ScriptableObject
    {
        public enum Flags
        {
            EnableUndo = 0
        }

        public abstract int AddColumn(Serializable.SerializableTypes columnType, string columnName, int insertAt = -1);
        public abstract void RemoveColumn(Serializable.SerializableTypes columnType, int removeAt = -1);
        public abstract int AddRow(string rowName = null, int insertAt = -1);
        public abstract void RemoveRow(int removeAt);
        public abstract ulong GetDataVersion();

        /// <summary>
        ///     Returns the number of columns in the <see cref="!:ITable" />.
        /// </summary>
        /// <returns>A count of columns.</returns>
        public abstract int GetColumnCount();

        /// <summary>
        ///     Returns the number of rows in the <see cref="!:ITable" />.
        /// </summary>
        /// <returns>A count of rows.</returns>
        public abstract int GetRowCount();

        public abstract string GetDisplayName();
        public abstract void SetDisplayName(string displayName);
        public abstract bool GetFlag(Flags flag);
        public abstract void SetFlag(Flags flag, bool toggle);
        public abstract RowDescription[] GetAllRowDescriptions();
        public abstract RowDescription GetRowDescription(string name);
        public abstract RowDescription GetRowDescription(int order);
        public abstract void SetAllRowDescriptionsOrder(RowDescription[] orderedRows);

        /// <summary>
        /// Returns an ordered array of all columns descriptions
        /// </summary>
        /// <returns></returns>
        public abstract ColumnDescription[] GetAllColumnDescriptions();

        public abstract ColumnDescription GetColumnDescription(string name);
        public abstract ColumnDescription GetColumnDescription(int order);
        public abstract void SetAllColumnDescriptionsOrder(ColumnDescription[] orderedColumns);
        public abstract void SetColumnName(string columnName, int column);
        public abstract string GetColumnName(int column);
        public abstract void SetRowName(string rowName, int row);
        public abstract string GetRowName(int row);
        public abstract string GetString(int row, int column);
        public abstract bool GetBool(int row, int column);
        public abstract char GetChar(int row, int column);
        public abstract sbyte GetSByte(int row, int column);
        public abstract byte GetByte(int row, int column);
        public abstract short GetShort(int row, int column);
        public abstract ushort GetUShort(int row, int column);
        public abstract int GetInt(int row, int column);
        public abstract uint GetUInt(int row, int column);
        public abstract long GetLong(int row, int column);
        public abstract ulong GetULong(int row, int column);
        public abstract float GetFloat(int row, int column);
        public abstract double GetDouble(int row, int column);
        public abstract Vector2 GetVector2(int row, int column);
        public abstract Vector3 GetVector3(int row, int column);
        public abstract Vector4 GetVector4(int row, int column);
        public abstract Vector2Int GetVector2Int(int row, int column);
        public abstract Vector3Int GetVector3Int(int row, int column);
        public abstract Quaternion GetQuaternion(int row, int column);
        public abstract Rect GetRect(int row, int column);
        public abstract RectInt GetRectInt(int row, int column);
        public abstract Color GetColor(int row, int column);
        public abstract LayerMask GetLayerMask(int row, int column);
        public abstract Bounds GetBounds(int row, int column);
        public abstract BoundsInt GetBoundsInt(int row, int column);
        public abstract Hash128 GetHash128(int row, int column);
        public abstract Gradient GetGradient(int row, int column);
        public abstract AnimationCurve GetAnimationCurve(int row, int column);
        public abstract Object GetObject(int row, int column);
        public abstract ulong SetString(int row, int column, string value);
        public abstract ulong SetBool(int row, int column, bool value);
        public abstract ulong SetChar(int row, int column, char value);
        public abstract ulong SetSByte(int row, int column, sbyte value);
        public abstract ulong SetByte(int row, int column, byte value);
        public abstract ulong SetShort(int row, int column, short value);
        public abstract ulong SetUShort(int row, int column, ushort value);
        public abstract ulong SetInt(int row, int column, int value);
        public abstract ulong SetUInt(int row, int column, uint value);
        public abstract ulong SetLong(int row, int column, long value);
        public abstract ulong SetULong(int row, int column, ulong value);
        public abstract ulong SetFloat(int row, int column, float value);
        public abstract ulong SetDouble(int row, int column, double value);
        public abstract ulong SetVector2(int row, int column, Vector2 value);
        public abstract ulong SetVector3(int row, int column, Vector3 value);
        public abstract ulong SetVector4(int row, int column, Vector4 value);
        public abstract ulong SetVector2Int(int row, int column, Vector2Int value);
        public abstract ulong SetVector3Int(int row, int column, Vector3Int value);
        public abstract ulong SetQuaternion(int row, int column, Quaternion value);
        public abstract ulong SetRect(int row, int column, Rect value);
        public abstract ulong SetRectInt(int row, int column, RectInt value);
        public abstract ulong SetColor(int row, int column, Color value);
        public abstract ulong SetLayerMask(int row, int column, LayerMask value);
        public abstract ulong SetBounds(int row, int column, Bounds value);
        public abstract ulong SetBoundsInt(int row, int column, BoundsInt value);
        public abstract ulong SetHash128(int row, int column, Hash128 value);
        public abstract ulong SetGradient(int row, int column, Gradient value);
        public abstract ulong SetAnimationCurve(int row, int column, AnimationCurve value);
        public abstract ulong SetObject(int row, int column, Object value);

        public struct ColumnDescription
        {
            public string Name;
            public int InternalIndex;
            public Serializable.SerializableTypes Type;
        }

        public struct RowDescription
        {
            public string Name;
            public int InternalIndex;
        }
    }
}