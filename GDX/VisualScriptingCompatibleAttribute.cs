// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

// ReSharper disable UnusedMember.Global

namespace GDX
{
    /// <summary>
    ///     Indicate that the tagged <see langword="class" /> should be considered for use with Visual Scripting.
    /// </summary>
    [HideFromDocFX]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
    public sealed class VisualScriptingCompatibleAttribute : Attribute
    {
        /// <summary>
        ///     Predetermined categories used when sorting functionality.
        /// </summary>
        [HideFromDocFX]
        [Flags]
        public enum VisualScriptingCategory : short
        {
            Default = 0,
            Collection = 1,
            Extension = 2,
            Type = 4,
            Utility = 8
        }

        /// <summary>
        ///     A specific objects categories.
        /// </summary>
        public readonly VisualScriptingCategory VisualScriptingCategories;

        /// <summary>
        ///     Indicate that the target should be part of <paramref name="visualScriptingCategories" />.
        /// </summary>
        /// <param name="visualScriptingCategories">The category flags.</param>
        public VisualScriptingCompatibleAttribute(VisualScriptingCategory visualScriptingCategories)
        {
            VisualScriptingCategories = visualScriptingCategories;
        }

        /// <summary>
        ///     Indicate that the target should be part of a specific <see cref="VisualScriptingCategory" />.
        /// </summary>
        /// <param name="visualScriptingCategory">The value of the intended category.</param>
        public VisualScriptingCompatibleAttribute(short visualScriptingCategory)
        {
            VisualScriptingCategories = (VisualScriptingCategory)visualScriptingCategory;
        }

        /// <summary>
        ///     Indicate that the target should be part of Visual Scripting, but not defining a category.
        /// </summary>
        public VisualScriptingCompatibleAttribute()
        {
            VisualScriptingCategories = VisualScriptingCategory.Default;
        }
    }
}