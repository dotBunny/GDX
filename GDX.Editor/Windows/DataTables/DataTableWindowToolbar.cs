// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.DataTables;
using GDX.Editor.Inspectors;
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
        readonly VisualElement m_ToolbarSourceStatus;
        readonly VisualElement m_ToolbarReferencesOnlyStatus;
        readonly ToolbarMenu m_ToolbarRowMenu;
        readonly ToolbarMenu m_ToolbarSourceMenu;
        readonly ToolbarMenu m_ToolbarTableMenu;

        internal DataTableWindowToolbar(Toolbar toolbar, DataTableWindow window)
        {
            m_Toolbar = toolbar;
            m_ParentWindow = window;

            // Create our File menu
            m_ToolbarTableMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-table");
            m_ToolbarTableMenu.menu.AppendAction("Write To Disk",
                _ => { m_ParentWindow.Save(true); }, CanSave);
            m_ToolbarTableMenu.menu.AppendSeparator();
            m_ToolbarTableMenu.menu.AppendAction("Export (CSV) ...",
                _ =>
                {
                    DataTableInspectorBase.ShowExportDialogForTable(DataTableInterchange.Format.CommaSeperatedValues,
                        m_ParentWindow.GetDataTable());
                }, CanInterchange);
            m_ToolbarTableMenu.menu.AppendAction("Export (JSON) ...",
                _ =>
                {
                    DataTableInspectorBase.ShowExportDialogForTable(
                        DataTableInterchange.Format.JavaScriptObjectNotation, m_ParentWindow.GetDataTable());
                }, CanInterchange);
            m_ToolbarTableMenu.menu.AppendAction("Import ...",
                _ => { DataTableInspectorBase.ShowImportDialogForTable(m_ParentWindow.GetDataTable()); },
                CanInterchange);
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

            m_ToolbarSourceMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-source");
            m_ToolbarSourceMenu.style.display = DisplayStyle.None;
            m_ToolbarSourceMenu.menu.AppendAction("Pull",
                _ => { DataTableInspectorBase.SourcePull(m_ParentWindow.GetDataTable()); });
            m_ToolbarSourceMenu.menu.AppendAction("Push",
                _ => { DataTableInspectorBase.SourcePush(m_ParentWindow.GetDataTable()); });

            m_ToolbarSourceStatus = m_Toolbar.Q<VisualElement>("gdx-table-toolbar-source-pull");
            m_ToolbarSourceStatus.style.display = DisplayStyle.None;
            m_ToolbarSourceStatus.tooltip = "The source of truth has been detected as newer then your local data.";

            m_ToolbarReferencesOnlyStatus = m_Toolbar.Q<VisualElement>("gdx-table-toolbar-references");
            m_ToolbarReferencesOnlyStatus.style.display = DisplayStyle.None;
            m_ToolbarReferencesOnlyStatus.tooltip = "The table is currently in References Only mode, this can be changed in it's settings.";

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
            m_ToolbarSourceMenu.focusable = state;

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
            DataTableBase dataTable = m_ParentWindow.GetDataTable();
            DataTableMetaData metaData = dataTable.GetMetaData();
            return EditorUtility.IsDirty(dataTable) || EditorUtility.IsDirty(metaData)
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

        public void UpdateForSettings()
        {
            DataTableMetaData metaData = m_ParentWindow.GetDataTable().GetMetaData();

            m_ToolbarSourceMenu.style.display = metaData.HasSourceOfTruth() ? DisplayStyle.Flex : DisplayStyle.None;
            m_ToolbarReferencesOnlyStatus.style.display = metaData.ReferencesOnlyMode ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}