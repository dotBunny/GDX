// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.DataTables;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public class DataTableWindowView
    {

        static StyleLength m_StyleLength25 = new StyleLength(new Length(25, LengthUnit.Pixel));
        static StyleLength m_StyleLength275 = new StyleLength(new Length(275, LengthUnit.Pixel));
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

            // Generate columns for MCLV
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };

            int columnCount = m_ColumnDescriptions.Count;
            Length columnSizePercentage = Length.Percent(100f / columnCount);


            // Create our "Row Name" column
            m_TableViewColumns.Insert(0,
                new Column
                {
                    makeCell = DataTableWindowCells.MakeRowHeader,
                    bindCell = BindRowHeader,
                    name = "RowName",
                    title = "Row Name"
                });

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
                    width = columnSizePercentage,
                    resizable = true,
                    unbindCell = UnbindCell,
                    destroyCell = DestroyCell
                };

                // Customize column based on type
                switch (columnDescription.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell += () => DataTableWindowCells.MakeStringCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindStringCell;
                        column.sortable = true;
                        column.minWidth = m_GenericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell += () => DataTableWindowCells.MakeCharCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindCharCell;
                        column.minWidth = m_ToggleMinWidth;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell += () => DataTableWindowCells.MakeBoolCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoolCell;
                        column.sortable = true;
                        column.minWidth = m_ToggleMinWidth;
                        break;
                    case Serializable.SerializableTypes.SByte:
                        column.makeCell += () => DataTableWindowCells.MakeSByteCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindSByteCell;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Byte:
                        column.makeCell += () => DataTableWindowCells.MakeByteCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindByteCell;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Short:
                        column.makeCell += () => DataTableWindowCells.MakeShortCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindShortCell;
                        column.minWidth = m_NumericMinWidth;
                        column.sortable = true;
                        break;
                    case Serializable.SerializableTypes.UShort:
                        column.makeCell += () => DataTableWindowCells.MakeUShortCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindUShortCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Int:
                        column.makeCell += () => DataTableWindowCells.MakeIntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindIntCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.UInt:
                        column.makeCell += () => DataTableWindowCells.MakeUIntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindUIntCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Long:
                        column.makeCell += () => DataTableWindowCells.MakeLongCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindLongCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.ULong:
                        column.makeCell += () => DataTableWindowCells.MakeULongCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindULongCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Float:
                        column.makeCell += () => DataTableWindowCells.MakeFloatCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindFloatCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Double:
                        column.makeCell += () => DataTableWindowCells.MakeDoubleCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindDoubleCell;
                        column.sortable = true;
                        column.minWidth = m_NumericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Vector2:
                        column.makeCell += () => DataTableWindowCells.MakeVector2Cell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector2Cell;
                        column.minWidth = m_Vector2MinWidth;
                        break;
                    case Serializable.SerializableTypes.Vector3:
                        column.makeCell += () => DataTableWindowCells.MakeVector3Cell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector3Cell;
                        column.minWidth = m_Vector3MinWidth;
                        break;
                    case Serializable.SerializableTypes.Vector4:
                        column.makeCell += () => DataTableWindowCells.MakeVector4Cell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector4Cell;
                        column.minWidth = m_Vector4MinWidth;
                        break;
                    case Serializable.SerializableTypes.Vector2Int:
                        column.makeCell += () => DataTableWindowCells.MakeVector2IntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector2IntCell;
                        column.minWidth = m_Vector2MinWidth;
                        break;
                    case Serializable.SerializableTypes.Vector3Int:
                        column.makeCell += () => DataTableWindowCells.MakeVector3IntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindVector3IntCell;
                        column.minWidth = m_Vector3MinWidth;
                        break;
                    case Serializable.SerializableTypes.Quaternion:
                        column.makeCell += () => DataTableWindowCells.MakeQuaternionCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindQuaternionCell;
                        column.minWidth = m_Vector4MinWidth;
                        break;
                    case Serializable.SerializableTypes.Rect:
                        column.makeCell += () => DataTableWindowCells.MakeRectCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindRectCell;
                        column.minWidth = m_Vector2MinWidth;
                        break;
                    case Serializable.SerializableTypes.RectInt:
                        column.makeCell += () => DataTableWindowCells.MakeRectIntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindRectIntCell;
                        column.minWidth = m_Vector2MinWidth;
                        break;
                    case Serializable.SerializableTypes.Color:
                        column.makeCell += () => DataTableWindowCells.MakeColorCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindColorCell;
                        column.minWidth = m_GenericMinWidth;
                        break;
                    case Serializable.SerializableTypes.LayerMask:
                        column.makeCell += () => DataTableWindowCells.MakeLayerMaskCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindLayerMaskCell;
                        column.minWidth = m_GenericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Bounds:
                        column.makeCell += () => DataTableWindowCells.MakeBoundsCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoundsCell;
                        column.minWidth = m_BoundsMinWidth;
                        SetDesiredRowHeightMultiplier(DataTableWindowCells.k_DoubleHeight);
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        column.makeCell += () => DataTableWindowCells.MakeBoundsIntCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindBoundsIntCell;
                        column.minWidth = m_BoundsMinWidth;
                        SetDesiredRowHeightMultiplier(DataTableWindowCells.k_DoubleHeight);
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        column.makeCell += () => DataTableWindowCells.MakeHash128Cell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindHash128Cell;
                        column.sortable = false;
                        column.minWidth = m_HashMinWidth;
                        break;
                    case Serializable.SerializableTypes.Gradient:
                        column.makeCell += () => DataTableWindowCells.MakeGradientCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindGradientCell;
                        column.minWidth = m_GenericMinWidth;
                        break;
                    case Serializable.SerializableTypes.AnimationCurve:
                        column.makeCell += () => DataTableWindowCells.MakeAnimationCurveCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindAnimationCurveCell;
                        column.minWidth = m_GenericMinWidth;
                        break;
                    case Serializable.SerializableTypes.Object:
                        column.makeCell += () => DataTableWindowCells.MakeObjectCell(tableTicket, columnIndex);
                        column.bindCell = DataTableWindowCells.BindObjectCell;
                        column.minWidth = m_GenericMinWidth;
                        break;
                }

                m_TableViewColumns.Add(column);
            }

            // Create MCLV
            m_MultiColumnListView = new MultiColumnListView(m_TableViewColumns)
            {
                sortingEnabled = false,
                name = "gdx-table-view",
                selectionType = SelectionType.Single,
                itemsSource = m_RowDescriptions,
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                fixedItemHeight = m_DesiredRowHeight
            };
            m_MultiColumnListView.style.height = new StyleLength(new Length(100f, LengthUnit.Percent));
            m_MultiColumnListView.headerContextMenuPopulateEvent += AppendColumnContextMenu;
            //m_MultiColumnListView.columnSortingChanged += SortItems;

            rootElement.Insert(1, m_MultiColumnListView);

            // Link other parts of table that we need for reflection functionality
            m_TableViewHeader = m_MultiColumnListView.Q<VisualElement>(null, "unity-multi-column-header");

            RebuildRowData();
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

        public void SetDesiredRowHeightMultiplier(float neededHeight)
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

        // void SortItems()
        // {
        //     foreach (SortColumnDescription sortedColumn in m_MultiColumnListView.sortedColumns)
        //     {
        //         Debug.Log($"{sortedColumn.columnName}  {sortedColumn.direction.ToString()}");
        //     }
        // }

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
            DataTableBase dataTable = m_DataTableWindow.GetDataTable();
            m_RowDescriptions.Clear();
            if (dataTable.GetRowCount() > 0)
            {
                m_RowDescriptions.AddRange(dataTable.GetAllRowDescriptions());
            }
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
                    a => m_DataTableWindow.GetController().ShowRenameColumnDialog(columnIdentifier));
                evt.menu.AppendAction("Remove",
                    a => m_DataTableWindow.GetController().ShowRemoveColumnDialog(columnIdentifier), CanRemoveColumn);
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
            return m_DataTableWindow.GetDataTable().GetColumnCount() > 1
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Disabled;
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}