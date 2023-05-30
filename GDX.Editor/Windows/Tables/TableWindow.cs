using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindow : EditorWindow, TableCache.IColumnDefinitionChangeCallbackReceiver, TableCache.ICellValueChangedCallbackReceiver, TableCache.IRowDefinitionChangeCallbackReceiver
    {
        int m_TableTicket;
        TableWindowController m_Controller;
        TableWindowOverlay m_Overlay;

        TableBase m_TargetTable;


        TableWindowToolbar m_Toolbar;
        TableWindowView m_View;
        TableCache.ICellValueChangedCallbackReceiver m_ICellValueChangedCallbackReceiverImplementation;
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
            if (m_TargetTable != null)
            {
                BindTable(m_TargetTable, true);
            }

            EditorApplication.delayCall += CheckForNoTable;
        }

        void OnDestroy()
        {
            if (m_Bound)
            {
                TableCache.UnregisterColumnChanged(this, m_TableTicket);
                TableCache.UnregisterRowChanged(this, m_TableTicket);
                TableCache.UnregisterCellValueChanged(this, m_TableTicket);
                TableCache.UnregisterUsage(m_TableTicket);
            }

            if (m_TargetTable != null)
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

        public TableBase GetTable()
        {
            return m_TargetTable;
        }

        public int GetTableTicket()
        {
            return m_TableTicket;
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
            if (m_TargetTable == null)
            {
                Close();
            }
        }

        public void Save(bool skipDialog = false)
        {
            if (!EditorUtility.IsDirty(m_TargetTable))
            {
                return;
            }

            if (skipDialog || EditorUtility.DisplayDialog($"Save {m_TargetTable.GetDisplayName()}", "There are changes made to the table (in memory) which have not been saved to disk. Would you like to write those changes to disk now?", "Yes", "No"))
            {
                AssetDatabase.SaveAssetIfDirty(m_TargetTable);
                m_Toolbar.UpdateSaveButton();
            }
        }


        public void BindTable(TableBase table, bool fromDomainReload = false)
        {
            m_TargetTable = table;
            m_TableTicket = fromDomainReload ? TableCache.RegisterTable(table, m_TableTicket) : TableCache.RegisterTable(table);

            TableWindowProvider.RegisterTableWindow(this, m_TargetTable);

            RebindTable();

            if (m_Bound)
            {
                TableCache.UnregisterColumnChanged(this, m_TableTicket);
                TableCache.UnregisterRowChanged(this, m_TableTicket);
                TableCache.UnregisterCellValueChanged(this, m_TableTicket);

                // We need to handle not unregistering things when on domain reload so that the count doesnt go negative.
                if (!fromDomainReload)
                {
                    TableCache.UnregisterUsage(m_TableTicket);
                }
            }

            TableCache.RegisterColumnChanged(this, m_TableTicket);
            TableCache.RegisterRowChanged(this, m_TableTicket);
            TableCache.RegisterCellValueChanged(this, m_TableTicket);
            TableCache.RegisterUsage(m_TableTicket);

            m_Bound = true;
            m_Toolbar?.UpdateSaveButton();
        }

        public bool IsBound()
        {
            return m_Bound;
        }

        public void RebindTable()
        {
            titleContent = new GUIContent(m_TargetTable.GetDisplayName());

            int columnCount = m_TargetTable.GetColumnCount();
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
        public void OnCellValueChanged(int rowInternalIndex, int columnInternalIndex)
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
#endif
}