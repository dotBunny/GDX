using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using GDX.IO.Compression;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
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
        ///     The key used by <see cref="EditorPrefs" /> to store the last update version we notified the user about.
        /// </summary>
        private const string LastNotifiedVersionKey = "GDX.UpdateProvider.LastNotifiedVersion";

        /// <summary>
        ///     The public URI of the latest changes, as Markdown.
        /// </summary>
        /// <remarks>The main branch is used to contain released versions only, so if it is found there, it is the latest release.</remarks>
        public const string GitHubChangelogUri = "https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md";

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

            GDXConfig config = Config.Get();
            if (!config.updateProviderCheckForUpdates)
            {
                return;
            }

            // Should we check for updates?
            DateTime targetDate = GetLastChecked().AddDays(GDXSettings.UpdateDayCountSetting);
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
            if (updatePackageDefinition == null)
            {
                return false;
            }

            // Package versions
            SemanticVersion updatePackageVersion = new SemanticVersion(UpdatePackageDefinition.version);
            SemanticVersion localPackageVersion = new SemanticVersion(LocalPackage.Definition.version);

            // Unity versions
            SemanticVersion currentUnityVersion = new SemanticVersion(Application.unityVersion);
            SemanticVersion minimumUnityVersion = new SemanticVersion(UpdatePackageDefinition.unity);

            // Actually figure out if we have something
            return updatePackageVersion > localPackageVersion &&
                   currentUnityVersion >= minimumUnityVersion;
        }

        /// <summary>
        ///     Attempt to do the upgrade of the package based on the established <see cref="PackageProvider.InstallationType" />.
        /// </summary>
        public static void AttemptUpgrade()
        {
            string messageStart =
                $"There is a new version of GDX available ({UpdatePackageDefinition.version}).\n";
            switch (LocalPackage.InstallationMethod)
            {
                case PackageProvider.InstallationType.Unknown:
                    EditorUtility.DisplayDialog("GDX Update Available",
                        $"{messageStart}Unfortunately we are unable to determine how your package was installed. We are UNABLE to upgrade your package for you.",
                        "Doh!");
                    SetLastNotifiedVersion(UpdatePackageDefinition.version);
                    break;

                case PackageProvider.InstallationType.UPM:
                    if (EditorUtility.DisplayDialog("GDX Update Available",
                        $"{messageStart}Would you like to have the package attempt to upgrade itself through UPM to the newest version automatically?",
                        "Yes", "No"))
                    {
                        // TODO: Unsure if this actually will work if it is not an internal package?
                        Client.Add(Strings.PackageName);
                    }
                    else
                    {
                        SetLastNotifiedVersion(UpdatePackageDefinition.version);
                    }

                    break;

                case PackageProvider.InstallationType.GitHub:
                    if (EditorUtility.DisplayDialog("GDX Update Available",
                        $"{messageStart}Would you like your cloned repository updated?\n\nIMPORTANT!\n\nThis will \"reset hard\" and \"pull\" the repository, wiping any local changes made.",
                        "Yes", "No"))
                    {
                        // ReSharper disable once HeapView.ObjectAllocation.Evident
                        Process process = new Process();
                        // ReSharper disable once HeapView.ObjectAllocation.Evident
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            WindowStyle = ProcessWindowStyle.Normal, FileName = "git.exe"
                        };
                        process.StartInfo = startInfo;

                        try
                        {
                            // Pause asset database
                            AssetDatabase.StartAssetEditing();

                            if (LocalPackage?.PackageManifestPath != null)
                            {
                                startInfo.WorkingDirectory =
                                    Path.GetDirectoryName(LocalPackage.PackageManifestPath) ?? string.Empty;

                                startInfo.Arguments = "reset --hard";
                                process.Start();
                                process.WaitForExit();

                                startInfo.Arguments = "pull";
                                process.Start();
                                process.WaitForExit();

                                // Lets force the import anyways now
                                AssetDatabase.ImportAsset(LocalPackage.PackageManifestPath);
                            }
                        }
                        finally
                        {
                            // Return asset database monitoring back to normal
                            AssetDatabase.StopAssetEditing();
                        }
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


                        string tempExtractFolder = Path.Combine(Path.GetTempPath(), Strings.PackageName);

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

                        // TODO: VCS Checkout? maybe make a function
                        if (Provider.enabled && Provider.isActive)
                        {
                            // UnityEditor.VersionControl.Provider.Checkout(mat, UnityEditor.VersionControl.CheckoutMode.Both);
                        }

                        // Remove all existing content
                        if (targetPath != null)
                        {
                            Directory.Delete(targetPath, true);

                            // Drop in new content
                            Directory.Move(
                                Path.Combine(tempExtractFolder, "GDX-" + UpdatePackageDefinition.version),
                                targetPath);
                        }
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