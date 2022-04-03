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
        static StringKeyDictionary<VisualElement> s_ConfigSectionContents =
            new StringKeyDictionary<VisualElement>(ProjectSettingsProvider.SectionCount);

        static StringKeyDictionary<VisualElement> s_ConfigSectionHeaders =
            new StringKeyDictionary<VisualElement>(ProjectSettingsProvider.SectionCount);

        // public static VisualElement Build()
        // {
        //
        // }

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
                    string helpLink = $"{ProjectSettingsProvider.DocumentationUri}{section.GetSectionHelpLink()}";
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
            IConfigSection section = ProjectSettingsProvider.ConfigSections[sectionKey];
            bool setting = ProjectSettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility());
            ProjectSettingsProvider.SetCachedEditorBoolean(sectionKey, !setting);
        }

        static void OnToggleSectionHeaderClicked(VisualElement toggleElement, string sectionKey, bool newValue)
        {
            // Do not toggle during search mode
            if (ProjectSettingsProvider.IsSearching()) return;

            IConfigSection section = ProjectSettingsProvider.ConfigSections[sectionKey];
            section.SetToggleState(toggleElement, newValue);
            UpdateSectionContent(sectionKey);
            UpdateSectionHeader(sectionKey);
        }

        public static void UpdateAll()
        {
            int iterator = 0;
            while (ProjectSettingsProvider.ConfigSections.MoveNext(ref iterator, out StringKeyEntry<IConfigSection> item))
            {
                UpdateSectionHeader(item.Key);
                UpdateSectionContent(item.Key);
            }
        }

        public static void UpdateSectionContent(string sectionKey)
        {
            IConfigSection section = ProjectSettingsProvider.ConfigSections[sectionKey];

            // This can happen due to the order of how events fire.
            if (!s_ConfigSectionContents.ContainsKey(sectionKey))
            {
                return;
            }

            VisualElement element = s_ConfigSectionContents[sectionKey];

            if (ProjectSettingsProvider.IsSearching())
            {
                // If we arent actually matched to the query
                if (!section.GetSearchKeywords().PartialMatch(ProjectSettingsProvider.SearchString))
                {
                    element.AddToClassList(ResourcesProvider.HiddenClass);
                    return;
                }

                // Ok so we do fall into it, so lets remove the hidden class
                element.RemoveFromClassList(ResourcesProvider.HiddenClass);
            }
            else
            {
                // Default visible/hidden behaviour
                if (ProjectSettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility()))
                {
                    element.RemoveFromClassList(ResourcesProvider.HiddenClass);
                }
                else
                {
                    element.AddToClassList(ResourcesProvider.HiddenClass);
                }
            }

            // Update the actual content
            section.UpdateSectionContent();
        }

        public static void UpdateSectionHeader(string sectionKey)
        {
            IConfigSection section = ProjectSettingsProvider.ConfigSections[sectionKey];

            // This can happen due to the order of how events fire.
            if (!s_ConfigSectionContents.ContainsKey(sectionKey))
            {
                return;
            }

            VisualElement sectionHeaderElement = s_ConfigSectionHeaders[sectionKey];

            if (ProjectSettingsProvider.IsSearching())
            {
                if (!section.GetSearchKeywords().PartialMatch(ProjectSettingsProvider.SearchString))
                {
                    sectionHeaderElement.AddToClassList(ResourcesProvider.HiddenClass);
                }
                else if(sectionHeaderElement.ClassListContains(ResourcesProvider.HiddenClass))
                {
                    sectionHeaderElement.RemoveFromClassList(ResourcesProvider.HiddenClass);
                }
            }
            else if (sectionHeaderElement.ClassListContains(ResourcesProvider.HiddenClass))
            {
                sectionHeaderElement.RemoveFromClassList(ResourcesProvider.HiddenClass);
            }

            if (section.GetToggleSupport())
            {
                // Do this here
                Toggle enabledToggle = sectionHeaderElement.Q<Toggle>("toggle-enabled");
                enabledToggle.value = section.GetToggleState();

                bool toggleState = section.GetToggleState();
                if (toggleState)
                {
                    sectionHeaderElement.RemoveFromClassList(ResourcesProvider.DisabledClass);
                    sectionHeaderElement.AddToClassList(ResourcesProvider.EnabledClass);
                }
                else
                {
                    sectionHeaderElement.RemoveFromClassList(ResourcesProvider.EnabledClass);
                    sectionHeaderElement.AddToClassList(ResourcesProvider.DisabledClass);
                }
            }

            if (ProjectSettingsProvider.GetCachedEditorBoolean(sectionKey, section.GetDefaultVisibility()))
            {
                sectionHeaderElement.AddToClassList(ResourcesProvider.ExpandedClass);
            }
            else
            {
                sectionHeaderElement.RemoveFromClassList(ResourcesProvider.ExpandedClass);
            }
        }


        public static void SetClassChangeCheck<T1, T2>(T1 element, T2 lhs, T2 rhs)
            where T1 : BaseField<T2> where T2 : class
        {
            element.SetValueWithoutNotify(rhs);

            if (lhs != rhs)
            {
                element.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }
        }

        public static void SetStructChangeCheck<T1, T2>(T1 element, T2 lhs, T2 rhs)
            where T1 : BaseField<T2> where T2 : struct
        {
            element.SetValueWithoutNotify(rhs);

            if (!lhs.Equals(rhs))
            {
                element.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }
        }
        public static void SetEnumChangeCheck<T>(EnumField element, T lhs, T rhs) where T : Enum
        {
            element.SetValueWithoutNotify(rhs);
            if (lhs.ToString() != rhs.ToString())
            {
                element.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }
        }

        public static void SetMaskChangeCheck(MaskField element, int lhs, int rhs)
        {
            element.SetValueWithoutNotify(rhs);
            if (lhs != rhs)
            {
                element.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                element.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }
        }

    }
}