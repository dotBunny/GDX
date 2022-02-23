// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    public static class ConfigSectionsProvider
    {
        public const string ChangedClass = "changed";
        public const string HiddenClass = "hidden";
        const string k_DisabledClass = "disabled";
        const string k_EnabledClass = "enabled";
        const string k_ExpandedClass = "expanded";

        static StringKeyDictionary<VisualElement> s_ConfigSectionContents =
            new StringKeyDictionary<VisualElement>(SettingsProvider.SectionCount);

        static StringKeyDictionary<VisualElement> s_ConfigSectionHeaders =
            new StringKeyDictionary<VisualElement>(SettingsProvider.SectionCount);

        public static VisualElement CreateAndBindSectionHeader(IConfigSection section)
        {
            string sectionKey = section.GetSectionKey();
            if (s_ConfigSectionHeaders.ContainsKey(sectionKey))
            {
                return s_ConfigSectionContents[sectionKey];
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
                    OnExpandSectionHeaderClicked(sectionKey);
                    UpdateSectionHeader(sectionKey);
                    UpdateSectionContent(sectionKey);
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
                        OnToggleSectionHeaderClicked(enabledToggle, sectionKey, evt.newValue);
                    });
                }
                else
                {
                    enabledToggle.visible = false;
                    enabledToggle.tooltip = null;
                }
            }

            // Send back created instance
            s_ConfigSectionHeaders.AddUnchecked(sectionKey, headerInstance);
            return headerInstance;
        }

        public static VisualElement CreateAndBindSectionContent(IConfigSection section)
        {
            string sectionKey = section.GetSectionKey();
            if (s_ConfigSectionContents.ContainsKey(sectionKey))
            {
                return s_ConfigSectionContents[sectionKey];
            }

            VisualTreeAsset sectionAsset =
                ResourcesProvider.GetVisualTreeAsset(section.GetTemplateName());

            VisualElement sectionInstance = sectionAsset.Instantiate()[0];

            section.BindSectionContent(sectionInstance);

            // Record the whole section
            s_ConfigSectionContents.AddUnchecked(sectionKey, sectionInstance);

            return sectionInstance;
        }

        public static void ClearSectionCache()
        {
            s_ConfigSectionHeaders.Clear();
            s_ConfigSectionContents.Clear();
        }

        static void OnExpandSectionHeaderClicked(string sectionKey)
        {
            GUIUtility.hotControl = 0;
            IConfigSection section = SettingsProvider.ConfigSections[sectionKey];
            bool setting = SettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility());
            SettingsProvider.SetCachedEditorBoolean(sectionKey, !setting);
        }

        static void OnToggleSectionHeaderClicked(VisualElement toggleElement, string sectionKey, bool newValue)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionKey];
            section.SetToggleState(toggleElement, newValue);
            UpdateSectionHeader(sectionKey);
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

        public static void UpdateSectionContent(string sectionKey)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionKey];
            VisualElement element = s_ConfigSectionContents[sectionKey];

            if (SettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility()))
            {
                element.RemoveFromClassList(HiddenClass);
            }
            else
            {
                element.AddToClassList(HiddenClass);
            }

            section.UpdateSectionContent();
        }

        public static void UpdateSectionHeader(string sectionKey)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionKey];
            VisualElement sectionHeaderElement = s_ConfigSectionHeaders[sectionKey];

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

            if (SettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility()))
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