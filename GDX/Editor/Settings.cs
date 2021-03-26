// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using GDX.Developer;
using GDX.Editor.Build;
using GDX.Mathematics.Random;
#if GDX_VISUALSCRIPTING
using Unity.VisualScripting;
#endif
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
                    EditorGUILayout.BeginVertical(Styles.WrapperStyle);

                    Sections.PackageStatus();

                    // Build out sections
                    Sections.AutomaticUpdates(settings);
                    GUILayout.Space(5);
                    Sections.BuildInfo(settings);
                    GUILayout.Space(5);
                    Sections.CommandLineProcessor(settings);
                    GUILayout.Space(5);
                    Sections.Environment(settings);
                    GUILayout.Space(5);
                    Sections.Locale(settings);
#if GDX_VISUALSCRIPTING
                    GUILayout.Space(5);
                    Sections.VisualScripting();
#endif


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
        private static class Content
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
            ///     Settings content for <see cref="GDXConfig.environmentScriptingDefineSymbol" />.
            /// </summary>
            public static readonly GUIContent EnvironmentScriptingDefineSymbol = new GUIContent(
                "Ensure GDX Symbol",
                "Should GDX make sure that there is a GDX scripting define symbol across all viable build target groups.");

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
            ///     Settings content for <see cref="GDXConfig.traceDevelopmentLevels" />.
            /// </summary>
            public static readonly GUIContent TraceDevelopmentLevels = new GUIContent(
                "Development Tracing",
                "The levels of trace call to be logged in a development/editor build.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.traceDebugLevels" />.
            /// </summary>
            public static readonly GUIContent TraceDebugLevels = new GUIContent(
                "Debug Tracing",
                "The levels of trace call to be logged in a debug build.");

            /// <summary>
            ///     Settings content for <see cref="GDXConfig.traceReleaseLevels" />.
            /// </summary>
            public static readonly GUIContent TraceReleaseLevels = new GUIContent(
                "Release Tracing",
                "The levels of trace call to be logged in a release build.");

            /// <summary>
            ///     Initialize the <see cref="Content" />.
            /// </summary>
            static Content()
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
        private static class Layout
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
                    GUI.backgroundColor = Styles.DefaultBlueColor;
                    GUILayout.BeginHorizontal(Styles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id, defaultVisibility);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.PlusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.MinusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }

                    GUILayout.Label(text, Styles.SectionHeaderTextDefaultStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(Content.HelpIcon, Styles.HelpButtonStyle))
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
                    GUI.backgroundColor = Styles.EnabledGreenColor;
                    GUILayout.BeginHorizontal(Styles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.PlusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.MinusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }

                    GUILayout.Label(text, Styles.SectionHeaderTextDefaultStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(Content.HelpIcon, Styles.HelpButtonStyle))
                        {
                            GUIUtility.hotControl = 0;
                            Application.OpenURL(helpUri);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                        Styles.SectionHeaderToggleLayoutOptions);
                }
                else
                {
                    GUI.backgroundColor = Styles.DisabledYellowColor;
                    GUILayout.BeginHorizontal(Styles.SectionHeaderStyle);
                    bool setting = GetCachedEditorBoolean(id);
                    if (!setting)
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.PlusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, true);
                        }
                    }
                    else
                    {
                        // ReSharper disable once InvertIf
                        if (GUILayout.Button(Content.MinusIcon, Styles.SectionHeaderExpandButtonStyle,
                            Styles.SectionHeaderExpandLayoutOptions))
                        {
                            GUIUtility.hotControl = 0;
                            SetCachedEditorBoolean(id, false);
                        }
                    }


                    GUILayout.Label(text, Styles.SectionHeaderTextDisabledStyle);
                    if (!string.IsNullOrEmpty(helpUri))
                    {
                        if (GUILayout.Button(Content.HelpIcon, Styles.HelpButtonStyle))
                        {
                            GUIUtility.hotControl = 0;
                            Application.OpenURL(helpUri);
                        }
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                        Styles.SectionHeaderToggleLayoutOptions);
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
        private static class Sections
        {
            /// <summary>
            ///     Draw the Automatic Updates section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void AutomaticUpdates(SerializedObject settings)
            {
                const string sectionID = "GDX.Editor.UpdateProvider";
                GUI.enabled = true;

                bool packageSectionEnabled = Layout.CreateSettingsSection(
                    sectionID, true,
                    "Automatic Package Updates", null,
                    settings.FindProperty("updateProviderCheckForUpdates"),
                    Content.AutomaticUpdatesEnabled);
                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.BeginHorizontal(Styles.InfoBoxStyle);
                if (UpdateProvider.LocalPackage.Definition != null)
                {
                    GUILayout.BeginVertical();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Local Version:", EditorStyles.boldLabel,
                        Styles.FixedWidth130LayoutOptions);
                    GUILayout.Label(UpdateProvider.LocalPackage.Definition.version);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Installation Method:", EditorStyles.boldLabel,
                        Styles.FixedWidth130LayoutOptions);
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
                                Styles.FixedWidth130LayoutOptions);
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
                                Styles.FixedWidth130LayoutOptions);
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
                                Styles.FixedWidth130LayoutOptions);
                            GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                                ? UpdateProvider.LocalPackage.SourceTag
                                : "N/A");
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                    }

                    // Show remote version if we have something to show
                    if (UpdateProvider.UpdatePackageDefinition != null)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Remote Version:", EditorStyles.boldLabel, Styles.FixedWidth130LayoutOptions);
                        GUILayout.Label(UpdateProvider.UpdatePackageDefinition.version);
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Last Checked:", EditorStyles.boldLabel, Styles.FixedWidth130LayoutOptions);
                    GUILayout.Label(UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat));
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();

                    // Force things to the right
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    if (UpdateProvider.HasUpdate(UpdateProvider.UpdatePackageDefinition))
                    {
                        if (GUILayout.Button("Changelog", Styles.ButtonStyle))
                        {
                            Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
                        }

                        if (UpdateProvider.IsUpgradable())
                        {
                            if (GUILayout.Button("Update", Styles.ButtonStyle))
                            {
                                UpdateProvider.AttemptUpgrade();
                            }
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Manual Check", Styles.ButtonStyle))
                        {
                            UpdateProvider.CheckForUpdates();
                        }

                        // Special allowance to force pull dev branch to avoid having to increment the version code.
                        if ((UpdateProvider.LocalPackage.InstallationMethod ==
                             PackageProvider.InstallationType.GitHubBranch ||
                             UpdateProvider.LocalPackage.InstallationMethod ==
                             PackageProvider.InstallationType.UPMBranch) &&
                            UpdateProvider.LocalPackage.SourceTag == "dev")
                        {
                            if (GUILayout.Button("Force Upgrade", Styles.ButtonStyle))
                            {
                                UpdateProvider.AttemptUpgrade(true);
                            }
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
                    EditorGUILayout.IntSlider(Content.AutomaticUpdatesUpdateDayCount, UpdateDayCountSetting, 1,
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

                bool buildInfoEnabled = Layout.CreateSettingsSection(
                    sectionID, false,
                    "BuildInfo Generation", $"{DocumentationUri}api/GDX.Editor.Build.BuildInfoProvider.html",
                    settings.FindProperty("developerBuildInfoEnabled"),
                    Content.BuildInfoEnabled);

                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                string buildInfoFile = Path.Combine(Application.dataPath,
                    settings.FindProperty("developerBuildInfoPath").stringValue);
                if (!File.Exists(buildInfoFile))
                {
                    GUILayout.BeginVertical(Styles.InfoBoxStyle);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(
                        "There is currently no BuildInfo file in the target location. Would you like some default content written in its place?",
                        Styles.WordWrappedLabelStyle);
                    if (GUILayout.Button("Create Default", Styles.ButtonStyle))
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
                    Content.BuildInfoPath);
                EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoNamespace"),
                    Content.BuildInfoNamespace);
                EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoAssemblyDefinition"),
                    Content.BuildInfoAssemblyDefinition);


                GUILayout.Space(10);
                // Arguments (we're going to make sure they are forced to uppercase).
                GUILayout.Label("Build Arguments", Styles.SubSectionHeaderTextStyle);

                SerializedProperty buildNumberProperty =
                    settings.FindProperty("developerBuildInfoBuildNumberArgument");
                EditorGUILayout.PropertyField(buildNumberProperty, Content.BuildInfoBuildNumberArgument);
                if (buildNumberProperty.stringValue.HasLowerCase())
                {
                    buildNumberProperty.stringValue = buildNumberProperty.stringValue.ToUpper();
                }

                SerializedProperty buildDescriptionProperty =
                    settings.FindProperty("developerBuildInfoBuildDescriptionArgument");
                EditorGUILayout.PropertyField(buildDescriptionProperty,
                    Content.BuildInfoBuildDescriptionArgument);
                if (buildDescriptionProperty.stringValue.HasLowerCase())
                {
                    buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
                }

                SerializedProperty buildChangelistProperty =
                    settings.FindProperty("developerBuildInfoBuildChangelistArgument");
                EditorGUILayout.PropertyField(buildChangelistProperty,
                    Content.BuildInfoBuildChangelistArgument);
                if (buildChangelistProperty.stringValue.HasLowerCase())
                {
                    buildChangelistProperty.stringValue = buildChangelistProperty.stringValue.ToUpper();
                }

                SerializedProperty buildTaskProperty = settings.FindProperty("developerBuildInfoBuildTaskArgument");
                EditorGUILayout.PropertyField(buildTaskProperty, Content.BuildInfoBuildTaskArgument);
                if (buildTaskProperty.stringValue.HasLowerCase())
                {
                    buildTaskProperty.stringValue = buildTaskProperty.stringValue.ToUpper();
                }

                SerializedProperty buildStreamProperty =
                    settings.FindProperty("developerBuildInfoBuildStreamArgument");
                EditorGUILayout.PropertyField(buildStreamProperty, Content.BuildInfoBuildStreamArgument);
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

                Layout.CreateSettingsSection(sectionID, false, "Command Line Parser",
                    $"{DocumentationUri}api/GDX.Developer.CommandLineParser.html");

                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                    Content.CommandLineParserArgumentPrefix);
                EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                    Content.CommandLineParserArgumentSplit);
            }

            public static void Environment(SerializedObject settings)
            {
                const string sectionID = "GDX.Environment";
                GUI.enabled = true;

                Layout.CreateSettingsSection(sectionID, false, "Environment");

                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.PropertyField(settings.FindProperty("environmentScriptingDefineSymbol"),
                    Content.EnvironmentScriptingDefineSymbol);


                SerializedProperty developmentLevelsProperty = settings.FindProperty("traceDevelopmentLevels");
                EditorGUI.BeginChangeCheck();
                ushort newDevelopmentLevels = (ushort)EditorGUILayout.MaskField(Content.TraceDevelopmentLevels, developmentLevelsProperty.intValue,
                    developmentLevelsProperty.enumDisplayNames);
                if (EditorGUI.EndChangeCheck())
                {
                    developmentLevelsProperty.intValue = newDevelopmentLevels;
                }

                SerializedProperty debugLevelsProperty = settings.FindProperty("traceDebugLevels");
                EditorGUI.BeginChangeCheck();
                ushort newDebugLevels = (ushort)EditorGUILayout.MaskField(Content.TraceDebugLevels, debugLevelsProperty.intValue,
                    debugLevelsProperty.enumDisplayNames);
                if (EditorGUI.EndChangeCheck())
                {
                    debugLevelsProperty.intValue = newDebugLevels;
                }

                SerializedProperty releaseLevelsProperty = settings.FindProperty("traceReleaseLevels");
                EditorGUI.BeginChangeCheck();
                ushort newReleaseLevels = (ushort)EditorGUILayout.MaskField(Content.TraceReleaseLevels, releaseLevelsProperty.intValue,
                    releaseLevelsProperty.enumDisplayNames);
                if (EditorGUI.EndChangeCheck())
                {
                    releaseLevelsProperty.intValue = newReleaseLevels;
                }
            }

            /// <summary>
            ///     Draw the Localization section of settings.
            /// </summary>
            /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
            public static void Locale(SerializedObject settings)
            {
                const string sectionID = "GDX.Localization";
                GUI.enabled = true;

                Layout.CreateSettingsSection(sectionID, false, "Localization",
                    $"{DocumentationUri}api/GDX.Localization.html");

                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                EditorGUILayout.PropertyField(settings.FindProperty("localizationSetDefaultCulture"),
                    Content.LocalizationSetDefaultCulture);

                EditorGUILayout.PropertyField(settings.FindProperty("localizationDefaultCulture"),
                    Content.LocalizationDefaultCulture);
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

                GUILayout.Label(Content.AboutBlurb, Styles.WordWrappedLabelStyle);
                GUILayout.Space(10);


                GUILayout.BeginHorizontal();
                GUILayout.Label("-", Styles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                if (EditorGUILayout.LinkButton("Repository"))
#elif UNITY_2019_1_OR_NEWER
                if (GUILayout.Button("Repository", EditorStyles.linkLabel))
#else
                if (GUILayout.Button("Repository", EditorStyles.boldLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL("https://github.com/dotBunny/GDX/");
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("-", Styles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                if (EditorGUILayout.LinkButton("Documentation"))
#elif UNITY_2019_1_OR_NEWER
                if (GUILayout.Button("Documentation", EditorStyles.linkLabel))
#else
                if (GUILayout.Button("Documentation", EditorStyles.boldLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL(DocumentationUri);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("-", Styles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
                if (EditorGUILayout.LinkButton("Report an Issue"))
#elif UNITY_2019_1_OR_NEWER
                if (GUILayout.Button("Report an Issue", EditorStyles.linkLabel))
#else
                if (GUILayout.Button("Report an Issue", EditorStyles.boldLabel))
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
                EditorGUILayout.BeginVertical(Styles.InfoBoxStyle);
                GUILayout.Label("Packages Found", Styles.SubSectionHeaderTextStyle);
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                // ReSharper disable once StringLiteralTypo
                GUILayout.Label("Addressables");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasAddressablesPackage
                    ? Content.TestPassedIcon
                    : Content.TestNormalIcon);

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Burst");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasBurstPackage
                    ? Content.TestPassedIcon
                    : Content.TestNormalIcon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Mathematics");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasMathematicsPackage
                    ? Content.TestPassedIcon
                    : Content.TestNormalIcon);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Platforms");
                GUILayout.FlexibleSpace();
                GUILayout.Label(Conditionals.HasPlatformsPackage
                    ? Content.TestPassedIcon
                    : Content.TestNormalIcon);
                GUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                // ReSharper restore UnreachableCode
#pragma warning restore 162

                EditorGUILayout.EndHorizontal();

                // ReSharper enable ConditionIsAlwaysTrueOrFalse

                GUILayout.Space(5);
            }

#if GDX_VISUALSCRIPTING
            public static void VisualScripting()
            {
                const string sectionID = "GDX.VisualScripting";
                GUI.enabled = true;

                Layout.CreateSettingsSection(sectionID, false, "Visual Scripting",
                    $"{DocumentationUri}manual/features/visual-scripting.html");

                if (!Layout.GetCachedEditorBoolean(sectionID))
                {
                    return;
                }

                if (GUILayout.Button("ADD TO ASSEMBLIES"))
                {
                    if (!BoltCore.Configuration.assemblyOptions.Contains("GDX"))
                    {
                        BoltCore.Configuration.assemblyOptions.Add(new LooseAssemblyName("GDX"));
                        BoltCore.Configuration.Save();
                        Codebase.UpdateSettings();
                    }
                }

                if (GUILayout.Button("ADD TYPE"))
                {
                    Assembly gdxAssembly = Assembly.GetAssembly(typeof(GDXConfig));

                    List<Type> extensionTypes = new List<Type>();
                    List<Type> typeTypes = new List<Type>();
                    foreach(Type type in gdxAssembly.GetTypes())
                    {
                        var attributes = type.GetCustomAttributes(typeof(VisualScriptingAttribute), true);
                        foreach (object attribute in attributes)
                        {
                            VisualScriptingAttribute a = (VisualScriptingAttribute)attribute;
                            if (a != null)
                            {
                                switch (a._category)
                                {
                                    case VisualScriptingAttribute.Category.Extensions:
                                        extensionTypes.Add(type);
                                        break;
                                    case VisualScriptingAttribute.Category.Types:
                                        typeTypes.Add(type);
                                        break;
                                }
                            }
                        }

                    }


                    bool changed = false;
                    foreach (Type type in extensionTypes)
                    {
                        if (!BoltCore.Configuration.typeOptions.Contains(type))
                        {
                            BoltCore.Configuration.typeOptions.Add(type);
                            changed = true;
                        }
                    }
                    foreach (Type type in typeTypes)
                    {
                        if (!BoltCore.Configuration.typeOptions.Contains(type))
                        {
                            BoltCore.Configuration.typeOptions.Add(type);
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        BoltCore.Configuration.Save();
                        UnitBase.Rebuild();
                    }
                }
            }
#endif
        }

        /// <summary>
        ///     A collection of styles and layout options used by the settings window.
        /// </summary>
        private static class Styles
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
            ///     Initialize the <see cref="Styles" />, creating all of the associated <see cref="GUIStyle" />s.
            /// </summary>
            static Styles()
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