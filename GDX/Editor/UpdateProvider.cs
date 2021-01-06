using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using GDX.Collections;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    /// </summary>
    [InitializeOnLoad]
    public class UpdateProvider
    {
        /// <summary>
        ///     A defined collection of ways that the package could be found in a Unity project.
        /// </summary>
        public enum InstallationMethod
        {
            Unidentified = 0,
            UnityPackageManager = 1,
            UnityAssetStore = 2,

            // ReSharper disable once InconsistentNaming
            OpenUPM = 3,
            LocalPackage = 4,
            GitHubClone = 5
        }

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="CheckForUpdatesSetting" />.
        /// </summary>
        private const string CheckForUpdatesKey = "GDX.UpdateProvider.CheckForUpdates";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="LastChecked" />.
        /// </summary>
        private const string LastCheckedKey = "GDX.UpdateProvider.LastChecked";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private const string UpdateDayCountKey = "GDX.UpdateProvider.UpdateDayCount";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="Last" />.
        /// </summary>
        private const string LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        // ReSharper disable once MemberCanBePrivate.Global
        public static InstallationMethod Installation = InstallationMethod.Unidentified;

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly HashSet<string> s_settingsKeywords = new HashSet<string>(new[] {"gdx", "update"});

        /// <summary>
        ///     Settings content for <see cref="CheckForUpdatesSetting" />.
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
        ///     Should the package check the GitHub repository to see if there is a new version?
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        private static bool CheckForUpdatesSetting
        {
            get => EditorPrefs.GetBool(CheckForUpdatesKey, true);
            set => EditorPrefs.SetBool(CheckForUpdatesKey, value);
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

                    EditorGUILayout.BeginVertical(GDXStyles.HeaderStyle);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(
                        $"Local version: {GetLocalPackageDefinition().version}\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}.", EditorStyles.boldLabel);
                    if (GUILayout.Button("Manual Check", EditorStyles.miniButton))
                    {
                        CheckForUpdates();
                    }
                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.EndVertical();


                    SerializedObject settings = Config.GetSerializedConfig();
                    EditorGUILayout.PropertyField(settings.FindProperty("checkForUpdates"), CheckForUpdatesSetting);
                    settings.ApplyModifiedPropertiesWithoutUndo();

                    UpdateDayCountSetting = EditorGUILayout.IntSlider(s_settingsUpdateDayCount, UpdateDayCountSetting, 1, 31);

                    GDXStyles.EndGUILayout();

                },
                keywords = s_settingsKeywords
            };
        }

        public static readonly PackageDefinition Package = GetLocalPackageDefinition();

        private static void CheckForUpdates()
        {
            SetLastChecked();

            PackageDefinition remoteDefinition = GetMainPackageDefinition();
            if (remoteDefinition == null)
            {
                return;
            }

            SemanticVersion remoteVersion = new SemanticVersion(remoteDefinition.version);
            SemanticVersion localVersion = new SemanticVersion(Package.version);

            if (remoteVersion > localVersion)
            {
                // UPDATE
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

        public static string GetLastNotifiedVersion()
        {
            return EditorPrefs.GetString(LastNotifiedVersionKey);
        }

        public static void SetLastNotifiedVersion(string versionTag)
        {
            EditorPrefs.SetString(LastNotifiedVersionKey, versionTag);
        }

        public static string GetProjectRootPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        }

        private static PackageDefinition GetLocalPackageDefinition()
        {
            try
            {
                string[] packageGuids = AssetDatabase.FindAssets("package");
                int packageCount = packageGuids.Length;
                string packageIdentifierLine = $"\"name\": \"{Strings.PackageName}\",";
                for (int i = 0; i < packageCount; i++)
                {
                    string packagePath = Path.Combine(GetProjectRootPath(),
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
            catch (Exception e)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

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
            catch (Exception e)
            {
                // Don't go any further if there is an error
                return null;
            }
        }

        /// <summary>
        ///     Determine the current <see cref="InstallationMethod" /> of the GDX package.
        /// </summary>
        /// <returns>The discovered <see cref="InstallationMethod" />.</returns>
        private static InstallationMethod GetInstallationMethod()
        {
            // Well we reached this point and don't actually know, so guess we should admit it.
            return InstallationMethod.Unidentified;
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