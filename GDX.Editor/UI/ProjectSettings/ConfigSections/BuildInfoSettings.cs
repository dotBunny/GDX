// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Editor.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Build Info Settings
    /// </summary>
    internal class BuildInfoSettings : IConfigSection
    {
        private VisualElement _element;

        /// <summary>
        ///     Settings content for <see cref="Config.developerBuildInfoBuildStreamArgument" />.
        /// </summary>
        private readonly GUIContent s_buildStreamArgumentContent = new GUIContent(
            "Stream",
            "The argument key for the build stream to be passed to the BuildInfoProvider.");

        /// <summary>
        ///     Settings content for <see cref="Config.developerBuildInfoBuildTaskArgument" />.
        /// </summary>
        private readonly GUIContent s_buildTaskArgumentContent = new GUIContent(
            "Task",
            "The argument key for the build task to be passed to the BuildInfoProvider.");


        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new BuildInfoSettings());
        }

        /// <summary>
        ///     Draw the Build Info section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="Config" /> object to be modified.</param>
        public void BindSectionContent(VisualElement rootElement)
        {
            // GUI.enabled = true;
            //
            // bool buildInfoEnabled = SettingsGUIUtility.CreateSettingsSection(
            //     SectionID, false,
            //     "BuildInfo Generation", $"{SettingsProvider.DocumentationUri}api/GDX.Editor.Build.BuildInfoProvider.html",
            //     settings.FindProperty("developerBuildInfoEnabled"),
            //     s_enabledContent);
            //
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return;
            // }
            //
            // string buildInfoFile = Path.Combine(Application.dataPath,
            //     settings.FindProperty("developerBuildInfoPath").stringValue);
            // if (!File.Exists(buildInfoFile))
            // {
            //     GUILayout.BeginVertical(SettingsStyles.InfoBoxStyle);
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label(SettingsStyles.NoticeIcon, SettingsStyles.NoHorizontalStretchStyle);
            //     GUILayout.Label(
            //         "There is currently no BuildInfo file in the target location. Would you like some default content written in its place?",
            //         SettingsStyles.WordWrappedLabelStyle);
            //     if (GUILayout.Button("Create Default", SettingsStyles.ButtonStyle))
            //     {
            //         BuildInfoProvider.WriteDefaultFile();
            //         AssetDatabase.ImportAsset("Assets/" +
            //                                   settings.FindProperty("developerBuildInfoPath").stringValue);
            //     }
            //     GUILayout.EndHorizontal();
            //     GUILayout.EndVertical();
            // }
            //
            // // Only allow editing based on the feature being enabled
            // GUI.enabled = buildInfoEnabled;
            //
            // EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoPath"),
            //     s_outputPathContent);
            // EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoNamespace"),
            //     s_namespaceContent);
            // EditorGUILayout.PropertyField(settings.FindProperty("developerBuildInfoAssemblyDefinition"),
            //     s_assemblyDefinitionContent);
            //
            //
            // GUILayout.Space(10);
            // // Arguments (we're going to make sure they are forced to uppercase).
            // GUILayout.Label("Build Arguments", SettingsStyles.SubSectionHeaderTextStyle);
            //
            // SerializedProperty buildNumberProperty =
            //     settings.FindProperty("developerBuildInfoBuildNumberArgument");
            // EditorGUILayout.PropertyField(buildNumberProperty, s_buildNumberArgumentContent);
            // if (buildNumberProperty.stringValue.HasLowerCase())
            // {
            //     buildNumberProperty.stringValue = buildNumberProperty.stringValue.ToUpper();
            // }
            //
            // SerializedProperty buildDescriptionProperty =
            //     settings.FindProperty("developerBuildInfoBuildDescriptionArgument");
            // EditorGUILayout.PropertyField(buildDescriptionProperty,
            //     s_buildDescriptionArgumentContent);
            // if (buildDescriptionProperty.stringValue.HasLowerCase())
            // {
            //     buildDescriptionProperty.stringValue = buildDescriptionProperty.stringValue.ToUpper();
            // }
            //
            // SerializedProperty buildChangelistProperty =
            //     settings.FindProperty("developerBuildInfoBuildChangelistArgument");
            // EditorGUILayout.PropertyField(buildChangelistProperty,
            //     s_buildChangelistArgumentContent);
            // if (buildChangelistProperty.stringValue.HasLowerCase())
            // {
            //     buildChangelistProperty.stringValue = buildChangelistProperty.stringValue.ToUpper();
            // }
            //
            // SerializedProperty buildTaskProperty = settings.FindProperty("developerBuildInfoBuildTaskArgument");
            // EditorGUILayout.PropertyField(buildTaskProperty, s_buildTaskArgumentContent);
            // if (buildTaskProperty.stringValue.HasLowerCase())
            // {
            //     buildTaskProperty.stringValue = buildTaskProperty.stringValue.ToUpper();
            // }
            //
            // SerializedProperty buildStreamProperty =
            //     settings.FindProperty("developerBuildInfoBuildStreamArgument");
            // EditorGUILayout.PropertyField(buildStreamProperty, s_buildStreamArgumentContent);
            // if (buildStreamProperty.stringValue.HasLowerCase())
            // {
            //     buildStreamProperty.stringValue = buildStreamProperty.stringValue.ToUpper();
            // }
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public int GetPriority()
        {
            return 900;
        }
        public string GetSectionHeaderLabel()
        {
            return "BuildInfo Generation";
        }
        public string GetSectionID()
        {
            return "GDX.Editor.Build.BuildInfoProvider";
        }
        public string GetSectionHelpLink()
        {
            return "api/GDX.Editor.Build.BuildInfoProvider.html";
        }

        public bool GetToggleSupport()
        {
            return true;
        }
        public bool GetToggleState()
        {
            return GDX.Editor.UI.SettingsProvider.WorkingConfig.developerBuildInfoEnabled;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            GDX.Editor.UI.SettingsProvider.WorkingConfig.developerBuildInfoEnabled = newState;
            if (Core.Config.developerBuildInfoEnabled != newState)
            {
                toggleElement.AddToClassList(UI.ProjectSettings.ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(UI.ProjectSettings.ConfigSectionsProvider.ChangedClass);
            }
            GDX.Editor.UI.SettingsProvider.CheckForChanges();
        }

        public string GetToggleTooltip()
        {
            return "During the build process should a BuildInfo be written?";
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsBuildInfo";
        }

        public void UpdateSectionContent()
        {

        }
    }
}