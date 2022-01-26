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
            textArgumentPrefix.value = UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix;
            textArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix = evt.newValue;
                if (Core.Config.developerCommandLineParserArgumentPrefix != evt.newValue)
                {
                    textArgumentPrefix.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    textArgumentPrefix.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            TextField textArgumentSplit = _rootElement.Q<TextField>("text-argument-split");
            textArgumentSplit.value = UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit;
            textArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit = evt.newValue;
                if (Core.Config.developerCommandLineParserArgumentSplit != evt.newValue)
                {
                    textArgumentSplit.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    textArgumentSplit.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
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

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {

        }

        /// <inheritdoc />
        public void UpdateSectionContent()
        {
            TextField textArgumentPrefix = _rootElement.Q<TextField>("text-argument-prefix");
            textArgumentPrefix.SetValueWithoutNotify(UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix);
            if (Core.Config.developerCommandLineParserArgumentPrefix != UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentPrefix)
            {
                textArgumentPrefix.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                textArgumentPrefix.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }

            TextField textArgumentSplit = _rootElement.Q<TextField>("text-argument-split");
            textArgumentSplit.SetValueWithoutNotify(UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit);
            if (Core.Config.developerCommandLineParserArgumentSplit != UI.SettingsProvider.WorkingConfig.developerCommandLineParserArgumentSplit)
            {
                textArgumentSplit.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                textArgumentSplit.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }
        }
    }
}