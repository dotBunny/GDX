// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class CommandLineProcessorSettings : IConfigSection
    {
        private VisualElement _rootElement;
        private TextField _textArgumentPrefix;
        private TextField _textArgumentSplit;

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new CommandLineProcessorSettings());
        }

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            _textArgumentPrefix = _rootElement.Q<TextField>("text-argument-prefix");
            _textArgumentPrefix.value = UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix;
            _textArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix = evt.newValue;
                if (Core.Config.developerCommandLineParserArgumentPrefix != evt.newValue)
                {
                    _textArgumentPrefix.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _textArgumentPrefix.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });


            _textArgumentSplit = _rootElement.Q<TextField>("text-argument-split");
            _textArgumentSplit.value = UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit;
            _textArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit = evt.newValue;
                if (Core.Config.developerCommandLineParserArgumentSplit != evt.newValue)
                {
                    _textArgumentSplit.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _textArgumentSplit.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

        }

        public bool GetDefaultVisibility()
        {
            return false;
        }

        public int GetPriority()
        {
            return 800;
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

        public string GetToggleTooltip()
        {
            return null;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {

        }

        /// <inheritdoc />
        public void UpdateSectionContent()
        {
            ConfigSectionsProvider.SetClassChangeCheck(_textArgumentPrefix,
                Core.Config.developerCommandLineParserArgumentPrefix,
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix);

            ConfigSectionsProvider.SetClassChangeCheck(_textArgumentSplit,
                Core.Config.developerCommandLineParserArgumentSplit,
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit);
        }
    }
}