// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Editor.Windows.Tables;
using GDX.Tables;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers.CellValues
{
#if UNITY_2022_2_OR_NEWER
    public abstract class CellValueDrawerBase : PropertyDrawer
    {
        const string k_MessageClickToUnlock = "Click to unlock for editting.";
        const string k_MessageClickToLock = "Click to lock data.";
        const string k_MessageNoTableSelected = "No table selected.";
        const string k_PropertyRow = "Row";
        const string k_PropertyColumn = "Column";
        const string k_PropertyTable = "Table";

        const string k_StyleClassLinked = "linked";
        const string k_StyleClassUnlinked = "unlinked";
        const string k_StyleClassLocked = "locked";
        const string k_StyleClassUnlocked = "unlocked";

        protected const string k_CellFieldName = "gdx-table-inspector-field";

        VisualElement m_CellElement;
        TableBase.ColumnDescription[] m_ColumnDescriptions;
        protected int m_ColumnInternalIndex = -1;
        SerializedProperty m_ColumnProperty;

        VisualElement m_Container;
        Label m_FieldLabel;

        bool m_IsUnlocked;
        TableBase.RowDescription[] m_RowDescriptions;
        protected int m_RowInternalIndex = -1;
        SerializedProperty m_RowProperty;

        SerializedProperty m_SerializedProperty;

        protected TableBase m_Table;
        Button m_TableButton;
        SerializedProperty m_TableProperty;

        AssetDatabaseReference[] m_Tables;
        VisualElement m_ValueContainer;


        protected abstract Serializable.SerializableTypes GetSupportedType();

        protected abstract VisualElement GetCellElement();
        protected abstract ulong GetDataVersion();


        /// <summary>
        ///     Overrides the method to make a UIElements based GUI for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <returns>A disabled visual element.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Cache base reference
            m_SerializedProperty = property;

            // Reference serialization properties
            m_TableProperty = m_SerializedProperty.FindPropertyRelative(k_PropertyTable);
            m_RowProperty = m_SerializedProperty.FindPropertyRelative(k_PropertyRow);
            m_ColumnProperty = m_SerializedProperty.FindPropertyRelative(k_PropertyColumn);

            // Load Data
            m_Table = (TableBase)m_TableProperty.objectReferenceValue;
            if (m_Table != null)
            {
                m_RowInternalIndex = m_RowProperty.intValue;
                m_ColumnInternalIndex = m_ColumnProperty.intValue;
            }

            // Build our base level inspector
            m_Container = new VisualElement { name = "gdx-cell-value", style = { flexDirection = FlexDirection.Row } };
            ResourcesProvider.SetupSharedStylesheets(m_Container);
            ResourcesProvider.SetupStylesheet("GDXCellValueDrawer", m_Container);
            m_Container.AddToClassList("unity-base-field");
            m_Container.AddToClassList("unity-base-field__inspector-field");
            m_Container.AddToClassList("unity-base-field__aligned");
            m_Container.AddToClassList("unity-property-field");
            m_Container.AddToClassList("unity-property-field__inspector-property");
            m_Container.AddToClassList("unity-input-field");


            VisualElement fieldLabelContainer = new VisualElement
            {
                style =
                {
                    maxWidth = new StyleLength(new Length(42f, LengthUnit.Percent)),
                    flexGrow = 1,
                    flexDirection = FlexDirection.Row
                }
            };
            m_FieldLabel = new Label(property.name) { name = "gdx-cell-value-label" };
            m_FieldLabel.AddToClassList("unity-text-element");
            m_FieldLabel.AddToClassList("unity-label");
            m_FieldLabel.AddToClassList("unity-base-field__label");
            m_FieldLabel.AddToClassList("unity-input-field__label");
            m_FieldLabel.AddToClassList("unity-property-field__label");


            fieldLabelContainer.Add(m_FieldLabel);

            // Add spacer
            fieldLabelContainer.Add(new VisualElement { name = "gdx-cell-value-spacer" });

            m_TableButton = new Button(TableLinkStatusClicked) { name = "gdx-cell-value-table" };
            fieldLabelContainer.Add(m_TableButton);

            m_Container.Add(fieldLabelContainer);

            m_ValueContainer = new VisualElement { name = "gdx-cell-value-container" };
            m_ValueContainer.AddToClassList("unity-base-field__input");
            m_ValueContainer.AddToClassList("unity-text-field__input");
            m_ValueContainer.AddToClassList("unity-property-field__input");

            m_Container.Add(m_ValueContainer);

            // Determine what view should be shown based on available data
            DetectDrawerMode();

            return m_Container;
        }

        void UpdateTableLinkStatus()
        {
            if (m_Table != null && m_RowInternalIndex != -1 && m_ColumnInternalIndex != -1)
            {
                m_TableButton.AddToClassList(k_StyleClassLinked);
                m_TableButton.RemoveFromClassList(k_StyleClassUnlinked);
                m_TableButton.tooltip =
                    $"Table: {m_Table.GetDisplayName()}\nRow: {m_Table.GetRowName(m_RowInternalIndex)} ({m_RowInternalIndex})\nColumn: {m_Table.GetColumnName(m_ColumnInternalIndex)} ({m_ColumnInternalIndex})\nData Version: {GetDataVersion()}\n\nClick to reset link.";
            }
            else
            {
                m_TableButton.RemoveFromClassList(k_StyleClassLinked);
                m_TableButton.AddToClassList(k_StyleClassUnlinked);

                if (m_Table == null)
                {
                    m_TableButton.tooltip = k_MessageNoTableSelected;
                }
                else if (m_RowInternalIndex == -1)
                {
                    m_TableButton.tooltip =
                        $"Table: {m_Table.GetDisplayName()}\nRow: None selected.\n\nClick to reset link.";
                }
                else if (m_ColumnInternalIndex == -1)
                {
                    m_TableButton.tooltip =
                        $"Table: {m_Table.GetDisplayName()}\nRow: {m_Table.GetRowName(m_RowInternalIndex)} ({m_RowInternalIndex})\nColumn: None selected.\n\nClick to reset link.";
                }
                else
                {
                    m_TableButton.tooltip = string.Empty;
                }
            }
        }

        void TableLinkStatusClicked()
        {
            m_Table = null;
            m_RowInternalIndex = -1;
            m_ColumnInternalIndex = -1;
            SetDisplayMode(DisplayMode.SelectTable);
        }

        void ApplySettings()
        {
            m_TableProperty.objectReferenceValue = m_Table;
            m_TableProperty.objectReferenceInstanceIDValue = m_Table.GetInstanceID();
            m_RowProperty.intValue = m_RowInternalIndex;
            m_ColumnProperty.intValue = m_ColumnInternalIndex;

            m_SerializedProperty.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_SerializedProperty.serializedObject.targetObject);
        }

        void DetectDrawerMode()
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
                //m_Breadcrumbs.Add(new Label(m_Tables[arg].GetFileNameWithoutExtension()));
                DetectDrawerMode();
                return m_Tables[arg].GetPathWithoutExtension();
            }

            return "Select Table";
        }

        string OnRowSelected(int arg)
        {
            if (arg >= 0)
            {
                m_RowInternalIndex = m_RowDescriptions[arg].InternalIndex;
                //m_Breadcrumbs.Add(new Label(m_RowDescriptions[arg].Name));
                DetectDrawerMode();
                return "Updating ...";
            }

            return "Select Row";
        }

        string OnColumnSelected(int arg)
        {
            if (arg >= 0)
            {
                m_ColumnInternalIndex = m_ColumnDescriptions[arg].InternalIndex;
                //m_Breadcrumbs.Add(new Label(m_ColumnDescriptions[arg].Name));

                // Apply Properties
                ApplySettings();

                DetectDrawerMode();
                return "Updating ...";
            }

            return "Select Column";
        }

        VisualElement MakeSelectTableElement()
        {
            m_Tables = TableCache.FindTables();
            int count = m_Tables.Length;

            // Create choices list, cause we uses lists for these things
            List<int> choices = new List<int>(count + 1) { -1 };
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    choices.Add(i);
                }
            }

            return new PopupField<int>(null, choices, 0, OnTableSelected,
                FormatTableSelectionItem);
        }

        VisualElement MakeSelectRowElement()
        {
            if (m_Table.GetRowCount() == 0)
            {
                Debug.LogWarning($"The selected table '{m_Table.GetDisplayName()}' has no row data.");
                return null;
            }

            // Get table rows
            m_RowDescriptions = m_Table.GetAllRowDescriptions();
            int rowCount = m_RowDescriptions.Length;
            List<int> choices = new List<int>(rowCount + 1) { -1 };
            if (rowCount > 0)
            {
                for (int i = 0; i < rowCount; i++)
                {
                    choices.Add(i);
                }
            }

            return new PopupField<int>(choices, 0, OnRowSelected, FormatRowSelectionItem);
        }

        VisualElement MakeSelectColumnElement()
        {
            if (m_RowInternalIndex == -1 || m_Table == null || m_Table.GetColumnCount() == 0)
            {
                Debug.LogWarning("An error occured when trying to select a column. Please try again.");
                return null;
            }

            // Not needed any long?
            m_RowDescriptions = null;

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
                Debug.LogWarning($"No columns of {requiredType} are found in the table '{m_Table.GetDisplayName()}'.");
                return null;
            }

            List<int> choices = new List<int>(columnCount + 1) { -1 };
            for (int i = 0; i < columnCount; i++)
            {
                choices.Add(i);
            }

            return new PopupField<int>(choices, 0, OnColumnSelected, FormatColumnSelectionItem);
        }

        VisualElement MakeCellValueElement()
        {
            if (m_ColumnInternalIndex != -1)
            {
                m_RowDescriptions = null;
                m_ColumnDescriptions = null;

                VisualElement cellContainer = new VisualElement { style = { flexDirection = FlexDirection.Row } };


                VisualElement cellElement = GetCellElement();
                cellElement.AddToClassList("gdx-cell-field-inspector");
                Button lockButton = new Button(OnLockButtonClicked) { name = "gdx-cell-value-lock" };
                if (!m_IsUnlocked)
                {
                    cellElement.SetEnabled(false);
                    lockButton.AddToClassList(k_StyleClassLocked);
                    lockButton.tooltip = k_MessageClickToUnlock;
                }
                else
                {
                    lockButton.AddToClassList(k_StyleClassUnlocked);
                    lockButton.tooltip = k_MessageClickToLock;
                }

                cellContainer.Add(cellElement);
                cellContainer.Add(lockButton);

                return cellContainer;
            }

            return null;
        }


        void OnLockButtonClicked()
        {
            m_IsUnlocked = !m_IsUnlocked;
            SetDisplayMode(DisplayMode.DisplayValue);
        }

        void SetDisplayMode(DisplayMode displayMode)
        {
            // If we haven't built a container yet we really shouldn't even be here
            if (m_Container == null)
            {
                return;
            }

            // Remove existing value content because it will need to change based on the perspective mode
            int childCount = m_ValueContainer.childCount;
            if (m_ValueContainer.childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    m_ValueContainer.RemoveAt(i);
                }
            }

            // Get new content
            VisualElement contentElement = null;
            switch (displayMode)
            {
                case DisplayMode.SelectTable:
                    contentElement = MakeSelectTableElement();
                    break;
                case DisplayMode.SelectRow:
                    contentElement = MakeSelectRowElement();
                    break;
                case DisplayMode.SelectColumn:
                    contentElement = MakeSelectColumnElement();
                    break;
                case DisplayMode.DisplayValue:
                    contentElement = MakeCellValueElement();
                    break;
            }

            if (contentElement == null)
            {
                SetDisplayMode((DisplayMode)(int)displayMode - 1);
            }

            m_ValueContainer.Add(contentElement);
            UpdateTableLinkStatus();
        }


        enum DisplayMode
        {
            SelectTable,
            SelectRow,
            SelectColumn,
            DisplayValue
        }
    }
#endif
}