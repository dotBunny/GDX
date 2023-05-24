// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

#if !UNITY_2022_1_OR_NEWER
using UnityEditor.UIElements;
#endif


namespace GDX.Editor.Windows.ProjectSettings.ConfigSections
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    class LocaleSettings : IConfigSection
    {
        public const int SectionIndex = 5;
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
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleSetDefaultCulture);
            m_ToggleSetDefaultCulture.value = ProjectSettingsProvider.WorkingConfig.LocalizationSetDefaultCulture;
            m_ToggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.LocalizationSetDefaultCulture = evt.newValue;
                if (Config.LocalizationSetDefaultCulture != evt.newValue)
                {
                    m_ToggleSetDefaultCulture.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleSetDefaultCulture.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_EnumDefaultCulture = m_RootElement.Q<EnumField>("enum-default-culture");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_EnumDefaultCulture);
            m_EnumDefaultCulture.value = ProjectSettingsProvider.WorkingConfig.LocalizationDefaultCulture;
            m_EnumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.LocalizationDefaultCulture = (Localization.Language)evt.newValue;
                if (Config.LocalizationDefaultCulture != (Localization.Language)evt.newValue)
                {
                    m_EnumDefaultCulture.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_EnumDefaultCulture.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
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
            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleSetDefaultCulture,
                Config.LocalizationSetDefaultCulture,
                ProjectSettingsProvider.WorkingConfig.LocalizationSetDefaultCulture);
            ProjectSettingsProvider.SetEnumChangeCheck(m_EnumDefaultCulture,
                Config.LocalizationDefaultCulture,
                ProjectSettingsProvider.WorkingConfig.LocalizationDefaultCulture);
        }
    }
}