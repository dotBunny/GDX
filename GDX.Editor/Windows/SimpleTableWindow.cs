using System;
using System.Collections;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows
{
    public class SimpleTableWindow : EditorWindow
    {
        static readonly Dictionary<SimpleTable, SimpleTableWindow> k_Windows = new Dictionary<SimpleTable, SimpleTableWindow>();

        SimpleTable m_TargetTable;
        SimpleTable.ColumnEntry[] m_ColumnDefinitions;

        MultiColumnListView m_TableView;
        Columns m_TableViewColumns;

        bool m_Initialized;

        [OnOpenAsset(1)]
        public static bool OpenSimpleTable(int instanceID, int line)
        {
            UnityEngine.Object unityObject = EditorUtility.InstanceIDToObject(instanceID);
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

            if (!simpleTableWindow.m_Initialized)
            {
                VisualElement rootElement = simpleTableWindow.rootVisualElement;
                ResourcesProvider.SetupStylesheets(rootElement);
                ResourcesProvider.GetVisualTreeAsset("GDXSimpleTable").CloneTree(rootElement);
                ResourcesProvider.CheckTheme(rootElement);

                simpleTableWindow.m_Initialized = true;
            }

            simpleTableWindow.Rebind(table);

            simpleTableWindow.Show();
            simpleTableWindow.Focus();

            return simpleTableWindow;
        }



        void Rebind(SimpleTable table)
        {
            m_TargetTable = table;
            titleContent = new GUIContent(m_TargetTable.name);
            m_ColumnDefinitions = table.GetOrderedColumns();

            // Precache some things
            VisualElement rootElement = rootVisualElement[0];

            // Generate columns for MCLV
            m_TableViewColumns = new Columns();
            int columnCount = table.ColumnCount;
            for (int i = 0; i < columnCount; i++)
            {
                ref SimpleTable.ColumnEntry refColumn = ref m_ColumnDefinitions[i];
                Column column = new Column
                {
                    name = refColumn.Name,
                    title = refColumn.Name,
                };
                m_TableViewColumns.Add(column);
            }

            // Create MCLV
            if (m_TableView != null)
            {
                rootElement.Remove(m_TableView);
            }
            m_TableView = new MultiColumnListView(m_TableViewColumns);
            rootElement.Add(m_TableView);
        }

        public void OnDestroy()
        {
            k_Windows.Remove(m_TargetTable);
        }


        void RebuildView()
        {

        }
    }
}
