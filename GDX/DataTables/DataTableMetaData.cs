// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Serialization;

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
        ///     A `source of truth` binding for the data found in the <see cref="DataTableBase"/>.
        /// </summary>
        /// <remarks>
        ///     On-disk bindings are relative to the asset folder.
        /// </remarks>
        public string BindingUri = null;

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
        public DataTableInterchange.Format BindingFormat = DataTableInterchange.Format.Invalid;

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

        public void SetBinding(string uri)
        {
            DataTableInterchange.Format format = ValidateBinding(uri);
            if(format != DataTableInterchange.Format.Invalid)
            {
                BindingUri = uri;
                BindingFormat = format;
            }
            else
            {
                BindingUri = null;
                BindingFormat = DataTableInterchange.Format.Invalid;
            }
        }
        public static DataTableInterchange.Format ValidateBinding(string uri)
        {
            if (uri == null) return DataTableInterchange.Format.Invalid;

            string filePath = System.IO.Path.Combine(Application.dataPath, uri);
            if (System.IO.File.Exists(filePath))
            {
                return DataTableInterchange.GetFormatFromFile(filePath);
            }

            // Validate online?

            return DataTableInterchange.Format.Invalid;
        }

        public static string CreateBinding(string uri)
        {
            // Handle file path
            if (System.IO.File.Exists(uri))
            {
                return System.IO.Path.GetRelativePath(Application.dataPath, uri);
            }
            return null;
        }

        public bool HasBinding()
        {
            return BindingFormat != DataTableInterchange.Format.Invalid && BindingUri != null;
        }
    }
}