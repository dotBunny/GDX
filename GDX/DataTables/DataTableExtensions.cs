// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using TextGenerator = GDX.Developer.TextGenerator;

namespace GDX.DataTables
{
    /// <summary>
    ///     <see cref="DataTableBase" /> Based Extension Methods
    /// </summary
    public static class DataTableExtensions
    {
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

        /// <summary>
        ///     Update the <see cref="DataTableBase" /> with the CSV data found in the given file.
        /// </summary>
        /// <remarks>
        ///     It's important that the Row Identifier column remains unchanged, no structural changes have occured, and
        ///     no changes of column order were made. Object references will be maintained during update, only values will
        ///     be updated.
        /// </remarks>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> to apply changes to.</param>
        /// <param name="filePath">The path to the CSV file to read.</param>
        /// <param name="removeRowIfNotFound">Should rows that are not found in the CSV content be removed?</param>
        /// <returns>Was this operation successful?</returns>
        public static bool UpdateFromCommaSeperatedValues(this DataTableBase dataTable, string filePath,
            bool removeRowIfNotFound = true)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Unable to find {filePath}.");
                return false;
            }

            return UpdateFromCommaSeperatedValues(dataTable, File.ReadAllLines(filePath), removeRowIfNotFound);
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
        public static bool UpdateFromCommaSeperatedValues(this DataTableBase dataTable, string[] fileContent,
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
                int rowIdentifier = -1;
                string[] rowStrings = ParseCommaSeperatedValues(fileContent[i]);
                string rowName = string.Empty;
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
                    string columnString = rowStrings[j + 2];
                    switch (columnDescriptions[j].Type)
                    {
                        case Serializable.SerializableTypes.String:
                            dataTable.SetString(rowIdentifier, columnDescriptions[j].Identifier, columnString);
                            break;
                        case Serializable.SerializableTypes.Char:
                            dataTable.SetChar(rowIdentifier, columnDescriptions[j].Identifier, columnString[0]);
                            break;
                        case Serializable.SerializableTypes.Bool:
                            dataTable.SetBool(rowIdentifier, columnDescriptions[j].Identifier,
                                bool.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.SByte:
                            dataTable.SetSByte(rowIdentifier, columnDescriptions[j].Identifier,
                                sbyte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Byte:
                            dataTable.SetByte(rowIdentifier, columnDescriptions[j].Identifier,
                                byte.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Short:
                            dataTable.SetShort(rowIdentifier, columnDescriptions[j].Identifier,
                                short.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UShort:
                            dataTable.SetUShort(rowIdentifier, columnDescriptions[j].Identifier,
                                ushort.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Int:
                            dataTable.SetInt(rowIdentifier, columnDescriptions[j].Identifier, int.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.UInt:
                            dataTable.SetUInt(rowIdentifier, columnDescriptions[j].Identifier,
                                uint.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Long:
                            dataTable.SetLong(rowIdentifier, columnDescriptions[j].Identifier,
                                long.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.ULong:
                            dataTable.SetULong(rowIdentifier, columnDescriptions[j].Identifier,
                                ulong.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Float:
                            dataTable.SetFloat(rowIdentifier, columnDescriptions[j].Identifier,
                                float.Parse(columnString));
                            break;
                        case Serializable.SerializableTypes.Double:
                            dataTable.SetDouble(rowIdentifier, columnDescriptions[j].Identifier,
                                double.Parse(columnString));
                            break;
                    }
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
        ///     The data is able to be imported via
        ///     <see cref="UpdateFromCommaSeperatedValues(GDX.DataTables.DataTableBase,string,bool)" />,
        ///     however there is a requirement that the structure of the table does not change, nor does the column order. The Row
        ///     Identifier will be used to match up rows, with an option to create new rows.
        /// </remarks>
        /// <param name="dataTable">The target <see cref="DataTableBase" /> to export data from.</param>
        /// <param name="filePath">The absolute path where to write the data in CSV format.</param>
        public static void ExportToCommaSeperatedValues(this DataTableBase dataTable, string filePath)
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
                RowDescription rowDescription = dataTable.GetRowDescription(r);
                generator.Append($"{rowDescription.Identifier}, {MakeCommaSeperatedValue(rowDescription.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    ColumnDescription columnDescription = dataTable.GetColumnDescription(c);
                    generator.Append(", ");
                    switch (columnDescription.Type)
                    {
                        case Serializable.SerializableTypes.String:
                            generator.Append(MakeCommaSeperatedValue(dataTable.GetString(rowDescription.Identifier,
                                columnDescription.Identifier)));
                            break;
                        case Serializable.SerializableTypes.Char:
                            generator.Append(MakeCommaSeperatedValue(dataTable
                                .GetChar(rowDescription.Identifier, columnDescription.Identifier).ToString()));
                            break;
                        case Serializable.SerializableTypes.Bool:
                            generator.Append(dataTable
                                .GetBool(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.SByte:
                            generator.Append(dataTable
                                .GetSByte(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Byte:
                            generator.Append(dataTable
                                .GetByte(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Short:
                            generator.Append(dataTable
                                .GetShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.UShort:
                            generator.Append(dataTable
                                .GetUShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Int:
                            generator.Append(dataTable
                                .GetInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.UInt:
                            generator.Append(dataTable
                                .GetUInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Long:
                            generator.Append(dataTable
                                .GetLong(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.ULong:
                            generator.Append(dataTable
                                .GetULong(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Float:
                            generator.Append(dataTable
                                .GetFloat(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Double:
                            generator.Append(dataTable
                                .GetDouble(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString(CultureInfo.InvariantCulture));
                            break;
                        case Serializable.SerializableTypes.Vector2:
                            generator.Append(dataTable
                                .GetUShort(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3:
                            generator.Append(dataTable
                                .GetVector3(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector4:
                            generator.Append(dataTable
                                .GetVector4(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Vector2Int:
                            generator.Append(dataTable
                                .GetVector2Int(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Vector3Int:
                            generator.Append(dataTable
                                .GetVector3Int(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Quaternion:
                            generator.Append(dataTable
                                .GetQuaternion(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Rect:
                            generator.Append(dataTable
                                .GetRect(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.RectInt:
                            generator.Append(dataTable
                                .GetRectInt(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Color:
                            generator.Append(dataTable
                                .GetColor(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.LayerMask:
                            generator.Append(dataTable
                                .GetLayerMask(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Bounds:
                            generator.Append(dataTable
                                .GetBounds(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.BoundsInt:
                            generator.Append(dataTable
                                .GetBoundsInt(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Hash128:
                            generator.Append(dataTable
                                .GetHash128(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.Gradient:
                            generator.Append(dataTable
                                .GetGradient(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                        case Serializable.SerializableTypes.AnimationCurve:
                            generator.Append(dataTable
                                .GetAnimationCurve(rowDescription.Identifier, columnDescription.Identifier)
                                .ToString());
                            break;
                        case Serializable.SerializableTypes.Object:
                            generator.Append(dataTable
                                .GetObject(rowDescription.Identifier, columnDescription.Identifier).ToString());
                            break;
                    }
                }

                generator.NextLine();
            }

            File.WriteAllText(filePath, generator.ToString());
        }
    }
}