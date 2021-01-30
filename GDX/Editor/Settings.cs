// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
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
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private const string UpdateDayCountKey = "GDX.UpdateProvider.UpdateDayCount";

        private const int SectionPaddingBottom = 5;

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        public static readonly List<string> SearchKeywords = new List<string>(new[]
        {
            "gdx", "update", "parser", "commandline", "build"
        });

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.updateProviderCheckForUpdates" />.
        /// </summary>
        private static readonly GUIContent s_contentCheckForUpdates = new GUIContent(
            "",
            "Should the package check the GitHub repository to see if there is a new version?");

        /// <summary>
        ///     Settings content for <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private static readonly GUIContent s_contentUpdateDayCount = new GUIContent(
            "Update Timer (Days)",
            "After how many days should updates be checked for?");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoAssemblyDefinition" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoAssemblyDefinition = new GUIContent(
            "Assembly Definition",
            "Ensure that the folder of the BuildInfo has an assembly definition.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
        /// </summary>
        private static readonly GUIContent s_contentArgumentPrefix = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        private static readonly GUIContent s_contentArgumentSplit = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoEnabled" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoEnabled = new GUIContent(
            "",
            "During the build process should a BuildInfo be written?");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoPath" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoPath = new GUIContent(
            "Output Path",
            "The asset database relative path to output the file.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoNamespace" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoNamespace = new GUIContent(
            "Namespace",
            "The namespace where the BuildInfo should be placed.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildNumberArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildNumberArgument = new GUIContent(
            "Number",
            "The argument key for the build number to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildDescriptionArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildDescriptionArgument = new GUIContent(
            "Description",
            "The argument key for the build description to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildChangelistArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildChangelistArgument = new GUIContent(
            "Changelist",
            "The argument key for the build changelist to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildTaskArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildTaskArgument = new GUIContent(
            "Task",
            "The argument key for the build task to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildStreamArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildStreamArgument = new GUIContent(
            "Stream",
            "The argument key for the build stream to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     The number of days between checks for updates.
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        public static int UpdateDayCountSetting
        {
            get => EditorPrefs.GetInt(UpdateDayCountKey, 7);
            private set => EditorPrefs.SetInt(UpdateDayCountKey, value);
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

                    // Build out sections
                    AutomaticUpdatesSection(settings);
                    BuildInfoSection(settings);
                    CommandLineProcessorSection(settings);

                    // Apply any serialized setting changes
                    settings.ApplyModifiedPropertiesWithoutUndo();

                    // Stop wrapping the content
                    EditorGUILayout.EndVertical();
                },
                keywords = SearchKeywords
            };
        }

        /// <summary>
        ///     Build out Automatic Updates section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        private static void AutomaticUpdatesSection(SerializedObject settings)
        {
            const string sectionID = "GDX.Editor.UpdateProvider";
            GUI.enabled = true;

            bool packageSectionEnabled = SettingsStyles.BeginSettingsSection(
                sectionID, true,
                "Automatic Package Updates",
                settings.FindProperty("updateProviderCheckForUpdates"),
                s_contentCheckForUpdates);
            if (!SettingsStyles.GetCachedEditorBoolean(sectionID))
            {
                return;
            }

            EditorGUILayout.BeginHorizontal(SettingsStyles.InfoBoxStyle);
            if (UpdateProvider.LocalPackage.Definition != null)
            {
                GUILayout.Label(
                    UpdateProvider.UpdatePackageDefinition != null
                        ? $"Local version: {UpdateProvider.LocalPackage.Definition.version} ({UpdateProvider.LocalPackage.InstallationMethod.ToString()})\nGitHub version: {UpdateProvider.UpdatePackageDefinition.version}\nLast checked on {UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat)}."
                        : $"Local version: {UpdateProvider.LocalPackage.Definition.version} ({UpdateProvider.LocalPackage.InstallationMethod.ToString()})\nGitHub version: Unknown\nLast checked on {UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat)}.",
                    EditorStyles.boldLabel);

                // Force things to the right
                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();
                if (UpdateProvider.HasUpdate(UpdateProvider.UpdatePackageDefinition))
                {
                    if (GUILayout.Button("Changelog", SettingsStyles.ButtonStyle))
                    {
                        Application.OpenURL(UpdateProvider.GitHubChangelogUri);
                    }

                    if (GUILayout.Button("Update", SettingsStyles.ButtonStyle))
                    {
                        UpdateProvider.AttemptUpgrade();
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
                EditorGUILayout.IntSlider(s_contentUpdateDayCount, UpdateDayCountSetting, 1, 31);

            GUILayout.Space(SectionPaddingBottom);
        }

        /// <summary>
        ///     Build out Build Info section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        private static void BuildInfoSection(SerializedObject settings)
        {
            const string sectionID = "GDX.Editor.Build.BuildInfoProvider";
            GUI.enabled = true;

            bool buildInfoEnabled = SettingsStyles.BeginSettingsSection(
                sectionID, false,
                "BuildInfo Generation",
                settings.FindProperty("developerBuildInfoEnabled"),
                s_contentBuildInfoEnabled);

            if (!SettingsStyles.GetCachedEditorBoolean(sectionID))
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
                    "There is currently no BuildInfo file in the target location\nWould you like some default content written in its place?");
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
                s_contentBuildInfoPath);
            EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoNamespace"),
                s_contentBuildInfoNamespace);
            EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoAssemblyDefinition"),
                s_contentBuildInfoAssemblyDefinition);


            GUILayout.Space(10);
            // Arguments (we're going to make sure they are forced to uppercase).
            GUILayout.Label("Build Arguments", SettingsStyles.SubSectionHeaderTextStyle);

            SerializedProperty buildNumberProperty =
                settings.FindProperty("developerBuildInfoBuildNumberArgument");
            EditorGUILayout.PropertyField(buildNumberProperty, s_contentBuildInfoBuildNumberArgument);
            if (buildNumberProperty.stringValue.HasLowerCase())
            {
                buildNumberProperty.stringValue = buildNumberProperty.stringValue.ToUpper();
            }

            SerializedProperty buildDescriptionProperty =
                settings.FindProperty("developerBuildInfoBuildDescriptionArgument");
            EditorGUILayout.PropertyField(buildDescriptionProperty, s_contentBuildInfoBuildDescriptionArgument);
            if (buildDescriptionProperty.stringValue.HasLowerCase())
            {
                buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
            }

            SerializedProperty buildChangelistProperty =
                settings.FindProperty("developerBuildInfoBuildChangelistArgument");
            EditorGUILayout.PropertyField(buildChangelistProperty, s_contentBuildInfoBuildChangelistArgument);
            if (buildChangelistProperty.stringValue.HasLowerCase())
            {
                buildChangelistProperty.stringValue = buildChangelistProperty.stringValue.ToUpper();
            }

            SerializedProperty buildTaskProperty = settings.FindProperty("developerBuildInfoBuildTaskArgument");
            EditorGUILayout.PropertyField(buildTaskProperty, s_contentBuildInfoBuildTaskArgument);
            if (buildTaskProperty.stringValue.HasLowerCase())
            {
                buildTaskProperty.stringValue = buildTaskProperty.stringValue.ToUpper();
            }

            SerializedProperty buildStreamProperty =
                settings.FindProperty("developerBuildInfoBuildStreamArgument");
            EditorGUILayout.PropertyField(buildStreamProperty, s_contentBuildInfoBuildStreamArgument);
            if (buildStreamProperty.stringValue.HasLowerCase())
            {
                buildStreamProperty.stringValue = buildStreamProperty.stringValue.ToUpper();
            }

            GUILayout.Space(SectionPaddingBottom);
        }

        /// <summary>
        ///     Build out Command Line Processor section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        private static void CommandLineProcessorSection(SerializedObject settings)
        {
            const string sectionID = "GDX.Developer.CommandLineParser";
            GUI.enabled = true;

            SettingsStyles.BeginSettingsSection(sectionID, false, "Command Line Parser");

            if (!SettingsStyles.GetCachedEditorBoolean(sectionID))
            {
                return;
            }

            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                s_contentArgumentPrefix);
            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                s_contentArgumentSplit);
        }
    }
}