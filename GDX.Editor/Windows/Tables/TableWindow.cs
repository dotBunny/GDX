using System;
using System.Collections.Generic;
using GDX.Editor.Inspectors;
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
        // TODO:
        // - Settings
        // - Confirmation mode
        // - Rename columns bug fix
        // - sorting
        // - selection in objects

        internal TableWindowToolbar m_Toolbar;
        internal TableWindowOverlay m_Overlay;
        internal TableWindowView m_View;
        internal TableWindowController m_Controller;

        ITable m_TargetTable;
        ScriptableObject m_ScriptableObject;

        void OnEnable()
        {
            ResourcesProvider.SetupStylesheets(rootVisualElement);
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

            if (m_TargetTable != null)
            {
                TableWindowProvider.UnregisterTable(m_TargetTable);
            }
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
                        m_Overlay.SubmitAddColumn(); ;
                        break;
                    case TableWindowOverlay.OverlayState.AddRow:
                        m_Overlay.SubmitAddRow();
                        break;
                    case TableWindowOverlay.OverlayState.RenameRow:
                    case TableWindowOverlay.OverlayState.RenameColumn:
                        m_Overlay.SubmitRename();
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
        public ITable GetTable()
        {
            return m_TargetTable;
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



        public void BindTable(ITable table)
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