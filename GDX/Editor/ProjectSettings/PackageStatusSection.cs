// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Developer;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Package Status Section
    /// </summary>
    internal static  class PackageStatusSection
    {
        /// <summary>
        ///     Content for the initial introduction of the projects settings window.
        /// </summary>
        private static readonly GUIContent s_aboutBlurbContent = new GUIContent(
            "Game Development Extensions, a battle-tested library of game-ready high-performance C# code.");

        /// <summary>
        ///     Draw the packages status section of the settings window.
        /// </summary>
        internal static void Draw()
        {
            GUI.enabled = true;

            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            EditorGUILayout.BeginHorizontal();

            // Information
            GUILayout.BeginVertical();

            GUILayout.Label(s_aboutBlurbContent, SettingsStyles.WordWrappedLabelStyle);
            GUILayout.Space(10);


            GUILayout.BeginHorizontal();
            GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton("Repository"))
#elif UNITY_2019_1_OR_NEWER
            if (GUILayout.Button("Repository", EditorStyles.linkLabel))
#else
            if (GUILayout.Button("Repository", EditorStyles.boldLabel))
#endif
            {
                GUIUtility.hotControl = 0;
                Application.OpenURL("https://github.com/dotBunny/GDX/");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton("Documentation"))
#elif UNITY_2019_1_OR_NEWER
            if (GUILayout.Button("Documentation", EditorStyles.linkLabel))
#else
            if (GUILayout.Button("Documentation", EditorStyles.boldLabel))
#endif
            {
                GUIUtility.hotControl = 0;
                Application.OpenURL(SettingsProvider.DocumentationUri);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("-", SettingsStyles.BulletLayoutOptions);
#if UNITY_2021_1_OR_NEWER
            if (EditorGUILayout.LinkButton("Report an Issue"))
#elif UNITY_2019_1_OR_NEWER
            if (GUILayout.Button("Report an Issue", EditorStyles.linkLabel))
#else
            if (GUILayout.Button("Report an Issue", EditorStyles.boldLabel))
#endif
            {
                GUIUtility.hotControl = 0;
                Application.OpenURL("https://github.com/dotBunny/GDX/issues");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            GUILayout.Space(15);
            GUILayout.FlexibleSpace();

            // CHeck Packages
            // ReSharper disable UnreachableCode
#pragma warning disable 162
            EditorGUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
            GUILayout.Label("Packages Found", SettingsStyles.SubSectionHeaderTextStyle);
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            // ReSharper disable once StringLiteralTypo
            GUILayout.Label("Addressables");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Conditionals.HasAddressablesPackage
                ? SettingsStyles.TestPassedIcon
                : SettingsStyles.TestNormalIcon);

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Burst");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Conditionals.HasBurstPackage
                ? SettingsStyles.TestPassedIcon
                : SettingsStyles.TestNormalIcon);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mathematics");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Conditionals.HasMathematicsPackage
                ? SettingsStyles.TestPassedIcon
                : SettingsStyles.TestNormalIcon);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Platforms");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Conditionals.HasPlatformsPackage
                ? SettingsStyles.TestPassedIcon
                : SettingsStyles.TestNormalIcon);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Visual Scripting");
            GUILayout.FlexibleSpace();
            GUILayout.Label(Conditionals.HasVisualScriptingPackage
                ? SettingsStyles.TestPassedIcon
                : SettingsStyles.TestNormalIcon);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            // ReSharper restore UnreachableCode
#pragma warning restore 162

            EditorGUILayout.EndHorizontal();

            // ReSharper enable ConditionIsAlwaysTrueOrFalse

            GUILayout.Space(5);
        }
    }
}