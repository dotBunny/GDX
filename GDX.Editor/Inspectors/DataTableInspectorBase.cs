﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.DataTables;
using GDX.DataTables.DataBinding;
using UnityEditor;
using UnityEngine;
#if UNITY_2022_2_OR_NEWER
using System.IO;
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
        Label m_BindingDescription;
        Label m_BindingLabel;
        Button m_BindingPullButton;
        Button m_BindingPushButton;
        VisualElement m_BindingRow;

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
        VisualElement m_ExportContainer;
        Button m_ImportButton;
        Label m_InterchangeLabel;
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
                return new TextElement
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
            m_ExportContainer = new VisualElement { name = "gdx-datatable-inspector-interchange-export" };

            m_ImportButton =
                new Button(() =>
                {
                    ShowImportDialogForTable((DataTableBase)target);
                }) { text = "Import", name = "gdx-datatable-inspector-import" };

            m_BindingLabel = new Label("Data Binding") { name = "gdx-datatable-inspector-binding-label" };
            m_BindingLabel.AddToClassList("gdx-datatable-inspector-label");
            m_BindingDescription = new Label("") { name = "gdx-datatable-inspector-sync-description" };
            m_BindingDescription.AddToClassList("gdx-datatable-inspector-description");

            m_BindingPullButton =
                new Button(OnPushButton) { text = "Push", name = "gdx-datatable-inspector-binding-push" };
            m_BindingPushButton =
                new Button(OnPullButton) { text = "Pull", name = "gdx-datatable-inspector-binding-pull" };


            m_DataTableTrackerLabel = new Label("Tracker") { name = "gdx-datatable-inspector-tracker-label" };
            m_DataTableTrackerLabel.AddToClassList("gdx-datatable-inspector-label");
            m_DataTableTracker = new Label { name = "gdx-datatable-inspector-tracker" };


            m_RootElement.Add(m_DataTableLabel);

            m_RootElement.Add(m_RowLabel);
            m_RootElement.Add(m_RowDescription);
            m_RootElement.Add(m_ColumnLabel);
            m_RootElement.Add(m_ColumnDescription);

            m_RootElement.Add(m_BindingLabel);
            m_RootElement.Add(m_BindingDescription);
            m_BindingRow = new VisualElement();
            m_BindingRow.AddToClassList("gdx-datatable-inspector-row");
            m_BindingRow.Add(m_BindingPullButton);
            m_BindingRow.Add(m_BindingPushButton);
            m_RootElement.Add(m_BindingRow);

            m_RootElement.Add(m_InterchangeLabel);


            // Dynamic export
            FormatBase[] formats = DataBindingProvider.GetFormats();
            int formatCount = formats.Length;
            for (int i = 0; i < formatCount; i++)
            {
                FormatBase format = formats[i];
                if (format.IsOnDiskFormat())
                {
                    m_ExportContainer.Add(new Button(() =>
                    {
                        ShowExportDialogForTable((DataTableBase)target, format.GetFriendlyName(),
                            format.GetFilePreferredExtension());
                    }) { text = $"Export ({format.GetFriendlyName()})" });
                }
            }

            m_RootElement.Add(m_ExportContainer);


            m_RootElement.Add(m_ImportButton);

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
                    Debug.Log(
                        $"Migrated {dataTable.GetMetaData().DisplayName} from Structure version {tableStructureVersion.ToString()} to version {currentStructureVersion.ToString()}.");
                    AssetDatabase.SaveAssetIfDirty(dataTable);
                }
                else
                {
                    Debug.LogWarning(
                        $"Attempted to migrate {dataTable.GetMetaData().DisplayName} from Structure version {tableStructureVersion.ToString()} to version {currentStructureVersion.ToString()}.");
                }
            }

            UpdateInspector();

            m_RootElement.RegisterCallback<DetachFromPanelEvent>(Unbind);

            return m_RootElement;
        }

        void OnPushButton()
        {
            BindingPush(target as DataTableBase);
        }

        void OnPullButton()
        {
            BindingPull(target as DataTableBase);
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
            m_DataTableLabel.tooltip =
                $"Data Version: {dataTable.GetDataVersion().ToString()}\nStructure Version: {dataTable.GetStructureVersion().ToString()}";

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

            m_ExportContainer.SetEnabled(columnCount > 0);
            m_ImportButton.SetEnabled(columnCount > 0);

            DataTableMetaData metaData = dataTable.GetMetaData();

            if (metaData.HasBinding())
            {
                m_BindingDescription.text = metaData.BindingUri;
                m_BindingRow.style.display = DisplayStyle.Flex;
            }
            else
            {
                m_BindingDescription.text = "No binding has been set for this Data Table.";
                m_BindingRow.style.display = DisplayStyle.None;
            }
        }

        public static void ShowExportDialogForTable(DataTableBase dataTable, string friendlyName, string extension)
        {
            string savePath = EditorUtility.SaveFilePanel(
                $"Export {dataTable.GetMetaData().DisplayName} to {friendlyName}",
                Application.dataPath,
                dataTable.name, extension);

            if (!string.IsNullOrEmpty(savePath))
            {
                DataBindingProvider.Export(dataTable, savePath);
                Debug.Log($"'{dataTable.GetMetaData().DisplayName}' was exported to {savePath}.");
            }
        }

        public static void ShowImportDialogForTable(DataTableBase dataTable)
        {
            string openPath = EditorUtility.OpenFilePanelWithFilters(
                $"Import into {dataTable.GetMetaData().DisplayName}",
                Application.dataPath, DataBindingProvider.GetImportDialogExtensions());

            if (string.IsNullOrEmpty(openPath))
            {
                return;
            }

            if (EditorUtility.DisplayDialog($"Replace '{dataTable.GetMetaData().DisplayName}' Content",
                    "Are you sure you want to replace your tables content with the imported content?\n\nThe structural format of the content needs to match the column structure of the existing table; reference types will not replace the data in the existing cells at that location. Make sure the first row contains the column names, and that you have not altered columns.",
                    "Yes", "No"))
            {
                if (dataTable.GetMetaData().SupportsUndo)
                {
                    Undo.RegisterCompleteObjectUndo(dataTable,
                        $"DataTable {DataTableTracker.GetTicket(dataTable)} - Import");
                }

                if (DataBindingProvider.Import(dataTable, openPath))
                {
                    DataTableTracker.NotifyOfColumnChange(DataTableTracker.GetTicket(dataTable), -1);
                }
                else
                {
                    Debug.LogWarning("Import failed.");
                }
            }
        }

        public static void BindingPull(DataTableBase dataTable)
        {
            DataTableMetaData metaData = dataTable.GetMetaData();
            FormatBase format = DataBindingProvider.GetFormatFromUri(metaData.BindingUri);
            if (format == null)
            {
                return;
            }

            string uri = format.IsOnDiskFormat()
                ? Path.GetFullPath(Path.Combine(Application.dataPath, metaData.BindingUri))
                : metaData.BindingUri;

            if (metaData.SupportsUndo)
            {
                Undo.RegisterCompleteObjectUndo(dataTable,
                    $"DataTable {DataTableTracker.GetTicket(dataTable).ToString()} - Binding Import");
            }

            SerializableTable serializableTable =
                format.Pull(uri, dataTable.GetDataVersion(), dataTable.GetStructureVersion());

            if (serializableTable != null && serializableTable.Update(dataTable))
            {
                DataTableTracker.NotifyOfRowChange(DataTableTracker.GetTicket(dataTable), -1);
                metaData.BindingTimestamp = format.GetBindingTimestamp(uri);
                metaData.BindingDataVersion = dataTable.GetDataVersion();
                EditorUtility.SetDirty(metaData);
            }
        }

        public static void BindingPush(DataTableBase dataTable)
        {
            DataTableMetaData metaData = dataTable.GetMetaData();
            FormatBase format = DataBindingProvider.GetFormatFromUri(metaData.BindingUri);
            if (format == null)
            {
                return;
            }

            string uri = format.IsOnDiskFormat()
                ? Path.GetFullPath(Path.Combine(Application.dataPath, metaData.BindingUri))
                : metaData.BindingUri;

            DateTime currentTimestamp = format.GetBindingTimestamp(uri);
            if (currentTimestamp > metaData.BindingTimestamp)
            {
                if (!EditorUtility.DisplayDialog($"Overwrite '{metaData.BindingUri}'?",
                        "The bindings timestamp is newer then the last known sync timestamp which means the file could have newer data which you will stomp. Are you sure you want to do this?",
                        "Yes", "No"))
                {
                    return;
                }
            }

            if (format.Push(uri, new SerializableTable(dataTable)))
            {
                metaData.BindingTimestamp = format.GetBindingTimestamp(uri);
                metaData.BindingDataVersion = dataTable.GetDataVersion();
                EditorUtility.SetDirty(metaData);
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