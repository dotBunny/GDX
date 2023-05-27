// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    [System.Serializable]
    public struct SByteCellValue
    {
        sbyte m_CachedValue;
        public TableBase Table;
        public int Column;
        public int Row;
        ulong m_TableVersion;

        public SByteCellValue(TableBase table, int row, int column)
        {
            Table = table;
            Row = row;
            Column = column;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public sbyte Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetSByte(Row, Column);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public sbyte GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(sbyte value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetSByte(Row, Column, value);
            }
        }

        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.SByte;
        }
    }
}