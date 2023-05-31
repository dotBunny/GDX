// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.DataTables.CellValues
{
    /// <summary>
    ///     A <see cref="DataTableObject" /> <see cref="uint" /> cell reference.
    /// </summary>
    [Serializable]
    public struct UIntCellValue
    {
        /// <summary>
        ///     The cached value of the referenced <see cref="DataTable"/> cell.
        /// </summary>
        uint m_CachedValue;
        /// <summary>
        ///     The cached <see cref="DataTable"/> version found when last updating <see cref="m_CachedValue"/>.
        /// </summary>
        ulong m_TableVersion;
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     The <see cref="DataTableObject" /> polled for cell data.
        /// </summary>
        public DataTableObject DataTable;
        /// <summary>
        ///     The unique column identifier used when accessing the <see cref="DataTable"/>.
        /// </summary>
        public int ColumnIdentifier;
        /// <summary>
        ///     The unique row identifier used when accessing the <see cref="DataTable"/>.
        /// </summary>
        public int RowIdentifier;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        /// <summary>
        ///     Creates a <see cref="UIntCellValue" />.
        /// </summary>
        /// <param name="dataTable">The referenced <see cref="DataTableObject" />.</param>
        /// <param name="rowIdentifier">The unique row identifier to use when polling the <paramref name="dataTable" />.</param>
        /// <param name="columnIdentifier">The unique column identifier to use when polling the <paramref name="dataTable" />.</param>
        public UIntCellValue(DataTableObject dataTable, int rowIdentifier, int columnIdentifier)
        {
            DataTable = dataTable;
            RowIdentifier = rowIdentifier;
            ColumnIdentifier = columnIdentifier;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        /// <summary>
        ///     Get the <see cref="uint" /> value referenced from the <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>
        ///     This will evaluate if the version of the table matches the internally cached version, and will update
        ///     the cached reference if necessary.
        /// </remarks>
        /// <returns>A <see cref="uint" /> value.</returns>
        public uint Get()
        {
            if (m_TableVersion == DataTable.GetDataVersion())
            {
                return m_CachedValue;
            }

            m_CachedValue = DataTable.GetUInt(RowIdentifier, ColumnIdentifier);
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
        /// <returns>A <see cref="uint" /> value.</returns>
        public uint GetUnsafe()
        {
            return m_CachedValue;
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableObject" /> will update the cached table version.</remarks>
        /// <param name="newValue">A <see cref="uint" /> value.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableObject" /> cell?</param>
        public void Set(uint newValue, bool updateTable = true)
        {
            m_CachedValue = newValue;
            if (updateTable)
            {
                m_TableVersion = DataTable.SetUInt(RowIdentifier, ColumnIdentifier, newValue);
            }
        }

        /// <summary>
        ///     Get the <see cref="Serializable.SerializableTypes" /> which this struct supports.
        /// </summary>
        /// <returns>A <see cref="Serializable.SerializableTypes" />.</returns>
        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.UInt;
        }
    }
}