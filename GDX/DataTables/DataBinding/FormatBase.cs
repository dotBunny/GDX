// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.DataTables.DataBinding
{
    /// <summary>
    ///     A custom binding to be used with a <see cref="DataTableBase" />.
    /// </summary>
    public abstract class FormatBase
    {
        /// <summary>
        ///     Get the latest timestamp for the binding format.
        /// </summary>
        /// <param name="uri">The binding Uri.</param>
        /// <returns>A UTC timestamp.</returns>
        public abstract DateTime GetBindingTimestamp(string uri);

        /// <summary>
        ///     Get the preferred file extension for this format
        /// </summary>
        /// <remarks>If an on-disk format.</remarks>
        /// <returns>An extension, with period.</returns>
        public abstract string GetFilePreferredExtension();

        /// <summary>
        ///     Get the user-friendly name of the format.
        /// </summary>
        /// <returns>Returns the name of the format.</returns>
        public abstract string GetFriendlyName();

        /// <summary>
        ///     Get the information needed for a system level dialog to populate with this format.
        /// </summary>
        public abstract string[] GetImportDialogExtensions();

        /// <summary>
        ///     Does the file header match what we expect it to be for this format?
        /// </summary>
        /// <remarks>
        ///     This should be overloaded as always false for remote formats (think Google Sheets).
        /// </remarks>
        /// <param name="headerContent">A small read section of the file used to evaluate the header content.</param>
        /// <returns>true/false if it is the expected header.</returns>
        public abstract bool IsFileHeader(string headerContent);

        /// <summary>
        ///     Is this format expected to be on a local disk?
        /// </summary>
        /// <returns>true/false</returns>
        public abstract bool IsOnDiskFormat();

        /// <summary>
        ///     Does the binding Uri match what we would expect for this platform?
        /// </summary>
        /// <param name="uri">The binding Uri.</param>
        /// <returns>true/false if it matches as expected.</returns>
        public abstract bool IsUri(string uri);

        /// <summary>
        ///     Get an updated data set to apply to the <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="uri">The URI to access data from, an absolute path if on local disk.</param>
        /// <param name="currentDataVersion">The data version known to Unity.</param>
        /// <param name="currentStructuralVersion">The structural version known to Unity.</param>
        /// <returns>A built out representation of the data.</returns>
        public abstract SerializableTable Pull(string uri, ulong currentDataVersion, int currentStructuralVersion);

        /// <summary>
        ///     Pushes a set of data onto the binding.
        /// </summary>
        /// <param name="uri">The URI to send data to, an absolute path if on local disk.</param>
        /// <param name="serializableTable">The full data set.</param>
        /// <returns>true/false was successful.</returns>
        public abstract bool Push(string uri, SerializableTable serializableTable);
    }
}