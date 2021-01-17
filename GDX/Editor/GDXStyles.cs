// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace GDX.Editor
{
    /// <summary>
    ///     A helper class for generating a common editor experience.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class GDXStyles
    {
        /// <summary>
        ///     A defined type of section header indicating the possible states.
        /// </summary>
        public enum SectionHeaderMode
        {
            Default = -1,
            Disabled = 0,
            Enabled = 1
        }

        /// <summary>
        ///     A shade of the <see cref="Color"/> green.
        /// </summary>
        /// <remarks>Meant for enabled things.</remarks>
        private static readonly Color s_colorEnabledGreen = new Color(0.1803921568627451f, 0.6431372549019608f, 0.3098039215686275f);

        /// <summary>
        ///     A shade of the <see cref="Color"/> blue.
        /// </summary>
        /// <remarks>Meant for default things.</remarks>
        private static readonly Color s_colorDefaultBlue = new Color(0.0941176470588235f, 0.4549019607843137f, 0.8549019607843137f);

        /// <summary>
        ///     A shade of the <see cref="Color"/> yellow.
        /// </summary>
        /// <remarks>Meant for disabled things.</remarks>
        private static readonly Color s_colorDisabledYellow = new Color(0.8941176470588235f, 0.9019607843137255f, 0.4117647058823529f);

        /// <summary>
        ///     A blendable shade of the <see cref="Color"/> white at 25% opacity.
        /// </summary>
        private static readonly Color s_colorWhiteBlend25 = new Color(1f, 1f, 1f, 0.25f);

        /// <summary>
        ///     A blendable shade of the <see cref="Color"/> white at 75% opacity.
        /// </summary>
        private static readonly Color s_colorWhiteBlend75 = new Color(1f, 1f, 1f, 0.75f);

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a horizontal line which respects margins/padding.
        /// </summary>
        private static readonly GUIStyle s_line;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a default section header.
        /// </summary>
        private static readonly GUIStyle s_sectionHeader;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a default section header text.
        /// </summary>
        private static readonly GUIStyle s_sectionHeaderTextDefault;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a disabled section header text.
        /// </summary>
        private static readonly GUIStyle s_sectionHeaderTextDisabled;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a enabled section header text.
        /// </summary>
        private static readonly GUIStyle s_sectionHeaderTextEnabled;

        /// <summary>
        ///     A <see cref="GUIStyle" /> used to wrap all GDX editor user interfaces.
        /// </summary>
        private static readonly GUIStyle s_wrapper;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a button.
        /// </summary>
        public static readonly GUIStyle Button;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a heading at level 1.
        /// </summary>
        public static readonly GUIStyle H1;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a heading at level 2.
        /// </summary>
        public static readonly GUIStyle H2;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a heading at level 3.
        /// </summary>
        public static readonly GUIStyle H3;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing an info box.
        /// </summary>
        public static readonly GUIStyle InfoBox;

        /// <summary>
        ///     Initialize the <see cref="GDXStyles" />, creating all of the associated <see cref="GUIStyle" />s.
        /// </summary>
        static GDXStyles()
        {
            s_line =
                new GUIStyle("box")
                {
                    border = {top = 0, bottom = 0},
                    margin = {top = 0, bottom = 0, left = 0, right = 0}, // makes sure to
                    padding = {top = 0, bottom = 0}
                };

            InfoBox = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10}, margin = {bottom = 10}
            };

            s_wrapper = new GUIStyle {margin = {left = 5, right = 5, bottom = 5}};


            Button = new GUIStyle(EditorStyles.miniButton) {stretchWidth = false, padding = {left = 10, right = 10}};

            // Build our headers
            H1 = new GUIStyle(EditorStyles.largeLabel) {fontStyle = FontStyle.Bold};
            H2 = new GUIStyle(H1) {fontSize = H1.fontSize - 2};
            H3 = new GUIStyle(H2) {fontSize = H2.fontSize - 2};

            // Section Headers
            s_sectionHeader = new GUIStyle("box") { margin = { left = -20}};
            s_sectionHeaderTextDefault = new GUIStyle(H1);
            s_sectionHeaderTextEnabled = new GUIStyle(s_sectionHeaderTextDefault) {normal = {textColor = s_colorWhiteBlend75}};
            s_sectionHeaderTextDisabled = new GUIStyle(s_sectionHeaderTextDefault) {normal = {textColor = s_colorWhiteBlend25}};
        }

        /// <summary>
        ///     Simplified start of a wrapped user interface experience.
        /// </summary>
        public static void BeginGUILayout()
        {
            EditorGUILayout.BeginVertical(s_wrapper);
        }

        /// <summary>
        ///     Simplified end of a wrapped user interface experience.
        /// </summary>
        public static void EndGUILayout()
        {
            EditorGUILayout.EndVertical();
        }


        /// <summary>
        ///     Draw a line during the definition of a user interface experience which respects padding/margins, but also adds its
        ///     own vertical padding.
        /// </summary>
        /// <remarks>Has a small temp allocation for the height of the line.</remarks>
        /// <param name="height">The pixel height of the line drawn, aka thickness.</param>
        /// <param name="topPadding">The additional pixel spacing above the drawn line.</param>
        /// <param name="bottomPadding">The additional pixel spacing below the drawn line.</param>
        public static void DrawLine(float height = 1f, float topPadding = 0f, float bottomPadding = 0f)
        {
            // Add top padding
            if (topPadding > 0)
            {
                GUILayout.Space(topPadding);
            }

            // Draw line of sorts
            GUILayout.Box(GUIContent.none, s_line,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(height));

            // Add bottom padding
            if (bottomPadding > 0)
            {
                GUILayout.Space(bottomPadding);
            }
        }

        /// <summary>
        ///     Get the <see cref="SectionHeaderMode" /> from a <see cref="bool" /> flag.
        /// </summary>
        /// <param name="enabled">Is the section enabled?</param>
        /// <returns>The specific <see cref="SectionHeaderMode" />.</returns>
        public static SectionHeaderMode GetSectionHeaderMode(bool enabled)
        {
            return enabled ? SectionHeaderMode.Enabled : SectionHeaderMode.Disabled;
        }

        /// <summary>
        ///     Draw a section header useful for project settings.
        /// </summary>
        /// <param name="text">The section header content.</param>
        /// <param name="headerMode">What mode should the header be rendered in?</param>
        public static void SectionHeader(string text, SectionHeaderMode headerMode)
        {
            Color previousColor = GUI.backgroundColor;

            switch (headerMode)
            {
                case SectionHeaderMode.Enabled:
                    GUI.backgroundColor = s_colorEnabledGreen;
                    GUILayout.BeginHorizontal(s_sectionHeader);
                    GUILayout.Label(text, s_sectionHeaderTextEnabled);
                    break;
                case SectionHeaderMode.Disabled:
                    GUI.backgroundColor = s_colorDisabledYellow;
                    GUILayout.BeginHorizontal(s_sectionHeader);
                    GUILayout.Label(text, s_sectionHeaderTextDisabled);
                    break;
                default:
                    GUI.backgroundColor = s_colorDefaultBlue;
                    GUILayout.BeginHorizontal(s_sectionHeader);
                    GUILayout.Label(text, s_sectionHeaderTextDefault);
                    break;
            }

            GUILayout.EndHorizontal();
            GUI.backgroundColor = previousColor;
        }
    }
}