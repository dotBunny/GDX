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
            DataTableTransfer dataTableTransfer = new DataTableTransfer(dataTable);

            if (format == Format.JavaScriptObjectNotation)
            {
                File.WriteAllText(filePath, JsonUtility.ToJson(dataTableTransfer), new UTF8Encoding());
            }
            else if (format == Format.CommaSeperatedValues)
            {
                File.WriteAllText(filePath, ToCommaSeperatedValues(dataTableTransfer), new UTF8Encoding());
            }
        }

        public static Format GetFormatFromContent(string fileContent)
        {
            if (fileContent.StartsWith("{\"DataVersion\":"))
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
            int read = fileStream.Read(chunk, 0, k_HeaderSize);
            return read == k_HeaderSize
                ? GetFormatFromContent(Encoding.ASCII.GetString(chunk, 0, k_HeaderSize))
                : Format.Invalid;
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

            DataTableTransfer dataTableTransfer = null;
            if (format == Format.JavaScriptObjectNotation)
            {
                dataTableTransfer = JsonUtility.FromJson<DataTableTransfer>(File.ReadAllText(filePath));
            }
            else if (format == Format.CommaSeperatedValues)
            {
                dataTableTransfer = FromCommaSeperatedValues(File.ReadAllLines(filePath));
            }

            return dataTableTransfer != null && dataTableTransfer.Update(dataTable, removeRowIfNotFound);
        }

        /// <summary>
        ///     Creates a <see cref="DataTableTransfer" /> from a CSV files contents.
        /// </summary>
        /// <param name="fileContent">An array of lines from a csv file.</param>
        /// <returns>An object if it successfully parses, or null if it fails.</returns>
        static DataTableTransfer FromCommaSeperatedValues(string[] fileContent)
        {
            try
            {
                DataTableTransfer returnData = new DataTableTransfer();

                int rowCount = fileContent.Length - 2;
                if (rowCount <= 0)
                {
                    return null;
                }

                // Build headers
                string[] headers = ParseCommaSeperatedValues(fileContent[0]);
                int actualHeaderCount = headers.Length - 2;
                returnData.Headers = new string[actualHeaderCount];
                Array.Copy(headers, 2, returnData.Headers, 0, actualHeaderCount);

                // Build types plus additional packed versions
                string[] types = ParseCommaSeperatedValues(fileContent[1]);
                int actualTypesCount = types.Length - 2;
                returnData.Types = new string[actualTypesCount];
                returnData.DataVersion = ulong.Parse(types[0]);
                returnData.StructureVersion = int.Parse(types[1]);
                Array.Copy(types, 2, returnData.Types, 0, actualTypesCount);

                // Extract rows
                returnData.Rows = new DataTableTransfer.DataTableTransferRow[rowCount];
                int rowIndex = 0;
                for (int i = 2; i < fileContent.Length; i++)
                {
                    string[] rowData = ParseCommaSeperatedValues(fileContent[i]);

                    DataTableTransfer.DataTableTransferRow transferRow =
                        new DataTableTransfer.DataTableTransferRow(actualTypesCount)
                        {
                            Identifier = int.Parse(rowData[0]),
                            Name = rowData[1],
                            Data = new string[actualTypesCount]
                        };
                    Array.Copy(rowData, 2, transferRow.Data, 0, actualTypesCount);

                    returnData.Rows[rowIndex] = transferRow;
                    rowIndex++;
                }

                // Return our built object from CSV
                return returnData;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to parse provided CVS\n{e.Message}");
                return null;
            }
        }

        /// <summary>
        ///     Creates the content for a Comma Seperated Values file from a <see cref="DataTableTransfer" />.
        /// </summary>
        /// <param name="dataTableTransfer">The target to create the file from.</param>
        /// <returns>The content of the file.</returns>
        static string ToCommaSeperatedValues(DataTableTransfer dataTableTransfer)
        {
            int rowCount = dataTableTransfer.Rows.Length;
            int columnCount = dataTableTransfer.Types.Length;

            TextGenerator generator = new TextGenerator();

            // Build first line
            generator.Append("Row Identifier, Row Name");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(dataTableTransfer.Headers[i]);
            }

            generator.NextLine();

            // Build info line
            generator.Append(
                $"{dataTableTransfer.DataVersion.ToString()}, {dataTableTransfer.StructureVersion.ToString()}");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(dataTableTransfer.Types[i]);
            }

            generator.NextLine();

            // Build lines for rows
            for (int r = 0; r < rowCount; r++)
            {
                DataTableTransfer.DataTableTransferRow transferRow = dataTableTransfer.Rows[r];

                generator.Append($"{transferRow.Identifier.ToString()}, {MakeCommaSeperatedValue(transferRow.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    generator.Append(", ");

                    if (dataTableTransfer.Types[c] == Serializable.SerializableTypes.String.GetLabel() ||
                        dataTableTransfer.Types[c] == Serializable.SerializableTypes.Char.GetLabel())
                    {
                        generator.Append(MakeCommaSeperatedValue(transferRow.Data[c]));
                    }
                    else
                    {
                        generator.Append(transferRow.Data[c]);
                    }
                }

                generator.NextLine();
            }

            return generator.ToString();
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