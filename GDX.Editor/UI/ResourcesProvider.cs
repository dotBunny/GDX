// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.UI
{
    public static class ResourcesProvider
    {
        /// <summary>
        ///     A collection of queried <see cref="VisualTreeAsset" /> assets keyed by their search.
        /// </summary>
        private static readonly Dictionary<string, VisualTreeAsset> s_assets = new Dictionary<string, VisualTreeAsset>();

        /// <summary>
        ///     The cached reference to the global stylesheet.
        /// </summary>
        private static StyleSheet s_stylesheet;

        /// <summary>
        ///     A cached pathing to where our UXML are stored.
        /// </summary>
        private static string s_foundAssetFolder;

        /// <summary>
        ///     Return the global stylesheet.
        /// </summary>
        /// <remarks>This will find and cache the stylesheet reference for future use, using the first asset to match the query.</remarks>
        /// <returns>The stylesheet if found, or null.</returns>
        public static StyleSheet GetStyleSheet()
        {
            if (s_stylesheet != null)
            {
                return s_stylesheet;
            }

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStyle");
            if (potentialStyles.Length <= 0)
            {
                return s_stylesheet;
            }

            s_stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_stylesheet;
        }

        /// <summary>
        ///     Return the target visual tree asset, by name.
        /// </summary>
        /// <remarks>This will find and cache the reference for future use, using the first asset to match the query.</remarks>
        /// <param name="targetName">Target assets name (with no extension)</param>
        /// <returns>The queried asset if found, or null.</returns>
        internal static VisualTreeAsset GetVisualTreeAsset(string targetName)
        {
            if (s_assets.ContainsKey(targetName))
            {
                return s_assets[targetName];
            }

            if (s_foundAssetFolder == null)
            {
                string[] potentialTree = AssetDatabase.FindAssets($"t:VisualTreeAsset {targetName}");
                if (potentialTree.Length <= 0)
                {
                    return null;
                }

                string assetPath = AssetDatabase.GUIDToAssetPath(potentialTree[0]);

                s_foundAssetFolder = assetPath.Substring(0, assetPath.IndexOf(targetName));
                s_assets.Add(targetName,
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath));
            }
            else
            {
                s_assets.Add(targetName,
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{s_foundAssetFolder}{targetName}.uxml"));
            }

            return s_assets[targetName];
        }
    }
}
