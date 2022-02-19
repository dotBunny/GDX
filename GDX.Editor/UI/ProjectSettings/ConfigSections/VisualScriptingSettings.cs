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
    // ReSharper disable once UnusedType.Global
    internal class VisualScriptingSettings : IConfigSection
    {
        private VisualElement m_RootElement;

        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        public const string SectionID = "GDX.VisualScripting";

        /// <summary>
        ///     Information regarding the assembly
        /// </summary>
        private static AssemblyProvider s_Assembly;

        private static readonly GUIContent s_CategoryCollectionsContent = new GUIContent(
            "An extensive group of collection based classes and structs designed with performance-sensitive environments in mind.");
        private static readonly GUIContent s_CategoryExtensionsContent = new GUIContent(
            "A gathering of aggressively inlined functionality that expands on many of the built-in Unity types.");
        private static readonly GUIContent s_CategoryTypesContent = new GUIContent(
            "Additional types proven useful in specific situations.");
        private static readonly GUIContent s_CategoryUtilitiesContent = new GUIContent(
            "Function libraries useful throughout different areas of a games development.");
        
        /// <summary>
        ///     Content for the warning about regenerating units.
        /// </summary>
        private static readonly GUIContent s_VisualScriptingGenerationWarningContent =
            new GUIContent(
                "You still NEED to Regenerate Units!");

        /// <summary>
        ///     Content for the notice when visual scripting needs to regenerate its nodes.
        /// </summary>
        private static readonly GUIContent s_VisualScriptingLoadingContent = new GUIContent(
            "The visual scripting subsystem is currently loading.");

        
#if GDX_VISUALSCRIPTING
        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;
            
            // Bind top part
            
            // Get container
            
            // We need to spawn the sections as well
            VisualTreeAsset categoryAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsVisualScriptingCategory");

            VisualElement categoryInstance = categoryAsset.Instantiate()[0];
            
            // add to container
            // Bind those
            
            
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
        public int GetPriority()
        {
            return 0;
        }
        public string GetSectionHeaderLabel()
        {
            return "Visual Scripting";
        }
        public string GetSectionID()
        {
            return SectionID;
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

        public string GetToggleTooltip()
        {
            return null;
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