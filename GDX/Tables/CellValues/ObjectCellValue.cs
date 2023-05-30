// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using Object = UnityEngine.Object;

namespace GDX.Tables.CellValues
{
    /// <summary>
    ///     A <see cref="TableBase"/> <see cref="UnityEngine.Object"/> cell reference.
    /// </summary>
    [Serializable]
    public struct ObjectCellValue
    {
        Object m_CachedValue;
        ulong m_TableVersion;
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        public TableBase Table;
        public int ColumnIdentifier;
        public int RowIdentifier;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        public ObjectCellValue(TableBase table, int rowIdentifier, int columnIdentifier)
        {
            Table = table;
            RowIdentifier = rowIdentifier;
            ColumnIdentifier = columnIdentifier;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        /// <summary>
        ///     Get the <see cref="UnityEngine.Object" /> referenced from the <see cref="TableBase" />.
        /// </summary>
        /// <remarks>
        ///     This will evaluate if the version of the table matches the internally cached version, and will update
        ///     the cached reference if necessary.
        /// </remarks>
        /// <returns>An <see cref="UnityEngine.Object" />.</returns>
        public Object Get()
        {
            if (m_TableVersion == Table.GetDataVersion())
            {
                return m_CachedValue;
            }

            m_CachedValue = Table.GetObject(RowIdentifier, ColumnIdentifier);
            m_TableVersion = Table.GetDataVersion();
            return m_CachedValue;
        }

        /// <summary>
        ///     Get the internally cached version of the <see cref="Table" />'s data version.
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
        /// <returns>An <see cref="UnityEngine.Object" />.</returns>
        public Object GetUnsafe()
        {
            return m_CachedValue;
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="TableBase" />.
        /// </summary>
        /// <remarks>Updating the <see cref="TableBase" /> will update the cached table version.</remarks>
        /// <param name="newValue">An <see cref="UnityEngine.Object" />.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="TableBase" /> cell?</param>
        public void Set(Object newValue, bool updateTable = true)
        {
            m_CachedValue = newValue;
            if (updateTable)
            {
                m_TableVersion = Table.SetObject(RowIdentifier, ColumnIdentifier, newValue);
            }
        }

        /// <summary>
        ///     Get the <see cref="Serializable.SerializableTypes"/> which this struct supports.
        /// </summary>
        /// <returns>A <see cref="Serializable.SerializableTypes"/>.</returns>
        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.Object;
        }
    }
}