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
        static readonly string[] k_Keywords = { "cli", "arguments", "argument" };
        VisualElement m_RootElement;
        TextField m_TextArgumentPrefix;
        TextField m_TextArgumentSplit;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextArgumentPrefix = m_RootElement.Q<TextField>("text-argument-prefix");
            SearchProvider.RegisterElement<TextField>(this, m_TextArgumentPrefix);
            m_TextArgumentPrefix.value = ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix;
            m_TextArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix = evt.newValue;
                if (Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextArgumentPrefix.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentPrefix.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.CheckForChanges();
            });


            m_TextArgumentSplit = m_RootElement.Q<TextField>("text-argument-split");
            SearchProvider.RegisterElement<TextField>(this, m_TextArgumentSplit);
            m_TextArgumentSplit.value = ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit;
            m_TextArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit = evt.newValue;
                if (Config.DeveloperCommandLineParserArgumentSplit != evt.newValue)
                {
                    m_TextArgumentSplit.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentSplit.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.CheckForChanges();
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

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
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
                Config.DeveloperCommandLineParserArgumentPrefix,
                ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentPrefix);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextArgumentSplit,
                Config.DeveloperCommandLineParserArgumentSplit,
                ProjectSettingsProvider.WorkingConfig.DeveloperCommandLineParserArgumentSplit);
        }
    }
}