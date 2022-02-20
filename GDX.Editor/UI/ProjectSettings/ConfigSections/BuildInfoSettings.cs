// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Toggle = UnityEngine.UIElements.Toggle;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Build Info Settings
    /// </summary>
    // ReSharper disable once UnusedType.Global
    internal class BuildInfoSettings : IConfigSection
    {
        public const string SectionID = "GDX.Editor.Build.BuildInfoProvider";
        private VisualElement m_RootElement;
        private VisualElement m_Content;
        private TextField m_TextOutputPath;
        private TextField m_TextNamespace;
        private Toggle m_ToggleAssemblyDefinition;
        private TextField m_TextNumber;
        private TextField m_TextDescription;
        private TextField m_TextChangelist;
        private TextField m_TextTask;
        private TextField m_TextStream;
        private Button m_ButtonCreate;
        private VisualElement m_Notice;

        /// <summary>
        ///     Bind the Build Info content.
        /// </summary>
        /// <param name="rootElement">The Build Info section <see cref="VisualElement"/>.</param>
        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;
            m_Content = m_RootElement.Q<VisualElement>("gdx-build-info-content");
            m_Notice = m_RootElement.Q<VisualElement>("notice");


            m_ButtonCreate = m_RootElement.Q<Button>("button-create-default");
            if (m_ButtonCreate != null)
            {
                m_ButtonCreate.clicked += () =>
                {
                    Build.BuildInfoProvider.WriteDefaultFile();
                    AssetDatabase.ImportAsset("Assets/" + Core.Config.DeveloperBuildInfoPath);
                };
            }


            m_TextOutputPath = m_RootElement.Q<TextField>("text-output-path");
            m_TextOutputPath.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoPath;
            m_TextOutputPath.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoPath = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextOutputPath.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextOutputPath.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });


            m_TextNamespace = m_RootElement.Q<TextField>("text-namespace");
            m_TextNamespace.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace;
            m_TextNamespace.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace = evt.newValue;
                if (Core.Config.DeveloperBuildInfoNamespace != evt.newValue)
                {
                    m_TextNamespace.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextNamespace.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_ToggleAssemblyDefinition = m_RootElement.Q<Toggle>("toggle-assembly-definition");
            m_ToggleAssemblyDefinition.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition;
            m_ToggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition = evt.newValue;
                if (Core.Config.DeveloperBuildInfoAssemblyDefinition != evt.newValue)
                {
                    m_ToggleAssemblyDefinition.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleAssemblyDefinition.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });


            m_TextNumber = m_RootElement.Q<TextField>("text-number");
            m_TextNumber.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument;
            m_TextNumber.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildNumberArgument != evt.newValue)
                {
                    m_TextNumber.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextNumber.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_TextDescription = m_RootElement.Q<TextField>("text-description");
            m_TextDescription.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument;
            m_TextDescription.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildDescriptionArgument != evt.newValue)
                {
                    m_TextDescription.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextDescription.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_TextChangelist = m_RootElement.Q<TextField>("text-changelist");
            m_TextChangelist.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument;
            m_TextChangelist.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildChangelistArgument != evt.newValue)
                {
                    m_TextChangelist.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextChangelist.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_TextTask = m_RootElement.Q<TextField>("text-task");
            m_TextTask.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument;
            m_TextTask.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildTaskArgument != evt.newValue)
                {
                    m_TextTask.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextTask.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_TextStream = m_RootElement.Q<TextField>("text-stream");
            m_TextStream.value = UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument;
            m_TextStream.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildStreamArgument != evt.newValue)
                {
                    m_TextStream.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextStream.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            CheckForDisabledContent();
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }

        public string GetSectionHeaderLabel()
        {
            return "BuildInfo Generation";
        }
        public string GetSectionID()
        {
            return SectionID;
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
            return UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled = newState;
            if (Core.Config.DeveloperBuildInfoEnabled != newState)
            {
                toggleElement.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }

            CheckForDisabledContent();
            UI.SettingsProvider.CheckForChanges();
        }

        public string GetToggleTooltip()
        {
            return "During the build process should a BuildInfo be written?";
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsBuildInfo";
        }

        private void CheckForDisabledContent()
        {
            m_Content?.SetEnabled(GetToggleState());
            if (File.Exists(Path.Combine(Application.dataPath, Core.Config.DeveloperBuildInfoPath)))
            {
                m_Notice.AddToClassList(ConfigSectionsProvider.HiddenClass);
            }
            else
            {
                m_Notice.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
            }
        }

        public void UpdateSectionContent()
        {
            ConfigSectionsProvider.SetClassChangeCheck(m_TextOutputPath,
                Core.Config.DeveloperBuildInfoPath,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoPath);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNamespace,
                Core.Config.DeveloperBuildInfoNamespace,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleAssemblyDefinition,
                Core.Config.DeveloperBuildInfoAssemblyDefinition,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNumber,
                Core.Config.DeveloperBuildInfoBuildNumberArgument,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextDescription,
                Core.Config.DeveloperBuildInfoBuildDescriptionArgument,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextChangelist,
                Core.Config.DeveloperBuildInfoBuildChangelistArgument,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextTask,
                Core.Config.DeveloperBuildInfoBuildTaskArgument,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextStream,
                Core.Config.DeveloperBuildInfoBuildStreamArgument,
                UI.SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument);
        }
    }
}