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
        ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
        /// </summary>
        private static readonly Dictionary<string, bool> s_cachedEditorPreferences = new Dictionary<string, bool>();

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly List<string> s_searchKeywords = new List<string>(new[]
        {
            "gdx", "update", "parser", "commandline", "build"
        });

        public static readonly Dictionary<string, IConfigSection> ConfigSections = new Dictionary<string, IConfigSection>();

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

                    ProjectSettings.ConfigSectionsProvider.ClearSectionCache();
                    foreach (KeyValuePair<string, IConfigSection> section in ConfigSections)
                    {
                        VisualElement sectionHeader = ProjectSettings.ConfigSectionsProvider.CreateAndBindSectionHeader(section.Value);
                        contentScrollView.contentContainer.Add(sectionHeader);
                        ProjectSettings.ConfigSectionsProvider.UpdateSectionHeaderStyles(section.Key);

                        VisualElement sectionContentBase = ProjectSettings.ConfigSectionsProvider.CreateAndBindSectionContent(section.Value);
                        contentScrollView.contentContainer.Add(sectionContentBase); // Add wrapper
                        ProjectSettings.ConfigSectionsProvider.UpdateSectionContent(section.Key);
                    }
                },
                keywords = s_searchKeywords
            };
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns></returns>
        public static bool GetCachedEditorBoolean(string id, bool defaultValue = true)
        {
            if (!s_cachedEditorPreferences.ContainsKey(id))
            {
                s_cachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_cachedEditorPreferences[id];
        }

        // TODO: Sections need to register ... somehow order
        public static void RegisterConfigSection(IConfigSection section, Type afterSection = null,
            Type beforeSection = null)
        {
            if(ConfigSections.ContainsKey(section.GetSectionID())) return;


            ConfigSections.Add(section.GetSectionID(), section);
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetCachedEditorBoolean(string id, bool setValue)
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