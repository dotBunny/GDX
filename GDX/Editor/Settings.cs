// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using GDX.Developer;
using GDX.Editor.Build;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper HeapView.ObjectAllocation.Evident

namespace GDX.Editor
{
    /// <summary>
    ///     GDX Assembly Settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string DocumentationUri = "https://gdx.dotbunny.com/";

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        public static readonly List<string> SearchKeywords = new List<string>(new[]
        {
            "gdx", "update", "parser", "commandline", "build"
        });

        /// <summary>
        ///     The number of days between checks for updates.
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        public static int UpdateDayCountSetting
        {
            get => EditorPrefs.GetInt("GDX.UpdateProvider.UpdateDayCount", 7);
            private set => EditorPrefs.SetInt("GDX.UpdateProvider.UpdateDayCount", value);
        }

        /// <summary>
        ///     Get <see cref="SettingsProvider" /> for GDX assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident

            return new SettingsProvider("Project/GDX", SettingsScope.Project)
            {
                label = "GDX",
                guiHandler = searchContext =>
                {
                    // Get a serialized version of the settings
                    SerializedObject settings = ConfigProvider.GetSerializedConfig();

                    // Start wrapping the content
                    EditorGUILayout.BeginVertical(SettingsStyles.WrapperStyle);

                    SettingsSections.PackageStatus();

                    // Build out sections
                    SettingsSections.AutomaticUpdates(settings);
                    GUILayout.Space(5);
                    SettingsSections.BuildInfo(settings);
                    GUILayout.Space(5);
                    SettingsSections.CommandLineProcessor(settings);
                    GUILayout.Space(5);
                    SettingsSections.Locale(settings);

                    // Apply any serialized setting changes
                    settings.ApplyModifiedPropertiesWithoutUndo();

                    // Stop wrapping the content
                    EditorGUILayout.EndVertical();
                },
                keywords = SearchKeywords
            };
        }

        /// <summary>
        ///     A static collection of <see cref="GUIContent" /> used by <see cref="Settings" />.
        /// </summary>
        private static class SettingsContent
        {
            /// <summary>
            ///     Content for the initial introduction of the projects settings window.
            /// </summary>
            public static readonly GUIContent AboutBlurb = new GUIContent(
                "Game Development Extensions, a battle-tested library of game-ready high-performance C# code.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.updateProviderCheckForUpdates" />.
            /// </summary>
            public static readonly GUIContent AutomaticUpdatesEnabled = new GUIContent(
                "",
                "Should the package check the GitHub repository to see if there is a new version?");

            /// <summary>
            ///     Settings content for <see cref="Settings.UpdateDayCountSetting" />.
            /// </summary>
            public static readonly GUIContent AutomaticUpdatesUpdateDayCount = new GUIContent(
                "Update Timer (Days)",
                "After how many days should updates be checked for?");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoAssemblyDefinition" />.
            /// </summary>
            public static readonly GUIContent BuildInfoAssemblyDefinition = new GUIContent(
                "Assembly Definition",
                "Ensure that the folder of the BuildInfo has an assembly definition.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildChangelistArgument" />.
            /// </summary>
            public static readonly GUIContent BuildInfoBuildChangelistArgument = new GUIContent(
                "Changelist",
                "The argument key for the build changelist to be passed to the BuildInfoProvider.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildDescriptionArgument" />.
            /// </summary>
            public static readonly GUIContent BuildInfoBuildDescriptionArgument = new GUIContent(
                "Description",
                "The argument key for the build description to be passed to the BuildInfoProvider.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildNumberArgument" />.
            /// </summary>
            public static readonly GUIContent BuildInfoBuildNumberArgument = new GUIContent(
                "Number",
                "The argument key for the build number to be passed to the BuildInfoProvider.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildStreamArgument" />.
            /// </summary>
            public static readonly GUIContent BuildInfoBuildStreamArgument = new GUIContent(
                "Stream",
                "The argument key for the build stream to be passed to the BuildInfoProvider.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildTaskArgument" />.
            /// </summary>
            public static readonly GUIContent BuildInfoBuildTaskArgument = new GUIContent(
                "Task",
                "The argument key for the build task to be passed to the BuildInfoProvider.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoEnabled" />.
            /// </summary>
            public static readonly GUIContent BuildInfoEnabled = new GUIContent(
                "",
                "During the build process should a BuildInfo be written?");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoNamespace" />.
            /// </summary>
            public static readonly GUIContent BuildInfoNamespace = new GUIContent(
                "Namespace",
                "The namespace where the BuildInfo should be placed.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerBuildInfoPath" />.
            /// </summary>
            public static readonly GUIContent BuildInfoPath = new GUIContent(
                "Output Path",
                "The asset database relative path to output the file.");


            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
            /// </summary>
            public static readonly GUIContent CommandLineParserArgumentPrefix = new GUIContent(
                "Argument Prefix",
                "The prefix used to denote arguments in the command line.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
            /// </summary>
            public static readonly GUIContent CommandLineParserArgumentSplit = new GUIContent(
                "Argument Split",
                "The string used to split arguments from their values.");

            /// <summary>
            ///     A cached <see cref="GUIContent" /> containing a help symbol.
            /// </summary>
            public static readonly GUIContent HelpIcon;

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.localizationSetDefaultCulture" />.
            /// </summary>
            public static readonly GUIContent LocalizationSetDefaultCulture = new GUIContent(
                "Set Default Culture",
                "For situations where a culture does not have a calendar available, this can be corrected after assemblies have been loaded by setting the CultureInfo.DefaultThreadCurrentCulture to a setting with one.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.localizationDefaultCulture" />.
            /// </summary>
            public static readonly GUIContent LocalizationDefaultCulture = new GUIContent(
                "Default Culture",
                "The value to be used when setting the CultureInfo.DefaultThreadCurrentCulture.");

            /// <summary>
            ///     A cached <see cref="GUIContent" /> containing a minus symbol.
            /// </summary>
            public static readonly GUIContent MinusIcon;

            /// <summary>
            ///     A cached <see cref="GUIContent" /> containing a plus symbol.
            /// </summary>
            public static readonly GUIContent PlusIcon;

            /// <summary>
            ///     A cached <see cref="GUIContent" /> containing a cross symbol.
            /// </summary>
            public static readonly GUIContent TestNormalIcon;

            /// <summary>
            ///     A cached <see cref="GUIContent" /> containing a checkmark symbol.
            /// </summary>
            public static readonly GUIContent TestPassedIcon;

            /// <summary>
            ///     Initialize the <see cref="SettingsContent" />.
            /// </summary>
            static SettingsContent()
            {
                if (EditorGUIUtility.isProSkin)
                {
                    PlusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
                    MinusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
                    TestPassedIcon = EditorGUIUtility.IconContent("TestPassed");
                    TestNormalIcon = EditorGUIUtility.IconContent("TestNormal");
                    HelpIcon = EditorGUIUtility.IconContent("_Help");
                }
                else
                {
                    PlusIcon = EditorGUIUtility.IconContent("d_Toolbar Plus");
                    MinusIcon = EditorGUIUtility.IconContent("d_Toolbar Minus");
                    TestPassedIcon = EditorGUIUtility.IconContent("TestPassed");
                    TestNormalIcon = EditorGUIUtility.IconContent("TestNormal");
                    HelpIcon = EditorGUIUtility.IconContent("d__Help");
                }
            }
        }

        /// <summary>
        ///     A collection of IMGUI based layout methods used by the settings window.
        /// </summary>
        private static class SettingsLayout
        {
            /// <summary>
            ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
            /// </summary>
            private static readonly Dictionary<string, bool> s_cachedEditorPreferences = new Dictionary<string, bool>();

            /// <summary>
            ///     Draw a section header useful for project settings.
            /// </summary>
            /// <param name="id">Identifier used for editor preferences to determine if the section is collapsed or not.</param>
            /// <param name="defaultVisibility">Should this sections content be visible by default?</param>
            /// <param name="text">The section header content.</param>
            /// <param name="helpUri">The destination of the help button, if present.</param>
            /// <param name="sectionToggleProperty">
            ///     A <see cref="UnityEditor.SerializedProperty" /> which will dictate if a section is enabled or
            ///     not.
            /// </param>
            /// <param name="sectionToggleContent">The <see cref="UnityEngine.GUIContent" /> associated with a setting.</param>
            /// <returns>true/false if the sections content should be enabled.</returns>
            public static bool CreateSettingsSection(string id, bool defaultVisibility, string text,
                string helpUri = null,
                SerializedProperty sectionToggleProperty = null,
                GUIContent sectionToggleContent = null)
            {
                Color previousColor = GUI.backgroundColor;

                if (sectionToggleProperty == null)
                {
                    GUI.backgroundColor = SettingsStyles.DefaultBlueColor;
                    GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id, defaultVisibility);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }

                    GUILayout.Label(text, SettingsStyles.SectionHeaderTextDefaultStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(SettingsContent.HelpIcon, SettingsStyles.HelpButtonStyle))
                        {
                            GUIUtility.hotControl = 0;
                            Application.OpenURL(helpUri);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUI.backgroundColor = previousColor;
                    GUILayout.Space(5);
                    return true;
                }

                if (sectionToggleProperty.boolValue)
                {
                    GUI.backgroundColor = SettingsStyles.EnabledGreenColor;
                    GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }

                    GUILayout.Label(text, SettingsStyles.SectionHeaderTextDefaultStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(SettingsContent.HelpIcon, SettingsStyles.HelpButtonStyle))
                        {
                            GUIUtility.hotControl = 0;
                            Application.OpenURL(helpUri);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                        SettingsStyles.SectionHeaderToggleLayoutOptions);
                }
                else
                {
                    GUI.backgroundColor = SettingsStyles.DisabledYellowColor;
                    GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(SettingsContent.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                            SettingsStyles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }


                    GUILayout.Label(text, SettingsStyles.SectionHeaderTextDisabledStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(SettingsContent.HelpIcon, SettingsStyles.HelpButtonStyle))
                        {
                            GUIUtility.hotControl = 0;
                            Application.OpenURL(helpUri);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                        SettingsStyles.SectionHeaderToggleLayoutOptions);
                }

                GUILayout.EndHorizontal();
                GUI.backgroundColor = previousColor;
                GUILayout.Space(5);
                return sectionToggleProperty.boolValue;
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

            /// <summary>
            ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
            /// </summary>
            /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
            /// <param name="setValue">The desired value to set.</param>
            private static void SetCachedEditorBoolean(string id, bool setValue)
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

        /// <summary>
        ///     A collection of IMGUI based section builders for the settings window.
        /// </summary>
        private static class SettingsSections
        {
            /// <summary>
            ///     Draw the Automatic Updates section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void AutomaticUpdates(SerializedObject settings)
            {
                const string sectionID = "GDX.Editor.UpdateProvider";
                GUI.enabled = true;

                bool packageSectionEnabled = SettingsLayout.CreateSettingsSection(
                    sectionID, true,
                    "Automatic Package Updates", null,
                    settings.FindProperty("updateProviderCheckForUpdates"),
                    SettingsContent.AutomaticUpdatesEnabled);
                if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.BeginHorizontal(SettingsStyles.InfoBoxStyle);
                if (UpdateProvider.LocalPackage.Definition != null)
                {
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Local Version:", EditorStyles.boldLabel,
                        SettingsStyles.FixedWidth130LayoutOptions);
                    GUILayout.Label(UpdateProvider.LocalPackage.Definition.version);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Installation Method:", EditorStyles.boldLabel,
                        SettingsStyles.FixedWidth130LayoutOptions);
                    GUILayout.Label(PackageProvider.GetFriendlyName(UpdateProvider.LocalPackage.InstallationMethod));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    // Handle additional information

                    switch (UpdateProvider.LocalPackage.InstallationMethod)
                    {
                        case PackageProvider.InstallationType.UPMBranch:
                        case PackageProvider.InstallationType.GitHubBranch:
                        case PackageProvider.InstallationType.GitHub:
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Source Branch:", EditorStyles.boldLabel,
                                SettingsStyles.FixedWidth130LayoutOptions);
                            GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                                ? UpdateProvider.LocalPackage.SourceTag
                                : "N/A");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                        case PackageProvider.InstallationType.UPMTag:
                        case PackageProvider.InstallationType.GitHubTag:
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Source Tag:", EditorStyles.boldLabel,
                                SettingsStyles.FixedWidth130LayoutOptions);
                            GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                                ? UpdateProvider.LocalPackage.SourceTag
                                : "N/A");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                        case PackageProvider.InstallationType.UPMCommit:
                        case PackageProvider.InstallationType.GitHubCommit:
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Source Commit:", EditorStyles.boldLabel,
                                SettingsStyles.FixedWidth130LayoutOptions);
                            GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                                ? UpdateProvider.LocalPackage.SourceTag
                                : "N/A");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Last Checked:", EditorStyles.boldLabel, SettingsStyles.FixedWidth130LayoutOptions);
                    GUILayout.Label(UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    // Force things to the right
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    if (UpdateProvider.HasUpdate(UpdateProvider.UpdatePackageDefinition))
                    {
                        if (GUILayout.Button("Changelog", SettingsStyles.ButtonStyle))
                        {
                            Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
                        }

                        if (UpdateProvider.IsUpgradable())
                        {
                            if (GUILayout.Button("Update", SettingsStyles.ButtonStyle))
                            {
                                UpdateProvider.AttemptUpgrade();
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Manual Check", SettingsStyles.ButtonStyle))
                        {
                            UpdateProvider.CheckForUpdates();
                        }
                    }

                    EditorGUILayout.EndVertical();
                }
                else
                {
                    GUILayout.Label(
                        $"An error occured trying to find the package definition.\nPresumed Root: {UpdateProvider.LocalPackage.PackageAssetPath}\nPresumed Manifest:{UpdateProvider.LocalPackage.PackageManifestPath})",
                        EditorStyles.boldLabel);
                }

                EditorGUILayout.EndHorizontal();

                // Disable based on if we have this enabled
                GUI.enabled = packageSectionEnabled;

                UpdateDayCountSetting =
                    EditorGUILayout.IntSlider(SettingsContent.AutomaticUpdatesUpdateDayCount, UpdateDayCountSetting, 1,
                        31);
            }

            /// <summary>
            ///     Draw the Build Info section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void BuildInfo(SerializedObject settings)
            {
                const string sectionID = "GDX.Editor.Build.BuildInfoProvider";
                GUI.enabled = true;

                bool buildInfoEnabled = SettingsLayout.CreateSettingsSection(
                    sectionID, false,
                    "BuildInfo Generation", $"{DocumentationUri}api/GDX.Editor.Build.BuildInfoProvider.html",
                    settings.FindProperty("developerBuildInfoEnabled"),
                    SettingsContent.BuildInfoEnabled);

                if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                string buildInfoFile = Path.Combine(Application.dataPath,
                    settings.FindProperty("developerBuildInfoPath").stringValue);
                if (!File.Exists(buildInfoFile))
                {
                    GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(
                        "There is currently no BuildInfo file in the target location. Would you like some default content written in its place?",
                        SettingsStyles.WordWrappedLabelStyle);
                    if (GUILayout.Button("Create Default", SettingsStyles.ButtonStyle))
                    {
                        BuildInfoProvider.WriteDefaultFile();
                        AssetDatabase.ImportAsset("Assets/" +
                                                  settings.FindProperty("developerBuildInfoPath").stringValue);
                    }

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }

                // Only allow editing based on the feature being enabled
                GUI.enabled = buildInfoEnabled;

                EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoPath"),
                    SettingsContent.BuildInfoPath);
                EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoNamespace"),
                    SettingsContent.BuildInfoNamespace);
                EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoAssemblyDefinition"),
                    SettingsContent.BuildInfoAssemblyDefinition);


                GUILayout.Space(10);
                // Arguments (we're going to make sure they are forced to uppercase).
                GUILayout.Label("Build Arguments", SettingsStyles.SubSectionHeaderTextStyle);

                SerializedProperty buildNumberProperty =
                    settings.FindProperty("developerBuildInfoBuildNumberArgument");
                EditorGUILayout.PropertyField(buildNumberProperty, SettingsContent.BuildInfoBuildNumberArgument);
                if (buildNumberProperty.stringValue.HasLowerCase())
                {
                    buildNumberProperty.stringValue = buildNumberProperty.stringValue.ToUpper();
                }

                SerializedProperty buildDescriptionProperty =
                    settings.FindProperty("developerBuildInfoBuildDescriptionArgument");
                EditorGUILayout.PropertyField(buildDescriptionProperty,
                    SettingsContent.BuildInfoBuildDescriptionArgument);
                if (buildDescriptionProperty.stringValue.HasLowerCase())
                {
                    buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
                }

                SerializedProperty buildChangelistProperty =
                    settings.FindProperty("developerBuildInfoBuildChangelistArgument");
                EditorGUILayout.PropertyField(buildChangelistProperty,
                    SettingsContent.BuildInfoBuildChangelistArgument);
                if (buildChangelistProperty.stringValue.HasLowerCase())
                {
                    buildChangelistProperty.stringValue = buildChangelistProperty.stringValue.ToUpper();
                }

                SerializedProperty buildTaskProperty = settings.FindProperty("developerBuildInfoBuildTaskArgument");
                EditorGUILayout.PropertyField(buildTaskProperty, SettingsContent.BuildInfoBuildTaskArgument);
                if (buildTaskProperty.stringValue.HasLowerCase())
                {
                    buildTaskProperty.stringValue = buildTaskProperty.stringValue.ToUpper();
                }

                SerializedProperty buildStreamProperty =
                    settings.FindProperty("developerBuildInfoBuildStreamArgument");
                EditorGUILayout.PropertyField(buildStreamProperty, SettingsContent.BuildInfoBuildStreamArgument);
                if (buildStreamProperty.stringValue.HasLowerCase())
                {
                    buildStreamProperty.stringValue = buildStreamProperty.stringValue.ToUpper();
                }
            }

            /// <summary>
            ///     Draw the Command Line Processor section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void CommandLineProcessor(SerializedObject settings)
            {
                const string sectionID = "GDX.Developer.CommandLineParser";
                GUI.enabled = true;

                SettingsLayout.CreateSettingsSection(sectionID, false, "Command Line Parser",
                    $"{DocumentationUri}api/GDX.Developer.CommandLineParser.html");

                if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                    SettingsContent.CommandLineParserArgumentPrefix);
                EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                    SettingsContent.CommandLineParserArgumentSplit);
            }

            /// <summary>
            ///     Draw the Localization section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void Locale(SerializedObject settings)
            {
                const string sectionID = "GDX.Localization";
                GUI.enabled = true;

                SettingsLayout.CreateSettingsSection(sectionID, false, "Localization",
                    $"{DocumentationUri}api/GDX.Localization.html");

                if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.PropertyField(settings.FindProperty("localizationSetDefaultCulture"),
                    SettingsContent.LocalizationSetDefaultCulture);

                EditorGUILayout.PropertyField(settings.FindProperty("localizationDefaultCulture"),
                    SettingsContent.LocalizationDefaultCulture);
            }

            /// <summary>
            ///     Draw the packages status section of the settings window.
            /// </summary>
            public static void PackageStatus()
            {
                GUI.enabled = true;

                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                EditorGUILayout.BeginHorizontal();

                // Information
                GUILayout.BeginVertical();

                GUILayout.Label(SettingsContent.AboutBlurb, SettingsStyles.WordWrappedLabelStyle);
                GUILayout.Space(10);


                GUILayout.BeginHorizontal();
                GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                             if (EditorGUILayout.LinkButton("Repository"))
#else
                if (GUILayout.Button("Repository", EditorStyles.linkLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL("https://github.com/dotBunny/GDX/");
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                             if (EditorGUILayout.LinkButton("Documentation"))
#else
                if (GUILayout.Button("Documentation", EditorStyles.linkLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL(DocumentationUri);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                             if (EditorGUILayout.LinkButton("Report an Issue"))
#else
                if (GUILayout.Button("Report an Issue", EditorStyles.linkLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL("https://github.com/dotBunny/GDX/issues");
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();

                GUILayout.Space(15);
                GUILayout.FlexibleSpace();

                // CHeck Packages
                // ReSharper disable UnreachableCode
#pragma warning disable 162
                EditorGUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
                GUILayout.Label("Packages Found", SettingsStyles.SubSectionHeaderTextStyle);
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                // ReSharper disable once StringLiteralTypo
                GUILayout.Label("Addressables");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasAddressablesPackage
                    ? SettingsContent.TestPassedIcon
                    : SettingsContent.TestNormalIcon);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Burst");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasBurstPackage
                    ? SettingsContent.TestPassedIcon
                    : SettingsContent.TestNormalIcon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Jobs");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasJobsPackage
                    ? SettingsContent.TestPassedIcon
                    : SettingsContent.TestNormalIcon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Mathematics");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasMathematicsPackage
                    ? SettingsContent.TestPassedIcon
                    : SettingsContent.TestNormalIcon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Platforms");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasPlatformsPackage
                    ? SettingsContent.TestPassedIcon
                    : SettingsContent.TestNormalIcon);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                // ReSharper restore UnreachableCode
#pragma warning restore 162

                EditorGUILayout.EndHorizontal();

                // ReSharper enable ConditionIsAlwaysTrueOrFalse

                GUILayout.Space(5);
            }
        }

        /// <summary>
        ///     A collection of styles and layout options used by the settings window.
        /// </summary>
        private static class SettingsStyles
        {
            /// <summary>
            ///     A collection of layout parameters to use when rendering the expand button on section headers.
            /// </summary>
            public static readonly GUILayoutOption[] BulletLayoutOptions;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" />" /> representing a button.
            /// </summary>
            public static readonly GUIStyle ButtonStyle;

            /// <summary>
            ///     A shade of the <see cref="UnityEngine.Color" /> blue.
            /// </summary>
            /// <remarks>Meant for default things.</remarks>
            public static readonly Color DefaultBlueColor =
                new Color(0.0941176470588235f, 0.4549019607843137f, 0.8549019607843137f);

            /// <summary>
            ///     A shade of the <see cref="UnityEngine.Color" /> yellow.
            /// </summary>
            /// <remarks>Meant for disabled things.</remarks>
            public static readonly Color DisabledYellowColor =
                new Color(0.8941176470588235f, 0.9019607843137255f, 0.4117647058823529f);

            /// <summary>
            ///     A shade of the <see cref="UnityEngine.Color" /> green.
            /// </summary>
            /// <remarks>Meant for enabled things.</remarks>
            public static readonly Color EnabledGreenColor =
                new Color(0.1803921568627451f, 0.6431372549019608f, 0.3098039215686275f);

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing a help button.
            /// </summary>
            public static readonly GUIStyle HelpButtonStyle;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing an info box.
            /// </summary>
            public static readonly GUIStyle InfoBoxStyle;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing a default section header.
            /// </summary>
            public static readonly GUIStyle SectionHeaderStyle;

            /// <summary>
            ///     A specific max width (130) for layout options to allow for organized width layouts.
            /// </summary>
            public static readonly GUILayoutOption[] FixedWidth130LayoutOptions;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing a default section header text.
            /// </summary>
            public static readonly GUIStyle SectionHeaderTextDefaultStyle;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing a disabled section header text.
            /// </summary>
            public static readonly GUIStyle SectionHeaderTextDisabledStyle;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing the expand button for section headers.
            /// </summary>
            public static readonly GUIStyle SectionHeaderExpandButtonStyle;

            /// <summary>
            ///     A collection of layout parameters to use when rendering the expand button on section headers.
            /// </summary>
            public static readonly GUILayoutOption[] SectionHeaderExpandLayoutOptions;

            /// <summary>
            ///     A collection of layout parameters to use when rendering the toggle option on section headers.
            /// </summary>
            public static readonly GUILayoutOption[] SectionHeaderToggleLayoutOptions;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> representing the header of a sub section definition.
            /// </summary>
            public static readonly GUIStyle SubSectionHeaderTextStyle;

            /// <summary>
            ///     A generic label with wordwrap <see cref="GUIStyle" />.
            /// </summary>
            public static readonly GUIStyle WordWrappedLabelStyle;

            /// <summary>
            ///     A <see cref="UnityEngine.GUIStyle" /> used to wrap all GDX editor user interfaces.
            /// </summary>
            public static readonly GUIStyle WrapperStyle;

            /// <summary>
            ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 25% opacity.
            /// </summary>
            private static readonly Color s_whiteBlend25Color = new Color(1f, 1f, 1f, 0.25f);

            /// <summary>
            ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 75% opacity.
            /// </summary>
            private static readonly Color s_whiteBlend75Color = new Color(1f, 1f, 1f, 0.75f);

            /// <summary>
            ///     Initialize the <see cref="SettingsStyles" />, creating all of the associated <see cref="GUIStyle" />s.
            /// </summary>
            static SettingsStyles()
            {
                InfoBoxStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    padding = {top = 10, bottom = 10, left = 10, right = 10}, margin = {bottom = 10}
                };
                WrapperStyle = new GUIStyle {margin = {left = 5, right = 5, bottom = 5}};
                ButtonStyle =
                    new GUIStyle(EditorStyles.miniButton) {stretchWidth = false, padding = {left = 10, right = 10}};

                // Section Headers
                SectionHeaderStyle = new GUIStyle("box") {margin = {left = -20}};

                SectionHeaderTextDefaultStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    fontStyle = FontStyle.Bold, normal = {textColor = s_whiteBlend75Color}
                };
                SectionHeaderTextDisabledStyle =
                    new GUIStyle(SectionHeaderTextDefaultStyle) {normal = {textColor = s_whiteBlend25Color}};

                SectionHeaderToggleLayoutOptions =
                    new[] {GUILayout.Width(EditorStyles.toggle.CalcSize(GUIContent.none).x)};

                SectionHeaderExpandButtonStyle = new GUIStyle("button") {fontStyle = FontStyle.Bold};

                SectionHeaderExpandLayoutOptions = new[] {GUILayout.Width(25)};
                BulletLayoutOptions = new[] {GUILayout.Width(10)};

                SubSectionHeaderTextStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    fontStyle = FontStyle.Bold, fontSize = EditorStyles.largeLabel.fontSize - 1, margin = {left = 2}
                };

                WordWrappedLabelStyle = new GUIStyle("label") {wordWrap = true};
                HelpButtonStyle = new GUIStyle("IconButton") {margin = {top = 5}};

                FixedWidth130LayoutOptions =
                    new[] {GUILayout.MaxWidth(130), GUILayout.MinWidth(130), GUILayout.Width(130)};
            }
        }
    }
}