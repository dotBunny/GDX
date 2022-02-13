// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SettingsProvider = GDX.Editor.UI.SettingsProvider;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Automatic Updates Settings
    /// </summary>
    internal class AutomaticUpdatesSettings : IConfigSection
    {
        public const string SectionID = "GDX.Editor.UpdateProvider";

        private VisualElement m_RootElement;
        private VisualElement m_ElementLocalVersion;
        private Label m_LabelLocalVersion;
        private VisualElement m_ElementInstallationMethod;
        private Label m_LabelInstallationMethod;
        private VisualElement m_ElementSource;
        private Label m_LabelSourceItem;
        private Label m_LabelSourceData;
        private VisualElement m_ElementRemoteVersion;
        private Label m_LabelRemoteVersion;
        private VisualElement m_ElementLastChecked;
        private Label m_LabelLastChecked;
        private Button m_ButtonManualUpgrade;
        private Button m_ButtonChangeLog;
        private Button m_ButtonUpdate;
        private Button m_ButtonManualCheck;

        private SliderInt m_SliderUpdateTime;

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
                case PackageProvider.InstallationType.UPMBranch:
                case PackageProvider.InstallationType.GitHubBranch:
                case PackageProvider.InstallationType.GitHub:
                    m_ElementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    m_LabelSourceItem.text = "Branch:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.UPMTag:
                case PackageProvider.InstallationType.GitHubTag:
                    m_ElementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    m_LabelSourceItem.text = "Tag:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.UPMCommit:
                case PackageProvider.InstallationType.GitHubCommit:
                    m_ElementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    m_LabelSourceItem.text = "Commit:";
                    m_LabelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                default:
                    m_ElementSource.AddToClassList(ConfigSectionsProvider.HiddenClass);
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
                m_ElementRemoteVersion.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                m_LabelRemoteVersion.text = UpdateProvider.UpdatePackageDefinition.version;
            }
            else
            {
                m_ElementRemoteVersion.AddToClassList(ConfigSectionsProvider.HiddenClass);
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
        public string GetSectionID()
        {
            return SectionID;
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
            return SettingsProvider.WorkingConfig.UpdateProviderCheckForUpdates;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            SettingsProvider.WorkingConfig.UpdateProviderCheckForUpdates = newState;
            if (Core.Config.UpdateProviderCheckForUpdates != newState)
            {
                toggleElement.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }
            SettingsProvider.CheckForChanges();
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
                m_ButtonManualCheck.AddToClassList(ConfigSectionsProvider.HiddenClass);
                m_ButtonManualUpgrade.AddToClassList(ConfigSectionsProvider.HiddenClass);

                m_ButtonChangeLog.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);

                if (UpdateProvider.IsUpgradable())
                {
                    m_ButtonUpdate.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                }
                else
                {
                    m_ButtonUpdate.AddToClassList(ConfigSectionsProvider.HiddenClass);
                }
            }
            else
            {
                m_ButtonChangeLog.AddToClassList(ConfigSectionsProvider.HiddenClass);
                m_ButtonUpdate.AddToClassList(ConfigSectionsProvider.HiddenClass);

                m_ButtonManualCheck.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);

                if ((UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.GitHubBranch ||
                     UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.UPMBranch) &&
                    UpdateProvider.LocalPackage.SourceTag == "dev")
                {
                    m_ButtonManualUpgrade.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                }
                else
                {
                    m_ButtonManualUpgrade.AddToClassList(ConfigSectionsProvider.HiddenClass);
                }

            }
        }
    }
}