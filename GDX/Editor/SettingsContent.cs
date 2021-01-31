// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

// ReSharper disable HeapView.ObjectAllocation.Evident

namespace GDX.Editor
{
    /// <summary>
    ///     A static collection of <see cref="GUIContent" /> used by <see cref="Settings" />.
    /// </summary>
    public static class SettingsContent
    {
        /// <summary>
        ///     Content for the initial introduction of the projects settings window.
        /// </summary>
        public static readonly GUIContent AboutBlurb = new GUIContent(
            "Game Development Extensions, a battle-tested library of game-ready high-performance C# code.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.updateProviderCheckForUpdates" />.
        /// </summary>
        public static readonly GUIContent AutomaticUpdatesEnabled = new GUIContent(
            "",
            "Should the package check the GitHub repository to see if there is a new version?");

        /// <summary>
        ///     Settings content for <see cref="Settings.UpdateDayCountSetting" />.
        /// </summary>
        public static readonly GUIContent AutomaticUpdatesUpdateDayCount = new GUIContent(
            "Update Timer (Days)",
            "After how many days should updates be checked for?");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoAssemblyDefinition" />.
        /// </summary>
        public static readonly GUIContent BuildInfoAssemblyDefinition = new GUIContent(
            "Assembly Definition",
            "Ensure that the folder of the BuildInfo has an assembly definition.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildChangelistArgument" />.
        /// </summary>
        public static readonly GUIContent BuildInfoBuildChangelistArgument = new GUIContent(
            "Changelist",
            "The argument key for the build changelist to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildDescriptionArgument" />.
        /// </summary>
        public static readonly GUIContent BuildInfoBuildDescriptionArgument = new GUIContent(
            "Description",
            "The argument key for the build description to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildNumberArgument" />.
        /// </summary>
        public static readonly GUIContent BuildInfoBuildNumberArgument = new GUIContent(
            "Number",
            "The argument key for the build number to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildStreamArgument" />.
        /// </summary>
        public static readonly GUIContent BuildInfoBuildStreamArgument = new GUIContent(
            "Stream",
            "The argument key for the build stream to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildTaskArgument" />.
        /// </summary>
        public static readonly GUIContent BuildInfoBuildTaskArgument = new GUIContent(
            "Task",
            "The argument key for the build task to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoEnabled" />.
        /// </summary>
        public static readonly GUIContent BuildInfoEnabled = new GUIContent(
            "",
            "During the build process should a BuildInfo be written?");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoNamespace" />.
        /// </summary>
        public static readonly GUIContent BuildInfoNamespace = new GUIContent(
            "Namespace",
            "The namespace where the BuildInfo should be placed.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoPath" />.
        /// </summary>
        public static readonly GUIContent BuildInfoPath = new GUIContent(
            "Output Path",
            "The asset database relative path to output the file.");


        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
        /// </summary>
        public static readonly GUIContent CommandLineParserArgumentPrefix = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        public static readonly GUIContent CommandLineParserArgumentSplit = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a help symbol.
        /// </summary>
        public static readonly GUIContent HelpIcon;

        /// <summary>
        ///     A cached <see cref="GUIContent" /> containing a minus symbol.
        /// </summary>
        public static readonly GUIContent MinusIcon;

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
        ///     Initialize the <see cref="SettingsContent" />.
        /// </summary>
        static SettingsContent()
        {
            if (EditorGUIUtility.isProSkin)
            {
                PlusIcon = EditorGUIUtility.IconContent("Toolbar Plus");
                MinusIcon = EditorGUIUtility.IconContent("Toolbar Minus");
                TestPassedIcon = EditorGUIUtility.IconContent("TestPassed");
                TestNormalIcon = EditorGUIUtility.IconContent("TestNormal");
                HelpIcon = EditorGUIUtility.IconContent("_Help");
            }
            else
            {
                PlusIcon = EditorGUIUtility.IconContent("d_Toolbar Plus");
                MinusIcon = EditorGUIUtility.IconContent("d_Toolbar Minus");
                TestPassedIcon = EditorGUIUtility.IconContent("TestPassed");
                TestNormalIcon = EditorGUIUtility.IconContent("TestNormal");
                HelpIcon = EditorGUIUtility.IconContent("d__Help");
            }

        }
    }
}