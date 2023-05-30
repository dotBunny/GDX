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
        public abstract void SetColumnName(string columnName, int columnIdentifier);
        public abstract string GetColumnName(int columnIdentifier);
        public abstract void SetTypeNameForObjectColumn(int columnIdentifier, string assemblyQualifiedName);
        public abstract string GetTypeNameForObjectColumn(int columnIdentifier);
        public abstract void SetRowName(string rowName, int rowIdentifier);
        public abstract string GetRowName(int rowIdentifier);
        public abstract string GetString(int rowIdentifier, int columnIdentifier);
        public abstract bool GetBool(int rowIdentifier, int columnIdentifier);
        public abstract char GetChar(int rowIdentifier, int columnIdentifier);
        public abstract sbyte GetSByte(int rowIdentifier, int columnIdentifier);
        public abstract byte GetByte(int rowIdentifier, int columnIdentifier);
        public abstract short GetShort(int rowIdentifier, int columnIdentifier);
        public abstract ushort GetUShort(int rowIdentifier, int columnIdentifier);
        public abstract int GetInt(int rowIdentifier, int columnIdentifier);
        public abstract uint GetUInt(int rowIdentifier, int columnIdentifier);
        public abstract long GetLong(int rowIdentifier, int columnIdentifier);
        public abstract ulong GetULong(int rowIdentifier, int columnIdentifier);
        public abstract float GetFloat(int rowIdentifier, int columnIdentifier);
        public abstract double GetDouble(int rowIdentifier, int columnIdentifier);
        public abstract Vector2 GetVector2(int rowIdentifier, int columnIdentifier);
        public abstract Vector3 GetVector3(int rowIdentifier, int columnIdentifier);
        public abstract Vector4 GetVector4(int rowIdentifier, int columnIdentifier);
        public abstract Vector2Int GetVector2Int(int rowIdentifier, int columnIdentifier);
        public abstract Vector3Int GetVector3Int(int rowIdentifier, int columnIdentifier);
        public abstract Quaternion GetQuaternion(int rowIdentifier, int columnIdentifier);
        public abstract Rect GetRect(int rowIdentifier, int columnIdentifier);
        public abstract RectInt GetRectInt(int rowIdentifier, int columnIdentifier);
        public abstract Color GetColor(int rowIdentifier, int columnIdentifier);
        public abstract LayerMask GetLayerMask(int rowIdentifier, int columnIdentifier);
        public abstract Bounds GetBounds(int rowIdentifier, int columnIdentifier);
        public abstract BoundsInt GetBoundsInt(int rowIdentifier, int columnIdentifier);
        public abstract Hash128 GetHash128(int rowIdentifier, int columnIdentifier);
        public abstract Gradient GetGradient(int rowIdentifier, int columnIdentifier);
        public abstract AnimationCurve GetAnimationCurve(int rowIdentifier, int columnIdentifier);
        public abstract Object GetObject(int rowIdentifier, int columnIdentifier);
        public abstract ulong SetString(int rowIdentifier, int columnIdentifier, string newValue);
        public abstract ulong SetBool(int rowIdentifier, int columnIdentifier, bool newValue);
        public abstract ulong SetChar(int rowIdentifier, int columnIdentifier, char newValue);
        public abstract ulong SetSByte(int rowIdentifier, int columnIdentifier, sbyte newValue);
        public abstract ulong SetByte(int rowIdentifier, int columnIdentifier, byte newValue);
        public abstract ulong SetShort(int rowIdentifier, int columnIdentifier, short newValue);
        public abstract ulong SetUShort(int rowIdentifier, int columnIdentifier, ushort newValue);
        public abstract ulong SetInt(int rowIdentifier, int columnIdentifier, int newValue);
        public abstract ulong SetUInt(int rowIdentifier, int columnIdentifier, uint newValue);
        public abstract ulong SetLong(int rowIdentifier, int columnIdentifier, long newValue);
        public abstract ulong SetULong(int rowIdentifier, int columnIdentifier, ulong newValue);
        public abstract ulong SetFloat(int rowIdentifier, int columnIdentifier, float newValue);
        public abstract ulong SetDouble(int rowIdentifier, int columnIdentifier, double newValue);
        public abstract ulong SetVector2(int rowIdentifier, int columnIdentifier, Vector2 newValue);
        public abstract ulong SetVector3(int rowIdentifier, int columnIdentifier, Vector3 newValue);
        public abstract ulong SetVector4(int rowIdentifier, int columnIdentifier, Vector4 value);
        public abstract ulong SetVector2Int(int rowIdentifier, int columnIdentifier, Vector2Int newValue);
        public abstract ulong SetVector3Int(int rowIdentifier, int columnIdentifier, Vector3Int newValue);
        public abstract ulong SetQuaternion(int rowIdentifier, int columnIdentifier, Quaternion newValue);
        public abstract ulong SetRect(int rowIdentifier, int columnIdentifier, Rect newValue);
        public abstract ulong SetRectInt(int rowIdentifier, int columnIdentifier, RectInt newValue);
        public abstract ulong SetColor(int rowIdentifier, int columnIdentifier, Color newValue);
        public abstract ulong SetLayerMask(int rowIdentifier, int columnIdentifier, LayerMask newValue);
        public abstract ulong SetBounds(int rowIdentifier, int columnIdentifier, Bounds newValue);
        public abstract ulong SetBoundsInt(int rowIdentifier, int columnIdentifier, BoundsInt newValue);
        public abstract ulong SetHash128(int rowIdentifier, int columnIdentifier, Hash128 newValue);
        public abstract ulong SetGradient(int rowIdentifier, int columnIdentifier, Gradient newValue);
        public abstract ulong SetAnimationCurve(int rowIdentifier, int columnIdentifier, AnimationCurve newValue);
        public abstract ulong SetObject(int rowIdentifier, int columnIdentifier, Object newValue);

        public struct ColumnDescription
        {
            public string Name;
            public int Identifier;
            public Serializable.SerializableTypes Type;
        }

        public struct RowDescription
        {
            public string Name;
            public int Identifier;
        }
    }
}