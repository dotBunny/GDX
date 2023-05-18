using System;
using System.Collections.Generic;
using GDX.Data;
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
    public class SimpleTableWindow : EditorWindow
    {
        static readonly Dictionary<SimpleTable, SimpleTableWindow> k_Windows =
            new Dictionary<SimpleTable, SimpleTableWindow>();

        Button m_AddColumnAddButton;
        Button m_AddColumnCancelButton;
        TextField m_AddColumnName;

        VisualElement m_AddColumnOverlay;
        EnumField m_AddColumnType;
        ITable.ColumnEntry[] m_ColumnDefinitions;


        VisualElement m_Overlay;

        MultiColumnListView m_TableView;
        Columns m_TableViewColumns;

        SimpleTable m_TargetTable;

        Button m_ToolbarAddColumn;


        public void OnDestroy()
        {
            AssetDatabase.SaveAssetIfDirty(m_TargetTable);
            if (k_Windows.ContainsKey(m_TargetTable))
            {
                k_Windows.Remove(m_TargetTable);
            }
        }

        [OnOpenAsset(1)]
        public static bool OpenSimpleTable(int instanceID, int line)
        {
            Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
            if (unityObject is SimpleTable table)
            {
                OpenAsset(table);
                return true;
            }

            return false;
        }

        public static SimpleTableWindow OpenAsset(SimpleTable table)
        {
            SimpleTableWindow simpleTableWindow;
            if (k_Windows.TryGetValue(table, out SimpleTableWindow window))
            {
                simpleTableWindow = window;
            }
            else
            {
                simpleTableWindow = CreateWindow<SimpleTableWindow>();
                k_Windows.Add(table, simpleTableWindow);
            }



            simpleTableWindow.BindSimpleTable(table);

            simpleTableWindow.Show();
            simpleTableWindow.Focus();

            return simpleTableWindow;
        }

        void OnEnable()
        {
            ResourcesProvider.SetupStylesheets(rootVisualElement);
            ResourcesProvider.GetVisualTreeAsset("GDXSimpleTable").CloneTree(rootVisualElement);
            ResourcesProvider.CheckTheme(rootVisualElement);

            BindWindow();

            // Catch domain reload and rebind/relink window
            if (m_TargetTable != null)
            {
                k_Windows[m_TargetTable] = this;
                BindSimpleTable(m_TargetTable);
            }
        }


        void BindSimpleTable(SimpleTable table)
        {
            m_TargetTable = table;
            titleContent = new GUIContent(m_TargetTable.name);
            m_ColumnDefinitions = table.GetOrderedColumns();

            // Precache some things
            VisualElement rootElement = rootVisualElement[0];

            // Generate columns for MCLV
            m_TableViewColumns = new Columns();
            int columnCount = table.ColumnCount;
            Length columnSizePercentage = Length.Percent(1f / columnCount);
            for (int i = 0; i < columnCount; i++)
            {
                ref ITable.ColumnEntry refColumn = ref m_ColumnDefinitions[i];
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
            m_ToolbarAddColumn = rootVisualElement.Q<Button>("gdx-simple-table-toolbar-add-column");
            m_ToolbarAddColumn.clicked += AddColumn_Show;

            m_Overlay = rootVisualElement.Q<VisualElement>("gdx-simple-table-overlay");
            // Ensure that the overlay is not visible
            m_Overlay.visible = false;

            // Build out references for adding a column
            m_AddColumnOverlay = m_Overlay.Q<VisualElement>("gdx-simple-table-add-column");
            m_AddColumnName = m_AddColumnOverlay.Q<TextField>("gdx-simple-table-column-name");

            m_AddColumnType = m_AddColumnOverlay.Q<EnumField>("gdx-simple-table-column-type");

            m_AddColumnAddButton = m_AddColumnOverlay.Q<Button>("gdx-simple-table-column-add");
            m_AddColumnAddButton.clicked += AddColumn_AddButtonClicked;
            m_AddColumnCancelButton = m_AddColumnOverlay.Q<Button>("gdx-simple-table-column-cancel");
            m_AddColumnCancelButton.clicked += AddColumn_CancelButtonClicked;
        }


        void AddColumn_AddButtonClicked()
        {
            ITable.ColumnType selectedType = (ITable.ColumnType)m_AddColumnType.value;
            switch (selectedType)
            {
                case ITable.ColumnType.String:
                    m_TargetTable.AddStringColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Char:
                    m_TargetTable.AddCharColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Bool:
                    m_TargetTable.AddBoolColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.SByte:
                    m_TargetTable.AddSbyteColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Byte:
                    m_TargetTable.AddByteColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Short:
                    m_TargetTable.AddShortColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.UShort:
                    m_TargetTable.AddUshortColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Int:
                    m_TargetTable.AddIntColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.UInt:
                    m_TargetTable.AddUintColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Long:
                    m_TargetTable.AddLongColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.ULong:
                    m_TargetTable.AddUlongColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Float:
                    m_TargetTable.AddFloatColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Double:
                    m_TargetTable.AddDoubleColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Vector2:
                    m_TargetTable.AddVector2Column(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Vector3:
                    m_TargetTable.AddVector3Column(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Vector4:
                    m_TargetTable.AddVector4Column(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Vector2Int:
                    m_TargetTable.AddVector2IntColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Vector3Int:
                    m_TargetTable.AddVector3IntColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Quaternion:
                    m_TargetTable.AddQuaternionColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Rect:
                    m_TargetTable.AddRectColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.RectInt:
                    m_TargetTable.AddRectIntColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Color:
                    m_TargetTable.AddColorColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.LayerMask:
                    m_TargetTable.AddLayerMaskColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Bounds:
                    m_TargetTable.AddBoundsColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.BoundsInt:
                    m_TargetTable.AddBoundsIntColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Hash128:
                    m_TargetTable.AddHash128Column(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Gradient:
                    m_TargetTable.AddGradientColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.AnimationCurve:
                    m_TargetTable.AddAnimationCurveColumn(m_AddColumnName.value);
                    break;
                case ITable.ColumnType.Object:
                    m_TargetTable.AddObjectColumn(m_AddColumnName.value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            // Do we want this , are we going to handle save?
            EditorUtility.SetDirty(m_TargetTable);

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
    }
#endif
}