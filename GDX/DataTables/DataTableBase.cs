// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.DataTables
{
    /// <summary>
    ///     The base framework of a <see cref="ScriptableObject" /> backed DataTable.
    /// </summary>
    /// <remarks>
    ///     Similar to UE's https://docs.unrealengine.com/5.2/en-US/data-driven-gameplay-elements-in-unreal-engine/.
    /// </remarks>
    public abstract class DataTableBase : ScriptableObject
    {
        /// <summary>
        ///     A collection of indices that can be passed to <see cref="DataTableBase.SetFlag" /> to toggle settings.
        /// </summary>
        public enum Settings
        {
            /// <summary>
            ///     Does the <see cref="DataTableBase" /> support Unity's undo feature?
            /// </summary>
            EnableUndo = 0
        }

        /// <summary>
        ///     The assembly qualified name for <see cref="UnityEngine.Object" />
        /// </summary>
        protected static readonly string UnityObjectName = typeof(Object).AssemblyQualifiedName;

        /// <summary>
        ///     Add a column.
        /// </summary>
        /// <param name="columnType">The type of data being stored in the column.</param>
        /// <param name="columnName">The user-friendly name of the column.</param>
        /// <param name="insertAtColumnIdentifier">
        ///     The column identifier to insert the column at, otherwise -1 will place the
        ///     column at the end.
        /// </param>
        /// <returns>The unique column identifier of the created column.</returns>
        public abstract int AddColumn(Serializable.SerializableTypes columnType, string columnName,
            int insertAtColumnIdentifier = -1);

        /// <summary>
        ///     Remove a column.
        /// </summary>
        /// <param name="columnType">The type of data being stored in the column.</param>
        /// <param name="columnIdentifier">The known column's identifier.</param>
        public abstract void RemoveColumn(Serializable.SerializableTypes columnType, int columnIdentifier = -1);

        /// <summary>
        ///     Add a row
        /// </summary>
        /// <param name="rowName">The user-friendly name of the column.</param>
        /// <param name="insertAtRowIdentifier">
        ///     The row identifier to insert the row at, otherwise -1 will place the row at the
        ///     end.
        /// </param>
        /// <returns>The unique row identifier of the created row.</returns>
        public abstract int AddRow(string rowName = null, int insertAtRowIdentifier = -1);

        /// <summary>
        ///     Remove a row.
        /// </summary>
        /// <param name="rowIdentifier">The known row's identifier.</param>
        public abstract void RemoveRow(int rowIdentifier);

        /// <summary>
        ///     Get the internally stored data version for the <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>
        ///     Every time something changes in the table, be it structural or data, this value is changed. This allows
        ///     for checks of if cached values need to be re-polled.
        /// </remarks>
        /// <returns></returns>
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

        /// <summary>
        ///     Get the user-assigned display name for the <see cref="DataTableBase" />.
        /// </summary>
        /// <returns></returns>
        public abstract string GetDisplayName();

        /// <summary>
        ///     Set the user-assigned display name for the <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="displayName"></param>
        public abstract void SetDisplayName(string displayName);

        /// <summary>
        ///     Get the given flag status based on the provided <see cref="Settings" /> index.
        /// </summary>
        /// <param name="setting">The desired index to check.</param>
        /// <returns>The true/false status of the requested setting.</returns>
        public abstract bool GetFlag(Settings setting);

        /// <summary>
        ///     Set the given flag status based on the provided <see cref="Settings" /> index.
        /// </summary>
        /// <param name="setting">The desired index to set.</param>
        /// <param name="toggle">The new value of the flag.</param>
        public abstract void SetFlag(Settings setting, bool toggle);

        /// <summary>
        ///     Get all rows' <see cref="RowDescription" />; ordered.
        /// </summary>
        /// <returns>An array of <see cref="RowDescription" />s.</returns>
        public abstract RowDescription[] GetAllRowDescriptions();

        /// <summary>
        ///     Get a <see cref="RowDescription" /> describing a row in the specified position.
        /// </summary>
        /// <param name="order">The ordered index/position.</param>
        /// <returns>A <see cref="RowDescription" /> for the target row.</returns>
        public abstract RowDescription GetRowDescription(int order);

        /// <summary>
        ///     Get all columns' <see cref="ColumnDescription" />; ordered.
        /// </summary>
        /// <returns>An array of <see cref="ColumnDescription" />s.</returns>
        public abstract ColumnDescription[] GetAllColumnDescriptions();

        /// <summary>
        ///     Get a <see cref="ColumnDescription" /> describing a column in the specified position.
        /// </summary>
        /// <param name="order">The ordered index/position.</param>
        /// <returns>A <see cref="ColumnDescription" /> for the target column.</returns>
        public abstract ColumnDescription GetColumnDescription(int order);

        /// <summary>
        ///     Set the specified row's order.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="newSortOrder">The ordered index/position.</param>
        public abstract void SetRowOrder(int rowIdentifier, int newSortOrder);

        /// <summary>
        ///     Set the order of rows in the <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="orderedRowIdentifiers">An array of row unique identifiers, in the order to be set.</param>
        public abstract void SetAllRowOrders(int[] orderedRowIdentifiers);

        /// <summary>
        ///     Set the specified column's order.
        /// </summary>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newSortOrder">The ordered index/position.</param>
        public abstract void SetColumnOrder(int columnIdentifier, int newSortOrder);

        /// <summary>
        ///     Set the order of columns in the <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="orderedColumnIdentifiers">An array of column unique identifiers, in the order to be set.</param>
        public abstract void SetAllColumnOrders(int[] orderedColumnIdentifiers);

        /// <summary>
        ///     Set the type's assembly qualified name for the specified <see cref="UnityEngine.Object" /> column.
        /// </summary>
        /// <remarks>
        ///     More info can be found at
        ///     https://learn.microsoft.com/en-us/dotnet/api/system.type.assemblyqualifiedname?view=net-7.0.
        ///     This allows for filtering of the generated fields.
        /// </remarks>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="assemblyQualifiedName">The assembly qualified name.</param>
        public abstract void SetTypeNameForObjectColumn(int columnIdentifier, string assemblyQualifiedName);

        /// <summary>
        ///     Get the type's assembly qualified name for the specified <see cref="UnityEngine.Object" /> column.
        /// </summary>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>An assembly qualified name.</returns>
        public abstract string GetTypeNameForObjectColumn(int columnIdentifier);

        /// <summary>
        ///     Set the user-friendly name of the identified column.
        /// </summary>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="columnName">The desired user-friendly name for the column.</param>
        public abstract void SetColumnName(int columnIdentifier, string columnName);

        /// <summary>
        ///     Get the user-friendly name of the identified column.
        /// </summary>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A user-friendly name.</returns>
        public abstract string GetColumnName(int columnIdentifier);

        /// <summary>
        ///     Set the user-friendly name of the identified row.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="rowName">The desired user-friendly name for the row.</param>
        public abstract void SetRowName(int rowIdentifier, string rowName);

        /// <summary>
        ///     Get the user-friendly name of the identified row.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <returns>A user-friendly name.</returns>
        public abstract string GetRowName(int rowIdentifier);

        /// <summary>
        ///     Get a <see cref="string" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="string" /> value.</returns>
        public abstract string GetString(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="bool" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="bool" /> value.</returns>
        public abstract bool GetBool(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="char" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="char" /> value.</returns>
        public abstract char GetChar(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="sbyte" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="sbyte" /> value.</returns>
        public abstract sbyte GetSByte(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="byte" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="byte" /> value.</returns>
        public abstract byte GetByte(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="short" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="short" /> value.</returns>
        public abstract short GetShort(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="ushort" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="ushort" /> value.</returns>
        public abstract ushort GetUShort(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="int" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="int" /> value.</returns>
        public abstract int GetInt(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="uint" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="uint" /> value.</returns>
        public abstract uint GetUInt(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="long" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="long" /> value.</returns>
        public abstract long GetLong(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="ulong" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="ulong" /> value.</returns>
        public abstract ulong GetULong(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="float" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="float" /> value.</returns>
        public abstract float GetFloat(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="double" /> value from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="double" /> value.</returns>
        public abstract double GetDouble(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Vector2" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Vector2" /> struct.</returns>
        public abstract Vector2 GetVector2(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Vector3" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Vector3" /> struct.</returns>
        public abstract Vector3 GetVector3(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Vector4" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Vector4" /> struct.</returns>
        public abstract Vector4 GetVector4(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Vector2Int" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Vector2Int" /> struct.</returns>
        public abstract Vector2Int GetVector2Int(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Vector3Int" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Vector3Int" /> struct.</returns>
        public abstract Vector3Int GetVector3Int(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Quaternion" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Quaternion" /> struct.</returns>
        public abstract Quaternion GetQuaternion(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Rect" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Rect" /> struct.</returns>
        public abstract Rect GetRect(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="RectInt" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="RectInt" /> struct.</returns>
        public abstract RectInt GetRectInt(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Color" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Color" /> struct.</returns>
        public abstract Color GetColor(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="LayerMask" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="LayerMask" /> struct.</returns>
        public abstract LayerMask GetLayerMask(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Bounds" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Bounds" /> struct.</returns>
        public abstract Bounds GetBounds(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="BoundsInt" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="BoundsInt" /> struct.</returns>
        public abstract BoundsInt GetBoundsInt(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Hash128" /> struct from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Hash128" /> struct.</returns>
        public abstract Hash128 GetHash128(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="Gradient" /> object from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="Gradient" /> object.</returns>
        public abstract Gradient GetGradient(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get a <see cref="AnimationCurve" /> object from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>A <see cref="AnimationCurve" /> object.</returns>
        public abstract AnimationCurve GetAnimationCurve(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Get an <see cref="UnityEngine.Object" /> object from the specified cell.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <returns>An <see cref="UnityEngine.Object" />.</returns>
        public abstract Object GetObject(int rowIdentifier, int columnIdentifier);

        /// <summary>
        ///     Sets the specified cell's <see cref="string" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetString(int rowIdentifier, int columnIdentifier, string newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="bool" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetBool(int rowIdentifier, int columnIdentifier, bool newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="char" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetChar(int rowIdentifier, int columnIdentifier, char newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="sbyte" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetSByte(int rowIdentifier, int columnIdentifier, sbyte newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="byte" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetByte(int rowIdentifier, int columnIdentifier, byte newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="short" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetShort(int rowIdentifier, int columnIdentifier, short newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="ushort" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetUShort(int rowIdentifier, int columnIdentifier, ushort newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="int" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetInt(int rowIdentifier, int columnIdentifier, int newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="uint" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetUInt(int rowIdentifier, int columnIdentifier, uint newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="long" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetLong(int rowIdentifier, int columnIdentifier, long newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="ulong" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetULong(int rowIdentifier, int columnIdentifier, ulong newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="float" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetFloat(int rowIdentifier, int columnIdentifier, float newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="double" /> value.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newValue">The updated value.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetDouble(int rowIdentifier, int columnIdentifier, double newValue);

        /// <summary>
        ///     Sets the specified cell's <see cref="Vector2" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetVector2(int rowIdentifier, int columnIdentifier, Vector2 newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Vector3" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetVector3(int rowIdentifier, int columnIdentifier, Vector3 newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Vector4" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetVector4(int rowIdentifier, int columnIdentifier, Vector4 newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetVector2Int(int rowIdentifier, int columnIdentifier, Vector2Int newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Vector3Int" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetVector3Int(int rowIdentifier, int columnIdentifier, Vector3Int newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Quaternion" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetQuaternion(int rowIdentifier, int columnIdentifier, Quaternion newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Rect" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetRect(int rowIdentifier, int columnIdentifier, Rect newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="RectInt" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetRectInt(int rowIdentifier, int columnIdentifier, RectInt newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Color" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetColor(int rowIdentifier, int columnIdentifier, Color newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="LayerMask" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetLayerMask(int rowIdentifier, int columnIdentifier, LayerMask newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Bounds" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetBounds(int rowIdentifier, int columnIdentifier, Bounds newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="BoundsInt" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetBoundsInt(int rowIdentifier, int columnIdentifier, BoundsInt newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Hash128" /> struct.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newStruct">The updated struct.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetHash128(int rowIdentifier, int columnIdentifier, Hash128 newStruct);

        /// <summary>
        ///     Sets the specified cell's <see cref="Gradient" /> object.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newObject">The updated object.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetGradient(int rowIdentifier, int columnIdentifier, Gradient newObject);

        /// <summary>
        ///     Sets the specified cell's <see cref="AnimationCurve" /> object.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newObject">The updated object.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetAnimationCurve(int rowIdentifier, int columnIdentifier, AnimationCurve newObject);

        /// <summary>
        ///     Sets the specified cell's <see cref="UnityEngine.Object" /> object.
        /// </summary>
        /// <param name="rowIdentifier">The unique row identifier.</param>
        /// <param name="columnIdentifier">The unique column identifier.</param>
        /// <param name="newObject">The updated object.</param>
        /// <returns>The <see cref="DataTableBase" />'s updated data version.</returns>
        public abstract ulong SetObject(int rowIdentifier, int columnIdentifier, Object newObject);
    }
}