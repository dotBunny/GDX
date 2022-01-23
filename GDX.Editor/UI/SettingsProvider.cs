// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.UI
{
    /// <summary>
    ///     GDX Assembly Settings Provider
    /// </summary>
    [HideFromDocFX]
    public static class SettingsProvider
    {
        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string DocumentationUri = "https://gdx.dotbunny.com/";

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly List<string> s_searchKeywords = new List<string>(new[]
        {
            "gdx", "update", "parser", "commandline", "build"
        });

        private static readonly Dictionary<string, IConfigSection> s_configSections = new Dictionary<string, IConfigSection>();

        private static readonly Dictionary<string, VisualElement> s_configSectionHeaders =
            new Dictionary<string, VisualElement>();
        private static readonly Dictionary<string, VisualElement> s_configSectionContents =
            new Dictionary<string, VisualElement>();


        /// <summary>
        ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
        /// </summary>
        private static readonly Dictionary<string, bool> s_cachedEditorPreferences = new Dictionary<string, bool>();


        /// <summary>
        ///     Get <see cref="UnityEditor.SettingsProvider" /> for GDX assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [UnityEditor.SettingsProvider]
        public static UnityEditor.SettingsProvider Get()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new UnityEditor.SettingsProvider("Project/GDX", UnityEditor.SettingsScope.Project)
            {
                label = "GDX",
                activateHandler = (searchContext, rootElement) =>
                {
                    rootElement.styleSheets.Add(ResourcesProvider.GetStyleSheet());
                    ResourcesProvider.GetVisualTreeAsset("GDXProjectSettings").CloneTree(rootElement);

                    // Build some useful references
                    ScrollView contentScrollView = rootElement.Q<ScrollView>("gdx-project-settings-content");

                    // Build out panels
                    GDXConfig tempConfig = new GDXConfig(Core.Config);

                    s_configSectionHeaders.Clear();
                    s_configSectionContents.Clear();
                    foreach (KeyValuePair<string, IConfigSection> section in s_configSections)
                    {
                        s_configSectionHeaders.Add(section.Key, BuildSectionHeader(section.Value));
                        contentScrollView.contentContainer.Add(s_configSectionHeaders[section.Key]);
                        UpdateSectionHeaderStyle(section.Key);

                        //section.DrawSectionContent(tempConfig);
                    }
                },
                // guiHandler = searchContext =>
                // {
                //     // Get a working copy
                //
                //
                //     // Start wrapping the content
                //     UnityEditor.EditorGUILayout.BeginVertical(GDX.Editor.ProjectSettings.SettingsStyles.WrapperStyle);
                //
                //    // PackageStatusSection.Draw(tempConfig);
                //
                //     foreach (IConfigSection section in s_configSections)
                //     {
                //         section.Draw(tempConfig);
                //     }
                //
                //
                //     // check for changes
                //     // save newconfig?
                //
                //
                //     // Stop wrapping the content
                //     UnityEditor.EditorGUILayout.EndVertical();
                // },
                keywords = s_searchKeywords
            };
        }

        // TODO: Sections need to register ... somehow order
        public static void RegisterConfigSection(IConfigSection section, Type afterSection = null,
            Type beforeSection = null)
        {
            if(s_configSections.ContainsKey(section.GetSectionID())) return;


            s_configSections.Add(section.GetSectionID(), section);
        }

        private static VisualElement BuildSectionHeader(IConfigSection section)
        {
            string sectionID = section.GetSectionID();

            VisualTreeAsset headerAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsSectionHeader");

            VisualElement headerInstance = headerAsset.Instantiate().contentContainer; ;

            // Expansion
            Button expandButton = headerInstance.Q<Button>("button-expand");
            if (expandButton != null)
            {
                expandButton.clicked += () =>
                {
                    OnExpandSectionHeaderClicked(sectionID);
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
                    string helpLink = $"{DocumentationUri}{section.GetSectionHelpLink()}";
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
                    //enabledToggle.RegisterValueChangedCallback()
                }
                else
                {
                    enabledToggle.visible = false;
                    //enabledToggle.UnregisterValueChangedCallback();
                }
            }

            // Send back created instance
            return headerInstance;
        }

        static void UpdateSectionHeaderStyle(string sectionID)
        {
            IConfigSection section = s_configSections[sectionID];
            VisualElement element = s_configSectionHeaders[sectionID];

            // Determine expansion
            bool setting = GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());

            // APply styles
        }

        static void OnExpandSectionHeaderClicked(string sectionID)
        {
            GUIUtility.hotControl = 0;
            IConfigSection section = s_configSections[sectionID];
            bool setting = GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());

            // Toggle
            SetCachedEditorBoolean(sectionID, !setting);
        }

        static void OnToggleSectionHeaderClicked(string sectionID)
        {
            IConfigSection section = s_configSections[sectionID];
            VisualElement element = s_configSectionHeaders[sectionID];

            bool setting = GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());
        }

        public static void UpdateSectionHeader(VisualElement element, IConfigSection section)
        {
            //bool setting = GetCachedEditorBoolean(sectionID, section.GetDefaultVisibility());
            //         if (!setting)
            //         {
            //             // ReSharper disable once InvertIf
            //             if (GUILayout.Button(SettingsStyles.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
            //                 SettingsLayoutOptions.SectionHeaderExpandLayoutOptions))
            //             {
            //                 GUIUtility.hotControl = 0;
            //                 SetCachedEditorBoolean(id, true);
            //             }
            //         }
            //         else
            //         {
            //             // ReSharper disable once InvertIf
            //             if (GUILayout.Button(SettingsStyles.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
            //                 SettingsLayoutOptions.SectionHeaderExpandLayoutOptions))
            //             {
            //                 GUIUtility.hotControl = 0;
            //                 SetCachedEditorBoolean(id, false);
            //             }
            //         }
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns></returns>
        static bool GetCachedEditorBoolean(string id, bool defaultValue = true)
        {
            if (!s_cachedEditorPreferences.ContainsKey(id))
            {
                s_cachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_cachedEditorPreferences[id];
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        static void SetCachedEditorBoolean(string id, bool setValue)
        {
            if (!s_cachedEditorPreferences.ContainsKey(id))
            {
                s_cachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
            else
            {
                if (s_cachedEditorPreferences[id] == setValue)
                {
                    return;
                }

                s_cachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
        }
    }
}