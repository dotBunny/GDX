// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.Tables.CellValues
{
    public struct QuaternionCellValue
    {
        Quaternion m_CachedValue;
        public readonly ITable Table;
        public readonly int Column;
        public readonly int Row;
        ulong m_TableVersion;

        public QuaternionCellValue(ITable table, int row, int column)
        {
            Table = table;
            Row = row;
            Column = column;
            m_TableVersion = 0;
            m_CachedValue = default;
            Get();
        }

        public Quaternion Get()
        {
            if (m_TableVersion != Table.GetDataVersion())
            {
                m_CachedValue = Table.GetQuaternion(Row, Column);
                m_TableVersion = Table.GetDataVersion();
            }
            return m_CachedValue;
        }

        public Quaternion GetUnsafe()
        {
            return m_CachedValue;
        }

        public void Set(Vector3 eulerAngles, bool updateTable = true)
        {
            Set(Quaternion.Euler(eulerAngles), updateTable);
        }

        public void Set(Vector4 value, bool updateTable = true)
        {
            Set(new Quaternion(value.x, value.y, value.z, value.w), updateTable);
        }

        public void Set(Quaternion value, bool updateTable = true)
        {
            m_CachedValue = value;
            if (updateTable)
            {
                m_TableVersion =  Table.SetQuaternion(Row, Column, value);
            }
        }
    }
}