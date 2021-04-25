// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using UnityEditor.VersionControl;

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of version control related helper utilities.
    /// </summary>
    public static class VersionControl
    {
        /// <summary>
        /// Get an <see cref="UnityEditor.VersionControl.AssetList"/> from an <paramref name="absoluteDirectoryPath"/>.
        /// </summary>
        /// <param name="absoluteDirectoryPath">A fully qualified path on disk to query.</param>
        /// <param name="searchPattern">The search pattern to look for files with.</param>
        /// <param name="searchOption">What level of searching should be done.</param>
        /// <returns>An <see cref="UnityEditor.VersionControl.AssetList"/> containing any valid assets under version control.</returns>
        public static AssetList GetAssetListFromFolder(string absoluteDirectoryPath, string searchPattern = "*.*",
            SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (absoluteDirectoryPath == null|| !Directory.Exists(absoluteDirectoryPath))
            {
                return null;
            }

            AssetList checkoutAssets = new AssetList();
            string[] filePaths = Directory.GetFiles(absoluteDirectoryPath,searchPattern, searchOption);
            int length = filePaths.Length;
            for (int i = 0; i < length; i++)
            {
                Asset foundAsset =
                    Provider.GetAssetByPath(filePaths[i].Replace(UnityEngine.Application.dataPath, ""));
                if (foundAsset != null)
                {
                    checkoutAssets.Add(foundAsset);
                }
            }

            return checkoutAssets;
        }
    }
}