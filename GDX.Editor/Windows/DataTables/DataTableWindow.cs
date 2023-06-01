using GDX.DataTables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    public class DataTableWindow : EditorWindow, DataTableTracker.IColumnDefinitionChangeCallbackReceiver, DataTableTracker.ICellValueChangedCallbackReceiver, DataTableTracker.IRowDefinitionChangeCallbackReceiver
    {
        int m_DataTableTicket;
        TableWindowController m_Controller;
        TableWindowOverlay m_Overlay;

        DataTableObject m_DataTable;


        TableWindowToolbar m_Toolbar;
        TableWindowView m_View;
        DataTableTracker.ICellValueChangedCallbackReceiver m_ICellValueChangedCallbackReceiverImplementation;
        bool m_Bound = false;

        void OnEnable()
        {
            ResourcesProvider.SetupSharedStylesheets(rootVisualElement);
            ResourcesProvider.SetupStylesheet("GDXTableWindow", rootVisualElement);
            ResourcesProvider.GetVisualTreeAsset("GDXTableWindow").CloneTree(rootVisualElement);
            ResourcesProvider.CheckTheme(rootVisualElement);

            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyboardEvent);

            // Build out our window controls
            m_Toolbar = new TableWindowToolbar(rootVisualElement.Q<Toolbar>("gdx-table-toolbar"), this);
            m_Overlay = new TableWindowOverlay(rootVisualElement.Q<VisualElement>("gdx-table-overlay"), this);
            m_Controller = new TableWindowController(this, m_Overlay);

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
                DataTableTracker.UnregisterColumnChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterRowChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterCellValueChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterUsage(m_DataTableTicket);
            }

            if (m_DataTable != null)
            {
                Save();
            }
            TableWindowProvider.UnregisterTableWindow(this);
        }

        void OnKeyboardEvent(KeyDownEvent evt)
        {
            TableWindowOverlay.OverlayState state = m_Overlay.GetPrimaryState();
            // Escape to cancel overlay
            if (evt.keyCode == KeyCode.Escape && state != TableWindowOverlay.OverlayState.Hide)
            {
                m_Overlay.SetOverlayStateHidden();
            }

            // Submit on enter
            if (evt.keyCode == KeyCode.Percent || evt.keyCode == KeyCode.Return)
            {
                switch (state)
                {
                    case TableWindowOverlay.OverlayState.AddColumn:
                        m_Overlay.SubmitAddColumn();
                        break;
                    case TableWindowOverlay.OverlayState.AddRow:
                        m_Overlay.SubmitAddRow();
                        break;
                    case TableWindowOverlay.OverlayState.RenameRow:
                    case TableWindowOverlay.OverlayState.RenameColumn:
                        m_Overlay.SubmitRename();
                        break;
                    case TableWindowOverlay.OverlayState.Settings:
                        m_Overlay.SubmitSettings();
                        break;
                    case TableWindowOverlay.OverlayState.Confirmation:
                        m_Overlay.SubmitConfirmation();
                        break;
                }
            }
        }

        public TableWindowController GetController()
        {
            return m_Controller;
        }

        public DataTableObject GetDataTable()
        {
            return m_DataTable;
        }

        public int GetDataTableTicket()
        {
            return m_DataTableTicket;
        }

        internal TableWindowToolbar GetToolbar()
        {
            return m_Toolbar;
        }

        public TableWindowView GetView()
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
                m_Toolbar.UpdateSaveButton();
            }
        }


        public void BindTable(DataTableObject dataTable, bool fromDomainReload = false)
        {
            m_DataTable = dataTable;
            m_DataTableTicket = fromDomainReload ? DataTableTracker.RegisterTable(dataTable, m_DataTableTicket) : DataTableTracker.RegisterTable(dataTable);

            TableWindowProvider.RegisterTableWindow(this, m_DataTable);

            RebindTable();

            if (m_Bound)
            {
                DataTableTracker.UnregisterColumnChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterRowChanged(this, m_DataTableTicket);
                DataTableTracker.UnregisterCellValueChanged(this, m_DataTableTicket);

                // We need to handle not unregistering things when on domain reload so that the count doesnt go negative.
                if (!fromDomainReload)
                {
                    DataTableTracker.UnregisterUsage(m_DataTableTicket);
                }
            }

            DataTableTracker.RegisterColumnChanged(this, m_DataTableTicket);
            DataTableTracker.RegisterRowChanged(this, m_DataTableTicket);
            DataTableTracker.RegisterCellValueChanged(this, m_DataTableTicket);
            DataTableTracker.RegisterUsage(m_DataTableTicket);

            m_Bound = true;
            m_Toolbar?.UpdateSaveButton();
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
            m_View = new TableWindowView(rootVisualElement[0], this);

            // Next frame resize things
            EditorApplication.delayCall += m_Controller.AutoResizeColumns;
        }

        /// <inheritdoc />
        public void OnColumnDefinitionChange()
        {
            RebindTable();
            m_Toolbar.UpdateSaveButton();
        }

        /// <inheritdoc />
        public void OnCellValueChanged(int rowIdentifier, int columnIdentifier)
        {
            // We can do better then this, what if cells actually had more awareness
            // TODO: @matt Need to do better here
            m_View.RefreshItems();
            m_Toolbar.UpdateSaveButton();
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange()
        {
            m_View.RebuildRowData();
            m_Toolbar.UpdateSaveButton();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}