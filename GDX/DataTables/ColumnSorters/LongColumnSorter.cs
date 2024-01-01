// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.DataTables.ColumnSorters
{
    class LongColumnSorter : ColumnSorterBase, IComparer<RowDescription>
    {
        /// <inheritdoc />
        public LongColumnSorter(DataTableBase dataTable, int rowCount, int columnIdentifier, int sortDirection,
            bool supportMultiSort = false) :
            base(dataTable, rowCount, columnIdentifier, sortDirection, supportMultiSort)
        {
        }

        /// <inheritdoc />
        public int Compare(RowDescription x, RowDescription y)
        {
            long lhs = DataTable.GetLong(x.Identifier, ColumnIdentifier);
            long rhs = DataTable.GetLong(y.Identifier, ColumnIdentifier);

            if (lhs > rhs)
            {
                return ProcessCompare(-1);
            }

            return rhs > lhs ? ProcessCompare(1) : ProcessCompare(0, x.Identifier, y.Identifier);
        }
    }
}