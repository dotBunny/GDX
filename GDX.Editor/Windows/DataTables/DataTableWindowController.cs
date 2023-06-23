// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public class DataTableWindowController
    {
        readonly DataTableWindowOverlay m_Overlay;
        readonly DataTableWindow m_DataTableWindow;

        internal DataTableWindowController(DataTableWindow window, DataTableWindowOverlay overlay)
        {
            m_DataTableWindow = window;
            m_Overlay = overlay;
        }

        public void ShowAddRowDialog()
        {
            m_Overlay.SetState(DataTableWindowOverlay.OverlayState.AddRow);
        }

        public void ShowAddColumnDialog()
        {
            m_Overlay.SetState(DataTableWindowOverlay.OverlayState.AddColumn);
        }

        public void ShowRenameRowDialog(int rowIdentifier)
        {
            m_Overlay.SetState(DataTableWindowOverlay.OverlayState.RenameRow, rowIdentifier,
                m_DataTableWindow.GetDataTable().GetRowName(rowIdentifier));
        }

        public void ShowRenameColumnDialog(int columnIdentifier)
        {
            m_Overlay.SetState(DataTableWindowOverlay.OverlayState.RenameColumn, columnIdentifier,
                m_DataTableWindow.GetDataTable().GetColumnName(columnIdentifier));
        }

        public void ShowRemoveColumnDialog(int columnIdentifier)
        {
            m_Overlay.SetConfirmationState(DataTableWindowOverlay.ConfirmationState.RemoveColumn, columnIdentifier,
                "Remove Column",
                $"Are you sure you wish to delete column '{m_DataTableWindow.GetDataTable().GetColumnName(columnIdentifier)}'?");
        }

        public void ShowRemoveRowDialog(int rowIdentifier)
        {
            m_Overlay.SetConfirmationState(DataTableWindowOverlay.ConfirmationState.RemoveRow, rowIdentifier,
                "Remove Row",
                $"Are you sure you wish to delete row '{m_DataTableWindow.GetDataTable().GetRowName(rowIdentifier)}'?");
        }

        public void ShowSettings()
        {
            m_Overlay.SetState(DataTableWindowOverlay.OverlayState.Settings);
        }

        public void OpenHelp()
        {
            GUIUtility.hotControl = 0;
            Application.OpenURL($"{PackageProvider.GetDocumentationBaseUri()}/manual/features/data-tables.html");
        }

        public bool AddColumn(string name, Serializable.SerializableTypes type, string secondary = null, int orderedIndex = -1)
        {
            DataTableTracker.RecordColumnDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), -1, $"Add '{name}'");

            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            int columnIdentifier = dataTable.AddColumn(type, name, orderedIndex);
            if (!string.IsNullOrEmpty(secondary))
            {
                System.Type newType = System.Type.GetType(secondary);
                if (newType != null)
                {
                    dataTable.SetTypeNameForColumn(columnIdentifier, secondary);
                }
            }

            DataTableTracker.NotifyOfColumnChange(m_DataTableWindow.GetDataTableTicket(), columnIdentifier);
            return true;
        }

        public bool AddRow(string name, int orderedIndex = -1)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();

            if (dataTable.GetColumnCount() == 0)
            {
                return false;
            }

            DataTableTracker.RecordRowDefinitionUndo(m_DataTableWindow.GetDataTableTicket(),-1, $"Add '{name}'");
            int rowIdentifier = dataTable.AddRow(name, orderedIndex);

            DataTableTracker.NotifyOfRowChange(m_DataTableWindow.GetDataTableTicket(), rowIdentifier);
            return true;
        }

        public void AddRowDefault()
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            if (dataTable.GetColumnCount() == 0)
            {
                return;
            }

            DataTableTracker.RecordRowDefinitionUndo(m_DataTableWindow.GetDataTableTicket(),-1, "Add Default");
            int rowIdentifier = dataTable.AddRow($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");

            m_Overlay.SetOverlayStateHidden();

            DataTableTracker.NotifyOfRowChange(m_DataTableWindow.GetDataTableTicket(), rowIdentifier);
        }

        public bool MoveColumnRight(int columnIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            if (dataTable.GetColumnCount() <= 1)
            {
                return false;
            }

            int currentOrder = dataTable.GetColumnOrder(columnIdentifier);
            if(currentOrder < (dataTable.GetColumnCount() - 1))
            {
                DataTableTracker.RecordColumnDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), columnIdentifier, "Move Column Right");
                dataTable.SetColumnOrder(columnIdentifier, currentOrder + 1);
                DataTableTracker.NotifyOfColumnChange(m_DataTableWindow.GetDataTableTicket(), columnIdentifier);
                return true;
            }
            return false;

        }

        public bool MoveColumnLeft(int columnIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            if (dataTable.GetColumnCount() <= 1)
            {
                return false;
            }

            int currentOrder = dataTable.GetColumnOrder(columnIdentifier);
            if (currentOrder > 0)
            {
                DataTableTracker.RecordColumnDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), columnIdentifier, "Move Column Left");
                dataTable.SetColumnOrder(columnIdentifier, currentOrder - 1);
                DataTableTracker.NotifyOfColumnChange(m_DataTableWindow.GetDataTableTicket(), columnIdentifier);
                return true;
            }
            return false;
        }

        public void RemoveSelectedRow()
        {
            object selectedItem = m_DataTableWindow.GetView().GetMultiColumnListView().selectedItem;
            if (selectedItem == null) return;

            RowDescription selectedRow =
                (RowDescription)m_DataTableWindow.GetView().GetMultiColumnListView().selectedItem;
            DataTableTracker.RecordRowDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), selectedRow.Identifier, "Remove Selected");

            m_DataTableWindow.GetDataTable().RemoveRow(selectedRow.Identifier);

            DataTableTracker.NotifyOfRowChange(m_DataTableWindow.GetDataTableTicket(), selectedRow.Identifier);
        }

        public bool RemoveColumn(int columnIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            if (dataTable.GetColumnCount() <= 1)
            {
                return false;
            }

            DataTableTracker.RecordColumnDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), columnIdentifier, "Remove");

            dataTable.RemoveColumn(m_DataTableWindow.GetView().GetColumnType(columnIdentifier), columnIdentifier);

            DataTableTracker.NotifyOfColumnChange(m_DataTableWindow.GetDataTableTicket(), columnIdentifier);
            return true;
        }

        public bool RemoveRow(int rowIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            DataTableTracker.RecordRowDefinitionUndo(m_DataTableWindow.GetDataTableTicket(),rowIdentifier, "Remove");
            dataTable.RemoveRow(rowIdentifier);

            DataTableTracker.NotifyOfRowChange(m_DataTableWindow.GetDataTableTicket(), rowIdentifier);
            return true;
        }

        public bool RenameRow(int rowIdentifier, string name)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();

            DataTableTracker.RecordRowDefinitionUndo(m_DataTableWindow.GetDataTableTicket(),rowIdentifier);
            dataTable.SetRowName(rowIdentifier, name);
            DataTableTracker.NotifyOfRowChange(m_DataTableWindow.GetDataTableTicket(), rowIdentifier);
            return true;
        }

        public bool RenameColumn(int columnIdentifier, string name)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            DataTableTracker.RecordColumnDefinitionUndo(m_DataTableWindow.GetDataTableTicket(), columnIdentifier, $"Rename '{name}'");

            // Update column data in place
            m_DataTableWindow.GetView().UpdateColumnData(columnIdentifier, name);

            dataTable.SetColumnName(columnIdentifier, name);

            DataTableTracker.NotifyOfColumnChange(m_DataTableWindow.GetDataTableTicket(), columnIdentifier,  m_DataTableWindow);
            return true;
        }

        public bool SetTableSettings(string displayName, string sourceOfTruth, bool enableUndo, bool supportReferenceOnlyMode)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            DataTableTracker.RecordSettingsUndo(m_DataTableWindow.GetDataTableTicket());

            // Check if there is a change
            DataTableMetaData metaData = dataTable.GetMetaData();
            metaData.DisplayName = displayName;
            metaData.SupportsUndo = enableUndo;
            metaData.ReferencesOnlyMode = supportReferenceOnlyMode;
            metaData.SetSourceOfTruth(sourceOfTruth); // this will validate again

            EditorUtility.SetDirty(m_DataTableWindow.GetDataTable());

            DataTableTracker.NotifyOfSettingsChange(m_DataTableWindow.GetDataTableTicket());
            return true;
        }

        public void AutoResizeColumns()
        {
            Reflection.InvokeMethod(m_DataTableWindow.GetView().GetColumnContainer(), "ResizeToFit");
        }

        // Double delays the auto resize call
        public void DelayedAutoResizeColumns()
        {
            EditorApplication.delayCall += AutoResizeColumns;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}