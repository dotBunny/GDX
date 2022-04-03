// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using GDX.Collections.Generic;
using GDX.Editor.ProjectSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    /// <summary>
    ///     GDX Assembly Settings Provider
    /// </summary>
    [HideFromDocFX]
    public static class ProjectSettingsProvider
    {

        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string DocumentationUri = "https://gdx.dotbunny.com/";

        public const int SectionCount = 8;

        public static StringKeyDictionary<IConfigSection> ConfigSections = new StringKeyDictionary<IConfigSection>(SectionCount);

        public static TransientConfig WorkingConfig;
        public static string SearchString;

        /// <summary>
        ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
        /// </summary>
        static StringKeyDictionary<bool> s_CachedEditorPreferences = new StringKeyDictionary<bool>(30);

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        static string[] s_SearchKeywords;


        static VisualElement s_ChangesElement;
        static VisualElement s_RootElement;
        static VisualElement s_HolderElement;
        static Button s_ClearButton;
        static Button s_SaveButton;

        static EditorWindow s_ProjectSettingsWindow;

        /// <summary>
        ///     Get <see cref="UnityEditor.SettingsProvider" /> for GDX assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static UnityEditor.SettingsProvider Get()
        {
            // Create our working copy of the config - we do this to catch if theres an override that happens
            // during domain reload callbacks
            WorkingConfig ??= new TransientConfig();

            // Initialize some things here instead of static initializers
            List<string> keywords = new List<string>(100);

            // Register settings
            if (!ConfigSections.ContainsKey(AutomaticUpdatesSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(AutomaticUpdatesSettings.SectionKey, new AutomaticUpdatesSettings());
                keywords.AddUniqueRange(ConfigSections[AutomaticUpdatesSettings.SectionKey].GetSearchKeywords());
            }
            if (!ConfigSections.ContainsKey(ConfigSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(ConfigSettings.SectionKey, new ConfigSettings());
                keywords.AddUniqueRange(ConfigSections[ConfigSettings.SectionKey].GetSearchKeywords());
            }
            if (!ConfigSections.ContainsKey(BuildInfoSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(BuildInfoSettings.SectionKey, new BuildInfoSettings());
                keywords.AddUniqueRange(ConfigSections[BuildInfoSettings.SectionKey].GetSearchKeywords());
            }
            if (!ConfigSections.ContainsKey(CommandLineProcessorSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(CommandLineProcessorSettings.SectionKey, new CommandLineProcessorSettings());
                keywords.AddUniqueRange(ConfigSections[CommandLineProcessorSettings.SectionKey].GetSearchKeywords());
            }
            if (!ConfigSections.ContainsKey(EnvironmentSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(EnvironmentSettings.SectionKey, new EnvironmentSettings());
                keywords.AddUniqueRange(ConfigSections[EnvironmentSettings.SectionKey].GetSearchKeywords());

            }
            if (!ConfigSections.ContainsKey(PlatformSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(PlatformSettings.SectionKey, new PlatformSettings());
                keywords.AddUniqueRange(ConfigSections[PlatformSettings.SectionKey].GetSearchKeywords());
            }
            if (!ConfigSections.ContainsKey(LocaleSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(LocaleSettings.SectionKey, new LocaleSettings());
                keywords.AddUniqueRange(ConfigSections[LocaleSettings.SectionKey].GetSearchKeywords());
            }
#if GDX_VISUALSCRIPTING
            if (!ConfigSections.ContainsKey(VisualScriptingSettings.SectionKey))
            {
                ConfigSections.AddUnchecked(VisualScriptingSettings.SectionKey, new VisualScriptingSettings());
                keywords.AddUniqueRange(ConfigSections[VisualScriptingSettings.SectionKey].GetSearchKeywords());
            }
#endif
            s_SearchKeywords = keywords.ToArray();

            // Prebuild Content
            SearchProvider.Reset();



            return new UnityEditor.SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                activateHandler = (searchContext, rootElement) =>
                {
                    s_RootElement = rootElement;
                    SearchProvider.Reset();

                    // Add base style sheet
                    if (ResourcesProvider.GetStyleSheet() != null)
                    {
                        rootElement.styleSheets.Add(ResourcesProvider.GetStyleSheet());
                    }

                    // Add a light mode style sheet if we have to
                    if (!EditorGUIUtility.isProSkin)
                    {
                        if (ResourcesProvider.GetLightThemeStylesheet() != null)
                        {
                            rootElement.styleSheets.Add(ResourcesProvider.GetLightThemeStylesheet());
                        }
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

                    s_ChangesElement = rootElement.Q<VisualElement>("gdx-config-changes");

                    // Handle state buttons
                    s_ClearButton = s_ChangesElement.Q<Button>("button-clear-changes");
                    s_ClearButton.clicked += () =>
                    {
                        WorkingConfig = new TransientConfig();
                        ConfigSectionsProvider.UpdateAll();
                        CheckForChanges();
                    };

                    s_SaveButton = s_ChangesElement.Q<Button>("button-save-changes");
                    s_SaveButton.clicked += () =>
                    {
                        AssetDatabase.StartAssetEditing();

                        // Remove old file
                        string previousPath =
                            Path.Combine(Application.dataPath, Config.ConfigOutputPath);
                        if (File.Exists(previousPath))
                        {
                            AssetDatabase.DeleteAsset(Path.Combine("Assets", Config.ConfigOutputPath));
                        }

                        if (!WorkingConfig.HasChanges())
                        {
                            // Generate new file
                            string codePath = Path.Combine(Application.dataPath,
                                WorkingConfig.ConfigOutputPath);

                            // Ensure folder structure is present
                            Platform.EnsureFileFolderHierarchyExists(codePath);

                            // Write file
                            File.WriteAllText(codePath, ConfigGenerator.Build(WorkingConfig));

                            string projectRelative =
                                Path.Combine("Assets", WorkingConfig.ConfigOutputPath);

                            AssetDatabase.StopAssetEditing();
                            AssetDatabase.ImportAsset(projectRelative);
                        }
                        else
                        {
                            AssetDatabase.StopAssetEditing();
                        }
                    };

                    // Handle Links
                    Button buttonRepository = rootElement.Q<Button>("button-repository");
                    buttonRepository.clicked += () =>
                    {
                        Application.OpenURL("https://github.com/dotBunny/GDX/");
                    };
                    Button buttonDocumentation = rootElement.Q<Button>("button-documentation");
                    buttonDocumentation.clicked += () =>
                    {
                        Application.OpenURL("https://gdx.dotbunny.com/");
                    };
                    Button buttonIssue = rootElement.Q<Button>("button-issue");
                    buttonIssue.clicked += () =>
                    {
                        Application.OpenURL("https://github.com/dotBunny/GDX/issues");
                    };

                    VisualElement packageHolderElement = rootElement.Q<VisualElement>("gdx-project-settings-packages");
                    packageHolderElement.Add(GetPackageStatus("Addressables",
                        Developer.Conditionals.HasAddressablesPackage));
                    packageHolderElement.Add(GetPackageStatus("Platforms", Developer.Conditionals.HasPlatformsPackage));
                    packageHolderElement.Add(GetPackageStatus("Visual Scripting",
                        Developer.Conditionals.HasVisualScriptingPackage));

                    // Build some useful references
                    ScrollView contentScrollView = rootElement.Q<ScrollView>("gdx-project-settings-content");

                    // Update first state
                    CheckForChanges();

                    ConfigSectionsProvider.ClearSectionCache();

                    // Create ordered list of sections
                    int iterator = 0;
                    while (ConfigSections.MoveNext(ref iterator, out StringKeyEntry<IConfigSection> item))
                    {
                        IConfigSection section = item.Value;
                        string sectionID = section.GetSectionKey();

                        VisualElement sectionHeader = ConfigSectionsProvider.CreateAndBindSectionHeader(section);

                        contentScrollView.contentContainer.Add(sectionHeader);
                        ConfigSectionsProvider.UpdateSectionHeader(sectionID);

                        VisualElement sectionContentBase =
                            ConfigSectionsProvider.CreateAndBindSectionContent(section);

                        contentScrollView.contentContainer.Add(sectionContentBase);
                        ConfigSectionsProvider.UpdateSectionContent(sectionID);
                    }
                },
                keywords = s_SearchKeywords,
                hasSearchInterestHandler = (searchString) => s_SearchKeywords.PartialMatch(searchString),
                inspectorUpdateHandler = () =>
                {
                    if (s_ProjectSettingsWindow == null)
                    {
                        s_ProjectSettingsWindow = SettingsService.OpenProjectSettings();
                    }
                    if (s_ProjectSettingsWindow == null)
                    {
                        return;
                    }

                    string searchContext = Reflection.GetFieldValue<string>(
                        s_ProjectSettingsWindow, "UnityEditor.SettingsWindow", "m_SearchText");

                    if (SearchString == searchContext)
                        return;

                    if (string.IsNullOrEmpty(searchContext))
                    {
                        s_RootElement.RemoveFromClassList(ResourcesProvider.SearchClass);
                    }
                    else
                    {
                        s_RootElement.AddToClassList(ResourcesProvider.SearchClass);
                    }
                    SearchString = searchContext;
                    ConfigSectionsProvider.UpdateAll();
                }
            };
        }

        static VisualElement GetPackageStatus(string package, bool status)
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
            if (s_CachedEditorPreferences.IndexOf(id) == -1)
            {
                s_CachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_CachedEditorPreferences[id];
        }

        public static bool IsSearching()
        {
            if (s_RootElement == null) return false;
            return s_RootElement.ClassListContains(ResourcesProvider.SearchClass);
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetCachedEditorBoolean(string id, bool setValue)
        {
            if (!s_CachedEditorPreferences.ContainsKey(id))
            {
                s_CachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
            else
            {
                if (s_CachedEditorPreferences[id] == setValue)
                {
                    return;
                }

                s_CachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
        }

        public static void CheckForChanges()
        {
            if (!WorkingConfig.HasChanges())
            {
                s_ChangesElement.RemoveFromClassList(ResourcesProvider.HiddenClass);
            }
            else
            {
                s_ChangesElement.AddToClassList(ResourcesProvider.HiddenClass);
            }
        }

    }
}