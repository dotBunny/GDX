// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine.UIElements;

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
            toggleSetDefaultCulture.value = Core.Config.localizationSetDefaultCulture;
            toggleSetDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                Core.Config.localizationSetDefaultCulture = evt.newValue;
                Core.ConfigDirty = true;
            });

            EnumField enumDefaultCulture= _rootElement.Q<EnumField>("enum-default-culture");
            enumDefaultCulture.value = Core.Config.localizationDefaultCulture;
            enumDefaultCulture.RegisterValueChangedCallback(evt =>
            {
                Core.Config.localizationDefaultCulture = (Localization.Language)evt.newValue;
                Core.ConfigDirty = true;
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

        public void SetToggleState(bool newState)
        {

        }

        /// <inheritdoc />
        public void UpdateSectionContent()
        {
            Toggle toggleSetDefaultCulture = _rootElement.Q<Toggle>("toggle-set-default-culture");
            toggleSetDefaultCulture.SetValueWithoutNotify(Core.Config.localizationSetDefaultCulture);

            EnumField enumDefaultCulture= _rootElement.Q<EnumField>("enum-default-culture");
            enumDefaultCulture.SetValueWithoutNotify(Core.Config.localizationDefaultCulture);
        }
    }
}