// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEngine.UIElements;

#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif


namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class LocaleSettings : IConfigSection
    {
        public const string SectionID = "GDX.Localization";
        private VisualElement m_RootElement;
        private Toggle m_ToggleSetDefaultCulture;
        private EnumField m_EnumDefaultCulture;

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_ToggleSetDefaultCulture = m_RootElement.Q<Toggle>("toggle-set-default-culture");
            m_ToggleSetDefaultCulture.value = UI.SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture;
            m_ToggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture = evt.newValue;
                if (Core.Config.LocalizationSetDefaultCulture != evt.newValue)
                {
                    m_ToggleSetDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleSetDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_EnumDefaultCulture = m_RootElement.Q<EnumField>("enum-default-culture");
            m_EnumDefaultCulture.value = UI.SettingsProvider.WorkingConfig.LocalizationDefaultCulture;
            m_EnumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.LocalizationDefaultCulture = (Localization.Language)evt.newValue;
                if (Core.Config.LocalizationDefaultCulture != (Localization.Language)evt.newValue)
                {
                    m_EnumDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_EnumDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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
            return "Localization";
        }

        public string GetSectionHelpLink()
        {
            return "api/GDX.Localization.html";
        }

        public string GetSectionID()
        {
            return SectionID;
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
                Core.Config.LocalizationSetDefaultCulture,
                UI.SettingsProvider.WorkingConfig.LocalizationSetDefaultCulture);
            ConfigSectionsProvider.SetEnumChangeCheck(m_EnumDefaultCulture,
                Core.Config.LocalizationDefaultCulture,
                UI.SettingsProvider.WorkingConfig.LocalizationDefaultCulture);
        }
    }
}