﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace GDX
{
    /// <summary>
    ///     Make the field in the inspector disabled by toggling the GUI.enabled before and after.
    /// </summary>
    [HideFromDocFX]
    [ExcludeFromCodeCoverage]
    public sealed class DisableInInspectorAttribute : PropertyAttribute
    {
    }
}