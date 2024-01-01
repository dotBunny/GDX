// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    static class DataTableWindowShortcuts
    {
        [GDXShortcut("Add Row", KeyCode.Equals, ShortcutModifiers.Control, typeof(DataTableWindow))]
        internal static void AddRow()
        {
            DataTableWindow dataTable = (DataTableWindow)EditorWindow.focusedWindow;
            if (dataTable != null)
            {
                dataTable.GetController().ShowAddRowDialog();
            }
        }

        [GDXShortcut("Add Row (Default)", KeyCode.Equals, ShortcutModifiers.Control | ShortcutModifiers.Shift,
            typeof(DataTableWindow))]
        internal static void AddRowDefault()
        {
            DataTableWindow dataTable = (DataTableWindow)EditorWindow.focusedWindow;
            if (dataTable != null)
            {
                dataTable.GetController().AddRowDefault();
            }
        }

        [GDXShortcut("Add Column", KeyCode.Minus, ShortcutModifiers.Control | ShortcutModifiers.Shift,
            typeof(DataTableWindow))]
        internal static void AddColumn()
        {
            DataTableWindow dataTable = (DataTableWindow)EditorWindow.focusedWindow;
            if (dataTable != null)
            {
                dataTable.GetController().ShowAddColumnDialog();
            }
        }

        [GDXShortcut("Remove Selected Row", KeyCode.Delete, ShortcutModifiers.None, typeof(DataTableWindow))]
        internal static void RemoveSelectedRow()
        {
            DataTableWindow dataTable = (DataTableWindow)EditorWindow.focusedWindow;
            if (dataTable != null)
            {
                dataTable.GetController().ShowRemoveRowDialog(dataTable.GetView().GetSelectedRowIdentifier());
            }
        }

        [GDXShortcut("Remove Selected Row (Quick)", KeyCode.Delete, ShortcutModifiers.Shift, typeof(DataTableWindow))]
        internal static void RemoveSelectedRowQuick()
        {
            DataTableWindow dataTable = (DataTableWindow)EditorWindow.focusedWindow;
            if (dataTable != null)
            {
                dataTable.GetController().RemoveSelectedRow();
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}