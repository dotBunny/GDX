// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine.UIElements;

// ReSharper disable UnusedMember.Global

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
        private static StringKeyDictionary<bool> s_cachedEditorPreferences = new StringKeyDictionary<bool>(10);

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly List<string> s_searchKeywords = new List<string>(new[]
        {
            "gdx", "automatic", "package", "update", "buildinfo", "task", "stream", "changelist",
            "cli", "argument", "environment", "trace", "symbol", "locale", "localization", "culture",
            "visual", "scripting", "vs", "parser", "commandline", "build"
        });

        public static StringKeyDictionary<IConfigSection> ConfigSections = new StringKeyDictionary<IConfigSection>(10);

        public static GDXConfig WorkingConfig;

        private static VisualElement _changesElement;
        private static Button _clearButton;
        private static Button _saveButton;


        /// <summary>
        ///     Get <see cref="UnityEditor.SettingsProvider" /> for GDX assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static UnityEditor.SettingsProvider Get()
        {
            // Create our working copy of the config - we do this to catch if theres an override that happens
            // during domain reload callbacks
            WorkingConfig ??= new GDXConfig(Core.Config);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new UnityEditor.SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                activateHandler = (searchContext, rootElement) =>
                {
                    // Add base style sheet
                    if (ResourcesProvider.GetStyleSheet() != null)
                    {
                        rootElement.styleSheets.Add(ResourcesProvider.GetStyleSheet());
                    }

                    // Add any overrides
                    if (ResourcesProvider.GetStyleSheetOverride() != null)
                    {
                        rootElement.styleSheets.Add(ResourcesProvider.GetStyleSheetOverride());
                    }

                    // Get our base element
                    ResourcesProvider.GetVisualTreeAsset("GDXProjectSettings").CloneTree(rootElement);

                    // Early handle of theme
                    ResourcesProvider.CheckTheme(rootElement);

                    _changesElement = rootElement.Q<VisualElement>("gdx-config-changes");

                    // Handle state buttons
                    _clearButton = _changesElement.Q<Button>("button-clear-changes");
                    _clearButton.clicked += () =>
                    {
                        WorkingConfig = new GDXConfig(Core.Config);
                        ConfigSectionsProvider.UpdateAll();
                        CheckForChanges();
                    };

                    _saveButton = _changesElement.Q<Button>("button-save-changes");
                    _saveButton.clicked += () =>
                    {
                        string codePath =
                            System.IO.Path.Combine(UnityEngine.Application.dataPath, "Generated", "GDXSettings.cs");

                        // Ensure folder structure is present
                        Platform.EnsureFileFolderHierarchyExists(codePath);

                        // Write file
                        System.IO.File.WriteAllText(codePath, CodeGenerators.SettingsCodeGenerator.Build(Core.Config, WorkingConfig));
                        AssetDatabase.ImportAsset("Assets/Generated/GDXSettings.cs");
                    };

                    // Handle Links
                    Button buttonRepository = rootElement.Q<Button>("button-repository");
                    buttonRepository.clicked += () =>
                    {
                        UnityEngine.Application.OpenURL("https://github.com/dotBunny/GDX/");
                    };
                    Button buttonDocumentation = rootElement.Q<Button>("button-documentation");
                    buttonDocumentation.clicked += () =>
                    {
                        UnityEngine.Application.OpenURL("https://gdx.dotbunny.com/");
                    };
                    Button buttonIssue = rootElement.Q<Button>("button-issue");
                    buttonIssue.clicked += () =>
                    {
                        UnityEngine.Application.OpenURL("https://github.com/dotBunny/GDX/issues");
                    };

                    VisualElement packageHolderElement = rootElement.Q<VisualElement>("gdx-project-settings-packages");
                    packageHolderElement.Add(GetPackageStatus("Addressables", Developer.Conditionals.HasAddressablesPackage));
                    packageHolderElement.Add(GetPackageStatus("Platforms", Developer.Conditionals.HasPlatformsPackage));
                    packageHolderElement.Add(GetPackageStatus("Visual Scripting", Developer.Conditionals.HasVisualScriptingPackage));

                    // Build some useful references
                    ScrollView contentScrollView = rootElement.Q<ScrollView>("gdx-project-settings-content");

                    // Update first state
                    CheckForChanges();

                    ConfigSectionsProvider.ClearSectionCache();


                    // Create ordered list of sections
                    List<IConfigSection> sections = new List<IConfigSection>(ConfigSections.Count);
                    int iterator = 0;
                    while (ConfigSections.MoveNext(ref iterator, out StringKeyEntry<IConfigSection> item))
                    {
                        sections.Add(item.value);
                    }
                    sections.Sort((lhs, rhs) =>
                    {
                        int l = lhs.GetPriority();
                        int r = rhs.GetPriority();
                        if (l > r) return -1;
                        if (l < r) return 1;
                        return 0;
                    });

                    foreach (IConfigSection section in sections)
                    {
                        string sectionID = section.GetSectionID();
                        VisualElement sectionHeader = ConfigSectionsProvider.CreateAndBindSectionHeader(section);

                        contentScrollView.contentContainer.Add(sectionHeader);
                        ConfigSectionsProvider.UpdateSectionHeader(sectionID);

                        VisualElement sectionContentBase = ConfigSectionsProvider.CreateAndBindSectionContent(section);

                        contentScrollView.contentContainer.Add(sectionContentBase);
                        ConfigSectionsProvider.UpdateSectionContent(sectionID);
                    }
                },
                keywords = s_searchKeywords
            };
        }

        private static VisualElement GetPackageStatus(string package, bool status)
        {
            VisualTreeAsset packageStatusAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsPackageStatus");

            VisualElement newInstance = packageStatusAsset.Instantiate()[0];

            Label label = newInstance.Q<Label>("label-package");
            label.text = package;

            VisualElement statusElement = newInstance.Q<VisualElement>("element-status");
            if (status)
            {
                statusElement.AddToClassList("found");
            }

            return newInstance;
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns></returns>
        public static bool GetCachedEditorBoolean(string id, bool defaultValue = true)
        {
            if (s_cachedEditorPreferences.IndexOf(id) == -1)
            {
                s_cachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_cachedEditorPreferences[id];
        }


        public static void RegisterConfigSection(IConfigSection section)
        {
            if(ConfigSections.ContainsKey(section.GetSectionID())) return;
            ConfigSections.AddWithExpandCheck(section.GetSectionID(), section);
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

        public static void CheckForChanges()
        {
            if (!Core.Config.Compare(WorkingConfig))
            {
                _changesElement.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
            }
            else
            {
                _changesElement.AddToClassList(ConfigSectionsProvider.HiddenClass);
            }
        }

    }
}