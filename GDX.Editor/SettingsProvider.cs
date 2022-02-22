// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using GDX.Collections.Generic;
using GDX.Editor.ProjectSettings;
using UnityEditor;
using UnityEngine.UIElements;

// ReSharper disable UnusedMember.Global

namespace GDX.Editor
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

        public const int SectionCount = 7;

        /// <summary>
        ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
        /// </summary>
        private static StringKeyDictionary<bool> s_CachedEditorPreferences = new StringKeyDictionary<bool>(30);

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static List<string> s_SearchKeywords;

        public static StringKeyDictionary<IConfigSection> ConfigSections = new StringKeyDictionary<IConfigSection>(SectionCount);

        public static GDXConfig WorkingConfig;

        private static VisualElement s_ChangesElement;
        private static Button s_ClearButton;
        private static Button s_SaveButton;


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

            // Initialize some things here instead of static initializers
            s_SearchKeywords ??= new List<string>(new[]
            {
                // ReSharper disable once StringLiteralTypo
                "gdx", "automatic", "package", "update", "buildinfo", "task", "stream", "changelist",
                "cli", "argument", "environment", "trace", "symbol", "locale", "localization", "culture",
                "visual", "scripting", "vs", "parser", "commandline", "build"
            });

            // Register settings
            if (!ConfigSections.ContainsKey(AutomaticUpdatesSettings.SectionID))
            {
                ConfigSections.AddUnchecked(AutomaticUpdatesSettings.SectionID, new AutomaticUpdatesSettings());
            }
            if (!ConfigSections.ContainsKey(ConfigSettings.SectionID))
            {
                ConfigSections.AddUnchecked(ConfigSettings.SectionID, new ConfigSettings());
            }
            if (!ConfigSections.ContainsKey(BuildInfoSettings.SectionID))
            {
                ConfigSections.AddUnchecked(BuildInfoSettings.SectionID, new BuildInfoSettings());
            }
            if (!ConfigSections.ContainsKey(CommandLineProcessorSettings.SectionID))
            {
                ConfigSections.AddUnchecked(CommandLineProcessorSettings.SectionID, new CommandLineProcessorSettings());
            }
            if (!ConfigSections.ContainsKey(EnvironmentSettings.SectionID))
            {
                ConfigSections.AddUnchecked(EnvironmentSettings.SectionID, new EnvironmentSettings());
            }
            if (!ConfigSections.ContainsKey(LocaleSettings.SectionID))
            {
                ConfigSections.AddUnchecked(LocaleSettings.SectionID, new LocaleSettings());
            }
#if GDX_VISUALSCRIPTING
            if (!ConfigSections.ContainsKey(VisualScriptingSettings.SectionID))
            {
                ConfigSections.AddUnchecked(VisualScriptingSettings.SectionID, new VisualScriptingSettings());
            }
#endif

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new UnityEditor.SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                // ReSharper disable once UnusedParameter.Local
                activateHandler = (searchContext, rootElement) =>
                {
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
                        WorkingConfig = new GDXConfig(Core.Config);
                        ConfigSectionsProvider.UpdateAll();
                        CheckForChanges();
                    };

                    s_SaveButton = s_ChangesElement.Q<Button>("button-save-changes");
                    s_SaveButton.clicked += () =>
                    {
                        AssetDatabase.StartAssetEditing();

                        // Remove old file
                        string previousPath = Path.Combine(UnityEngine.Application.dataPath, Core.Config.ConfigOutputPath);
                        if (File.Exists(previousPath))
                        {
                            AssetDatabase.DeleteAsset(Path.Combine("Assets",  Core.Config.ConfigOutputPath));
                        }

                        GDXConfig baseConfig = new GDXConfig();
                        if (!baseConfig.Compare(WorkingConfig))
                        {
                            // Generate new file
                            string codePath = Path.Combine(UnityEngine.Application.dataPath,
                                WorkingConfig.ConfigOutputPath);

                            // Ensure folder structure is present
                            Platform.EnsureFileFolderHierarchyExists(codePath);

                            // Write file
                            File.WriteAllText(codePath, SettingsGenerator.Build(baseConfig, WorkingConfig));

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
                    // ReSharper disable once StringLiteralTypo
                    packageHolderElement.Add(GetPackageStatus("Addressables", Developer.Conditionals.HasAddressablesPackage));
                    packageHolderElement.Add(GetPackageStatus("Platforms", Developer.Conditionals.HasPlatformsPackage));
                    packageHolderElement.Add(GetPackageStatus("Visual Scripting", Developer.Conditionals.HasVisualScriptingPackage));

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
                        string sectionID = section.GetSectionID();
                        VisualElement sectionHeader = ConfigSectionsProvider.CreateAndBindSectionHeader(section);

                        contentScrollView.contentContainer.Add(sectionHeader);
                        ConfigSectionsProvider.UpdateSectionHeader(sectionID);

                        VisualElement sectionContentBase = ConfigSectionsProvider.CreateAndBindSectionContent(section);

                        contentScrollView.contentContainer.Add(sectionContentBase);
                        ConfigSectionsProvider.UpdateSectionContent(sectionID);
                    }
                },
                keywords = s_SearchKeywords
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
            if (s_CachedEditorPreferences.IndexOf(id) == -1)
            {
                s_CachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_CachedEditorPreferences[id];
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
            if (!Core.Config.Compare(WorkingConfig))
            {
                s_ChangesElement.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
            }
            else
            {
                s_ChangesElement.AddToClassList(ConfigSectionsProvider.HiddenClass);
            }
        }

    }
}