// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using GDX.Editor;
using GDX.Editor.UI;
using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using SettingsProvider = UnityEditor.SettingsProvider;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Automatic Updates Settings
    /// </summary>
    internal class AutomaticUpdatesSettings : IConfigSection
    {
        private VisualElement _rootElement;
        private VisualElement _elementLocalVersion;
        private Label _labelLocalVersion;
        private VisualElement _elementInstallationMethod;
        private Label _labelInstallationMethod;
        private VisualElement _elementSource;
        private Label _labelSourceItem;
        private Label _labelSourceData;
        private VisualElement _elementRemoteVersion;
        private Label _labelRemoteVersion;
        private VisualElement _elementLastChecked;
        private Label _labelLastChecked;
        private Button _buttonManualUpgrade;
        private Button _buttonChangeLog;
        private Button _buttonUpdate;
        private Button _buttonManualCheck;

        private SliderInt _sliderUpdateTime;

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
        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            _elementLocalVersion = _rootElement.Q<VisualElement>("gdx-automatic-update-local-version");
            _labelLocalVersion = _elementLocalVersion.Q<Label>("label-data");
            _elementInstallationMethod = _rootElement.Q<VisualElement>("gdx-automatic-update-installation-method");
            _labelInstallationMethod = _elementInstallationMethod.Q<Label>("label-data");

            _elementSource = _rootElement.Q<VisualElement>("gdx-automatic-update-source");
            _labelSourceItem = _elementSource.Q<Label>("label-item");
            _labelSourceData = _elementSource.Q<Label>("label-data");

            _elementRemoteVersion = _rootElement.Q<VisualElement>("gdx-automatic-update-remote-version");
            _labelRemoteVersion = _elementRemoteVersion.Q<Label>("label-data");

            _elementLastChecked = _rootElement.Q<VisualElement>("gdx-automatic-update-last-checked");
            _labelLastChecked = _elementLastChecked.Q<Label>("label-data");

            _buttonUpdate = _rootElement.Q<Button>("button-update");
            _buttonChangeLog = _rootElement.Q<Button>("button-changelog");
            _buttonManualUpgrade = _rootElement.Q<Button>("button-manual-upgrade");
            _buttonManualCheck = _rootElement.Q<Button>("button-manual-check");

            _sliderUpdateTime = _rootElement.Q<SliderInt>("gdx-slider-update-time");
            _sliderUpdateTime.value = UpdateDayCountSetting;
            _sliderUpdateTime.RegisterValueChangedCallback(evt =>
            {
                UpdateDayCountSetting = evt.newValue;
            });


            if (UpdateProvider.LocalPackage.Definition != null)
            {
                _labelLocalVersion.text = UpdateProvider.LocalPackage.Definition.version;
                _labelInstallationMethod.text =
                    PackageProvider.GetFriendlyName(UpdateProvider.LocalPackage.InstallationMethod);

                switch (UpdateProvider.LocalPackage.InstallationMethod)
                {
                case PackageProvider.InstallationType.UPMBranch:
                case PackageProvider.InstallationType.GitHubBranch:
                case PackageProvider.InstallationType.GitHub:
                    _elementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    _labelSourceItem.text = "Branch:";
                    _labelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.UPMTag:
                case PackageProvider.InstallationType.GitHubTag:
                    _elementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    _labelSourceItem.text = "Tag:";
                    _labelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                case PackageProvider.InstallationType.UPMCommit:
                case PackageProvider.InstallationType.GitHubCommit:
                    _elementSource.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                    _labelSourceItem.text = "Commit:";
                    _labelSourceData.text = !string.IsNullOrEmpty(UpdateProvider.LocalPackage.SourceTag)
                        ? UpdateProvider.LocalPackage.SourceTag
                        : "N/A";
                    break;
                default:
                    _elementSource.AddToClassList(ConfigSectionsProvider.HiddenClass);
                    break;
                }
            }
            else
            {
                _labelLocalVersion.text = "N/A";
                _labelInstallationMethod.text = "N/A";
                _labelSourceItem.text = "Branch:";
                _labelSourceData.text = "N/A";
            }

            if (UpdateProvider.UpdatePackageDefinition != null)
            {
                _elementRemoteVersion.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                _labelRemoteVersion.text = UpdateProvider.UpdatePackageDefinition.version;
            }
            else
            {
                _elementRemoteVersion.AddToClassList(ConfigSectionsProvider.HiddenClass);
                _labelRemoteVersion.text = "N/A";
                _labelLastChecked.text = "N/A";
            }

            _labelLastChecked.text = UpdateProvider.GetLastChecked().ToString(Localization.LocalTimestampFormat);

            _buttonChangeLog.clicked += () =>
            {
                Application.OpenURL("https://github.com/dotBunny/GDX/blob/main/CHANGELOG.md");
            };
            _buttonUpdate.clicked += () =>
            {
                UpdateProvider.AttemptUpgrade();
            };
            _buttonManualCheck.clicked += UpdateProvider.CheckForUpdates;
            _buttonManualUpgrade.clicked += () =>
            {
                UpdateProvider.AttemptUpgrade(true);
            };





            //     EditorGUILayout.EndVertical();
            // }
            // else
            // {
            //     GUILayout.Label(
            //         $"An error occured trying to find the package definition.\nPresumed Root: {UpdateProvider.LocalPackage.PackageAssetPath}\nPresumed Manifest:{UpdateProvider.LocalPackage.PackageManifestPath})",
            //         EditorStyles.boldLabel);
            // }

        }

        public bool GetDefaultVisibility()
        {
            return true;
        }
        public int GetPriority()
        {
            return 1000;
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
            return GDX.Editor.UI.SettingsProvider.WorkingConfig.updateProviderCheckForUpdates;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            GDX.Editor.UI.SettingsProvider.WorkingConfig.updateProviderCheckForUpdates = newState;
            if (Core.Config.updateProviderCheckForUpdates != newState)
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
                _buttonManualCheck.AddToClassList(ConfigSectionsProvider.HiddenClass);
                _buttonManualUpgrade.AddToClassList(ConfigSectionsProvider.HiddenClass);

                _buttonChangeLog.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);

                if (UpdateProvider.IsUpgradable())
                {
                    _buttonUpdate.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                }
                else
                {
                    _buttonUpdate.AddToClassList(ConfigSectionsProvider.HiddenClass);
                }
            }
            else
            {
                _buttonChangeLog.AddToClassList(ConfigSectionsProvider.HiddenClass);
                _buttonUpdate.AddToClassList(ConfigSectionsProvider.HiddenClass);

                _buttonManualCheck.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);

                if ((UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.GitHubBranch ||
                     UpdateProvider.LocalPackage.InstallationMethod ==
                     PackageProvider.InstallationType.UPMBranch) &&
                    UpdateProvider.LocalPackage.SourceTag == "dev")
                {
                    _buttonManualUpgrade.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
                }
                else
                {
                    _buttonManualUpgrade.AddToClassList(ConfigSectionsProvider.HiddenClass);
                }

            }
        }
    }
}