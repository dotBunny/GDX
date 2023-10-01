// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using GDX.Collections.Generic;
using GDX.Developer;
using GDX.Editor.Windows.ProjectSettings.ConfigSections;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings
{
    /// <summary>
    ///     A settings provider scoped to the project which is backed by the <see cref="Config" />.
    /// </summary>
    /// <remarks>
    ///     This operates under the assumption that there can only ever be one Project Settings window.
    /// </remarks>
    [HideFromDocFX]
    public static class ProjectSettingsProvider
    {
        /// <summary>
        ///     The expected number of sections to be available in the project settings.
        /// </summary>
        const int k_SectionCount = 9;

        /// <summary>
        ///     The currently known search string of the Project Settings window.
        /// </summary>
        static string s_SearchString;

        /// <summary>
        ///     The unsaved working copy of the <see cref="Config" /> which the project settings operates on.
        /// </summary>
        public static TransientConfig WorkingConfig;

        /// <summary>
        ///     A mapping of section's content <see cref="VisualElement" /> to <see cref="IConfigSection" /> by its index.
        /// </summary>
        static readonly VisualElement[] k_ConfigSectionContents = new VisualElement[k_SectionCount];

        /// <summary>
        ///     A mapping of section's header <see cref="VisualElement" /> to <see cref="IConfigSection" /> by its index.
        /// </summary>
        static readonly VisualElement[] k_ConfigSectionHeaders = new VisualElement[k_SectionCount];

        /// <summary>
        ///     A mapping of the actual <see cref="IConfigSection" /> by its index.
        /// </summary>
        static readonly IConfigSection[] k_ConfigSections = new IConfigSection[k_SectionCount];

        /// <summary>
        ///     A list of keywords to exclude from the parsed keywords, these are just common words which are not
        ///     relevant when it comes to searching.
        /// </summary>
        static readonly int[] k_SearchKeywordExclusions =
        {
            "the".GetStableHashCode(), "for".GetStableHashCode(), "and".GetStableHashCode(),
            "can".GetStableHashCode(), "its".GetStableHashCode(), "not".GetStableHashCode(),
            "all".GetStableHashCode(), "like".GetStableHashCode(), "that".GetStableHashCode(),
            "then".GetStableHashCode(), "with".GetStableHashCode()
        };

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        static SimpleList<string> s_SearchKeywords = new SimpleList<string>(150);

        static SimpleList<int> s_SearchKeywordsHashes = new SimpleList<int>(150);

        /// <summary>
        ///     A map of search keywords to individual <see cref="VisualElement" />s which should be highlighted.
        /// </summary>
        static readonly Dictionary<int, List<VisualElement>> k_SearchKeywordMap =
            new Dictionary<int, List<VisualElement>>(150);

        static SimpleList<VisualElement> s_SearchContentResults = new SimpleList<VisualElement>(10);

        /// <summary>
        ///     A map of <see cref="VisualElement" />s that have been processed for keyword search, to their
        ///     <see cref="IConfigSection" /> by section index.
        /// </summary>
        static readonly SimpleList<VisualElement>[] k_SearchSectionElementMap =
        {
            new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10),
            new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10),
            new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10), new SimpleList<VisualElement>(10)
        };


        static VisualElement s_ChangesElement;
        static VisualElement s_RootElement;
        static VisualElement s_HolderElement;
        static VisualElement s_PackageBadgeElement;
        static GenericMenu s_PopupMenu;

        static Button s_ClearButton;
        static Button s_ExtrasButton;
        static Button s_SaveButton;
        static ScrollView s_ChangelogScrollView;

        static EditorWindow s_ProjectSettingsWindow;


        static void CacheSectionContent(int sectionIndex)
        {
            // We already have something made, we don't want to redo the work
            if (k_ConfigSectionContents[sectionIndex] != null)
            {
                return;
            }

            IConfigSection section = k_ConfigSections[sectionIndex];

            VisualTreeAsset sectionAsset =
                ResourcesProvider.GetVisualTreeAsset(section.GetTemplateName());

            VisualElement sectionInstance = sectionAsset.Instantiate()[0];

            section.BindSectionContent(sectionInstance);

            // Record the whole section
            k_ConfigSectionContents[sectionIndex] = sectionInstance;
        }

        static void CacheSectionHeader(int sectionIndex)
        {
            if (k_ConfigSectionHeaders[sectionIndex] != null)
            {
                return;
            }

            IConfigSection section = k_ConfigSections[sectionIndex];

            VisualTreeAsset headerAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsSectionHeader");

            VisualElement headerInstance = headerAsset.Instantiate()[0];

            // Expansion
            Button expandButton = headerInstance.Q<Button>("button-expand");
            if (expandButton != null)
            {
                expandButton.clicked += () =>
                {
                    OnExpandSectionHeaderClicked(sectionIndex);
                    UpdateSectionHeader(sectionIndex);
                    UpdateSectionContent(sectionIndex);
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
                    string helpLink = $"{PackageProvider.GetDocumentationBaseUri()}{section.GetSectionHelpLink()}";
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
                        OnToggleSectionHeaderClicked(enabledToggle, sectionIndex, evt.newValue);
                    });
                }
                else
                {
                    enabledToggle.visible = false;
                    enabledToggle.tooltip = null;
                }
            }

            // Send back created instance
            k_ConfigSectionHeaders[sectionIndex] = headerInstance;
        }

        /// <summary>
        ///     Get the <see cref="UnityEditor.SettingsProvider" /> for GDX.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider Get()
        {
            Initialize();
            return new SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                // ReSharper disable once UnusedParameter.Local
                activateHandler = (searchContext, rootElement) =>
                {
                    s_RootElement = rootElement;

                    ResourcesProvider.SetupSharedStylesheets(rootElement);
                    ResourcesProvider.SetupStylesheet("GDXProjectSettings", rootElement);
                    ResourcesProvider.GetVisualTreeAsset("GDXProjectSettings").CloneTree(rootElement);
                    ResourcesProvider.CheckTheme(rootElement);

                    s_ChangesElement = rootElement.Q<VisualElement>("gdx-config-changes");

                    // Handle state buttons
                    s_ClearButton = s_ChangesElement.Q<Button>("button-clear-changes");
                    s_ClearButton.clicked += () =>
                    {
                        WorkingConfig = new TransientConfig();
                        UpdateAllSections();
                        UpdateForChanges();
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
                    Button buttonDocumentation = rootElement.Q<Button>("button-documentation");
                    buttonDocumentation.clicked += () =>
                    {
                        Application.OpenURL("https://gdx.dotbunny.com/");
                    };
                    Button buttonIssue = rootElement.Q<Button>("button-issue");
                    buttonIssue.clicked += () =>
                    {
                        Application.OpenURL("https://dotbunny.youtrack.cloud/youtrack/issues/GDX");
                    };
                    Button buttonLicense = rootElement.Q<Button>("button-license");
                    buttonLicense.clicked += () =>
                    {
                        Application.OpenURL(
                            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork
                                ? "https://github.com/dotBunny/GDX/blob/dev/LICENSE"
                                : UpdateProvider.GetLocalLicensePath());
                    };

                    VisualElement banner = rootElement.Q<VisualElement>("gdx-banner");
                    banner.style.backgroundImage = new StyleBackground(ResourcesProvider.GetBanner());
                    s_ExtrasButton = rootElement.Q<Button>("gdx-extras-button");
                    s_ExtrasButton.style.backgroundImage = new StyleBackground(ResourcesProvider.GetLogo());
                    s_ExtrasButton.clicked += ShowPopupMenu;

                    s_ChangelogScrollView = rootElement.Q<ScrollView>("gdx-changelog");
                    ChangelogProvider.StartTask(s_ChangelogScrollView.contentContainer);
                    rootElement.Q<Label>("gdx-version").text = UpdateProvider.LocalPackage.Definition.version;

                    // Badges
                    s_PackageBadgeElement = rootElement.Q<VisualElement>("gdx-packages");
                    AddPackageBadge(s_PackageBadgeElement, "Addressables",
                        Conditionals.HasAddressablesPackage);
                    AddPackageBadge(s_PackageBadgeElement, "Platforms",
                        Conditionals.HasPlatformsPackage);
                    AddPackageBadge(s_PackageBadgeElement, "Visual Scripting",
                        Conditionals.HasVisualScriptingPackage);

                    // Build some useful references
                    ScrollView contentScrollView = rootElement.Q<ScrollView>("gdx-project-settings-content");

                    // Update first state
                    UpdateForChanges();

                    // Create ordered list of sections
                    for (int i = 0; i < k_SectionCount; i++)
                    {
                        IConfigSection section = k_ConfigSections[i];
                        if (section == null)
                        {
                            continue;
                        }

                        contentScrollView.contentContainer.Add(k_ConfigSectionHeaders[i]);
                        UpdateSectionHeader(i);

                        contentScrollView.contentContainer.Add(k_ConfigSectionContents[i]);
                        UpdateSectionContent(i);
                    }
                },
                hasSearchInterestHandler = searchString => s_SearchKeywords.PartialMatch(searchString),
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

                    Reflection.TryGetFieldValue(
                        s_ProjectSettingsWindow, Reflection.GetType("UnityEditor.SettingsWindow"), "m_SearchText",
                        out string searchContext);

                    if (s_SearchString == searchContext)
                    {
                        return;
                    }

                    if (string.IsNullOrEmpty(searchContext))
                    {
                        s_RootElement.RemoveFromClassList(ResourcesProvider.SearchClass);
                        for (int i = 0; i < k_SectionCount; i++)
                        {
                            k_ConfigSectionHeaders[i]?.RemoveFromClassList(ResourcesProvider.HiddenClass);
                            k_ConfigSectionContents[i]?.RemoveFromClassList(ResourcesProvider.HiddenClass);
                        }
                    }
                    else
                    {
                        s_RootElement.AddToClassList(ResourcesProvider.SearchClass);
                    }

                    s_SearchString = searchContext;

                    UpdateForSearch();
                    UpdateAllSections();
                }
            };
        }


        static void AddPackageBadge(VisualElement container, string content, bool found)
        {
            Label packageLabel = new Label(content);
            packageLabel.AddToClassList("gdx-badge");
            packageLabel.AddToClassList("gdx-package");
            packageLabel.AddToClassList(found ? "gdx-package-found" : "gdx-package-not-found");
            container.Add(packageLabel);
        }

        static void ShowPopupMenu()
        {
            if (s_PopupMenu == null)
            {
                s_PopupMenu = new GenericMenu();

                s_PopupMenu.AddItem(new GUIContent("About dotBunny"), false, () =>
                {
                    Application.OpenURL("https://dotbunny.com/about");
                });

                s_PopupMenu.AddSeparator("");

                s_PopupMenu.AddItem(new GUIContent("Changelog"), false, () =>
                {
                    Application.OpenURL(UpdateProvider.GetLocalChangelogPath());
                });

                s_PopupMenu.AddItem(new GUIContent("Git Repository"), false, () =>
                {
                    Application.OpenURL("https://github.com/dotBunny/GDX/");
                });

                s_PopupMenu.AddSeparator("");

                s_PopupMenu.AddItem(new GUIContent("Expand Sections"), false, () =>
                {
                    // Open
                    for (int i = 0; i < k_SectionCount; i++)
                    {
                        string sectionKey = k_ConfigSections[i].GetSectionKey();
                        EditorPrefsCache.SetBoolean(sectionKey, true);
                        UpdateSectionHeader(i);
                        UpdateSectionContent(i);
                    }
                });
                s_PopupMenu.AddItem(new GUIContent("Collapse Sections"), false, () =>
                {
                    // Close
                    for (int i = 0; i < k_SectionCount; i++)
                    {
                        string sectionKey = k_ConfigSections[i].GetSectionKey();
                        EditorPrefsCache.SetBoolean(sectionKey, false);
                        UpdateSectionHeader(i);
                        UpdateSectionContent(i);
                    }
                });
            }

            s_PopupMenu.ShowAsContext();
        }

        static void QueryElements(string searchContext)
        {
            s_SearchContentResults.Clear();
            int count = s_SearchKeywords.Count;
            for (int i = 0; i < count; i++)
            {
                string keyword = s_SearchKeywords.Array[i];
                if (!keyword.Contains(searchContext))
                {
                    continue;
                }

                int hash = s_SearchKeywordsHashes.Array[i];
                int elementCount = k_SearchKeywordMap[hash].Count;
                for (int j = 0; j < elementCount; j++)
                {
                    s_SearchContentResults.AddWithExpandCheckUniqueItem(k_SearchKeywordMap[hash][j]);
                }
            }
        }

        public static void Reset()
        {
            WorkingConfig = null;

            // Register settings - if necessary
            for (int i = 0; i < k_SectionCount; i++)
            {
                k_ConfigSections[i] = null;
                k_ConfigSectionContents[i] = null;
                k_ConfigSectionHeaders[i] = null;
                k_SearchSectionElementMap[i] = new SimpleList<VisualElement>(10);
            }

            s_SearchKeywords.Clear();
            s_SearchKeywordsHashes.Clear();
            k_SearchKeywordMap.Clear();
        }

        static void Initialize()
        {
            // Create our working copy of the config - we do this to catch if theres an override that happens
            // during domain reload callbacks.
            WorkingConfig = new TransientConfig();

            // Register settings - if necessary
            k_ConfigSections[AutomaticUpdatesSettings.SectionIndex] ??= new AutomaticUpdatesSettings();
            k_ConfigSections[ConfigSettings.SectionIndex] ??= new ConfigSettings();
            k_ConfigSections[BuildInfoSettings.SectionIndex] ??= new BuildInfoSettings();
            k_ConfigSections[CommandLineProcessorSettings.SectionIndex] ??= new CommandLineProcessorSettings();
            k_ConfigSections[EnvironmentSettings.SectionIndex] ??= new EnvironmentSettings();
            k_ConfigSections[PlatformSettings.SectionIndex] ??= new PlatformSettings();
            k_ConfigSections[LocaleSettings.SectionIndex] ??= new LocaleSettings();
            k_ConfigSections[TaskDirectorSettings.SectionIndex] ??= new TaskDirectorSettings();
#if GDX_VISUALSCRIPTING
            k_ConfigSections[VisualScriptingSettings.SectionIndex] ??= new VisualScriptingSettings();
#endif // GDX_VISUALSCRIPTING


            // Prewarm Section Elements
            for (int i = 0; i < k_SectionCount; i++)
            {
                // Check for non built, just skip
                if (k_ConfigSections[i] == null)
                {
                    continue;
                }

                CacheSectionHeader(i);
                CacheSectionContent(i);
            }
        }

        static bool IsSearching()
        {
            return !string.IsNullOrEmpty(s_SearchString);
        }

        static void OnExpandSectionHeaderClicked(int sectionIndex)
        {
            GUIUtility.hotControl = 0;
            IConfigSection section = k_ConfigSections[sectionIndex];
            string sectionKey = section.GetSectionKey();
            bool setting = EditorPrefsCache.GetBoolean(sectionKey, section.GetDefaultVisibility());
            EditorPrefsCache.SetBoolean(sectionKey, !setting);
        }

        static void OnToggleSectionHeaderClicked(VisualElement toggleElement, int sectionIndex, bool newValue)
        {
            // Do not toggle during search mode
            if (IsSearching())
            {
                return;
            }

            IConfigSection section = k_ConfigSections[sectionIndex];
            section.SetToggleState(toggleElement, newValue);

            UpdateSectionContent(sectionIndex);
            UpdateSectionHeader(sectionIndex);
        }


        static void FindValidWords(ref SimpleList<string> validWords, string content)
        {
            SegmentedString splitString = SegmentedString.SplitOnNonAlphaNumericToLowerHashed(content);
            int count = splitString.GetCount();
            for (int i = 0; i < count; i++)
            {
                if (splitString.GetSegmentLength(i) < 3)
                {
                    continue;
                }

                // Check for exclusions
                if (k_SearchKeywordExclusions.ContainsValue(splitString.GetHashCode(i)))
                {
                    continue;
                }

                validWords.AddWithExpandCheckUniqueValue(splitString.AsString(i));
            }
        }

        public static void RegisterElementForSearch(int sectionIndex, VisualElement element,
            string additionalDescription = null)
        {
            // Create our working list
            SimpleList<string> validWords = new SimpleList<string>(25);

            switch (element)
            {
                case TextField textField:
                    FindValidWords(ref validWords, textField.label);
                    FindValidWords(ref validWords, textField.tooltip);
                    break;
                case EnumField enumField:
                    FindValidWords(ref validWords, enumField.label);
                    FindValidWords(ref validWords, enumField.tooltip);
                    break;
                case Toggle toggleField:
                    FindValidWords(ref validWords, toggleField.label);
                    FindValidWords(ref validWords, toggleField.tooltip);
                    break;
                case MaskField maskField:
                    FindValidWords(ref validWords, maskField.label);
                    FindValidWords(ref validWords, maskField.tooltip);
                    break;
                case Slider sliderField:
                    FindValidWords(ref validWords, sliderField.label);
                    FindValidWords(ref validWords, sliderField.tooltip);
                    break;
            }

            // Passed in words
            if (additionalDescription != null)
            {
                FindValidWords(ref validWords, additionalDescription);
            }

            // Build Map
            int validWordsCount = validWords.Count;
            for (int i = 0; i < validWordsCount; i++)
            {
                string word = validWords.Array[i];
                int hash = word.GetStableHashCode();
                if (!s_SearchKeywordsHashes.ContainsValue(hash))
                {
                    s_SearchKeywords.AddWithExpandCheck(word);
                    s_SearchKeywordsHashes.AddWithExpandCheck(hash);
                    k_SearchKeywordMap.Add(hash, new List<VisualElement>(10));
                }

                k_SearchKeywordMap[hash].Add(element);
            }

            // Register element to a section
            k_SearchSectionElementMap[sectionIndex].AddWithExpandCheckUniqueItem(element);
        }

        public static void SetClassChangeCheck<T1, T2>(T1 element, T2 lhs, T2 rhs)
            where T1 : BaseField<T2> where T2 : class
        {
            if (element == null)
            {
                return;
            }

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

        public static void SetEnumChangeCheck<T>(EnumField element, T lhs, T rhs) where T : Enum
        {
            if (element == null)
            {
                return;
            }

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
            if (element == null)
            {
                return;
            }

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
            if (element == null)
            {
                return;
            }

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

        static void UpdateAllSections()
        {
            for (int i = 0; i < k_SectionCount; i++)
            {
                if (k_ConfigSections[i] == null)
                {
                    continue;
                }

                UpdateSectionHeader(i);
                UpdateSectionContent(i);
            }
        }

        public static void UpdateForChanges()
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

        static void UpdateForSearch()
        {
            // Reset Previous Highlights
            int existingCount = s_SearchContentResults.Count;
            if (existingCount > 0)
            {
                for (int i = 0; i < existingCount; i++)
                {
                    if (s_SearchContentResults.Array[i] == null)
                    {
                        continue;
                    }

                    s_SearchContentResults.Array[i].RemoveFromClassList(ResourcesProvider.SearchHighlightClass);
                }

                s_SearchContentResults.Clear();
            }

            if (!IsSearching())
            {
                return;
            }

            QueryElements(s_SearchString);
            if (s_SearchContentResults.Count == 0)
            {
                for (int i = 0; i < k_SectionCount; i++)
                {
                    k_ConfigSectionHeaders[i].AddToClassList(ResourcesProvider.HiddenClass);
                    k_ConfigSectionContents[i].AddToClassList(ResourcesProvider.HiddenClass);
                }

                return;
            }

            // Iterate through each section, showing if it has found elements
            for (int i = 0; i < k_SectionCount; i++)
            {
                int count = k_SearchSectionElementMap[i].Count;
                bool found = false;
                for (int j = 0; j < count; j++)
                {
                    VisualElement element = k_SearchSectionElementMap[i].Array[j];
                    if (s_SearchContentResults.ContainsReference(element))
                    {
                        found = true;
                        element.AddToClassList(ResourcesProvider.SearchHighlightClass);
                    }
                    else
                    {
                        element.RemoveFromClassList(ResourcesProvider.SearchHighlightClass);
                    }
                }

                if (found)
                {
                    k_ConfigSectionHeaders[i]?.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    k_ConfigSectionContents[i]?.RemoveFromClassList(ResourcesProvider.HiddenClass);
                }
                else
                {
                    k_ConfigSectionHeaders[i]?.AddToClassList(ResourcesProvider.HiddenClass);
                    k_ConfigSectionContents[i]?.AddToClassList(ResourcesProvider.HiddenClass);
                }
            }
        }

        static void UpdateSectionContent(int sectionIndex)
        {
            // This can happen due to the order of how events fire.
            if (k_ConfigSectionContents[sectionIndex] == null)
            {
                return;
            }

            IConfigSection section = k_ConfigSections[sectionIndex];
            string sectionKey = section.GetSectionKey();

            VisualElement element = k_ConfigSectionContents[sectionIndex];

            if (!IsSearching())
            {
                // Default visible/hidden behaviour
                if (EditorPrefsCache.GetBoolean(sectionKey, section.GetDefaultVisibility()))
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

        static void UpdateSectionHeader(int sectionIndex)
        {
            // This can happen due to the order of how events fire.
            if (k_ConfigSectionContents[sectionIndex] == null || IsSearching())
            {
                return;
            }

            IConfigSection section = k_ConfigSections[sectionIndex];

            VisualElement sectionHeaderElement = k_ConfigSectionHeaders[sectionIndex];

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

            if (EditorPrefsCache.GetBoolean(section.GetSectionKey(), section.GetDefaultVisibility()))
            {
                sectionHeaderElement.AddToClassList(ResourcesProvider.ExpandedClass);
            }
            else
            {
                sectionHeaderElement.RemoveFromClassList(ResourcesProvider.ExpandedClass);
            }
        }
    }
}