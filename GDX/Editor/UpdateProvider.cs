using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using UnityEditor;
using UnityEditor.PackageManager;
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
        ///     The public URI of the latest changes, as Markdown.
        /// </summary>
        /// <remarks>The main branch is used to contain released versions only, so if it is found there, it is the latest release.</remarks>
        private const string GitHubChangelogUri = "https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md";

        /// <summary>
        ///     The public URI of releases for GDX.
        /// </summary>
        private const string GitHubReleasesUri = "https://github.com/dotBunny/GDX/releases";

        private const string GitHubLatestUri = "https://github.com/dotBunny/GDX/archive/v";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last time we checked for an update.
        /// </summary>
        private const string LastCheckedKey = "GDX.UpdateProvider.LastChecked";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last update version we notified the user about.
        /// </summary>
        private const string LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store <see cref="UpdateDayCountSetting" />.
        /// </summary>
        private const string UpdateDayCountKey = "GDX.UpdateProvider.UpdateDayCount";

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
            // Create a copy of the local package provider
            // ReSharper disable once HeapView.ObjectAllocation.Evident
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

                    EditorGUILayout.BeginHorizontal(GDXStyles.Header);
                    GUILayout.Label(
                        UpdatePackageDefinition != null
                            ? $"Local version: {s_localPackage.Definition.version}\nGitHub version: {UpdatePackageDefinition.version}\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}."
                            : $"Local version: {s_localPackage.Definition.version}\nGitHub version: Unknown\nLast checked on {GetLastChecked().ToString(Localization.LocalTimestampFormat)}.",
                        EditorStyles.boldLabel);

                    // Force things to the right
                    GUILayout.FlexibleSpace();

                    EditorGUILayout.BeginVertical();
                    if (HasUpdate(UpdatePackageDefinition))
                    {
                        if (GUILayout.Button("Changelog", GDXStyles.Button))
                        {
                            Application.OpenURL(GitHubChangelogUri);
                        }

                        if (GUILayout.Button("Update", GDXStyles.Button))
                        {
                            AttemptUpgrade();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Manual Check", GDXStyles.Button))
                        {
                            CheckForUpdates();
                        }
                    }

                    EditorGUILayout.EndVertical();


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

        private static bool HasUpdate(PackageProvider.PackageDefinition updatePackageDefinition)
        {
            if (updatePackageDefinition == null)
            {
                return false;
            }

            // Package versions
            SemanticVersion updatePackageVersion = new SemanticVersion(UpdatePackageDefinition.version);
            SemanticVersion localPackageVersion = new SemanticVersion(s_localPackage.Definition.version);

            // Unity versions
            SemanticVersion currentUnityVersion = new SemanticVersion(Application.unityVersion);
            SemanticVersion minimumUnityVersion = new SemanticVersion(UpdatePackageDefinition.unity);

            // Actually figure out if we have something
            return updatePackageVersion > localPackageVersion &&
                   currentUnityVersion >= minimumUnityVersion;
        }

        private static void AttemptUpgrade()
        {
            // string messageStart =
            //     $"There is a new version of GDX available ({UpdatePackageDefinition.version}).\n";
            // switch (s_localPackage.InstallationMethod)
            // {
            //     case PackageProvider.InstallationType.Unidentified:
            //         EditorUtility.DisplayDialog("GDX Update Available",
            //             $"{messageStart}Unfortunately we are unable to determine how your package was installed. We are UNABLE to upgrade your package for you.",
            //             "Doh!");
            //         break;
            //
            //     case PackageProvider.InstallationType.UnityPackageManager:
            //         if (EditorUtility.DisplayDialog("GDX Update Available",
            //             $"{messageStart}Would you like to have the package attempt to upgrade itself through UPM to the newest version automatically?",
            //             "Yes", "No"))
            //         {
            //             // TODO: Unsure if this actually will work if it is not an internal package?
            //             Client.Add(Strings.PackageName);
            //         }
            //         else
            //         {
            //             SetLastNotifiedVersion(UpdatePackageDefinition.version);
            //         }
            //
            //         break;
            //
            //     case PackageProvider.InstallationType.GitHubClone:
            //         if (EditorUtility.DisplayDialog("GDX Update Available",
            //             $"{messageStart}Would you like your cloned repository updated?\n\nIMPORTANT!\n\nThis will \"reset hard\" and \"pull\" the repository, wiping any local changes made.",
            //             "Yes", "No"))
            //         {
            //             // ReSharper disable once HeapView.ObjectAllocation.Evident
            //             Process process = new Process();
            //             // ReSharper disable once HeapView.ObjectAllocation.Evident
            //             ProcessStartInfo startInfo = new ProcessStartInfo
            //             {
            //                 WindowStyle = ProcessWindowStyle.Normal, FileName = "git.exe"
            //             };
            //             process.StartInfo = startInfo;
            //
            //             try
            //             {
            //                 // Pause asset database
            //                 AssetDatabase.StartAssetEditing();
            //
            //                 if (s_localPackage?.PackagePath != null)
            //                 {
            //                     startInfo.WorkingDirectory =
            //                         Path.GetDirectoryName(s_localPackage.PackagePath) ?? string.Empty;
            //
            //                     startInfo.Arguments = "reset --hard";
            //                     process.Start();
            //                     process.WaitForExit();
            //
            //                     startInfo.Arguments = "pull";
            //                     process.Start();
            //                     process.WaitForExit();
            //
            //                     // Lets force the import anyways now
            //                     AssetDatabase.ImportAsset(s_localPackage.PackagePath);
            //                 }
            //             }
            //             finally
            //             {
            //                 // Return asset database monitoring back to normal
            //                 AssetDatabase.StopAssetEditing();
            //             }
            //         }
            //         else
            //         {
            //             SetLastNotifiedVersion(UpdatePackageDefinition.version);
            //         }
            //
            //         break;
            //     case PackageProvider.InstallationType.UnderAssets:

                    // Convert over to GitHub clone?

                    // Get a temporary file
                    // TODO: This will need to be changed for newer .NET (System.IO.GetTempFileName())
                    string tempFile = Path.GetTempFileName();

                    // Download the file
                    EditorUtility.DisplayProgressBar("GDX", "Downloading Update ...", 0.25f);
                    try
                    {
                        using WebClient webClient = new WebClient();
                        webClient.DownloadFile(GitHubLatestUri + UpdatePackageDefinition.version + ".tar.gz",
                            tempFile);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    finally
                    {
                        EditorUtility.ClearProgressBar();
                    }

                    string targetPath = Path.GetDirectoryName(s_localPackage.PackagePath);
                    string tempExtractFolder = Path.Combine(Path.GetTempPath(), Strings.PackageName);

                    // Remove previous upgrade folder (if it exists)
                    if (Directory.Exists(tempExtractFolder))
                    {
                        Directory.Delete(tempExtractFolder, true);
                    }
                    Platform.EnsureFolderHierarchyExists(tempExtractFolder);

                    UnityEngine.Debug.Log($"Extract {tempFile} to {tempExtractFolder}");
                    GDX.IO.Compression.TarFile.ExtractToDirectory(tempFile, tempExtractFolder, true);



                    //         break;
                    // }
        }

        /// <summary>
        ///     Check for updates!
        /// </summary>
        private static void CheckForUpdates()
        {
            SetLastChecked();

            // Cache the update information
            UpdatePackageDefinition = GetMainPackageDefinition();

            if (HasUpdate(UpdatePackageDefinition) &&
                GetLastNotifiedVersion() != UpdatePackageDefinition.version)
            {
                AttemptUpgrade();
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
        // ReSharper disable once MemberCanBePrivate.Global
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