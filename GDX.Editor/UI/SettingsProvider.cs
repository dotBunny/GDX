// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.Editor.UI
{
    /// <summary>
    ///     GDX Assembly Settings Provider
    /// </summary>
    [HideFromDocFX]
    public static class SettingsProvider
    {
        /// <summary>
        ///     The public URI of the package's documentation.
        /// </summary>
        public const string DocumentationUri = "https://gdx.dotbunny.com/";

        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly List<string> s_searchKeywords = new List<string>(new[]
        {
            "gdx", "update", "parser", "commandline", "build"
        });

        private static readonly List<IConfigSection> s_configSections = new List<IConfigSection>();

        /// <summary>
        ///     Get <see cref="UnityEditor.SettingsProvider" /> for GDX assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [UnityEditor.SettingsProvider]
        public static UnityEditor.SettingsProvider Get()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new UnityEditor.SettingsProvider("Project/GDX", UnityEditor.SettingsScope.Project)
            {
                label = "GDX",
                activateHandler = (searchContext, rootElement) =>
                {
                    rootElement.styleSheets.Add(ResourcesProvider.GetStyleSheet());
                    ResourcesProvider.GetVisualTreeAsset("GDXProjectSettings").CloneTree(rootElement);
                },
                // guiHandler = searchContext =>
                // {
                //     // Get a working copy
                //     GDXConfig tempConfig = new GDXConfig(Core.Config);
                //
                //     // Start wrapping the content
                //     UnityEditor.EditorGUILayout.BeginVertical(GDX.Editor.ProjectSettings.SettingsStyles.WrapperStyle);
                //
                //    // PackageStatusSection.Draw(tempConfig);
                //
                //     foreach (IConfigSection section in s_configSections)
                //     {
                //         section.Draw(tempConfig);
                //     }
                //
                //
                //     // check for changes
                //     // save newconfig?
                //
                //
                //     // Stop wrapping the content
                //     UnityEditor.EditorGUILayout.EndVertical();
                // },
                keywords = s_searchKeywords
            };
        }

        public static void RegisterConfigSection(IConfigSection section)
        {
            if(s_configSections.Contains(section)) return;
            s_configSections.Add(section);
        }
    }
}