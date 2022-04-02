// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///    GDX Config Settings
    /// </summary>
    class PlatformSettings : IConfigSection
    {
        public const string SectionKey = "GDX.Platform";
        static readonly string[] k_Keywords = { "platform", "automation", "cache" };
        VisualElement m_RootElement;
        TextField m_TextAutomationFolder;
        TextField m_TextCacheFolder;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TextAutomationFolder = m_RootElement.Q<TextField>("text-automation-folder");
            m_TextAutomationFolder.SetValueWithoutNotify(GDXConfig.PlatformAutomationFolder);
            m_TextAutomationFolder.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.PlatformAutomationFolder = evt.newValue;
                if (GDXConfig.PlatformAutomationFolder != evt.newValue)
                {
                    m_TextAutomationFolder.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextAutomationFolder.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_TextCacheFolder = m_RootElement.Q<TextField>("text-cache-folder");
            m_TextCacheFolder.SetValueWithoutNotify(GDXConfig.PlatformCacheFolder);
            m_TextCacheFolder.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.PlatformCacheFolder = evt.newValue;
                if (GDXConfig.PlatformCacheFolder != evt.newValue)
                {
                    m_TextCacheFolder.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextCacheFolder.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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
            return "Platform";
        }

        public string GetSectionHelpLink()
        {
            return null;
        }

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetSectionKey()
        {
            return SectionKey;
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsPlatform";
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
            ConfigSectionsProvider.SetClassChangeCheck(m_TextAutomationFolder, GDXConfig.PlatformAutomationFolder,
                SettingsProvider.WorkingTransientConfig.PlatformAutomationFolder);
            ConfigSectionsProvider.SetClassChangeCheck(m_TextCacheFolder, GDXConfig.PlatformCacheFolder,
                SettingsProvider.WorkingTransientConfig.PlatformCacheFolder);
        }
    }
}