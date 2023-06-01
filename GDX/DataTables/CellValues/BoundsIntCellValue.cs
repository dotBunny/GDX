// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.DataTables.CellValues
{
    /// <summary>
    ///     A <see cref="DataTableBase" /> <see cref="BoundsInt" /> cell reference.
    /// </summary>
    [Serializable]
    public struct BoundsIntCellValue
    {
        /// <summary>
        ///     The cached struct of the referenced <see cref="DataTable" /> cell.
        /// </summary>
        BoundsInt m_CachedValue;

        /// <summary>
        ///     The cached <see cref="DataTable" /> version found when last updating <see cref="m_CachedValue" />.
        /// </summary>
        ulong m_TableVersion;
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     The <see cref="DataTableBase" /> polled for cell data.
        /// </summary>
        public DataTableBase DataTable;

        /// <summary>
        ///     The unique column identifier used when accessing the <see cref="DataTable" />.
        /// </summary>
        public int ColumnIdentifier;

        /// <summary>
        ///     The unique row identifier used when accessing the <see cref="DataTable" />.
        /// </summary>
        public int RowIdentifier;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        /// <summary>
        ///     Creates a <see cref="BoundsIntCellValue" />.
        /// </summary>
        /// <param name="dataTable">The referenced <see cref="DataTableBase" />.</param>
        /// <param name="rowIdentifier">The unique row identifier to use when polling the <paramref name="dataTable" />.</param>
        /// <param name="columnIdentifier">The unique column identifier to use when polling the <paramref name="dataTable" />.</param>
        public BoundsIntCellValue(DataTableBase dataTable, int rowIdentifier, int columnIdentifier)
        {
            DataTable = dataTable;
            RowIdentifier = rowIdentifier;
            ColumnIdentifier = columnIdentifier;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        /// <summary>
        ///     Get the <see cref="BoundsInt" /> value referenced from the <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>
        ///     This will evaluate if the version of the table matches the internally cached version, and will update
        ///     the cached reference if necessary.
        /// </remarks>
        /// <returns>A <see cref="BoundsInt" /> struct.</returns>
        public BoundsInt Get()
        {
            if (m_TableVersion == DataTable.GetDataVersion())
            {
                return m_CachedValue;
            }

            m_CachedValue = DataTable.GetBoundsInt(RowIdentifier, ColumnIdentifier);
            m_TableVersion = DataTable.GetDataVersion();
            return m_CachedValue;
        }

        /// <summary>
        ///     Get the internally cached version of the <see cref="DataTableBase" />'s data version.
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
        /// <returns>A <see cref="BoundsInt" /> struct.</returns>
        public BoundsInt GetUnsafe()
        {
            return m_CachedValue;
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableBase" /> will update the cached table version.</remarks>
        /// <param name="newValue">A <see cref="BoundsInt" /> struct.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableBase" /> cell?</param>
        public void Set(BoundsInt newValue, bool updateTable = true)
        {
            m_CachedValue = newValue;
            if (updateTable)
            {
                m_TableVersion = DataTable.SetBoundsInt(RowIdentifier, ColumnIdentifier, newValue);
            }
        }

        /// <summary>
        ///     Get the <see cref="Serializable.SerializableTypes" /> which this struct supports.
        /// </summary>
        /// <returns>A <see cref="Serializable.SerializableTypes" />.</returns>
        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.BoundsInt;
        }
    }
}