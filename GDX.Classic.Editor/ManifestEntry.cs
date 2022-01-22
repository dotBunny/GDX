// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX;
using GDX.Editor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    /// An object representative of the GDX entry in a manifest lockfile.
    /// </summary>
    [Serializable]
    public class ManifestEntry
    {
        public string version;
        public int depth;
        public string source;
        public string[] dependencies;
        public string hash;

        [NonSerialized]
        public PackageProvider.InstallationType installationType;
        [NonSerialized]
        public string tag;

        public static ManifestEntry Get(string json)
        {
            ManifestEntry returnObject = JsonUtility.FromJson<ManifestEntry>(json);
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
                    returnObject.installationType = PackageProvider.InstallationType.UPMTag;
                    returnObject.tag = tag;
                    return returnObject;
                }

                // Check for what we assume is a commit hash
                if (tag.Length == 40 && tag.HasLowerCase() && !tag.HasUpperCase())
                {
                    returnObject.installationType = PackageProvider.InstallationType.GitHubCommit;
                    returnObject.tag = tag;
                    return returnObject;
                }

                // Left with assuming its a branch?
                returnObject.installationType = PackageProvider.InstallationType.UPMBranch;
                returnObject.tag = tag;
                return returnObject;
            }

            returnObject.installationType = PackageProvider.InstallationType.UPM;
            returnObject.tag = "main";
            return returnObject;
        }
    }
}