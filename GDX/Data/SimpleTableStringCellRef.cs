// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.Data
{
    public struct SimpleTableCellStringRef
    {
        string m_CachedValue;
        public readonly SimpleTable Table;
        public readonly int ColumnID;
        public readonly int RowID;
        ulong m_TableVersion;

        public SimpleTableCellStringRef(SimpleTable table, int rowID, int columnID)
        {
            Table = table;
            RowID = rowID;
            ColumnID = columnID;
            m_TableVersion = 0;
            m_CachedValue = null;
            GetValue();
        }

        public string GetValue()
        {
            if (m_TableVersion != Table.dataVersion)
            {
                m_CachedValue = Table.GetString(RowID, ColumnID);
                m_TableVersion = Table.dataVersion;
            }
            return m_CachedValue;
        }

        public string GetUnsafeValue()
        {
            return m_CachedValue;
        }
    }
}