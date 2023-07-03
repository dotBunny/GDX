// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings.ConfigSections
{
    /// <summary>
    ///     Automatic Updates Settings
    /// </summary>
    class AutomaticUpdatesSettings : IConfigSection
    {
        public const int SectionIndex = 0;
        public const string SectionKey = "GDX.Editor.UpdateProvider";
        static readonly string[] k_Keywords = { "git", "upm", "automatic", "updates", "package", "gdx" };

        VisualElement m_RootElement;
        VisualElement m_ElementLocalVersion;
        Label m_LabelLocalVersion;
        VisualElement m_ElementInstallationMethod;
        Label m_LabelInstallationMethod;
        VisualElement m_ElementSource;
        Label m_LabelSourceItem;
        Label m_LabelSourceData;
        VisualElement m_ElementRemoteVersion;
        Label m_LabelRemoteVersion;
        VisualElement m_ElementLastChecked;
        Label m_LabelLastChecked;
        Button m_ButtonManualUpgrade;
        Button m_ButtonChangeLog;
        Button m_ButtonUpdate;
        Button m_ButtonManualCheck;
        SliderInt m_SliderUpdateTime;

        /// <summary>
        ///     The number of days between checks for updates.
        /// </summary>
        /// <remarks>We use a property over methods in this case so that Unity's UI can be easily tied to this value.</remarks>
        public static int UpdateDayCountSetting
        {
            get => EditorPrefs.GetInt("GDX.UpdateProvider.UpdateDayCount", 7);
            private set => EditorPrefs.SetInt("GDX.UpdateProvider.UpdateDayCount", value);
        }

        /// <summary>
        ///     Bind the Automatic Update section.
        /// </summary>
        /// <param name="rootElement">The Automatic Update section <see cref="VisualElement"/>.</param>
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_ElementLocalVersion = m_RootElement.Q<VisualElement>("gdx-automatic-update-local-version");
            m_LabelLocalVersion = m_ElementLocalVersion.Q<Label>("label-data");
            m_ElementInstallationMethod = m_RootElement.Q<VisualElement>("gdx-automatic-update-installation-method");
            m_LabelInstallationMethod = m_ElementInstallationMethod.Q<Label>("label-data");

            m_ElementSource = m_RootElement.Q<VisualElement>("gdx-automatic-update-source");
            m_LabelSourceItem = m_ElementSource.Q<Label>("label-item");
            m_LabelSourceData = m_ElementSource.Q<Label>("label-data");

            m_ElementRemoteVersion = m_RootElement.Q<VisualElement>("gdx-automatic-update-remote-version");
            m_LabelRemoteVersion = m_ElementRemoteVersion.Q<Label>("label-data");

            m_ElementLastChecked = m_RootElement.Q<VisualElement>("gdx-automatic-update-last-checked");
            m_LabelLastChecked = m_ElementLastChecked.Q<Label>("label-data");

            m_ButtonUpdate = m_RootElement.Q<Button>("button-update");
            m_ButtonChangeLog = m_RootElement.Q<Button>("button-changelog");
            m_ButtonManualUpgrade = m_RootElement.Q<Button>("button-manual-upgrade");
            m_ButtonManualCheck = m_RootElement.Q<Button>("button-manual-check");

            m_SliderUpdateTime = m_RootElement.Q<SliderInt>("gdx-slider-update-time");
            m_SliderUpdateTime.value = UpdateDayCountSetting;
            m_SliderUpdateTime.RegisterValueChangedCallback(evt =>
            {
                UpdateDayCountSetting = evt.newValue;
            });


            if (UpdateProvider.LocalPackage.Definition != null)
            {
                m_LabelLocalVersion.text = UpdateProvider.LocalPackage.Definition.version;
                m_LabelInstallationMethod.text =
                    PackageProvider.GetFriendlyName(UpdateProvider.LocalPackage.InstallationMethod);

                switch (UpdateProvider.LocalPackage.InstallationMethod)
                {
                case PackageProvider.InstallationType.PackageManagerBranch:
                case PackageProvider.InstallationType.GitHubBranch:
                case PackageProvider.InstallationType.GitHub:
                    m_ElementSource.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_LabelSourceItem.text = "Branch:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.PackageManagerTag:
                case PackageProvider.InstallationType.GitHubTag:
                    m_ElementSource.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_LabelSourceItem.text = "Tag:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.PackageManagerCommit:
                case PackageProvider.InstallationType.GitHubCommit:
                    m_ElementSource.RemoveFromClassList(ResourcesProvider.HiddenClass);
                    m_LabelSourceItem.text = "Commit:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                default:
                    m_ElementSource.AddToClassList(ResourcesProvider.HiddenClass);
                    break;
                }
            }
            else
            {
                m_LabelLocalVersion.text = "N/A";
                m_LabelInstallationMethod.text = "N/A";
                m_LabelSourceItem.text = "Branch:";
                m_LabelSourceData.text = "N/A";
            }

            if (UpdateProvider.UpdatePackageDefinition != null)
            {
                m_ElementRemoteVersion.RemoveFromClassList(ResourcesProvider.HiddenClass);
                m_LabelRemoteVersion.text = UpdateProvider.UpdatePackageDefinition.version;
            }
            else
            {
                m_ElementRemoteVersion.AddToClassList(ResourcesProvider.HiddenClass);
                m_LabelRemoteVersion.text = "N/A";
                m_LabelLastChecked.text = "N/A";
            }

            m_LabelLastChecked.text = UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat);

            m_ButtonChangeLog.clicked += () =>
            {
                Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
            };
            m_ButtonUpdate.clicked += () =>
            {
                UpdateProvider.AttemptUpgrade();
            };
            m_ButtonManualCheck.clicked += UpdateProvider.CheckForUpdates;
            m_ButtonManualUpgrade.clicked += () =>
            {
                UpdateProvider.AttemptUpgrade(true);
            };
        }

        public bool GetDefaultVisibility()
        {
            return true;
        }

        public string GetSectionHeaderLabel()
        {
            return "Automatic Package Updates";
        }

        public int GetSectionIndex()
        {
            return SectionIndex;
        }

        public string GetSectionKey()
        {
            return SectionKey;
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
            return ProjectSettingsProvider.WorkingConfig.UpdateProviderCheckForUpdates;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            ProjectSettingsProvider.WorkingConfig.UpdateProviderCheckForUpdates = newState;
            if (Config.UpdateProviderCheckForUpdates != newState)
            {
                toggleElement.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }
            ProjectSettingsProvider.UpdateForChanges();
        }

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetToggleTooltip()
        {
            return "Should the package check the GitHub repository to see if there is a new version?";
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsAutomaticUpdates";
        }

        public void UpdateSectionContent()
        {
            if (UpdateProvider.HasUpdate(UpdateProvider.UpdatePackageDefinition))
            {
                m_ButtonManualCheck.AddToClassList(ResourcesProvider.HiddenClass);
                m_ButtonManualUpgrade.AddToClassList(ResourcesProvider.HiddenClass);

                m_ButtonChangeLog.RemoveFromClassList(ResourcesProvider.HiddenClass);

                if (UpdateProvider.IsUpgradable())
                {
                    m_ButtonUpdate.RemoveFromClassList(ResourcesProvider.HiddenClass);
                }
                else
                {
                    m_ButtonUpdate.AddToClassList(ResourcesProvider.HiddenClass);
                }
            }
            else
            {
                m_ButtonChangeLog.AddToClassList(ResourcesProvider.HiddenClass);
                m_ButtonUpdate.AddToClassList(ResourcesProvider.HiddenClass);

                m_ButtonManualCheck.RemoveFromClassList(ResourcesProvider.HiddenClass);

                if ((UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.GitHubBranch ||
                     UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.PackageManagerBranch) &&
                    UpdateProvider.LocalPackage.SourceTag == "dev")
                {
                    m_ButtonManualUpgrade.RemoveFromClassList(ResourcesProvider.HiddenClass);
                }
                else
                {
                    m_ButtonManualUpgrade.AddToClassList(ResourcesProvider.HiddenClass);
                }

            }
        }
    }
}