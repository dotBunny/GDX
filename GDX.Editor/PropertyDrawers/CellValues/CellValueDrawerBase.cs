// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#if !UNITY_2022_2_OR_NEWER
using UnityEngine;
#endif

namespace GDX.Editor.PropertyDrawers.CellValues
{
    public abstract class CellValueDrawerBase : PropertyDrawer
    {
        protected const string k_CellFieldName = "gdx-table-inspector-field";

        UnityEngine.Object m_TargetObject;
        AssetDatabaseReference[] m_Tables;
        VisualElement m_Container;
        ToolbarBreadcrumbs m_Breadcrumbs;

        PopupField<int> m_SelectionPopup;
        List<int> m_SelectionPopupChoices;

        ITable.RowDescription[] m_RowDescriptions;
        ITable.ColumnDescription[] m_ColumnDescriptions;

        VisualElement m_CellElement;

        protected ITable m_Table;
        protected int m_RowInternalIndex = -1;
        protected int m_ColumnInternalIndex = -1;



        protected abstract void Init(SerializedProperty serializedProperty);
        protected abstract void CopyFromInspector(ITable table, int rowInternalIndex, int columnInternalIndex);
        protected abstract void CopyToInspector();

        protected abstract VisualElement GetCellElement();

        // ITable m_Table;
        // int m_columnInternalIndex;
        // int m_rowInternalIndex;
        // public override

#if UNITY_2022_2_OR_NEWER


        void UpdateDisplayMode()
        {
            // If we dont have a table we need to select one
            if (m_Table == null && m_RowInternalIndex == -1 && m_ColumnInternalIndex == -1)
            {
                SetDisplayMode(DisplayMode.SelectTable);
            }
            else if (m_Table != null && m_RowInternalIndex == -1)
            {
                SetDisplayMode(DisplayMode.SelectRow);
            }
            else if (m_Table != null && m_RowInternalIndex != -1 && m_ColumnInternalIndex == -1)
            {
                SetDisplayMode(DisplayMode.SelectColumn);
            }
            else if (m_Table != null && m_RowInternalIndex != -1 && m_ColumnInternalIndex != -1)
            {
                SetDisplayMode(DisplayMode.DisplayValue);
            }
            else
            {
                SetDisplayMode(DisplayMode.SelectTable);
            }
        }

        /// <summary>
        /// Overrides the method to make a UIElements based GUI for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <returns>A disabled visual element.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            m_TargetObject = property.serializedObject.targetObject;

            Init(property);
            m_Container = new VisualElement();
            m_Breadcrumbs = new ToolbarBreadcrumbs();
            m_Container.Add(m_Breadcrumbs);
            UpdateDisplayMode();
            return m_Container;
        }

        protected void NotifyOfChange()
        {

        }

        string FormatTableSelectionItem(int arg)
        {
            return arg == -1 ? "Select Table" : m_Tables[arg].GetPath().Replace(".asset", "");
        }
        string FormatRowSelectionItem(int arg)
        {
            return arg == -1 ? "Select Row" : m_RowDescriptions[arg].Name;
        }
        string FormatColumnSelectionItem(int arg)
        {
            return arg == -1 ? "Select Row" : m_ColumnDescriptions[arg].Name;
        }

        string OnTableSelected(int arg)
        {
            if (arg >= 0)
            {
                m_Table = (ITable)m_Tables[arg].GetOrLoadAsset();
                m_Breadcrumbs.Add(new Label(m_Tables[arg].GetFileNameWithoutExtension()));
                UpdateDisplayMode();
                return m_Tables[arg].GetPathWithoutExtension();
            }
            return "Select Table";
        }

        string OnRowSelected(int arg)
        {
            if (arg >= 0)
            {
                m_RowInternalIndex = m_RowDescriptions[arg].InternalIndex;
                m_Breadcrumbs.Add(new Label(m_RowDescriptions[arg].Name));
                UpdateDisplayMode();
                return "Updating ...";
            }
            return "Select Row";
        }

        string OnColumnSelected(int arg)
        {
            if (arg >= 0)
            {
                m_ColumnInternalIndex = m_ColumnDescriptions[arg].InternalIndex;
                m_Breadcrumbs.Add(new Label(m_ColumnDescriptions[arg].Name));

                // Save?
                CopyFromInspector(m_Table, m_RowInternalIndex, m_ColumnInternalIndex);
                EditorUtility.SetDirty(m_TargetObject);

                UpdateDisplayMode();
                return "Updating ...";
            }
            return "Select Column";
        }




        enum DisplayMode
        {
            SelectTable,
            SelectRow,
            SelectColumn,
            DisplayValue
        }

        void SetDisplayMode(DisplayMode displayMode)
        {
            if (m_Container == null) return;

            // Clear out existing content
            for (int i = 1; i < m_Container.childCount; i++)
            {
                m_Container.RemoveAt(i);
            }

            switch (displayMode)
            {
                case DisplayMode.SelectTable:
                    m_Tables = TableCache.FindTables();

                    int count = m_Tables.Length;

                    // Create choices list, cause we uses lists for these things
                    m_SelectionPopupChoices = new List<int>(count + 1) { -1 };
                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            m_SelectionPopupChoices.Add(i);
                        }
                    }
                    m_SelectionPopup = new PopupField<int>("Table", m_SelectionPopupChoices, 0, OnTableSelected,
                        FormatTableSelectionItem);

                    m_Container.Add(m_SelectionPopup);
                    break;
                case DisplayMode.SelectRow:
                    // Get table rows
                    m_RowDescriptions = m_Table.GetAllRowDescriptions();
                    int rowCount = m_RowDescriptions.Length;
                    m_SelectionPopupChoices = new List<int>(rowCount + 1) { -1 };
                    if (rowCount > 0)
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            m_SelectionPopupChoices.Add(i);
                        }
                    }
                    m_SelectionPopup = new PopupField<int>("Row", m_SelectionPopupChoices, 0, OnRowSelected,
                        FormatRowSelectionItem);
                    m_Container.Add(m_SelectionPopup);

                    break;
                case DisplayMode.SelectColumn:
                    if (m_RowInternalIndex == -1)
                    {
                        SetDisplayMode(DisplayMode.SelectTable);
                        return;
                    }

                    m_RowDescriptions = null;

                    // Get table rows
                    m_ColumnDescriptions = m_Table.GetAllColumnDescriptions();
                    int columnCount = m_ColumnDescriptions.Length;
                    m_SelectionPopupChoices = new List<int>(columnCount + 1) { -1 };
                    if (columnCount > 0)
                    {
                        for (int i = 0; i < columnCount; i++)
                        {
                            m_SelectionPopupChoices.Add(i);
                        }
                    }
                    m_SelectionPopup = new PopupField<int>("Column", m_SelectionPopupChoices, 0, OnColumnSelected,
                        FormatColumnSelectionItem);
                    m_Container.Add(m_SelectionPopup);
                    break;
                case DisplayMode.DisplayValue:
                    if (m_ColumnInternalIndex == -1)
                    {
                        SetDisplayMode(DisplayMode.SelectTable);
                        return;
                    }
                    m_RowDescriptions = null;
                    m_ColumnDescriptions = null;
                    m_Container.Add(GetCellElement());
                    break;
            }



        }


#else
        /// <summary>
        ///     Unity IMGUI Draw Event
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
#endif
    }
}