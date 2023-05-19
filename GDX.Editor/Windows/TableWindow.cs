using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;
using Toggle = UnityEngine.UIElements.Toggle;

namespace GDX.Editor.Windows
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindow : EditorWindow
    {
        static readonly Dictionary<ITable, TableWindow> k_Windows =
            new Dictionary<ITable, TableWindow>();

        Button m_AddColumnAddButton;
        Button m_AddColumnCancelButton;
        TextField m_AddColumnName;

        VisualElement m_AddColumnOverlay;
        PopupField<int> m_AddColumnType;
        ITable.ColumnDescription[] m_ColumnDescriptions;


        VisualElement m_Overlay;

        MultiColumnListView m_TableView;
        Columns m_TableViewColumns;

        ITable m_TargetTable;
        ScriptableObject m_ScriptableObject = null;

        Button m_ToolbarAddColumn;
        Button m_ToolbarAddRow;


        readonly List<ITable.RowDescription> k_RowDescriptions = new List<ITable.RowDescription>();

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

                // We embed the column stable index
                Column column = new Column { name = $"Column_{refColumn.Index.ToString()}", title = refColumn.Name, width = columnSizePercentage };
                // Customize column based on type
                switch (refColumn.Type)
                {
                    case Serializable.SerializableTypes.String:
                        column.makeCell = MakeStringCell;
                        column.bindCell = BindStringCell;
                        break;
                    case Serializable.SerializableTypes.Char:
                        column.makeCell = MakeCharCell;
                        break;
                    case Serializable.SerializableTypes.Bool:
                        column.makeCell = MakeBoolCell;
                        break;
                    case Serializable.SerializableTypes.SByte:
                        column.makeCell = MakeSByteCell;
                        break;
                    case Serializable.SerializableTypes.Byte:
                        column.makeCell = MakeByteCell;
                        break;
                    case Serializable.SerializableTypes.Short:
                        break;
                    case Serializable.SerializableTypes.UShort:
                        break;
                    case Serializable.SerializableTypes.Int:
                        break;
                    case Serializable.SerializableTypes.UInt:
                        break;
                    case Serializable.SerializableTypes.Long:
                        break;
                    case Serializable.SerializableTypes.ULong:
                        break;
                    case Serializable.SerializableTypes.Float:
                        break;
                    case Serializable.SerializableTypes.Double:
                        break;
                    case Serializable.SerializableTypes.Vector2:
                        break;
                    case Serializable.SerializableTypes.Vector3:
                        break;
                    case Serializable.SerializableTypes.Vector4:
                        break;
                    case Serializable.SerializableTypes.Vector2Int:
                        break;
                    case Serializable.SerializableTypes.Vector3Int:
                        break;
                    case Serializable.SerializableTypes.Quaternion:
                        break;
                    case Serializable.SerializableTypes.Rect:
                        break;
                    case Serializable.SerializableTypes.RectInt:
                        break;
                    case Serializable.SerializableTypes.Color:
                        break;
                    case Serializable.SerializableTypes.LayerMask:
                        break;
                    case Serializable.SerializableTypes.Bounds:
                        break;
                    case Serializable.SerializableTypes.BoundsInt:
                        break;
                    case Serializable.SerializableTypes.Hash128:
                        break;
                    case Serializable.SerializableTypes.Gradient:
                        break;
                    case Serializable.SerializableTypes.AnimationCurve:
                        break;
                    case Serializable.SerializableTypes.Object:
                        break;
                }
                m_TableViewColumns.Add(column);
            }



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
                itemsSource = k_RowDescriptions
            };
            rootElement.Insert(1, m_TableView);
            RebuildRowData();
        }

        void RebuildRowData()
        {
            k_RowDescriptions.Clear();
            k_RowDescriptions.AddRange(m_TargetTable.GetAllRowDescriptions());
            // TODO : Trigger update of each cell? or does it use version to know it needs to update
        }

        void BindWindow()
        {
            m_ToolbarAddColumn = rootVisualElement.Q<Button>("gdx-table-toolbar-add-column");
            m_ToolbarAddColumn.clicked += AddColumn_Show;

            m_ToolbarAddRow = rootVisualElement.Q<Button>("gdx-table-toolbar-add-row");
            m_ToolbarAddRow.clicked += AddRow_Clicked;

            m_Overlay = rootVisualElement.Q<VisualElement>("gdx-table-overlay");
            // Ensure that the overlay is not visible
            m_Overlay.visible = false;

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
            m_AddColumnType = new PopupField<int>(typeValues, 0, Serializable.GetSerializableTypesLabel, Serializable.GetSerializableTypesLabel)
            {
                label = "Type",
                name = "gdx-table-column-type"
            };
            m_AddColumnOverlay.Insert(columnNameIndex + 1, m_AddColumnType);

            m_AddColumnAddButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-add");
            m_AddColumnAddButton.clicked += AddColumn_AddButtonClicked;
            m_AddColumnCancelButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_AddColumnCancelButton.clicked += AddColumn_CancelButtonClicked;
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
            AddColumn_Hide();
        }

        void AddColumn_Show()
        {
            m_Overlay.visible = true;
            m_AddColumnOverlay.visible = true;
        }

        void AddColumn_Hide()
        {
            m_Overlay.visible = false;
            m_AddColumnOverlay.visible = false;
        }

        void AddColumn_CancelButtonClicked()
        {
            AddColumn_Hide();
        }

        void AddRow_Clicked()
        {
            m_TargetTable.AddRow();
            //BindTable(m_TargetTable);
            RebuildRowData();
        }

        const string k_CellFieldName = "gdx-data-field";


        VisualElement MakeStringCell()
        {
            return new TextField(null) { name = k_CellFieldName };;
        }
        VisualElement MakeCharCell()
        {
            return new TextField(null, 1, false, false, ' ') { name = k_CellFieldName };
        }
        VisualElement MakeBoolCell()
        {
            return new Toggle(null) { name = k_CellFieldName };
        }
        VisualElement MakeSByteCell()
        {
            return new SliderInt(-128, 127) { name = k_CellFieldName, showInputField = true };
        }
        VisualElement MakeByteCell()
        {
            return new SliderInt(0, 255) { name = k_CellFieldName, showInputField = true };
        }

        void BindStringCell(VisualElement cell, int row)
        {

        }




                    //
                    // case Serializable.SerializableTypes.Short:
                    //     break;
                    // case Serializable.SerializableTypes.UShort:
                    //     break;
                    // case Serializable.SerializableTypes.Int:
                    //     break;
                    // case Serializable.SerializableTypes.UInt:
                    //     break;
                    // case Serializable.SerializableTypes.Long:
                    //     break;
                    // case Serializable.SerializableTypes.ULong:
                    //     break;
                    // case Serializable.SerializableTypes.Float:
                    //     break;
                    // case Serializable.SerializableTypes.Double:
                    //     break;
                    // case Serializable.SerializableTypes.Vector2:
                    //     break;
                    // case Serializable.SerializableTypes.Vector3:
                    //     break;
                    // case Serializable.SerializableTypes.Vector4:
                    //     break;
                    // case Serializable.SerializableTypes.Vector2Int:
                    //     break;
                    // case Serializable.SerializableTypes.Vector3Int:
                    //     break;
                    // case Serializable.SerializableTypes.Quaternion:
                    //     break;
                    // case Serializable.SerializableTypes.Rect:
                    //     break;
                    // case Serializable.SerializableTypes.RectInt:
                    //     break;
                    // case Serializable.SerializableTypes.Color:
                    //     break;
                    // case Serializable.SerializableTypes.LayerMask:
                    //     break;
                    // case Serializable.SerializableTypes.Bounds:
                    //     break;
                    // case Serializable.SerializableTypes.BoundsInt:
                    //     break;
                    // case Serializable.SerializableTypes.Hash128:
                    //     break;
                    // case Serializable.SerializableTypes.Gradient:
                    //     break;
                    // case Serializable.SerializableTypes.AnimationCurve:
                    //     break;
                    // case Serializable.SerializableTypes.Object:
                    //     break;

    }
#endif
}