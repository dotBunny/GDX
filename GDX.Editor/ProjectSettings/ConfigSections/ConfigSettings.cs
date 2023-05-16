// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///    GDX Config Settings
    /// </summary>
    class ConfigSettings : IConfigSection
    {
        public const int SectionIndex = 3;
        public const string SectionKey = "GDX.Config";
        static readonly string[] k_Keywords = { "config", "override" };
        VisualElement m_RootElement;
        TextField m_TextOutputFolder;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextOutputFolder = m_RootElement.Q<TextField>("text-output-path");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextOutputFolder);

            m_TextOutputFolder.SetValueWithoutNotify(Config.ConfigOutputPath);
            m_TextOutputFolder.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.ConfigOutputPath = evt.newValue;
                if (Config.ConfigOutputPath != evt.newValue)
                {
                    m_TextOutputFolder.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextOutputFolder.RemoveFromClassList(ResourcesProvider.ChangedClass);
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
            return "Configuration Override";
        }

        public string GetSectionHelpLink()
        {
            return null;
        }

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
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
            return "GDXProjectSettingsConfig";
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
            ProjectSettingsProvider.SetClassChangeCheck(m_TextOutputFolder, Config.ConfigOutputPath,
                ProjectSettingsProvider.WorkingConfig.ConfigOutputPath);
        }
    }
}