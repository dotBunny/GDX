// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    internal class CommandLineProcessorSettings : IConfigSection
    {
        private VisualElement _rootElement;

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new CommandLineProcessorSettings());
        }

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            TextField textArgumentPrefix = _rootElement.Q<TextField>("text-argument-prefix");
            textArgumentPrefix.value = Core.Config.developerCommandLineParserArgumentPrefix;
            textArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                Core.Config.developerCommandLineParserArgumentPrefix = evt.newValue;
                Core.ConfigDirty = true;
            });

            TextField textArgumentSplit = _rootElement.Q<TextField>("text-argument-split");
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

        public string GetSectionHelpLink()
        {
            return "api/GDX.Developer.CommandLineParser.html";
        }

        public string GetSectionID()
        {
            return "GDX.Developer.CommandLineParser";
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsCommandLineProcessor";
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

        /// <inheritdoc />
        public void UpdateSectionContent()
        {
            TextField textArgumentPrefix = _rootElement.Q<TextField>("text-argument-prefix");
            textArgumentPrefix.SetValueWithoutNotify(Core.Config.developerCommandLineParserArgumentPrefix);

            TextField textArgumentSplit = _rootElement.Q<TextField>("text-argument-split");
            textArgumentSplit.SetValueWithoutNotify(Core.Config.developerCommandLineParserArgumentSplit);
        }
    }
}