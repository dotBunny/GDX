// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    public static class UpdateProviderSettings
    {
        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private const string UpdateDayCountKey = "GDX.UpdateProvider.UpdateDayCount";

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        // ReSharper disable HeapView.ObjectAllocation.Evident
        private static readonly HashSet<string> s_settingsKeywords = new HashSet<string>(new[] {"gdx", "update"});
        // ReSharper restore HeapView.ObjectAllocation.Evident

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.updateProviderCheckForUpdates" />.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly GUIContent s_settingsCheckForUpdates = new GUIContent(
            "Check For Updates",
            "Should the package check the GitHub repository to see if there is a new version?");

        /// <summary>
        ///     Settings content for <see cref="UpdateDayCountSetting" />.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly GUIContent s_settingsUpdateDayCount = new GUIContent(
            "Update Timer (Days)",
            "After how many days should updates be checked for?");


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
        ///     Get <see cref="SettingsProvider" /> for GDX updates.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SettingsProvider("Project/GDX/Updates", SettingsScope.Project)
            {
                label = "Updates",
                guiHandler = searchContext =>
                {
                    GDXStyles.BeginGUILayout();

                    EditorGUILayout.BeginHorizontal(GDXStyles.Header);
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
                            if (GUILayout.Button("Changelog", GDXStyles.Button))
                            {
                                Application.OpenURL(UpdateProvider.GitHubChangelogUri);
                            }

                            if (GUILayout.Button("Update", GDXStyles.Button))
                            {
                                UpdateProvider.AttemptUpgrade();
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Manual Check", GDXStyles.Button))
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


                    SerializedObject settings = Config.GetSerializedConfig();
                    EditorGUILayout.PropertyField(settings.FindProperty("updateProviderCheckForUpdates"), s_settingsCheckForUpdates);
                    settings.ApplyModifiedPropertiesWithoutUndo();

                    UpdateDayCountSetting =
                        EditorGUILayout.IntSlider(s_settingsUpdateDayCount, UpdateDayCountSetting, 1, 31);

                    GDXStyles.EndGUILayout();
                },
                keywords = s_settingsKeywords
            };
        }
    }
}