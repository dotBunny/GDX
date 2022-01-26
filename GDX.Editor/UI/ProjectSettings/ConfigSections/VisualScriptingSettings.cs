// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if GDX_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

#endif

using GDX.Editor;
using GDX.Editor.UI;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Visual Scripting Settings
    /// </summary>
    internal class VisualScriptingSettings : IConfigSection
    {
        private VisualElement _element;

        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.VisualScripting";

        /// <summary>
        ///     Information regarding the assembly
        /// </summary>
        private static AssemblyProvider s_assembly;

        private static readonly GUIContent s_categoryCollectionsContent = new GUIContent(
            "An extensive group of collection based classes and structs designed with performance-sensitive environments in mind.");
        private static readonly GUIContent s_categoryExtensionsContent = new GUIContent(
            "A gathering of aggressively inlined functionality that expands on many of the built-in Unity types.");
        private static readonly GUIContent s_categoryTypesContent = new GUIContent(
            "Additional types proven useful in specific situations.");
        private static readonly GUIContent s_categoryUtilitiesContent = new GUIContent(
            "Function libraries useful throughout different areas of a games development.");

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

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new VisualScriptingSettings());
        }

#if GDX_VISUALSCRIPTING
        /// <summary>
        ///     Draw the Visual Scripting settings section.
        /// </summary>
        /// <param name="settings">Serialized <see cref="Config" /> object to be modified.</param>
        public void BindSectionContent(VisualElement rootElement)
        {
            if (_element == null)
            {
                VisualTreeAsset templateAsset =
                    ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsVisualScripting");
                _element = templateAsset.Instantiate()[0];
            }
            rootElement.Add(_element);
            // GUI.enabled = true;
            //
            //
            // SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Visual Scripting",
            //     $"{SettingsProvider.DocumentationUri}manual/features/visual-scripting.html");
            //
            // // Don't show section if not enabled OR if the visual scripting has not been initialized
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return;
            // }
            //
            // if (!ProductContainer.initialized)
            // {
            //     GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
            //     GUILayout.Label(s_visualScriptingLoadingContent);
            //     GUILayout.EndVertical();
            //     return;
            // }
            //
            // if (s_assembly == null)
            // {
            //     s_assembly = new AssemblyProvider();
            // }
            //
            // GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
            // GUILayout.BeginHorizontal();
            //
            // GUILayout.Label(SettingsStyles.WarningIcon, SettingsStyles.NoHorizontalStretchStyle);
            // GUILayout.BeginVertical();
            // GUILayout.Label(s_visualScriptingGenerationNoticeContent, SettingsStyles.WordWrappedLabelStyle);
            // GUILayout.Space(5);
            // GUILayout.Label(s_visualScriptingGenerationWarningContent, EditorStyles.boldLabel);
            // GUILayout.Space(5);
            // GUILayout.Label(s_visualScriptingGenerationContent, SettingsStyles.WordWrappedLabelStyle);
            // GUILayout.Space(5);
            // GUILayout.Label(s_visualScriptingGenerationAdditionalContent, SettingsStyles.WordWrappedLabelStyle);
            // GUILayout.EndVertical();
            // GUILayout.Space(10);
            // GUILayout.BeginVertical();
            // if (GUILayout.Button("Regenerate Units", SettingsStyles.ButtonStyle))
            // {
            //     BoltCore.Configuration.Save();
            //     UnitBase.Rebuild();
            // }
            // GUILayout.Space(10);
            // if (GUILayout.Button("Install Docs", SettingsStyles.ButtonStyle))
            // {
            //     if (UpdateProvider.LocalPackage != null)
            //     {
            //         string sourceFile = System.IO.Path.Combine(
            //             Application.dataPath.Substring(0, Application.dataPath.Length - 6),
            //             UpdateProvider.LocalPackage.PackageAssetPath,
            //             ".docfx", "GDX.xml");
            //
            //         GDX.Platform.EnsureFolderHierarchyExists(BoltCore.Paths.assemblyDocumentations);
            //
            //         string targetFile = System.IO.Path.Combine(BoltCore.Paths.assemblyDocumentations, "GDX.xml");
            //         System.IO.File.Copy(sourceFile,targetFile, true);
            //
            //         XmlDocumentation.ClearCache();
            //
            //         AssetDatabase.ImportAsset("Assets/" + targetFile.Replace(Application.dataPath, "").Replace("\\", "/"));
            //     }
            // }
            // GUILayout.EndVertical();
            //
            // GUILayout.EndHorizontal();
            // GUILayout.EndVertical();
            //
            // DrawNodeSection("Collections", s_categoryCollectionsContent,  s_assembly.VisualScriptingCollections);
            //
            // GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            //
            // DrawNodeSection("Extensions", s_categoryExtensionsContent,  s_assembly.VisualScriptingExtensions);
            //
            // GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            //
            // DrawNodeSection("Types", s_categoryTypesContent, s_assembly.VisualScriptingTypes);
            //
            // GUILayout.Box(GUIContent.none, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            //
            // DrawNodeSection("Utilities", s_categoryUtilitiesContent, s_assembly.VisualScriptingUtilities);
        }
