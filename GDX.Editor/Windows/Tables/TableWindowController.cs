// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.Inspectors;
using GDX.Tables;
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
                    if (m_TableWindow.GetTable().UpdateFromCommaSeperatedValues(openPath))
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
                m_TableWindow.GetTable().name, "csv");

            if (savePath != null)
            {
                m_TableWindow.GetTable().ExportToCommaSeperatedValues(savePath);
                Debug.Log($"'{m_TableWindow.GetTable().GetDisplayName()}' was exported to CSV at {savePath}");
            }
        }

        public void ShowAddColumnDialog()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.AddColumn);
        }

        public void ShowRenameRowDialog(int internalIndex)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameRow, internalIndex,
                m_TableWindow.GetTable().GetRowName(internalIndex));
        }

        public void ShowRenameColumnDialog(int internalIndex)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameColumn, internalIndex,
                m_TableWindow.GetTable().GetColumnName(internalIndex));
        }

        public void ShowRemoveColumnDialog(int internalIndex)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveColumn, internalIndex,
                "Remove Column",
                $"Are you sure you wish to delete column '{m_TableWindow.GetTable().GetColumnName(internalIndex)}'?");
        }

        public void ShowRemoveRowDialog(int internalIndex)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveRow, internalIndex,
                "Remove Row",
                $"Are you sure you wish to delete row '{m_TableWindow.GetTable().GetRowName(internalIndex)}'?");
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

            TableBase table = m_TableWindow.GetTable();
            int columnIndex = table.AddColumn(type, name, orderedIndex);
            if (!string.IsNullOrEmpty(secondary))
            {
                System.Type newType = System.Type.GetType(secondary);
                if (newType != null)
                {
                    table.SetTypeNameForObjectColumn(columnIndex, secondary);
                }
            }

            TableCache.NotifyOfColumnChange(table);
            return true;
        }

        public bool AddRow(string name, int orderedIndex = -1)
        {
            TableBase table = m_TableWindow.GetTable();
            if (table.GetColumnCount() == 0)
            {
                return false;
            }

            RegisterUndo($"Add Row ({name})");
            table.AddRow(name, orderedIndex);

            TableCache.NotifyOfRowChange(table);
            return true;
        }

        public void AddRowDefault()
        {
            TableBase table = m_TableWindow.GetTable();
            if (table.GetColumnCount() == 0)
            {
                return;
            }

            RegisterUndo("Add Default Row");
            table.AddRow($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");

            m_Overlay.SetOverlayStateHidden();

            TableCache.NotifyOfRowChange(table);
        }

        public void RemoveSelectedRow()
        {
            object selectedItem = m_TableWindow.GetView().GetMultiColumnListView().selectedItem;
            if (selectedItem == null) return;

            TableBase.RowDescription selectedRow =
                (TableBase.RowDescription)m_TableWindow.GetView().GetMultiColumnListView().selectedItem;
            RegisterUndo($"Remove Row ({selectedRow.Name})");
            m_TableWindow.GetTable().RemoveRow(selectedRow.Identifier);

            TableCache.NotifyOfRowChange(m_TableWindow.GetTable());
        }

        public bool RemoveColumn(int internalIndex)
        {
            TableBase table = m_TableWindow.GetTable();
            if (table.GetColumnCount() <= 1)
            {
                return false;
            }

            RegisterUndo($"Remove Column ({table.GetColumnName(internalIndex)})");

            table.RemoveColumn(m_TableWindow.GetView().GetColumnType(internalIndex), internalIndex);

            TableCache.NotifyOfColumnChange(table);
            return true;
        }

        public bool RemoveRow(int internalIndex)
        {
            TableBase table = m_TableWindow.GetTable();
            RegisterUndo($"Remove Row ({table.GetRowName(internalIndex)})");
            table.RemoveRow(internalIndex);

            TableCache.NotifyOfRowChange(table);
            return true;
        }

        public bool RenameRow(int internalIndex, string name)
        {
            TableBase table = m_TableWindow.GetTable();
            RegisterUndo($"Rename Row ({name})");
            table.SetRowName(name, internalIndex);

            TableCache.NotifyOfRowChange(table);
            return true;
        }

        public bool RenameColumn(int internalIndex, string name)
        {
            TableBase table = m_TableWindow.GetTable();
            RegisterUndo($"Rename Column ({name})");

            // Update column data in place
            m_TableWindow.GetView().UpdateColumnData(internalIndex, name);

            table.SetColumnName(name, internalIndex);

            TableCache.NotifyOfColumnChange(table, m_TableWindow);
            m_TableWindow.GetToolbar().UpdateSaveButton();
            return true;
        }

        public bool SetTableSettings(string displayName, bool enableUndo)
        {
            TableBase table = m_TableWindow.GetTable();
            RegisterUndo("Table Settings");

            // Check if there is a change
            string tableDisplayName = table.GetDisplayName();
            if (tableDisplayName != displayName)
            {
                table.SetDisplayName(displayName);
                m_TableWindow.titleContent = new GUIContent(displayName);
            }

            table.SetFlag(TableBase.Flags.EnableUndo, enableUndo);
            EditorUtility.SetDirty(m_TableWindow.GetTable());
            m_TableWindow.GetToolbar().UpdateSaveButton();
            return true;
        }

        public void AutoResizeColumns()
        {
            Reflection.InvokeMethod(m_TableWindow.GetView().GetColumnContainer(), "ResizeToFit");
        }

        void RegisterUndo(string name)
        {
            ScriptableObject scriptableObject = m_TableWindow.GetTable();
            if (scriptableObject != null && m_TableWindow.GetTable().GetFlag(TableBase.Flags.EnableUndo))
            {
                Undo.RegisterCompleteObjectUndo(scriptableObject, $"{TableWindowProvider.UndoPrefix} {name}");
            }
        }
    }
#endif
}