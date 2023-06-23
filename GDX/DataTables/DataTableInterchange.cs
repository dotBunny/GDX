// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using TextGenerator = GDX.Developer.TextGenerator;

namespace GDX.DataTables
{
    /// <summary>
    ///     Data translation functionality for <see cref="DataTableBase" />.
    /// </summary>
    public static class DataTableInterchange
    {
        /// <summary>
        ///     Supported interchange formats.
        /// </summary>
        public enum Format
        {
            Invalid = -1,
            CommaSeperatedValues,
            JavaScriptObjectNotation
        }

        /// <summary>
        ///     Export the content of a given <see cref="DataTableBase" /> to a target format.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /></param>
        /// <param name="format"></param>
        /// <param name="filePath"></param>
        public static void Export(DataTableBase dataTable, Format format, string filePath)
        {
            if (format == Format.JavaScriptObjectNotation)
            {
                DataTableJson json = new DataTableJson(dataTable);

                // TODO: do we want to set encoding?
                File.WriteAllText(filePath, json.ToString(), new UTF8Encoding());
            }
            else
            {
                ExportCommaSeperatedValues(dataTable, filePath);
            }
        }

        public static Format GetFormatFromContent(string fileContent)
        {
            if (fileContent.StartsWith("{\"Headers\":["))
            {
                return Format.JavaScriptObjectNotation;
            }
            if (fileContent.StartsWith("Row Identifier, Row Name,"))
            {
                return Format.CommaSeperatedValues;
            }

            return Format.Invalid;
        }
        public static Format GetFormatFromFile(string filePath)
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            const int k_HeaderSize = 25;

            byte[] chunk = new byte[k_HeaderSize];
            int read =  fileStream.Read(chunk, 0, k_HeaderSize);
            return read == k_HeaderSize ? GetFormatFromContent(Encoding.ASCII.GetString(chunk, 0, k_HeaderSize)) : Format.Invalid;
        }

