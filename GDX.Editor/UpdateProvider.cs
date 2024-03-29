using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using GDX.Developer;
using GDX.Editor.Windows.ProjectSettings.ConfigSections;
using GDX.IO.Compression;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace GDX.Editor
{
    /// <summary>
    ///     An autonomous provider which detects and notifies if updates are available for the GDX package.
    /// </summary>
    [HideFromDocFX]
    [InitializeOnLoad]
    public static class UpdateProvider
    {
        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last time we checked for an update.
        /// </summary>
        const string k_LastCheckedKey = "GDX.UpdateProvider.LastChecked";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last update version we notified the user about.
        /// </summary>
        const string k_LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        /// <summary>
        ///     The base URI for downloading the latest released tarball.
        /// </summary>
        const string k_GitHubLatestUri = "https://github.com/dotBunny/GDX/archive/v";

        /// <summary>
        ///     A collection of information about the locally installed GDX package.
        /// </summary>
        public static readonly PackageProvider LocalPackage;

        /// <summary>
        ///     If an update check has occured, this will be filled with its <see cref="PackageProvider.PackageDefinition" />.
        /// </summary>
        public static PackageProvider.PackageDefinition UpdatePackageDefinition;

        /// <summary>
        ///     Initialize the update provider, and check if necessary.
        /// </summary>
        static UpdateProvider()
        {
            // Create a copy of the local package provider
            LocalPackage = new PackageProvider();

            EditorApplication.delayCall += DelayCall;
        }

        /// <summary>
        ///     Execute delayed logic that won't interfere with a current import process.
        /// </summary>
        static void DelayCall()
        {
            if (!Config.UpdateProviderCheckForUpdates)
            {
                return;
            }

            // Should we check for updates?
            DateTime targetDate = GetLastChecked().AddDays(AutomaticUpdatesSettings.UpdateDayCountSetting);
            if (DateTime.Now >= targetDate)
            {
                CheckForUpdates();
            }
        }

        /// <summary>
        ///     Is there an update available to the local package, based on the provided
        ///     <see cref="PackageProvider.PackageDefinition" />.
        /// </summary>
        /// <param name="updatePackageDefinition">The found <see cref="PackageProvider.PackageDefinition" /> on GitHub.</param>
        /// <returns>true/false if an update is found.</returns>
        public static bool HasUpdate(PackageProvider.PackageDefinition updatePackageDefinition)
        {
            if (updatePackageDefinition == null || LocalPackage == null)
            {
                return false;
            }

            // Package versions
            SemanticVersion updatePackageVersion = new SemanticVersion(updatePackageDefinition.version);
            SemanticVersion localPackageVersion = new SemanticVersion(LocalPackage.Definition.version);

            // Unity versions
            SemanticVersion currentUnityVersion = new SemanticVersion(Application.unityVersion);
            SemanticVersion minimumUnityVersion = new SemanticVersion(updatePackageDefinition.unity);

            // Actually figure out if we have something
            return updatePackageVersion > localPackageVersion &&
                   currentUnityVersion >= minimumUnityVersion;
        }

        /// <summary>
        ///     Attempt to do the upgrade of the package based on the established <see cref="PackageProvider.InstallationType" />.
        /// </summary>
        /// <param name="forceUpgrade">Should we bypass all safety checks?</param>
        public static void AttemptUpgrade(bool forceUpgrade = false)
        {
            UpdatePackageDefinition ??= GetMainPackageDefinition();

            string messageStart =
                $"There is a new version of GDX available ({UpdatePackageDefinition.version}).\n";
            if (!forceUpgrade && !IsUpgradable())
            {
                EditorUtility.DisplayDialog("GDX Update Available",
                    $"{messageStart}Unfortunately we are unable to determine a proper upgrade path for your package. We are UNABLE to upgrade your package for you automatically.",
                    "Doh!");

                SetLastNotifiedVersion(UpdatePackageDefinition.version);

                return;
            }

            switch (LocalPackage.InstallationMethod)
            {
                // Currently this option doesnt function due to the IsUpgrade check, but its WIP
                case PackageProvider.InstallationType.PackageManager:
                case PackageProvider.InstallationType.PackageManagerBranch:
                case PackageProvider.InstallationType.PackageManagerTag:
                    if (EditorUtility.DisplayDialog("GDX Update Available",
                            $"{messageStart}Would you like to have the package attempt to upgrade itself through UPM to the newest version automatically?",
                            "Yes", "No"))
                    {
                        UpgradeUnityPackageManager();
                    }
                    else
                    {
                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                    }

                    break;

                case PackageProvider.InstallationType.GitHub:
                case PackageProvider.InstallationType.GitHubBranch:
                case PackageProvider.InstallationType.GitHubTag:
                    if (EditorUtility.DisplayDialog("GDX Update Available",
                            $"{messageStart}Would you like your cloned repository updated?\n\nIMPORTANT!\n\nThis will \"reset hard\" and \"pull\" the repository, wiping any local changes made.",
                            "Yes", "No"))
                    {
                        UpgradeGitHub();
                    }
                    else
                    {
                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                    }

                    break;
                case PackageProvider.InstallationType.Assets:
                    if (EditorUtility.DisplayDialog("GDX Update Available",
                            $"{messageStart}Would you like your install replaced?\n\nIMPORTANT!\n\nThis will remove any local changes to GDX.",
                            "Yes", "No"))
                    {
                        UpgradeAssetDatabase();
                    }
                    else
                    {
                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                    }

                    break;
            }
        }

        /// <summary>
        ///     Check for updates!
        /// </summary>
        public static void CheckForUpdates()
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
        ///     Get the local changelog path.
        /// </summary>
        /// <returns>A full path if it exists, otherwise null.</returns>
        public static string GetLocalChangelogPath()
        {
            string filePath = Path.Combine(
                Path.GetDirectoryName(
                    LocalPackage.PackageManifestPath) ??
                string.Empty, "CHANGELOG.md");

            return File.Exists(filePath) ? filePath : null;
        }

        /// <summary>
        ///     Get some or all of the version information from the local changelog.
        /// </summary>
        /// <param name="versionLimit">
        ///     An optional limit to the number of versions worth of information to return.
        /// </param>
        /// <returns>The changelog lines corresponding to the versions.</returns>
        public static string[] GetLocalChangelog(int versionLimit = -1)
        {
            string path = GetLocalChangelogPath();
            if (path != null)
            {
                string[] lines = File.ReadAllLines(path);
                int lineCount = lines.Length;
                int versionCount = 0;
                List<string> returnLines = new List<string>(100);
                for (int i = 7; i < lineCount; i++)
                {
                    string line = lines[i];
                    if (line.StartsWith("## "))
                    {
                        versionCount++;
                    }

                    if (versionLimit > 0 && versionCount > versionLimit)
                    {
                        return returnLines.ToArray();
                    }

                    returnLines.Add(line);
                }

                return returnLines.ToArray();
            }

            return null;
        }

        /// <summary>
        ///     Gets the last time that we checked for an update to the package.
        /// </summary>
        public static DateTime GetLastChecked()
        {
            DateTime lastTime = new DateTime(2020, 12, 14);
            if (EditorPrefs.HasKey(k_LastCheckedKey))
            {
                DateTime.TryParse(EditorPrefs.GetString(k_LastCheckedKey), out lastTime);
            }

            return lastTime;
        }

        /// <summary>
        ///     Get the path to the local license file.
        /// </summary>
        /// <returns>A full path if it exists, otherwise null.</returns>
        public static string GetLocalLicensePath()
        {
            string filePath = Path.Combine(
                Path.GetDirectoryName(
                    LocalPackage.PackageManifestPath) ??
                string.Empty, "LICENSE");

            return File.Exists(filePath) ? filePath : null;
        }

        /// <summary>
        ///     Can the local package be upgraded automatically through various means?
        /// </summary>
        /// <returns>A true/false answer to the question.</returns>
        public static bool IsUpgradable()
        {
            switch (LocalPackage.InstallationMethod)
            {
                case PackageProvider.InstallationType.GitHub:
                case PackageProvider.InstallationType.GitHubBranch:
                case PackageProvider.InstallationType.GitHubTag:
                case PackageProvider.InstallationType.PackageManager:
                case PackageProvider.InstallationType.PackageManagerBranch:
                case PackageProvider.InstallationType.PackageManagerTag:
                case PackageProvider.InstallationType.Assets:
                    return true;
                // case PackageProvider.InstallationType.Unknown:
                // case PackageProvider.InstallationType.UPMCommit:
                // case PackageProvider.InstallationType.UPMLocal:
                // case PackageProvider.InstallationType.GitHubCommit:
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Set the last time that the package was checked for updates to right now.
        /// </summary>
        static void SetLastChecked()
        {
            EditorPrefs.SetString(k_LastCheckedKey, DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        ///     Get the last version of the package which was presented to the user for update.
        /// </summary>
        /// <returns>A version string.</returns>
        public static string GetLastNotifiedVersion()
        {
            return EditorPrefs.GetString(k_LastNotifiedVersionKey);
        }

        /// <summary>
        ///     Set the version of the package presented to update too.
        /// </summary>
        /// <param name="versionTag">The package version string.</param>
        static void SetLastNotifiedVersion(string versionTag)
        {
            EditorPrefs.SetString(k_LastNotifiedVersionKey, versionTag);
        }

        /// <summary>
        ///     Poll the main GitHub repository to find its package.json, parsing into a
        ///     <see cref="PackageProvider.PackageDefinition" />.
        /// </summary>
        /// <returns>A <see cref="PackageProvider.PackageDefinition" /> instance.</returns>
        static PackageProvider.PackageDefinition GetMainPackageDefinition()
        {
            try
            {
                using WebClient webClient = new WebClient();

                // Get content of the package definition file
                string updateLocation = "main";
                if (LocalPackage.InstallationMethod == PackageProvider.InstallationType.GitHubBranch ||
                    LocalPackage.InstallationMethod == PackageProvider.InstallationType.PackageManagerBranch)
                {
                    updateLocation = LocalPackage.SourceTag;
                }

                string packageJsonContent =
                    webClient.DownloadString(
                        $"https://raw.githubusercontent.com/dotBunny/GDX/{updateLocation}/package.json");

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

        /// <summary>
        ///     Upgrade the package, with the understanding that it is located in the Asset database.
        /// </summary>
        static void UpgradeAssetDatabase()
        {
            // Get a temporary file
            // TODO: This will need to be changed for newer .NET (System.IO.GetTempFileName())
            string tempFile = Path.GetTempFileName();

            // Download the file
            EditorUtility.DisplayProgressBar("GDX", "Downloading Update ...", 0.25f);
            try
            {
                using WebClient webClient = new WebClient();
                webClient.DownloadFile(k_GitHubLatestUri + UpdatePackageDefinition.version + ".tar.gz",
                    tempFile);
            }
            catch (Exception e)
            {
                // We will end up here if the formulated Uri is bad.
                Debug.LogException(e);
                return;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            string tempExtractFolder = Path.Combine(Path.GetTempPath(), PackageProvider.PackageName);

            // Remove previous upgrade folder (if it exists)
            if (Directory.Exists(tempExtractFolder))
            {
                Directory.Delete(tempExtractFolder, true);
            }

            Platform.EnsureFolderHierarchyExists(tempExtractFolder);

            // Extract downloaded tarball to the temp folder
            TarFile.ExtractToDirectory(tempFile, tempExtractFolder, true);

            // Get desired target placement
            string targetPath = Path.GetDirectoryName(LocalPackage.PackageManifestPath);


            // Remove all existing content
            if (targetPath != null)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
                    VersionControl.CheckoutFolder(targetPath);
                    Directory.Delete(targetPath, true);

                    // Drop in new content
                    Directory.Move(
                        Path.Combine(tempExtractFolder, "GDX-" + UpdatePackageDefinition.version),
                        targetPath);

                    AssetDatabase.ImportAsset(LocalPackage.PackageAssetPath);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
            }
        }

        /// <summary>
        ///     Upgrade the package, with the understanding that is a standard GitHub clone.
        /// </summary>
        static void UpgradeGitHub()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal, FileName = "git.exe"
            };
            process.StartInfo = startInfo;

            string targetPath = Path.GetDirectoryName(LocalPackage.PackageManifestPath);
            if (targetPath != null)
            {
                try
                {
                    // Pause asset database
                    AssetDatabase.StartAssetEditing();
                    VersionControl.CheckoutFolder(targetPath);

                    if (LocalPackage?.PackageManifestPath != null)
                    {
                        startInfo.WorkingDirectory = targetPath;

                        startInfo.Arguments = "reset --hard";
                        process.Start();
                        process.WaitForExit();

                        startInfo.Arguments = "pull";
                        process.Start();
                        process.WaitForExit();

                        // Lets force the import anyways now
                        AssetDatabase.ImportAsset(LocalPackage.PackageAssetPath);
                    }
                }
                finally
                {
                    // Return asset database monitoring back to normal
                    AssetDatabase.StopAssetEditing();
                }
            }
        }

        /// <summary>
        ///     Upgrade the package, with the understanding that it was added via UPM.
        /// </summary>
        static void UpgradeUnityPackageManager()
        {
            // We're going to remove the entry from the lockfile triggering it to record an update
            string packageManifestLockFile =
                Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6), "Packages",
                    "packages-lock.json");

            // We can only do this if the file exists!
            if (File.Exists(packageManifestLockFile))
            {
                string[] lockFileContents = File.ReadAllLines(packageManifestLockFile);
                int lockFileLength = lockFileContents.Length;
                int depth = 0;

                List<string> newFileContent = new List<string>(lockFileLength);
                for (int i = 0; i < lockFileLength; i++)
                {
                    // Identify the block
                    // ReSharper disable once StringLiteralTypo
                    if (lockFileContents[i].Trim() == "\"com.dotbunny.gdx\": {")
                    {
                        depth++;
                        continue;
                    }

                    // Rebuild replacement file while were iterating
                    if (depth == 0)
                    {
                        newFileContent.Add(lockFileContents[i]);
                    }
                    else
                    {
                        if (lockFileContents[i].Contains("{"))
                        {
                            depth++;
                        }

                        if (lockFileContents[i].Contains("}"))
                        {
                            depth--;
                        }
                    }
                }

                // We have a change to write
                if (newFileContent.Count != lockFileLength)
                {
                    File.WriteAllLines(packageManifestLockFile, newFileContent.ToArray());
                    // Tell PackageManager to resolve our newly altered file.
                    Client.Resolve();
                }
            }
        }
    }
}