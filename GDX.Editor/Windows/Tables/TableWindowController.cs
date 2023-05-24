﻿// Copyright (c) 2020-2023 dotBunny Inc.
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
        readonly TableWindow m_TableWindow;
        readonly TableWindowOverlay m_Overlay;

        internal TableWindowController(TableWindow window, TableWindowOverlay overlay)
        {
            m_TableWindow = window;
            m_Overlay = overlay;
        }

        public void ShowAddRowDialog()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.AddRow);
        }

        public void ShowAddColumnDialog()
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.AddColumn);
        }

        public void ShowRenameRowDialog(int internalIndex)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameRow, internalIndex, m_TableWindow.GetTable().GetRowName(internalIndex));
        }

        public void ShowRenameColumnDialog(int internalIndex)
        {
            m_Overlay.SetState(TableWindowOverlay.OverlayState.RenameColumn, internalIndex, m_TableWindow.GetTable().GetColumnName(internalIndex));
        }

        public void ShowRemoveColumnDialog(int internalIndex)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveColumn, internalIndex,
                "Remove Column", "Are you sure you wish to delete this column?");
        }

        public void ShowRemoveRowDialog(int internalIndex)
        {
            m_Overlay.SetConfirmationState(TableWindowOverlay.ConfirmationState.RemoveRow, internalIndex,
                "Remove Row", "Are you sure you wish to delete this row?");
        }

        public bool AddColumn(string name, Serializable.SerializableTypes type,  int orderedIndex = -1)
        {
            RegisterUndo($"Add Column ({name})");

            ITable table = m_TableWindow.GetTable();
            table.AddColumn(type, name, orderedIndex);

            SetDirty();
            m_TableWindow.BindTable(table);
            TableInspectorBase.RedrawInspector(table);
            return true;
        }

        public bool AddRow(string name, int orderedIndex = -1)
        {
            ITable table = m_TableWindow.GetTable();
            if (table.GetColumnCount() == 0)
            {
                return false;
            }

            RegisterUndo($"Add Row ({name})");
            table.AddRow(name, orderedIndex);

            m_TableWindow.GetView().RebuildRowData();
            TableInspectorBase.RedrawInspector(table);
            return true;
        }

        public void AddRowDefault()
        {
            ITable table = m_TableWindow.GetTable();
            if (table.GetColumnCount() == 0) return;

            RegisterUndo("Add Default Row");
            table.AddRow($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");

            m_TableWindow.GetView().RebuildRowData();
            TableInspectorBase.RedrawInspector(table);
            m_Overlay.SetOverlayStateHidden();
        }

        public void RemoveSelectedRow()
        {
            ITable.RowDescription selectedRow = (ITable.RowDescription)m_TableWindow.GetView().GetMultiColumnListView().selectedItem;
            RegisterUndo($"Remove Row ({selectedRow.Name})");
            m_TableWindow.GetTable().RemoveRow(selectedRow.InternalIndex);
            m_TableWindow.GetView().RebuildRowData();
        }

        public bool RemoveColumn(int internalIndex)
        {
            ITable table = m_TableWindow.GetTable();
            if (table.GetColumnCount() <= 1)
            {
                return false;
            }
            RegisterUndo($"Remove Column ({table.GetColumnName(internalIndex)})");
            table.RemoveColumn(m_TableWindow.GetView().GetColumnType(internalIndex), internalIndex);
            m_TableWindow.BindTable(table);
            return true;
        }
        public bool RemoveRow(int internalIndex)
        {
            ITable table = m_TableWindow.GetTable();
            RegisterUndo($"Remove Row ({table.GetRowName(internalIndex)})");
            table.RemoveRow(internalIndex);
            m_TableWindow.GetView().RebuildRowData();
            return true;
        }

        public bool RenameRow(int internalIndex, string name)
        {
            ITable table = m_TableWindow.GetTable();
            RegisterUndo($"Rename Row ({name})");
            table.SetRowName(name, internalIndex);

            m_TableWindow.GetView().RebuildRowData();
            return true;
        }

        public bool RenameColumn(int internalIndex, string name)
        {
            ITable table = m_TableWindow.GetTable();
            RegisterUndo($"Rename Column ({name})");

            // Update column data in place
            m_TableWindow.GetView().UpdateColumnData(internalIndex, name);

            table.SetColumnName(name, internalIndex);

          //  m_TableWindow.GetView().RebuildRowData();
            return true;
        }

        public void AutoResizeColumns()
        {
            Reflection.InvokeMethod(m_TableWindow.GetView().GetColumnContainer(), "ResizeToFit");
        }

        void RegisterUndo(string name)
        {
            ScriptableObject scriptableObject = m_TableWindow.GetScriptableObject();
            if (scriptableObject != null && m_TableWindow.GetTable().GetFlag(ITable.EnableUndoFlag))
            {
                Undo.RegisterCompleteObjectUndo(scriptableObject, name);
            }
        }
        void SetDirty()
        {
            ScriptableObject scriptableObject = m_TableWindow.GetScriptableObject();
            if (scriptableObject != null)
            {
                EditorUtility.SetDirty(scriptableObject);
            }
        }
    }
#endif
}