// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Text;
using GDX.DataTables.DataBinding.Formats;

namespace GDX.DataTables.DataBinding
{
    /// <summary>
    ///     Data translation functionality for <see cref="DataTableBase" />.
    /// </summary>
    public static class DataBindingProvider
    {
        static readonly List<FormatBase> k_KnownFormats = new List<FormatBase>(3);
        static readonly CommaSeperatedValueFormat k_CommaSeperatedValueFormat = new CommaSeperatedValueFormat();
        static readonly JavaScriptObjectNotationFormat k_JavaScriptObjectNotationFormat =
            new JavaScriptObjectNotationFormat();

        /// <summary>
        ///     Export the content of a given <see cref="DataTableBase" /> to a target format.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTableBase" /></param>
        /// <param name="uri">The output path/uri where to send the data, absolute if on disk</param>
        /// <param name="jsonFallback">If the format cannot be determined by the uri, fallback to JSON.</param>
        public static void Export(DataTableBase dataTable, string uri, bool jsonFallback = true)
        {
            SerializableTable serializableTable = new SerializableTable(dataTable);
            FormatBase format = GetFormatFromUri(uri);
            if (format == null && jsonFallback)
            {
                format = k_JavaScriptObjectNotationFormat;
            }
            format?.Push(uri, serializableTable);
        }

        public static FormatBase[] GetFormats()
        {
            return k_KnownFormats.ToArray();
        }

        public static FormatBase GetFormatFromFile(string filePath)
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open);
            const int k_HeaderSize = 25;

            byte[] chunk = new byte[k_HeaderSize];
            int read = fileStream.Read(chunk, 0, k_HeaderSize);
            return read == k_HeaderSize
                ? GetFormatFromFileHeader(Encoding.ASCII.GetString(chunk, 0, k_HeaderSize))
                : null;
        }

        static FormatBase GetFormatFromFileHeader(string header)
        {
            for (int i = 0; i < k_KnownFormats.Count; i++)
            {
                if (k_KnownFormats[i].IsFileHeader(header))
                {
                    return k_KnownFormats[i];
                }
            }
            return null;
        }

        public static FormatBase GetFormatFromUri(string uri)
        {
            for (int i = 0; i < k_KnownFormats.Count; i++)
            {
                if (k_KnownFormats[i].IsUri(uri))
                {
                    return k_KnownFormats[i];
                }
            }
            return null;
        }

        public static string[] GetImportDialogExtensions()
        {
            List<string> returnData = new List<string>(4);
            returnData.Add("Auto");
            returnData.Add("*");
            for (int i = 0; i < k_KnownFormats.Count; i++)
            {
                string[] extensions = k_KnownFormats[i].GetImportDialogExtensions();
                if (extensions != null)
                {
                    returnData.AddRange(extensions);
                }
            }
            return returnData.ToArray();
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
        /// <param name="uri">The resource path to load data from, absolute if on disk.</param>
        /// <param name="removeRowIfNotFound">Should rows that are not found in the file content be removed?</param>
        /// <param name="jsonFallback">If the importer is unable to determine the format based on the URI, fallback to assuming its JSON.</param>
        /// <returns>Was the import successful?</returns>
        public static bool Import(DataTableBase dataTable, string uri, bool removeRowIfNotFound = true,
            bool jsonFallback = true)
        {
            SerializableTable serializableTable = null;
            FormatBase format = GetFormatFromUri(uri);
            if (format == null && jsonFallback)
            {
                format = k_JavaScriptObjectNotationFormat;
            }
            if (format != null)
            {
                serializableTable = format.Pull(uri, dataTable.GetDataVersion(), dataTable.GetStructureVersion());
            }
            return serializableTable != null && serializableTable.Update(dataTable, removeRowIfNotFound);
        }

        public static void RegisterFormat(FormatBase format)
        {
            if (!k_KnownFormats.Contains(format))
            {
                k_KnownFormats.Add(format);
            }
        }

        public static void UnregisterFormat(FormatBase format)
        {
            k_KnownFormats.Remove(format);
        }
    }
}