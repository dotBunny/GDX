﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Tables.CellValues
{
    public struct DoubleCellValue
    {
        double m_CachedValue;
        public readonly ITable Table;
        public readonly int ColumnID;
        public readonly int RowID;
        ulong m_TableVersion;

        public DoubleCellValue(ITable table, int rowID, int columnID)
        {
            Table = table;
            RowID = rowID;
            ColumnID = columnID;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public double Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetDouble(RowID, ColumnID);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public double GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(double value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetDouble(RowID, ColumnID, value);
            }
        }
    }
}