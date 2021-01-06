// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    public static class GDXStyles
    {
        private static readonly GUIStyle s_marginLessLineStyle;
        private static readonly GUIStyle s_lineStyle;
        private static readonly GUIStyle s_wrapperStyle;

        public static readonly GUIStyle HeaderStyle;

        static GDXStyles()
        {
            s_marginLessLineStyle =
                new GUIStyle("box")
                {
                    border = {top = 0, bottom = 0},
                    margin = {top = 0, bottom = 0, left = -50, right = -50 }, // makes sure to
                    padding = {top = 0, bottom = 0}
                };
            s_lineStyle =
                new GUIStyle(s_marginLessLineStyle)
                {
                    margin = {left = 0, right = 0 }
                };

            HeaderStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10},
                margin = { bottom = 10 }
            };

            s_wrapperStyle = new GUIStyle()
            {
                margin = { left = 5, right = 5, bottom = 5 }
            };
        }

        public static void BeginGUILayout()
        {
            EditorGUILayout.BeginVertical(s_wrapperStyle);
        }

        public static void EndGUILayout()
        {
            EditorGUILayout.EndVertical();
        }

        public static void DrawFullLine(float height = 1f, float topPadding = 0f, float bottomPadding = 0f)
        {
            // Add top padding
            if (topPadding > 0)
            {
                GUILayout.Space(topPadding);
            }

            // Draw line of sorts
            GUILayout.Box(GUIContent.none, s_marginLessLineStyle,
                GUILayout.ExpandWidth(true),
                GUILayout.Height(height));

            // Add bottom padding
            if (bottomPadding > 0)
            {
                GUILayout.Space(bottomPadding);
            }
        }
        public static void DrawLine(float height = 1f, float topPadding = 0f, float bottomPadding = 0f)
        {
            // Add top padding
            if (topPadding > 0)
            {
                GUILayout.Space(topPadding);
            }

            // Draw line of sorts
            GUILayout.Box(GUIContent.none, s_lineStyle,
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