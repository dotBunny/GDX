using System.Collections;
using System.Collections.Generic;
using GDX.Editor;
using UnityEditor;
using UnityEngine;

namespace GDX.Developer.Editor
{
    /// <summary>
    /// Project settings for the <see cref="CommandLineParser"/>.
    /// </summary>
    public static class CommandLineParserSettings
    {
        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        // ReSharper disable HeapView.ObjectAllocation.Evident
        private static readonly HashSet<string> s_settingsKeywords = new HashSet<string>(new[] {"gdx", "developer", "parser"});
        // ReSharper restore HeapView.ObjectAllocation.Evident

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly GUIContent s_settingsArgumentPrefix = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly GUIContent s_settingsArgumentSplit = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     Get <see cref="SettingsProvider" /> for GDX.Developer updates.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SettingsProvider("Project/GDX/Command Line Parser", SettingsScope.Project)
            {
                label = "Command Line Parser",
                guiHandler = searchContext =>
                {
                    GDXStyles.BeginGUILayout();

                    SerializedObject settings = Config.GetSerializedConfig();
                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"), s_settingsArgumentPrefix);
                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"), s_settingsArgumentSplit);

                    settings.ApplyModifiedPropertiesWithoutUndo();

                    GDXStyles.EndGUILayout();
                },
                keywords = s_settingsKeywords
            };
        }
    }
}
