// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GDX.DataTables.DataBindings
{
    /// <summary>
    /// A JavaScript Object Notation format.
    /// </summary>
    class JavaScriptObjectNotationFormat : FormatBase
    {
        public JavaScriptObjectNotationFormat()
        {
            // We can register the format this way because we automatically include these formats in the
            // DataBindingsProvider as private members.
            DataBindingProvider.RegisterFormat(this);
        }

        ~JavaScriptObjectNotationFormat()
        {
            DataBindingProvider.UnregisterFormat(this);
        }

        /// <inheritdoc />
        public override DateTime GetBindingTimestamp(string uri)
        {
            return File.GetLastWriteTimeUtc(uri);
        }

        /// <inheritdoc />
        public override string GetFilePreferredExtension()
        {
            return "json";
        }

        /// <inheritdoc />
        public override string GetFriendlyName()
        {
            return "JSON";
        }

        /// <inheritdoc />
        public override bool IsFileHeader(string header)
        {
            return header.StartsWith("{\"DataVersion\":", StringComparison.OrdinalIgnoreCase);
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

            return JsonUtility.FromJson<SerializableTable>(File.ReadAllText(uri));
        }

        /// <inheritdoc />
        public override bool Push(string uri, SerializableTable serializableTable)
        {
            if (uri == null)
            {
                return false;
            }

            File.WriteAllText(uri, JsonUtility.ToJson(serializableTable), new UTF8Encoding());
            return File.Exists(uri);
        }
    }
}