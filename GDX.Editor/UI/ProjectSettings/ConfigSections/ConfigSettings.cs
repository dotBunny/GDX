// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///    GDX Config Settings
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class ConfigSettings : IConfigSection
    {
        public const string SectionID = "GDX.Config";
        private VisualElement m_RootElement;
        private TextField m_TextOutputFolder;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextOutputFolder = m_RootElement.Q<TextField>("text-output-path");
            
            m_TextOutputFolder.SetValueWithoutNotify(Core.Config.ConfigOutputPath);
            m_TextOutputFolder.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.ConfigOutputPath = evt.newValue;
                if (Core.Config.ConfigOutputPath != evt.newValue)
                {
                    m_TextOutputFolder.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextOutputFolder.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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
            return "Configuration Override";
        }

        public string GetSectionHelpLink()
        {
            return null;
        }

        public string GetSectionID()
        {
            return SectionID;
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
            ConfigSectionsProvider.SetClassChangeCheck(m_TextOutputFolder, Core.Config.ConfigOutputPath,
                UI.SettingsProvider.WorkingConfig.ConfigOutputPath);
        }
    }
}