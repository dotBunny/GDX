﻿// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Tables;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindowView
    {
        readonly List<ITable.RowDescription> k_RowDescriptions = new List<ITable.RowDescription>();
        ITable.ColumnDescription[] m_ColumnDescriptions;


        TableWindow parentWindow;
        internal MultiColumnListView m_TableView;
        VisualElement m_TableViewHeader;
        Columns m_TableViewColumns;

        public TableWindowView(VisualElement rootElement, TableWindow window)
        {
            parentWindow = window;
            ITable table = window.GetTable();
            int columnCount = table.GetColumnCount();
            m_ColumnDescriptions = table.GetAllColumnDescriptions();

            // Generate columns for MCLV
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };
            int tableTicket = TableWindowProvider.RegisterTable(table);

            Length columnSizePercentage = Length.Percent(100f / columnCount);

            for (int i = 0; i < columnCount; i++)
            {
                ref ITable.ColumnDescription refColumn = ref m_ColumnDescriptions[i];
                int columnIndex = refColumn.Index;

                // We embed the column stable index
                Column column = new Column
                {
                    name = $"Column_{columnIndex}",
                    title = refColumn.Name,
                    width = columnSizePercentage,
                    destroyCell = TableWindowCells.DestroyCell,
                    resizable = true
                };

                // Customize column based on type
                switch (refColumn.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell += () => TableWindowCells.MakeStringCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindStringCell;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell += () => TableWindowCells.MakeCharCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindCharCell;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell += () => TableWindowCells.MakeBoolCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindBoolCell;
                        break;
                    case Serializable.SerializableTypes.SByte:
                        column.makeCell += () => TableWindowCells.MakeSByteCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindSByteCell;
                        break;
                    case Serializable.SerializableTypes.Byte:
                        column.makeCell += () => TableWindowCells.MakeByteCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindByteCell;
                        break;
                    case Serializable.SerializableTypes.Short:
                        column.makeCell += () => TableWindowCells.MakeShortCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindShortCell;
                        break;
                    case Serializable.SerializableTypes.UShort:
                        column.makeCell += () => TableWindowCells.MakeUShortCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindUShortCell;
                        break;
                    case Serializable.SerializableTypes.Int:
                        column.makeCell += () => TableWindowCells.MakeIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindIntCell;
                        break;
                    case Serializable.SerializableTypes.UInt:
                        column.makeCell += () => TableWindowCells.MakeUIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindUIntCell;
                        break;
                    case Serializable.SerializableTypes.Long:
                        column.makeCell += () => TableWindowCells.MakeLongCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindLongCell;
                        break;
                    case Serializable.SerializableTypes.ULong:
                        column.makeCell += () => TableWindowCells.MakeULongCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindULongCell;
                        break;
                    case Serializable.SerializableTypes.Float:
                        column.makeCell += () => TableWindowCells.MakeFloatCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindFloatCell;
                        break;
                    case Serializable.SerializableTypes.Double:
                        column.makeCell += () => TableWindowCells.MakeDoubleCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindDoubleCell;
                        break;
                    case Serializable.SerializableTypes.Vector2:
                        column.makeCell += () => TableWindowCells.MakeVector2Cell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindVector2Cell;
                        break;
                    case Serializable.SerializableTypes.Vector3:
                        column.makeCell += () => TableWindowCells.MakeVector3Cell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindVector3Cell;
                        break;
                    case Serializable.SerializableTypes.Vector4:
                        column.makeCell += () => TableWindowCells.MakeVector4Cell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindVector4Cell;
                        break;
                    case Serializable.SerializableTypes.Vector2Int:
                        column.makeCell += () => TableWindowCells.MakeVector2IntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindVector2IntCell;
                        break;
                    case Serializable.SerializableTypes.Vector3Int:
                        column.makeCell += () => TableWindowCells.MakeVector3IntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindVector3IntCell;
                        break;
                    case Serializable.SerializableTypes.Quaternion:
                        column.makeCell += () => TableWindowCells.MakeQuaternionCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindQuaternionCell;
                        break;
                    case Serializable.SerializableTypes.Rect:
                        column.makeCell += () => TableWindowCells.MakeRectCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindRectCell;
                        break;
                    case Serializable.SerializableTypes.RectInt:
                        column.makeCell += () => TableWindowCells.MakeRectIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindRectIntCell;
                        break;
                    case Serializable.SerializableTypes.Color:
                        column.makeCell += () => TableWindowCells.MakeColorCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindColorCell;
                        break;
                    case Serializable.SerializableTypes.LayerMask:
                        column.makeCell += () => TableWindowCells.MakeLayerMaskCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindLayerMaskCell;
                        break;
                    case Serializable.SerializableTypes.Bounds:
                        column.makeCell += () => TableWindowCells.MakeBoundsCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindBoundsCell;
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        column.makeCell += () => TableWindowCells.MakeBoundsIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindBoundsIntCell;
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        column.makeCell += () => TableWindowCells.MakeHash128Cell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindHash128Cell;
                        break;
                    case Serializable.SerializableTypes.Gradient:
                        column.makeCell += () => TableWindowCells.MakeGradientCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindGradientCell;
                        break;
                    case Serializable.SerializableTypes.AnimationCurve:
                        column.makeCell += () => TableWindowCells.MakeAnimationCurveCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindAnimationCurveCell;
                        break;
                    case Serializable.SerializableTypes.Object:
                        column.makeCell += () => TableWindowCells.MakeObjectCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindObjectCell;
                        break;
                }

                m_TableViewColumns.Add(column);
            }

            // Add row header column
            m_TableViewColumns.Insert(0,
                new Column { makeCell = TableWindowCells.MakeRowHeader, bindCell = BindRowHeader, name = "RowName", title = "Row Name" });

            // Create MCLV
            if (m_TableView != null)
            {
                rootElement.Remove(m_TableView);
            }

            m_TableView = new MultiColumnListView(m_TableViewColumns)
            {
                sortingEnabled = false, // TODO: make this yes when we can move rows?
                name = "gdx-table-view",
                selectionType = SelectionType.Single,
                itemsSource = k_RowDescriptions,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
            };

            //BuildTableWindowContextMenu(m_TableView);
            rootElement.Insert(1, m_TableView);

            // Link other parts of table that we need for functionality
            m_TableViewHeader = m_TableView.Q<VisualElement>(null, "unity-multi-column-header");
            VisualElement columnContainer = m_TableViewHeader[0];
            for (int i = 0; i < columnContainer.childCount; i++)
            {
                string columnName = columnContainer[i].name;
                if (columnName.StartsWith("Column_", StringComparison.OrdinalIgnoreCase))
                {
                    int index = int.Parse(columnName.Split('_', StringSplitOptions.RemoveEmptyEntries)[1]);
                    MakeColumnContextualMenu(columnContainer[i], index);
                }
            }

            RebuildRowData();
        }

        public int GetRowDescriptionIndex(int row)
        {
            return k_RowDescriptions[row].Index;
        }

        public MultiColumnListView GetTableView()
        {
            return m_TableView;
        }

        public VisualElement GetColumnContainer()
        {
            return m_TableViewHeader;
        }

        public void Hide()
        {
            m_TableView.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            m_TableView.style.display = DisplayStyle.Flex;
        }

        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            ITable.RowDescription description = k_RowDescriptions[row];
            label.text = description.Name;

            // Make the context menu effect the entirety of the row, this is a bit brittle as it relies on the actual
            // layout of the UXML document to be the same across versions.
            MakeRowContextMenu(label.parent.parent, description.Index);
        }

        internal void UpdateColumnData(int index, string newName)
        {
            // Figure out index of target
            int columnCount = m_TableViewColumns.Count;
            int foundIndex = -1;
            for (int i = 0; i < columnCount; i++)
            {
                int indexOfSplit = m_TableViewColumns[i].name.IndexOf("_", StringComparison.Ordinal);
                string column = m_TableViewColumns[i].name.Substring(indexOfSplit);
                int columnInteger = int.Parse(column);
                if (columnInteger == index)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                m_ColumnDescriptions[foundIndex].Name = newName;
                m_TableViewColumns[foundIndex].title = newName;
            }
        }


        internal void RebuildRowData()
        {
            ITable table = parentWindow.GetTable();
            k_RowDescriptions.Clear();
            if (table.GetRowCount() > 0)
            {
                k_RowDescriptions.AddRange(table.GetAllRowDescriptions());
            }
            m_TableView.RefreshItems();

            // TODO : Trigger update of each cell? or does it use version to know it needs to update
        }

        void MakeColumnContextualMenu(VisualElement element, int stableColumnID)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Rename",
                    a => parentWindow.GetController().ShowRenameColumnDialog(stableColumnID));
                evt.menu.AppendAction("Remove",
                    a => parentWindow.GetController().ShowRemoveColumnDialog(stableColumnID));
                evt.menu.AppendSeparator();
            }));
        }

        void MakeRowContextMenu(VisualElement element, int stableRowID)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Rename",
                    a => parentWindow.GetController().ShowRenameRowDialog(stableRowID));
                evt.menu.AppendAction("Remove",
                    a => parentWindow.GetController().ShowRemoveRowDialog(stableRowID));
            }));
        }
    }
#endif
}