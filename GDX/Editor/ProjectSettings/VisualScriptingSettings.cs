// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

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
        ///     Draw the Build Info section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="GDXConfig" /> object to be modified.</param>
#if GDX_VISUALSCRIPTING
        public static void Draw(SerializedObject settings)
        {
            GUI.enabled = true;

            SettingsGUIUtility.CreateSettingsSection(SectionID, false, "Visual Scripting",
                $"{SettingsProvider.DocumentationUri}manual/features/visual-scripting.html");

            if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            {
                return;
            }



            if (GUILayout.Button("ADD TO ASSEMBLIES"))
            {
                EnsureAssemblyReferenced();
            }

            if (GUILayout.Button("ADD TYPE"))
            {
                Assembly gdxAssembly = Assembly.GetAssembly(typeof(GDXConfig));

                List<Type> extensionTypes = new List<Type>();
                List<Type> typeTypes = new List<Type>();
                foreach(Type type in gdxAssembly.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(VisualScriptingNodeAttribute), true);
                    foreach (object attribute in attributes)
                    {
                        VisualScriptingNodeAttribute visualScriptingNodeAttribute = (VisualScriptingNodeAttribute)attribute;
                        if (visualScriptingNodeAttribute != null)
                        {
                            switch (visualScriptingNodeAttribute._type)
                            {
                                case VisualScriptingNodeAttribute.Category.Extensions:
                                    extensionTypes.Add(type);
                                    break;
                                case VisualScriptingNodeAttribute.Category.Types:
                                    typeTypes.Add(type);
                                    break;
                            }
                        }
                    }

                }


                bool changed = false;
                foreach (Type type in extensionTypes)
                {
                    if (!BoltCore.Configuration.typeOptions.Contains(type))
                    {
                        BoltCore.Configuration.typeOptions.Add(type);
                        changed = true;
                    }
                }
                foreach (Type type in typeTypes)
                {
                    if (!BoltCore.Configuration.typeOptions.Contains(type))
                    {
                        BoltCore.Configuration.typeOptions.Add(type);
                        changed = true;
                    }
                }

                if (changed)
                {
                    BoltCore.Configuration.Save();
                    UnitBase.Rebuild();
                }
            }
        }
#endif



        private static bool AddTypes(List<Type> types)
        {

            bool changed = false;
#if GDX_VISUALSCRIPTING
            foreach (Type type in types)
            {
                if (!BoltCore.Configuration.typeOptions.Contains(type))
                {
                    BoltCore.Configuration.typeOptions.Add(type);
                    changed = true;
                }
            }
#endif
            return changed;

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
    }
}