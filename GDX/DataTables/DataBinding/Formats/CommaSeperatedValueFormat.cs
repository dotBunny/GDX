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

namespace GDX.DataTables.DataBindings
{
    /// <summary>
    /// A comma-seperated values format.
    /// </summary>
    class CommaSeperatedValueFormat : FormatBase
    {
        public CommaSeperatedValueFormat()
        {
            // We can register the format this way because we automatically include these formats in the
            // DataBindingsProvider as private members.
            DataBindingProvider.RegisterFormat(this);
        }

        ~CommaSeperatedValueFormat()
        {
            DataBindingProvider.UnregisterFormat(this);
        }

        /// <inheritdoc />
        public override DateTime GetBindingTimestamp(string uri)
        {
            return File.GetLastWriteTimeUtc(Path.Combine(Application.dataPath, uri));
        }

        /// <inheritdoc />
        public override string GetFilePreferredExtension()
        {
            return "csv";
        }

        /// <inheritdoc />
        public override string GetFriendlyName()
        {
            return "CSV";
        }

        /// <inheritdoc />
        public override bool IsFileHeader(string header)
        {
            return header.StartsWith("Row Identifier, Row Name,", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string[] GetImportDialogExtensions()
        {
            return new [] { GetFriendlyName(), GetFilePreferredExtension() };
        }

        /// <inheritdoc />
        public override bool IsOnDiskFormat()
        {
            return true;
        }

        /// <inheritdoc />
        public override bool IsUri(string uri)
        {
            return uri.EndsWith(GetFilePreferredExtension(), StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override SerializableTable Pull(string uri, ulong currentDataVersion, int currentStructuralVersion)
        {
            if (uri == null || !File.Exists(uri))
            {
                return null;
            }

            return Parse(File.ReadAllLines(uri));
        }

        /// <inheritdoc />
        public override bool Push(string uri, SerializableTable serializableTable)
        {
            if (uri == null)
            {
                return false;
            }

            File.WriteAllText(uri, Generate(serializableTable), new UTF8Encoding());
            return File.Exists(uri);
        }

        /// <summary>
        ///     Creates the content for a Comma Seperated Values file from a <see cref="SerializableTable" />.
        /// </summary>
        /// <param name="serializableTable">The target to create the file from.</param>
        /// <returns>The content of the file.</returns>
        static string Generate(SerializableTable serializableTable)
        {
            int rowCount = serializableTable.Rows.Length;
            int columnCount = serializableTable.Types.Length;

            TextGenerator generator = new TextGenerator();

            // Build first line
            generator.Append("Row Identifier, Row Name");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(serializableTable.Headers[i]);
            }

            generator.NextLine();

            // Build info line
            generator.Append(
                $"{serializableTable.DataVersion.ToString()}, {serializableTable.StructureVersion.ToString()}");
            for (int i = 0; i < columnCount; i++)
            {
                generator.Append(", ");
                generator.Append(serializableTable.Types[i]);
            }

            generator.NextLine();

            // Build lines for rows
            for (int r = 0; r < rowCount; r++)
            {
                SerializableRow transferRow = serializableTable.Rows[r];

                generator.Append($"{transferRow.Identifier.ToString()}, {MakeCommaSeperatedValue(transferRow.Name)}");
                for (int c = 0; c < columnCount; c++)
                {
                    generator.Append(", ");

                    if (serializableTable.Types[c] == Serializable.SerializableTypes.String.GetLabel() ||
                        serializableTable.Types[c] == Serializable.SerializableTypes.Char.GetLabel())
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
        ///     Creates a <see cref="SerializableTable" /> from a CSV files contents.
        /// </summary>
        /// <param name="fileContent">An array of lines from a csv file.</param>
        /// <returns>An object if it successfully parses, or null if it fails.</returns>
        static SerializableTable Parse(string[] fileContent)
        {
            try
            {
                SerializableTable returnSerializableTable = new SerializableTable();

                int rowCount = fileContent.Length - 2;
                if (rowCount <= 0)
                {
                    return null;
                }

                // Build headers
                string[] headers = ParseCommaSeperatedValues(fileContent[0]);
                int actualHeaderCount = headers.Length - 2;
                returnSerializableTable.Headers = new string[actualHeaderCount];
                Array.Copy(headers, 2, returnSerializableTable.Headers, 0, actualHeaderCount);

                // Build types plus additional packed versions
                string[] types = ParseCommaSeperatedValues(fileContent[1]);
                int actualTypesCount = types.Length - 2;
                returnSerializableTable.Types = new string[actualTypesCount];
                returnSerializableTable.DataVersion = ulong.Parse(types[0]);
                returnSerializableTable.StructureVersion = int.Parse(types[1]);
                Array.Copy(types, 2, returnSerializableTable.Types, 0, actualTypesCount);

                // Extract rows
                returnSerializableTable.Rows = new SerializableRow[rowCount];
                int rowIndex = 0;
                for (int i = 2; i < fileContent.Length; i++)
                {
                    string[] rowData = ParseCommaSeperatedValues(fileContent[i]);

                    SerializableRow transferRow = new SerializableRow(actualTypesCount)
                    {
                        Identifier = int.Parse(rowData[0]), Name = rowData[1], Data = new string[actualTypesCount]
                    };
                    Array.Copy(rowData, 2, transferRow.Data, 0, actualTypesCount);

                    returnSerializableTable.Rows[rowIndex] = transferRow;
                    rowIndex++;
                }

                // Return our built object from CSV
                return returnSerializableTable;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Unable to parse provided CVS\n{e.Message}");
                return null;
            }
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