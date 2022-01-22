// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace GDX.Editor.ProjectSettings
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
                guiHandler = searchContext =>
                {
                    // Get a serialized version of the settings
                    UnityEditor.SerializedObject settingsObject = new UnityEditor.SerializedObject(Config.Get());

                    // Start wrapping the content
                    UnityEditor.EditorGUILayout.BeginVertical(SettingsStyles.WrapperStyle);

                    PackageStatusSection.Draw();

                    // Build out sections
                    AutomaticUpdatesSettings.Draw(settingsObject);
                    UnityEngine.GUILayout.Space(5);

                    BuildInfoSettings.Draw(settingsObject);
                    UnityEngine.GUILayout.Space(5);

                    CommandLineProcessorSettings.Draw(settingsObject);
                    UnityEngine.GUILayout.Space(5);

                    EnvironmentSettings.Draw(settingsObject);
                    UnityEngine.GUILayout.Space(5);

                    LocaleSettings.Draw(settingsObject);
#if GDX_VISUALSCRIPTING
                    UnityEngine.GUILayout.Space(5);
                    VisualScriptingSettings.Draw(settingsObject);
#endif
                    // Apply any serialized setting changes
                    settingsObject.ApplyModifiedPropertiesWithoutUndo();

                    // Stop wrapping the content
                    UnityEditor.EditorGUILayout.EndVertical();
                },
                keywords = s_searchKeywords
            };
        }
    }
}