// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    [System.Serializable]
    public struct Vector4CellValue
    {
        Vector4 m_CachedValue;
        public TableBase Table;
        public int Column;
        public int Row;
        ulong m_TableVersion;

        public Vector4CellValue(TableBase table, int row, int column)
        {
            Table = table;
            Row = row;
            Column = column;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public Vector4 Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetVector4(Row, Column);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public Vector4 GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(Vector4 value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion = Table.SetVector4(Row, Column, value);
            }
        }

        public static Serializable.SerializableTypes GetSupportedType()
        {
            return Serializable.SerializableTypes.BoundsInt;
        }
    }
}