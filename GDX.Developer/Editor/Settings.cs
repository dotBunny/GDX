using System.Collections;
using System.Collections.Generic;
using GDX.Editor;
using UnityEditor;
using UnityEngine;

namespace GDX.Developer.Editor
{
    /// <summary>
    /// GDX.Developer Assembly Settings
    /// </summary>
    public static class Settings
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
        private static readonly GUIContent s_contentArgumentPrefix = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        private static readonly GUIContent s_contentArgumentSplit = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     Get <see cref="SettingsProvider" /> for GDX.Developer assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SettingsProvider("Project/GDX/Developer", SettingsScope.Project)
            {
                label = "Developer",
                guiHandler = searchContext =>
                {
                    SerializedObject settings = Config.GetSerializedConfig();
                    GDXStyles.BeginGUILayout();

                    #region Command Line Parser
                    GDXStyles.SectionHeader("Command Line Parser");

                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"), s_contentArgumentPrefix);
                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"), s_contentArgumentSplit);

                    #endregion

                    GDXStyles.EndGUILayout();
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = s_settingsKeywords
            };
        }
    }
}
