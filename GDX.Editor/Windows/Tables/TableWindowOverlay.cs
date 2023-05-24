// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.Tables
{
#if UNITY_2022_2_OR_NEWER
    class TableWindowOverlay
    {
        VisualElement rootElement;
        TableWindow parentWindow;

        Button m_AddColumnAddButton;
        Button m_AddColumnCancelButton;
        TextField m_AddColumnName;


        VisualElement m_AddColumnOverlay;
        PopupField<int> m_AddColumnType;
        VisualElement m_AddRowOverlay;
        Button m_AddRowAddButton;
        Button m_AddRowCancelButton;
        TextField m_AddRowName;
        Button m_RenameColumnCancelButton;
        TextField m_RenameColumnName;

        VisualElement m_RenameColumnOverlay;
        Button m_RenameColumnRenameButton;
        Button m_RenameRowCancelButton;
        TextField m_RenameRowName;

        VisualElement m_RenameRowOverlay;
        Button m_RenameRowRenameButton;

        OverlayState m_OverlayState;

        int cachedIndex = 0;

        public int GetCachedIndex()
        {
            return cachedIndex;
        }


        internal TableWindowOverlay(VisualElement element, TableWindow window)
        {
            rootElement = element;
            parentWindow = window;


               // Build out references for adding a column
            m_AddColumnOverlay = rootElement.Q<VisualElement>("gdx-table-add-column");
            m_AddColumnName = m_AddColumnOverlay.Q<TextField>("gdx-table-column-name");
            // Build our custom column type enum
            int columnNameIndex = m_AddColumnOverlay.IndexOf(m_AddColumnName);
            List<int> typeValues = new List<int>(Serializable.SerializableTypesCount);
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                typeValues.Add(i);
            }

            m_AddColumnType =
                new PopupField<int>(typeValues, 0, Serializable.GetSerializableTypesLabel,
                    Serializable.GetSerializableTypesLabel) { label = "Type", name = "gdx-table-column-type" };
            m_AddColumnOverlay.Insert(columnNameIndex + 1, m_AddColumnType);
            m_AddColumnAddButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-add");
            m_AddColumnAddButton.clicked += SubmitAddColumn;
            m_AddColumnCancelButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_AddColumnCancelButton.clicked += SetOverlayStateHidden;

            // Build out our Adding Rows
            m_AddRowOverlay = rootElement.Q<VisualElement>("gdx-table-add-row");
            m_AddRowName = m_AddRowOverlay.Q<TextField>("gdx-table-row-name");
            m_AddRowAddButton = m_AddRowOverlay.Q<Button>("gdx-table-row-add");
            m_AddRowAddButton.clicked += SubmitAddRow;
            m_AddRowCancelButton = m_AddRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_AddRowCancelButton.clicked += SetOverlayStateHidden;

            // Bind our Renaming Columns
            m_RenameColumnOverlay = rootElement.Q<VisualElement>("gdx-table-rename-column");
            m_RenameColumnName = m_RenameColumnOverlay.Q<TextField>("gdx-table-column-name");
            m_RenameColumnRenameButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-rename");
            m_RenameColumnRenameButton.clicked += SubmitRenameColumn;
            m_RenameColumnCancelButton = m_RenameColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_RenameColumnCancelButton.clicked += SetOverlayStateHidden;

            // Bind our Renaming Rows
            m_RenameRowOverlay = rootElement.Q<VisualElement>("gdx-table-rename-row");
            m_RenameRowName = m_RenameRowOverlay.Q<TextField>("gdx-table-row-name");
            m_RenameRowRenameButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-rename");
            m_RenameRowRenameButton.clicked += SubmitRenameRow;
            m_RenameRowCancelButton = m_RenameRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_RenameRowCancelButton.clicked += SetOverlayStateHidden;

            // Ensure state of everything
            SetState(OverlayState.Hide);

        }

        public OverlayState GetState()
        {
            return m_OverlayState;
        }

        internal void SetState(OverlayState state, int stableIndex = -1, string previousValue = null)
        {
            cachedIndex = stableIndex;

            // Handle focus
            if (state == OverlayState.Hide)
            {
                parentWindow.m_Toolbar.SetFocusable(true);
                rootElement.focusable = false;
                if (parentWindow.GetView()?.GetTableView() != null)
                {
                    parentWindow.GetView().GetTableView().focusable = true;
                }
            }
            else
            {
                parentWindow.m_Toolbar.SetFocusable(false);
                if (parentWindow.GetView().GetTableView() != null)
                {
                    parentWindow.GetView().GetTableView().focusable = false;
                }
                rootElement.focusable = true;
            }

            switch (state)
            {
                case OverlayState.AddColumn:
                    rootElement.style.display = DisplayStyle.Flex;
                    m_AddColumnOverlay.style.display = DisplayStyle.Flex;
                    m_AddColumnName.SetValueWithoutNotify($"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_AddColumnAddButton.Focus();
                    break;
                case OverlayState.AddRow:
                    rootElement.style.display = DisplayStyle.Flex;
                    m_AddRowOverlay.style.display = DisplayStyle.Flex;
                    m_AddRowName.SetValueWithoutNotify($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_AddRowAddButton.Focus();
                    break;
                case OverlayState.RenameColumn:
                    rootElement.style.display = DisplayStyle.Flex;
                    m_RenameColumnOverlay.style.display = DisplayStyle.Flex;
                    m_RenameColumnName.SetValueWithoutNotify(previousValue ??
                                                             $"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_RenameColumnName.Focus();
                    break;
                case OverlayState.RenameRow:
                    rootElement.style.display = DisplayStyle.Flex;
                    m_RenameRowOverlay.style.display = DisplayStyle.Flex;
                    m_RenameRowName.SetValueWithoutNotify(previousValue ??
                                                             $"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_RenameRowName.Focus();
                    break;
                default:
                    rootElement.style.display = DisplayStyle.None;
                    m_AddColumnOverlay.style.display = DisplayStyle.None;
                    m_AddRowOverlay.style.display = DisplayStyle.None;
                    m_RenameColumnOverlay.style.display = DisplayStyle.None;
                    m_RenameRowOverlay.style.display = DisplayStyle.None;
                    if (parentWindow.GetView()?.GetTableView() != null)
                    {
                        parentWindow.GetView()?.GetTableView().Focus();
                    }

                    break;
            }

            m_OverlayState = state;
        }
        internal void SetOverlayStateHidden()
        {
            SetState(OverlayState.Hide);
        }

        public enum OverlayState
        {
            Hide,
            AddColumn,
            AddRow,
            RenameColumn,
            RenameRow,
            Settings,
            Confirmation
        }

        internal void SubmitAddColumn()
        {
            if (m_OverlayState != OverlayState.AddColumn) return;

            if (parentWindow.GetController().AddColumn(m_AddColumnName.text, (Serializable.SerializableTypes)m_AddColumnType.value))
            {
                SetOverlayStateHidden();
            }
        }

        internal void SubmitAddRow()
        {
            if (m_OverlayState != OverlayState.AddRow) return;

            if (parentWindow.GetController().AddRow(m_AddRowName.text))
            {
                SetOverlayStateHidden();
            }
        }

        internal void SubmitRenameColumn()
        {
            if (m_OverlayState != OverlayState.RenameColumn) return;

            if (parentWindow.GetController().RenameColumn(cachedIndex, m_RenameColumnName.text))
            {
                SetOverlayStateHidden();
            }
        }
        internal void SubmitRenameRow()
        {
            if (m_OverlayState != OverlayState.RenameRow) return;

            if (parentWindow.GetController().RenameRow(cachedIndex, m_RenameRowName.text))
            {
                SetOverlayStateHidden();
            }
        }


    }
#endif
}