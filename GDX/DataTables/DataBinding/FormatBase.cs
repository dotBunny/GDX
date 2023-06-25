// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX.DataTables.DataBindings
{
    /// <summary>
    ///     A custom binding to be used with a <see cref="DataTableBase"/>.
    /// </summary>
    public abstract class FormatBase
    {
        /// <summary>
        ///     Get an updated data set to apply to the <see cref="DataTableBase" />.
        /// </summary>
        /// <param name="uri">The URI to access data from.</param>
        /// <param name="currentDataVersion">The data version known to Unity.</param>
        /// <param name="currentStructuralVersion">The structural version known to Unity.</param>
        /// <returns>A built out representation of the data.</returns>
        public abstract SerializableTable Pull(string uri, ulong currentDataVersion, int currentStructuralVersion);

        /// <summary>
        ///     Pushes a set of data onto the binding.
        /// </summary>
        /// <param name="uri">The URI to send data to.</param>
        /// <param name="serializableTable">The full data set.</param>
        /// <returns>true/false was successful.</returns>
        public abstract bool Push(string uri, SerializableTable serializableTable);

        public abstract DateTime GetBindingTimestamp(string uri);

        public abstract bool FoundUriHint(string uri);
    }
}