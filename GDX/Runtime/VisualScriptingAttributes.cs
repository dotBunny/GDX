// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{
    /// <summary>
    ///     Indicate that the tagged <c>class</c> or <c>struct</c> should be considered as containing a collection when adding to Visual Scripting.
    /// </summary>
    [HideFromDocFX]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class VisualScriptingCollectionAttribute : Attribute
    {

    }

    /// <summary>
    ///     Indicate that the tagged <c>class</c> should be considered as containing extension methods when adding to Visual Scripting.
    /// </summary>
    [HideFromDocFX]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VisualScriptingExtensionAttribute : Attribute
    {

    }

    /// <summary>
    ///     Indicate that the tagged <c>class</c> or <c>struct</c> should be considered as containing a type when adding to Visual Scripting.
    /// </summary>
    [HideFromDocFX]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class VisualScriptingTypeAttribute : Attribute
    {

    }

    /// <summary>
    ///     Indicate that the tagged <c>class</c> should be considered as containing utility functionality when adding to Visual Scripting.
    /// </summary>
    [HideFromDocFX]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VisualScriptingUtilityAttribute : Attribute
    {

    }
}