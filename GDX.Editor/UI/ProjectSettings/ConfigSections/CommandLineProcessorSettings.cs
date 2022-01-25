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
    ///     Command Line Processor Settings
    /// </summary>
    internal class CommandLineProcessorSettings : IConfigSection
    {
        private VisualElement _element;

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new CommandLineProcessorSettings());
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsCommandLineProcessor";
        }

        /// <summary>
        ///     Draw the Command Line Processor section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="Config" /> object to be modified.</param>
        public void BindSectionContent(VisualElement rootElement, GDXConfig settings)
        {
            TextField textArgumentPrefix = rootElement.Q<TextField>("text-argument-prefix");
            TextField textArgumentSplit= rootElement.Q<TextField>("text-argument-split");

            // GUI.enabled = true;
            //
            // SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Command Line Parser",
            //     $"{SettingsProvider.DocumentationUri}api/GDX.Developer.CommandLineParser.html");
            //
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return;
            // }
            //
            // EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
            //     s_argumentPrefixContent);
            // EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
            //     s_argumentSplitContent);
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public string GetSectionHeaderLabel()
        {
            return "Command Line Parser";
        }
        public string GetSectionID()
        {
            return "GDX.Developer.CommandLineParser";
        }
        public string GetSectionHelpLink()
        {
            return "api/GDX.Developer.CommandLineParser.html";
        }
        public bool GetToggleSupport()
        {
            return false;
        }
        public bool GetToggleState()
        {
            return false;
        }

        public void SetToggleState(bool newState)
        {

        }

        public void UpdateSectionContent(GDXConfig config)
        {

        }
    }
}