// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.DataTables.DataBinding
{
    /// <summary>
    ///     A serializable object used during import and export of <see cref="DataTableBase" /> based data.
    /// </summary>
    [Serializable]
    public class SerializableTable
    {
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     The version of the data at the point of export.
        /// </summary>
        public ulong DataVersion;

        /// <summary>
        ///     The structural version of the data at the point of export.
        /// </summary>
        public int StructureVersion;

        /// <summary>
        ///     The column names.
        /// </summary>
        public string[] Headers;

        /// <summary>
        ///     The column types.
        /// </summary>
        public string[] Types;

        /// <summary>
        ///     The row data.
        /// </summary>
        public SerializableRow[] Rows;

        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

        /// <summary>
        ///     Creates a new empty <see cref="SerializableTable" />.
        /// </summary>
        public SerializableTable()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="SerializableTable" /> from a <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="dataTable">Populate the data set from this target.</param>
        public SerializableTable(DataTableBase dataTable)
        {
            ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();
            int columnCount = columnDescriptions.Length;

            DataVersion = dataTable.GetDataVersion();
            StructureVersion = dataTable.GetStructureVersion();

            // Build Headers
            Headers = new string[columnCount];
            Types = new string[columnCount];

            for (int i = 0; i < columnCount; i++)
            {
                Headers[i] = columnDescriptions[i].Name;
                Types[i] = columnDescriptions[i].Type.GetLabel();
            }

            RowDescription[] rowDescriptions = dataTable.GetAllRowDescriptions();
            int rowCount = rowDescriptions.Length;

            Rows = new SerializableRow[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                RowDescription rowDescription = rowDescriptions[i];

                Rows[i] = new SerializableRow(columnCount)
                {
                    Identifier = rowDescription.Identifier, Name = rowDescription.Name
                };

                for (int c = 0; c < columnCount; c++)
                {
                    ColumnDescription columnDescription = columnDescriptions[c];
                    Rows[i].Data[c] = dataTable.GetCellValueAsString(rowDescription.Identifier,
                        columnDescription.Identifier,
                        columnDescription.Type);
                }
            }
        }

        /// <summary>
        ///     Update the target <paramref name="dataTable" /> based on the data found in the <see cref="SerializableTable" />
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /> to update.</param>
        /// <param name="removeRowIfNotFound">
        ///     Should rows not found in the <see cref="SerializableTable" /> be removed from the
        ///     <see cref="DataTableBase" />.
        /// </param>
        /// <returns>Was this operation successful?</returns>
        public bool Update(DataTableBase dataTable, bool removeRowIfNotFound = true)
        {
            int tableRowCount = dataTable.GetRowCount();
            int tableColumnCount = dataTable.GetColumnCount();

            ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();
            int neededColumnCount = columnDescriptions.Length;
            if (neededColumnCount != Types.Length || neededColumnCount != Headers.Length)
            {
                Debug.LogError(
                    $"The importing data has {Types.Length.ToString()} columns where {neededColumnCount.ToString()} was expected.");
                return false;
            }

            // Build a list of previous row ID, so we know what was removed
            List<int> previousRowInternalIndices = new List<int>(tableRowCount);
            RowDescription[] rowDescriptions = dataTable.GetAllRowDescriptions();
            int rowDescriptionsLength = rowDescriptions.Length;
            for (int i = 0; i < rowDescriptionsLength; i++)
            {
                previousRowInternalIndices.Add(rowDescriptions[i].Identifier);
            }

            int fileRowCount = Rows.Length;
            List<int> foundRowInternalIndices = new List<int>(tableRowCount);

            for (int i = 0; i < fileRowCount; i++)
            {
                int rowIdentifier;
                string rowName;
                if (Rows[i].Identifier == -1)
                {
                    rowName = Rows[i].Name;
                    if (string.IsNullOrEmpty(rowName))
                    {
                        rowName = "Unnamed";
                    }

                    rowIdentifier = dataTable.AddRow(rowName);
                }
                else
                {
                    rowIdentifier = Rows[i].Identifier;
                    rowName = Rows[i].Name;
                }

                foundRowInternalIndices.Add(rowIdentifier);
                dataTable.SetRowName(rowIdentifier, rowName);

                for (int j = 0; j < tableColumnCount; j++)
                {
                    dataTable.SetCellValueFromString(rowIdentifier, columnDescriptions[j].Identifier, Rows[i].Data[j],
                        columnDescriptions[j].Type);
                }
            }

            // Remove indices that were not found any more?
            if (removeRowIfNotFound)
            {
                int foundIndicesCount = foundRowInternalIndices.Count;
                for (int i = 0; i < foundIndicesCount; i++)
                {
                    if (previousRowInternalIndices.Contains(foundRowInternalIndices[i]))
                    {
                        previousRowInternalIndices.Remove(foundRowInternalIndices[i]);
                    }
                }

                int indicesToRemove = previousRowInternalIndices.Count;
                for (int i = 0; i < indicesToRemove; i++)
                {
                    dataTable.RemoveRow(previousRowInternalIndices[i]);
                }
            }

            return true;
        }
    }
}