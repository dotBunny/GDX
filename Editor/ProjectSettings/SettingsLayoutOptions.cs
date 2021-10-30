// Copyright (c) 2020-2021 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    public class SettingsLayoutOptions
    {
        /// <summary>
        ///     A collection of layout parameters to use when rendering the expand button on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] BulletLayoutOptions;

        /// <summary>
        ///     A specific max width (130) for layout options to allow for organized width layouts.
        /// </summary>
        public static readonly GUILayoutOption[] FixedWidth130LayoutOptions;

        /// <summary>
        ///     A specific max width (150) for layout options to allow for organized width layouts with a margin on the right.
        /// </summary>
        public static readonly GUILayoutOption[] FixedWidth150LayoutOptions;

        /// <summary>
        ///     A collection of layout parameters to use when rendering the expand button on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] SectionHeaderExpandLayoutOptions;

        /// <summary>
        ///     A collection of layout parameters to use when rendering the toggle option on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] SectionHeaderToggleLayoutOptions;

        static SettingsLayoutOptions()
        {
            BulletLayoutOptions = new[] {GUILayout.Width(10)};
            SectionHeaderToggleLayoutOptions =
                new[] {GUILayout.Width(EditorStyles.toggle.CalcSize(GUIContent.none).x)};
            SectionHeaderExpandLayoutOptions = new[] {GUILayout.Width(25)};
            FixedWidth130LayoutOptions =
                new[] {GUILayout.MaxWidth(130), GUILayout.MinWidth(130), GUILayout.Width(130)};
            FixedWidth150LayoutOptions =
                new[] {GUILayout.MaxWidth(150), GUILayout.MinWidth(150), GUILayout.Width(150)};
        }
    }
}