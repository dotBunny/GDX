﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    public struct RectCellValue
    {
        Rect m_CachedValue;
        public readonly ITable Table;
        public readonly int ColumnID;
        public readonly int RowID;
        ulong m_TableVersion;

        public RectCellValue(ITable table, int rowID, int columnID)
        {
            Table = table;
            RowID = rowID;
            ColumnID = columnID;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public Rect Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetRect(RowID, ColumnID);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public Rect GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(Rect value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetRect(RowID, ColumnID, value);
            }
        }
    }
}