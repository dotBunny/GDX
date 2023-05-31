﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.DataTables.CellValues
{
    /// <summary>
    ///     A <see cref="DataTableObject" /> <see cref="AnimationCurve" /> cell reference.
    /// </summary>
    [Serializable]
    public struct AnimationCurveCellValue
    {
        AnimationCurve m_CachedValue;
        ulong m_TableVersion;
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        public DataTableObject DataTable;
        public int ColumnIdentifier;
        public int RowIdentifier;
        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        public AnimationCurveCellValue(DataTableObject dataTable, int rowIdentifier, int columnIdentifier)
        {
            DataTable = dataTable;
            RowIdentifier = rowIdentifier;
            ColumnIdentifier = columnIdentifier;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        /// <summary>
        ///     Get the <see cref="AnimationCurve" /> object referenced from the <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>
        ///     This will evaluate if the version of the table matches the internally cached version, and will update
        ///     the cached reference if necessary.
        /// </remarks>
        /// <returns>An <see cref="AnimationCurve" /> object.</returns>
        public AnimationCurve Get()
        {
            if (m_TableVersion == DataTable.GetDataVersion())
            {
                return m_CachedValue;
            }

            m_CachedValue = DataTable.GetAnimationCurve(RowIdentifier, ColumnIdentifier);
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
        /// <returns>An <see cref="AnimationCurve" /> object.</returns>
        public AnimationCurve GetUnsafe()
        {
            return m_CachedValue;
        }

        /// <summary>
        ///     Sets the cached value of the struct and by default, updates the associated <see cref="DataTableObject" />.
        /// </summary>
        /// <remarks>Updating the <see cref="DataTableObject" /> will update the cached table version.</remarks>
        /// <param name="newValue">An <see cref="AnimationCurve" /> object.</param>
        /// <param name="updateTable">Should the value be pushed back to the referenced <see cref="DataTableObject" /> cell?</param>
        public void Set(AnimationCurve newValue, bool updateTable = true)
        {
            m_CachedValue = newValue;
            if (updateTable)
            {
                m_TableVersion = DataTable.SetAnimationCurve(RowIdentifier, ColumnIdentifier, newValue);
            }
        }

        /// <summary>
        ///     Get the <see cref="Serializable.SerializableTypes" /> which this struct supports.
        /// </summary>
        /// <returns>A <see cref="Serializable.SerializableTypes" />.</returns>
        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.AnimationCurve;
        }
    }
}