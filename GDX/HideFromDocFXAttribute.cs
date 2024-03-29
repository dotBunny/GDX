﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
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
    [HideFromDocFX]
    [ExcludeFromCodeCoverage]
#pragma warning disable IDE1006
    // ReSharper disable once InconsistentNaming
    public sealed class HideFromDocFXAttribute : Attribute
#pragma warning restore IDE1006
    {
    }
}