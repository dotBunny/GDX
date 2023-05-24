// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    [System.Serializable]
    public struct Vector2IntCellValue
    {
        Vector2Int m_CachedValue;
        public readonly ITable Table;
        public readonly int Column;
        public readonly int Row;
        ulong m_TableVersion;

        public Vector2IntCellValue(ITable table, int row, int column)
        {
            Table = table;
            Row = row;
            Column = column;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public Vector2Int Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetVector2Int(Row, Column);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public Vector2Int GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(Vector2Int value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetVector2Int(Row, Column, value);
            }
        }
    }
}