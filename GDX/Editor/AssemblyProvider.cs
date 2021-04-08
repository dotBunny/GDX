// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace GDX.Editor
{
    /// <summary>
    ///     A information warehouse about the GDX assemblies used in editor specific logic.
    /// </summary>
    [HideFromDocFX]
    public class AssemblyProvider
    {
        /// <summary>
        ///     A reference to the main GDX assembly
        /// </summary>
        public readonly Assembly RuntimeAssembly;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingCollectionAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingCollections;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingExtensionAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingExtensions;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingTypeAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingTypes;

        /// <summary>
        ///     A list of all types with the <see cref="GDX.VisualScriptingUtilityAttribute"/> attribute.
        /// </summary>
        public readonly List<Type> VisualScriptingUtilities;

        /// <summary>
        ///     <see cref="AssemblyProvider"/> Constructor.
        /// </summary>
        public AssemblyProvider()
        {
            RuntimeAssembly = Assembly.GetAssembly(typeof(GDXConfig));

            // Populate Visual Scripting Lists
            VisualScriptingCollections = new List<Type>();
            VisualScriptingExtensions = new List<Type>();
            VisualScriptingTypes = new List<Type>();
            VisualScriptingUtilities = new List<Type>();

            foreach(Type type in RuntimeAssembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(VisualScriptingCollectionAttribute), true).Length > 0)
                {
                    VisualScriptingCollections.Add(type);
                }
                if (type.GetCustomAttributes(typeof(VisualScriptingExtensionAttribute), true).Length > 0)
                {
                    VisualScriptingExtensions.Add(type);
                }
                if (type.GetCustomAttributes(typeof(VisualScriptingUtilityAttribute), true).Length > 0)
                {
                    VisualScriptingUtilities.Add(type);
                }
                if (type.GetCustomAttributes(typeof(VisualScriptingTypeAttribute), true).Length > 0)
                {
                    VisualScriptingTypes.Add(type);
                }
            }
        }
    }
}