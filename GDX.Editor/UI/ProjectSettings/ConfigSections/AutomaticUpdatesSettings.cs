// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Automatic Updates Settings
    /// </summary>
    internal class AutomaticUpdatesSettings : IConfigSection
    {
        /// <summary>
        ///     Internal section identifier.
        /// </summary>
        private const string SectionID = "GDX.Editor.UpdateProvider";

        /// <summary>
        ///     Settings content for <see cref="Config.updateProviderCheckForUpdates" />.
        /// </summary>
        private readonly GUIContent s_enabledContent = new GUIContent(
            "",
            "Should the package check the GitHub repository to see if there is a new version?");

        /// <summary>
        ///     Settings content for <see cref="AutomaticUpdatesSettings.UpdateDayCountSetting" />.
        /// </summary>
        private readonly GUIContent s_updateDayCountContent = new GUIContent(
            "Update Timer (Days)",
            "After how many days should updates be checked for?");

        /// <summary>
        ///     The number of days between checks for updates.
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        public static int UpdateDayCountSetting
        {
            get => EditorPrefs.GetInt("GDX.UpdateProvider.UpdateDayCount", 7);
            private set => EditorPrefs.SetInt("GDX.UpdateProvider.UpdateDayCount", value);
        }

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new AutomaticUpdatesSettings());
        }

        /// <summary>
        ///     Draw the Automatic Updates section of settings.
        /// </summary>
        /// <param name="settings">Serialized <see cref="Config" /> object to be modified.</param>
        public void DrawSectionContent(GDXConfig settings)
        {
            // GUI.enabled = true;
            //
            // bool packageSectionEnabled = SettingsGUIUtility.CreateSettingsSection(
            //     SectionID, true,
            //     "Automatic Package Updates", null,
            //     settings.FindProperty("updateProviderCheckForUpdates"),
            //     s_enabledContent);
            // if (!SettingsGUIUtility.GetCachedEditorBoolean(SectionID))
            // {
            //     return;
            // }
            //
            // EditorGUILayout.BeginHorizontal(SettingsStyles.InfoBoxStyle);
            // if (UpdateProvider.LocalPackage.Definition != null)
            // {
            //     GUILayout.BeginVertical();
            //
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label("Local Version:", EditorStyles.boldLabel,
            //         SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //     GUILayout.Label(UpdateProvider.LocalPackage.Definition.version);
            //     GUILayout.FlexibleSpace();
            //     GUILayout.EndHorizontal();
            //
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label("Installation Method:", EditorStyles.boldLabel,
            //         SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //     GUILayout.Label(PackageProvider.GetFriendlyName(UpdateProvider.LocalPackage.InstallationMethod));
            //     GUILayout.FlexibleSpace();
            //     GUILayout.EndHorizontal();
            //
            //     // Handle additional information
            //     switch (UpdateProvider.LocalPackage.InstallationMethod)
            //     {
            //         case PackageProvider.InstallationType.UPMBranch:
            //         case PackageProvider.InstallationType.GitHubBranch:
            //         case PackageProvider.InstallationType.GitHub:
            //             GUILayout.BeginHorizontal();
            //             GUILayout.Label("Source Branch:", EditorStyles.boldLabel,
            //                 SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //             GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
            //                 ? UpdateProvider.LocalPackage.SourceTag
            //                 : "N/A");
            //             GUILayout.FlexibleSpace();
            //             GUILayout.EndHorizontal();
            //             break;
            //         case PackageProvider.InstallationType.UPMTag:
            //         case PackageProvider.InstallationType.GitHubTag:
            //             GUILayout.BeginHorizontal();
            //             GUILayout.Label("Source Tag:", EditorStyles.boldLabel,
            //                 SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //             GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
            //                 ? UpdateProvider.LocalPackage.SourceTag
            //                 : "N/A");
            //             GUILayout.FlexibleSpace();
            //             GUILayout.EndHorizontal();
            //             break;
            //         case PackageProvider.InstallationType.UPMCommit:
            //         case PackageProvider.InstallationType.GitHubCommit:
            //             GUILayout.BeginHorizontal();
            //             GUILayout.Label("Source Commit:", EditorStyles.boldLabel,
            //                 SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //             GUILayout.Label(!string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
            //                 ? UpdateProvider.LocalPackage.SourceTag
            //                 : "N/A");
            //             GUILayout.FlexibleSpace();
            //             GUILayout.EndHorizontal();
            //             break;
            //     }
            //
            //     // Show remote version if we have something to show
            //     if (UpdateProvider.UpdatePackageDefinition != null)
            //     {
            //         GUILayout.BeginHorizontal();
            //         GUILayout.Label("Remote Version:", EditorStyles.boldLabel,
            //             SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //         GUILayout.Label(UpdateProvider.UpdatePackageDefinition.version);
            //         GUILayout.FlexibleSpace();
            //         GUILayout.EndHorizontal();
            //     }
            //
            //     GUILayout.BeginHorizontal();
            //     GUILayout.Label("Last Checked:", EditorStyles.boldLabel, SettingsLayoutOptions.FixedWidth130LayoutOptions);
            //     GUILayout.Label(UpdateProvider.GetLastChecked().ToString(GDX.Localization.LocalTimestampFormat));
            //     GUILayout.FlexibleSpace();
            //     GUILayout.EndHorizontal();
            //
            //     GUILayout.EndVertical();
            //
            //     // Force things to the right
            //     GUILayout.FlexibleSpace();
            //
            //     EditorGUILayout.BeginVertical();
            //     if (UpdateProvider.HasUpdate(UpdateProvider.UpdatePackageDefinition))
            //     {
            //         if (GUILayout.Button("Changelog", SettingsStyles.ButtonStyle))
            //         {
            //             Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
            //         }
            //
            //         if (UpdateProvider.IsUpgradable())
            //         {
            //             if (GUILayout.Button("Update", SettingsStyles.ButtonStyle))
            //             {
            //                 UpdateProvider.AttemptUpgrade();
            //             }
            //         }
            //     }
            //     else
            //     {
            //         if (GUILayout.Button("Manual Check", SettingsStyles.ButtonStyle))
            //         {
            //             UpdateProvider.CheckForUpdates();
            //         }
            //
            //         // Special allowance to force pull dev branch to avoid having to increment the version code.
            //         if ((UpdateProvider.LocalPackage.InstallationMethod ==
            //              PackageProvider.InstallationType.GitHubBranch ||
            //              UpdateProvider.LocalPackage.InstallationMethod ==
            //              PackageProvider.InstallationType.UPMBranch) &&
            //             UpdateProvider.LocalPackage.SourceTag == "dev")
            //         {
            //             if (GUILayout.Button("Force Upgrade", SettingsStyles.ButtonStyle))
            //             {
            //                 UpdateProvider.AttemptUpgrade(true);
            //             }
            //         }
            //     }
            //
            //     EditorGUILayout.EndVertical();
            // }
            // else
            // {
            //     GUILayout.Label(
            //         $"An error occured trying to find the package definition.\nPresumed Root: {UpdateProvider.LocalPackage.PackageAssetPath}\nPresumed Manifest:{UpdateProvider.LocalPackage.PackageManifestPath})",
            //         EditorStyles.boldLabel);
            // }
            //
            // EditorGUILayout.EndHorizontal();
            //
            // // Disable based on if we have this enabled
            // GUI.enabled = packageSectionEnabled;
            //
            // UpdateDayCountSetting =
            //     EditorGUILayout.IntSlider(s_updateDayCountContent, UpdateDayCountSetting, 1,
            //         31);
        }

        public void DrawSectionHeader(GDXConfig config)
        {

        }


        public bool GetDefaultVisibility()
        {
            return true;
        }
        public string GetSectionHeaderLabel()
        {
            return "Automatic Package Updates";
        }
        public string GetSectionID()
        {
            return "GDX.Editor.UpdateProvider";
        }

        public string GetSectionHelpLink()
        {
            return null;
        }

        public bool GetToggleSupport()
        {
            return true;
        }

        public bool GetToggleState()
        {
            return false;
        }

        public void SetToggleState(bool newState)
        {

        }
    }
}