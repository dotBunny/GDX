using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     An autonomous provider which detects and notifies if updates are available for the GDX package.
    /// </summary>
    [InitializeOnLoad]
    public static class UpdateProvider
    {
        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last time we checked for an update.
        /// </summary>
        private const string LastCheckedKey = "GDX.UpdateProvider.LastChecked";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private const string UpdateDayCountKey = "GDX.UpdateProvider.UpdateDayCount";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last update version we notified the user about.
        /// </summary>
        private const string LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        /// <summary>
        ///     A collection of information about the locally installed GDX package.
        /// </summary>
        private static readonly PackageProvider s_localPackage;

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        // ReSharper disable HeapView.ObjectAllocation.Evident
        private static readonly HashSet<string> s_settingsKeywords = new HashSet<string>(new[] {"gdx", "update"});
        // ReSharper restore HeapView.ObjectAllocation.Evident

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.checkForUpdates" />.
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
        ///     If an update check has occured, this will be filled with its <see cref="PackageProvider.PackageDefinition" />.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static PackageProvider.PackageDefinition UpdatePackageDefinition;

        /// <summary>
        ///     Initialize the update provider, and check if necessary.
        /// </summary>
        static UpdateProvider()
        {
            // Create a copy of the local p
            s_localPackage = new PackageProvider();

            GDXConfig config = Config.Get();
            if (!config.checkForUpdates)
            {
                return;
            }

            // Should we check for updates?
            DateTime targetDate = GetLastChecked().AddDays(UpdateDayCountSetting);
            if (DateTime.Now >= targetDate)
            {
                CheckForUpdates();
            }
        }

        /// <summary>
        ///     The number of days between checks for updates.
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        private static int UpdateDayCountSetting
        {
            get => EditorPrefs.GetInt(UpdateDayCountKey, 7);
            set => EditorPrefs.SetInt(UpdateDayCountKey, value);
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

                    EditorGUILayout.BeginHorizontal(GDXStyles.HeaderStyle);
                    GUILayout.Label(
                        UpdatePackageDefinition != null
                            ? $"Local version: {s_localPackage.Definition.version}\nGitHub version: {UpdatePackageDefinition.version}\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}."
                            : $"Local version: {s_localPackage.Definition.version}\nGitHub version: Unknown\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}.",
                        EditorStyles.boldLabel);

                    if (GUILayout.Button("Manual Check", EditorStyles.miniButton))
                    {
                        CheckForUpdates();
                    }

                    EditorGUILayout.EndHorizontal();


                    SerializedObject settings = Config.GetSerializedConfig();
                    EditorGUILayout.PropertyField(settings.FindProperty("checkForUpdates"), s_settingsCheckForUpdates);
                    settings.ApplyModifiedPropertiesWithoutUndo();

                    UpdateDayCountSetting =
                        EditorGUILayout.IntSlider(s_settingsUpdateDayCount, UpdateDayCountSetting, 1, 31);

                    GDXStyles.EndGUILayout();
                },
                keywords = s_settingsKeywords
            };
        }

        /// <summary>
        ///     Check for updates!
        /// </summary>
        private static void CheckForUpdates()
        {
            SetLastChecked();

            UpdatePackageDefinition = GetMainPackageDefinition();
            if (UpdatePackageDefinition == null)
            {
                return;
            }

            // Package versions
            SemanticVersion updatePackageVersion = new SemanticVersion(UpdatePackageDefinition.version);
            SemanticVersion localPackageVersion = new SemanticVersion(s_localPackage.Definition.version);

            // Unity versions
            SemanticVersion currentUnityVersion = new SemanticVersion(Application.unityVersion);
            SemanticVersion minimumUnityVersion = new SemanticVersion(UpdatePackageDefinition.unity);

            string notifiedVersion = GetLastNotifiedVersion();

            if (updatePackageVersion > localPackageVersion &&
                currentUnityVersion >= minimumUnityVersion &&
                notifiedVersion != UpdatePackageDefinition.version)
            {
                string updateMessage =
                    $"Good news!\n\nThere is a new version of GDX available ({UpdatePackageDefinition.version}). Would you like to have the package attempt to upgrade itself to the newest version automatically?";
                int updateOption = EditorUtility.DisplayDialogComplex(
                    "GDX Update Available", updateMessage, "Update", "Skip", "Changelog");

                switch (updateOption)
                {
                    case 0:
                        switch (s_localPackage.InstallationMethod)
                        {
                            case PackageProvider.InstallationType.Unidentified:
                                // Flat out replace? Or just tell
                                break;
                            case PackageProvider.InstallationType.UnityPackageManager:
                                // How to get UPM to update?
                                break;
                            case PackageProvider.InstallationType.GitHubClone:
                                // GitHub pull?
                                break;
                            case PackageProvider.InstallationType.UnderAssets:
                                // Zip extract?
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                        break;

                    case 2:
                        // Open Changelog
                        Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
                        break;

                    default:
                        // Skip Version
                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                        break;
                }
            }
        }

        /// <summary>
        ///     Gets the last time that we checked for an update to the package.
        /// </summary>
        private static DateTime GetLastChecked()
        {
            DateTime lastTime = new DateTime(2020, 12, 14);
            if (EditorPrefs.HasKey(LastCheckedKey))
            {
                DateTime.TryParse(EditorPrefs.GetString(LastCheckedKey), out lastTime);
            }

            return lastTime;
        }

        /// <summary>
        ///     Set the last time that the package was checked for updates to right now.
        /// </summary>
        private static void SetLastChecked()
        {
            EditorPrefs.SetString(LastCheckedKey, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Get the last version of the package which was presented to the user for update.
        /// </summary>
        /// <returns>A version string.</returns>
        public static string GetLastNotifiedVersion()
        {
            return EditorPrefs.GetString(LastNotifiedVersionKey);
        }

        /// <summary>
        ///     Set the version of the package presented to update too.
        /// </summary>
        /// <param name="versionTag">The package version string.</param>
        private static void SetLastNotifiedVersion(string versionTag)
        {
            EditorPrefs.SetString(LastNotifiedVersionKey, versionTag);
        }

        /// <summary>
        ///     Poll the main GitHub repository to find its package.json, parsing into a
        ///     <see cref="PackageProvider.PackageDefinition" />.
        /// </summary>
        /// <returns>A <see cref="PackageProvider.PackageDefinition" /> instance.</returns>
        private static PackageProvider.PackageDefinition GetMainPackageDefinition()
        {
            try
            {
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                using WebClient webClient = new WebClient();

                // Get content of the package definition file
                string packageJsonContent =
                    webClient.DownloadString("https://raw.githubusercontent.com/dotBunny/GDX/main/package.json");

                // Return back the parsed object or null if there was no content.
                return string.IsNullOrEmpty(packageJsonContent)
                    ? null
                    : JsonUtility.FromJson<PackageProvider.PackageDefinition>(packageJsonContent);
            }
            catch (Exception)
            {
                // Don't go any further if there is an error
                return null;
            }
        }
    }
}