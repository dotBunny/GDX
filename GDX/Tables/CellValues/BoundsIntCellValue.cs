﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    public struct BoundsIntCellValue
    {
        BoundsInt m_CachedValue;
        public readonly ITable Table;
        public readonly int ColumnID;
        public readonly int RowID;
        ulong m_TableVersion;

        public BoundsIntCellValue(ITable table, int rowID, int columnID)
        {
            Table = table;
            RowID = rowID;
            ColumnID = columnID;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public BoundsInt Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetBoundsInt(RowID, ColumnID);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public BoundsInt GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(BoundsInt value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetBoundsInt(RowID, ColumnID, value);
            }
        }
    }
}