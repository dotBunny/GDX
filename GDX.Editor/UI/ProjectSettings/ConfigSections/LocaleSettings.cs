// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine.UIElements;
#if UNITY_2020_3
using UnityEditor.UIElements;
#endif


namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    internal class LocaleSettings : IConfigSection
    {
        private VisualElement _rootElement;
        private Toggle _toggleSetDefaultCulture;
        private EnumField _enumDefaultCulture;

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new LocaleSettings());
        }

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            _toggleSetDefaultCulture = _rootElement.Q<Toggle>("toggle-set-default-culture");
            _toggleSetDefaultCulture.value = UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture;
            _toggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture = evt.newValue;
                if (Core.Config.localizationSetDefaultCulture != evt.newValue)
                {
                    _toggleSetDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _toggleSetDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            _enumDefaultCulture = _rootElement.Q<EnumField>("enum-default-culture");
            _enumDefaultCulture.value = UI.SettingsProvider.WorkingConfig.localizationDefaultCulture;
            _enumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.localizationDefaultCulture = (Localization.Language)evt.newValue;
                if (Core.Config.localizationDefaultCulture != (Localization.Language)evt.newValue)
                {
                    _enumDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _enumDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public int GetPriority()
        {
            return 600;
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
            return "GDX.Localization";
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
            ConfigSectionsProvider.SetStructChangeCheck(_toggleSetDefaultCulture,
                Core.Config.localizationSetDefaultCulture,
                UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture);
            ConfigSectionsProvider.SetEnumChangeCheck(_enumDefaultCulture,
                Core.Config.localizationDefaultCulture,
                UI.SettingsProvider.WorkingConfig.localizationDefaultCulture);
        }
    }
}