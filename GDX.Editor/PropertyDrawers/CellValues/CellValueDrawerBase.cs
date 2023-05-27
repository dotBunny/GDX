// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Editor.Windows.Tables;
using GDX.Tables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
#if !UNITY_2022_2_OR_NEWER
using UnityEngine;
#endif

namespace GDX.Editor.PropertyDrawers.CellValues
{
    public abstract class CellValueDrawerBase : PropertyDrawer
    {
        protected const string k_CellFieldName = "gdx-table-inspector-field";

        protected SerializedProperty m_SerializedProperty;
        protected SerializedProperty m_TableProperty;
        protected SerializedProperty m_RowProperty;
        protected SerializedProperty m_ColumnProperty;


        AssetDatabaseReference[] m_Tables;
        VisualElement m_Container;
        ToolbarBreadcrumbs m_Breadcrumbs;

        PopupField<int> m_SelectionPopup;
        List<int> m_SelectionPopupChoices;

        TableBase.RowDescription[] m_RowDescriptions;
        TableBase.ColumnDescription[] m_ColumnDescriptions;

        VisualElement m_CellElement;

        protected TableBase m_Table;
        protected int m_RowInternalIndex = -1;
        protected int m_ColumnInternalIndex = -1;


        protected abstract Serializable.SerializableTypes GetSupportedType();

        protected abstract VisualElement GetCellElement();

        void SaveSelection()
        {
            SerializedProperty tableProperty = m_SerializedProperty.FindPropertyRelative("Table");
            tableProperty.objectReferenceValue = m_Table;
            tableProperty.objectReferenceInstanceIDValue = m_Table.GetInstanceID();
            SerializedProperty rowProperty = m_SerializedProperty.FindPropertyRelative("Row");
            rowProperty.intValue = m_RowInternalIndex;
            SerializedProperty columnProperty = m_SerializedProperty.FindPropertyRelative("Column");
            columnProperty.intValue = m_ColumnInternalIndex;
            m_SerializedProperty.serializedObject.ApplyModifiedProperties();
        }

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
            m_SerializedProperty = property;

            // Load Data
            m_TableProperty = m_SerializedProperty.FindPropertyRelative("Table");
            m_Table = (TableBase)m_TableProperty.objectReferenceValue;

            if (m_Table != null)
            {
                m_RowProperty = m_SerializedProperty.FindPropertyRelative("Row");
                m_RowInternalIndex = m_RowProperty.intValue;

                m_ColumnProperty = m_SerializedProperty.FindPropertyRelative("Column");
                m_ColumnInternalIndex = m_ColumnProperty.intValue;
            }

            m_Container = new VisualElement();
            m_Breadcrumbs = new ToolbarBreadcrumbs();
            m_Container.Add(m_Breadcrumbs);
            UpdateDisplayMode();
            return m_Container;
        }

        protected void NotifyOfChange()
        {
            // TODO: Needs to target just a cell?
            TableWindow window = TableWindowProvider.GetTableWindow(m_Table);
            if (window != null)
            {
                window.GetView().RefreshItems();
            }
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
                m_Table = (TableBase)m_Tables[arg].GetOrLoadAsset();
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

                // Apply Properties
                SaveSelection();

                EditorUtility.SetDirty(m_SerializedProperty.serializedObject.targetObject);

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
                    if (m_Table.GetRowCount() == 0)
                    {
                        m_Container.Add(new Label("Table has no rows"));
                    }
                    else
                    {


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
                    }
                    break;
                case DisplayMode.SelectColumn:
                    if (m_RowInternalIndex == -1)
                    {
                        SetDisplayMode(DisplayMode.SelectTable);
                        return;
                    }

                    m_RowDescriptions = null;

                    if (m_Table.GetColumnCount() == 0)
                    {
                        m_Container.Add(new Label("Table has no column"));
                    }
                    else
                    {
                        // Get table columns
                        TableBase.ColumnDescription[] allColumns = m_Table.GetAllColumnDescriptions();
                        int allColumnCount = allColumns.Length;
                        Serializable.SerializableTypes requiredType = GetSupportedType();
                        List<TableBase.ColumnDescription> validColumns =
                            new List<TableBase.ColumnDescription>(allColumns.Length);

                        for (int i = 0; i < allColumnCount; i++)
                        {
                            if (allColumns[i].Type == requiredType)
                            {
                                validColumns.Add(allColumns[i]);
                            }
                        }

                        //Filtered
                        m_ColumnDescriptions = validColumns.ToArray();
                        int columnCount = m_ColumnDescriptions.Length;
                        if (columnCount == 0)
                        {
                            m_Container.Add(new Label("No " + requiredType.ToString() + " columns found"));

                        }
                        else
                        {

                            m_SelectionPopupChoices = new List<int>(columnCount + 1) { -1 };
                            for (int i = 0; i < columnCount; i++)
                            {
                                m_SelectionPopupChoices.Add(i);
                            }
                            m_SelectionPopup = new PopupField<int>("Column", m_SelectionPopupChoices, 0,
                                OnColumnSelected,
                                FormatColumnSelectionItem);
                            m_Container.Add(m_SelectionPopup);
                        }
                    }

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