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
        public const int SectionIndex = 2;
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
                    AssetDatabase.ImportAsset("Assets/" + Config.DeveloperBuildInfoPath);
                };
            }


            m_TextOutputPath = m_RootElement.Q<TextField>("text-output-path");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextOutputPath);
            m_TextOutputPath.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoPath;
            m_TextOutputPath.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoPath = evt.newValue;
                if (Config.DeveloperCommandLineParserArgumentPrefix != evt.newValue)
                {
                    m_TextOutputPath.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextOutputPath.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });


            m_TextNamespace = m_RootElement.Q<TextField>("text-namespace");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextNamespace);
            m_TextNamespace.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace;
            m_TextNamespace.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace = evt.newValue;
                if (Config.DeveloperBuildInfoNamespace != evt.newValue)
                {
                    m_TextNamespace.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextNamespace.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_ToggleAssemblyDefinition = m_RootElement.Q<Toggle>("toggle-assembly-definition");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleAssemblyDefinition);
            m_ToggleAssemblyDefinition.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition;
            m_ToggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition = evt.newValue;
                if (Config.DeveloperBuildInfoAssemblyDefinition != evt.newValue)
                {
                    m_ToggleAssemblyDefinition.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleAssemblyDefinition.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });


            m_TextNumber = m_RootElement.Q<TextField>("text-number");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextNumber);
            m_TextNumber.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument;
            m_TextNumber.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument = evt.newValue;
                if (Config.DeveloperBuildInfoBuildNumberArgument != evt.newValue)
                {
                    m_TextNumber.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextNumber.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_TextDescription = m_RootElement.Q<TextField>("text-description");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextDescription);
            m_TextDescription.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument;
            m_TextDescription.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument = evt.newValue;
                if (Config.DeveloperBuildInfoBuildDescriptionArgument != evt.newValue)
                {
                    m_TextDescription.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextDescription.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_TextChangelist = m_RootElement.Q<TextField>("text-changelist");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextChangelist);
            m_TextChangelist.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument;
            m_TextChangelist.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument = evt.newValue;
                if (Config.DeveloperBuildInfoBuildChangelistArgument != evt.newValue)
                {
                    m_TextChangelist.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextChangelist.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_TextTask = m_RootElement.Q<TextField>("text-task");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextTask);
            m_TextTask.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument;
            m_TextTask.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument = evt.newValue;
                if (Config.DeveloperBuildInfoBuildTaskArgument != evt.newValue)
                {
                    m_TextTask.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextTask.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_TextStream = m_RootElement.Q<TextField>("text-stream");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextStream);
            m_TextStream.value = ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument;
            m_TextStream.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument = evt.newValue;
                if (Config.DeveloperBuildInfoBuildStreamArgument != evt.newValue)
                {
                    m_TextStream.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TextStream.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
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
            return "api/GDX.Editor.Build.BuildInfoProvider.html";
        }

        public bool GetToggleSupport()
        {
            return true;
        }
        public bool GetToggleState()
        {
            return ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoEnabled = newState;
            if (Config.DeveloperBuildInfoEnabled != newState)
            {
                toggleElement.AddToClassList(ResourcesProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ResourcesProvider.ChangedClass);
            }

            CheckForDisabledContent();
            ProjectSettingsProvider.UpdateForChanges();
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
            if (File.Exists(Path.Combine(Application.dataPath, Config.DeveloperBuildInfoPath)))
            {
                m_Notice.AddToClassList(ResourcesProvider.HiddenClass);
            }
            else
            {
                m_Notice.RemoveFromClassList(ResourcesProvider.HiddenClass);
            }
        }

        public void UpdateSectionContent()
        {
            ProjectSettingsProvider.SetClassChangeCheck(m_TextOutputPath,
                Config.DeveloperBuildInfoPath,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoPath);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextNamespace,
                Config.DeveloperBuildInfoNamespace,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoNamespace);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleAssemblyDefinition,
                Config.DeveloperBuildInfoAssemblyDefinition,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoAssemblyDefinition);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextNumber,
                Config.DeveloperBuildInfoBuildNumberArgument,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildNumberArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextDescription,
                Config.DeveloperBuildInfoBuildDescriptionArgument,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildDescriptionArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextChangelist,
                Config.DeveloperBuildInfoBuildChangelistArgument,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildChangelistArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextTask,
                Config.DeveloperBuildInfoBuildTaskArgument,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildTaskArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextStream,
                Config.DeveloperBuildInfoBuildStreamArgument,
                ProjectSettingsProvider.WorkingConfig.DeveloperBuildInfoBuildStreamArgument);
        }
    }
}