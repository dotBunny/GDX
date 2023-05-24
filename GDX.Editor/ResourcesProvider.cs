// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class ResourcesProvider
    {
        public const string ChangedClass = "changed";
        public const string HiddenClass = "hidden";
        public const string DisabledClass = "disabled";
        public const string EnabledClass = "enabled";
        public const string ExpandedClass = "expanded";
        public const string SearchClass = "search";
        public const string SearchHighlightClass = "search-item";

        /// <summary>
        ///     A collection of queried <see cref="VisualTreeAsset" /> assets keyed by their search.
        /// </summary>
        static StringKeyDictionary<VisualTreeAsset> s_Assets = new StringKeyDictionary<VisualTreeAsset>(10);

        /// <summary>
        ///     The GDX logo banner image.
        /// </summary>
        /// <remarks>Cleverly lifted from the docs templates out of project.</remarks>
        static Texture2D s_Banner;

        /// <summary>
        ///     A dotBunny logo image.
        /// </summary>
        static Texture2D s_CompanyLogo;

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
#pragma warning disable IDE0051
        static StyleSheet s_StylesheetOverride;
#pragma warning restore IDE0051

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
        ///     Returns an instance of the GDX logo banner image.
        /// </summary>
        /// <returns>An image loaded from disk, if not cached.</returns>
        public static Texture2D GetBanner()
        {
            if (s_Banner != null) return s_Banner;

            string imagePath = Path.Combine(
                Path.GetDirectoryName(UpdateProvider.LocalPackage.PackageManifestPath),
                ".docfx", "images", "home", "gdx-banner.png" );

            if (File.Exists(imagePath))
            {
                byte[] bytes = File.ReadAllBytes(imagePath);
                s_Banner = new Texture2D(1200, 630, TextureFormat.RGBA32, true);
                s_Banner.LoadImage(bytes);
            }
            return s_Banner;
        }

        /// <summary>
        ///     Returns an instance of the dotBunny logo image.
        /// </summary>
        /// <returns>An image loaded from disk, if not cached.</returns>
        public static Texture2D GetLogo()
        {
            if (s_CompanyLogo != null) return s_CompanyLogo;

            string imagePath = Path.Combine(
                Path.GetDirectoryName(UpdateProvider.LocalPackage.PackageManifestPath),
                ".docfx", "images", "branding", "dotBunny.png" );

            if (File.Exists(imagePath))
            {
                byte[] bytes = File.ReadAllBytes(imagePath);
                s_CompanyLogo = new Texture2D(800, 800, TextureFormat.RGBA32, true);
                s_CompanyLogo.LoadImage(bytes);
            }
            return s_CompanyLogo;
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
            s_Stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{UpdateProvider.LocalPackage.PackageAssetPath}/GDX.Editor/UIElements/GDXStylesShared.uxml");

            return s_Stylesheet;
        }

        public static StyleSheet GetLightThemeStylesheet()
        {
            if (s_LightThemeStylesheet != null)
            {
                return s_LightThemeStylesheet;
            }

            s_LightThemeStylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{UpdateProvider.LocalPackage.PackageAssetPath}/GDX.Editor/UIElements/GDXStylesLightTheme.uss");

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

            s_StylesheetOverride = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                $"{UpdateProvider.LocalPackage.PackageAssetPath}/GDX.Editor/UIElements/GDXStylesUnity2020.uss");

            return s_StylesheetOverride;
#endif
        }

        /// <summary>
        /// Sets up the initial stylesheets for a <see cref="VisualElement"/>.
        /// </summary>
        /// <param name="rootElement">The target <see cref="VisualElement"/> to have the stylesheets applied to.</param>
        public static void SetupStylesheets(VisualElement rootElement)
        {
            if (GetStyleSheet() != null)
            {
                rootElement.styleSheets.Add(GetStyleSheet());
            }
            if (!EditorGUIUtility.isProSkin)
            {
                if (GetLightThemeStylesheet() != null)
                {
                    rootElement.styleSheets.Add(GetLightThemeStylesheet());
                }
            }
            // Add any overrides
            if (GetStyleSheetOverride() != null)
            {
                rootElement.styleSheets.Add(GetStyleSheetOverride());
            }

            CheckTheme(rootElement);
        }

        /// <summary>
        ///     Return the target visual tree asset, by name.
        /// </summary>
        /// <remarks>This will find and cache the reference for future use, using the first asset to match the query.</remarks>
        /// <param name="targetName">Target assets name (with no extension)</param>
        /// <returns>The queried asset if found, or null.</returns>
        internal static VisualTreeAsset GetVisualTreeAsset(string targetName)
        {
            // We check for null asset as after a change it will null the original with the reference still existing
            if (!s_Assets.ContainsKey(targetName) || s_Assets[targetName] == null)
            {
                s_Assets.AddWithExpandCheck(targetName, AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                    $"{UpdateProvider.LocalPackage.PackageAssetPath}/GDX.Editor/UIElements/{targetName}.uxml"));
            }

            return s_Assets[targetName];
        }
    }
}
