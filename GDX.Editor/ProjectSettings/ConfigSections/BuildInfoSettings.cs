// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
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
    class BuildInfoSettings : IConfigSection
    {
        public const string SectionKey = "GDX.Editor.Build.BuildInfoProvider";
        VisualElement m_RootElement;
        VisualElement m_Content;
        TextField m_TextOutputPath;
        TextField m_TextNamespace;
        Toggle m_ToggleAssemblyDefinition;
        TextField m_TextNumber;
        TextField m_TextDescription;
        TextField m_TextChangelist;
        TextField m_TextTask;
        TextField m_TextStream;
        Button m_ButtonCreate;
        VisualElement m_Notice;

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
            m_TextOutputPath.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoPath;
            m_TextOutputPath.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoPath = evt.newValue;
                if (Core.Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextOutputPath.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextOutputPath.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });


            m_TextNamespace = m_RootElement.Q<TextField>("text-namespace");
            m_TextNamespace.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace;
            m_TextNamespace.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace = evt.newValue;
                if (Core.Config.DeveloperBuildInfoNamespace != evt.newValue)
                {
                    m_TextNamespace.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextNamespace.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_ToggleAssemblyDefinition = m_RootElement.Q<Toggle>("toggle-assembly-definition");
            m_ToggleAssemblyDefinition.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition;
            m_ToggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition = evt.newValue;
                if (Core.Config.DeveloperBuildInfoAssemblyDefinition != evt.newValue)
                {
                    m_ToggleAssemblyDefinition.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleAssemblyDefinition.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });


            m_TextNumber = m_RootElement.Q<TextField>("text-number");
            m_TextNumber.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument;
            m_TextNumber.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildNumberArgument != evt.newValue)
                {
                    m_TextNumber.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextNumber.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_TextDescription = m_RootElement.Q<TextField>("text-description");
            m_TextDescription.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument;
            m_TextDescription.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildDescriptionArgument != evt.newValue)
                {
                    m_TextDescription.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextDescription.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_TextChangelist = m_RootElement.Q<TextField>("text-changelist");
            m_TextChangelist.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument;
            m_TextChangelist.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildChangelistArgument != evt.newValue)
                {
                    m_TextChangelist.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextChangelist.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_TextTask = m_RootElement.Q<TextField>("text-task");
            m_TextTask.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument;
            m_TextTask.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildTaskArgument != evt.newValue)
                {
                    m_TextTask.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextTask.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_TextStream = m_RootElement.Q<TextField>("text-stream");
            m_TextStream.value = SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument;
            m_TextStream.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument = evt.newValue;
                if (Core.Config.DeveloperBuildInfoBuildStreamArgument != evt.newValue)
                {
                    m_TextStream.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_TextStream.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
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
        public string GetSectionKey()
        {
            return SectionKey;
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
            return SettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            SettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled = newState;
            if (Core.Config.DeveloperBuildInfoEnabled != newState)
            {
                toggleElement.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }

            CheckForDisabledContent();
            SettingsProvider.CheckForChanges();
        }

        public string GetToggleTooltip()
        {
            return "During the build process should a BuildInfo be written?";
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsBuildInfo";
        }

        void CheckForDisabledContent()
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
                SettingsProvider.WorkingConfig.DeveloperBuildInfoPath);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNamespace,
                Core.Config.DeveloperBuildInfoNamespace,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleAssemblyDefinition,
                Core.Config.DeveloperBuildInfoAssemblyDefinition,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNumber,
                Core.Config.DeveloperBuildInfoBuildNumberArgument,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextDescription,
                Core.Config.DeveloperBuildInfoBuildDescriptionArgument,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextChangelist,
                Core.Config.DeveloperBuildInfoBuildChangelistArgument,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextTask,
                Core.Config.DeveloperBuildInfoBuildTaskArgument,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextStream,
                Core.Config.DeveloperBuildInfoBuildStreamArgument,
                SettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument);
        }
    }
}