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

                    // Display top level information
                    PackageStatus();

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

            bool packageSectionEnabled = SettingsLayout.CreateSettingsSection(
                sectionID, true,
                "Automatic Package Updates",
                settings.FindProperty("updateProviderCheckForUpdates"),
                SettingsContent.AutomaticUpdatesEnabled);
            if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
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
                EditorGUILayout.IntSlider(SettingsContent.AutomaticUpdatesUpdateDayCount, UpdateDayCountSetting, 1, 31);

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

            bool buildInfoEnabled = SettingsLayout.CreateSettingsSection(
                sectionID, false,
                "BuildInfo Generation",
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
            EditorGUILayout.PropertyField(buildDescriptionProperty, SettingsContent.BuildInfoBuildDescriptionArgument);
            if (buildDescriptionProperty.stringValue.HasLowerCase())
            {
                buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
            }

            SerializedProperty buildChangelistProperty =
                settings.FindProperty("developerBuildInfoBuildChangelistArgument");
            EditorGUILayout.PropertyField(buildChangelistProperty, SettingsContent.BuildInfoBuildChangelistArgument);
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

            SettingsLayout.CreateSettingsSection(sectionID, false, "Command Line Parser");

            if (!SettingsLayout.GetCachedEditorBoolean(sectionID))
            {
                return;
            }

            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                SettingsContent.CommandLineParserArgumentPrefix);
            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                SettingsContent. CommandLineParserArgumentSplit);
        }

        private static void PackageStatus()
        {
            GUI.enabled = true;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (Developer.Conditionals.HasAddressablesPackage)
            {

            }

            if (Developer.Conditionals.HasBurstPackage)
            {

            }
            if (Developer.Conditionals.HasJobsPackage)
            {

            }
            if (Developer.Conditionals.HasMathematicsPackage)
            {

            }
            // ReSharper enable ConditionIsAlwaysTrueOrFalse
        }
    }
}