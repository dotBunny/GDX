// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Locale Settings
    /// </summary>
    internal static class LocaleSettings
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.Localization";

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.localizationDefaultCulture" />.
        /// </summary>
        private static readonly GUIContent s_defaultCultureContent = new GUIContent(
            "Default Culture",
            "The value to be used when setting the CultureInfo.DefaultThreadCurrentCulture.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.localizationSetDefaultCulture" />.
        /// </summary>
        private static readonly GUIContent s_setDefaultCultureContent = new GUIContent(
            "Set Default Culture",
            "For situations where a culture does not have a calendar available, this can be corrected after assemblies have been loaded by setting the CultureInfo.DefaultThreadCurrentCulture to a setting with one.");

        /// <summary>
        ///     Draw the Localization section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
        internal static void Draw(SerializedObject settings)
        {
            GUI.enabled = true;

            SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Localization",
                $"{SettingsProvider.DocumentationUri}api/GDX.Localization.html");

            if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            {
                return;
            }

            EditorGUILayout.PropertyField(settings.FindProperty("localizationSetDefaultCulture"),
                s_setDefaultCultureContent);

            EditorGUILayout.PropertyField(settings.FindProperty("localizationDefaultCulture"),
                s_defaultCultureContent);
        }
    }
}