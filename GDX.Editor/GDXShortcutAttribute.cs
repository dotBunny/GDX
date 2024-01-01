// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace GDX.Editor
{
    public class GDXShortcutAttribute : ShortcutAttribute
    {
        /// <summary>
        ///     GDXShortcutAttribute Constructor
        /// </summary>
        /// <param name="id">
        ///     Id to register the shortcut. It will automatically be prefix by 'GDX/' in order to be in the 'GDX'
        ///     section of the shortcut manager.
        /// </param>
        /// <param name="defaultKeyCode">Optional key code for default binding.</param>
        /// <param name="defaultShortcutModifiers">Optional shortcut modifiers for default binding.</param>
        /// <param name="context">Any context which limits the use of the shortcut.</param>
        public GDXShortcutAttribute(string id, KeyCode defaultKeyCode,
            ShortcutModifiers defaultShortcutModifiers =
                ShortcutModifiers.None, Type context = null)
            : base($"GDX/{id}", context, defaultKeyCode, defaultShortcutModifiers)
        {
        }
    }
}