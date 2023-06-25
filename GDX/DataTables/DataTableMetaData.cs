// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using GDX.DataTables.DataBindings;
using UnityEngine;

namespace GDX.DataTables
{
    /// <summary>
    ///     A collection of metadata for a <see cref="DataTableBase" />.
    /// </summary>
    /// <remarks>
    ///     These objects are created with the <see cref="HideFlags" /> preventing them from entering a build,
    ///     or being seen/editted directly in the editor.
    /// </remarks>
    public class DataTableMetaData : ScriptableObject
    {
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     A `source of truth` binding for the data found in the <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>
        ///     On-disk bindings are relative to the asset folder.
        /// </remarks>
        public string BindingUri;

        /// <summary>
        ///     The timestamp last gotten from the binding.
        /// </summary>
        public DateTime BindingTimestamp;

        /// <summary>
        ///     The data version number used the last time data was pushed to the binding.
        /// </summary>
        public ulong BindingDataVersion;

        /// <summary>
        ///     What format of data is the sync done with.
        /// </summary>
        public DataBindingProvider.Format BindingFormat = DataBindingProvider.Format.Invalid;

        /// <summary>
        ///     The user-friendly name used throughout the author-time experience for the <see cref="DataTableBase" />.
        /// </summary>
        public string DisplayName = "GDX DataTable";

        /// <summary>
        ///     Locks out fields on the <see cref="DataTableBase" /> which are not reference based.
        ///     This is useful for when a dataset is being driven by a binding.
        /// </summary>
        public bool ReferencesOnlyMode;

        /// <summary>
        ///     EXPERIMENTAL! Supports undo/redo operations on the <see cref="DataTableBase" />.
        /// </summary>
        public bool SupportsUndo;

        // ReSharper enable InconsistentNaming
#pragma warning restore IDE1006

#if UNITY_2021_3_OR_NEWER
        public void SetBinding(string uri)
        {
            DataBindingProvider.Format format = ValidateBinding(uri);
            if (format != DataBindingProvider.Format.Invalid)
            {
                BindingUri = uri;
                BindingFormat = format;
            }
            else
            {
                BindingUri = null;
                BindingFormat = DataBindingProvider.Format.Invalid;
            }
        }

        public static DataBindingProvider.Format ValidateBinding(string uri)
        {
            if (uri == null)
            {
                return DataBindingProvider.Format.Invalid;
            }

            string filePath = Path.Combine(Application.dataPath, uri);
            if (File.Exists(filePath))
            {
                return DataBindingProvider.GetFormatFromFile(filePath);
            }

            // Use custom hint check
            if (DataBindingProvider.CustomFormat != null && DataBindingProvider.CustomFormat.FoundUriHint(uri))
            {
                return DataBindingProvider.Format.Custom;
            }

            return DataBindingProvider.Format.Invalid;
        }

        public static string CreateBinding(string uri)
        {
            // Handle file path
            if (File.Exists(uri))
            {
                return Path.GetRelativePath(Application.dataPath, uri);
            }

            return null;
        }

        public bool HasBinding()
        {
            return BindingFormat != DataBindingProvider.Format.Invalid && BindingUri != null;
        }

#endif // UNITY_2021_3_OR_NEWER
    }
}