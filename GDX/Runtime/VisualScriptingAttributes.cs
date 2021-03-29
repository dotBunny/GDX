// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace GDX
{
    /// <summary>
    ///     Indicate that the tagged class should be considered as containing extension methods when adding to Visual Scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VisualScriptingExtensionAttribute : Attribute
    {

    }

    /// <summary>
    ///     Indicate that the tagged class should be considered as containing a type when adding to Visual Scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class VisualScriptingTypeAttribute : Attribute
    {

    }

    /// <summary>
    ///     Indicate that the tagged class should be considered as containing utility methods when adding to Visual Scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class VisualScriptingUtilityAttribute : Attribute
    {

    }
}