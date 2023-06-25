// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GDX.DataTables.DataBindings
{
    public class JavaScriptObjectNotationFormat : FormatBase
    {
        public override SerializableTable Pull(string uri, ulong currentDataVersion, int currentStructuralVersion)
        {
            if (uri == null || !File.Exists(uri)) return null;
            return JsonUtility.FromJson<SerializableTable>(File.ReadAllText(uri));
        }

        public override bool Push(string uri, SerializableTable serializableTable)
        {
            if (uri == null) return false;
            File.WriteAllText(uri, JsonUtility.ToJson(serializableTable), new UTF8Encoding());
            return true;
        }

        public static bool IsHeader(string header)
        {
            return header.StartsWith("{\"DataVersion\":", StringComparison.OrdinalIgnoreCase);
        }

        public override DateTime GetBindingTimestamp(string uri)
        {
            return File.GetLastWriteTimeUtc(uri);
        }

        public override bool FoundUriHint(string uri)
        {
            return uri.EndsWith("csv", StringComparison.OrdinalIgnoreCase);
        }


    }
}