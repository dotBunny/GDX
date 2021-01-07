// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    public static class GDXStyles
    {
        private static readonly GUIStyle s_marginLessLine;
        private static readonly GUIStyle s_line;
        private static readonly GUIStyle s_wrapper;

        public static readonly GUIStyle Header;
        public static readonly GUIStyle Button;

        static GDXStyles()
        {
            s_marginLessLine =
                new GUIStyle("box")
                {
                    border = {top = 0, bottom = 0},
                    margin = {top = 0, bottom = 0, left = -50, right = -50 }, // makes sure to
                    padding = {top = 0, bottom = 0}
                };
            s_line =
                new GUIStyle(s_marginLessLine)
                {
                    margin = {left = 0, right = 0 }
                };

            Header = new GUIStyle(EditorStyles.helpBox)
            {
                padding = {top = 10, bottom = 10, left = 10, right = 10},
                margin = { bottom = 10 }
            };

            s_wrapper = new GUIStyle()
            {
                margin = { left = 5, right = 5, bottom = 5 }
            };

            Button = new GUIStyle(EditorStyles.miniButton)
            {
                stretchWidth = false,
                padding = { left = 10, right = 10 },
            };
        }

        public static void BeginGUILayout()
        {
            EditorGUILayout.BeginVertical(s_wrapper);
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
            GUILayout.Box(GUIContent.none, s_marginLessLine,
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