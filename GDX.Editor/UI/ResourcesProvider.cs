// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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
        ///     The cached reference to the override stylesheet.
        /// </summary>
        /// <remarks>
        ///     Used to accomodate version specific styling.
        /// </remarks>
        private static StyleSheet s_styleSheetOverride;

        /// <summary>
        ///     A cached pathing to where our UXML are stored.
        /// </summary>
        private static string s_foundAssetFolder;

        /// <summary>
        ///     Apply light/dark mode classes.
        /// </summary>
        /// <param name="element">The target element to have classes applied to.</param>
        public static void CheckTheme(VisualElement element)
        {
            // Handle Skin
            if (EditorGUIUtility.isProSkin)
            {
                element.AddToClassList("dark");
                element.RemoveFromClassList("light");
            }
            else
            {
                element.RemoveFromClassList("dark");
                element.AddToClassList("light");
            }
        }

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

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStylesShared");
            if (potentialStyles.Length <= 0)
            {
                return s_stylesheet;
            }

            s_stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_stylesheet;
        }

        /// <summary>
        ///     Return the override stylesheet.
        /// </summary>
        /// <remarks>This will find and cache the stylesheet reference for future use, using the first asset to match the query.</remarks>
        /// <returns>The stylesheet if found, or null.</returns>
        public static StyleSheet GetStyleSheetOverride()
        {
#if UNITY_2021_1_OR_NEWER
            return null;
#else
            if (s_styleSheetOverride != null)
            {
                return s_styleSheetOverride;
            }

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStylesUnity2020");
            if (potentialStyles.Length <= 0)
            {
                return s_styleSheetOverride;
            }

            s_styleSheetOverride = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_styleSheetOverride;
#endif
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

                s_foundAssetFolder = assetPath.Substring(0,
                    assetPath.IndexOf(targetName, StringComparison.Ordinal));
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
