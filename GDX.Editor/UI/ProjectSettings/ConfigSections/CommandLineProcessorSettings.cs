// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class CommandLineProcessorSettings : IConfigSection
    {
        public const string SectionID = "GDX.Developer.CommandLineParser";
        private VisualElement m_RootElement;
        private TextField m_TextArgumentPrefix;
        private TextField m_TextArgumentSplit;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextArgumentPrefix = m_RootElement.Q<TextField>("text-argument-prefix");
            m_TextArgumentPrefix.value = UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix;
            m_TextArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextArgumentPrefix.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentPrefix.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });


            m_TextArgumentSplit = m_RootElement.Q<TextField>("text-argument-split");
            m_TextArgumentSplit.value = UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit;
            m_TextArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentSplit != evt.newValue)
                {
                    m_TextArgumentSplit.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentSplit.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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
            return SectionID;
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
            ConfigSectionsProvider.SetClassChangeCheck(m_TextArgumentPrefix,
                Core.Config.DeveloperCommandLineParserArgumentPrefix,
                UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextArgumentSplit,
                Core.Config.DeveloperCommandLineParserArgumentSplit,
                UI.SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit);
        }
    }
}