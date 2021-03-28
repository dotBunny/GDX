// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     A collection of styles and layout options used by the settings window.
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
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a default section header.
        /// </summary>
        public static readonly GUIStyle SectionHeaderStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> representing a table row.
        /// </summary>
        public static readonly GUIStyle TableRowStyle;

        /// <summary>
        ///     A specific max width (130) for layout options to allow for organized width layouts.
        /// </summary>
        public static readonly GUILayoutOption[] FixedWidth130LayoutOptions;

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
        ///     A <see cref="UnityEngine.GUIStyle" /> representing content that doesnt stretch horizontally.
        /// </summary>
        public static readonly GUIStyle NoHorizontalStretchStyle;

        /// <summary>
        ///     A generic label with wordwrap <see cref="GUIStyle" />.
        /// </summary>
        public static readonly GUIStyle WordWrappedLabelStyle;

        /// <summary>
        ///     A <see cref="UnityEngine.GUIStyle" /> used to wrap all GDX editor user interfaces.
        /// </summary>
        public static readonly GUIStyle WrapperStyle;

        /// <summary>
        ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 25% opacity.
        /// </summary>
        private static readonly Color s_whiteBlend25Color = new Color(1f, 1f, 1f, 0.25f);

        /// <summary>
        ///     A blendable shade of the <see cref="UnityEngine.Color" /> white at 75% opacity.
        /// </summary>
        private static readonly Color s_whiteBlend75Color = new Color(1f, 1f, 1f, 0.75f);

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a help symbol.
        /// </summary>
        public static readonly GUIContent HelpIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a minus symbol.
        /// </summary>
        public static readonly GUIContent MinusIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent"/> containing a notice symbol.
        /// </summary>
        public static readonly GUIContent NoticeIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a plus symbol.
        /// </summary>
        public static readonly GUIContent PlusIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a cross symbol.
        /// </summary>
        public static readonly GUIContent TestNormalIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a checkmark symbol.
        /// </summary>
        public static readonly GUIContent TestPassedIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a warning symbol.
        /// </summary>
        public static readonly GUIContent WarningIcon;

        /// <summary>
        ///     Initialize the <see cref="SettingsStyles" />, creating all of the associated <see cref="GUIStyle" />s.
        /// </summary>
        static SettingsStyles()
        {
            if (EditorGUIUtility.isProSkin)
            {
                PlusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
                MinusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
                HelpIcon = EditorGUIUtility.IconContent("_Help");
                WarningIcon = EditorGUIUtility.IconContent("d_console.warnicon@2x");
                NoticeIcon = EditorGUIUtility.IconContent("d_console.infoicon@2x");
            }
            else
            {
                PlusIcon = EditorGUIUtility.IconContent("d_Toolbar Plus");
                MinusIcon = EditorGUIUtility.IconContent("d_Toolbar Minus");
                HelpIcon = EditorGUIUtility.IconContent("d__Help");
                WarningIcon = EditorGUIUtility.IconContent("console.warnicon@2x");
                NoticeIcon = EditorGUIUtility.IconContent("console.infoicon@2x");
            }
            TestPassedIcon = EditorGUIUtility.IconContent("TestPassed");
            TestNormalIcon = EditorGUIUtility.IconContent("TestNormal");

            TableRowStyle = new GUIStyle()
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10},
                margin = {bottom = 10},
            };
            InfoBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10}, margin = {bottom = 10}
            };
            WrapperStyle = new GUIStyle {margin = {left = 5, right = 5, bottom = 5}};
            ButtonStyle =
                new GUIStyle(EditorStyles.miniButton) {stretchWidth = false, padding = {left = 10, right = 10}};
            NoHorizontalStretchStyle = new GUIStyle() {stretchWidth = false};

            // Section Headers
            SectionHeaderStyle = new GUIStyle("box") {margin = {left = -20}};

            SectionHeaderTextDefaultStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontStyle = FontStyle.Bold, normal = {textColor = s_whiteBlend75Color}
            };
            SectionHeaderTextDisabledStyle =
                new GUIStyle(SectionHeaderTextDefaultStyle) {normal = {textColor = s_whiteBlend25Color}};

            SectionHeaderToggleLayoutOptions =
                new[] {GUILayout.Width(EditorStyles.toggle.CalcSize(GUIContent.none).x)};

            SectionHeaderExpandButtonStyle = new GUIStyle("button") {fontStyle = FontStyle.Bold};

            SectionHeaderExpandLayoutOptions = new[] {GUILayout.Width(25)};
            BulletLayoutOptions = new[] {GUILayout.Width(10)};

            SubSectionHeaderTextStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontStyle = FontStyle.Bold, fontSize = EditorStyles.largeLabel.fontSize - 1, margin = {left = 2}
            };

            WordWrappedLabelStyle = new GUIStyle("label") {wordWrap = true};
            HelpButtonStyle = new GUIStyle("IconButton") {margin = {top = 5}};

            FixedWidth130LayoutOptions =
                new[] {GUILayout.MaxWidth(130), GUILayout.MinWidth(130), GUILayout.Width(130)};
        }
    }
}