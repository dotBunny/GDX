// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

#if GDX_VISUALSCRIPTING
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer.Internal;

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

        private static AssemblyProvider s_assembly = null;
        private static int s_visualScriptingNodeChangeCount = 0;

        /// <summary>
        ///     Content for the notice when visual scripting needs to regenerate its nodes.
        /// </summary>
        private static readonly GUIContent s_visualScriptingGenerationContent = new GUIContent(
            "The visual scripting system needs to rebuild its node database.");

        /// <summary>
        ///     Content for the notice when visual scripting needs to regenerate its nodes.
        /// </summary>
        private static readonly GUIContent s_visualScriptingLoadingContent = new GUIContent(
            "The visual scripting system is currently loading.");

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

            if (s_assembly == null) s_assembly = new AssemblyProvider();

            DrawNodeSection("Extensions", s_assembly.VisualScriptingExtensionsContent,
                s_assembly.VisualScriptingExtensions);

            GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));

            DrawNodeSection("Types", s_assembly.VisualScriptingTypesContent,
                s_assembly.VisualScriptingTypes);

            GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));

            DrawNodeSection("Utilities", s_assembly.VisualScriptingUtilitiesContent,
                s_assembly.VisualScriptingUtilities);


            if (s_visualScriptingNodeChangeCount > 0)
            {
                GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
                //GUILayout.BeginHorizontal();

                GUILayout.Label(s_visualScriptingGenerationContent);
                if (GUILayout.Button("Update Node Database"))
                {
                    BoltCore.Configuration.Save();
                    UnitBase.Rebuild();
                    s_visualScriptingNodeChangeCount = 0;
                }
                //GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

        }
#endif

        private static void DrawNodeSection(string section, GUIContent content, List<Type> types)
        {
            GUILayout.BeginVertical(SettingsStyles.TableRowStyle);
            GUILayout.BeginHorizontal();
            GUILayout.Label(section, EditorStyles.boldLabel,SettingsStyles.FixedWidth130LayoutOptions);
            GUILayout.Label(content);

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

                s_visualScriptingNodeChangeCount = 1;
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