// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
        Label m_SourceLabel;
        Label m_SourceDescription;
        Button m_SourcePullButton;
        Button m_SourcePushButton;
        Label m_InterchangeLabel;
        Button m_ExportToCommaSeperatedValuesButton;
        Button m_ImportButton;
        Button m_ExportToJavaScriptObjectNotationButton;
        VisualElement m_RootElement;
        Label m_RowDescription;
        Label m_RowLabel;

        /// <summary>
        ///     Cached version of the <see cref="DataTableBase" /> ticket from <see cref="m_DataTableTracker" />.
        /// </summary>
        int m_TableTicket = -1;

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
        /// <param name="evt">The detach event.</param>
        void Unbind(DetachFromPanelEvent evt)
        {
            m_RootElement.UnregisterCallback<DetachFromPanelEvent>(Unbind);

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

            // This is for when it is being created, we do not want to show anything until they have saved to disk the first time.
            if (!EditorUtility.IsPersistent(target))
            {
                return new TextElement()
                {
                    text = "A DataTable must be present on disk before this inspector will populate.",
                    style = { unityFontStyleAndWeight = FontStyle.Italic }
                };
            }

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
            m_ImportButton =
                new Button(Import) { text = "Import", name = "gdx-datatable-inspector-import" };
            m_ExportToJavaScriptObjectNotationButton =
                new Button(ExportToJavaScriptNotationObjects) { text = "Export (JSON)", name = "gdx-datatable-inspector-export-json" };


            m_SourceLabel = new Label("Source Of Truth") { name = "gdx-datatable-inspector-sync-label" };
            m_SourceLabel.AddToClassList("gdx-datatable-inspector-label");
            m_SourceDescription = new Label("") { name = "gdx-datatable-inspector-sync-description" };
            m_SourceDescription.AddToClassList("gdx-datatable-inspector-description");

            m_SourcePullButton = new Button(OnPushButton) { text = "Push", name = "gdx-datatable-inspector-source-push" };
            m_SourcePushButton = new Button(OnPullButton) { text = "Pull", name = "gdx-datatable-inspector-source-pull" };



            m_DataTableTrackerLabel = new Label("Tracker") { name = "gdx-datatable-inspector-tracker-label" };
            m_DataTableTrackerLabel.AddToClassList("gdx-datatable-inspector-label");
            m_DataTableTracker = new Label { name = "gdx-datatable-inspector-tracker" };


            m_RootElement.Add(m_DataTableLabel);

            m_RootElement.Add(m_RowLabel);
            m_RootElement.Add(m_RowDescription);
            m_RootElement.Add(m_ColumnLabel);
            m_RootElement.Add(m_ColumnDescription);

            m_RootElement.Add(m_SourceLabel);
            m_RootElement.Add(m_SourceDescription);
            VisualElement sourceRow = new VisualElement();
            sourceRow.AddToClassList("gdx-datatable-inspector-row");
            sourceRow.Add(m_SourcePullButton);
            sourceRow.Add(m_SourcePushButton);
            m_RootElement.Add(sourceRow);

            m_RootElement.Add(m_InterchangeLabel);

            VisualElement firstRow = new VisualElement();
            firstRow.AddToClassList("gdx-datatable-inspector-row");
            firstRow.Add(m_ExportToCommaSeperatedValuesButton);
            firstRow.Add(m_ExportToJavaScriptObjectNotationButton);
            m_RootElement.Add(firstRow);

            VisualElement secondRow = new VisualElement();
            secondRow.AddToClassList("gdx-datatable-inspector-row");
            secondRow.Add(m_ImportButton);
            m_RootElement.Add(secondRow);

            m_RootElement.Add(m_DataTableTrackerLabel);
            m_RootElement.Add(m_DataTableTracker);

            DataTableTracker.RegisterStructuralChanged(this, m_TableTicket);
            DataTableTracker.RegisterUndoRedoEvent(this, m_TableTicket);
            DataTableTracker.AddUsage(m_TableTicket);
            m_Bound = true;

            int currentStructureVersion = dataTable.GetStructureCurrentVersion();
            int tableStructureVersion = dataTable.GetStructureVersion();

            if (currentStructureVersion != tableStructureVersion)
            {
                if (dataTable.Migrate(currentStructureVersion))
                {
                    Debug.Log($"Migrated {dataTable.GetMetaData().DisplayName} from Structure version {tableStructureVersion.ToString()} to version {currentStructureVersion.ToString()}.");
                    AssetDatabase.SaveAssetIfDirty(dataTable);
                }
                else
                {
                    Debug.LogWarning($"Attempted to migrate {dataTable.GetMetaData().DisplayName} from Structure version {tableStructureVersion.ToString()} to version {currentStructureVersion.ToString()}.");
                }
            }

            UpdateInspector();

            m_RootElement.RegisterCallback<DetachFromPanelEvent>(Unbind);

            return m_RootElement;
        }

        void OnPushButton()
        {
            SourcePush(target as DataTableBase);
        }
        void OnPullButton()
        {
            SourcePull(target as DataTableBase);
        }


        /// <summary>
        ///     Update the inspectors information about the <see cref="DataTableBase" />.
        /// </summary>
        /// <remarks>The stats are not always live, as the inspector is not constantly updating.</remarks>
        void UpdateInspector()
        {
            DataTableBase dataTable = (DataTableBase)target;
            StringBuilder content = new StringBuilder(100);

            m_DataTableLabel.text = dataTable.GetMetaData().DisplayName;
            m_DataTableLabel.tooltip = $"Data Version: {dataTable.GetDataVersion().ToString()}\nStructure Version: {dataTable.GetStructureVersion().ToString()}";

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
            m_ExportToJavaScriptObjectNotationButton.SetEnabled(columnCount > 0);
            m_ImportButton.SetEnabled(columnCount > 0);

            DataTableMetaData metaData = dataTable.GetMetaData();

            if (metaData.HasSourceOfTruth())
            {
                m_SourceDescription.text = metaData.AssetRelativePath;
                m_SourcePullButton.SetEnabled(true);
                m_SourcePushButton.SetEnabled(true);
            }
            else
            {
                m_SourceDescription.text = "No source of truth set.";
                m_SourcePullButton.SetEnabled(false);
                m_SourcePushButton.SetEnabled(false);
            }
        }

        void ExportToCommaSeperatedValues()
        {
            ShowExportDialogForTable(DataTableInterchange.Format.CommaSeperatedValues, (DataTableBase)target);
        }

        void Import()
        {
            ShowImportDialogForTable((DataTableBase)target);
        }

        void ExportToJavaScriptNotationObjects()
        {
            ShowExportDialogForTable(DataTableInterchange.Format.JavaScriptObjectNotation, (DataTableBase)target);
        }


        public static void ShowExportDialogForTable(DataTableInterchange.Format format, DataTableBase dataTable)
        {

            string savePath;
            if (format == DataTableInterchange.Format.CommaSeperatedValues)
            {
                savePath = EditorUtility.SaveFilePanel($"Export {dataTable.GetMetaData().DisplayName} to CSV",
                    Application.dataPath,
                    dataTable.name, "csv");
            }
            else
            {
                savePath = EditorUtility.SaveFilePanel($"Export {dataTable.GetMetaData().DisplayName} to JSON",
                    Application.dataPath,
                    dataTable.name, "json");
            }


            if (!string.IsNullOrEmpty(savePath))
            {
                if (format == DataTableInterchange.Format.CommaSeperatedValues)
                {
                    DataTableInterchange.Export(dataTable, DataTableInterchange.Format.CommaSeperatedValues, savePath);
                }
                else
                {
                    DataTableInterchange.Export(dataTable, DataTableInterchange.Format.JavaScriptObjectNotation,
                        savePath);
                }
                Debug.Log($"'{dataTable.GetMetaData().DisplayName}' was exported to {savePath}.");
            }
        }

        public static void ShowImportDialogForTable(DataTableBase dataTable)
        {
            string openPath = EditorUtility.OpenFilePanelWithFilters($"Import into {dataTable.GetMetaData().DisplayName}",
                Application.dataPath, new[] { "JSON", "json", "CSV", "csv" });

            if (string.IsNullOrEmpty(openPath))
            {
                return;
            }

            string extension = System.IO.Path.GetExtension(openPath).ToLower();
            DataTableInterchange.Format format;
            switch (extension)
            {
                case ".csv":
                    format = DataTableInterchange.Format.CommaSeperatedValues;
                    break;
                case ".json":
                    format = DataTableInterchange.Format.JavaScriptObjectNotation;
                    break;
                default:
                    Debug.LogError($"Unrecognized format extension '{extension}'.");
                    return;
            }

            if (EditorUtility.DisplayDialog($"Replace '{dataTable.GetMetaData().DisplayName}' Content",
                    $"Are you sure you want to replace your tables content with the imported content?\n\nThe structural format of the content needs to match the column structure of the existing table; reference types will not replace the data in the existing cells at that location. Make sure the first row contains the column names, and that you have not altered columns.",
                    "Yes", "No"))
            {
                if (dataTable.GetMetaData().SupportsUndo)
                {
                    Undo.RegisterCompleteObjectUndo(dataTable, $"DataTable {DataTableTracker.GetTicket(dataTable)} - Import");
                }

                if (DataTableInterchange.Import(dataTable, format, openPath))
                {
                    DataTableTracker.NotifyOfColumnChange(DataTableTracker.GetTicket(dataTable), -1);
                }
            }
        }

        public static void SourcePull(DataTableBase dataTable)
        {
            DataTableMetaData metaData = dataTable.GetMetaData();
            string filePath =  System.IO.Path.Combine(Application.dataPath, metaData.AssetRelativePath);
            switch (metaData.SyncFormat)
            {
                case DataTableInterchange.Format.CommaSeperatedValues:
                case DataTableInterchange.Format.JavaScriptObjectNotation:

                    if (metaData.SupportsUndo)
                    {
                        Undo.RegisterCompleteObjectUndo(dataTable, $"DataTable {DataTableTracker.GetTicket(dataTable)} - Source Import");
                    }
                    DataTableInterchange.Import(dataTable, metaData.SyncFormat, filePath);
                    DataTableTracker.NotifyOfRowChange(DataTableTracker.GetTicket(dataTable), -1);

                    metaData.SyncTimestamp = System.IO.File.GetLastWriteTimeUtc(filePath);;
                    metaData.SyncDataVersion = dataTable.GetDataVersion();
                    EditorUtility.SetDirty(metaData);
                    break;
            }
        }

        public static void SourcePush(DataTableBase dataTable)
        {
            DataTableMetaData metaData = dataTable.GetMetaData();
            string filePath =  System.IO.Path.Combine(Application.dataPath, metaData.AssetRelativePath);
            switch (metaData.SyncFormat)
            {
                case DataTableInterchange.Format.CommaSeperatedValues:
                case DataTableInterchange.Format.JavaScriptObjectNotation:

                    // Check the timestamps as a sanity check.
                    DateTime currentTimestamp = System.IO.File.GetLastWriteTimeUtc(filePath);
                    if (currentTimestamp > metaData.SyncTimestamp)
                    {
                        if (!EditorUtility.DisplayDialog($"Overwrite '{metaData.AssetRelativePath}'?",
                                $"The files timestamp is newer then the last known sync timestamp which means the file could have newer data which you will stomp. Are you sure you want to do this?",
                                "Yes", "No"))
                        {
                            return;
                        }
                    }

                    DataTableInterchange.Export(dataTable, metaData.SyncFormat, filePath);
                    metaData.SyncTimestamp = System.IO.File.GetLastWriteTimeUtc(filePath);;
                    metaData.SyncDataVersion = dataTable.GetDataVersion();
                    EditorUtility.SetDirty(metaData);
                    break;
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