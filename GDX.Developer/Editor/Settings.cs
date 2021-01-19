using System.Collections.Generic;
using System.IO;
using GDX.Developer.Editor.Build;
using GDX.Editor;
using UnityEditor;
using UnityEngine;

// ReSharper disable once HeapView.ObjectAllocation.Evident

namespace GDX.Developer.Editor
{
    /// <summary>
    ///     GDX.Developer Assembly Settings
    /// </summary>
    public static class Settings
    {
        /// <summary>
        ///     A list of keywords to flag when searching project settings.
        /// </summary>
        private static readonly HashSet<string> s_settingsKeywords =
            new HashSet<string>(new[] {"gdx", "developer", "parser"});

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoAssemblyDefinition" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoAssemblyDefinition = new GUIContent(
            "Assembly Definition",
            "Ensure that the folder of the BuildInfo has an assembly definition.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentPrefix" />.
        /// </summary>
        private static readonly GUIContent s_contentArgumentPrefix = new GUIContent(
            "Argument Prefix",
            "The prefix used to denote arguments in the command line.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerCommandLineParserArgumentSplit" />.
        /// </summary>
        private static readonly GUIContent s_contentArgumentSplit = new GUIContent(
            "Argument Split",
            "The string used to split arguments from their values.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoEnabled" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoEnabled = new GUIContent(
            "",
            "During the build process should a BuildInfo be written?");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoPath" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoPath = new GUIContent(
            "Output Path",
            "The asset database relative path to output the file.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoNamespace" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoNamespace = new GUIContent(
            "Namespace",
            "The namespace where the BuildInfo should be placed.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildNumberArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildNumberArgument = new GUIContent(
            "Number",
            "The argument key for the build number to be passed to the BuildInfoGenerator.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildDescriptionArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildDescriptionArgument = new GUIContent(
            "Description",
            "The argument key for the build description to be passed to the BuildInfoGenerator.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildChangelistArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildChangelistArgument = new GUIContent(
            "Changelist",
            "The argument key for the build changelist to be passed to the BuildInfoGenerator.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildTaskArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildTaskArgument = new GUIContent(
            "Task",
            "The argument key for the build task to be passed to the BuildInfoGenerator.");

        /// <summary>
        ///     Settings content for <see cref="GDXConfig.developerBuildInfoBuildStreamArgument" />.
        /// </summary>
        private static readonly GUIContent s_contentBuildInfoBuildStreamArgument = new GUIContent(
            "Stream",
            "The argument key for the build stream to be passed to the BuildInfoGenerator.");

        /// <summary>
        ///     Get <see cref="SettingsProvider" /> for GDX.Developer assembly.
        /// </summary>
        /// <returns>A provider for project settings.</returns>
        [SettingsProvider]
        public static SettingsProvider SettingsProvider()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            return new SettingsProvider("Project/GDX/Developer", SettingsScope.Project)
            {
                label = "Developer",
                guiHandler = searchContext =>
                {
                    SerializedObject settings = ConfigProvider.GetSerializedConfig();
                    SettingsGUILayout.BeginGUILayout();

                    #region BuildInfo Generation

                    bool buildInfoEnabled = SettingsGUILayout.SectionHeader(
                        "BuildInfo Generation",
                        settings.FindProperty("developerBuildInfoEnabled"),
                        s_contentBuildInfoEnabled);

                    string buildInfoFile = Path.Combine(Application.dataPath,
                        settings.FindProperty("developerBuildInfoPath").stringValue);
                    if (!File.Exists(buildInfoFile))
                    {
                        GUILayout.BeginVertical(SettingsGUILayout.InfoBoxStyle);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(
                            "There is currently no BuildInfo file in the target location\nWould you like some default content written in its place?");
                        if (GUILayout.Button("Create Default", SettingsGUILayout.ButtonStyle))
                        {
                            BuildInfoProvider.WriteDefaultFile();
                            AssetDatabase.ImportAsset("Assets/" +
                                                      settings.FindProperty("developerBuildInfoPath").stringValue);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }

                    // Only allow editing based on the feature being enabled
                    GUI.enabled = buildInfoEnabled;

                    EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoPath"),
                        s_contentBuildInfoPath);
                    EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoNamespace"),
                        s_contentBuildInfoNamespace);
                    EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoAssemblyDefinition"),
                        s_contentBuildInfoAssemblyDefinition);


                    GUILayout.Space(10);
                    // Arguments (we're going to make sure they are forced to uppercase).
                    GUILayout.Label("Build Arguments", SettingsGUILayout.H2Style);

                    SerializedProperty buildNumberProperty =
                        settings.FindProperty("developerBuildInfoBuildNumberArgument");
                    EditorGUILayout.PropertyField(buildNumberProperty, s_contentBuildInfoBuildNumberArgument);
                    if (buildNumberProperty.stringValue.HasLowerCase())
                    {
                        buildNumberProperty.stringValue = buildNumberProperty.stringValue.ToUpper();
                    }

                    SerializedProperty buildDescriptionProperty =
                        settings.FindProperty("developerBuildInfoBuildDescriptionArgument");
                    EditorGUILayout.PropertyField(buildDescriptionProperty, s_contentBuildInfoBuildDescriptionArgument);
                    if (buildDescriptionProperty.stringValue.HasLowerCase())
                    {
                        buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
                    }

                    SerializedProperty buildChangelistProperty =
                        settings.FindProperty("developerBuildInfoBuildChangelistArgument");
                    EditorGUILayout.PropertyField(buildChangelistProperty, s_contentBuildInfoBuildChangelistArgument);
                    if (buildChangelistProperty.stringValue.HasLowerCase())
                    {
                        buildChangelistProperty.stringValue = buildChangelistProperty.stringValue.ToUpper();
                    }

                    SerializedProperty buildTaskProperty = settings.FindProperty("developerBuildInfoBuildTaskArgument");
                    EditorGUILayout.PropertyField(buildTaskProperty, s_contentBuildInfoBuildTaskArgument);
                    if (buildTaskProperty.stringValue.HasLowerCase())
                    {
                        buildTaskProperty.stringValue = buildTaskProperty.stringValue.ToUpper();
                    }

                    SerializedProperty buildStreamProperty =
                        settings.FindProperty("developerBuildInfoBuildStreamArgument");
                    EditorGUILayout.PropertyField(buildStreamProperty, s_contentBuildInfoBuildStreamArgument);
                    if (buildStreamProperty.stringValue.HasLowerCase())
                    {
                        buildStreamProperty.stringValue = buildStreamProperty.stringValue.ToUpper();
                    }

                    // Restore regardless
                    GUI.enabled = true;

                    #endregion

                    // Create some distance between next section
                    GUILayout.Space(10);

                    #region Command Line Parser

                    SettingsGUILayout.SectionHeader("Command Line Parser");

                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentPrefix"),
                        s_contentArgumentPrefix);
                    EditorGUILayout.PropertyField(settings.FindProperty("developerCommandLineParserArgumentSplit"),
                        s_contentArgumentSplit);

                    #endregion

                    SettingsGUILayout.EndGUILayout();
                    settings.ApplyModifiedPropertiesWithoutUndo();
                },
                keywords = s_settingsKeywords
            };
        }
    }
}