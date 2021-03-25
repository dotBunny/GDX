// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Text;
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
        ///     Reference to the Unity package name for GDX.
        /// </summary>
        public const string PackageName = "com.dotbunny.gdx";

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
            ///     The package was installed via Unity's traditional UPM process, however with a branch specified.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            UPMBranch = 2,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with a tag specified.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            UPMTag = 3,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with a commit specified.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            UPMCommit = 4,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with local file reference.
            /// </summary>
            // ReSharper disable once InconsistentNaming
            UPMLocal = 5,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub.
            /// </summary>
            GitHub = 10,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub, however with a branch specified.
            /// </summary>
            GitHubBranch = 11,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub, however with a tag specified.
            /// </summary>
            GitHubTag = 12,

            /// <summary>
            ///     The package was cloned into a folder in the project from GitHub, however with a commit specified.
            /// </summary>
            GitHubCommit = 13,

            /// <summary>
            ///     The package was found in the assets folder. This could be a Asset Store installation or even
            ///     just a zip decompressed into a project.
            /// </summary>
            Assets = 20
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
        ///     Additional information outline what sort of package has been detected. This usually will indicate a
        ///     specified version to include, or a specific commit.
        /// </summary>
        public readonly string SourceTag;

        /// <summary>
        ///     Initialize a new <see cref="PackageProvider" />.
        /// </summary>
        public PackageProvider()
        {
            EditorApplication.delayCall += DelayCall;

            // Find Local Definition
            // ReSharper disable once StringLiteralTypo
            string[] editorAssemblyDefinition = AssetDatabase.FindAssets("GDX.Editor t:asmdef");
            if (editorAssemblyDefinition.Length > 0)
            {
                // Establish package root path
                PackageAssetPath =
                    Path.Combine(
                        Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(editorAssemblyDefinition[0])) ??
                        string.Empty, "..", "..");

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

            // Lets figure out where we came from
            (InstallationType installationType, string sourceTag) = GetInstallationType();
            InstallationMethod = installationType;
            SourceTag = sourceTag;
        }

        /// <summary>
        /// Execute delayed logic that won't interfere with a current import process.
        /// </summary>
        private void DelayCall()
        {
            // Make sure that the project has the GDX preprocessor added
            if (ConfigProvider.Get().environmentScriptingDefineSymbol)
            {
                EnsureScriptingDefineSymbol();
            }
        }

        /// <summary>
        ///     Ensure that the GDX define is present across all viable platforms.
        /// </summary>
        public static void EnsureScriptingDefineSymbol()
        {
            // Create a list of all the build targets
            Array buildTargets = Enum.GetValues(typeof(BuildTargetGroup));
            int buildTargetsCount = buildTargets.Length;

            // Iterate over them all - skipping unknown
            for (int i = 1; i < buildTargetsCount; i++)
            {
                // Get our object
                object target = buildTargets.GetValue(i);

                // Cast back
                BuildTargetGroup group = (BuildTargetGroup)target;
                Type enumType =  group.GetType();

                // Check if we can find an ObsoleteAttribute
                FieldInfo fieldInfo = enumType.GetField(Enum.GetName(enumType, target));
                Attribute foundAttribute = fieldInfo.GetCustomAttribute(typeof(ObsoleteAttribute), false);

                // It doesnt have one, so we should assume we can update the scripting defines for this target.
                if (foundAttribute != null)
                {
                    continue;
                }

                string[] defines = null;
#if UNITY_2020_1_OR_NEWER
                PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out defines);
#else
                defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group).Split(';');
#endif
                int location = defines.FirstIndexOfItem("GDX");

                // Found
                if (location != -1)
                {
                    continue;
                }

                // Add to it!
                int oldLength = defines.Length;
                string[] newDefines = new string[oldLength + 1];
                Array.Copy(defines, newDefines, oldLength);
                newDefines[oldLength] = "GDX";


