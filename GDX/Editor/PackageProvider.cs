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
            Unknown = 0,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            UPM = 1,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub.
            /// </summary>
            GitHub = 2,

            /// <summary>
            ///     The package was found in the assets folder. This could be a Asset Store installation or even
            ///     just a zip decompressed into a project.
            /// </summary>
            Assets = 3
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
        ///     Asset database path to the root of the package.
        /// </summary>
        /// <remarks>
        ///     This is useful for situations where you need to provide an asset database relative path.
        /// </remarks>
        public readonly string PackageAssetPath;

        /// <summary>
        ///     Fully qualified path to the package.json file.
        /// </summary>
        public readonly string PackageManifestPath;

        /// <summary>
        ///     Initialize a new <see cref="PackageProvider" />.
        /// </summary>
        public PackageProvider()
        {
            // Find Local Definition
            // ReSharper disable once StringLiteralTypo
            string[] editorAssemblyDefinition = AssetDatabase.FindAssets("GDX.Editor t:asmdef");
            if (editorAssemblyDefinition.Length > 0)
            {
                // Establish package root path
                PackageAssetPath =
                    Path.Combine(
                        Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(editorAssemblyDefinition[0])) ??
                        string.Empty, Strings.PreviousFolder, Strings.PreviousFolder);

                // Build the package manifest path
                PackageManifestPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                    PackageAssetPath ?? string.Empty, "package.json");

                // Make sure the file exists
                if (!File.Exists(PackageManifestPath))
                {
                    return;
                }
            }

            // Lets try and parse the package JSON
            try
            {
                Definition = JsonUtility.FromJson<PackageDefinition>(File.ReadAllText(PackageManifestPath));
            }
            catch (Exception)
            {
                // Don't go any further if there is an error
            }

            // It didn't actually parse correctly so lets just stop right now.
            if (Definition == null)
            {
                InstallationMethod = InstallationType.Unknown;
                return;
            }

            InstallationMethod = GetInstallationType();
        }

        /// <summary>
        ///     Determine the current <see cref="PackageProvider.InstallationType" /> of the GDX package.
        /// </summary>
        /// <returns>The discovered <see cref="PackageProvider.InstallationType" />.</returns>
        private InstallationType GetInstallationType()
        {
            if (Definition == null)
            {
                return InstallationType.Unknown;
            }

            // Cache directory where the package.json was found
            string packageDirectory = Path.GetDirectoryName(PackageManifestPath);
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
                        return projectManifest[i].Contains("\"file:") ? InstallationType.Unknown : InstallationType.UPM;
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
                        return InstallationType.GitHub;
                    }
                }
            }

            // Assets Folder ... at least?
            if (packageDirectory != null && packageDirectory.StartsWith(Application.dataPath))
            {
                return InstallationType.Assets;
            }

            // Well we reached this point and don't actually know, so guess we should admit it.
            return InstallationType.Unknown;
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