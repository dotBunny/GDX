using GDX.Collections.Generic;
using GDX.DataTables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public class DataTableWindow : EditorWindow, DataTableTracker.IStructuralChangeCallbackReceiver, DataTableTracker.ICellValueChangedCallbackReceiver, DataTableTracker.IUndoRedoEventCallbackReceiver
    {
        int m_DataTableTicket;
        DataTableWindowController m_Controller;
        DataTableWindowOverlay m_Overlay;

        DataTableBase m_DataTable;
        DataTableWindowToolbar m_Toolbar;
        DataTableWindowView m_View;
        bool m_Bound;


        void OnEnable()
        {
            ResourcesProvider.SetupSharedStylesheets(rootVisualElement);
            ResourcesProvider.SetupStylesheet("GDXTableWindow", rootVisualElement);
            ResourcesProvider.GetVisualTreeAsset("GDXTableWindow").CloneTree(rootVisualElement);
            ResourcesProvider.CheckTheme(rootVisualElement);

            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);

            // Build out our window controls
            m_Toolbar = new DataTableWindowToolbar(rootVisualElement.Q<Toolbar>("gdx-table-toolbar"), this);
            m_Overlay = new DataTableWindowOverlay(rootVisualElement.Q<VisualElement>("gdx-table-overlay"), this);
            m_Controller = new DataTableWindowController(this, m_Overlay);

            // Catch domain reload and rebind/relink window
            if (m_DataTable != null)
            {
                BindTable(m_DataTable, true);
            }

            EditorApplication.delayCall += CheckForNoTable;
        }

        void OnDestroy()
        {
            if (m_Bound)
            {
                DataTableTracker.UnregisterStructuralChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterCellValueChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterUndoRedoEvent(this, m_DataTableTicket);
                DataTableTracker.RemoveUsage(m_DataTableTicket);
            }

            if (m_DataTable != null)
            {
                Save();
            }
            DataTableWindowProvider.UnregisterTableWindow(this);
        }

        void OnKeyboardEvent(KeyDownEvent evt)
        {
            DataTableWindowOverlay.OverlayState state = m_Overlay.GetPrimaryState();
            // Escape to cancel overlay
            if (evt.keyCode == KeyCode.Escape && state != DataTableWindowOverlay.OverlayState.Hide)
            {
                m_Overlay.SetOverlayStateHidden();
            }

            // Submit on enter
            if (evt.keyCode == KeyCode.Percent || evt.keyCode == KeyCode.Return)
            {
                switch (state)
                {
                    case DataTableWindowOverlay.OverlayState.AddColumn:
                        m_Overlay.SubmitAddColumn();
                        break;
                    case DataTableWindowOverlay.OverlayState.AddRow:
                        m_Overlay.SubmitAddRow();
                        break;
                    case DataTableWindowOverlay.OverlayState.RenameRow:
                    case DataTableWindowOverlay.OverlayState.RenameColumn:
                        m_Overlay.SubmitRename();
                        break;
                    case DataTableWindowOverlay.OverlayState.Settings:
                        m_Overlay.SubmitSettings();
                        break;
                    case DataTableWindowOverlay.OverlayState.Confirmation:
                        m_Overlay.SubmitConfirmation();
                        break;
                }
            }
        }

        public DataTableWindowController GetController()
        {
            return m_Controller;
        }

        public DataTableBase GetDataTable()
        {
            return m_DataTable;
        }

        public int GetDataTableTicket()
        {
            return m_DataTableTicket;
        }

        internal DataTableWindowToolbar GetToolbar()
        {
            return m_Toolbar;
        }

        public DataTableWindowView GetView()
        {
            return m_View;
        }

        void CheckForNoTable()
        {
            if (m_DataTable == null)
            {
                Close();
            }
        }

        public void Save(bool skipDialog = false)
        {
            // We're not dirty, or were in batch mode
            if (!EditorUtility.IsDirty(m_DataTable) || Application.isBatchMode)
            {
                return;
            }


            if (skipDialog || EditorUtility.DisplayDialog($"Save {m_DataTable.GetDisplayName()}", "There are changes made to the table (in memory) which have not been saved to disk. Would you like to write those changes to disk now?", "Yes", "No"))
            {
                AssetDatabase.SaveAssetIfDirty(m_DataTable);
            }
        }


        public void BindTable(DataTableBase dataTable, bool fromDomainReload = false)
        {
            m_DataTable = dataTable;
            m_DataTableTicket = fromDomainReload ? DataTableTracker.RegisterTableAfterReload(dataTable, m_DataTableTicket) : DataTableTracker.RegisterTable(dataTable);

            DataTableWindowProvider.RegisterTableWindow(this, m_DataTable);

            RebindTable();

            if (m_Bound)
            {
                DataTableTracker.UnregisterStructuralChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterCellValueChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterUndoRedoEvent(this, m_DataTableTicket);

                // We need to handle not unregistering things when on domain reload so that the count doesnt go negative.
                if (!fromDomainReload)
                {
                    DataTableTracker.RemoveUsage(m_DataTableTicket);
                }
            }

            DataTableTracker.RegisterStructuralChanged(this, m_DataTableTicket);
            DataTableTracker.RegisterCellValueChanged(this, m_DataTableTicket);
            DataTableTracker.RegisterUndoRedoEvent(this, m_DataTableTicket);
            DataTableTracker.AddUsage(m_DataTableTicket);

            m_Bound = true;
        }

        public bool IsBound()
        {
            return m_Bound;
        }

        public void RebindTable()
        {
            titleContent = new GUIContent(m_DataTable.GetDisplayName());

            int columnCount = m_DataTable.GetColumnCount();
            if (columnCount == 0)
            {
                m_View?.Hide();
                m_Controller.ShowAddColumnDialog();
                return;
            }


            m_View?.Destroy();


            // Build our view out
            m_View = new DataTableWindowView(rootVisualElement[0], this);

            // Next frame resize things
            EditorApplication.delayCall += m_Controller.AutoResizeColumns;
        }

        /// <inheritdoc />
        public void OnColumnDefinitionChange(int columnIdentifier)
        {
            RebindTable();
        }

        /// <inheritdoc />
        public void OnCellValueChanged(int rowIdentifier, int columnIdentifier)
        {
            int orderedIndex = m_DataTable.GetRowOrder(rowIdentifier);
            m_View.GetMultiColumnListView().RefreshItem(orderedIndex);
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange(int rowIdentifier)
        {
            m_View.RebuildRowData();
        }

        public void OnSettingsChange()
        {
            titleContent = new GUIContent(m_DataTable.GetDisplayName());
        }

        public void OnUndoRedoRowDefinitionChange(int rowIdentifier)
        {
            m_View.RebuildRowData();
        }

        public void OnUndoRedoColumnDefinitionChange(int columnIdentifier)
        {
            RebindTable();
        }

        public void OnUndoRedoCellValueChanged(int rowIdentifier, int columnIdentifier)
        {
            int orderedIndex = m_DataTable.GetRowOrder(rowIdentifier);
            m_View.GetMultiColumnListView().RefreshItem(orderedIndex);
        }

        public void OnUndoRedoSettingsChanged()
        {
            titleContent = new GUIContent(m_DataTable.GetDisplayName());
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}