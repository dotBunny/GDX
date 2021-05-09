// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    internal static  class CommandLineProcessorSettings
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        const string SectionID = "GDX.Developer.CommandLineParser";

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
        /// </summary>
        private static readonly GUIContent s_argumentPrefixContent = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        private static readonly GUIContent s_argumentSplitContent = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     Draw the Command Line Processor section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        internal static void Draw(SerializedObject settings)
        {
            GUI.enabled = true;

            SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Command Line Parser",
                $"{SettingsProvider.DocumentationUri}api/GDX.Developer.CommandLineParser.html");

            if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            {
                return;
            }

            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                s_argumentPrefixContent);
            EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                s_argumentSplitContent);
        }
    }
}