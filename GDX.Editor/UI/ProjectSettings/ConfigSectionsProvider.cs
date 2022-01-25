// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.UI.ProjectSettings
{
    public static class ConfigSectionsProvider
    {
        public const string ExpandedClass = "expanded";
        public const string EnabledClass = "enabled";
        public const string DisabledClass = "disabled";

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
                    UpdateSectionHeaderStyles(sectionID);
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

                    // enabledToggle.RegisterValueChangedCallback(evt =>
                    // {
                    //     OnToggleSectionHeaderClicked()
                    // });
                }
                else
                {
                    enabledToggle.visible = false;
                    //enabledToggle.UnregisterValueChangedCallback();
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

            VisualTreeAsset wrapperAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsSectionContent");

            VisualElement wrapperInstance = wrapperAsset.Instantiate()[0];

            VisualTreeAsset sectionAsset =
                ResourcesProvider.GetVisualTreeAsset(section.GetTemplateName());

            VisualElement sectionInstance = sectionAsset.Instantiate()[0];

            section.BindSectionContent(sectionInstance, Core.Config);

            wrapperInstance.Add(sectionInstance);

            // Record the whole section
            s_configSectionContents.Add(sectionID, sectionInstance);



            return wrapperInstance;
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

        static void OnToggleSectionHeaderClicked(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            section.SetToggleState(!section.GetToggleState());
            UpdateSectionHeaderStyles(sectionID);
        }
        public static void UpdateSectionContent(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            VisualElement element = s_configSectionContents[sectionID];

            if (SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility()))
            {
                element.AddToClassList(ExpandedClass);
            }
            else
            {
                element.RemoveFromClassList(ExpandedClass);
            }

            // TODO : Transient?
            section.UpdateSectionContent(Core.Config);
        }
        public static void UpdateSectionHeaderStyles(string sectionID)
        {
            IConfigSection section = SettingsProvider.ConfigSections[sectionID];
            VisualElement element = s_configSectionHeaders[sectionID];

            if (section.GetToggleSupport())
            {
                bool toggleState = section.GetToggleState();
                if (toggleState)
                {
                    element.RemoveFromClassList(DisabledClass);
                    element.AddToClassList(EnabledClass);
                }
                else
                {
                    element.RemoveFromClassList(EnabledClass);
                    element.AddToClassList(DisabledClass);
                }
            }

            if (SettingsProvider.GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility()))
            {
                element.AddToClassList(ExpandedClass);
            }
            else
            {
                element.RemoveFromClassList(ExpandedClass);
            }
        }


    }
}