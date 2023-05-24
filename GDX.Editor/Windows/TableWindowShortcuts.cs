// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GDX.Editor.Windows
{
    internal static class TableWindowShortcuts
    {
        [GDXShortcut("Add Row",  KeyCode.Equals, ShortcutModifiers.Control, typeof(TableWindow))]
        internal static void AddRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.AddRow();
        }

        [GDXShortcut("Add Row (Quick)",  KeyCode.Equals, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void AddQuickRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.AddRowQuick();
        }

        [GDXShortcut("Add Column", KeyCode.Minus, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void AddColumn()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.AddColumn();
        }

        [GDXShortcut("Remove Selected Row(s)", KeyCode.Delete, ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void RemoveSelectedRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if(table != null) table.RemoveSelectedRows();
        }

    }
}