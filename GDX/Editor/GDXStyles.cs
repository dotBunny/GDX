// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     A helper class for generating a common editor experience.
    /// </summary>
    public static class GDXStyles
    {
        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a horizontal line which negates margins/padding.
        /// </summary>
        private static readonly GUIStyle s_marginLessLine;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a horizontal line which respects margins/padding.
        /// </summary>
        private static readonly GUIStyle s_line;

        /// <summary>
        ///     A <see cref="GUIStyle" /> used to wrap all GDX editor user interfaces.
        /// </summary>
        private static readonly GUIStyle s_wrapper;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a header.
        /// </summary>
        public static readonly GUIStyle Header;

        /// <summary>
        ///     A <see cref="GUIStyle" /> representing a button.
        /// </summary>
        public static readonly GUIStyle Button;

        /// <summary>
        ///     Initialize the <see cref="GDXStyles" />, creating all of the associated <see cref="GUIStyle" />s.
        /// </summary>
        static GDXStyles()
        {
            s_marginLessLine =
                new GUIStyle("box")
                {
                    border = {top = 0, bottom = 0},
                    margin = {top = 0, bottom = 0, left = -50, right = -50}, // makes sure to
                    padding = {top = 0, bottom = 0}
                };
            s_line =
                new GUIStyle(s_marginLessLine) {margin = {left = 0, right = 0}};

            Header = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10}, margin = {bottom = 10}
            };

            s_wrapper = new GUIStyle {margin = {left = 5, right = 5, bottom = 5}};

            Button = new GUIStyle(EditorStyles.miniButton) {stretchWidth = false, padding = {left = 10, right = 10}};
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
        ///     Draw a line during the definition of a user interface experience which does not respect padding/margins, instead
        ///     using its own vertical padding.
        /// </summary>
        /// <param name="height">The pixel height of the line drawn, aka thickness.</param>
        /// <param name="topPadding">The pixel spacing above the drawn line.</param>
        /// <param name="bottomPadding">The pixel spacing below the drawn line.</param>
        public static void DrawFullLine(float height = 1f, float topPadding = 0f, float bottomPadding = 0f)
        {
            // Add top padding
            if (topPadding > 0)
            {
                GUILayout.Space(topPadding);
            }

            // Draw line of sorts
            GUILayout.Box(GUIContent.none, s_marginLessLine,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(height));

            // Add bottom padding
            if (bottomPadding > 0)
            {
                GUILayout.Space(bottomPadding);
            }
        }

        /// <summary>
        ///     Draw a line during the definition of a user interface experience which respects padding/margins, but also adds its
        ///     own vertical padding.
        /// </summary>
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