using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        ///     A defined collection of ways that the package could be found in a Unity project.
        /// </summary>
        public enum InstallationType
        {
            /// <summary>
            ///     Unable to determine how the package was installed.
            /// </summary>
            Unidentified = 0,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process.
            /// </summary>
            UnityPackageManager = 1,

            /// <summary>
            ///     The package is included into the project via an asset store inclusion.
            /// </summary>
            UnityAssetStore = 2,

            /// <summary>
            ///     The package was installed through Unity's UPM using a reference to the OpenUPM repo.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            OpenUPM = 3,

            /// <summary>
            ///     The package is placed in the project's asset folder.
            /// </summary>
            LocalPackage = 4,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub.
            /// </summary>
            GitHubClone = 5
        }

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
        ///     The <see cref="InstallationType" /> detected during construction of the package.
        /// </summary>
        public static readonly InstallationType InstallationMethod = GetInstallationType();

        /// <summary>
        ///     The <see cref="PackageDefinition" /> for the installed package.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static readonly PackageDefinition Package = GetLocalPackageDefinition();

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
        ///     If an update check has occured, this will be filled with its <see cref="PackageDefinition" />.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public static PackageDefinition MainPackageDefinition;

        /// <summary>
        ///     Initialize the update provider, and check if necessary.
        /// </summary>
        static UpdateProvider()
        {
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
                        MainPackageDefinition != null
                            ? $"Local version: {GetLocalPackageDefinition().version}\nGitHub version: {MainPackageDefinition.version}\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}."
                            : $"Local version: {GetLocalPackageDefinition().version}\nGitHub version: Unknown\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}.",
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

            MainPackageDefinition = GetMainPackageDefinition();
            if (MainPackageDefinition == null)
            {
                return;
            }

            // Package versions
            SemanticVersion updatePackageVersion = new SemanticVersion(MainPackageDefinition.version);
            SemanticVersion localPackageVersion = new SemanticVersion(Package.version);

            // Unity versions
            SemanticVersion currentUnityVersion = new SemanticVersion(Application.unityVersion);
            SemanticVersion minimumUnityVersion = new SemanticVersion(MainPackageDefinition.unity);
            
            if (updatePackageVersion > localPackageVersion &&
                currentUnityVersion >= minimumUnityVersion)
            {
                // Store the notification value
                SetLastNotifiedVersion(MainPackageDefinition.version);
                // TODO: Notify!
                // TODO: Update?
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
        ///     Find and parse the local package.json into a <see cref="PackageDefinition" />.
        /// </summary>
        /// <returns>A <see cref="PackageDefinition" /> instance.</returns>
        private static PackageDefinition GetLocalPackageDefinition()
        {
            try
            {
                string[] packageGuids = AssetDatabase.FindAssets("package");
                int packageCount = packageGuids.Length;
                string packageIdentifierLine = $"\"name\": \"{Strings.PackageName}\",";
                for (int i = 0; i < packageCount; i++)
                {
                    string packagePath = Path.Combine(
                        Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                        AssetDatabase.GUIDToAssetPath(packageGuids[i]));
                    string[] packageContent = File.ReadAllLines(packagePath);

                    if (packageContent.Length > 15 &&
                        packageContent[1].Trim() == packageIdentifierLine &&
                        packageContent[14].Trim() == "\"name\": \"dotBunny\",")
                    {
                        return JsonUtility.FromJson<PackageDefinition>(File.ReadAllText(packagePath));
                    }
                }

                return null;
            }
            catch (Exception)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

        /// <summary>
        ///     Poll the main GitHub repository to find its package.json, parsing into a <see cref="PackageDefinition" />.
        /// </summary>
        /// <returns>A <see cref="PackageDefinition" /> instance.</returns>
        private static PackageDefinition GetMainPackageDefinition()
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
                    : JsonUtility.FromJson<PackageDefinition>(packageJsonContent);
            }
            catch (Exception)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

        /// <summary>
        ///     Determine the current <see cref="InstallationType" /> of the GDX package.
        /// </summary>
        /// <returns>The discovered <see cref="InstallationType" />.</returns>
        private static InstallationType GetInstallationType()
        {
            // Well we reached this point and don't actually know, so guess we should admit it.
            return InstallationType.Unidentified;
        }


        /// <summary>
        ///     A miniature package definition useful for quickly parsing a remote package definition.
        /// </summary>
        [Serializable]
        public class PackageDefinition
        {
            public string version;
            public string unity;
        }
    }
}