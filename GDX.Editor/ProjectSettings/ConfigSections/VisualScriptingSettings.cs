// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
#if GDX_VISUALSCRIPTING
using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
#endif

using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Visual Scripting Settings
    /// </summary>
    class VisualScriptingSettings : IConfigSection
    {
        public const int SectionIndex = 7;
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        public const string SectionKey = "GDX.VisualScripting";
        static readonly string[] k_Keywords = { "vs", "visual scripting" };

        Button m_ButtonRegenerateUnits;
        Button m_ButtonInstallDocs;
        VisualElement m_CategorySectionsContainer;
        VisualElement m_RootElement;

#if GDX_VISUALSCRIPTING
        /// <inheritdoc />
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            // Generate assembly information
            if (!AssemblyProvider.IsPopulated)
            {
                AssemblyProvider.Populate();
            }

            // Bind Top
            m_ButtonRegenerateUnits = m_RootElement.Q<Button>("button-regenerate-units");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ButtonRegenerateUnits);
            m_ButtonRegenerateUnits.clicked += () =>
            {
                BoltCore.Configuration.Save();
                UnitBase.Rebuild();
            };
            m_ButtonInstallDocs = m_RootElement.Q<Button>("button-install-docs");
            m_ButtonInstallDocs.clicked += () =>
            {
                if (UpdateProvider.LocalPackage != null)
                {
                    string sourceFile = System.IO.Path.Combine(
                        Application.dataPath.Substring(0, Application.dataPath.Length - 6),
                        UpdateProvider.LocalPackage.PackageAssetPath,
                        ".docfx", "GDX.xml");

                    Platform.EnsureFolderHierarchyExists(BoltCore.Paths.assemblyDocumentations);

                    string targetFile = System.IO.Path.Combine(BoltCore.Paths.assemblyDocumentations, "GDX.xml");
                    System.IO.File.Copy(sourceFile,targetFile, true);

                    XmlDocumentation.ClearCache();

                    AssetDatabase.ImportAsset("Assets/" + targetFile.Replace(Application.dataPath, "").Replace("\\", "/"));
                }
            };

            m_CategorySectionsContainer = m_RootElement.Q<VisualElement>("sections");

            m_CategorySectionsContainer.Add(CreateBindUpdateCategorySection(
                    "Collections",
                    "An extensive group of collection based classes and structs designed with performance-sensitive environments in mind.",
                    AssemblyProvider.VisualScriptingCollections));

            m_CategorySectionsContainer.Add(CreateBindUpdateCategorySection(
                "Extensions",
                "A gathering of aggressively inlined functionality that expands on many of the built-in Unity types.",
                AssemblyProvider.VisualScriptingExtensions));

            m_CategorySectionsContainer.Add(CreateBindUpdateCategorySection(
                "Types",
                "Additional types proven useful in specific situations.",
                AssemblyProvider.VisualScriptingTypes));

            m_CategorySectionsContainer.Add(CreateBindUpdateCategorySection(
                "Utilities",
                "Function libraries useful throughout different areas of a games development.",
                AssemblyProvider.VisualScriptingUtilities, false));
        }

        static VisualElement CreateBindUpdateCategorySection(string sectionName, string sectionDescription, List<Type> sectionTypes, bool bottomBorder = true)
        {
            VisualTreeAsset categoryAsset =
                ResourcesProvider.GetVisualTreeAsset("GDXProjectSettingsVisualScriptingCategory");
            VisualElement categoryInstance = categoryAsset.Instantiate()[0];

            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, categoryInstance, sectionDescription);

            Label labelCategory = categoryInstance.Q<Label>("label-category");
            labelCategory.text = sectionName;
            Label labelDescription  = categoryInstance.Q<Label>("label-description");

            labelDescription.text = sectionDescription;
            VisualElement elementTypeContainer = categoryInstance.Q<VisualElement>("type-container");

            Button buttonFoldout = categoryInstance.Q<Button>("button-foldout");
            buttonFoldout.clicked += () =>
            {
                buttonFoldout.ToggleInClassList("expanded");
                if (buttonFoldout.ClassListContains("expanded"))
                {
                    elementTypeContainer.RemoveFromClassList("hidden");
                }
                else
                {
                    elementTypeContainer.AddToClassList("hidden");
                }
            };

            // Populate the sub list of types
            foreach (Type type in sectionTypes)
            {
                string typeString = type.ToString();
                string cleanedType = typeString.GetBeforeFirst("`") ?? typeString;

                Button buttonType = new Button();
                buttonType.AddToClassList("gdx-link");
                buttonType.text = typeString;
                buttonType.name = $"button-{cleanedType}";
                buttonType.clicked += () =>
                {
                    string extrasType = typeString.GetAfterFirst("`");

                    if (extrasType != null)
                    {
                        int parameterCount = extrasType.CountOccurence(',') + 1;
                        Application.OpenURL(
                            $"https://gdx.dotbunny.com/api/{cleanedType}-{parameterCount.ToString()}.html");
                    }
                    else
                    {
                        Application.OpenURL($"https://gdx.dotbunny.com/api/{cleanedType}.html");
                    }
                };
                elementTypeContainer.Add(buttonType);
            }

            // Build toggle
            Toggle toggleCategory = categoryInstance.Q<Toggle>("toggle-category");
            toggleCategory.SetValueWithoutNotify(HasAllTypesInConfiguration(sectionTypes));
            toggleCategory.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue)
                {
                    EnsureAssemblyReferenced();
                    AddTypesToVisualScripting(sectionTypes);
                }
                else if (!evt.newValue)
                {
                    RemoveTypesFromVisualScripting(sectionTypes);
                }
            });

            // Do we want the divider?
            if (bottomBorder)
            {
                categoryInstance.AddToClassList("gdx-visual-scripting-divider");
            }
            // Add back
            return categoryInstance;
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
        static void AddTypesToVisualScripting(List<Type> types)
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
        ///     Make sure GDX is added to the assemblies used by Visual Scripting.
        /// </summary>
        static void EnsureAssemblyReferenced()
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
        static bool HasAllTypesInConfiguration(List<Type> types)
        {
#if !GDX_VISUALSCRIPTING
            return false;
#else
            foreach (Type type in types)
            {
                if (BoltCore.instance == null ||
                    BoltCore.instance.configuration == null)
                {
                    return false;
                }

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
        static void RemoveTypesFromVisualScripting(List<Type> types)
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

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetSectionHeaderLabel()
        {
            return "Visual Scripting";
        }
        public int GetSectionIndex()
        {
            return SectionIndex;
        }

        public string GetSectionKey()
        {
            return SectionKey;
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