#if UNITY_2020_2_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbolsForGroup(@group, newDefines);
#else
                StringBuilder output = new StringBuilder();
                foreach (string s in newDefines)
                {
                    output.Append(s);
                    output.Append(";");
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group,output.ToString().TrimEnd(new[] {';'}));
#endif


            }
        }

        /// <summary>
        ///     Get a friendly <see cref="string" /> name of an <see cref="InstallationType" />.
        /// </summary>
        /// <param name="installationType">The <see cref="InstallationType" /> to return a name for.</param>
        /// <returns>A friendly name for <paramref name="installationType" />.</returns>
        public static string GetFriendlyName(InstallationType installationType)
        {
            switch (installationType)
            {
                case InstallationType.UPM:
                    return "Unity Package Manager";
                case InstallationType.UPMBranch:
                    return "Unity Package Manager (Branch)";
                case InstallationType.UPMTag:
                    return "Unity Package Manager (Tag)";
                case InstallationType.UPMCommit:
                    return "Unity Package Manager (Commit)";
                case InstallationType.UPMLocal:
                    return "Unity Package Manager (Local)";
                case InstallationType.GitHub:
                    return "GitHub";
                case InstallationType.GitHubBranch:
                    return "GitHub (Branch)";
                case InstallationType.GitHubTag:
                    return "GitHub (Tag)";
                case InstallationType.GitHubCommit:
                    return "GitHub (Commit)";
                case InstallationType.Assets:
                    return "Asset Database";
                default:
                    return "Unknown";
            }
        }

        /// <summary>
        ///     Determine the current <see cref="PackageProvider.InstallationType" /> of the GDX package.
        /// </summary>
        /// <returns>
        ///     A <see cref="Tuple" /> containing the discovered <see cref="PackageProvider.InstallationType" /> and any
        ///     source tag.
        /// </returns>
        private (InstallationType, string) GetInstallationType()
        {
            // NOTHING - If we dont have any sort of definition to work with, we really cant do anything, honest.
            if (Definition == null)
            {
                return (InstallationType.Unknown, null);
            }

            // Cache directory where the package.json was found
            string packageDirectory = Path.GetDirectoryName(PackageManifestPath);
            string projectDirectory = Application.dataPath.Substring(0, Application.dataPath.Length - 6);

            // UNITY PACKAGE MANAGER - The package was added via UPM, now the fun task begins of identifying what sort of UPM inclusion was made.
            string projectManifestPath = Path.Combine(projectDirectory, "Packages", "manifest.json");
            string manifestLine = null;
            if (File.Exists(projectManifestPath))
            {
                string[] projectManifest = File.ReadAllLines(projectManifestPath);
                int projectManifestLength = projectManifest.Length;

                // Loop through manifest looking for the package name
                for (int i = 0; i < projectManifestLength; i++)
                {
                    if (!projectManifest[i].Contains(PackageName))
                    {
                        continue;
                    }

                    manifestLine = projectManifest[i];
                    break;
                }
            }

            if (!string.IsNullOrEmpty(manifestLine))
            {
                // Local UPM reference
                if (manifestLine.Contains("\"file:"))
                {
                    return (InstallationType.UPMLocal, null);
                }

                // Time to see whats in the lock file
                string packageManifestLockFilePath =
                    Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6), "Packages",
                        "packages-lock.json");

                if (!File.Exists(packageManifestLockFilePath))
                {
                    // No lock go bold!
                    return (InstallationType.UPM, null);
                }

                string[] lockFile = File.ReadAllLines(packageManifestLockFilePath);
                int lockFileLength = lockFile.Length;

                // Loop through lockfile for the package to determine further information on how it has been added.
                bool insidePackage = false;
                for (int i = 0; i < lockFileLength; i++)
                {
                    string workingLine = lockFile[i].Trim();
                    if (workingLine.StartsWith("\"com.dotbunny.gdx\""))
                    {
                        insidePackage = true;
                        continue;
                    }

                    // We want to make sure that we are inside the package definition and that we are looking at the version line.
                    if (!insidePackage || !workingLine.StartsWith("\"version\""))
                    {
                        continue;
                    }

                    string versionLine = workingLine.Substring(12, workingLine.Length - (12 + 2));
                    string tag = versionLine.GetAfterLast("#");


                    // No actual tag found, so we are a straight UPM but we should consider it main line
                    if (tag == versionLine)
                    {
                        return (InstallationType.UPM, "main");
                    }

                    // All of our release tags start with 'v'
                    if (tag.StartsWith("v"))
                    {
                        return (InstallationType.UPMTag, tag);
                    }

                    // Check for what we assume is a commit hash
                    if (tag.Length == 40 && tag.HasLowerCase() && !tag.HasUpperCase())
                    {
                        return (InstallationType.GitHubCommit, tag);
                    }

                    return (InstallationType.UPMBranch, tag);
                }

                // Well we at least can say it was UPM bound
                return (InstallationType.UPM, null);
            }

            // GITHUB - The package was added via some sort of GitHub cloning into the project.
            string gitDirectory = Path.Combine(packageDirectory ?? string.Empty, ".git");

            if (packageDirectory != null && Directory.Exists(gitDirectory))
            {
                string gitConfigPath = Path.Combine(gitDirectory, "config");
                if (File.Exists(gitConfigPath))
                {
                    string[] gitConfig = File.ReadAllLines(gitConfigPath);
                    int gitConfigLength = gitConfig.Length;
                    for (int i = 0; i < gitConfigLength; i++)
                    {
                        // We look for a non-version locked URI
                        if (gitConfig[i].Trim() != "url = https://github.com/dotBunny/GDX.git")
                        {
                            continue;
                        }

                        // Lets check for that HEAD file now to see if we can get a branch that is being targeted.
                        string gitHeadPath = Path.Combine(gitDirectory, "HEAD");
                        if (!File.Exists(gitHeadPath))
                        {
                            return (InstallationType.GitHub, null);
                        }
                        string[] gitHead = File.ReadAllLines(gitHeadPath);
                        return (InstallationType.GitHubBranch, gitHead[0].GetAfterLast("/").Trim());
                    }
                }
            }

            // ASSET DATABASE - A fallback where I guess this is what we are doing?
            if (packageDirectory != null && packageDirectory.StartsWith(Application.dataPath))
            {
                return (InstallationType.Assets, null);
            }

            // Well we reached this point and don't actually know, so guess we should admit it.
            return (InstallationType.Unknown, null);
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