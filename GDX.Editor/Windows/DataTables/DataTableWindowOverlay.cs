// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.DataTables;
using GDX.Editor.VisualElements;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.DataTables
{
#if UNITY_2022_2_OR_NEWER
    class DataTableWindowOverlay
    {
        const string k_ValidClass = "valid";
        const string k_WarningClass = "warning";
        const string k_ErrorClass = "error";

        public enum ConfirmationState
        {
            Invalid,
            RemoveRow,
            RemoveColumn
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

        readonly Button m_AddColumnAddButton;
        readonly Button m_AddColumnCancelButton;
        readonly TextField m_AddColumnName;

        readonly VisualElement m_AddColumnOverlay;
        readonly PopupField<int> m_AddColumnType;
        readonly TextField m_AddColumnFilter;
        readonly VisualElement m_AddColumnFilterStatus;
        readonly TypePicker m_AddColumnFilterPicker;

        readonly Button m_AddRowAddButton;
        readonly Button m_AddRowCancelButton;
        readonly TextField m_AddRowName;

        readonly VisualElement m_AddRowOverlay;
        readonly Button m_ConfirmationAcceptButton;
        readonly Button m_ConfirmationCancelButton;
        readonly Label m_ConfirmationMessageLabel;

        readonly VisualElement m_ConfirmationOverlay;
        readonly Label m_ConfirmationTitleLabel;

        readonly Button m_RenameAcceptButton;
        readonly Button m_RenameCancelButton;
        readonly TextField m_RenameName;
        readonly VisualElement m_RenameOverlay;
        readonly Label m_RenameTitleLabel;

        readonly VisualElement m_RootElement;
        readonly Button m_SettingsCancelButton;
        readonly TextField m_SettingsDisplayName;

        readonly VisualElement m_SettingsOverlay;
        readonly Button m_SettingsSaveButton;
        readonly Toggle m_SettingsSupportUndoToggle;
        readonly DataTableWindow m_DataTableWindow;
        int m_CachedIndex;
        ConfirmationState m_ConfirmationState;

        OverlayState m_CurrentState;


        internal DataTableWindowOverlay(VisualElement element, DataTableWindow window)
        {
            // Cache a few things
            m_RootElement = element;
            m_DataTableWindow = window;

            // Bind our column adding overlay
            m_AddColumnOverlay = m_RootElement.Q<VisualElement>("gdx-table-add-column");
            m_AddColumnName = m_AddColumnOverlay.Q<TextField>("gdx-table-column-name");
            int columnNameIndex = m_AddColumnOverlay.IndexOf(m_AddColumnName);
            List<int> typeValues = new List<int>(Serializable.SerializableTypesCount);
            for (int i = 0; i < Serializable.SerializableTypesCount; i++)
            {
                typeValues.Add(i);
            }
            m_AddColumnType =
                new PopupField<int>(typeValues, 0, Serializable.GetLabelFromTypeValue,
                    Serializable.GetLabelFromTypeValue) { label = "Type", name = "gdx-table-column-type" };
            m_AddColumnType.RegisterValueChangedCallback(e =>
            {
                UpdateAddColumnBasedOnType(e.newValue);
            });
            m_AddColumnOverlay.Insert(columnNameIndex + 1, m_AddColumnType);

            m_AddColumnFilter = m_AddColumnOverlay.Q<TextField>("gdx-table-column-filter");
            m_AddColumnFilter.AddToClassList(ResourcesProvider.HiddenClass);
            m_AddColumnFilter.RegisterValueChangedCallback(e =>
            {
                ValidateAssemblyQualifiedName(e.newValue);
            });
            m_AddColumnFilterPicker = new TypePicker(m_AddColumnFilter, m_AddColumnOverlay, m_RootElement, ValidateAssemblyQualifiedNameInPlace);
            m_AddColumnFilterStatus = m_AddColumnOverlay.Q<VisualElement>("gdx-table-add-column-filter-status");
            m_AddColumnFilterStatus.AddToClassList(ResourcesProvider.HiddenClass);

            m_AddColumnAddButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-add");
            m_AddColumnAddButton.clicked += SubmitAddColumn;
            m_AddColumnCancelButton = m_AddColumnOverlay.Q<Button>("gdx-table-column-cancel");
            m_AddColumnCancelButton.clicked += SetOverlayStateHidden;

            // Bind our row adding overlay
            m_AddRowOverlay = m_RootElement.Q<VisualElement>("gdx-table-add-row");
            m_AddRowName = m_AddRowOverlay.Q<TextField>("gdx-table-row-name");
            m_AddRowAddButton = m_AddRowOverlay.Q<Button>("gdx-table-row-add");
            m_AddRowAddButton.clicked += SubmitAddRow;
            m_AddRowCancelButton = m_AddRowOverlay.Q<Button>("gdx-table-row-cancel");
            m_AddRowCancelButton.clicked += SetOverlayStateHidden;

            // Bind our renaming overlay
            m_RenameOverlay = m_RootElement.Q<VisualElement>("gdx-table-rename");
            m_RenameName = m_RenameOverlay.Q<TextField>("gdx-table-rename-name");
            m_RenameTitleLabel = m_RenameOverlay.Q<Label>("gdx-table-rename-title");
            m_RenameAcceptButton = m_RenameOverlay.Q<Button>("gdx-table-rename-accept");
            m_RenameAcceptButton.clicked += SubmitRename;
            m_RenameCancelButton = m_RenameOverlay.Q<Button>("gdx-table-rename-cancel");
            m_RenameCancelButton.clicked += SetOverlayStateHidden;

            // Bind our generic confirmation overlay
            m_ConfirmationOverlay = m_RootElement.Q<VisualElement>("gdx-table-confirmation");
            m_ConfirmationAcceptButton = m_ConfirmationOverlay.Q<Button>("gdx-table-confirmation-accept");
            m_ConfirmationAcceptButton.clicked += SubmitConfirmation;
            m_ConfirmationCancelButton = m_ConfirmationOverlay.Q<Button>("gdx-table-confirmation-cancel");
            m_ConfirmationCancelButton.clicked += SetOverlayStateHidden;
            m_ConfirmationTitleLabel = m_ConfirmationOverlay.Q<Label>("gdx-confirmation-title");
            m_ConfirmationMessageLabel = m_ConfirmationOverlay.Q<Label>("gdx-confirmation-message");

            // Bind our settings overlay
            m_SettingsOverlay = m_RootElement.Q<VisualElement>("gdx-table-settings");
            m_SettingsDisplayName = m_SettingsOverlay.Q<TextField>("gdx-table-display-name");
            m_SettingsSupportUndoToggle = m_SettingsOverlay.Q<Toggle>("gdx-table-flag-undo");
            m_SettingsSaveButton = m_SettingsOverlay.Q<Button>("gdx-table-settings-save");
            m_SettingsSaveButton.clicked += SubmitSettings;
            m_SettingsCancelButton = m_SettingsOverlay.Q<Button>("gdx-table-settings-cancel");
            m_SettingsCancelButton.clicked += SetOverlayStateHidden;

            // Ensure state of everything
            SetState(OverlayState.Hide);
        }

        public OverlayState GetPrimaryState()
        {
            return m_CurrentState;
        }

        internal void SetConfirmationState(ConfirmationState state, int stableIndex, string title, string message)
        {
            m_ConfirmationTitleLabel.text = title;
            m_ConfirmationMessageLabel.text = message;
            m_ConfirmationState = state;
            SetState(OverlayState.Confirmation, stableIndex);
        }

        void UpdateAddColumnBasedOnType(int type)
        {
            switch ((Serializable.SerializableTypes)type)
            {
                case Serializable.SerializableTypes.EnumInt:
                    m_AddColumnFilter.value = Reflection.SerializedTypesName;
                    m_AddColumnFilter.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.SetType(typeof(System.Enum));
                    m_AddColumnFilterPicker.ScheduleUpdateSizeAndPosition();
                    break;
                case Serializable.SerializableTypes.Object:
                    m_AddColumnFilter.value = Reflection.UnityObjectName;
                    m_AddColumnFilter.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.SetType(typeof(UnityEngine.Object));
                    m_AddColumnFilterPicker.ScheduleUpdateSizeAndPosition();
                    break;
                default:
                    m_AddColumnFilter.value = null;
                    m_AddColumnFilter.AddToClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterStatus.AddToClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.AddToClassList(ResourcesProvider.HiddenClass);
                    m_AddColumnFilterPicker.Hide();
                    break;
            }
        }

        void ValidateAssemblyQualifiedNameInPlace()
        {
            ValidateAssemblyQualifiedName(m_AddColumnFilter.text);
        }
        void ValidateAssemblyQualifiedName(string newValue)
        {
            if (string.IsNullOrEmpty(newValue))
            {
                m_AddColumnFilterStatus.AddToClassList(k_WarningClass);
                m_AddColumnFilterStatus.RemoveFromClassList(k_ValidClass);
                m_AddColumnFilterStatus.RemoveFromClassList(k_ErrorClass);
            }
            else
            {
                System.Type newType = System.Type.GetType(newValue);
                if (newType != null)
                {
                    m_AddColumnFilterStatus.AddToClassList(k_ValidClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(k_WarningClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(k_ErrorClass);
                }
                else
                {
                    m_AddColumnFilterStatus.AddToClassList(k_ErrorClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(k_WarningClass);
                    m_AddColumnFilterStatus.RemoveFromClassList(k_ValidClass);
                }
            }
        }

        internal void SetState(OverlayState state, int stableIndex = -1, string previousValue = null)
        {
            m_CachedIndex = stableIndex;

            // Handle focus
            DataTableWindowView view = m_DataTableWindow.GetView();
            if (state == OverlayState.Hide)
            {
                m_DataTableWindow.GetToolbar().SetFocusable(true);
                m_RootElement.focusable = false;

                if (view?.GetMultiColumnListView() != null)
                {
                    view.GetMultiColumnListView().focusable = true;
                }
            }
            else
            {
                m_DataTableWindow.GetToolbar().SetFocusable(false);
                if (view?.GetMultiColumnListView() != null)
                {
                    view.GetMultiColumnListView().focusable = false;
                }

                m_RootElement.focusable = true;
            }

            switch (state)
            {
                case OverlayState.AddColumn:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_AddColumnOverlay.style.display = DisplayStyle.Flex;
                    m_AddColumnName.SetValueWithoutNotify($"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_AddColumnType?.SetValueWithoutNotify(0);
                    UpdateAddColumnBasedOnType(0);
                    m_AddColumnAddButton.Focus();
                    // Don't allow cancel if we dont have column
                    m_AddColumnCancelButton.SetEnabled(m_DataTableWindow.GetDataTable().GetColumnCount() > 0);
                    break;
                case OverlayState.AddRow:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_AddRowOverlay.style.display = DisplayStyle.Flex;
                    m_AddRowName.SetValueWithoutNotify($"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_AddRowAddButton.Focus();
                    break;
                case OverlayState.RenameColumn:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_RenameOverlay.style.display = DisplayStyle.Flex;
                    m_RenameTitleLabel.text = "Rename Column";
                    m_RenameName.SetValueWithoutNotify(previousValue ??
                                                       $"Column_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_RenameName.Focus();
                    break;
                case OverlayState.RenameRow:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_RenameOverlay.style.display = DisplayStyle.Flex;
                    m_RenameTitleLabel.text = "Rename Row";
                    m_RenameName.SetValueWithoutNotify(previousValue ??
                                                       $"Row_{Core.Random.NextInteger(1, 9999).ToString()}");
                    m_RenameName.Focus();
                    break;
                case OverlayState.Confirmation:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_ConfirmationOverlay.style.display = DisplayStyle.Flex;
                    m_ConfirmationAcceptButton.Focus();
                    break;
                case OverlayState.Settings:
                    m_RootElement.style.display = DisplayStyle.Flex;
                    m_SettingsOverlay.style.display = DisplayStyle.Flex;
                    m_SettingsDisplayName.SetValueWithoutNotify(m_DataTableWindow.GetDataTable().GetDisplayName());
#if UNITY_2022_2_OR_NEWER
                    m_SettingsSupportUndoToggle.SetValueWithoutNotify(m_DataTableWindow.GetDataTable()
                        .GetFlag(DataTableBase.Settings.EnableUndo));
#else
                    m_SettingsSupportUndoToggle.SetEnabled(false);
                    m_SettingsSupportUndoToggle.SetValueWithoutNotify(false);
                    m_SettingsSupportUndoToggle.tooltip = "Unsupported before Unity 2022.2";
#endif
                    break;
                default:
                    m_RootElement.style.display = DisplayStyle.None;
                    m_AddColumnOverlay.style.display = DisplayStyle.None;
                    m_AddRowOverlay.style.display = DisplayStyle.None;
                    m_RenameOverlay.style.display = DisplayStyle.None;
                    m_ConfirmationOverlay.style.display = DisplayStyle.None;
                    m_SettingsOverlay.style.display = DisplayStyle.None;
                    if (m_DataTableWindow.GetView()?.GetMultiColumnListView() != null)
                    {
                        m_DataTableWindow.GetView()?.GetMultiColumnListView().Focus();
                    }

                    m_ConfirmationState = ConfirmationState.Invalid;
                    break;
            }

            m_CurrentState = state;
        }

        internal void SetOverlayStateHidden()
        {
            SetState(OverlayState.Hide);
        }

        internal void SubmitAddColumn()
        {
            if (m_CurrentState != OverlayState.AddColumn)
            {
                return;
            }

            if (m_DataTableWindow.GetController()
                .AddColumn(m_AddColumnName.text, (Serializable.SerializableTypes)m_AddColumnType.value, m_AddColumnFilter.value))
            {
                SetOverlayStateHidden();
            }
        }

        internal void SubmitAddRow()
        {
            if (m_CurrentState != OverlayState.AddRow)
            {
                return;
            }

            if (m_DataTableWindow.GetController().AddRow(m_AddRowName.text))
            {
                SetOverlayStateHidden();
            }
        }

        internal void SubmitRename()
        {
            switch (m_CurrentState)
            {
                case OverlayState.RenameColumn:
                    if (m_DataTableWindow.GetController().RenameColumn(m_CachedIndex, m_RenameName.text))
                    {
                        SetOverlayStateHidden();
                    }

                    break;
                case OverlayState.RenameRow:
                    if (m_DataTableWindow.GetController().RenameRow(m_CachedIndex, m_RenameName.text))
                    {
                        SetOverlayStateHidden();
                    }

                    break;
            }
        }

        internal void SubmitConfirmation()
        {
            switch (m_ConfirmationState)
            {
                case ConfirmationState.RemoveRow:
                    if (m_DataTableWindow.GetController().RemoveRow(m_CachedIndex))
                    {
                        SetOverlayStateHidden();
                    }

                    break;
                case ConfirmationState.RemoveColumn:
                    if (m_DataTableWindow.GetController().RemoveColumn(m_CachedIndex))
                    {
                        SetOverlayStateHidden();
                    }

                    break;
            }
        }

        internal void SubmitSettings()
        {
            if (m_CurrentState != OverlayState.Settings)
            {
                return;
            }

            if (m_DataTableWindow.GetController()
                .SetTableSettings(m_SettingsDisplayName.text, m_SettingsSupportUndoToggle.value))
            {
                SetOverlayStateHidden();
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}