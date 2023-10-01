// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings.ConfigSections
{
    /// <summary>
    ///     Command Line Processor Settings
    /// </summary>
    class CommandLineProcessorSettings : IConfigSection
    {
        public const int SectionIndex = 2;
        public const string SectionKey = "GDX.Developer.CommandLineParser";
        static readonly string[] k_Keywords = { "cli", "arguments", "argument", "args" };
        VisualElement m_RootElement;
        TextField m_TextArgumentPrefix;
        TextField m_TextArgumentSplit;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextArgumentPrefix = m_RootElement.Q<TextField>("text-argument-prefix");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextArgumentPrefix);
            m_TextArgumentPrefix.value = ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentPrefix;
            m_TextArgumentPrefix.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentPrefix = evt.newValue;
                if (Config.CommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextArgumentPrefix.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentPrefix.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });


            m_TextArgumentSplit = m_RootElement.Q<TextField>("text-argument-split");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextArgumentSplit);
            m_TextArgumentSplit.value = ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentSplit;
            m_TextArgumentSplit.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentSplit = evt.newValue;
                if (Config.CommandLineParserArgumentSplit != evt.newValue)
                {
                    m_TextArgumentSplit.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextArgumentSplit.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
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

        public int GetSectionIndex()
        {
            return SectionIndex;
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
            ProjectSettingsProvider.SetClassChangeCheck(m_TextArgumentPrefix,
                Config.CommandLineParserArgumentPrefix,
                ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentPrefix);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextArgumentSplit,
                Config.CommandLineParserArgumentSplit,
                ProjectSettingsProvider.WorkingConfig.CommandLineParserArgumentSplit);
        }
    }
}