// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.Inspectors;
using GDX.DataTables;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindowController
    {
        readonly TableWindowOverlay m_Overlay;
        readonly TableWindow m_TableWindow;

        internal TableWindowController(TableWindow window, TableWindowOverlay overlay)
        {
            m_TableWindow = window;
            m_Overlay = overlay;
        }

        public void ShowAddRowDialog()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.AddRow);
        }

        public void ShowImportDialog()
        {
            string openPath = EditorUtility.OpenFilePanel($"Import CSV into {m_TableWindow.titleContent.text}",
                Application.dataPath,
                "csv");

            if (!string.IsNullOrEmpty(openPath))
            {
                if (EditorUtility.DisplayDialog($"Replace '{m_TableWindow.titleContent.text}' Content",
                        "Are you sure you want to replace your tables content with the imported CSV content?\n\nThe structural format of the CSV needs to match the column structure of the existing table; reference types will not replace the data in the existing cells at that location. Make sure the first row contains the column names, and that you have not reordered the rows or columns.",
                        "Yes", "No"))
                {
                    if (m_TableWindow.GetDataTable().UpdateFromCommaSeperatedValues(openPath))
                    {
                        m_TableWindow.RebindTable();
                    }
                }
            }
        }

        public void ShowExportDialog()
        {
            string savePath = EditorUtility.SaveFilePanel($"Export {m_TableWindow.titleContent.text} to CSV",
                Application.dataPath,
                m_TableWindow.GetDataTable().name, "csv");

            if (savePath != null)
            {
                m_TableWindow.GetDataTable().ExportToCommaSeperatedValues(savePath);
                Debug.Log($"'{m_TableWindow.GetDataTable().GetDisplayName()}' was exported to CSV at {savePath}");
            }
        }

        public void ShowAddColumnDialog()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.AddColumn);
        }

        public void ShowRenameRowDialog(int rowIdentifier)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameRow, rowIdentifier,
                m_TableWindow.GetDataTable().GetRowName(rowIdentifier));
        }

        public void ShowRenameColumnDialog(int columnIdentifier)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameColumn, columnIdentifier,
                m_TableWindow.GetDataTable().GetColumnName(columnIdentifier));
        }

        public void ShowRemoveColumnDialog(int columnIdentifier)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveColumn, columnIdentifier,
                "Remove Column",
                $"Are you sure you wish to delete column '{m_TableWindow.GetDataTable().GetColumnName(columnIdentifier)}'?");
        }

        public void ShowRemoveRowDialog(int rowIdentifier)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveRow, rowIdentifier,
                "Remove Row",
                $"Are you sure you wish to delete row '{m_TableWindow.GetDataTable().GetRowName(rowIdentifier)}'?");
        }

        public void ShowSettings()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.Settings);
        }

        public void OpenHelp()
        {
            GUIUtility.hotControl = 0;
            Application.OpenURL($"{PackageProvider.GetDocumentationBaseUri()}/manual/features/tables.html");
        }

        public bool AddColumn(string name, Serializable.SerializableTypes type, string secondary = null, int orderedIndex = -1)
        {
            RegisterUndo($"Add Column ({name})");

            DataTableObject dataTable = m_TableWindow.GetDataTable();
            int columnIndex = dataTable.AddColumn(type, name, orderedIndex);
            if (!string.IsNullOrEmpty(secondary))
            {
                System.Type newType = System.Type.GetType(secondary);
                if (newType != null)
                {
                    dataTable.SetTypeNameForObjectColumn(columnIndex, secondary);
                }
            }

            TableCache.NotifyOfColumnChange(dataTable);
            return true;
        }

        public bool AddRow(string name, int orderedIndex = -1)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            if (dataTable.GetColumnCount() == 0)
            {
                return false;
            }

            RegisterUndo($"Add Row ({name})");
            dataTable.AddRow(name, orderedIndex);

            TableCache.NotifyOfRowChange(dataTable);
            return true;
        }

        public void AddRowDefault()
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            if (dataTable.GetColumnCount() == 0)
            {
                return;
            }

            RegisterUndo("Add Default Row");
            dataTable.AddRow($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");

            m_Overlay.SetOverlayStateHidden();

            TableCache.NotifyOfRowChange(dataTable);
        }

        public void RemoveSelectedRow()
        {
            object selectedItem = m_TableWindow.GetView().GetMultiColumnListView().selectedItem;
            if (selectedItem == null) return;

            DataTableObject.RowDescription selectedRow =
                (DataTableObject.RowDescription)m_TableWindow.GetView().GetMultiColumnListView().selectedItem;
            RegisterUndo($"Remove Row ({selectedRow.Name})");
            m_TableWindow.GetDataTable().RemoveRow(selectedRow.Identifier);

            TableCache.NotifyOfRowChange(m_TableWindow.GetDataTable());
        }

        public bool RemoveColumn(int columnIdentifier)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            if (dataTable.GetColumnCount() <= 1)
            {
                return false;
            }

            RegisterUndo($"Remove Column ({dataTable.GetColumnName(columnIdentifier)})");

            dataTable.RemoveColumn(m_TableWindow.GetView().GetColumnType(columnIdentifier), columnIdentifier);

            TableCache.NotifyOfColumnChange(dataTable);
            return true;
        }

        public bool RemoveRow(int rowIdentifier)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            RegisterUndo($"Remove Row ({dataTable.GetRowName(rowIdentifier)})");
            dataTable.RemoveRow(rowIdentifier);

            TableCache.NotifyOfRowChange(dataTable);
            return true;
        }

        public bool RenameRow(int rowIdentifier, string name)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            RegisterUndo($"Rename Row ({name})");
            dataTable.SetRowName(name, rowIdentifier);

            TableCache.NotifyOfRowChange(dataTable);
            return true;
        }

        public bool RenameColumn(int columnIdentifier, string name)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            RegisterUndo($"Rename Column ({name})");

            // Update column data in place
            m_TableWindow.GetView().UpdateColumnData(columnIdentifier, name);

            dataTable.SetColumnName(name, columnIdentifier);

            TableCache.NotifyOfColumnChange(dataTable, m_TableWindow);
            m_TableWindow.GetToolbar().UpdateSaveButton();
            return true;
        }

        public bool SetTableSettings(string displayName, bool enableUndo)
        {
            DataTableObject dataTable = m_TableWindow.GetDataTable();
            RegisterUndo("Table Settings");

            // Check if there is a change
            string tableDisplayName = dataTable.GetDisplayName();
            if (tableDisplayName != displayName)
            {
                dataTable.SetDisplayName(displayName);
                m_TableWindow.titleContent = new GUIContent(displayName);
            }

            dataTable.SetFlag(DataTableObject.Flags.EnableUndo, enableUndo);
            EditorUtility.SetDirty(m_TableWindow.GetDataTable());
            m_TableWindow.GetToolbar().UpdateSaveButton();
            return true;
        }

        public void AutoResizeColumns()
        {
            Reflection.InvokeMethod(m_TableWindow.GetView().GetColumnContainer(), "ResizeToFit");
        }

        void RegisterUndo(string name)
        {
            ScriptableObject scriptableObject = m_TableWindow.GetDataTable();
            if (scriptableObject != null && m_TableWindow.GetDataTable().GetFlag(DataTableObject.Flags.EnableUndo))
            {
                Undo.RegisterCompleteObjectUndo(scriptableObject, $"{TableWindowProvider.UndoPrefix} {name}");
            }
        }
    }
#endif
}