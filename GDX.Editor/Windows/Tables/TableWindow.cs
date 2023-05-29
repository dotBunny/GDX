using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    public class TableWindow : EditorWindow
    {
        TableWindowController m_Controller;
        TableWindowOverlay m_Overlay;
        ScriptableObject m_ScriptableObject;

        TableBase m_TargetTable;


        TableWindowToolbar m_Toolbar;
        TableWindowView m_View;

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
                TableWindowProvider.RegisterTableWindow(this, m_TargetTable);
                BindTable(m_TargetTable);
            }

            EditorApplication.delayCall += CheckForNoTable;
        }

        void OnDestroy()
        {
            if (m_ScriptableObject != null)
            {
                // TODO: do we need to dirty this if its not a SO
                AssetDatabase.SaveAssetIfDirty(m_ScriptableObject);
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

        public ScriptableObject GetScriptableObject()
        {
            return m_ScriptableObject;
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
            titleContent = new GUIContent(m_TargetTable.GetDisplayName());
            if (m_TargetTable is ScriptableObject targetTable)
            {
                m_ScriptableObject = targetTable;
            }

            int columnCount = table.GetColumnCount();
            if (columnCount == 0)
            {
                m_View?.Hide();
                m_Controller.ShowAddColumnDialog();
                return;
            }

            // Build our view out
            m_View = new TableWindowView(rootVisualElement[0], this);

            // Next frame resize things
            EditorApplication.delayCall += m_Controller.AutoResizeColumns;
        }
    }
#endif
}