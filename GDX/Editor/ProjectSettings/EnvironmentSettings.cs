// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    internal static class EnvironmentSettings
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.Environment";

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.traceDebugLevels" />.
        /// </summary>
        private static readonly GUIContent s_debugLevelsContent = new GUIContent(
            "Debug Tracing",
            "The levels of trace call to be logged in a debug build.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.traceDevelopmentLevels" />.
        /// </summary>
        private static readonly GUIContent s_developmentLevelsContent = new GUIContent(
            "Development Tracing",
            "The levels of trace call to be logged in a development/editor build.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.traceReleaseLevels" />.
        /// </summary>
        private static readonly GUIContent s_releaseLevelsContent = new GUIContent(
            "Release Tracing",
            "The levels of trace call to be logged in a release build.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.environmentScriptingDefineSymbol" />.
        /// </summary>
        private static readonly GUIContent s_scriptingDefineSymbolContent = new GUIContent(
            "Ensure GDX Symbol",
            "Should GDX make sure that there is a GDX scripting define symbol across all viable build target groups.");

        internal static void Draw(SerializedObject settings)
        {
            GUI.enabled = true;

            SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Environment");

            if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            {
                return;
            }

            EditorGUILayout.PropertyField(settings.FindProperty("environmentScriptingDefineSymbol"),
                s_scriptingDefineSymbolContent);


            SerializedProperty developmentLevelsProperty = settings.FindProperty("traceDevelopmentLevels");
            EditorGUI.BeginChangeCheck();
            ushort newDevelopmentLevels = (ushort)EditorGUILayout.MaskField(s_developmentLevelsContent, developmentLevelsProperty.intValue,
                developmentLevelsProperty.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                developmentLevelsProperty.intValue = newDevelopmentLevels;
            }

            SerializedProperty debugLevelsProperty = settings.FindProperty("traceDebugLevels");
            EditorGUI.BeginChangeCheck();
            ushort newDebugLevels = (ushort)EditorGUILayout.MaskField(s_debugLevelsContent, debugLevelsProperty.intValue,
                debugLevelsProperty.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                debugLevelsProperty.intValue = newDebugLevels;
            }

            SerializedProperty releaseLevelsProperty = settings.FindProperty("traceReleaseLevels");
            EditorGUI.BeginChangeCheck();
            ushort newReleaseLevels = (ushort)EditorGUILayout.MaskField(s_releaseLevelsContent, releaseLevelsProperty.intValue,
                releaseLevelsProperty.enumDisplayNames);
            if (EditorGUI.EndChangeCheck())
            {
                releaseLevelsProperty.intValue = newReleaseLevels;
            }
        }
    }
}