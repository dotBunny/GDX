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
                BindTable(m_TargetTable);
            }

            EditorApplication.delayCall += CheckForNoTable;
        }

        void OnDestroy()
        {
            if (m_TargetTable != null)
            {
                AssetDatabase.SaveAssetIfDirty(m_TargetTable);
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

        internal TableWindowToolbar GetToolbar()
        {
            return m_Toolbar;
        }

        public TableWindowView GetView()
        {
            return m_View;
        }

        public int GetTableTicket()
        {
            return m_TableTicket;
        }


        void CheckForNoTable()
        {
            if (m_TargetTable == null)
            {
                Close();
            }
        }


        public void BindTable(TableBase table)
        {
            m_TargetTable = table;
            m_TableTicket = TableCache.RegisterTable(table);
            TableWindowProvider.RegisterTableWindow(this, m_TargetTable);

            RebindTable();

            TableCache.RegisterColumnChanged(this, m_TableTicket);
            TableCache.RegisterRowChanged(this, m_TableTicket);
            TableCache.RegisterCellValueChanged(this, m_TableTicket);
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
        }

        /// <inheritdoc />
        public void OnCellValueChanged(int rowInternalIndex, int columnInternalIndex)
        {
            // We can do better then this, what if cells actually had more awareness
            m_View.RefreshItems();
        }

        /// <inheritdoc />
        public void OnRowDefinitionChange()
        {
            RebindTable();
            //m_View.RebuildRowData();
        }
    }
#endif
}