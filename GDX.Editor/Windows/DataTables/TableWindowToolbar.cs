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
    class TableWindowToolbar
    {
        readonly DataTableWindow m_ParentWindow;
        readonly Toolbar m_Toolbar;
        readonly ToolbarMenu m_ToolbarColumnMenu;
        readonly ToolbarButton m_ToolbarHelpButton;
        readonly ToolbarButton m_ToolbarSaveButton;
        readonly ToolbarMenu m_ToolbarInterchangeMenu;
        readonly ToolbarMenu m_ToolbarRowMenu;
        readonly ToolbarButton m_ToolbarSettingsButton;

        internal TableWindowToolbar(Toolbar toolbar, DataTableWindow window)
        {
            m_Toolbar = toolbar;
            m_ParentWindow = window;

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
            m_ToolbarRowMenu.menu.AppendAction("Rename Selected", _ => { RenameRow(); }, CanOperateOnRow);
            m_ToolbarRowMenu.menu.AppendAction("Remove Selected", _ => { RemoveRow(); }, CanOperateOnRow);

            m_ToolbarInterchangeMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-interchange");
            m_ToolbarInterchangeMenu.menu.AppendAction("Export to CSV",
                _ => { m_ParentWindow.GetController().ShowExportDialog(); }, CanInterchange);
            m_ToolbarInterchangeMenu.menu.AppendAction("Import from CSV",
                _ => { m_ParentWindow.GetController().ShowImportDialog(); }, CanInterchange);

            m_ToolbarSaveButton = m_Toolbar.Q<ToolbarButton>("gdx-table-toolbar-save");
            m_ToolbarSaveButton.text = string.Empty;
            m_ToolbarSaveButton.clicked += Save;
            m_ToolbarSaveButton.SetEnabled(false);

            m_ToolbarSettingsButton = m_Toolbar.Q<ToolbarButton>("gdx-table-toolbar-settings");
            m_ToolbarSettingsButton.text = string.Empty;
            m_ToolbarSettingsButton.clicked += ShowSettings;
            m_ToolbarHelpButton = m_Toolbar.Q<ToolbarButton>("gdx-table-toolbar-help");
            m_ToolbarHelpButton.text = string.Empty;
            m_ToolbarHelpButton.clicked += OpenHelp;

        }

        internal void SetFocusable(bool state)
        {
            m_Toolbar.focusable = state;
            m_ToolbarColumnMenu.focusable = state;
            m_ToolbarRowMenu.focusable = state;
            m_ToolbarSettingsButton.focusable = state;
            m_ToolbarInterchangeMenu.focusable = state;
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

        void RenameRow()
        {
            DataTableObject.RowDescription selectedItem =
                (DataTableObject.RowDescription)m_ParentWindow.GetView().GetMultiColumnListView().selectedItem;
            m_ParentWindow.GetController().ShowRenameRowDialog(selectedItem.Identifier);
        }

        void RemoveRow()
        {
            DataTableObject.RowDescription selectedItem =
                (DataTableObject.RowDescription)m_ParentWindow.GetView().GetMultiColumnListView().selectedItem;
            m_ParentWindow.GetController().ShowRemoveRowDialog(selectedItem.Identifier);
        }

        void Save()
        {
            m_ParentWindow.Save();
        }

        void ShowSettings()
        {
            m_ParentWindow.GetController().ShowSettings();
        }

        void OpenHelp()
        {
            m_ParentWindow.GetController().OpenHelp();
        }

        public void UpdateSaveButton()
        {
            m_ToolbarSaveButton?.SetEnabled(EditorUtility.IsDirty(m_ParentWindow.GetDataTable()));
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}