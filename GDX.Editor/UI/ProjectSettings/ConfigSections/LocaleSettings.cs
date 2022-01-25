// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI;
using UnityEditor;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    internal class LocaleSettings : IConfigSection
    {
        private VisualElement _element;

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new LocaleSettings());
        }

        /// <summary>
        ///     Draw the Localization section of settings.
        /// </summary>
        /// <param name="rootElement"></param>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        public void BindSectionContent(VisualElement rootElement, GDXConfig settings)
        {
            Toggle toggleSetDefaultCulture = rootElement.Q<Toggle>("toggle-set-default-culture");
            EnumField enumDefaultCulture= rootElement.Q<EnumField>("enum-default-culture");
        }
        public string GetTemplateName()
        {
            return "GDXProjectSettingsLocale";
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public string GetSectionHeaderLabel()
        {
            return "Localization";
        }
        public string GetSectionID()
        {
            return "GDX.Localization";
        }
        public string GetSectionHelpLink()
        {
            return "api/GDX.Localization.html";
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
        public void UpdateSectionContent(GDXConfig config)
        {

        }
    }
}