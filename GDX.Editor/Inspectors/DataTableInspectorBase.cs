// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.DataTables;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using System.Text;
using UnityEngine.UIElements;
#endif // UNITY_2022_2_OR_NEWER

namespace GDX.Editor.Inspectors
{
    /// <summary>
    ///     A custom inspector for <see cref="DataTableBase" /> inheritors.
    /// </summary>
#if UNITY_2022_2_OR_NEWER
    public abstract class DataTableInspectorBase : UnityEditor.Editor,
        DataTableTracker.IStructuralChangeCallbackReceiver, DataTableTracker.IUndoRedoEventCallbackReceiver
    {
        /// <summary>
        ///     Is the bound/subscribed to the <see cref="DataTableTracker" />.
        /// </summary>
        bool m_Bound;

        Label m_ColumnDescription;
        Label m_ColumnLabel;
        Label m_DataTableLabel;
        Label m_DataTableTracker;
        Label m_DataTableTrackerLabel;
        Button m_DataTableTrackerRefreshButton;
        Button m_OpenAssetButton;
        Label m_InterchangeLabel;
        Button m_ExportToCommaSeperatedValuesButton;
        Button m_ImportFromCommaSeperatedValuesButton;
        VisualElement m_RootElement;
        Label m_RowDescription;
        Label m_RowLabel;

        /// <summary>
        ///     Cached version of the <see cref="DataTableBase" /> ticket from <see cref="m_DataTableTracker" />.
        /// </summary>
        int m_TableTicket;

        /// <inheritdoc />
        public void OnColumnDefinitionChange(int columnIdentifier)
        {
            UpdateInspector();
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange(int rowIdentifier)
        {
            UpdateInspector();
        }

        public void OnSettingsChange()
        {
            //throw new System.NotImplementedException();
        }

        public void OnUndoRedoRowDefinitionChange(int rowIdentifier)
        {
            UpdateInspector();
        }

        public void OnUndoRedoColumnDefinitionChange(int columnIdentifier)
        {
            UpdateInspector();
        }

        public void OnUndoRedoCellValueChanged(int rowIdentifier, int columnIdentifier)
        {
        }

        public void OnUndoRedoSettingsChanged()
        {
            UpdateInspector();
        }

        /// <summary>
        ///     Unity driven event when the inspector is disabled.
        /// </summary>
        /// <remarks>
        ///     Callbacks are unregistered and the usage is removed from the <see cref="DataTableTracker" />.
        /// </remarks>
        void Unbind()
        {
            if (!m_Bound)
            {
                return;
            }

            DataTableTracker.UnregisterStructuralChanged(this, m_TableTicket);
            DataTableTracker.UnregisterUndoRedoEvent(this, m_TableTicket);
            DataTableTracker.RemoveUsage(m_TableTicket);
        }

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            DataTableBase dataTable = (DataTableBase)target;

            m_TableTicket = DataTableTracker.RegisterTable(dataTable);
            m_RootElement = new VisualElement();
            m_DataTableTrackerRefreshButton =
                new Button(UpdateInspector) { text = "Refresh", name = "gdx-datatable-refresh" };
            m_RootElement.Add(m_DataTableTrackerRefreshButton);

            ResourcesProvider.SetupSharedStylesheets(m_RootElement);
            ResourcesProvider.SetupStylesheet("GDXDataTableInspector", m_RootElement);

            m_DataTableLabel = new Label { name = "gdx-datatable-inspector-name" };

            m_RowLabel = new Label("Rows") { name = "gdx-datatable-inspector-rows-label" };
            m_RowLabel.AddToClassList("gdx-datatable-inspector-label");
            m_RowDescription = new Label("") { name = "gdx-datatable-inspector-rows-description" };
            m_RowDescription.AddToClassList("gdx-datatable-inspector-description");

            m_ColumnLabel = new Label("Columns") { name = "gdx-datatable-inspector-columns-label" };
            m_ColumnLabel.AddToClassList("gdx-datatable-inspector-label");
            m_ColumnDescription = new Label { name = "gdx-datatable-inspector-columns-description" };
            m_ColumnDescription.AddToClassList("gdx-datatable-inspector-description");

            m_InterchangeLabel = new Label("Interchange") { name = "gdx-datatable-inspector-interchange-label" };
            m_InterchangeLabel.AddToClassList("gdx-datatable-inspector-label");
            m_ExportToCommaSeperatedValuesButton =
                new Button(ExportToCommaSeperatedValues) { text = "Export (CSV)", name = "gdx-datatable-inspector-export-csv" };
            m_ImportFromCommaSeperatedValuesButton =
                new Button(ImportFromCommaSeperatedValues) { text = "Import (CSV)", name = "gdx-datatable-inspector-import-csv" };

            m_DataTableTrackerLabel = new Label("Tracker") { name = "gdx-datatable-inspector-tracker-label" };


            m_DataTableTrackerLabel.AddToClassList("gdx-datatable-inspector-label");
            m_DataTableTracker = new Label { name = "gdx-datatable-inspector-tracker" };


            m_RootElement.Add(m_DataTableLabel);
            m_RootElement.Add(m_RowLabel);
            m_RootElement.Add(m_RowDescription);
            m_RootElement.Add(m_ColumnLabel);
            m_RootElement.Add(m_ColumnDescription);
            m_RootElement.Add(m_OpenAssetButton);

            m_RootElement.Add(m_InterchangeLabel);
            VisualElement csvRow = new VisualElement();
            csvRow.AddToClassList("gdx-datatable-inspector-row");
            csvRow.Add(m_ExportToCommaSeperatedValuesButton);
            csvRow.Add(m_ImportFromCommaSeperatedValuesButton);
            m_RootElement.Add(csvRow);

            m_RootElement.Add(m_DataTableTrackerLabel);
            m_RootElement.Add(m_DataTableTracker);

            DataTableTracker.RegisterStructuralChanged(this, m_TableTicket);
            DataTableTracker.RegisterUndoRedoEvent(this, m_TableTicket);
            DataTableTracker.AddUsage(m_TableTicket);
            m_Bound = true;

            UpdateInspector();

            m_RootElement.RegisterCallback<DetachFromPanelEvent>(_ => Unbind());

            return m_RootElement;
        }

        /// <summary>
        ///     Update the inspectors information about the <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>The stats are not always live, as the inspector is not constantly updating.</remarks>
        void UpdateInspector()
        {
            DataTableBase dataTable = (DataTableBase)target;
            StringBuilder content = new StringBuilder(100);

            m_DataTableLabel.text = dataTable.GetDisplayName();

            if (dataTable.GetRowCount() == 0)
            {
                m_RowLabel.text = "No Rows";
                m_RowDescription.text = string.Empty;
            }
            else
            {
                RowDescription[] rowDescriptions = dataTable.GetAllRowDescriptions();
                int rowCount = rowDescriptions.Length;
                m_RowLabel.text = $"Rows ({rowCount})";
                int rowCountMinusOne = rowCount - 1;
                for (int i = 0; i < rowCount; i++)
                {
                    content.Append(rowDescriptions[i]);

                    if (i < rowCountMinusOne)
                    {
                        content.Append(", ");
                    }
                }
                m_RowDescription.text = content.ToString();
            }
            content.Clear();

            int columnCount = dataTable.GetColumnCount();
            if (columnCount == 0)
            {
                m_ColumnLabel.text = "No Columns";
                m_ColumnDescription.text = string.Empty;
            }
            else
            {
                ColumnDescription[] columnDescriptions = dataTable.GetAllColumnDescriptions();
                m_ColumnLabel.text = $"Columns ({columnCount})";
                int columnCountMinusOne = columnCount - 1;
                for (int i = 0; i < columnCount; i++)
                {
                    content.Append(columnDescriptions[i]);
                    if (i < columnCountMinusOne)
                    {
                        content.Append(", ");
                    }
                }

                m_ColumnDescription.text = content.ToString();
            }


            DataTableTracker.DataTableTrackerStats stats = DataTableTracker.GetStats(m_TableTicket);
            m_DataTableTracker.text =
                $"{stats.Usages} Usages\n{stats.CellValueChanged} Cell Monitors\n{stats.StructuralChange} Structural Monitors";


            m_ExportToCommaSeperatedValuesButton.SetEnabled(columnCount > 0);
            m_ImportFromCommaSeperatedValuesButton.SetEnabled(columnCount > 0);
        }

        void ExportToCommaSeperatedValues()
        {
            ShowExportDialogForTable((DataTableBase)target);
        }

        void ImportFromCommaSeperatedValues()
        {
            ShowImportDialogForTable((DataTableBase)target);
        }

        public static void ShowExportDialogForTable(DataTableBase dataTable)
        {
            string savePath = EditorUtility.SaveFilePanel($"Export {dataTable.GetDisplayName()} to CSV",
                Application.dataPath,
                dataTable.name, "csv");

            if (!string.IsNullOrEmpty(savePath))
            {
                dataTable.ExportToCommaSeperatedValues(savePath);
                Debug.Log($"'{dataTable.GetDisplayName()}' was exported to CSV at {savePath}");
            }
        }

        public static void ShowImportDialogForTable(DataTableBase dataTable)
        {
             string openPath = EditorUtility.OpenFilePanel($"Import CSV into {dataTable.GetDisplayName()}",
                            Application.dataPath,
                            "csv");

            if (!string.IsNullOrEmpty(openPath))
            {
                if (EditorUtility.DisplayDialog($"Replace '{dataTable.GetDisplayName()}' Content",
                        "Are you sure you want to replace your tables content with the imported CSV content?\n\nThe structural format of the CSV needs to match the column structure of the existing table; reference types will not replace the data in the existing cells at that location. Make sure the first row contains the column names, and that you have not reordered the rows or columns.",
                        "Yes", "No"))
                {
                    if (dataTable.UpdateFromCommaSeperatedValues(openPath))
                    {
                        DataTableTracker.NotifyOfColumnChange(DataTableTracker.GetTicket(dataTable), -1);
                    }
                }
            }
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
#endif // UNITY_2022_2_OR_NEWER
}