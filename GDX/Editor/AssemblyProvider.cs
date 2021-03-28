// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     A information warehouse about the GDX assemblies used in editor specific logic.
    /// </summary>
    public class AssemblyProvider
    {
        /// <summary>
        ///     A reference to the main GDX assembly
        /// </summary>
        public readonly Assembly RuntimeAssembly;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingExtensionAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingExtensions;

        /// <summary>
        ///     A multi line list of all types with the <see cref="GDX.VisualScriptingExtensionAttribute"/> attribute.
        /// </summary>
        public readonly GUIContent VisualScriptingExtensionsContent;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingTypeAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingTypes;

        /// <summary>
        ///     A multi line list of all types with the <see cref="GDX.VisualScriptingTypeAttribute"/> attribute.
        /// </summary>
        public readonly GUIContent VisualScriptingTypesContent;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingUtilityAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingUtilities;

        /// <summary>
        ///     A multi line list of all types with the <see cref="GDX.VisualScriptingUtilityAttribute"/> attribute.
        /// </summary>
        public readonly GUIContent VisualScriptingUtilitiesContent;

        /// <summary>
        ///     <see cref="AssemblyProvider"/> Constructor.
        /// </summary>
        public AssemblyProvider()
        {
            RuntimeAssembly = Assembly.GetAssembly(typeof(GDXConfig));

            // Populate Visual Scripting Lists
            VisualScriptingExtensions = new List<Type>();
            VisualScriptingTypes = new List<Type>();
            VisualScriptingUtilities = new List<Type>();

            // We're also going to do some building of a string output of whats in the list
            StringBuilder builderExtensions = new StringBuilder();
            StringBuilder builderTypes= new StringBuilder();
            StringBuilder builderUtilities = new StringBuilder();

            foreach(Type type in RuntimeAssembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(VisualScriptingExtensionAttribute), true).Length > 0)
                {
                    VisualScriptingExtensions.Add(type);
                    builderExtensions.AppendLine(type.ToString());
                }
                if (type.GetCustomAttributes(typeof(VisualScriptingUtilityAttribute), true).Length > 0)
                {
                    VisualScriptingUtilities.Add(type);
                    builderUtilities.AppendLine(type.ToString());
                }
                if (type.GetCustomAttributes(typeof(VisualScriptingTypeAttribute), true).Length > 0)
                {
                    VisualScriptingTypes.Add(type);
                    builderTypes.AppendLine(type.ToString());
                }
            }

            VisualScriptingExtensionsContent = new GUIContent(builderExtensions.ToString().Trim());
            VisualScriptingTypesContent = new GUIContent(builderTypes.ToString().Trim());
            VisualScriptingUtilitiesContent = new GUIContent(builderUtilities.ToString().Trim());
        }
    }
}