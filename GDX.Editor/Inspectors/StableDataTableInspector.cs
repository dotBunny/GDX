﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables;
using UnityEditor;

namespace GDX.Editor.Inspectors
{
    /// <summary>
    ///     Adds a custom inspector to <see cref="StableDataTable"/>s.
    /// </summary>
    [CustomEditor(typeof(StableDataTable))]
    public class StableDataTableInspector : DataTableInspectorBase
    {

    }
}