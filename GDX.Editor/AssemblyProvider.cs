// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
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
    public static class AssemblyProvider
    {
        /// <summary>
        ///     The assembly information been populated?
        /// </summary>
        public static bool IsPopulated;

        /// <summary>
        ///     A reference to the main GDX assembly
        /// </summary>
        public static Assembly RuntimeAssembly;

        /// <summary>
        ///     A list of all types with the <see cref="VisualScriptingCompatibleAttribute"/> attribute.
        /// </summary>
        public static List<Type> VisualScriptingCollections;

        /// <summary>
        ///     A list of all types with the <see cref="VisualScriptingCompatibleAttribute"/> attribute.
        /// </summary>
        public static List<Type> VisualScriptingExtensions;

        /// <summary>
        ///     A list of all types with the <see cref="VisualScriptingCompatibleAttribute"/> attribute.
        /// </summary>
        public static List<Type> VisualScriptingTypes;

        /// <summary>
        ///     A list of all types with the <see cref="VisualScriptingCompatibleAttribute"/> attribute.
        /// </summary>
        public static List<Type> VisualScriptingUtilities;

        public static void Populate()
        {
            RuntimeAssembly = Assembly.GetAssembly(typeof(Core));

            // Populate Visual Scripting Lists
            VisualScriptingCollections = new List<Type>();
            VisualScriptingExtensions = new List<Type>();
            VisualScriptingTypes = new List<Type>();
            VisualScriptingUtilities = new List<Type>();

            foreach(Type type in RuntimeAssembly.GetTypes())
            {
                VisualScriptingCompatibleAttribute targetType = (VisualScriptingCompatibleAttribute)type.GetCustomAttribute(typeof(VisualScriptingCompatibleAttribute), true);
                if (targetType == null)
                {
                    continue;
                }

                // A whole lot of frustrating boxing
                if (targetType.VisualScriptingCategories.HasFlag(VisualScriptingCompatibleAttribute.VisualScriptingCategory.Collection))
                {
                    VisualScriptingCollections.Add(type);
                }
                if (targetType.VisualScriptingCategories.HasFlag(VisualScriptingCompatibleAttribute.VisualScriptingCategory.Extension))
                {
                    VisualScriptingExtensions.Add(type);
                }
                if (targetType.VisualScriptingCategories.HasFlag(VisualScriptingCompatibleAttribute.VisualScriptingCategory.Type))
                {
                    VisualScriptingTypes.Add(type);
                }
                if (targetType.VisualScriptingCategories.HasFlag(VisualScriptingCompatibleAttribute.VisualScriptingCategory.Utility))
                {
                    VisualScriptingUtilities.Add(type);
                }
            }

            IsPopulated = true;
        }
    }
}