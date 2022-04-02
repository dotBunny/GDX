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
        static readonly string[] k_Keywords = { "build", "buildinfo", "changelist", "stream" };
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
                    AssetDatabase.ImportAsset("Assets/" + GDXConfig.DeveloperBuildInfoPath);
                };
            }


            m_TextOutputPath = m_RootElement.Q<TextField>("text-output-path");
            m_TextOutputPath.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoPath;
            m_TextOutputPath.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoPath = evt.newValue;
                if (GDXConfig.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
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
            m_TextNamespace.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoNamespace;
            m_TextNamespace.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoNamespace = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoNamespace != evt.newValue)
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
            m_ToggleAssemblyDefinition.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoAssemblyDefinition;
            m_ToggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoAssemblyDefinition = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoAssemblyDefinition != evt.newValue)
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
            m_TextNumber.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildNumberArgument;
            m_TextNumber.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildNumberArgument = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoBuildNumberArgument != evt.newValue)
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
            m_TextDescription.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildDescriptionArgument;
            m_TextDescription.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildDescriptionArgument = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoBuildDescriptionArgument != evt.newValue)
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
            m_TextChangelist.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildChangelistArgument;
            m_TextChangelist.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildChangelistArgument = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoBuildChangelistArgument != evt.newValue)
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
            m_TextTask.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildTaskArgument;
            m_TextTask.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildTaskArgument = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoBuildTaskArgument != evt.newValue)
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
            m_TextStream.value = SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildStreamArgument;
            m_TextStream.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildStreamArgument = evt.newValue;
                if (GDXConfig.DeveloperBuildInfoBuildStreamArgument != evt.newValue)
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
            return SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoEnabled;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoEnabled = newState;
            if (GDXConfig.DeveloperBuildInfoEnabled != newState)
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

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
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
            if (File.Exists(Path.Combine(Application.dataPath, GDXConfig.DeveloperBuildInfoPath)))
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
                GDXConfig.DeveloperBuildInfoPath,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoPath);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNamespace,
                GDXConfig.DeveloperBuildInfoNamespace,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoNamespace);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleAssemblyDefinition,
                GDXConfig.DeveloperBuildInfoAssemblyDefinition,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoAssemblyDefinition);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextNumber,
                GDXConfig.DeveloperBuildInfoBuildNumberArgument,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildNumberArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextDescription,
                GDXConfig.DeveloperBuildInfoBuildDescriptionArgument,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildDescriptionArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextChangelist,
                GDXConfig.DeveloperBuildInfoBuildChangelistArgument,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildChangelistArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextTask,
                GDXConfig.DeveloperBuildInfoBuildTaskArgument,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildTaskArgument);

            ConfigSectionsProvider.SetClassChangeCheck(m_TextStream,
                GDXConfig.DeveloperBuildInfoBuildStreamArgument,
                SettingsProvider.WorkingTransientConfig.DeveloperBuildInfoBuildStreamArgument);
        }
    }
}