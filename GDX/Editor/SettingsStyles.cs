// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    /// <summary>
    ///     A collection of styles and layout options used by GDX's editor experience.
    /// </summary>
    public static class SettingsStyles
    {
        /// <summary>
        ///     A collection of layout parameters to use when rendering the expand button on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] BulletLayoutOptions;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" />" /> representing a button.
        /// </summary>
        public static readonly GUIStyle ButtonStyle;

        /// <summary>
        ///     A shade of the <see cref="UnityEngine.Color" /> blue.
        /// </summary>
        /// <remarks>Meant for default things.</remarks>
        public static readonly Color DefaultBlueColor =
            new Color(0.0941176470588235f, 0.4549019607843137f, 0.8549019607843137f);

        /// <summary>
        ///     A shade of the <see cref="UnityEngine.Color" /> yellow.
        /// </summary>
        /// <remarks>Meant for disabled things.</remarks>
        public static readonly Color DisabledYellowColor =
            new Color(0.8941176470588235f, 0.9019607843137255f, 0.4117647058823529f);

        /// <summary>
        ///     A shade of the <see cref="UnityEngine.Color" /> green.
        /// </summary>
        /// <remarks>Meant for enabled things.</remarks>
        public static readonly Color EnabledGreenColor =
            new Color(0.1803921568627451f, 0.6431372549019608f, 0.3098039215686275f);


        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a help button.
        /// </summary>
        public static readonly GUIStyle HelpButtonStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing an info box.
        /// </summary>
        public static readonly GUIStyle InfoBoxStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a horizontal line which respects margins/padding.
        /// </summary>
        public static readonly GUIStyle LineStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a default section header.
        /// </summary>
        public static readonly GUIStyle SectionHeaderStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a default section header text.
        /// </summary>
        public static readonly GUIStyle SectionHeaderTextDefaultStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a disabled section header text.
        /// </summary>
        public static readonly GUIStyle SectionHeaderTextDisabledStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing the expand button for section headers.
        /// </summary>
        public static readonly GUIStyle SectionHeaderExpandButtonStyle;

        /// <summary>
        ///     A collection of layout parameters to use when rendering the expand button on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] SectionHeaderExpandLayoutOptions;

        /// <summary>
        ///     A collection of layout parameters to use when rendering the toggle option on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] SectionHeaderToggleLayoutOptions;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing the header of a sub section definition.
        /// </summary>
        public static readonly GUIStyle SubSectionHeaderTextStyle;

        /// <summary>
        ///     A generic label with wordwrap <see cref="GUIStyle" />.
        /// </summary>
        public static readonly GUIStyle WordWrappedLabelStyle;

        /// <summary>
        ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 25% opacity.
        /// </summary>
        public static readonly Color WhiteBlend25Color = new Color(1f, 1f, 1f, 0.25f);

        /// <summary>
        ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 75% opacity.
        /// </summary>
        public static readonly Color WhiteBlend75Color = new Color(1f, 1f, 1f, 0.75f);

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> used to wrap all GDX editor user interfaces.
        /// </summary>
        public static readonly GUIStyle WrapperStyle;

        /// <summary>
        ///     Initialize the <see cref="SettingsStyles" />, creating all of the associated <see cref="GUIStyle" />s.
        /// </summary>
        static SettingsStyles()
        {
            LineStyle =
                new GUIStyle("box")
                {
                    border = {top = 0, bottom = 0},
                    margin = {top = 0, bottom = 0, left = 0, right = 0}, // makes sure to
                    padding = {top = 0, bottom = 0}
                };
            InfoBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10}, margin = {bottom = 10}
            };
            WrapperStyle = new GUIStyle {margin = {left = 5, right = 5, bottom = 5}};
            ButtonStyle =
                new GUIStyle(EditorStyles.miniButton) {stretchWidth = false, padding = {left = 10, right = 10}};

            // Section Headers
            SectionHeaderStyle = new GUIStyle("box") {margin = {left = -20}};

            SectionHeaderTextDefaultStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontStyle = FontStyle.Bold, normal = {textColor = WhiteBlend75Color}
            };
            SectionHeaderTextDisabledStyle =
                new GUIStyle(SectionHeaderTextDefaultStyle) {normal = {textColor = WhiteBlend25Color}};

            SectionHeaderToggleLayoutOptions = new[] {GUILayout.Width(EditorStyles.toggle.CalcSize(GUIContent.none).x)};

            SectionHeaderExpandButtonStyle = new GUIStyle("button") {fontStyle = FontStyle.Bold};

            SectionHeaderExpandLayoutOptions = new[] {GUILayout.Width(25)};
            BulletLayoutOptions = new[] {GUILayout.Width(10)};

            SubSectionHeaderTextStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontStyle = FontStyle.Bold, fontSize = EditorStyles.largeLabel.fontSize - 1, margin = {left = 2}
            };

            WordWrappedLabelStyle = new GUIStyle("label") {wordWrap = true};
            HelpButtonStyle = new GUIStyle("IconButton") {margin = {top = 5}};
        }
    }
}