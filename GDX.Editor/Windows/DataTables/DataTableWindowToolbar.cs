﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    class DataTableWindowToolbar
    {
        readonly DataTableWindow m_ParentWindow;
        readonly Toolbar m_Toolbar;
        readonly ToolbarMenu m_ToolbarColumnMenu;
        readonly ToolbarButton m_ToolbarHelpButton;
        readonly ToolbarMenu m_ToolbarRowMenu;
        readonly ToolbarMenu m_ToolbarTableMenu;

        internal DataTableWindowToolbar(Toolbar toolbar, DataTableWindow window)
        {
            m_Toolbar = toolbar;
            m_ParentWindow = window;

            // Create our File menu
            m_ToolbarTableMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-table");
            m_ToolbarTableMenu.menu.AppendAction("Write To Disk",
                _ => { m_ParentWindow.Save(); }, CanSave);
            m_ToolbarTableMenu.menu.AppendSeparator();
            m_ToolbarTableMenu.menu.AppendAction("Export to CSV",
                _ => { m_ParentWindow.GetController().ShowExportDialog(); }, CanInterchange);
            m_ToolbarTableMenu.menu.AppendAction("Import from CSV",
                _ => { m_ParentWindow.GetController().ShowImportDialog(); }, CanInterchange);
            m_ToolbarTableMenu.menu.AppendSeparator();
            m_ToolbarTableMenu.menu.AppendAction("Settings",
                _ => { m_ParentWindow.GetController().ShowSettings(); });

            // Create our Col
            m_ToolbarColumnMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-column");
            m_ToolbarColumnMenu.menu.AppendAction("Add",
                _ => { m_ParentWindow.GetController().ShowAddColumnDialog(); });
            m_ToolbarColumnMenu.menu.AppendSeparator();
            m_ToolbarColumnMenu.menu.AppendAction("Resize To Fit",
                _ => { m_ParentWindow.GetController().AutoResizeColumns(); });

            m_ToolbarRowMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-row");
            m_ToolbarRowMenu.menu.AppendAction("Add", _ => { m_ParentWindow.GetController().ShowAddRowDialog(); },
                CanAddRow);
            m_ToolbarRowMenu.menu.AppendAction("Add (Default)",
                _ => { m_ParentWindow.GetController().AddRowDefault(); }, CanAddRow);
            m_ToolbarRowMenu.menu.AppendSeparator();
            m_ToolbarRowMenu.menu.AppendAction("Rename Selected", _ =>
            {
                RowDescription selectedItem =
                    (RowDescription)m_ParentWindow.GetView().GetMultiColumnListView().selectedItem;
                m_ParentWindow.GetController().ShowRenameRowDialog(selectedItem.Identifier);
            }, CanOperateOnRow);
            m_ToolbarRowMenu.menu.AppendAction("Remove Selected", _ =>
            {
                RowDescription selectedItem =
                    (RowDescription)m_ParentWindow.GetView().GetMultiColumnListView().selectedItem;
                m_ParentWindow.GetController().ShowRemoveRowDialog(selectedItem.Identifier);
            }, CanOperateOnRow);
            m_ToolbarRowMenu.menu.AppendSeparator();
            m_ToolbarRowMenu.menu.AppendAction("Commit Order",
                _ => { m_ParentWindow.GetView().CommitOrder(); }, CanCommitOrder);
            m_ToolbarRowMenu.menu.AppendAction("Reset Order",
                _ => { m_ParentWindow.GetView().RebuildRowData(); }, CanCommitOrder);

            m_ToolbarHelpButton = m_Toolbar.Q<ToolbarButton>("gdx-table-toolbar-help");
            m_ToolbarHelpButton.text = string.Empty;
            m_ToolbarHelpButton.clicked += OpenHelp;
        }

        internal void SetFocusable(bool state)
        {
            m_Toolbar.focusable = state;

            m_ToolbarTableMenu.focusable = state;
            m_ToolbarColumnMenu.focusable = state;
            m_ToolbarRowMenu.focusable = state;

            m_ToolbarHelpButton.focusable = state;
        }


        public DropdownMenuAction.Status CanCommitOrder(DropdownMenuAction action)
        {
            return m_ParentWindow.GetView().HasSortedColumns() || m_ParentWindow.GetView().HasManualRows()
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        public DropdownMenuAction.Status HasSortedColumns(DropdownMenuAction action)
        {
            return m_ParentWindow.GetView().HasSortedColumns()
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanSave(DropdownMenuAction action)
        {
            return EditorUtility.IsDirty(m_ParentWindow.GetDataTable())
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanAddRow(DropdownMenuAction action)
        {
            return m_ParentWindow.GetDataTable().GetColumnCount() > 0
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanInterchange(DropdownMenuAction action)
        {
            return m_ParentWindow.GetDataTable().GetColumnCount() > 0
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanOperateOnRow(DropdownMenuAction action)
        {
            return m_ParentWindow.GetView().GetMultiColumnListView().selectedItem != null
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }

        void OpenHelp()
        {
            m_ParentWindow.GetController().OpenHelp();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}