#else
        public void BindSectionContent(VisualElement rootElement)
        {
        }
#endif

        /// <summary>
        ///     Adds a provided list of types to the Visual Scripting configuration; the database still
        ///     needs to be rebuilt afterwards.
        /// </summary>
        /// <param name="types">A collection of <see cref="Type"/>.</param>
        private static void AddTypesToVisualScripting(List<Type> types)
        {
#if GDX_VISUALSCRIPTING
            foreach (Type type in types)
            {
                if (!BoltCore.Configuration.typeOptions.Contains(type))
                {
                    BoltCore.Configuration.typeOptions.Add(type);
                }
            }
#endif
        }

        /// <summary>
        ///     Draw the individual node sections for the Visual Scripting settings.
        /// </summary>
        /// <param name="category">The category label.</param>
        /// <param name="description">The <see cref="GUIContent"/> used as the description for the category.</param>
        /// <param name="types">A collection of <see cref="Type"/>.</param>
        private static void DrawNodeSection(string category, GUIContent description, List<Type> types)
        {
//             GUILayout.BeginVertical(SettingsStyles.TableRowStyle);
//             GUILayout.BeginHorizontal();
//
//             string foldoutID = $"{SectionID}_{category}";
//
//             bool sectionFoldout = EditorGUILayout.Foldout(SettingsGUIUtility.GetCachedEditorBoolean(foldoutID, false), "",
//                 SettingsStyles.CombinedFoldoutStyle);
//             SettingsGUIUtility.SetCachedEditorBoolean(foldoutID, sectionFoldout);
//
//             GUILayout.Label(category, EditorStyles.boldLabel, SettingsLayoutOptions.FixedWidth130LayoutOptions);
//
//             GUILayout.BeginVertical();
//             GUILayout.Label(description, SettingsStyles.WordWrappedLabelStyle);
//             if (sectionFoldout)
//             {
//                 GUILayout.Space(5);
//                 foreach (Type type in types)
//                 {
//                     string typeString = type.ToString();
//                     string cleanedType = typeString.GetBeforeFirst("`");
//                     if (cleanedType == null)
//                     {
//                         cleanedType = typeString;
//                     }
//
// #if UNITY_2021_1_OR_NEWER
//                     if (EditorGUILayout.LinkButton(cleanedType))
// #else
//                     if (GUILayout.Button(cleanedType, EditorStyles.linkLabel))
// #endif
//                     {
//                         GUIUtility.hotControl = 0;
//
//                         string extrasType = typeString.GetAfterFirst("`");
//
//                         if (extrasType != null)
//                         {
//                             int parameterCount = extrasType.CountOccurence(',') + 1;
//                             Application.OpenURL($"https://gdx.dotbunny.com/api/{cleanedType}-{parameterCount.ToString()}.html");
//                         }
//                         else
//                         {
//                             Application.OpenURL($"https://gdx.dotbunny.com/api/{cleanedType}.html");
//                         }
//                     }
//                 }
//                 GUILayout.Space(5);
//             }
//             GUILayout.EndVertical();
//
//             bool hasAllTypes = HasAllTypesInConfiguration(types);
//             bool changed = GUILayout.Toggle(hasAllTypes, "", SettingsLayoutOptions.SectionHeaderToggleLayoutOptions);
//
//             // A change has occured
//             if (changed != hasAllTypes)
//             {
//                 if (changed)
//                 {
//                     EnsureAssemblyReferenced();
//                     AddTypesToVisualScripting(types);
//                 }
//                 else
//                 {
//                     RemoveTypesFromVisualScripting(types);
//                 }
//             }
//
//             GUILayout.EndHorizontal();
//             GUILayout.EndVertical();
        }

        /// <summary>
        ///     Make sure GDX is added to the assemblies used by Visual Scripting.
        /// </summary>
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

        /// <summary>
        ///     Check to make sure all the provided <see cref="Type"/> are found in the Visual Scripting configuration.
        /// </summary>
        /// <param name="types">A collection of <see cref="Type"/>.</param>
        /// <returns>true/false if all the provided <see cref="Type"/> are in the configuration.</returns>
        private static bool HasAllTypesInConfiguration(List<Type> types)
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

        /// <summary>
        ///     Removes a provided list of types from the Visual Scripting configuration; the database still
        ///     needs to be rebuilt afterwards.
        /// </summary>
        /// <param name="types">A collection of <see cref="Type"/>.</param>
        private static void RemoveTypesFromVisualScripting(List<Type> types)
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

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public string GetSectionHeaderLabel()
        {
            return "Visual Scripting";
        }
        public string GetSectionID()
        {
            return "GDX.VisualScripting";
        }
        public string GetSectionHelpLink()
        {
            return "manual/features/visual-scripting.html";
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

        public void UpdateSectionContent()
        {

        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsVisualScripting";
        }
    }
}