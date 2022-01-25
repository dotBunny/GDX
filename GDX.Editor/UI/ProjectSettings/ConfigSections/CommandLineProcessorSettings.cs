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

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement, GDXConfig settings)
        {
            TextField textArgumentPrefix = rootElement.Q<TextField>("text-argument-prefix");
            textArgumentPrefix.value = Core.Config.developerCommandLineParserArgumentPrefix;
            textArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                Core.Config.developerCommandLineParserArgumentPrefix = evt.newValue;
                Core.ConfigDirty = true;
            });

            TextField textArgumentSplit = rootElement.Q<TextField>("text-argument-split");
            textArgumentSplit.value = Core.Config.developerCommandLineParserArgumentSplit;
            textArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                Core.Config.developerCommandLineParserArgumentSplit = evt.newValue;
                Core.ConfigDirty = true;
            });
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