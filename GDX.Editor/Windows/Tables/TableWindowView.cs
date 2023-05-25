// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Tables;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindowView
    {
        readonly List<ITable.RowDescription> k_RowDescriptions = new List<ITable.RowDescription>();
        readonly List<ITable.ColumnDescription> m_ColumnDescriptions = new List<ITable.ColumnDescription>();
        readonly MultiColumnListView m_MultiColumnListView;
        readonly Columns m_TableViewColumns;
        readonly VisualElement m_TableViewHeader;


        readonly TableWindow parentWindow;

        float m_DesiredRowHeight = 22f;

        public void SetDesiredRowHeightMultiplier(float neededHeight)
        {
            if (neededHeight > m_DesiredRowHeight)
            {
                m_DesiredRowHeight = neededHeight;
            }
        }

        public TableWindowView(VisualElement rootElement, TableWindow window)
        {
            parentWindow = window;

            // Remove previous
            if (rootElement.childCount == 2)
            {
                rootElement.RemoveAt(1);
            }

            ITable table = window.GetTable();
            int tableTicket = TableWindowProvider.RegisterTable(table);


            // Add row header column ahead of actual columns
            m_ColumnDescriptions.Add(new ITable.ColumnDescription
            {
                Name = "RowName", InternalIndex = -1, Type = Serializable.SerializableTypes.String
            });
            m_ColumnDescriptions.AddRange(table.GetAllColumnDescriptions());

            // Generate columns for MCLV
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };

            int columnCount = m_ColumnDescriptions.Count;
            Length columnSizePercentage = Length.Percent(100f / columnCount);

            m_TableViewColumns.Insert(0,
                new Column
                {
                    makeCell = TableWindowCells.MakeRowHeader,
                    bindCell = BindRowHeader,
                    name = "RowName",
                    title = "Row Name"
                });
            for (int i = 1; i < columnCount; i++)
            {
                ITable.ColumnDescription columnDescription = m_ColumnDescriptions[i];
                int columnIndex = columnDescription.InternalIndex;

                // We embed the column stable index
                Column column = new Column
                {
                    name = $"Column_{columnIndex}",
                    title = columnDescription.Name,
                    width = columnSizePercentage,
                    destroyCell = TableWindowCells.DestroyCell,
                    resizable = true
                };

                // Customize column based on type
                switch (columnDescription.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell += () => TableWindowCells.MakeStringCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindStringCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell += () => TableWindowCells.MakeCharCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindCharCell;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell += () => TableWindowCells.MakeBoolCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindBoolCell;
                        column.sortable = true;
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
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.UShort:
                        column.makeCell += () => TableWindowCells.MakeUShortCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindUShortCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Int:
                        column.makeCell += () => TableWindowCells.MakeIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindIntCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.UInt:
                        column.makeCell += () => TableWindowCells.MakeUIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindUIntCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Long:
                        column.makeCell += () => TableWindowCells.MakeLongCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindLongCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.ULong:
                        column.makeCell += () => TableWindowCells.MakeULongCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindULongCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Float:
                        column.makeCell += () => TableWindowCells.MakeFloatCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindFloatCell;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Double:
                        column.makeCell += () => TableWindowCells.MakeDoubleCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindDoubleCell;
                        column.sortable = true;
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
                        SetDesiredRowHeightMultiplier(TableWindowCells.k_DoubleHeight);
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        column.makeCell += () => TableWindowCells.MakeBoundsIntCell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindBoundsIntCell;
                        SetDesiredRowHeightMultiplier(TableWindowCells.k_DoubleHeight);
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        column.makeCell += () => TableWindowCells.MakeHash128Cell(tableTicket, columnIndex);
                        column.bindCell = TableWindowCells.BindHash128Cell;
                        column.sortable = false;
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

            // Create MCLV
            m_MultiColumnListView = new MultiColumnListView(m_TableViewColumns)
            {
                sortingEnabled = true,
                name = "gdx-table-view",
                selectionType = SelectionType.Single,
                itemsSource = k_RowDescriptions,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                fixedItemHeight = m_DesiredRowHeight
            };
            m_MultiColumnListView.style.height = new StyleLength(new Length(100f, LengthUnit.Percent));
            m_MultiColumnListView.headerContextMenuPopulateEvent += AppendColumnContextMenu;
            m_MultiColumnListView.columnSortingChanged += SortItems;

            rootElement.Insert(1, m_MultiColumnListView);

            // Link other parts of table that we need for reflection functionality
            m_TableViewHeader = m_MultiColumnListView.Q<VisualElement>(null, "unity-multi-column-header");

            RebuildRowData();
        }

        void SortItems()
        {
            foreach (SortColumnDescription sortedColumn in m_MultiColumnListView.sortedColumns)
            {
                Debug.Log($"{sortedColumn.columnName}  {sortedColumn.direction.ToString()}");
            }
        }

        public int GetRowDescriptionIndex(int row)
        {
            return k_RowDescriptions[row].InternalIndex;
        }

        public MultiColumnListView GetMultiColumnListView()
        {
            return m_MultiColumnListView;
        }

        public VisualElement GetColumnContainer()
        {
            return m_TableViewHeader;
        }

        public void Hide()
        {
            m_MultiColumnListView.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            m_MultiColumnListView.style.display = DisplayStyle.Flex;
        }

        public int GetSelectedRowInternalIndex()
        {
            if (m_MultiColumnListView.selectedItem == null)
            {
                return -1;
            }

            ITable.RowDescription selectedItem = (ITable.RowDescription)m_MultiColumnListView.selectedItem;
            return selectedItem.InternalIndex;
        }

        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            ITable.RowDescription description = k_RowDescriptions[row];
            label.text = description.Name;

            // Make the context menu effect the entirety of the row, this is a bit brittle as it relies on the actual
            // layout of the UXML document to be the same across versions.
            //MakeRowContextMenu(label.parent.parent, description.InternalIndex);
        }

        internal Serializable.SerializableTypes GetColumnType(int internalIndex)
        {
            int columnCount = m_TableViewColumns.Count;
            // Skip first column as its the row header
            for (int i = 1; i < columnCount; i++)
            {
                int indexOfSplit = m_TableViewColumns[i].name.IndexOf("_", StringComparison.Ordinal);
                string column = m_TableViewColumns[i].name.Substring(indexOfSplit + 1);
                int columnInteger = int.Parse(column);
                if (columnInteger == internalIndex)
                {
                    return m_ColumnDescriptions[i].Type;
                }
            }

            return Serializable.SerializableTypes.Invalid;
        }

        internal void UpdateColumnData(int internalIndex, string newName)
        {
            // Figure out index of target
            int columnCount = m_TableViewColumns.Count;
            int foundIndex = -1;

            // Skip first column as its the row header
            for (int i = 1; i < columnCount; i++)
            {
                int indexOfSplit = m_TableViewColumns[i].name.IndexOf("_", StringComparison.Ordinal);
                string column = m_TableViewColumns[i].name.Substring(indexOfSplit + 1);
                int columnInteger = int.Parse(column);
                if (columnInteger == internalIndex)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                m_ColumnDescriptions[foundIndex] = new ITable.ColumnDescription()
                {
                    Name = newName,
                    InternalIndex = m_ColumnDescriptions[foundIndex].InternalIndex,
                    Type = m_ColumnDescriptions[foundIndex].Type
                };
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

            m_MultiColumnListView.RefreshItems();

            // TODO : Trigger update of each cell? or does it use version to know it needs to update
        }

        void AppendColumnContextMenu(ContextualMenuPopulateEvent evt, Column column)
        {
            if (column == null || column.name == null) return;

            int indexOfSplit = column.name.IndexOf("_", StringComparison.Ordinal);
            if (indexOfSplit != -1)
            {
                string columnInteger = column.name.Substring(indexOfSplit + 1);
                int internalIndex = int.Parse(columnInteger);
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Rename",
                    a => parentWindow.GetController().ShowRenameColumnDialog(internalIndex));
                evt.menu.AppendAction("Remove",
                    a => parentWindow.GetController().ShowRemoveColumnDialog(internalIndex), CanRemoveColumn);
            }
        }

        // void MakeRowContextMenu(VisualElement element, int stableRowID)
        // {
        //     element.AddManipulator(new ContextualMenuManipulator(evt =>
        //     {
        //         evt.menu.AppendSeparator();
        //         evt.menu.AppendAction("Rename",
        //             a => parentWindow.GetController().ShowRenameRowDialog(stableRowID));
        //         evt.menu.AppendAction("Remove",
        //             a => parentWindow.GetController().ShowRemoveRowDialog(stableRowID));
        //     }));
        // }

        DropdownMenuAction.Status CanRemoveColumn(DropdownMenuAction action)
        {
            return parentWindow.GetTable().GetColumnCount() > 1
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }
    }
#endif
}