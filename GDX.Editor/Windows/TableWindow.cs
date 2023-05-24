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
        // TODO:
        // - Settings
        // - Confirmation mode
        // - Refactor
        // - Rename columns bug fix
        // - sorting
        // - selection in objects

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
        ToolbarMenu m_ToolbarRowMenu;
        ToolbarMenu m_ToolbarColumnMenu;
        ToolbarButton m_ToolbarSettingsButton;

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
                SetOverlayState(OverlayState.AddColumn);
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
                    BuildHeaderMenu(columnContainer[i], index);
                }
            }


            RebuildRowData();

            // Next frame resize things
            EditorApplication.delayCall += AutoResizeColumns;
        }

        void BuildHeaderMenu(VisualElement element, int index)
        {
            element.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Rename", a => RenameColumn(index));
                evt.menu.AppendAction("Remove", a => RemoveColumn(index));
                evt.menu.AppendSeparator();
            }));
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


        DropdownMenuAction.Status ToolbarMenu_CanAddRow(DropdownMenuAction action)
        {
            return m_TargetTable.GetColumnCount() > 0 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }
        DropdownMenuAction.Status ToolbarMenu_CanRenameRow(DropdownMenuAction action)
        {
            return m_TableView.selectedItem != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }
        void ToolbarMenu_RenameRow()
        {
            ITable.RowDescription selectedItem = (ITable.RowDescription)m_TableView.selectedItem;
            Debug.Log($"Rename {selectedItem.Index}");
            RenameRow(selectedItem.Index);
        }



        void BindWindow()
        {
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);
            //BuildTableWindowContextMenu(rootVisualElement);
            m_Toolbar = rootVisualElement.Q<Toolbar>("gdx-table-toolbar");

            m_ToolbarColumnMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-column");
            m_ToolbarColumnMenu.menu.AppendAction("Add", _ => { AddColumn(); });
            //m_ToolbarColumnMenu.menu.AppendAction("Rename", _ => { ToolbarMenu_RenameRow(); }, ToolbarMenu_CanRenameRow);

            m_ToolbarRowMenu = m_Toolbar.Q<ToolbarMenu>("gdx-table-toolbar-row");
            m_ToolbarRowMenu.menu.AppendAction("Add", _ => { AddRow(); }, ToolbarMenu_CanAddRow);
            m_ToolbarRowMenu.menu.AppendAction("Rename", _ => { ToolbarMenu_RenameRow(); }, ToolbarMenu_CanRenameRow);

            m_ToolbarSettingsButton = m_Toolbar.Q<ToolbarButton>("gdx-table-toolbar-settings");

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
            m_AddColumnCancelButton.clicked += SetOverlayStateHidden;

            // Build out our Adding Rows
            m_AddRowOverlay = m_Overlay.Q<VisualElement>("gdx-table-add-row");
            m_AddRowName = m_AddRowOverlay.Q<TextField>("gdx-table-row-name");
            m_AddRowAddButton = m_AddRowOverlay.Q<Button>("gdx-table-row-add");
            m_AddRowAddButton.clicked += AddRow_AddButtonClicked;
            m_AddRowCancelButton = m_AddRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_AddRowCancelButton.clicked += SetOverlayStateHidden;

            // Bind our Renaming Columns
            m_RenameColumnOverlay = m_Overlay.Q<VisualElement>("gdx-table-rename-column");
            m_RenameColumnName = m_RenameColumnOverlay.Q<TextField>("gdx-table-column-name");
            m_RenameColumnRenameButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-rename");
            m_RenameColumnRenameButton.clicked += RenameColumn_RenameButtonClicked;
            m_RenameColumnCancelButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_RenameColumnCancelButton.clicked += SetOverlayStateHidden;

            // Bind our Renaming Rows
            m_RenameRowOverlay = m_Overlay.Q<VisualElement>("gdx-table-rename-row");
            m_RenameRowName = m_RenameRowOverlay.Q<TextField>("gdx-table-row-name");
            m_RenameRowRenameButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-rename");
            m_RenameRowRenameButton.clicked += RenameRow_RenameButtonClicked;
            m_RenameRowCancelButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_RenameRowCancelButton.clicked += SetOverlayStateHidden;

            // Ensure state of everything
            SetOverlayState(OverlayState.Hide);
        }

        void OnKeyboardEvent(KeyDownEvent evt)
        {
            // Escape to cancel overlay
            if (evt.keyCode == KeyCode.Escape && m_OverlayState != OverlayState.Hide)
            {
                SetOverlayStateHidden();
            }

            // Submit on enter
            if (evt.keyCode == KeyCode.Percent || evt.keyCode == KeyCode.Return)
            {
                switch (m_OverlayState)
                {
                    case OverlayState.AddColumn:
                        AddColumn_AddButtonClicked();
                        break;
                    case OverlayState.AddRow:
                        AddRow_AddButtonClicked();
                        break;
                    case OverlayState.RenameColumn:
                        RenameColumn_RenameButtonClicked();
                        break;
                    case OverlayState.RenameRow:
                        RenameRow_RenameButtonClicked();
                        break;
                }
            }
        }


        public void AddColumn()
        {
            m_AddColumnName.SetValueWithoutNotify($"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
            SetOverlayState(OverlayState.AddColumn);
        }

        public void RemoveSelectedRows()
        {
            foreach (ITable.RowDescription r in m_TableView.selectedItems)
            {
                m_TargetTable.RemoveRow(r.Index);
            }
            RebuildRowData();
        }

        void RegisterUndo(string name)
        {
            if (m_ScriptableObject != null)
            {
                Undo.RegisterCompleteObjectUndo(m_ScriptableObject, name);
            }
        }

        void AddColumn_AddButtonClicked()
        {
            RegisterUndo("Add Column");

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
            SetOverlayState(OverlayState.Hide);

            // Fix inspector not being updated
            TableInspectorBase.RedrawInspector(m_TargetTable);
        }

        void RenameColumn(int column)
        {
            m_RenameColumnName.SetValueWithoutNotify(m_TargetTable.GetColumnName(column));
            m_RenameRowName.userData = column;
            SetOverlayState(OverlayState.RenameColumn);
        }
        void RemoveColumn(int column)
        {
            //TODO: actual
            m_RenameColumnName.SetValueWithoutNotify(m_TargetTable.GetColumnName(column));
            m_RenameRowName.userData = column;
            SetOverlayState(OverlayState.RenameColumn);
        }

        void RenameColumn_RenameButtonClicked()
        {
            RegisterUndo("Rename Column");
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

            SetOverlayState(OverlayState.Hide);
        }

        public void AddRow()
        {
            m_AddRowName.SetValueWithoutNotify($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
            SetOverlayState(OverlayState.AddRow);
        }
        public void AddRowQuick()
        {
            m_AddRowName.SetValueWithoutNotify($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
            AddRow_AddButtonClicked();
        }

        void AddRow_AddButtonClicked()
        {
            if (m_TargetTable.GetColumnCount() == 0) return;

            RegisterUndo("Add Row");
            m_TargetTable.AddRow(m_AddRowName.text);
            //BindTable(m_TargetTable);
            RebuildRowData();
            TableInspectorBase.RedrawInspector(m_TargetTable);
            SetOverlayState(OverlayState.Hide);
        }

        void RenameRow(int row)
        {
            m_RenameRowName.SetValueWithoutNotify(m_TargetTable.GetRowName(row));
            m_RenameRowName.userData = row;
            SetOverlayState(OverlayState.RenameRow);
        }

        void RemoveRow(int row)
        {
            // TODO remove workflow
            m_RenameRowName.SetValueWithoutNotify(m_TargetTable.GetRowName(row));
            m_RenameRowName.userData = row;
            SetOverlayState(OverlayState.RenameRow);
        }

        void RenameRow_RenameButtonClicked()
        {
            RegisterUndo("Rename Row");
            int userData = (int)m_RenameRowName.userData;
            string newName = m_RenameRowName.text;

            m_TargetTable.SetRowName(newName, userData);

            // TODO: optimized populate of data like RenameOfCOlumn

            //BindTable(m_TargetTable);
            RebuildRowData();
            SetOverlayState(OverlayState.Hide);
        }



        void SetOverlayState(OverlayState state)
        {
            // Handle focus
            if (state == OverlayState.Hide)
            {
                m_Toolbar.focusable = true;
                m_ToolbarColumnMenu.focusable = true;
                m_ToolbarRowMenu.focusable = true;
                m_ToolbarSettingsButton.focusable = true;
                m_Overlay.focusable = false;
                if (m_TableView != null)
                {
                    m_TableView.focusable = true;
                }
            }
            else
            {
                m_Toolbar.focusable = false;
                m_ToolbarColumnMenu.focusable = false;
                m_ToolbarRowMenu.focusable = false;
                m_ToolbarSettingsButton.focusable = false;
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
        void SetOverlayStateHidden()
        {
            SetOverlayState(OverlayState.Hide);
        }


        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            ITable.RowDescription description = k_RowDescriptions[row];
            label.text = description.Name;
            label.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Rename", a => RenameRow(description.Index));
                evt.menu.AppendAction("Remove", a => RemoveRow(description.Index));
            }));
        }

        enum OverlayState
        {
            Hide,
            AddColumn,
            AddRow,
            RenameColumn,
            RenameRow,
            Settings,
            Confirmation
        }
    }
#endif
}