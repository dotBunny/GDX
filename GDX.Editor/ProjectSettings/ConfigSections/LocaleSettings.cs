// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif


namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    class LocaleSettings : IConfigSection
    {
        public const string SectionKey = "GDX.Localization";
        static readonly string[] k_Keywords = { "locale", "loc", "localization" };
        VisualElement m_RootElement;
        Toggle m_ToggleSetDefaultCulture;
        EnumField m_EnumDefaultCulture;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_ToggleSetDefaultCulture = m_RootElement.Q<Toggle>("toggle-set-default-culture");
            SearchProvider.RegisterElement<Toggle>(this, m_ToggleSetDefaultCulture);
            m_ToggleSetDefaultCulture.value = SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture;
            m_ToggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture = evt.newValue;
                if (Config.LocalizationSetDefaultCulture != evt.newValue)
                {
                    m_ToggleSetDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleSetDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_EnumDefaultCulture = m_RootElement.Q<EnumField>("enum-default-culture");
            SearchProvider.RegisterElement<EnumField>(this, m_EnumDefaultCulture);
            m_EnumDefaultCulture.value = SettingsProvider.WorkingConfig.LocalizationDefaultCulture;
            m_EnumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.LocalizationDefaultCulture = (Localization.Language)evt.newValue;
                if (Config.LocalizationDefaultCulture != (Localization.Language)evt.newValue)
                {
                    m_EnumDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_EnumDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetSectionHeaderLabel()
        {
            return "Localization";
        }

        public string GetSectionHelpLink()
        {
            return "api/GDX.Localization.html";
        }

        public string GetSectionKey()
        {
            return SectionKey;
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsLocale";
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
            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleSetDefaultCulture,
                Config.LocalizationSetDefaultCulture,
                SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture);
            ConfigSectionsProvider.SetEnumChangeCheck(m_EnumDefaultCulture,
                Config.LocalizationDefaultCulture,
                SettingsProvider.WorkingConfig.LocalizationDefaultCulture);
        }
    }
}