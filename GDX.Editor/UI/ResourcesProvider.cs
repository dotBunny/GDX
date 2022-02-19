// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.UI
{
    public static class ResourcesProvider
    {
        /// <summary>
        ///     A collection of queried <see cref="VisualTreeAsset" /> assets keyed by their search.
        /// </summary>
        static StringKeyDictionary<VisualTreeAsset> s_Assets = new StringKeyDictionary<VisualTreeAsset>(10);

        /// <summary>
        ///     The cached reference to the global stylesheet.
        /// </summary>
        static StyleSheet s_Stylesheet;
        
        /// <summary>
        ///     The cached reference to the override stylesheet.
        /// </summary>
        /// <remarks>
        ///     Used to accomodate version specific styling.
        /// </remarks>
        static StyleSheet s_StylesheetOverride;

        /// <summary>
        ///     A cached pathing to where our UXML are stored.
        /// </summary>
        static string s_FoundAssetFolder;
        
        /// <summary>
        ///     The cached reference to the light theme stylesheet.
        /// </summary>
        /// <remarks>
        ///     Used to augment the styles used for those who use Unity's light theme. Who does this?
        /// </remarks>
        static StyleSheet s_LightThemeStylesheet;

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
            if (s_Stylesheet != null)
            {
                return s_Stylesheet;
            }

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStylesShared");
            if (potentialStyles.Length <= 0)
            {
                return s_Stylesheet;
            }

            s_Stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_Stylesheet;
        }

        public static StyleSheet GetLightThemeStylesheet()
        {
            if (s_LightThemeStylesheet != null)
            {
                return s_LightThemeStylesheet;
            }

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStylesLightTheme");
            if (potentialStyles.Length <= 0)
            {
                return s_LightThemeStylesheet;
            }

            s_LightThemeStylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_LightThemeStylesheet;
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
            if (s_StylesheetOverride != null)
            {
                return s_StylesheetOverride;
            }

            string[] potentialStyles = AssetDatabase.FindAssets("t:Stylesheet GDXStylesUnity2020");
            if (potentialStyles.Length <= 0)
            {
                return s_StylesheetOverride;
            }

            s_StylesheetOverride = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(potentialStyles[0]));

            return s_StylesheetOverride;
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
            if (s_Assets.ContainsKey(targetName))
            {
                return s_Assets[targetName];
            }

            if (s_FoundAssetFolder == null)
            {
                string[] potentialTree = AssetDatabase.FindAssets($"t:VisualTreeAsset {targetName}");
                if (potentialTree.Length <= 0)
                {
                    return null;
                }

                string assetPath = AssetDatabase.GUIDToAssetPath(potentialTree[0]);

                s_FoundAssetFolder = assetPath.Substring(0,
                    assetPath.IndexOf(targetName, StringComparison.Ordinal));
                s_Assets.AddWithExpandCheck(targetName,
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath));
            }
            else
            {
                s_Assets.AddWithExpandCheck(targetName,
                    AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{s_FoundAssetFolder}{targetName}.uxml"));
            }

            return s_Assets[targetName];
        }
    }
}
