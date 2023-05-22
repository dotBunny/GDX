using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Tables;
using GDX.Tables.CellValues;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindow : EditorWindow
    {
        const string k_CellFieldName = "gdx-data-field";
        const string k_RowHeaderFieldName = "gdx-data-row-header";

        static readonly Dictionary<ITable, TableWindow> k_Windows =
            new Dictionary<ITable, TableWindow>();

        static readonly Dictionary<VisualElement, int> k_CellToColumnIDMap =
            new SerializableDictionary<VisualElement, int>();


        readonly List<ITable.RowDescription> k_RowDescriptions = new List<ITable.RowDescription>();


        VisualElement m_AddColumnOverlay;
        Button m_AddColumnAddButton;
        Button m_AddColumnCancelButton;
        TextField m_AddColumnName;
        PopupField<int> m_AddColumnType;

        VisualElement m_AddRowOverlay;
        Button m_AddRowAddButton;
        Button m_AddRowCancelButton;
        TextField m_AddRowName;

        VisualElement m_RenameRowOverlay;
        Button m_RenameRowRenameButton;
        Button m_RenameRowCancelButton;
        TextField m_RenameRowName;

        VisualElement m_RenameColumnOverlay;
        Button m_RenameColumnRenameButton;
        Button m_RenameColumnCancelButton;
        TextField m_RenameColumnName;




        ITable.ColumnDescription[] m_ColumnDescriptions;


        VisualElement m_Overlay;
        ScriptableObject m_ScriptableObject;

        MultiColumnListView m_TableView;
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
                k_Windows[m_TargetTable] = this;
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

            if (m_TargetTable != null)
            {
                if (k_Windows.ContainsKey(m_TargetTable))
                {
                    k_Windows.Remove(m_TargetTable);
                }
            }
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAssetTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is ITable table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static TableWindow OpenAsset(ITable table)
        {
            TableWindow tableWindow;
            if (k_Windows.TryGetValue(table, out TableWindow window))
            {
                tableWindow = window;
            }
            else
            {
                tableWindow = CreateWindow<TableWindow>();
                k_Windows.Add(table, tableWindow);
            }

            tableWindow.BindTable(table);

            tableWindow.Show();
            tableWindow.Focus();

            return tableWindow;
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

        void BindTable(ITable table)
        {
            m_TargetTable = table;
            if (m_TargetTable is ScriptableObject targetTable)
            {
                m_ScriptableObject = targetTable;
                titleContent = new GUIContent(m_ScriptableObject.name);
            }
            else
            {
                titleContent = new GUIContent("Table"); // TODO?? Name tables?
            }

            m_ColumnDescriptions = table.GetAllColumnDescriptions();

            // Precache some things
            VisualElement rootElement = rootVisualElement[0];

            // Generate columns for MCLV
            m_TableViewColumns = new Columns { reorderable = true, resizable = true };

            int columnCount = table.GetColumnCount();
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
                    destroyCell = DestroyCell,
                };

                // Customize column based on type
                switch (refColumn.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell += () => MakeStringCell(columnIndex);
                        column.bindCell = BindStringCell;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell += () => MakeCharCell(columnIndex);
                        column.bindCell = BindCharCell;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell += () => MakeBoolCell(columnIndex);
                        column.bindCell = BindBoolCell;
                        break;
                    case Serializable.SerializableTypes.SByte:
                        column.makeCell += () => MakeSByteCell(columnIndex);
                        column.bindCell = BindSByteCell;
                        break;
                    case Serializable.SerializableTypes.Byte:
                        column.makeCell += () => MakeByteCell(columnIndex);
                        column.bindCell = BindByteCell;
                        break;
                    case Serializable.SerializableTypes.Short:
                        column.makeCell += () => MakeShortCell(columnIndex);
                        column.bindCell = BindShortCell;
                        break;
                    case Serializable.SerializableTypes.UShort:
                        column.makeCell += () => MakeUShortCell(columnIndex);
                        column.bindCell = BindUShortCell;
                        break;
                    case Serializable.SerializableTypes.Int:
                        column.makeCell += () => MakeIntCell(columnIndex);
                        column.bindCell = BindIntCell;
                        break;
                    case Serializable.SerializableTypes.UInt:
                        column.makeCell += () => MakeUIntCell(columnIndex);
                        column.bindCell = BindUIntCell;
                        break;
                    case Serializable.SerializableTypes.Long:
                        column.makeCell += () => MakeLongCell(columnIndex);
                        column.bindCell = BindLongCell;
                        break;
                    case Serializable.SerializableTypes.ULong:
                        column.makeCell += () => MakeULongCell(columnIndex);
                        column.bindCell = BindULongCell;
                        break;
                    case Serializable.SerializableTypes.Float:
                        column.makeCell += () => MakeFloatCell(columnIndex);
                        column.bindCell = BindFloatCell;
                        break;
                    case Serializable.SerializableTypes.Double:
                        column.makeCell += () => MakeDoubleCell(columnIndex);
                        column.bindCell = BindDoubleCell;
                        break;
                    case Serializable.SerializableTypes.Vector2:
                        column.makeCell += () => MakeVector2Cell(columnIndex);
                        column.bindCell = BindVector2Cell;
                        break;
                    case Serializable.SerializableTypes.Vector3:
                        column.makeCell += () => MakeVector3Cell(columnIndex);
                        column.bindCell = BindVector3Cell;
                        break;
                    case Serializable.SerializableTypes.Vector4:
                        column.makeCell += () => MakeVector4Cell(columnIndex);
                        column.bindCell = BindVector4Cell;
                        break;
                    case Serializable.SerializableTypes.Vector2Int:
                        column.makeCell += () => MakeVector2IntCell(columnIndex);
                        column.bindCell = BindVector2IntCell;
                        break;
                    case Serializable.SerializableTypes.Vector3Int:
                        column.makeCell += () => MakeVector3IntCell(columnIndex);
                        column.bindCell = BindVector3IntCell;
                        break;
                    case Serializable.SerializableTypes.Quaternion:
                        column.makeCell += () => MakeQuaternionCell(columnIndex);
                        column.bindCell = BindQuaternionCell;
                        break;
                    case Serializable.SerializableTypes.Rect:
                        column.makeCell += () => MakeRectCell(columnIndex);
                        column.bindCell = BindRectCell;
                        break;
                    case Serializable.SerializableTypes.RectInt:
                        column.makeCell += () => MakeRectIntCell(columnIndex);
                        column.bindCell = BindRectIntCell;
                        break;
                    case Serializable.SerializableTypes.Color:
                        column.makeCell += () => MakeColorCell(columnIndex);
                        column.bindCell = BindColorCell;
                        break;
                    case Serializable.SerializableTypes.LayerMask:
                        column.makeCell += () => MakeLayerMaskCell(columnIndex);
                        column.bindCell = BindLayerMaskCell;
                        break;
                    case Serializable.SerializableTypes.Bounds:
                        column.makeCell += () => MakeBoundsCell(columnIndex);
                        column.bindCell = BindBoundsCell;
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        column.makeCell += () => MakeBoundsIntCell(columnIndex);
                        column.bindCell = BindBoundsIntCell;
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        column.makeCell += () => MakeHash128Cell(columnIndex);
                        column.bindCell = BindHash128Cell;
                        break;
                    case Serializable.SerializableTypes.Gradient:
                        column.makeCell += () => MakeGradientCell(columnIndex);
                        column.bindCell = BindGradientCell;
                        break;
                    case Serializable.SerializableTypes.AnimationCurve:
                        column.makeCell += () => MakeAnimationCurveCell(columnIndex);
                        column.bindCell = BindAnimationCurveCell;
                        break;
                    case Serializable.SerializableTypes.Object:
                        column.makeCell += () => MakeObjectCell(columnIndex);
                        column.bindCell = BindObjectCell;
                        break;
                }

                m_TableViewColumns.Add(column);
            }

            // Add row header column
            m_TableViewColumns.Insert(0,
                new Column
                {
                    makeCell = MakeRowHeader,
                    bindCell = BindRowHeader,
                });

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
                showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly
            };
            rootElement.Insert(1, m_TableView);
            RebuildRowData();
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



        void BindWindow()
        {
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);
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
            m_TargetTable.AddRow(m_AddRowName.text);
            //BindTable(m_TargetTable);
            RebuildRowData();
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

        enum OverlayState
        {
            Hide,
            AddColumn,
            AddRow,
            RenameColumn,
            RenameRow
        }

        OverlayState m_OverlayState;
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
                m_TableView.focusable = false;
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
            return new Label() { name = k_RowHeaderFieldName};
        }

        void BindRowHeader(VisualElement cell, int row)
        {
            Label label = (Label)cell;
            ITable.RowDescription description = k_RowDescriptions[row];
            label.text = description.Name;
            label.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Rename", a => RenameRow(description.Index));
            }));;
        }

        void BindStringCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            StringCellValue cellValue =
                new StringCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);

            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                StringCellValue local = (StringCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindCharCell(VisualElement cell, int row)
        {
            TextField field = (TextField)cell;
            CharCellValue cellValue =
                new CharCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe().ToString());
            field.RegisterValueChangedCallback(e =>
            {
                CharCellValue local = (CharCellValue)field.userData;
                local.Set(e.newValue[0]);
            });
        }

        void BindBoolCell(VisualElement cell, int row)
        {
            Toggle field = (Toggle)cell;
            BoolCellValue cellValue =
                new BoolCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoolCellValue local = (BoolCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindSByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            SByteCellValue cellValue =
                new SByteCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                SByteCellValue local = (SByteCellValue)field.userData;
                local.Set((sbyte)e.newValue);
            });
        }

        void BindByteCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            ByteCellValue cellValue =
                new ByteCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ByteCellValue local = (ByteCellValue)field.userData;
                local.Set((byte)e.newValue);
            });
        }

        void BindShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            ShortCellValue cellValue =
                new ShortCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ShortCellValue local = (ShortCellValue)field.userData;
                local.Set((short)e.newValue);
            });
        }

        void BindUShortCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            UShortCellValue cellValue =
                new UShortCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                UShortCellValue local = (UShortCellValue)field.userData;
                local.Set((ushort)e.newValue);
            });
        }

        void BindIntCell(VisualElement cell, int row)
        {
            SliderInt field = (SliderInt)cell;
            IntCellValue cellValue =
                new IntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                IntCellValue local = (IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindUIntCell(VisualElement cell, int row)
        {
            IntegerField field = (IntegerField)cell;
            UIntCellValue cellValue =
                new UIntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify((int)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                UIntCellValue local = (UIntCellValue)field.userData;
                local.Set((uint)e.newValue);
            });
        }

        void BindLongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            LongCellValue cellValue =
                new LongCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                LongCellValue local = (LongCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindULongCell(VisualElement cell, int row)
        {
            LongField field = (LongField)cell;
            ULongCellValue cellValue =
                new ULongCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify((long)cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ULongCellValue local = (ULongCellValue)field.userData;
                local.Set((ulong)e.newValue);
            });
        }

        void BindFloatCell(VisualElement cell, int row)
        {
            FloatField field = (FloatField)cell;
            FloatCellValue cellValue =
                new FloatCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                FloatCellValue local = (FloatCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindDoubleCell(VisualElement cell, int row)
        {
            DoubleField field = (DoubleField)cell;
            DoubleCellValue cellValue =
                new DoubleCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                DoubleCellValue local = (DoubleCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindVector2Cell(VisualElement cell, int row)
        {
            Vector2Field field = (Vector2Field)cell;
            Vector2CellValue cellValue =
                new Vector2CellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector2CellValue local = (Vector2CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindVector3Cell(VisualElement cell, int row)
        {
            Vector3Field field = (Vector3Field)cell;
            Vector3CellValue cellValue =
                new Vector3CellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector3CellValue local = (Vector3CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindVector4Cell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            Vector4CellValue cellValue =
                new Vector4CellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector4CellValue local = (Vector4CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindVector2IntCell(VisualElement cell, int row)
        {
            Vector2IntField field = (Vector2IntField)cell;
            Vector2IntCellValue cellValue =
                new Vector2IntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector2IntCellValue local = (Vector2IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindVector3IntCell(VisualElement cell, int row)
        {
            Vector3IntField field = (Vector3IntField)cell;
            Vector3IntCellValue cellValue =
                new Vector3IntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Vector3IntCellValue local = (Vector3IntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindQuaternionCell(VisualElement cell, int row)
        {
            Vector4Field field = (Vector4Field)cell;
            QuaternionCellValue cellValue =
                new QuaternionCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;

            // Figure it out
            Quaternion localQuaternion = cellValue.GetUnsafe();
            Vector4 fieldValue =
                new Vector4(localQuaternion.x, localQuaternion.y, localQuaternion.z, localQuaternion.w);

            field.SetValueWithoutNotify(fieldValue);
            field.RegisterValueChangedCallback(e =>
            {
                QuaternionCellValue local = (QuaternionCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindRectCell(VisualElement cell, int row)
        {
            RectField field = (RectField)cell;
            RectCellValue cellValue =
                new RectCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                RectCellValue local = (RectCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindRectIntCell(VisualElement cell, int row)
        {
            RectIntField field = (RectIntField)cell;
            RectIntCellValue cellValue =
                new RectIntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                RectIntCellValue local = (RectIntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindColorCell(VisualElement cell, int row)
        {
            ColorField field = (ColorField)cell;
            ColorCellValue cellValue =
                new ColorCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ColorCellValue local = (ColorCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindLayerMaskCell(VisualElement cell, int row)
        {
            LayerMaskField field = (LayerMaskField)cell;
            LayerMaskCellValue cellValue =
                new LayerMaskCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                LayerMaskCellValue local = (LayerMaskCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindBoundsCell(VisualElement cell, int row)
        {
            BoundsField field = (BoundsField)cell;
            BoundsCellValue cellValue =
                new BoundsCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoundsCellValue local = (BoundsCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindBoundsIntCell(VisualElement cell, int row)
        {
            BoundsIntField field = (BoundsIntField)cell;
            BoundsIntCellValue cellValue =
                new BoundsIntCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                BoundsIntCellValue local = (BoundsIntCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindHash128Cell(VisualElement cell, int row)
        {
            Hash128Field field = (Hash128Field)cell;
            Hash128CellValue cellValue =
                new Hash128CellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                Hash128CellValue local = (Hash128CellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindGradientCell(VisualElement cell, int row)
        {
            GradientField field = (GradientField)cell;
            GradientCellValue cellValue =
                new GradientCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                GradientCellValue local = (GradientCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindAnimationCurveCell(VisualElement cell, int row)
        {
            CurveField field = (CurveField)cell;
            AnimationCurveCellValue cellValue =
                new AnimationCurveCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                AnimationCurveCellValue local = (AnimationCurveCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        void BindObjectCell(VisualElement cell, int row)
        {
            ObjectField field = (ObjectField)cell;
            ObjectCellValue cellValue =
                new ObjectCellValue(m_TargetTable, k_RowDescriptions[row].Index, k_CellToColumnIDMap[cell]);
            field.userData = cellValue;
            field.SetValueWithoutNotify(cellValue.GetUnsafe());
            field.RegisterValueChangedCallback(e =>
            {
                ObjectCellValue local = (ObjectCellValue)field.userData;
                local.Set(e.newValue);
            });
        }

        static void DestroyCell(VisualElement cell)
        {
            k_CellToColumnIDMap.Remove(cell);
        }

        static VisualElement MakeStringCell(int column)
        {
            TextField newField = new TextField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeCharCell(int column)
        {
            TextField newField =  new TextField(null, 1, false, false, ' ') { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeBoolCell(int column)
        {
            Toggle newField =  new Toggle(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeSByteCell(int column)
        {
            SliderInt newField = new SliderInt(sbyte.MinValue, sbyte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeByteCell(int column)
        {
            SliderInt newField =new SliderInt(byte.MinValue, byte.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeShortCell(int column)
        {
            SliderInt newField = new SliderInt(short.MinValue, short.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeUShortCell(int column)
        {
            SliderInt newField = new SliderInt(ushort.MinValue, ushort.MaxValue)
            {
                name = k_CellFieldName, showInputField = true, label = null
            };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeIntCell(int column)
        {
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeUIntCell(int column)
        {
            //TODO: Crunches
            IntegerField newField = new IntegerField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeLongCell(int column)
        {
            LongField newField = new LongField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeULongCell(int column)
        {
            //TODO: Crunches
            LongField newField = new LongField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeFloatCell(int column)
        {
            FloatField newField = new FloatField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeDoubleCell(int column)
        {
            DoubleField newField = new DoubleField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeVector2Cell(int column)
        {
            Vector2Field newField = new Vector2Field(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeVector3Cell(int column)
        {
            Vector3Field newField = new Vector3Field(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeVector4Cell(int column)
        {
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeVector2IntCell(int column)
        {
            Vector2IntField newField = new Vector2IntField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeVector3IntCell(int column)
        {
            Vector3IntField newField = new Vector3IntField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeQuaternionCell(int column)
        {
            Vector4Field newField = new Vector4Field(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeRectCell(int column)
        {
            RectField newField = new RectField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeRectIntCell(int column)
        {
            RectIntField newField = new RectIntField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeColorCell(int column)
        {
            ColorField newField = new ColorField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeLayerMaskCell(int column)
        {
            LayerMaskField newField = new LayerMaskField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeBoundsCell(int column)
        {
            BoundsField newField = new BoundsField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeBoundsIntCell(int column)
        {
            BoundsIntField newField = new BoundsIntField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeHash128Cell(int column)
        {
            Hash128Field newField = new Hash128Field(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeGradientCell(int column)
        {
            GradientField newField = new GradientField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeAnimationCurveCell(int column)
        {
            CurveField newField = new CurveField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }

        static VisualElement MakeObjectCell(int column)
        {
            ObjectField newField = new ObjectField(null) { name = k_CellFieldName };
            k_CellToColumnIDMap.Add(newField, column);
            return newField;
        }
    }
#endif
}