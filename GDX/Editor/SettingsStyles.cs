// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation.Evident
// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    /// <summary>
    ///     A helper class for generating the GDX editor experience.
    /// </summary>
    public static class SettingsStyles
    {
        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a button.
        /// </summary>
        public static readonly GUIStyle ButtonStyle;

        /// <summary>
        ///     A shade of the <see cref="Color" /> blue.
        /// </summary>
        /// <remarks>Meant for default things.</remarks>
        public static readonly Color DefaultBlueColor =
            new Color(0.0941176470588235f, 0.4549019607843137f, 0.8549019607843137f);

        /// <summary>
        ///     A shade of the <see cref="Color" /> yellow.
        /// </summary>
        /// <remarks>Meant for disabled things.</remarks>
        public static readonly Color DisabledYellowColor =
            new Color(0.8941176470588235f, 0.9019607843137255f, 0.4117647058823529f);

        /// <summary>
        ///     A shade of the <see cref="Color" /> green.
        /// </summary>
        /// <remarks>Meant for enabled things.</remarks>
        public static readonly Color EnabledGreenColor =
            new Color(0.1803921568627451f, 0.6431372549019608f, 0.3098039215686275f);

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing an info box.
        /// </summary>
        public static readonly GUIStyle InfoBoxStyle;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a horizontal line which respects margins/padding.
        /// </summary>
        public static readonly GUIStyle LineStyle;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a default section header.
        /// </summary>
        public static readonly GUIStyle SectionHeaderStyle;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a default section header text.
        /// </summary>
        public static readonly GUIStyle SectionHeaderTextDefaultStyle;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a disabled section header text.
        /// </summary>
        public static readonly GUIStyle SectionHeaderTextDisabledStyle;

        /// <summary>
        ///     A collection of layout parameters to use when rendering the toggle option on section headers.
        /// </summary>
        public static readonly GUILayoutOption[] SectionHeaderToggleLayoutOptions;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing the header of a sub section definition.
        /// </summary>
        public static readonly GUIStyle SubSectionHeaderTextStyle;

        /// <summary>
        ///     A blendable shade of the <see cref="Color" /> white at 25% opacity.
        /// </summary>
        public static readonly Color WhiteBlend25Color = new Color(1f, 1f, 1f, 0.25f);

        /// <summary>
        ///     A blendable shade of the <see cref="Color" /> white at 75% opacity.
        /// </summary>
        public static readonly Color WhiteBlend75Color = new Color(1f, 1f, 1f, 0.75f);

        /// <summary>
        ///     A <see cref="GUIStyle" /> used to wrap all GDX editor user interfaces.
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
            SectionHeaderTextDefaultStyle = new GUIStyle(EditorStyles.largeLabel) {fontStyle = FontStyle.Bold, normal = {textColor = WhiteBlend75Color}};
            SectionHeaderTextDisabledStyle =
                new GUIStyle(SectionHeaderTextDefaultStyle) {normal = {textColor = WhiteBlend25Color}};
            SectionHeaderToggleLayoutOptions = new[] {GUILayout.Width(EditorStyles.toggle.CalcSize(GUIContent.none).x)};
            SubSectionHeaderTextStyle = new GUIStyle(EditorStyles.largeLabel) {fontStyle = FontStyle.Bold, fontSize = EditorStyles.largeLabel.fontSize - 1, margin = { left = 2 }};
        }

        /// <summary>
        ///     Draw a line during the definition of a user interface experience which respects padding/margins, but also adds its
        ///     own vertical padding.
        /// </summary>
        /// <remarks>Has a small temp allocation for the height of the line.</remarks>
        /// <param name="height">The pixel height of the line drawn, aka thickness.</param>
        /// <param name="topPadding">The additional pixel spacing above the drawn line.</param>
        /// <param name="bottomPadding">The additional pixel spacing below the drawn line.</param>
        public static void Line(float height = 1f, float topPadding = 0f, float bottomPadding = 0f)
        {
            // Add top padding
            if (topPadding > 0)
            {
                GUILayout.Space(topPadding);
            }

            // Draw line of sorts
            GUILayout.Box(GUIContent.none, LineStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(height));

            // Add bottom padding
            if (bottomPadding > 0)
            {
                GUILayout.Space(bottomPadding);
            }
        }

        /// <summary>
        ///     Draw a section header useful for project settings.
        /// </summary>
        /// <param name="text">The section header content.</param>
        /// <param name="sectionToggleProperty">
        ///     A <see cref="SerializedProperty" /> which will dictate if a section is enabled or
        ///     not.
        /// </param>
        /// <param name="sectionToggleContent">The <see cref="GUIContent" /> associated with a setting.</param>
        /// <returns>true/false if the sections content should be enabled.</returns>
        public static bool SectionHeader(string text, SerializedProperty sectionToggleProperty = null,
            GUIContent sectionToggleContent = null)
        {
            Color previousColor = GUI.backgroundColor;

            if (sectionToggleProperty == null)
            {
                GUI.backgroundColor = DefaultBlueColor;
                GUILayout.BeginHorizontal(SectionHeaderStyle);
                GUILayout.Label(text, SectionHeaderTextDefaultStyle);
                GUILayout.EndHorizontal();
                GUI.backgroundColor = previousColor;
                GUILayout.Space(5);
                return true;
            }

            if (sectionToggleProperty.boolValue)
            {
                GUI.backgroundColor = EnabledGreenColor;
                GUILayout.BeginHorizontal(SectionHeaderStyle);
                GUILayout.Label(text, SectionHeaderTextDefaultStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                    SectionHeaderToggleLayoutOptions);
            }
            else
            {
                GUI.backgroundColor = DisabledYellowColor;
                GUILayout.BeginHorizontal(SectionHeaderStyle);
                GUILayout.Label(text, SectionHeaderTextDisabledStyle);
                GUILayout.FlexibleSpace();
                EditorGUILayout.PropertyField(sectionToggleProperty, sectionToggleContent,
                    SectionHeaderToggleLayoutOptions);
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = previousColor;
            GUILayout.Space(5);
            return sectionToggleProperty.boolValue;
        }
    }
}