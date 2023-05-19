using System;
using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Object = UnityEngine.Object;

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
        ITable.ColumnDescription[] m_ColumnDefinitions;


        VisualElement m_Overlay;

        MultiColumnListView m_TableView;
        Columns m_TableViewColumns;

        ITable m_TargetTable;
        ScriptableObject m_ScriptableObject = null;

        Button m_ToolbarAddColumn;
        Button m_ToolbarAddRow;


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
        public static bool OpenSimpleTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is StableTable table)
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

            tableWindow.BindSimpleTable(table);

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
                BindSimpleTable(m_TargetTable);
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


        void BindSimpleTable(ITable table)
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

            m_ColumnDefinitions = table.GetOrderedColumns();

            // Precache some things
            VisualElement rootElement = rootVisualElement[0];

            // Generate columns for MCLV
            m_TableViewColumns = new Columns();
            int columnCount = table.GetColumnCount();
            Length columnSizePercentage = Length.Percent(1f / columnCount);
            for (int i = 0; i < columnCount; i++)
            {
                ref ITable.ColumnDescription refColumn = ref m_ColumnDefinitions[i];
                Column column = new Column { name = refColumn.Name, title = refColumn.Name, width = columnSizePercentage };
                m_TableViewColumns.Add(column);
            }

            // Create MCLV
            if (m_TableView != null)
            {
                rootElement.Remove(m_TableView);
            }
            m_TableView = new MultiColumnListView(m_TableViewColumns);
            rootElement.Insert(1, m_TableView);
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
            BindSimpleTable(m_TargetTable);
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
            BindSimpleTable(m_TargetTable);
        }
    }
#endif
}