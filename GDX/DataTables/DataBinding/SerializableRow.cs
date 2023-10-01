// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.DataTables.DataBinding
{
    /// <summary>
    ///     A serializable representation of a row of data from a <see cref="DataTableBase" />.
    /// </summary>
    [Serializable]
    public class SerializableRow
    {
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     The unique row identifier.
        /// </summary>
        public int Identifier = -1;

        /// <summary>
        ///     The row's user-friendly name.
        /// </summary>
        public string Name;

        /// <summary>
        ///     The row's data.
        /// </summary>
        public string[] Data;

        /// <summary>
        ///     Create a new serializable row.
        /// </summary>
        /// <param name="columns">The number of columns in that row.</param>
        public SerializableRow(int columns)
        {
            Data = new string[columns];
        }

        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006
    }
}