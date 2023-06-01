// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Editor.Windows.DataTables;
using GDX.DataTables;
#if UNITY_2022_2_OR_NEWER
using System.Text;
using UnityEngine.UIElements;
#endif

namespace GDX.Editor.Inspectors
{
#if UNITY_2022_2_OR_NEWER
    public abstract class DataTableInspectorBase : UnityEditor.Editor, DataTableTracker.IColumnDefinitionChangeCallbackReceiver,
        DataTableTracker.IRowDefinitionChangeCallbackReceiver
    {
        const string k_ButtonText = "Open Table";
        static char[] s_Divider = new[] { ',', ' ' };

        VisualElement m_RootElement;
        Button m_OpenAssetButton;

        Label m_RowLabel;
        Label m_RowDescription;
        Label m_ColumnLabel;
        Label m_ColumnDescription;
        Label m_DataTableLabel;
        Label m_DataTableTrackerLabel;

        Label m_DataTableTracker;

        int m_TableTicket;
        bool m_Bound;

        void OnDisable()
        {
            if (!m_Bound)
            {
                return;
            }

            DataTableTracker.UnregisterColumnChanged(this, m_TableTicket);
            DataTableTracker.UnregisterRowChanged(this, m_TableTicket);
            DataTableTracker.UnregisterUsage(m_TableTicket);
        }

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            DataTableObject dataTable = (DataTableObject)target;

            m_TableTicket = DataTableTracker.RegisterTable(dataTable);
            m_RootElement = new VisualElement();
            ResourcesProvider.SetupSharedStylesheets(m_RootElement);
            ResourcesProvider.SetupStylesheet("GDXDataTableInspector", m_RootElement);

            m_DataTableLabel = new Label() { name = "gdx-datatable-inspector-name" };

            m_RowLabel = new Label("Rows") { name = "gdx-datatable-inspector-rows-label"};
            m_RowLabel.AddToClassList("gdx-datatable-inspector-label");
            m_RowDescription = new Label("") { name = "gdx-datatable-inspector-rows-description" };
            m_RowDescription.AddToClassList("gdx-datatable-inspector-description");

            m_ColumnLabel = new Label("Columns") { name = "gdx-datatable-inspector-columns-label"};
            m_ColumnLabel.AddToClassList("gdx-datatable-inspector-label");
            m_ColumnDescription = new Label() { name = "gdx-datatable-inspector-columns-description"};
            m_ColumnDescription.AddToClassList("gdx-datatable-inspector-description");

            m_OpenAssetButton = new Button(OpenTargetAsset) { text = k_ButtonText, name = "gdx-datatable-inspector-open"};

            m_DataTableTrackerLabel = new Label("Tracker") { name = "gdx-datatable-inspector-tracker-label" };
            m_DataTableTrackerLabel.AddToClassList("gdx-datatable-inspector-label");
            m_DataTableTracker = new Label() { name = "gdx-datatable-inspector-tracker" };

            m_RootElement.Add(m_DataTableLabel);
            m_RootElement.Add(m_RowLabel);
            m_RootElement.Add(m_RowDescription);
            m_RootElement.Add(m_ColumnLabel);
            m_RootElement.Add(m_ColumnDescription);
            m_RootElement.Add(m_OpenAssetButton);
            m_RootElement.Add(m_DataTableTrackerLabel);
            m_RootElement.Add(m_DataTableTracker);

            DataTableTracker.RegisterColumnChanged(this, m_TableTicket);
            DataTableTracker.RegisterRowChanged(this, m_TableTicket);
            DataTableTracker.RegisterUsage(m_TableTicket);
            m_Bound = true;

            UpdateInspector();

            return m_RootElement;
        }

        void UpdateInspector()
        {
            DataTableObject dataTable = (DataTableObject)target;

            DataTableObject.RowDescription[] rowDescriptions = dataTable.GetAllRowDescriptions();
            DataTableObject.ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();

            int rowCount = rowDescriptions.Length;
            int columnCount = columnDescriptions.Length;

            m_DataTableLabel.text = dataTable.GetDisplayName();
            m_RowLabel.text = $"Rows ({rowCount})";

            m_RowLabel.text = $"Rows ({rowCount})";
            StringBuilder content = new StringBuilder(100);
            for (int i = 0; i < rowCount; i++)
            {
                content.Append(rowDescriptions[i]);
                content.Append(", ");
            }
            m_RowDescription.text = content.ToString().TrimEnd(s_Divider);
            content.Clear();

            m_ColumnLabel.text = $"Columns ({columnCount})";
            for (int i = 0; i < columnCount; i++)
            {
                content.Append(columnDescriptions[i]);
                content.Append(", ");
            }
            m_ColumnDescription.text = content.ToString().TrimEnd(s_Divider);

            DataTableTracker.DataTableTrackerStats stats = DataTableTracker.GetStats(m_TableTicket);
            m_DataTableTracker.text = $"{stats.Usages} Usages\n{stats.CellValueChanged} Cell Monitors\n{stats.ColumnDefinitionChange} Column Monitors\n{stats.RowDefinitionChange} Row Monitors";
        }

        void OpenTargetAsset()
        {
            DataTableObject dataTable = (DataTableObject)target;
            TableWindowProvider.OpenAsset(dataTable);
            UpdateInspector();
        }

        /// <inheritdoc />
        public void OnColumnDefinitionChange()
        {
            UpdateInspector();
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange()
        {
            UpdateInspector();
        }
    }
#else
    public class DataTableInspectorBase : UnityEditor.Editor
    {
        /// <inheritdoc />
        public override void OnInspectorGUI()
        {
            UnityEngine.GUILayout.Label("Editing an DataTableBase is unsupported on this version of Unity.");
        }
    }
#endif
}