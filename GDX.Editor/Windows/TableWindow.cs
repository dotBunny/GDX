using System;
using System.Collections.Generic;
using GDX.Editor.Inspectors;
using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindow : EditorWindow
    {
        const string k_RowHeaderFieldName = "gdx-data-row-header";

        readonly List<ITable.RowDescription> k_RowDescriptions = new List<ITable.RowDescription>();
        Button m_AddColumnAddButton;
        Button m_AddColumnCancelButton;
        TextField m_AddColumnName;


        VisualElement m_AddColumnOverlay;
        PopupField<int> m_AddColumnType;
        Button m_AddRowAddButton;
        Button m_AddRowCancelButton;
        TextField m_AddRowName;

        VisualElement m_AddRowOverlay;


        ITable.ColumnDescription[] m_ColumnDescriptions;


        VisualElement m_Overlay;

        OverlayState m_OverlayState;
        Button m_RenameColumnCancelButton;
        TextField m_RenameColumnName;

        VisualElement m_RenameColumnOverlay;
        Button m_RenameColumnRenameButton;
        Button m_RenameRowCancelButton;
        TextField m_RenameRowName;

        VisualElement m_RenameRowOverlay;
        Button m_RenameRowRenameButton;
        ScriptableObject m_ScriptableObject;

        MultiColumnListView m_TableView;
        VisualElement m_TableViewHeader;
        Columns m_TableViewColumns;

        ITable m_TargetTable;

        Toolbar m_Toolbar;
        Button m_ToolbarAddColumn;
        Button m_ToolbarAddRow;

        void OnEnable()
        {
            ResourcesProvider.SetupStylesheets(rootVisualElement);
            ResourcesProvider.GetVisualTreeAsset("GDXTableWindow").CloneTree(rootVisualElement);
            ResourcesProvider.CheckTheme(rootVisualElement);

            BindWindow();

            // Catch domain reload and rebind/relink window
            if (m_TargetTable != null)
            {
                TableWindowProvider.RegisterTableWindow(this, m_TargetTable);
                BindTable(m_TargetTable);
            }

            EditorApplication.delayCall += CheckForNoTable;
        }

        public void OnDestroy()
        {
            if (m_ScriptableObject != null)
            {
                AssetDatabase.SaveAssetIfDirty(m_ScriptableObject);
            } // TODO: do we need to dirty this if its not a SO


            TableWindowProvider.UnregisterTableWindow(this);

            if (m_TargetTable != null)
            {
                TableWindowProvider.UnregisterTable(m_TargetTable);
            }
        }

        public int GetRowDescriptionIndex(int row)
        {
            return k_RowDescriptions[row].Index;
        }


        void CheckForNoTable()
        {
            if (m_TargetTable == null)
            {
                Close();
            }
        }

        void UpdateColumnDescriptions()
        {
            // TODO: get
            // update existing columns?
        }

        public void BindTable(ITable table)
        {
            m_TargetTable = table;
            int tableTicket = TableWindowProvider.RegisterTable(m_TargetTable);
            if (m_TargetTable is ScriptableObject targetTable)
            {
                m_ScriptableObject = targetTable;
                titleContent = new GUIContent(m_ScriptableObject.name);
            }
            else
            {
                titleContent = new GUIContent("Table"); // TODO?? Name tables?
            }

            int columnCount = table.GetColumnCount();
            if (columnCount == 0)
            {
                if (m_TableView != null)
                {
                    m_TableView.style.display = DisplayStyle.None;
                }
                SetOverlay(OverlayState.AddColumn);
                return;
            }

            m_ColumnDescriptions = table.GetAllColumnDescriptions();

            // Precache some things
            VisualElement rootElement = rootVisualElement[0];

            // Generate columns for MCLV
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };


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
                new Column { makeCell = MakeRowHeader, bindCell = BindRowHeader });

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
            BuildTableWindowContextMenu(m_TableView);
            rootElement.Insert(1, m_TableView);

            // Link other parts of table that we need for functionality
            m_TableViewHeader = m_TableView.Q<VisualElement>(null, "unity-multi-column-header");

            RebuildRowData();

            // Next frame resize things
            EditorApplication.delayCall += AutoResizeColumns;
        }

        void RebuildRowData()
        {
            k_RowDescriptions.Clear();
            if (m_TargetTable.GetRowCount() > 0)
            {
                k_RowDescriptions.AddRange(m_TargetTable.GetAllRowDescriptions());
            }

            m_TableView.RefreshItems();

            // TODO : Trigger update of each cell? or does it use version to know it needs to update
        }

        void AutoResizeColumns()
        {
            Reflection.InvokeMethod(m_TableViewHeader, "ResizeToFit");
        }


        void BindWindow()
        {
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);
            BuildTableWindowContextMenu(rootVisualElement);
            m_Toolbar = rootVisualElement.Q<Toolbar>("gdx-table-toolbar");
            m_ToolbarAddColumn = m_Toolbar.Q<Button>("gdx-table-toolbar-add-column");
            m_ToolbarAddColumn.clicked += AddColumn;

            m_ToolbarAddRow = m_Toolbar.Q<Button>("gdx-table-toolbar-add-row");
            m_ToolbarAddRow.clicked += AddRow;

            m_Overlay = rootVisualElement.Q<VisualElement>("gdx-table-overlay");

            // Build out references for adding a column
            m_AddColumnOverlay = m_Overlay.Q<VisualElement>("gdx-table-add-column");
            m_AddColumnName = m_AddColumnOverlay.Q<TextField>("gdx-table-column-name");
            // Build our custom column type enum
            int columnNameIndex = m_AddColumnOverlay.IndexOf(m_AddColumnName);
            List<int> typeValues = new List<int>(Serializable.SerializableTypesCount);
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                typeValues.Add(i);
            }

            m_AddColumnType =
                new PopupField<int>(typeValues, 0, Serializable.GetSerializableTypesLabel,
                    Serializable.GetSerializableTypesLabel) { label = "Type", name = "gdx-table-column-type" };
            m_AddColumnOverlay.Insert(columnNameIndex + 1, m_AddColumnType);
            m_AddColumnAddButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-add");
            m_AddColumnAddButton.clicked += AddColumn_AddButtonClicked;
            m_AddColumnCancelButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_AddColumnCancelButton.clicked += CancelOverlay;

            // Build out our Adding Rows
            m_AddRowOverlay = m_Overlay.Q<VisualElement>("gdx-table-add-row");
            m_AddRowName = m_AddRowOverlay.Q<TextField>("gdx-table-row-name");
            m_AddRowAddButton = m_AddRowOverlay.Q<Button>("gdx-table-row-add");
            m_AddRowAddButton.clicked += AddRow_AddButtonClicked;
            m_AddRowCancelButton = m_AddRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_AddRowCancelButton.clicked += CancelOverlay;

            // Bind our Renaming Columns
            m_RenameColumnOverlay = m_Overlay.Q<VisualElement>("gdx-table-rename-column");
            m_RenameColumnName = m_RenameColumnOverlay.Q<TextField>("gdx-table-column-name");
            m_RenameColumnRenameButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-rename");
            m_RenameColumnRenameButton.clicked += RenameColumn_RenameButtonClicked;
            m_RenameColumnCancelButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_RenameColumnCancelButton.clicked += CancelOverlay;

            // Bind our Renaming Rows
            m_RenameRowOverlay = m_Overlay.Q<VisualElement>("gdx-table-rename-row");
            m_RenameRowName = m_RenameRowOverlay.Q<TextField>("gdx-table-row-name");
            m_RenameRowRenameButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-rename");
            m_RenameRowRenameButton.clicked += RenameRow_RenameButtonClicked;
            m_RenameRowCancelButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_RenameRowCancelButton.clicked += CancelOverlay;

            // Ensure state of everything
            SetOverlay(OverlayState.Hide);
        }

        void OnKeyboardEvent(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Escape && m_OverlayState != OverlayState.Hide)
            {
                CancelOverlay();
            }
        }


        void AddColumn()
        {
            m_AddColumnName.SetValueWithoutNotify($"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
            SetOverlay(OverlayState.AddColumn);
        }

        void AddColumn_AddButtonClicked()
        {
            Serializable.SerializableTypes selectedType = (Serializable.SerializableTypes)m_AddColumnType.value;
            m_TargetTable.AddColumn(selectedType, m_AddColumnName.value);

            // Do we want this , are we going to handle save?
            if (m_ScriptableObject != null)
            {
                EditorUtility.SetDirty(m_ScriptableObject);
            }
            // TODO: need to flag parent of tables not scriptable?


            // TODO: REbnd data?
            BindTable(m_TargetTable);
            SetOverlay(OverlayState.Hide);
            TableInspectorBase.RedrawInspector(m_TargetTable);
        }

        void RenameColumn(int column)
        {
            m_RenameColumnName.SetValueWithoutNotify(m_TargetTable.GetColumnName(column));
            m_RenameRowName.userData = column;
            SetOverlay(OverlayState.RenameColumn);
        }

        void RenameColumn_RenameButtonClicked()
        {
            int userData = (int)m_RenameColumnName.userData;
            string newName = m_RenameColumnName.text;
            m_TargetTable.SetColumnName(newName, userData);

            // Figure out index of target
            int columnCount = m_TableViewColumns.Count;
            int foundIndex = -1;
            for (int i = 0; i < columnCount; i++)
            {
                int indexOfSplit = m_TableViewColumns[i].name.IndexOf("_", StringComparison.Ordinal);
                string column = m_TableViewColumns[i].name.Substring(indexOfSplit);
                int columnInteger = int.Parse(column);
                if (columnInteger == userData)
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

            SetOverlay(OverlayState.Hide);
        }

        void AddRow()
        {
            m_AddRowName.SetValueWithoutNotify($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
            SetOverlay(OverlayState.AddRow);
        }

        void AddRow_AddButtonClicked()
        {
            if (m_TargetTable.GetColumnCount() == 0) return;

            m_TargetTable.AddRow(m_AddRowName.text);
            //BindTable(m_TargetTable);
            RebuildRowData();
            TableInspectorBase.RedrawInspector(m_TargetTable);
            SetOverlay(OverlayState.Hide);
        }

        void RenameRow(int row)
        {
            m_RenameRowName.SetValueWithoutNotify(m_TargetTable.GetRowName(row));
            m_RenameRowName.userData = row;
            SetOverlay(OverlayState.RenameRow);
        }

        void RenameRow_RenameButtonClicked()
        {
            int userData = (int)m_RenameRowName.userData;
            string newName = m_RenameRowName.text;

            m_TargetTable.SetRowName(newName, userData);

            // TODO: optimized populate of data like RenameOfCOlumn

            //BindTable(m_TargetTable);
            RebuildRowData();
            SetOverlay(OverlayState.Hide);
        }

        void CancelOverlay()
        {
            SetOverlay(OverlayState.Hide);
        }

        void SetOverlay(OverlayState state)
        {
            // Handle focus
            if (state == OverlayState.Hide)
            {
                m_Toolbar.focusable = true;
                m_ToolbarAddColumn.focusable = true;
                m_ToolbarAddRow.focusable = true;
                m_Overlay.focusable = false;
                if (m_TableView != null)
                {
                    m_TableView.focusable = true;
                }
            }
            else
            {
                m_Toolbar.focusable = false;
                m_ToolbarAddColumn.focusable = false;
                m_ToolbarAddRow.focusable = false;
                if (m_TableView != null)
                {
                    m_TableView.focusable = false;
                }
                m_Overlay.focusable = true;
            }

            switch (state)
            {
                case OverlayState.AddColumn:
                    m_Overlay.style.display = DisplayStyle.Flex;
                    m_AddColumnOverlay.style.display = DisplayStyle.Flex;
                    m_AddColumnAddButton.Focus();
                    break;
                case OverlayState.AddRow:
                    m_Overlay.style.display = DisplayStyle.Flex;
                    m_AddRowOverlay.style.display = DisplayStyle.Flex;
                    m_AddRowAddButton.Focus();
                    break;
                case OverlayState.RenameColumn:
                    m_Overlay.style.display = DisplayStyle.Flex;
                    m_RenameColumnOverlay.style.display = DisplayStyle.Flex;
                    m_RenameColumnName.Focus();
                    break;
                case OverlayState.RenameRow:
                    m_Overlay.style.display = DisplayStyle.Flex;
                    m_RenameRowOverlay.style.display = DisplayStyle.Flex;
                    m_RenameRowName.Focus();
                    break;
                default:
                    m_Overlay.style.display = DisplayStyle.None;
                    m_AddColumnOverlay.style.display = DisplayStyle.None;
                    m_AddRowOverlay.style.display = DisplayStyle.None;
                    m_RenameColumnOverlay.style.display = DisplayStyle.None;
                    m_RenameRowOverlay.style.display = DisplayStyle.None;
                    if (m_TableView != null)
                    {
                        m_TableView.Focus();
                    }

                    break;
            }

            m_OverlayState = state;
        }


        static VisualElement MakeRowHeader()
        {
            return new Label { name = k_RowHeaderFieldName };
        }

        void BuildTableWindowContextMenu(VisualElement element)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Add Column", a => AddColumn());
                if (m_TargetTable.GetColumnCount() > 0)
                {
                    evt.menu.AppendAction("Add Row", a => AddRow());
                }
                else
                {
                    evt.menu.AppendAction("Add Row", a => AddRow(), DropdownMenuAction.Status.Disabled);
                }
            }));
        }

        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            ITable.RowDescription description = k_RowDescriptions[row];
            label.text = description.Name;
            label.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Rename", a => RenameRow(description.Index));
            }));
        }

        enum OverlayState
        {
            Hide,
            AddColumn,
            AddRow,
            RenameColumn,
            RenameRow
        }
    }
#endif
}