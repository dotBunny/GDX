// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine;

namespace GDX.DataTables
{
    public class DataTableMetaData : ScriptableObject
    {
        public string DisplayName = "GDX DataTable";
        public string SourcePath = null;
        public bool ReferencesOnlyMode = false;
        public bool AllowUndo = false;
    }
}