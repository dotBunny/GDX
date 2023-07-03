// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.DataTables.ColumnSorters
{
    abstract class ColumnSorterBase
    {
        protected readonly int ColumnIdentifier;
        protected readonly DataTableBase DataTable;
        public readonly List<int> EqualIdentifiers;
        readonly int m_SortDirection;
        readonly bool m_SupportMultiSort;

        protected ColumnSorterBase(DataTableBase dataTable, int rowCount, int columnIdentifier, int sortDirection,
            bool supportMultiSort = false)
        {
            DataTable = dataTable;
            ColumnIdentifier = columnIdentifier;
            m_SortDirection = sortDirection;
            m_SupportMultiSort = supportMultiSort;
            EqualIdentifiers = m_SupportMultiSort ? new List<int>(rowCount) : null;
        }

        protected int ProcessCompare(int compare, int lhsIdentifier = -1, int rhsIdentifier = -1)
        {
            if (compare == 0)
            {
                // Same
                if (!m_SupportMultiSort)
                {
                    return 0;
                }

                EqualIdentifiers.Add(lhsIdentifier);
                EqualIdentifiers.Add(rhsIdentifier);

                return 0;
            }

            if (m_SortDirection == 0)
            {
                return compare * -1;
            }

            return compare;
        }
    }
}