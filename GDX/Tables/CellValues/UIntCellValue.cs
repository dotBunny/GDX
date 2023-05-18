﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Tables.CellValues
{
    public struct UIntCellValue
    {
        uint m_CachedValue;
        public readonly ITable Table;
        public readonly int ColumnID;
        public readonly int RowID;
        ulong m_TableVersion;

        public UIntCellValue(ITable table, int rowID, int columnID)
        {
            Table = table;
            RowID = rowID;
            ColumnID = columnID;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public uint Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetUInt(RowID, ColumnID);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public uint GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(uint value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetUInt(RowID, ColumnID, value);
            }
        }
    }
}