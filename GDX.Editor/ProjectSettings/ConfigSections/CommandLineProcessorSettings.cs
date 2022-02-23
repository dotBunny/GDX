// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    class CommandLineProcessorSettings : IConfigSection
    {
        public const string SectionKey = "GDX.Developer.CommandLineParser";
        VisualElement m_RootElement;
        TextField m_TextArgumentPrefix;
        TextField m_TextArgumentSplit;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextArgumentPrefix = m_RootElement.Q<TextField>("text-argument-prefix");
            m_TextArgumentPrefix.value = SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix;
            m_TextArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextArgumentPrefix.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentPrefix.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });


            m_TextArgumentSplit = m_RootElement.Q<TextField>("text-argument-split");
            m_TextArgumentSplit.value = SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit;
            m_TextArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentSplit != evt.newValue)
                {
                    m_TextArgumentSplit.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentSplit.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
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

        public string GetSectionKey()
        {
            return SectionKey;
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
                SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextArgumentSplit,
                Core.Config.DeveloperCommandLineParserArgumentSplit,
                SettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit);
        }
    }
}