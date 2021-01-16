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
        ///     A <see cref="GUIStyle" /> representing a horizontal line which respects margins/padding.
        /// </summary>
        private static readonly GUIStyle s_line;

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
            H1 = new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold };
            H2 = new GUIStyle(H1) {fontSize = H1.fontSize - 2};
            H3 = new GUIStyle(H2) {fontSize = H2.fontSize - 2};

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
    }
}