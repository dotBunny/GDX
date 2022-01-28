// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor;
using GDX.Editor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    internal class EnvironmentSettings : IConfigSection
    {
        private VisualElement _element;

        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.Environment";

        /// <summary>
        ///     Settings content for <see cref="Config.traceDebugLevels" />.
        /// </summary>
        private readonly GUIContent s_debugLevelsContent = new GUIContent(
            "",
            "The levels of trace call to be logged in a debug build.");

        /// <summary>
        ///     Settings content for <see cref="Config.traceDebugOutputToUnityConsole" />.
        /// </summary>
        private readonly GUIContent s_debugConsoleOutput = new GUIContent(
            "",
            "Should traces be outputted to Unity's console in debug builds");

        /// <summary>
        ///     Settings content for <see cref="Config.traceDevelopmentLevels" />.
        /// </summary>
        private readonly GUIContent s_developmentLevelsContent = new GUIContent(
            "",
            "The levels of trace call to be logged in a development/editor build.");

        /// <summary>
        ///     Settings content for <see cref="Config.traceDevelopmentOutputToUnityConsole" />.
        /// </summary>
        private readonly GUIContent s_developmentConsoleOutput = new GUIContent(
            "",
            "Should traces be outputted to Unity's console in development builds.");

        /// <summary>
        ///     Settings content for <see cref="Config.traceReleaseLevels" />.
        /// </summary>
        private readonly GUIContent s_releaseLevelsContent = new GUIContent(
            "",
            "The levels of trace call to be logged in a release build.");

        /// <summary>
        ///     Settings content for <see cref="Config.environmentScriptingDefineSymbol" />.
        /// </summary>
        private readonly GUIContent s_scriptingDefineSymbolContent = new GUIContent(
            "Ensure GDX Symbol",
            "Should GDX make sure that there is a GDX scripting define symbol across all viable build target groups.");

        private readonly GUIContent s_traceNoticeContent = new GUIContent(
            "Make sure to disable console output if you have subscribed additional logging systems which may echo back to the console.");

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new EnvironmentSettings());
        }
        public string GetTemplateName()
        {
            return "GDXProjectSettingsEnvironment";
        }

        public void BindSectionContent(VisualElement rootElement)
        {
            // GUI.enabled = true;
            //
            // SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Environment");
            //
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return;
            // }
            //
            // settings.environmentScriptingDefineSymbol = EditorGUILayout.Toggle(s_scriptingDefineSymbolContent, settings.environmentScriptingDefineSymbol);
            //
            // GUILayout.Space(10);
            // // Arguments (we're going to make sure they are forced to uppercase).
            // GUILayout.Label("Traces", SettingsStyles.SubSectionHeaderTextStyle);
            //
            // GUILayout.BeginHorizontal(SettingsStyles.InfoBoxStyle);
            // GUILayout.Label(SettingsStyles.NoticeIcon, SettingsStyles.NoHorizontalStretchStyle);
            // GUILayout.Label(s_traceNoticeContent, SettingsStyles.WordWrappedLabelStyle);
            // GUILayout.EndHorizontal();
            //
            // EditorGUILayout.BeginHorizontal();
            // EditorGUILayout.LabelField(GUIContent.none, SettingsStyles.ColumnHeaderStyle, SettingsLayoutOptions.FixedWidth130LayoutOptions);
            // GUILayout.Space(10);
            // EditorGUILayout.LabelField("Flags", SettingsStyles.ColumnHeaderStyle, SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // GUILayout.Space(10);
            // EditorGUILayout.LabelField("Console Output", SettingsStyles.ColumnHeaderStyle, SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // EditorGUILayout.EndHorizontal();
            //
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label("Development", SettingsLayoutOptions.FixedWidth130LayoutOptions);
            // GUILayout.Space(10);
            // SerializedProperty developmentLevelsProperty = settings.FindProperty("traceDevelopmentLevels");
            // EditorGUI.BeginChangeCheck();
            // ushort newDevelopmentLevels = (ushort)EditorGUILayout.MaskField(s_developmentLevelsContent, developmentLevelsProperty.intValue,
            //     developmentLevelsProperty.enumDisplayNames,  SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     developmentLevelsProperty.intValue = newDevelopmentLevels;
            // }
            // GUILayout.Space(10);
            // EditorGUILayout.PropertyField(settings.FindProperty("traceDevelopmentOutputToUnityConsole"), s_developmentConsoleOutput, SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // EditorGUILayout.EndHorizontal();
            //
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label("Debug", SettingsLayoutOptions.FixedWidth130LayoutOptions);
            // GUILayout.Space(10);
            // SerializedProperty debugLevelsProperty = settings.FindProperty("traceDebugLevels");
            // EditorGUI.BeginChangeCheck();
            // ushort newDebugLevels = (ushort)EditorGUILayout.MaskField(s_debugLevelsContent, debugLevelsProperty.intValue,
            //     debugLevelsProperty.enumDisplayNames, SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     debugLevelsProperty.intValue = newDebugLevels;
            // }
            // GUILayout.Space(10);
            // EditorGUILayout.PropertyField(settings.FindProperty("traceDebugOutputToUnityConsole"),
            //     s_debugConsoleOutput, SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // EditorGUILayout.EndHorizontal();
            //
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.Label("Release", SettingsLayoutOptions.FixedWidth130LayoutOptions);
            // GUILayout.Space(10);
            // SerializedProperty releaseLevelsProperty = settings.FindProperty("traceReleaseLevels");
            // EditorGUI.BeginChangeCheck();
            // ushort newReleaseLevels = (ushort)EditorGUILayout.MaskField(s_releaseLevelsContent, releaseLevelsProperty.intValue,
            //     releaseLevelsProperty.enumDisplayNames,SettingsLayoutOptions.FixedWidth150LayoutOptions);
            // if (EditorGUI.EndChangeCheck())
            // {
            //     releaseLevelsProperty.intValue = newReleaseLevels;
            // }
            // EditorGUILayout.EndHorizontal();
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public int GetPriority()
        {
            return 700;
        }
        public string GetSectionHeaderLabel()
        {
            return "Environment";
        }
        public string GetSectionID()
        {
            return "GDX.Environment";
        }
        public string GetSectionHelpLink()
        {
            return null;
        }
        public bool GetToggleSupport()
        {
            return false;
        }

        public bool GetToggleState()
        {
            return false;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {

        }

        public void UpdateSectionContent()
        {

        }
    }
}