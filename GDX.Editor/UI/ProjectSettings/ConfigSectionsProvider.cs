// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.UI.ProjectSettings
{
    public static class ConfigSectionsProvider
    {
        private const string ExpandedClass = "expanded";
        private const string HiddenClass = "hidden";
        private const string EnabledClass = "enabled";
        private const string DisabledClass = "disabled";
        public const string ChangedClass = "changed";

        private static readonly Dictionary<string, VisualElement> s_configSectionContents =
            new Dictionary<string, VisualElement>();

        private static readonly Dictionary<string, VisualElement> s_configSectionHeaders =
            new Dictionary<string, VisualElement>();

        public static VisualElement CreateAndBindSectionHeader(IConfigSection section)
        {
            string sectionID = section.GetSectionID();
            if (s_configSectionHeaders.ContainsKey(sectionID))
            {
                return s_configSectionContents[sectionID];
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
            if (nameLabel != null) nameLabel.text = section.GetSectionHeaderLabel();

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
                    enabledToggle.visible = true;
                    enabledToggle.RegisterValueChangedCallback(evt =>
                    {
                        OnToggleSectionHeaderClicked(enabledToggle, sectionID, evt.newValue);
                    });
                }
                else
                {
                    enabledToggle.visible = false;
                }
            }

            // Send back created instance
            s_configSectionHeaders.Add(sectionID, headerInstance);
            return headerInstance;
        }

        public static VisualElement CreateAndBindSectionContent(IConfigSection section)
        {
            string sectionID = section.GetSectionID();
            if (s_configSectionContents.ContainsKey(sectionID))
            {
                return s_configSectionContents[sectionID];
            }

            VisualTreeAsset sectionAsset =
                ResourcesProvider.GetVisualTreeAsset(section.GetTemplateName());

            VisualElement sectionInstance = sectionAsset.Instantiate()[0];

            section.BindSectionContent(sectionInstance);

            // Record the whole section
            s_configSectionContents.Add(sectionID, sectionInstance);

            return sectionInstance;
        }

        public static void ClearSectionCache()
        {
            s_configSectionHeaders.Clear();
            s_configSectionContents.Clear();
        }

        static void OnExpandSectionHeaderClicked(string sectionID)
        {
            GUIUtility.hotControl = 0;
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            bool setting = SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());
            SettingsProvider.SetCachedEditorBoolean(sectionID, !setting);
        }

        static void OnToggleSectionHeaderClicked(VisualElement toggleElement, string sectionID, bool newValue)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            section.SetToggleState(toggleElement, newValue);
            UpdateSectionHeader(sectionID);
        }

        public static void UpdateAll()
        {
            foreach(KeyValuePair<string,IConfigSection> section in SettingsProvider.ConfigSections)
            {
                UpdateSectionHeader(section.Key);
                UpdateSectionContent(section.Key);
            }
        }

        public static void UpdateSectionContent(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            VisualElement element = s_configSectionContents[sectionID];

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
            VisualElement sectionHeaderElement = s_configSectionHeaders[sectionID];

            if (section.GetToggleSupport())
            {
                // Do this here
                Toggle enabledToggle = sectionHeaderElement.Q<Toggle>("toggle-enabled");
                enabledToggle.value = section.GetToggleState();

                bool toggleState = section.GetToggleState();
                if (toggleState)
                {
                    sectionHeaderElement.RemoveFromClassList(DisabledClass);
                    sectionHeaderElement.AddToClassList(EnabledClass);
                }
                else
                {
                    sectionHeaderElement.RemoveFromClassList(EnabledClass);
                    sectionHeaderElement.AddToClassList(DisabledClass);
                }
            }

            if (SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility()))
            {
                sectionHeaderElement.AddToClassList(ExpandedClass);
            }
            else
            {
                sectionHeaderElement.RemoveFromClassList(ExpandedClass);
            }
        }
    }
}