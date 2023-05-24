// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    static class TableWindowShortcuts
    {
        [GDXShortcut("Add Row",  KeyCode.Equals, ShortcutModifiers.Control, typeof(TableWindow))]
        internal static void AddRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if (table != null)
            {
                table.GetController().ShowAddRowDialog();
            }
        }

        [GDXShortcut("Add Row (Default)",  KeyCode.Equals, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void AddRowDefault()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if (table != null)
            {
                table.GetController().AddRowDefault();
            }
        }

        [GDXShortcut("Add Column", KeyCode.Minus, ShortcutModifiers.Control | ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void AddColumn()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if (table != null)
            {
                table.GetController().ShowAddColumnDialog();
            }
        }

        [GDXShortcut("Remove Selected Row", KeyCode.Delete, ShortcutModifiers.None, typeof(TableWindow))]
        internal static void RemoveSelectedRow()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if (table != null)
            {
                table.GetController().ShowRemoveRowDialog(table.GetView().GetSelectedRowInternalIndex());
            }
        }

        [GDXShortcut("Remove Selected Row (Quick)", KeyCode.Delete, ShortcutModifiers.Shift, typeof(TableWindow))]
        internal static void RemoveSelectedRowQuick()
        {
            TableWindow table = (TableWindow)EditorWindow.focusedWindow;
            if (table != null)
            {
                table.GetController().RemoveSelectedRow();
            }
        }
    }
#endif
}