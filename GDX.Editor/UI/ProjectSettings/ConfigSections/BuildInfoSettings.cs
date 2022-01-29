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
    internal class BuildInfoSettings : IConfigSection
    {
        private VisualElement _rootElement;
        private VisualElement _content;
        private TextField _textOutputPath;
        private TextField _textNamespace;
        private Toggle _toggleAssemblyDefinition;
        private TextField _textNumber;
        private TextField _textDescription;
        private TextField _textChangelist;
        private TextField _textTask;
        private TextField _textStream;
        private Button _buttonCreate;
        private VisualElement _notice;

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
            _rootElement = rootElement;
            _content = _rootElement.Q<VisualElement>("gdx-build-info-content");
            _notice = _rootElement.Q<VisualElement>("notice");


            _buttonCreate = _rootElement.Q<Button>("button-create-default");
            if (_buttonCreate != null)
            {
                _buttonCreate.clicked += () =>
                {
                    Build.BuildInfoProvider.WriteDefaultFile();
                    AssetDatabase.ImportAsset("Assets/" + Core.Config.developerBuildInfoPath);
                };
            }


            _textOutputPath = _rootElement.Q<TextField>("text-output-path");
            if (_textOutputPath != null)
            {
                _textOutputPath.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoPath;
                _textOutputPath.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoPath = evt.newValue;
                    if (Core.Config.developerCommandLineParserArgumentPrefix != evt.newValue)
                    {
                        _textOutputPath.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textOutputPath.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textNamespace = _rootElement.Q<TextField>("text-namespace");
            if (_textNamespace != null)
            {
                _textNamespace.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoNamespace;
                _textNamespace.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoNamespace = evt.newValue;
                    if (Core.Config.developerBuildInfoNamespace != evt.newValue)
                    {
                        _textNamespace.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textNamespace.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _toggleAssemblyDefinition = _rootElement.Q<Toggle>("toggle-assembly-definition");
            if (_toggleAssemblyDefinition != null)
            {
                _toggleAssemblyDefinition.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoAssemblyDefinition;
                _toggleAssemblyDefinition.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoAssemblyDefinition = evt.newValue;
                    if (Core.Config.developerBuildInfoAssemblyDefinition != evt.newValue)
                    {
                        _toggleAssemblyDefinition.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _toggleAssemblyDefinition.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textNumber = _rootElement.Q<TextField>("text-number");
            if (_textNumber != null)
            {
                _textNumber.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildNumberArgument;
                _textNumber.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildNumberArgument = evt.newValue;
                    if (Core.Config.developerBuildInfoBuildNumberArgument != evt.newValue)
                    {
                        _textNumber.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textNumber.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textDescription = _rootElement.Q<TextField>("text-description");
            if (_textDescription != null)
            {
                _textDescription.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildDescriptionArgument;
                _textDescription.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildDescriptionArgument = evt.newValue;
                    if (Core.Config.developerBuildInfoBuildDescriptionArgument != evt.newValue)
                    {
                        _textDescription.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textDescription.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textChangelist = _rootElement.Q<TextField>("text-changelist");
            if (_textChangelist != null)
            {
                _textChangelist.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildChangelistArgument;
                _textChangelist.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildChangelistArgument = evt.newValue;
                    if (Core.Config.developerBuildInfoBuildChangelistArgument != evt.newValue)
                    {
                        _textChangelist.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textChangelist.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textTask = _rootElement.Q<TextField>("text-task");
            if (_textTask != null)
            {
                _textTask.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildTaskArgument;
                _textTask.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildTaskArgument = evt.newValue;
                    if (Core.Config.developerBuildInfoBuildTaskArgument != evt.newValue)
                    {
                        _textTask.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textTask.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            _textStream = _rootElement.Q<TextField>("text-stream");
            if (_textStream != null)
            {
                _textStream.value = UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildStreamArgument;
                _textStream.RegisterValueChangedCallback(evt =>
                {
                    UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildStreamArgument = evt.newValue;
                    if (Core.Config.developerBuildInfoBuildStreamArgument != evt.newValue)
                    {
                        _textStream.AddToClassList(ConfigSectionsProvider.ChangedClass);
                    }
                    else
                    {
                        _textStream.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                    }

                    UI.SettingsProvider.CheckForChanges();
                });
            }

            CheckForDisabledContent();
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
                toggleElement.AddToClassList(ConfigSectionsProvider.ChangedClass);
            }
            else
            {
                toggleElement.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
            }

            CheckForDisabledContent();
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

        private void CheckForDisabledContent()
        {
            _content?.SetEnabled(GetToggleState());
            if (File.Exists(Path.Combine(Application.dataPath, Core.Config.developerBuildInfoPath)))
            {
                _notice.AddToClassList(ConfigSectionsProvider.HiddenClass);
            }
            else
            {
                _notice.RemoveFromClassList(ConfigSectionsProvider.HiddenClass);
            }
        }

        public void UpdateSectionContent()
        {
            ConfigSectionsProvider.SetClassChangeCheck(_textOutputPath,
                Core.Config.developerBuildInfoPath,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoPath);

            ConfigSectionsProvider.SetClassChangeCheck(_textNamespace,
                Core.Config.developerBuildInfoNamespace,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoNamespace);

            ConfigSectionsProvider.SetStructChangeCheck(_toggleAssemblyDefinition,
                Core.Config.developerBuildInfoAssemblyDefinition,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoAssemblyDefinition);

            ConfigSectionsProvider.SetClassChangeCheck(_textNumber,
                Core.Config.developerBuildInfoBuildNumberArgument,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildNumberArgument);

            ConfigSectionsProvider.SetClassChangeCheck(_textDescription,
                Core.Config.developerBuildInfoBuildDescriptionArgument,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildDescriptionArgument);

            ConfigSectionsProvider.SetClassChangeCheck(_textChangelist,
                Core.Config.developerBuildInfoBuildChangelistArgument,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildChangelistArgument);

            ConfigSectionsProvider.SetClassChangeCheck(_textTask,
                Core.Config.developerBuildInfoBuildTaskArgument,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildTaskArgument);

            ConfigSectionsProvider.SetClassChangeCheck(_textStream,
                Core.Config.developerBuildInfoBuildStreamArgument,
                UI.SettingsProvider.WorkingConfig.developerBuildInfoBuildStreamArgument);
        }
    }
}