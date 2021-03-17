// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace GDX
{
    /// <summary>
    ///     Hide the attributed item from DocFX documentation generation.
    /// </summary>
    /// <example>
    ///     <para>
    ///         For this to work a custom entry in a projects <c>filterConfig.yml</c> is necessary to define the exclusion.
    ///     </para>
    ///     <code>
    ///     - exclude:
    ///         hasAttribute:
    ///         uid: GDX.HideFromDocFXAttribute
    ///     </code>
    /// </example>
    [ExcludeFromCodeCoverage]
    public sealed class HideFromDocFXAttribute : Attribute
    {
    }
}