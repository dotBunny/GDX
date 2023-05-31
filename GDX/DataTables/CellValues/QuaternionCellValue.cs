// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.DataTables.CellValues
{
    /// <summary>
    ///     A <see cref="DataTableObject" /> <see cref="Quaternion" /> cell reference.
    /// </summary>
    [Serializable]
    public struct QuaternionCellValue
    {
        Quaternion m_CachedValue;
        ulong m_TableVersion;
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        public DataTableObject DataTable;
        public int ColumnIdentifier;
        public int RowIdentifier;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        public QuaternionCellValue(DataTableObject dataTable, int rowIdentifier, int columnIdentifier)
        {
            DataTable = dataTable;
            RowIdentifier = rowIdentifier;
            ColumnIdentifier = columnIdentifier;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        /// <summary>
        ///     Get the <see cref="Quaternion" /> value referenced from the <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>
        ///     This will evaluate if the version of the table matches the internally cached version, and will update
        ///     the cached reference if necessary.
        /// </remarks>
        /// <returns>A <see cref="Quaternion" /> struct.</returns>
        public Quaternion Get()
        {
            if (m_TableVersion == DataTable.GetDataVersion())
            {
                return m_CachedValue;
            }

            m_CachedValue = DataTable.GetQuaternion(RowIdentifier, ColumnIdentifier);
            m_TableVersion = DataTable.GetDataVersion();
            return m_CachedValue;
        }

        /// <summary>
        ///     Get the internally cached version of the <see cref="DataTableObject" />'s data version.
        /// </summary>
        /// <returns>A version number.</returns>
        public ulong GetDataVersion()
        {
            return m_TableVersion;
        }

        /// <summary>
        ///     Get the cached value without a version check.
        /// </summary>
        /// <remarks>
        ///     This can respond with a default value if a <see cref="Get" /> call has not been made yet to populate the
        ///     internally cached value.
        /// </remarks>
        /// <returns>A <see cref="Quaternion" /> struct.</returns>
        public Quaternion GetUnsafe()
        {
            return m_CachedValue;
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableObject" /> will update the cached table version.</remarks>
        /// <param name="eulerAngles">A <see cref="Vector3" /> struct.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableObject" /> cell?</param>
        public void Set(Vector3 eulerAngles, bool updateTable = true)
        {
            Set(Quaternion.Euler(eulerAngles), updateTable);
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableObject" /> will update the cached table version.</remarks>
        /// <param name="newValue">A <see cref="Vector4" /> struct.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableObject" /> cell?</param>
        public void Set(Vector4 newValue, bool updateTable = true)
        {
            Set(new Quaternion(newValue.x, newValue.y, newValue.z, newValue.w), updateTable);
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableObject" /> will update the cached table version.</remarks>
        /// <param name="newValue">A <see cref="Quaternion" /> struct.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableObject" /> cell?</param>
        public void Set(Quaternion newValue, bool updateTable = true)
        {
            m_CachedValue = newValue;
            if (updateTable)
            {
                m_TableVersion = DataTable.SetQuaternion(RowIdentifier, ColumnIdentifier, newValue);
            }
        }

        /// <summary>
        ///     Get the <see cref="Serializable.SerializableTypes" /> which this struct supports.
        /// </summary>
        /// <returns>A <see cref="Serializable.SerializableTypes" />.</returns>
        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.Quaternion;
        }
    }
}