// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.DataTables.ColumnSorters
{
    class StringColumnSorter : ColumnSorterBase, IComparer<RowDescription>
    {
        /// <inheritdoc />
        public StringColumnSorter(DataTableBase dataTable, int rowCount, int columnIdentifier, int sortDirection,
            bool supportMultiSort = false) :
            base(dataTable, rowCount, columnIdentifier, sortDirection, supportMultiSort)
        {
        }

        /// <inheritdoc />
        public int Compare(RowDescription x, RowDescription y)
        {
            string lhs = DataTable.GetString(x.Identifier, ColumnIdentifier);
            string rhs = DataTable.GetString(y.Identifier, ColumnIdentifier);
            return ProcessCompare(string.CompareOrdinal(lhs, rhs), x.Identifier, y.Identifier);
        }
    }
}