// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     An object representative of the GDX entry in a manifest lockfile.
    /// </summary>
    [Serializable]
    public class ManifestEntry
    {
        [NonSerialized] public PackageProvider.InstallationType InstallationType;

        [NonSerialized] public string Tag;

        public static ManifestEntry Get(string json)
        {
            ManifestEntry returnObject;

            // Catch badly formatted lines from exploding
            try
            {
                returnObject = JsonUtility.FromJson<ManifestEntry>(json);
            }
            catch
            {
                Debug.LogWarning($"Badly formatted ManifestEntry.\n{json}");
                return null;
            }

            if (returnObject == null)
            {
                return null;
            }

            // Evaluate for tag
            string tag = returnObject.version.GetAfterLast("#");
            if (tag != null)
            {
                if (tag.StartsWith("v"))
                {
                    returnObject.InstallationType = PackageProvider.InstallationType.PackageManagerTag;
                    returnObject.Tag = tag;
                    return returnObject;
                }

                // Check for what we assume is a commit hash
                if (tag.Length == 40 && tag.HasLowerCase() && !tag.HasUpperCase())
                {
                    returnObject.InstallationType = PackageProvider.InstallationType.GitHubCommit;
                    returnObject.Tag = tag;
                    return returnObject;
                }

                // Left with assuming its a branch?
                returnObject.InstallationType = PackageProvider.InstallationType.PackageManagerBranch;
                returnObject.Tag = tag;
                return returnObject;
            }

            returnObject.InstallationType = PackageProvider.InstallationType.PackageManager;
            returnObject.Tag = "main";
            return returnObject;
        }
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        public string version;
        public int depth;
        public string source;
        public string[] dependencies;
        public string hash;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
    }
}