// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Make the field in the inspector disabled by toggling GUI.enabled before and after.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class DisableInInspectorAttribute : PropertyAttribute
    {

    }
}