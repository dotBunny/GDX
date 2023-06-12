// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

namespace GDX.DataTables
{
    /// <summary>
    ///     A description of a column in a <see cref="DataTableBase"/>.
    /// </summary>
    public struct ColumnDescription
    {
        /// <summary>
        ///     The given name of the column.
        /// </summary>
        public string Name;
        /// <summary>
        ///     The unique identifier for the column.
        /// </summary>
        public int Identifier;
        /// <summary>
        ///     The type of data stored in the column.
        /// </summary>
        public Serializable.SerializableTypes Type;

        /// <summary>
        ///     The current sort order.
        /// </summary>
        public int SortOrder;

        /// <summary>
        ///     Generates a custom string based on the column.
        /// </summary>
        /// <returns>A user-friendly string representation of the <see cref="ColumnDescription"/>.</returns>
        public override string ToString()
        {
            return $"{Name} [{Serializable.GetLabelFromTypeValue((int)Type)}]";
        }
    }
}