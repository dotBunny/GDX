// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    internal class LocaleSettings : IConfigSection
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.Localization";

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.localizationDefaultCulture" />.
        /// </summary>
        private readonly GUIContent s_defaultCultureContent = new GUIContent(
            "Default Culture",
            "The value to be used when setting the CultureInfo.DefaultThreadCurrentCulture.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.localizationSetDefaultCulture" />.
        /// </summary>
        private readonly GUIContent s_setDefaultCultureContent = new GUIContent(
            "Set Default Culture",
            "For situations where a culture does not have a calendar available, this can be corrected after assemblies have been loaded by setting the CultureInfo.DefaultThreadCurrentCulture to a setting with one.");

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new LocaleSettings());
        }

        /// <summary>
        ///     Draw the Localization section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        public void DrawSectionContent(GDXConfig settings)
        {
            // GUI.enabled = true;
            //
            // SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Localization",
            //     $"{SettingsProvider.DocumentationUri}api/GDX.Localization.html");
            //
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return false;
            // }
            //
            // EditorGUI.BeginChangeCheck();
            //
            // settings.localizationSetDefaultCulture =
            //     EditorGUILayout.Toggle(s_setDefaultCultureContent, settings.localizationSetDefaultCulture);
            //
            // settings.localizationDefaultCulture =
            //     (GDX.Localization.Language)EditorGUILayout.EnumPopup(s_defaultCultureContent, settings.localizationDefaultCulture);
            //
            // return EditorGUI.EndChangeCheck();
        }

        public void DrawSectionHeader(GDXConfig config)
        {

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
    }
}