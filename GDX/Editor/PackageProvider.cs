// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of information regarding the GDX package.
    /// </summary>
    public class PackageProvider
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
            ///     The package was cloned into a folder in the project from GitHub.
            /// </summary>
            GitHubClone = 2,

            /// <summary>
            ///     The package was found in the assets folder. This could be a Asset Store installation or even
            ///     just a zip decompressed into a project.
            /// </summary>
            UnderAssets = 3
        }

        /// <summary>
        ///     The <see cref="PackageDefinition" /> for the installed package.
        /// </summary>
        public readonly PackageDefinition Definition;

        /// <summary>
        ///     The <see cref="PackageProvider.InstallationType" /> detected during construction of the package.
        /// </summary>
        public readonly InstallationType InstallationMethod;

        /// <summary>
        ///     Fully qualified path to the package.json file.
        /// </summary>
        public readonly string PackagePath;

        /// <summary>
        ///     Initialize a new <see cref="PackageProvider"/>.
        /// </summary>
        public PackageProvider()
        {
            // Find Local Definition
            try
            {
                PackagePath = GetPackagePath();
                if (PackagePath != null)
                {
                    Definition = JsonUtility.FromJson<PackageDefinition>(File.ReadAllText(PackagePath));
                }

                if (Definition == null)
                {
                    return;
                }

                InstallationMethod = GetInstallationType();
            }
            catch (Exception)
            {
                // Don't go any further if there is an error
            }
        }

        /// <summary>
        ///     Determine the current <see cref="PackageProvider.InstallationType" /> of the GDX package.
        /// </summary>
        /// <returns>The discovered <see cref="PackageProvider.InstallationType" />.</returns>
        private InstallationType GetInstallationType()
        {
            if (Definition == null)
            {
                return InstallationType.Unidentified;
            }

            // Cache directory where the package.json was found
            string packageDirectory = Path.GetDirectoryName(PackagePath);
            string projectDirectory = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

            // Unity Package Manager Check
            string projectManifestPath = Path.Combine(projectDirectory, "Packages", "manifest.json");
            if (File.Exists(projectManifestPath))
            {
                string[] projectManifest = File.ReadAllLines(projectManifestPath);
                int projectManifestLength = projectManifest.Length;
                for (int i = 0; i < projectManifestLength; i++)
                {
                    if (projectManifest[i].Contains(Strings.PackageName))
                    {
                        return InstallationType.UnityPackageManager;
                    }
                }
            }

            // GitHub Clone Check
            string gitDirectory = Path.Combine(packageDirectory ?? string.Empty, ".git");
            if (packageDirectory != null && Directory.Exists(gitDirectory))
            {
                string[] gitConfig = File.ReadAllLines(Path.Combine(gitDirectory, "config"));
                int gitConfigLength = gitConfig.Length;
                for (int i = 0; i < gitConfigLength; i++)
                {
                    if (gitConfig[i].Trim() == "url = https://github.com/dotBunny/GDX.git")
                    {
                        return InstallationType.GitHubClone;
                    }
                }
            }

            // Assets Folder ... at least?
            if (packageDirectory != null && packageDirectory.StartsWith(Application.dataPath))
            {
                return InstallationType.UnderAssets;
            }

            // Well we reached this point and don't actually know, so guess we should admit it.
            return InstallationType.Unidentified;
        }

        /// <summary>
        ///     Get the package's on disk path to its package.json.
        /// </summary>
        /// <returns>A full path to the package.json.</returns>
        private string GetPackagePath()
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
                    return packagePath;
                }
            }
            return null;
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