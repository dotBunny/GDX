// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Text;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Application = UnityEngine.Application;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of information regarding the GDX package.
    /// </summary>
    [HideFromDocFX]
    public class PackageProvider
    {
        /// <summary>
        ///     The public URI of the package's development documentation.
        /// </summary>
        public const string DevDocumentationUri = "https://gdx-dev.dotbunny.com/";

        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string MainDocumentationUri = "https://gdx.dotbunny.com/";
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
            PackageManager = 1,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with a branch specified.
            /// </summary>
            PackageManagerBranch = 2,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with a tag specified.
            /// </summary>
            PackageManagerTag = 3,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with a commit specified.
            /// </summary>
            PackageManagerCommit = 4,

            /// <summary>
            ///     The package was installed via Unity's traditional UPM process, however with local file reference.
            /// </summary>
            PackageManagerLocal = 5,

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
        ///     Unity's own information about the package.
        /// </summary>
        public readonly PackageInfo PackageManagerInfo;

        /// <summary>
        ///     Asset database base path of package.
        /// </summary>
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

            PackageManagerInfo = PackageInfo.FindForAssembly(Assembly.GetAssembly(typeof(PackageProvider)));
            if (PackageManagerInfo != null)
            {
                // Fast route
                PackageAssetPath = PackageManagerInfo.assetPath;
                PackageManifestPath = Path.Combine(PackageManagerInfo.resolvedPath, "package.json");
            }
            else
            {
                // Slow fallback for in project extraction
                string[] editorAssemblyDefinition = AssetDatabase.FindAssets("GDX.Editor t:asmdef");
                if (editorAssemblyDefinition.Length > 0)
                {
                    PackageAssetPath =
                        Path.Combine(
                            Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(editorAssemblyDefinition[0])) ??
                            string.Empty, "..");

                    PackageManifestPath = Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                        PackageAssetPath ?? string.Empty, "package.json");
                }
            }

            if (PackageManifestPath == null || !File.Exists(PackageManifestPath))
            {
                return;
            }

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
        ///     Ensure that the GDX based shaders are always included in builds.
        /// </summary>
        /// <remarks>
        ///     Useful to ensure that the default materials used by the <see cref="GDX.Developer.DebugDraw" /> are added
        ///     to the build.
        /// </remarks>
        public static void EnsureAlwaysIncludeShaders()
        {
            GraphicsSettings graphicsSettings =
                AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            SerializedObject serializedObject = new SerializedObject(graphicsSettings);
            SerializedProperty alreadyIncludedShaders = serializedObject.FindProperty("m_AlwaysIncludedShaders");

            Shader[] gdxShaders = Rendering.ShaderProvider.GetProvidedShaders();
            int gdxShaderCount = gdxShaders.Length;
            int alreadyCount = alreadyIncludedShaders.arraySize;

            bool altered = false;

            // Loop through all GDX shaders
            for (int i = 0; i < gdxShaderCount; i++)
            {
                bool foundShader = false;
                for (int j = 0; j < alreadyCount; j++)
                {
                    SerializedProperty element = alreadyIncludedShaders.GetArrayElementAtIndex(j);
                    if (gdxShaders[i] == element.objectReferenceValue)
                    {
                        foundShader = true;
                        break;
                    }
                }

                if (!foundShader)
                {
                    alreadyIncludedShaders.InsertArrayElementAtIndex(alreadyCount);
                    SerializedProperty newElement = alreadyIncludedShaders.GetArrayElementAtIndex(alreadyCount);
                    newElement.objectReferenceValue = gdxShaders[i];
                    alreadyCount++;
                    altered = true;
                }
            }

            if (altered)
            {
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        ///     Get the documentation base URI based on what branch the local package is detected as.
        /// </summary>
        /// <returns>A URI to the GDX documentation.</returns>
        public static string GetDocumentationBaseUri()
        {
            if (UpdateProvider.LocalPackage != null && UpdateProvider.LocalPackage.SourceTag != null &&
                UpdateProvider.LocalPackage.SourceTag.StartsWith("dev"))
            {
                return DevDocumentationUri;
            }
            return MainDocumentationUri;
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
                case InstallationType.PackageManager:
                    return "Unity Package Manager";
                case InstallationType.PackageManagerBranch:
                    return "Unity Package Manager (Branch)";
                case InstallationType.PackageManagerTag:
                    return "Unity Package Manager (Tag)";
                case InstallationType.PackageManagerCommit:
                    return "Unity Package Manager (Commit)";
                case InstallationType.PackageManagerLocal:
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
        (InstallationType, string) GetInstallationType()
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
                    return (InstallationType.PackageManagerLocal, null);
                }

                // Time to see whats in the lock file
                string packageManifestLockFilePath =
                    Path.Combine(Application.dataPath.Substring(0, Application.dataPath.Length - 6), "Packages",
                        "packages-lock.json");

                if (!File.Exists(packageManifestLockFilePath))
                {
                    // No lock go bold!
                    return (InstallationType.PackageManager, null);
                }

                string[] lockFile = File.ReadAllLines(packageManifestLockFilePath);
                int lockFileLength = lockFile.Length;

                // Loop through lockfile for the package to determine further information on how it has been added.
                int depth = -1;
                StringBuilder manifestContent = new StringBuilder();
                for (int i = 0; i < lockFileLength; i++)
                {
                    string workingLine = lockFile[i].Trim();

                    // ReSharper disable once StringLiteralTypo
                    if (workingLine.StartsWith("\"com.dotbunny.gdx\""))
                    {
                        manifestContent.AppendLine("{");
                        depth = 1;
                        continue;
                    }

                    if (depth > 0)
                    {
                        if (workingLine.Contains("{"))
                        {
                            depth++;
                        }
                        if (workingLine.Contains("}"))
                        {
                            depth--;
                        }
                        manifestContent.AppendLine(workingLine);

                        if (depth != 0)
                        {
                            continue;
                        }

                        ManifestEntry manifestEntry = ManifestEntry.Get(
                            manifestContent.ToString().Trim().TrimEnd(',').Trim());

                        return manifestEntry == null ? (InstallationType.PackageManager, null) :
                            (installationType: manifestEntry.InstallationType, tag: manifestEntry.Tag);
                    }
                }

                // Well we at least can say it was UPM bound
                return (InstallationType.PackageManager, null);
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
        ///     Ensure that the GDX define is present across all viable platforms.
        /// </summary>
        public static void UpdateScriptingDefineSymbols()
        {
            // Create a list of all the build targets
            Array buildTargets = Enum.GetValues(typeof(BuildTargetGroup));

            // Iterate over them all - skipping unknown
            int buildTargetsCount = buildTargets.Length;
            int changeCount = 0;
            for (int i = 1; i < buildTargetsCount; i++)
            {
                if (UpdateScriptingDefineSymbolsForBuildTargetGroup((BuildTargetGroup)buildTargets.GetValue(i)))
                {
                    changeCount++;
                }
            }

            if (changeCount > 0)
            {
                EditorApplication.ExecuteMenuItem("File/Save Project");
            }
        }

        /// <summary>
        /// Execute delayed logic that won't interfere with a current import process.
        /// </summary>
        static void DelayCall()
        {
            // Check our scripting defines
            UpdateScriptingDefineSymbols();

            if (Config.EnvironmentAlwaysIncludeShaders)
            {
                EnsureAlwaysIncludeShaders();
            }
        }

        /// <summary>
        ///     Update an given <see cref="BuildTargetGroup"/>'s scripting defines.
        /// </summary>
        /// <param name="group">The target build group to alter.</param>
        static bool UpdateScriptingDefineSymbolsForBuildTargetGroup(BuildTargetGroup group)
        {
            Type enumType =  group.GetType();

            // Check if we can find an ObsoleteAttribute
            FieldInfo fieldInfo = enumType.GetField(Enum.GetName(enumType, group));
            Attribute foundAttribute = fieldInfo.GetCustomAttribute(typeof(ObsoleteAttribute), false);

            // It doesnt have one, so we should assume we can update the scripting defines for this target.
            if (foundAttribute != null)
            {
                return false;
            }

#if UNITY_2023_1_OR_NEWER
            PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group),
                out string[] defines);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(group, out string[] defines);
#endif

            bool changes = false;
            SimpleList<string> lazyDefines = new SimpleList<string>(defines, defines.Length);

            // Check for GDX
            int symbolLocation = lazyDefines.FirstIndexOf("GDX");
            if (Config.EnvironmentScriptingDefineSymbol && symbolLocation == -1)
            {
                lazyDefines.AddWithExpandCheck("GDX");
                changes = true;
            }
            else if (!Config.EnvironmentScriptingDefineSymbol && symbolLocation != -1)
            {
                lazyDefines.RemoveAtSwapBack(symbolLocation);
                changes = true;
            }

            // Check for GDX Tools Menu
            int toolsLocation = lazyDefines.FirstIndexOf("GDX_TOOLS");
            if (Config.EnvironmentToolsMenu && toolsLocation == -1)
            {
                lazyDefines.AddWithExpandCheck("GDX_TOOLS");
                changes = true;
            }
            else if (!Config.EnvironmentToolsMenu && toolsLocation != -1)
            {
                lazyDefines.RemoveAtSwapBack(toolsLocation);
                changes = true;
            }

            if (changes && lazyDefines.Array != null)
            {
                lazyDefines.Compact();
#if UNITY_2023_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(group), lazyDefines.Array);
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, lazyDefines.Array);
#endif
            }

            return changes;
        }

        /// <summary>
        ///     A miniature package definition useful for quickly parsing a remote package definition.
        /// </summary>
        [Serializable]
        public class PackageDefinition
        {
#pragma warning disable IDE1006
            // ReSharper disable InconsistentNaming
            public string version;
            public string unity;
            // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
        }
    }
}