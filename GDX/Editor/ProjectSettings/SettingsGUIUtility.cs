// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     A collection of IMGUI based layout methods used by the settings window.
    /// </summary>
    public static class SettingsGUIUtility
    {
        /// <summary>
        ///     A cache of boolean values backed by <see cref="EditorPrefs" /> to assist with optimizing layout.
        /// </summary>
        private static readonly Dictionary<string, bool> s_cachedEditorPreferences = new Dictionary<string, bool>();

        /// <summary>
        ///     Draw a section header useful for project settings.
        /// </summary>
        /// <param name="id">Identifier used for editor preferences to determine if the section is collapsed or not.</param>
        /// <param name="defaultVisibility">Should this sections content be visible by default?</param>
        /// <param name="text">The section header content.</param>
        /// <param name="helpUri">The destination of the help button, if present.</param>
        /// <param name="sectionToggleProperty">
        ///     A <see cref="UnityEditor.SerializedProperty" /> which will dictate if a section is enabled or not.
        /// </param>
        /// <param name="sectionToggleContent">The <see cref="UnityEngine.GUIContent" /> associated with a setting.</param>
        /// <returns>true/false if the sections content should be enabled.</returns>
        public static bool CreateSettingsSection(string id, bool defaultVisibility, string text,
            string helpUri = null,
            SerializedProperty sectionToggleProperty = null,
            GUIContent sectionToggleContent = null)
        {
            Color previousColor = GUI.backgroundColor;

            if (sectionToggleProperty == null)
            {
                GUI.backgroundColor = SettingsStyles.DefaultBlueColor;
                GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                bool setting = GetCachedEditorBoolean(id, defaultVisibility);
                if (!setting)
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, true);
                    }
                }
                else
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, false);
                    }
                }

                GUILayout.Label(text, SettingsStyles.SectionHeaderTextDefaultStyle);
                if (!string.IsNullOrEmpty(helpUri))
                {
                    if (GUILayout.Button(SettingsStyles.HelpIcon, SettingsStyles.HelpButtonStyle))
                    {
                        GUIUtility.hotControl = 0;
                        Application.OpenURL(helpUri);
                    }
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUI.backgroundColor = previousColor;
                GUILayout.Space(5);
                return true;
            }

            if (sectionToggleProperty.boolValue)
            {
                GUI.backgroundColor = SettingsStyles.EnabledGreenColor;
                GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                bool setting = GetCachedEditorBoolean(id);
                if (!setting)
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, true);
                    }
                }
                else
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, false);
                    }
                }

                GUILayout.Label(text, SettingsStyles.SectionHeaderTextDefaultStyle);
                if (!string.IsNullOrEmpty(helpUri))
                {
                    if (GUILayout.Button(SettingsStyles.HelpIcon, SettingsStyles.HelpButtonStyle))
                    {
                        GUIUtility.hotControl = 0;
                        Application.OpenURL(helpUri);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                    SettingsStyles.SectionHeaderToggleLayoutOptions);
            }
            else
            {
                GUI.backgroundColor = SettingsStyles.DisabledYellowColor;
                GUILayout.BeginHorizontal(SettingsStyles.SectionHeaderStyle);
                bool setting = GetCachedEditorBoolean(id);
                if (!setting)
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.PlusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, true);
                    }
                }
                else
                {
                    // ReSharper disable once InvertIf
                    if (GUILayout.Button(SettingsStyles.MinusIcon, SettingsStyles.SectionHeaderExpandButtonStyle,
                        SettingsStyles.SectionHeaderExpandLayoutOptions))
                    {
                        GUIUtility.hotControl = 0;
                        SetCachedEditorBoolean(id, false);
                    }
                }


                GUILayout.Label(text, SettingsStyles.SectionHeaderTextDisabledStyle);
                if (!string.IsNullOrEmpty(helpUri))
                {
                    if (GUILayout.Button(SettingsStyles.HelpIcon, SettingsStyles.HelpButtonStyle))
                    {
                        GUIUtility.hotControl = 0;
                        Application.OpenURL(helpUri);
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                    SettingsStyles.SectionHeaderToggleLayoutOptions);
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = previousColor;
            GUILayout.Space(5);
            return sectionToggleProperty.boolValue;
        }

        /// <summary>
        ///     Get a cached value or fill it from <see cref="EditorPrefs" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="defaultValue">If no value is found, what should be used.</param>
        /// <returns></returns>
        public static bool GetCachedEditorBoolean(string id, bool defaultValue = true)
        {
            if (!s_cachedEditorPreferences.ContainsKey(id))
            {
                s_cachedEditorPreferences[id] = EditorPrefs.GetBool(id, defaultValue);
            }

            return s_cachedEditorPreferences[id];
        }

        /// <summary>
        ///     Sets the cached value (and <see cref="EditorPrefs" />) for the <paramref name="id" />.
        /// </summary>
        /// <param name="id">Identifier for the <see cref="bool" /> value.</param>
        /// <param name="setValue">The desired value to set.</param>
        public static void SetCachedEditorBoolean(string id, bool setValue)
        {
            if (!s_cachedEditorPreferences.ContainsKey(id))
            {
                s_cachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
            else
            {
                if (s_cachedEditorPreferences[id] == setValue)
                {
                    return;
                }

                s_cachedEditorPreferences[id] = setValue;
                EditorPrefs.SetBool(id, setValue);
            }
        }
    }
}