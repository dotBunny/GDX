// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;
using UnityEngine;

namespace GDX.DataTables.DataBindings
{
    /// <summary>
    ///     Data translation functionality for <see cref="DataTableBase" />.
    /// </summary>
    public static class DataBindingProvider
    {
        public static FormatBase CustomFormat;

        /// <summary>
        ///     Supported interchange formats.
        /// </summary>
        public enum Format
        {
            Invalid = -1,
            CommaSeperatedValues,
            JavaScriptObjectNotation,
            Custom
        }

        /// <summary>
        ///     Export the content of a given <see cref="DataTableBase" /> to a target format.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /></param>
        /// <param name="format">The desired format to export as.</param>
        /// <param name="uri">The output path/uri where to send the data, absolute if on disk</param>
        public static void Export(DataTableBase dataTable, Format format, string uri)
        {
            SerializableTable serializableTable = new SerializableTable(dataTable);
            FormatBase binding = null;
            switch (format)
            {
                case Format.CommaSeperatedValues:
                    binding = new CommaSeperatedValueFormat();
                    break;
                case Format.JavaScriptObjectNotation:
                    binding = new JavaScriptObjectNotationFormat();
                    break;
                case Format.Custom:
                    if (CustomFormat == null)
                    {
                        Debug.LogWarning("Unable to push changes as a custom format is being used, without a provider.");
                    }
                    else
                    {
                        binding = CustomFormat;
                    }
                    break;
            }
            binding?.Push(uri, serializableTable);
        }

        public static Format GetFormatFromFile(string filePath)
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            const int k_HeaderSize = 25;

            byte[] chunk = new byte[k_HeaderSize];
            int read = fileStream.Read(chunk, 0, k_HeaderSize);
            return read == k_HeaderSize
                ? GetFormatFromFileHeader(Encoding.ASCII.GetString(chunk, 0, k_HeaderSize))
                : Format.Invalid;
        }

        static Format GetFormatFromFileHeader(string header)
        {
            if (JavaScriptObjectNotationFormat.IsHeader(header))
            {
                return Format.JavaScriptObjectNotation;
            }
            if (CommaSeperatedValueFormat.IsHeader(header))
            {
                return Format.CommaSeperatedValues;
            }

            if (CustomFormat != null)
            {

            }

            return Format.Invalid;
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
        /// <param name="uri">The resource path to load data from, absolute if on disk.</param>
        /// <param name="removeRowIfNotFound">Should rows that are not found in the file content be removed?</param>
        /// <returns>Was the import successful?</returns>
        public static bool Import(DataTableBase dataTable, Format format, string uri,
            bool removeRowIfNotFound = true)
        {
            SerializableTable serializableTable = null;
            FormatBase binding = null;

            if (format == Format.JavaScriptObjectNotation)
            {
                binding = new JavaScriptObjectNotationFormat();
            }
            else if (format == Format.CommaSeperatedValues)
            {
                binding = new CommaSeperatedValueFormat();
            }
            else if (format == Format.Custom && CustomFormat != null)
            {
                binding = CustomFormat;
            }

            if (binding != null)
            {
                serializableTable = binding.Pull(uri, dataTable.GetDataVersion(), dataTable.GetStructureVersion());
            }

            return serializableTable != null && serializableTable.Update(dataTable, removeRowIfNotFound);
        }

    }
}