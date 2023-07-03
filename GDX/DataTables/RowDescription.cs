// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.DataTables
{
    /// <summary>
    ///     A description of a row in a <see cref="DataTableBase" />.
    /// </summary>
    public struct RowDescription
    {
        /// <summary>
        ///     The given name of the row.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The unique identifier for the row.
        /// </summary>
        public int Identifier;

        /// <summary>
        ///     The current sort order.
        /// </summary>
        public int SortOrder;

        /// <summary>
        ///     Generates a custom string based on the row.
        /// </summary>
        /// <returns>A user-friendly string representation of the <see cref="RowDescription" />.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}