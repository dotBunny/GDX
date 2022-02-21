// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.UI.ProjectSettings
{
    public static class ConfigSectionsProvider
    {
        private const string k_ExpandedClass = "expanded";
        public const string HiddenClass = "hidden";
        private const string k_EnabledClass = "enabled";
        private const string k_DisabledClass = "disabled";
        public const string ChangedClass = "changed";

        private static readonly Dictionary<string, VisualElement> s_ConfigSectionContents =
            new Dictionary<string, VisualElement>();

        private static readonly Dictionary<string, VisualElement> s_ConfigSectionHeaders =
            new Dictionary<string, VisualElement>();

        public static VisualElement CreateAndBindSectionHeader(IConfigSection section)
        {
            string sectionID = section.GetSectionID();
            if (s_ConfigSectionHeaders.ContainsKey(sectionID))
            {
                return s_ConfigSectionContents[sectionID];
            }

            VisualTreeAsset headerAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsSectionHeader");

            VisualElement headerInstance = headerAsset.Instantiate()[0];

            // Expansion
            Button expandButton = headerInstance.Q<Button>("button-expand");
            if (expandButton != null)
            {
                expandButton.clicked += () =>
                {
                    OnExpandSectionHeaderClicked(sectionID);
                    UpdateSectionHeader(sectionID);
                    UpdateSectionContent(sectionID);
                };
            }

            // Label
            Label nameLabel = headerInstance.Q<Label>("label-name");
            if (nameLabel != null)
            {
                nameLabel.text = section.GetSectionHeaderLabel();
            }

            // Help Button
            Button helpButton = headerInstance.Q<Button>("button-help");
            if (helpButton != null)
            {
                if (section.GetSectionHelpLink() != null)
                {
                    string helpLink = $"{SettingsProvider.DocumentationUri}{section.GetSectionHelpLink()}";
                    helpButton.clicked += () =>
                    {
                        GUIUtility.hotControl = 0;
                        Application.OpenURL(helpLink);
                    };
                    helpButton.visible = true;
                }
                else
                {
                    helpButton.visible = false;
                }
            }

            // Toggle
            Toggle enabledToggle = headerInstance.Q<Toggle>("toggle-enabled");
            if (enabledToggle != null)
            {
                if (section.GetToggleSupport())
                {
                    enabledToggle.tooltip = section.GetToggleTooltip();
                    enabledToggle.visible = true;
                    enabledToggle.RegisterValueChangedCallback(evt =>
                    {
                        OnToggleSectionHeaderClicked(enabledToggle, sectionID, evt.newValue);
                    });
                }
                else
                {
                    enabledToggle.visible = false;
                    enabledToggle.tooltip = null;
                }
            }

            // Send back created instance
            s_ConfigSectionHeaders.Add(sectionID, headerInstance);
            return headerInstance;
        }

        public static VisualElement CreateAndBindSectionContent(IConfigSection section)
        {
            string sectionID = section.GetSectionID();
            if (s_ConfigSectionContents.ContainsKey(sectionID))
            {
                return s_ConfigSectionContents[sectionID];
            }

            VisualTreeAsset sectionAsset =
                ResourcesProvider.GetVisualTreeAsset(section.GetTemplateName());

            VisualElement sectionInstance = sectionAsset.Instantiate()[0];

            section.BindSectionContent(sectionInstance);

            // Record the whole section
            s_ConfigSectionContents.Add(sectionID, sectionInstance);

            return sectionInstance;
        }

        public static void ClearSectionCache()
        {
            s_ConfigSectionHeaders.Clear();
            s_ConfigSectionContents.Clear();
        }

        private static void OnExpandSectionHeaderClicked(string sectionID)
        {
            GUIUtility.hotControl = 0;
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            bool setting = SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());
            SettingsProvider.SetCachedEditorBoolean(sectionID, !setting);
        }

        private static void OnToggleSectionHeaderClicked(VisualElement toggleElement, string sectionID, bool newValue)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            section.SetToggleState(toggleElement, newValue);
            UpdateSectionHeader(sectionID);
        }

        public static void UpdateAll()
        {
            int iterator = 0;
            while (SettingsProvider.ConfigSections.MoveNext(ref iterator, out StringKeyEntry<IConfigSection> item))
            {
                UpdateSectionHeader(item.Key);
                UpdateSectionContent(item.Key);
            }
        }

        public static void UpdateSectionContent(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            VisualElement element = s_ConfigSectionContents[sectionID];

            if (SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility()))
            {
                element.RemoveFromClassList(HiddenClass);
            }
            else
            {
                element.AddToClassList(HiddenClass);
            }

            section.UpdateSectionContent();
        }

        public static void UpdateSectionHeader(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            VisualElement sectionHeaderElement = s_ConfigSectionHeaders[sectionID];

            if (section.GetToggleSupport())
            {
                // Do this here
                Toggle enabledToggle = sectionHeaderElement.Q<Toggle>("toggle-enabled");
                enabledToggle.value = section.GetToggleState();

                bool toggleState = section.GetToggleState();
                if (toggleState)
                {
                    sectionHeaderElement.RemoveFromClassList(k_DisabledClass);
                    sectionHeaderElement.AddToClassList(k_EnabledClass);
                }
                else
                {
                    sectionHeaderElement.RemoveFromClassList(k_EnabledClass);
                    sectionHeaderElement.AddToClassList(k_DisabledClass);
                }
            }

            if (SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility()))
            {
                sectionHeaderElement.AddToClassList(k_ExpandedClass);
            }
            else
            {
                sectionHeaderElement.RemoveFromClassList(k_ExpandedClass);
            }
        }


        public static void SetClassChangeCheck<T1, T2>(T1 element, T2 lhs, T2 rhs)
            where T1 : BaseField<T2> where T2 : class
        {
            element.SetValueWithoutNotify(rhs);

            if (lhs != rhs)
            {
                element.AddToClassList(ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ChangedClass);
            }
        }

        public static void SetStructChangeCheck<T1, T2>(T1 element, T2 lhs, T2 rhs)
            where T1 : BaseField<T2> where T2 : struct
        {
            element.SetValueWithoutNotify(rhs);

            if (!lhs.Equals(rhs))
            {
                element.AddToClassList(ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ChangedClass);
            }
        }
        public static void SetEnumChangeCheck<T>(EnumField element, T lhs, T rhs) where T : Enum
        {
            element.SetValueWithoutNotify(rhs);
            if (lhs.ToString() != rhs.ToString())
            {
                element.AddToClassList(ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ChangedClass);
            }
        }

        public static void SetMaskChangeCheck(MaskField element, int lhs, int rhs)
        {
            element.SetValueWithoutNotify(rhs);
            if (lhs != rhs)
            {
                element.AddToClassList(ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ChangedClass);
            }
        }

    }
}