using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using GDX.Editor.ProjectSettings;
using GDX.IO.Compression;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

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
        private const string LastCheckedKey = "GDX.UpdateProvider.LastChecked";

        /// <summary>
        ///     The key used by <see cref="EditorPrefs" /> to store the last update version we notified the user about.
        /// </summary>
        private const string LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        /// <summary>
        ///     The base URI for downloading the latest released tarball.
        /// </summary>
        private const string GitHubLatestUri = "https://github.com/dotBunny/GDX/archive/v";

        /// <summary>
        ///     A collection of information about the locally installed GDX package.
        /// </summary>
        public static readonly PackageProvider LocalPackage;

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
            LocalPackage = new PackageProvider();

            EditorApplication.delayCall += DelayCall;

        }

        /// <summary>
        /// Execute delayed logic that won't interfere with a current import process.
        /// </summary>
        private static void DelayCall()
        {
            if (!GDXConfig.Get().updateProviderCheckForUpdates)
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
            if (UpdatePackageDefinition == null)
            {
                UpdatePackageDefinition = GetMainPackageDefinition();
            }

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
                case PackageProvider.InstallationType.UPM:
                case PackageProvider.InstallationType.UPMBranch:
                case PackageProvider.InstallationType.UPMTag:
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
        ///     Gets the last time that we checked for an update to the package.
        /// </summary>
        public static DateTime GetLastChecked()
        {
            DateTime lastTime = new DateTime(2020, 12, 14);
            if (EditorPrefs.HasKey(LastCheckedKey))
            {
                DateTime.TryParse(EditorPrefs.GetString(LastCheckedKey), out lastTime);
            }

            return lastTime;
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
                case PackageProvider.InstallationType.UPM:
                case PackageProvider.InstallationType.UPMBranch:
                case PackageProvider.InstallationType.UPMTag:
                case PackageProvider.InstallationType.Assets:
                    return true;
                case PackageProvider.InstallationType.Unknown:
                case PackageProvider.InstallationType.UPMCommit:
                case PackageProvider.InstallationType.UPMLocal:
                case PackageProvider.InstallationType.GitHubCommit:
                default:
                    return false;
            }
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
#if UNITY_2020_2_OR_NEWER
                using WebClient webClient = new WebClient();
#else
                using (WebClient webClient = new WebClient())
                {
#endif
                // Get content of the package definition file
                string updateLocation = "main";
                if (LocalPackage.InstallationMethod == PackageProvider.InstallationType.GitHubBranch ||
                    LocalPackage.InstallationMethod == PackageProvider.InstallationType.UPMBranch)
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
#if !UNITY_2020_2_OR_NEWER
                }
#endif
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
        private static void UpgradeAssetDatabase()
        {
            // Get a temporary file
            // TODO: This will need to be changed for newer .NET (System.IO.GetTempFileName())
            string tempFile = Path.GetTempFileName();

            // Download the file
            EditorUtility.DisplayProgressBar("GDX", "Downloading Update ...", 0.25f);
            try
            {
#if UNITY_2020_2_OR_NEWER
                using WebClient webClient = new WebClient();
                webClient.DownloadFile(GitHubLatestUri + UpdatePackageDefinition.version + ".tar.gz",
                    tempFile);
#else
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(GitHubLatestUri + UpdatePackageDefinition.version + ".tar.gz",
                        tempFile);
                }
#endif
            }
            catch (Exception e)
            {
                // We will end up here if the formulated Uri is bad.
                Trace.Output(Trace.TraceLevel.Warning, e.Message);
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

            // Handle VCS
            if (Provider.enabled && Provider.isActive)
            {
                AssetList checkoutAssets = VersionControl.GetAssetListFromFolder(targetPath);
                Task checkoutTask = Provider.Checkout(checkoutAssets, CheckoutMode.Both);
                checkoutTask.Wait();
            }

            // Remove all existing content
            if (targetPath != null)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
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
        private static void UpgradeGitHub()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            Process process = new Process();
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal, FileName = "git.exe"
            };
            process.StartInfo = startInfo;

            string targetPath = Path.GetDirectoryName(LocalPackage.PackageManifestPath);
            if (targetPath != null)
            {
                // Handle VCS
                if (Provider.enabled && Provider.isActive)
                {
                    AssetList checkoutAssets = VersionControl.GetAssetListFromFolder(targetPath);
                    Task checkoutTask = Provider.Checkout(checkoutAssets, CheckoutMode.Both);
                    checkoutTask.Wait();
                }

                try
                {
                    // Pause asset database
                    AssetDatabase.StartAssetEditing();

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
        private static void UpgradeUnityPackageManager()
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

#if UNITY_2020_1_OR_NEWER
                    // Tell PackageManager to resolve our newly altered file.
                    UnityEditor.PackageManager.Client.Resolve();
#else
                    EditorUtility.DisplayDialog("GDX Package Update",
                        "Your version of Unity requires that you either loose focus and return to Unity, or simply restart the editor to detect the change.",
                        "OK");
#endif
                }
            }
        }
    }
}