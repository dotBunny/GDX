// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Tables.CellValues
{
    [System.Serializable]
    public struct IntCellValue
    {
        int m_CachedValue;
        public readonly ITable Table;
        public readonly int Column;
        public readonly int Row;
        ulong m_TableVersion;

        public IntCellValue(ITable table, int row, int column)
        {
            Table = table;
            Row = row;
            Column = column;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public int Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetInt(Row, Column);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public int GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(int value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetInt(Row, Column, value);
            }
        }
    }
}