// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.IO;
using GDX.Editor.Build;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings.ConfigSections
{
    /// <summary>
    ///     Build Info Settings
    /// </summary>
    class BuildInfoSettings : IConfigSection
    {
        public const int SectionIndex = 1;
        public const string SectionKey = "GDX.Editor.Build.BuildInfoProvider";
        static readonly string[] k_Keywords = { "build", "info", "changelist", "stream" };
        Button m_ButtonCreate;
        VisualElement m_Content;
        VisualElement m_Notice;
        VisualElement m_RootElement;
        TextField m_TextChangelist;
        TextField m_TextDescription;
        TextField m_TextNamespace;
        TextField m_TextNumber;
        TextField m_TextOutputPath;
        TextField m_TextStream;
        TextField m_TextTask;
        Toggle m_ToggleAssemblyDefinition;

        /// <summary>
        ///     Bind the Build Info content.
        /// </summary>
        /// <param name="rootElement">The Build Info section <see cref="VisualElement" />.</param>
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
                    BuildInfoProvider.WriteDefaultFile();
                    AssetDatabase.ImportAsset("Assets/" + Config.BuildInfoOutputPath);
                };
            }


            m_TextOutputPath = m_RootElement.Q<TextField>("text-output-path");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TextOutputPath);
            m_TextOutputPath.value = ProjectSettingsProvider.WorkingConfig.BuildInfoOutputPath;
            m_TextOutputPath.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoOutputPath = evt.newValue;
                if (Config.CommandLineParserArgumentPrefix != evt.newValue)
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
            m_TextNamespace.value = ProjectSettingsProvider.WorkingConfig.BuildInfoNamespace;
            m_TextNamespace.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoNamespace = evt.newValue;
                if (Config.BuildInfoNamespace != evt.newValue)
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
            m_ToggleAssemblyDefinition.value = ProjectSettingsProvider.WorkingConfig.BuildInfoAssemblyDefinition;
            m_ToggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoAssemblyDefinition = evt.newValue;
                if (Config.BuildInfoAssemblyDefinition != evt.newValue)
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
            m_TextNumber.value = ProjectSettingsProvider.WorkingConfig.BuildInfoBuildNumberArgument;
            m_TextNumber.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildNumberArgument = evt.newValue;
                if (Config.BuildInfoBuildNumberArgument != evt.newValue)
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
            m_TextDescription.value = ProjectSettingsProvider.WorkingConfig.BuildInfoBuildDescriptionArgument;
            m_TextDescription.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildDescriptionArgument = evt.newValue;
                if (Config.BuildInfoBuildDescriptionArgument != evt.newValue)
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
            m_TextChangelist.value = ProjectSettingsProvider.WorkingConfig.BuildInfoBuildChangelistArgument;
            m_TextChangelist.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildChangelistArgument = evt.newValue;
                if (Config.BuildInfoBuildChangelistArgument != evt.newValue)
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
            m_TextTask.value = ProjectSettingsProvider.WorkingConfig.BuildInfoBuildTaskArgument;
            m_TextTask.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildTaskArgument = evt.newValue;
                if (Config.BuildInfoBuildTaskArgument != evt.newValue)
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
            m_TextStream.value = ProjectSettingsProvider.WorkingConfig.BuildInfoBuildStreamArgument;
            m_TextStream.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildStreamArgument = evt.newValue;
                if (Config.BuildInfoBuildStreamArgument != evt.newValue)
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
            return ProjectSettingsProvider.WorkingConfig.BuildInfo;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {
            ProjectSettingsProvider.WorkingConfig.BuildInfo = newState;
            if (Config.BuildInfo != newState)
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

        public void UpdateSectionContent()
        {
            ProjectSettingsProvider.SetClassChangeCheck(m_TextOutputPath,
                Config.BuildInfoOutputPath,
                ProjectSettingsProvider.WorkingConfig.BuildInfoOutputPath);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextNamespace,
                Config.BuildInfoNamespace,
                ProjectSettingsProvider.WorkingConfig.BuildInfoNamespace);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleAssemblyDefinition,
                Config.BuildInfoAssemblyDefinition,
                ProjectSettingsProvider.WorkingConfig.BuildInfoAssemblyDefinition);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextNumber,
                Config.BuildInfoBuildNumberArgument,
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildNumberArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextDescription,
                Config.BuildInfoBuildDescriptionArgument,
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildDescriptionArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextChangelist,
                Config.BuildInfoBuildChangelistArgument,
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildChangelistArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextTask,
                Config.BuildInfoBuildTaskArgument,
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildTaskArgument);

            ProjectSettingsProvider.SetClassChangeCheck(m_TextStream,
                Config.BuildInfoBuildStreamArgument,
                ProjectSettingsProvider.WorkingConfig.BuildInfoBuildStreamArgument);
        }

        void CheckForDisabledContent()
        {
            m_Content?.SetEnabled(GetToggleState());
            if (File.Exists(Path.Combine(Application.dataPath, Config.BuildInfoOutputPath)))
            {
                m_Notice.AddToClassList(ResourcesProvider.HiddenClass);
            }
            else
            {
                m_Notice.RemoveFromClassList(ResourcesProvider.HiddenClass);
            }
        }
    }
}