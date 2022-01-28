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

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new LocaleSettings());
        }

        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            Toggle toggleSetDefaultCulture = _rootElement.Q<Toggle>("toggle-set-default-culture");
            toggleSetDefaultCulture.value = UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture;
            toggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture = evt.newValue;
                if (Core.Config.localizationSetDefaultCulture != evt.newValue)
                {
                    toggleSetDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    toggleSetDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            EnumField enumDefaultCulture= _rootElement.Q<EnumField>("enum-default-culture");
            enumDefaultCulture.value = UI.SettingsProvider.WorkingConfig.localizationDefaultCulture;
            enumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.localizationDefaultCulture = (Localization.Language)evt.newValue;
                if (Core.Config.localizationDefaultCulture != (Localization.Language)evt.newValue)
                {
                    enumDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    enumDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {

        }

        /// <inheritdoc />
        public void UpdateSectionContent()
        {
            Toggle toggleSetDefaultCulture = _rootElement.Q<Toggle>("toggle-set-default-culture");
            toggleSetDefaultCulture.SetValueWithoutNotify(UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture);
            if (Core.Config.localizationSetDefaultCulture != UI.SettingsProvider.WorkingConfig.localizationSetDefaultCulture)
            {
                toggleSetDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleSetDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }

            EnumField enumDefaultCulture= _rootElement.Q<EnumField>("enum-default-culture");
            enumDefaultCulture.SetValueWithoutNotify(UI.SettingsProvider.WorkingConfig.localizationDefaultCulture);
            if (Core.Config.localizationDefaultCulture != UI.SettingsProvider.WorkingConfig.localizationDefaultCulture)
            {
                enumDefaultCulture.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                enumDefaultCulture.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }
        }
    }
}