        /// <summary>
        ///     Update the <see cref="DataTableBase" /> with the data found in the given file.
        /// </summary>
        /// <remarks>
        ///     It's important that the Row Identifier column remains unchanged, no structural changes have occured, and
        ///     no changes of column order were made. Object references will be maintained during update, only values will
        ///     be updated.
        /// </remarks>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> to apply changes to.</param>
        /// <param name="format">The file format.</param>
        /// <param name="filePath">The path to the file to read.</param>
        /// <param name="removeRowIfNotFound">Should rows that are not found in the file content be removed?</param>
        /// <returns>Was the import successful?</returns>
        public static bool Import(DataTableBase dataTable, Format format, string filePath,
            bool removeRowIfNotFound = true)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Unable to find {filePath}.");
                return false;
            }

            bool importStatus = false;

            if (format == Format.JavaScriptObjectNotation)
            {
                DataTableJson jsonObject = DataTableJson.Create(File.ReadAllText(filePath));
                if (jsonObject != null)
                {
                    importStatus = jsonObject.Update(dataTable, removeRowIfNotFound);
                }

            }
            else if (format == Format.CommaSeperatedValues)
            {
                importStatus = ImportCommaSeperatedValues(dataTable, File.ReadAllLines(filePath), removeRowIfNotFound);
            }

            return importStatus;
        }

        /// <summary>
        ///     Update the <see cref="DataTableBase" /> with the CSV data found in the given file.
        /// </summary>
        /// <remarks>
        ///     It's important that the Row Identifier column remains unchanged, no structural changes have occured, and
        ///     no changes of column order were made. Object references will be maintained during update, only values will
        ///     be updated.
        /// </remarks>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> to apply changes to.</param>
        /// <param name="fileContent">An array of CSV lines.</param>
        /// <param name="removeRowIfNotFound">Should rows that are not found in the CSV content be removed?</param>
        /// <returns>Was this operation successful?</returns>
        static bool ImportCommaSeperatedValues(DataTableBase dataTable, string[] fileContent,
            bool removeRowIfNotFound = true)
        {
            int tableRowCount = dataTable.GetRowCount();
            int tableColumnCount = dataTable.GetColumnCount();
            ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();

            // Test Columns
            string[] columnTest = ParseCommaSeperatedValues(fileContent[0]);
            if (columnTest.Length != tableColumnCount + 2)
            {
                Debug.LogError(
                    $"The importing data has {columnTest.Length} columns where {tableColumnCount + 2} was expected.");
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

            List<int> foundRowInternalIndices = new List<int>(tableRowCount);

            for (int i = 1; i < fileContent.Length; i++)
            {
                int rowIdentifier;
                string[] rowStrings = ParseCommaSeperatedValues(fileContent[i]);
                string rowName;
                if (string.IsNullOrEmpty(rowStrings[0]))
                {
                    rowName = rowStrings[1];
                    if (string.IsNullOrEmpty(rowName))
                    {
                        rowName = "Unnamed";
                    }

                    // Create new row
                    rowIdentifier = dataTable.AddRow(rowName);
                }
                else
                {
                    rowIdentifier = int.Parse(rowStrings[0]);
                    rowName = rowStrings[1];
                }

                foundRowInternalIndices.Add(rowIdentifier);

                dataTable.SetRowName(rowIdentifier, rowName);

                for (int j = 0; j < tableColumnCount; j++)
                {
                    dataTable.SetCellValueFromString(rowIdentifier, columnDescriptions[j].Identifier, rowStrings[j + 2],
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

        /// <summary>
        ///     Export the data found in the <see cref="DataTableBase" /> to a CSV file.
        /// </summary>
        /// <remarks>
        ///     The data is able to be imported via <see cref="Import" />,
        ///     however there is a requirement that the structure of the table does not change, nor does the column order. The Row
        ///     Identifier will be used to match up rows, with an option to create new rows.
        /// </remarks>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> to export data from.</param>
        /// <param name="filePath">The absolute path where to write the data in CSV format.</param>
        static void ExportCommaSeperatedValues(DataTableBase dataTable, string filePath)
        {
            int rowCount = dataTable.GetRowCount();
            int columnCount = dataTable.GetColumnCount();
            TextGenerator generator = new TextGenerator();

            // Build first line
            ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();
            generator.Append("Row Identifier, Row Name");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(columnDescriptions[i].Name);
            }

            generator.NextLine();

            // Build lines for rows
            for (int r = 0; r < rowCount; r++)
            {
                RowDescription rowDescription = dataTable.GetRowDescriptionByOrder(r);
                generator.Append($"{rowDescription.Identifier}, {MakeCommaSeperatedValue(rowDescription.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    ColumnDescription columnDescription = dataTable.GetColumnDescriptionByOrder(c);
                    generator.Append(", ");
                    if (columnDescription.Type == Serializable.SerializableTypes.String ||
                        columnDescription.Type == Serializable.SerializableTypes.Char)
                    {
                        generator.Append(MakeCommaSeperatedValue(dataTable.GetCellValueAsString(
                            rowDescription.Identifier, columnDescription.Identifier, columnDescription.Type)));
                    }
                    else
                    {
                        generator.Append(dataTable.GetCellValueAsString(rowDescription.Identifier,
                            columnDescription.Identifier, columnDescription.Type));
                    }
                }

                generator.NextLine();
            }

            File.WriteAllText(filePath, generator.ToString());
        }

        /// <summary>
        ///     Make a CSV safe version of the provided content.
        /// </summary>
        /// <param name="content">The content which needs to be made safe for CSV.</param>
        /// <returns>A CSV safe value string.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string MakeCommaSeperatedValue(string content)
        {
            if (content == null)
            {
                return string.Empty;
            }

            // Double quote quotes
            if (content.IndexOf('"') != -1)
            {
                content = content.Replace("\"", "\"\"");
            }

            // Ensure quotes for commas
            return content.IndexOf(',') != -1 ? $"\"{content}\"" : content;
        }

        /// <summary>
        ///     Parse a given <paramref name="line" /> into seperated values.
        /// </summary>
        /// <param name="line">The CSV line.</param>
        /// <returns>An array of string values.</returns>
        static string[] ParseCommaSeperatedValues(string line)
        {
            List<string> returnStrings = new List<string>();
            int lastIndex = -1;
            int currentIndex = 0;
            bool isQuoted = false;
            int length = line.Length;
            while (currentIndex < length)
            {
                switch (line[currentIndex])
                {
                    case '"':
                        isQuoted = !isQuoted;
                        break;
                    case ',':
                        if (!isQuoted)
                        {
                            returnStrings.Add(line.Substring(lastIndex + 1, currentIndex - lastIndex).Trim(' ', ','));
                            lastIndex = currentIndex;
                        }

                        break;
                }

                currentIndex++;
            }

            if (lastIndex != line.Length - 1)
            {
                returnStrings.Add(line.Substring(lastIndex + 1).Trim());
            }

            return returnStrings.ToArray();
        }
    }
}