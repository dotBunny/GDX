// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if GDX_VISUALSCRIPTING
using Unity.VisualScripting;

#endif

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Visual Scripting Settings
    /// </summary>
    internal static class VisualScriptingSettings
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.VisualScripting";

        /// <summary>
        ///     Information regarding the assembly
        /// </summary>
        private static AssemblyProvider s_assembly;

        /// <summary>
        ///     Content regarding this not being all of the content that can be added to visual scripting.
        /// </summary>
        private static readonly GUIContent s_visualScriptingGenerationAdditionalContent = new GUIContent(
            "These categories represents a curated selection of predefined content from the GDX API which has proven useful; it is not meant to represent everything available for inclusion to Visual Scripting.");

        /// <summary>
        ///     Content for instruction on how to regenerate quickly.
        /// </summary>
        private static readonly GUIContent s_visualScriptingGenerationContent = new GUIContent(
            "You can do this by clicking the Regenerate Units button to the right or by finding the same button available in the Visual Scripting section of the Project Settings.");

        /// <summary>
        ///     Content for the warning about changing types units.
        /// </summary>
        private static readonly GUIContent s_visualScriptingGenerationNoticeContent =
            new GUIContent(
                "By selecting and deselecting the below categories, items will be added or removed from the Visual Scripting type options configuration.");

        /// <summary>
        ///     Content for the warning about regenerating units.
        /// </summary>
        private static readonly GUIContent s_visualScriptingGenerationWarningContent =
            new GUIContent(
                "You still NEED to Regenerate Units!");

        /// <summary>
        ///     Content for the notice when visual scripting needs to regenerate its nodes.
        /// </summary>
        private static readonly GUIContent s_visualScriptingLoadingContent = new GUIContent(
            "The visual scripting subsystem is currently loading.");

        /// <summary>
        ///     Draw the Build Info section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
#if GDX_VISUALSCRIPTING
        public static void Draw(SerializedObject settings)
        {
            GUI.enabled = true;


            SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Visual Scripting",
                $"{SettingsProvider.DocumentationUri}manual/features/visual-scripting.html");

            // Don't show section if not enabled OR if the visual scripting has not been initialized
            if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            {
                return;
            }

            if (!ProductContainer.initialized)
            {
                GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
                GUILayout.Label(s_visualScriptingLoadingContent);
                GUILayout.EndVertical();
                return;
            }

            if (s_assembly == null)
            {
                s_assembly = new AssemblyProvider();
            }

            GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
            GUILayout.BeginHorizontal();

            GUILayout.Label(SettingsStyles.WarningIcon, SettingsStyles.NoHorizontalStretchStyle);
            GUILayout.BeginVertical();
            GUILayout.Label(s_visualScriptingGenerationNoticeContent, SettingsStyles.WordWrappedLabelStyle);
            GUILayout.Space(5);
            GUILayout.Label(s_visualScriptingGenerationWarningContent, EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.Label(s_visualScriptingGenerationContent, SettingsStyles.WordWrappedLabelStyle);
            GUILayout.Space(5);
            GUILayout.Label(s_visualScriptingGenerationAdditionalContent, SettingsStyles.WordWrappedLabelStyle);
            GUILayout.EndVertical();
            GUILayout.Space(10);
            if (GUILayout.Button("Regenerate Units", SettingsStyles.ButtonStyle))
            {
                BoltCore.Configuration.Save();
                UnitBase.Rebuild();
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();


            DrawNodeSection("Extensions", s_assembly.VisualScriptingExtensions);

            GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));

            DrawNodeSection("Types", s_assembly.VisualScriptingTypes);

            GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));

            DrawNodeSection("Utilities", s_assembly.VisualScriptingUtilities);
        }
#endif

        private static void DrawNodeSection(string section, List<Type> types)
        {
            GUILayout.BeginVertical(SettingsStyles.TableRowStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label(section, EditorStyles.boldLabel, SettingsStyles.FixedWidth130LayoutOptions);

            GUILayout.BeginVertical();
            foreach (Type type in types)
            {
#if UNITY_2021_1_OR_NEWER
                if (EditorGUILayout.LinkButton(type.ToString()))
#elif UNITY_2019_1_OR_NEWER
                if (GUILayout.Button(type.ToString(), EditorStyles.linkLabel))
#else
                if (GUILayout.Button(type.ToString(), EditorStyles.boldLabel))
#endif
                {
                    GUIUtility.hotControl = 0;
                    Application.OpenURL($"https://gdx.dotbunny.com/api/{type}.html");
                }
            }

            GUILayout.EndVertical();

            bool hasAllTypes = HasAllTypes(types);
            bool changed = GUILayout.Toggle(hasAllTypes, "", SettingsStyles.SectionHeaderToggleLayoutOptions);

            // A change has occured
            if (changed != hasAllTypes)
            {
                if (changed)
                {
                    EnsureAssemblyReferenced();
                    AddTypes(types);
                }
                else
                {
                    RemoveTypes(types);
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private static void AddTypes(List<Type> types)
        {
#if GDX_VISUALSCRIPTING
            // Itterate
            foreach (Type type in types)
            {
                if (!BoltCore.Configuration.typeOptions.Contains(type))
                {
                    BoltCore.Configuration.typeOptions.Add(type);
                }
            }
#endif
        }

        private static void EnsureAssemblyReferenced()
        {
#if GDX_VISUALSCRIPTING
            if (BoltCore.Configuration.assemblyOptions.Contains("GDX"))
            {
                return;
            }

            BoltCore.Configuration.assemblyOptions.Add(new LooseAssemblyName("GDX"));
            BoltCore.Configuration.Save();

            Codebase.UpdateSettings();
#endif
        }

        private static bool HasAllTypes(List<Type> types)
        {
#if !GDX_VISUALSCRIPTING
            return false;
#else
            foreach (Type type in types)
            {
                if (!BoltCore.Configuration.typeOptions.Contains(type))
                {
                    return false;
                }
            }

            return true;
#endif
        }

        private static void RemoveTypes(List<Type> types)
        {
#if GDX_VISUALSCRIPTING
            foreach (Type type in types)
            {
                if (BoltCore.Configuration.typeOptions.Contains(type))
                {
                    BoltCore.Configuration.typeOptions.Remove(type);
                }
            }
#endif
        }
    }
}