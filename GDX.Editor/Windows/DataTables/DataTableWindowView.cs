// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.DataTables;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public class DataTableWindowView
    {
        readonly List<RowDescription> m_RowDescriptions = new List<RowDescription>();
        readonly Length m_BoundsMinWidth = new Length(200, LengthUnit.Pixel);
        readonly List<ColumnDescription> m_ColumnDescriptions = new List<ColumnDescription>();

        readonly Length m_GenericMinWidth = new Length(75, LengthUnit.Pixel);
        readonly Length m_HashMinWidth = new Length(260, LengthUnit.Pixel);
        readonly MultiColumnListView m_MultiColumnListView;
        readonly Length m_NumericMinWidth = new Length(50, LengthUnit.Pixel);

        readonly Columns m_TableViewColumns;
        readonly VisualElement m_TableViewHeader;
        readonly Length m_ToggleMinWidth = new Length(25, LengthUnit.Pixel);
        readonly Length m_Vector2MinWidth = new Length(100, LengthUnit.Pixel);
        readonly Length m_Vector3MinWidth = new Length(150, LengthUnit.Pixel);
        readonly Length m_Vector4MinWidth = new Length(200, LengthUnit.Pixel);

        readonly DataTableWindow m_DataTableWindow;

        readonly StringKeyDictionary<int> m_ColumnIdentifierCache;
        readonly StringKeyDictionary<Serializable.SerializableTypes> m_ColumnTypeCache;

        float m_DesiredRowHeight = 27f;

        ~DataTableWindowView()
        {
            Destroy();
        }

        public void Destroy()
        {
            m_RowDescriptions.Clear();
            m_ColumnDescriptions.Clear();
        }

        public DataTableWindowView(VisualElement rootElement, DataTableWindow window)
        {
            m_DataTableWindow = window;

            // Remove previous
            if (rootElement.childCount == 2)
            {
                rootElement.RemoveAt(1);
            }

            DataTableBase dataTable = window.GetDataTable();
            int tableTicket = window.GetDataTableTicket();

            // Add row header column ahead of actual columns
            m_ColumnDescriptions.Add(new ColumnDescription
            {
                Name = "RowName", Identifier = -1, Type = Serializable.SerializableTypes.String
            });
            m_ColumnDescriptions.AddRange(dataTable.GetAllColumnDescriptions());

            // Generate columns for MultiColumnListView
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };

            int columnCount = m_ColumnDescriptions.Count;
            Length columnSizePercentage = Length.Percent(100f / columnCount);
            m_ColumnIdentifierCache = new StringKeyDictionary<int>(columnCount);
            m_ColumnTypeCache = new StringKeyDictionary<Serializable.SerializableTypes>(columnCount);

            // Create our "Row Name" column
            m_TableViewColumns.Insert(0,
                new Column
                {
                    makeCell = DataTableWindowCells.MakeRowHeader,
                    bindCell = BindRowHeader,
                    name = "RowName",
                    title = "Row Name",
                    sortable = true,
                    optional = false
                });
            m_ColumnIdentifierCache.AddUnchecked(m_TableViewColumns[0].name, -1);
            m_ColumnTypeCache.AddUnchecked(m_TableViewColumns[0].name, Serializable.SerializableTypes.String);

            // Don't allow for drag and drop of columns (? cant seem to get an event here)
            m_TableViewColumns[0].collection.reorderable = false;

            // Creat our other columns
            for (int i = 1; i < columnCount; i++)
            {
                ColumnDescription columnDescription = m_ColumnDescriptions[i];
                int columnIndex = columnDescription.Identifier;

                // We embed the column stable index
                Column column = new Column
                {
                    name = $"Column_{columnIndex}",
                    title = columnDescription.Name,
                    width = columnSizePercentage, // baseline
                    resizable = true,
                    unbindCell = UnbindCell,
                    destroyCell = DestroyCell
                };
                m_ColumnIdentifierCache.AddUnchecked(column.name, columnDescription.Identifier);
                m_ColumnTypeCache.AddUnchecked(column.name, columnDescription.Type);

                // Customize column based on type
                switch (columnDescription.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell += () => DataTableWindowCells.MakeStringCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindStringCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.EnumInt:
                        column.makeCell += () => DataTableWindowCells.MakeEnumIntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindEnumIntCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell += () => DataTableWindowCells.MakeCharCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindCharCell;
                        column.minWidth = m_ToggleMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell += () => DataTableWindowCells.MakeBoolCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoolCell;
                        column.minWidth = m_ToggleMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.SByte:
                        column.makeCell += () => DataTableWindowCells.MakeSByteCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindSByteCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Byte:
                        column.makeCell += () => DataTableWindowCells.MakeByteCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindByteCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Short:
                        column.makeCell += () => DataTableWindowCells.MakeShortCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindShortCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.UShort:
                        column.makeCell += () => DataTableWindowCells.MakeUShortCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindUShortCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Int:
                        column.makeCell += () => DataTableWindowCells.MakeIntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindIntCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.UInt:
                        column.makeCell += () => DataTableWindowCells.MakeUIntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindUIntCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Long:
                        column.makeCell += () => DataTableWindowCells.MakeLongCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindLongCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.ULong:
                        column.makeCell += () => DataTableWindowCells.MakeULongCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindULongCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Float:
                        column.makeCell += () => DataTableWindowCells.MakeFloatCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindFloatCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Double:
                        column.makeCell += () => DataTableWindowCells.MakeDoubleCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindDoubleCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.Vector2:
                        column.makeCell += () => DataTableWindowCells.MakeVector2Cell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector2Cell;
                        column.minWidth = m_Vector2MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Vector3:
                        column.makeCell += () => DataTableWindowCells.MakeVector3Cell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector3Cell;
                        column.minWidth = m_Vector3MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Vector4:
                        column.makeCell += () => DataTableWindowCells.MakeVector4Cell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector4Cell;
                        column.minWidth = m_Vector4MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Vector2Int:
                        column.makeCell += () => DataTableWindowCells.MakeVector2IntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector2IntCell;
                        column.minWidth = m_Vector2MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Vector3Int:
                        column.makeCell += () => DataTableWindowCells.MakeVector3IntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector3IntCell;
                        column.minWidth = m_Vector3MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Quaternion:
                        column.makeCell += () => DataTableWindowCells.MakeQuaternionCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindQuaternionCell;
                        column.minWidth = m_Vector4MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Rect:
                        column.makeCell += () => DataTableWindowCells.MakeRectCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindRectCell;
                        column.minWidth = m_Vector2MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.RectInt:
                        column.makeCell += () => DataTableWindowCells.MakeRectIntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindRectIntCell;
                        column.minWidth = m_Vector2MinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Color:
                        column.makeCell += () => DataTableWindowCells.MakeColorCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindColorCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.LayerMask:
                        column.makeCell += () => DataTableWindowCells.MakeLayerMaskCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindLayerMaskCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Bounds:
                        column.makeCell += () => DataTableWindowCells.MakeBoundsCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoundsCell;
                        column.minWidth = m_BoundsMinWidth;
                        SetDesiredRowHeightMultiplier(DataTableWindowCells.k_DoubleHeight);
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        column.makeCell += () => DataTableWindowCells.MakeBoundsIntCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoundsIntCell;
                        column.minWidth = m_BoundsMinWidth;
                        SetDesiredRowHeightMultiplier(DataTableWindowCells.k_DoubleHeight);
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        column.makeCell += () => DataTableWindowCells.MakeHash128Cell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindHash128Cell;
                        column.minWidth = m_HashMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Gradient:
                        column.makeCell += () => DataTableWindowCells.MakeGradientCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindGradientCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.AnimationCurve:
                        column.makeCell += () => DataTableWindowCells.MakeAnimationCurveCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindAnimationCurveCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = false;
                        break;
                    case Serializable.SerializableTypes.Object:
                        column.makeCell += () => DataTableWindowCells.MakeObjectCell(this, tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindObjectCell;
                        column.minWidth = m_GenericMinWidth;
                        column.sortable = false;
                        break;
                }

                m_TableViewColumns.Add(column);
            }

            // Create MultiColumnListView
            m_MultiColumnListView = new MultiColumnListView(m_TableViewColumns)
            {
                sortingEnabled = true,
                name = "gdx-table-view",
                selectionType = SelectionType.Single,
                itemsSource = m_RowDescriptions,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                fixedItemHeight = m_DesiredRowHeight,
                style = { height = new StyleLength(new Length(100f, LengthUnit.Percent)) },
                reorderable =  true,
                reorderMode = ListViewReorderMode.Simple
            };
            m_MultiColumnListView.headerContextMenuPopulateEvent += AppendColumnContextMenu;
            m_MultiColumnListView.columnSortingChanged += SortItems;
            m_MultiColumnListView.itemIndexChanged += OnRowsReordered;

            rootElement.Insert(1, m_MultiColumnListView);

            // Link other parts of table that we need for reflection functionality
            m_TableViewHeader = m_MultiColumnListView.Q<VisualElement>(null, "unity-multi-column-header");

            RebuildRowData();
        }


        public RowDescription GetRowDescriptionBySortedOrder(int row)
        {
            return m_RowDescriptions[row];
        }
        void UnbindCell(VisualElement cell, int row)
        {
            DataTableWindowCells.CellData data = (DataTableWindowCells.CellData)cell.userData;
            data.CellValue = null;
        }

        void DestroyCell(VisualElement cell)
        {
            cell.userData = null;
        }

        void SetDesiredRowHeightMultiplier(float neededHeight)
        {
            if (neededHeight > m_DesiredRowHeight)
            {
                m_DesiredRowHeight = neededHeight;
            }
        }

        public void RefreshItems()
        {
            m_MultiColumnListView.RefreshItems();
        }

        public void CommitOrder()
        {
            DataTableTracker.RecordSettingsUndo(m_DataTableWindow.GetDataTableTicket());

            int sortedOrderCount = m_RowDescriptions.Count;
            int[] sortedIdentifiers = new int [sortedOrderCount];
            for (int i = 0; i < sortedOrderCount; i++)
            {
                sortedIdentifiers[i] = m_RowDescriptions[i].Identifier;
            }

            m_DataTableWindow.GetDataTable()
                .SetAllRowOrders(sortedIdentifiers);


            DataTableTracker.NotifyOfSettingsChange(m_DataTableWindow.GetDataTableTicket(), m_DataTableWindow);

            RebuildRowData();
        }

        int m_SortedColumnCount;
        bool m_HasMadeManualRowOrderChanges = false;


        public bool HasSortedColumns()
        {
            return m_SortedColumnCount > 0;
        }

        public bool HasManualRows()
        {
            return m_HasMadeManualRowOrderChanges;
        }
        void SortItems()
        {
            List<int> sortedColumnIdentifiers = new List<int>(m_ColumnDescriptions.Count);
            List<SortDirection> sortedColumnDirections = new List<SortDirection>(m_ColumnDescriptions.Count);
            List<Serializable.SerializableTypes> sortedColumnTypes =
                new List<Serializable.SerializableTypes>(m_ColumnDescriptions.Count);

            m_SortedColumnCount = 0;
            foreach (SortColumnDescription sortedColumn in m_MultiColumnListView.sortedColumns)
            {
                sortedColumnIdentifiers.Add(m_ColumnIdentifierCache[sortedColumn.columnName]);
                sortedColumnDirections.Add(sortedColumn.direction);
                sortedColumnTypes.Add(m_ColumnTypeCache[sortedColumn.columnName]);
                m_SortedColumnCount++;
            }

            // Remove old descriptions
            m_RowDescriptions.Clear();

            // Replace with sorted descriptions
            m_RowDescriptions.AddRange(
                m_DataTableWindow.GetDataTable()
                    .GetAllRowDescriptionsSortedByColumns(sortedColumnIdentifiers.ToArray(), sortedColumnTypes.ToArray(),
                        sortedColumnDirections.ToArray()));

            m_MultiColumnListView.RefreshItems();
        }
        void OnRowsReordered(int oldIndex, int newIndex)
        {
            m_HasMadeManualRowOrderChanges = true;
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

        public int GetSelectedRowIdentifier()
        {
            if (m_MultiColumnListView.selectedItem == null)
            {
                return -1;
            }

            RowDescription selectedItem = (RowDescription)m_MultiColumnListView.selectedItem;
            return selectedItem.Identifier;
        }

        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            RowDescription description = m_RowDescriptions[row];
            label.text = description.Name;
        }

        internal Serializable.SerializableTypes GetColumnType(int columnIdentifier)
        {
            int columnCount = m_TableViewColumns.Count;
            // Skip first column as its the row header
            for (int i = 1; i < columnCount; i++)
            {
                int indexOfSplit = m_TableViewColumns[i].name.IndexOf("_", StringComparison.Ordinal);
                string column = m_TableViewColumns[i].name.Substring(indexOfSplit + 1);
                int columnInteger = int.Parse(column);
                if (columnInteger == columnIdentifier)
                {
                    return m_ColumnDescriptions[i].Type;
                }
            }

            return Serializable.SerializableTypes.Invalid;
        }

        internal void UpdateColumnData(int columnIdentifier, string newName)
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
                if (columnInteger == columnIdentifier)
                {
                    foundIndex = i;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                m_ColumnDescriptions[foundIndex] = new ColumnDescription
                {
                    Name = newName,
                    Identifier = m_ColumnDescriptions[foundIndex].Identifier,
                    Type = m_ColumnDescriptions[foundIndex].Type
                };
                m_TableViewColumns[foundIndex].title = newName;
            }
        }

        internal void RebuildRowData()
        {
            m_HasMadeManualRowOrderChanges = false;

            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            m_RowDescriptions.Clear();
            if (dataTable.GetRowCount() > 0)
            {
                m_RowDescriptions.AddRange(dataTable.GetAllRowDescriptions());
            }

            m_MultiColumnListView.ClearSelection();
            m_MultiColumnListView.sortColumnDescriptions.Clear();

            m_MultiColumnListView.Rebuild();
        }

        void AppendColumnContextMenu(ContextualMenuPopulateEvent evt, Column column)
        {
            if (column == null || column.name == null)
            {
                return;
            }
            int indexOfSplit = column.name.IndexOf("_", StringComparison.Ordinal);
            if (indexOfSplit != -1)
            {
                string columnInteger = column.name.Substring(indexOfSplit + 1);
                int columnIdentifier = int.Parse(columnInteger);
                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Rename",
                    _ => m_DataTableWindow.GetController().ShowRenameColumnDialog(columnIdentifier));
                evt.menu.AppendAction("Remove",
                    _ => m_DataTableWindow.GetController().ShowRemoveColumnDialog(columnIdentifier), CanRemoveColumn);
                evt.menu.AppendAction("Move Left",
                    _ => m_DataTableWindow.GetController().MoveColumnLeft(columnIdentifier),
                    _ => CanMoveColumnLeft(columnIdentifier));
                evt.menu.AppendAction("Move Right",
                    _ => m_DataTableWindow.GetController().MoveColumnRight(columnIdentifier),
                    _ => CanMoveColumnRight(columnIdentifier));

                evt.menu.AppendSeparator();

                evt.menu.AppendAction("Reset Order",
                    _ => { m_DataTableWindow.GetView().RebuildRowData(); },
                    m_DataTableWindow.GetToolbar().CanCommitOrder);

            }
        }

        DropdownMenuAction.Status CanMoveColumnLeft(int columnIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            if (dataTable.GetColumnCount() <= 1)
            {
                return DropdownMenuAction.Status.Disabled;
            }
            int currentOrder = dataTable.GetColumnOrder(columnIdentifier);
            return currentOrder > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanMoveColumnRight(int columnIdentifier)
        {
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            int columnCount = dataTable.GetColumnCount();
            if (columnCount <= 1)
            {
                return DropdownMenuAction.Status.Disabled;
            }
            int currentOrder = dataTable.GetColumnOrder(columnIdentifier);
            return currentOrder < (columnCount - 1) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }

        DropdownMenuAction.Status CanRemoveColumn(DropdownMenuAction action)
        {
            return m_DataTableWindow.GetDataTable().GetColumnCount() > 1
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}