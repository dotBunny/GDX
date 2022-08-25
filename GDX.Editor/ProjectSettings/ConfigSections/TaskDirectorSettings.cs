// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;
namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    class TaskDirectorSettings : IConfigSection
    {
        public const int SectionIndex = 8;
        public const string SectionKey = "GDX.TaskDirector";
        static readonly string[] k_Keywords = { "task", "tasks" };
        VisualElement m_RootElement;

        Slider m_EditorTaskDirectorTickRate;
        Toggle m_EditorTaskDirectorToggle;
        Slider m_TaskDirectorTickRate;
        Toggle m_TaskDirectorToggle;


        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsTaskDirector";
        }

        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_TaskDirectorToggle = m_RootElement.Q<Toggle>("toggle-task-director");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TaskDirectorToggle);
            m_TaskDirectorToggle.value = ProjectSettingsProvider.WorkingConfig.TaskDirectorSystem;
            m_TaskDirectorToggle.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TaskDirectorSystem = evt.newValue;
                if (Config.TaskDirectorSystem != evt.newValue)
                {
                    m_TaskDirectorToggle.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TaskDirectorToggle.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

            m_TaskDirectorTickRate = m_RootElement.Q<Slider>("slider-task-director-tick-rate");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_TaskDirectorTickRate);
            m_TaskDirectorTickRate.value = ProjectSettingsProvider.WorkingConfig.TaskDirectorSystemTickRate;
            m_TaskDirectorTickRate.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TaskDirectorSystemTickRate = evt.newValue;
                if (Math.Abs(Config.TaskDirectorSystemTickRate - evt.newValue) > Platform.FloatTolerance)
                {
                    m_TaskDirectorTickRate.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_TaskDirectorTickRate.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });


            m_EditorTaskDirectorToggle = m_RootElement.Q<Toggle>("toggle-editor-task-director");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_EditorTaskDirectorToggle);
            m_EditorTaskDirectorToggle.value = ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystem;
            m_EditorTaskDirectorToggle.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystem = evt.newValue;
                if (Config.EditorTaskDirectorSystem != evt.newValue)
                {
                    m_EditorTaskDirectorToggle.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_EditorTaskDirectorToggle.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

            m_EditorTaskDirectorTickRate = m_RootElement.Q<Slider>("slider-editor-task-director-tick-rate");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_EditorTaskDirectorTickRate);
            m_EditorTaskDirectorTickRate.value = (float)ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystemTickRate;
            m_EditorTaskDirectorTickRate.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystemTickRate = evt.newValue;
                if (Math.Abs(Config.EditorTaskDirectorSystemTickRate - evt.newValue) > Platform.FloatTolerance)
                {
                    m_EditorTaskDirectorTickRate.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_EditorTaskDirectorTickRate.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });
        }

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public string GetSectionHeaderLabel()
        {
            return "Task Director";
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
            return false;
        }

        public bool GetToggleState()
        {
            return false;
        }

        public string GetToggleTooltip()
        {
            return null;
        }

        public void SetToggleState(VisualElement toggleElement, bool newState)
        {

        }

        public void UpdateSectionContent()
        {
            ProjectSettingsProvider.SetStructChangeCheck(m_TaskDirectorToggle, Config.TaskDirectorSystem,
                ProjectSettingsProvider.WorkingConfig.TaskDirectorSystem);

            ProjectSettingsProvider.SetStructChangeCheck(m_TaskDirectorTickRate,
                Config.TaskDirectorSystemTickRate,
                ProjectSettingsProvider.WorkingConfig.TaskDirectorSystemTickRate);

            ProjectSettingsProvider.SetStructChangeCheck(m_EditorTaskDirectorToggle,
                Config.EditorTaskDirectorSystem,
                ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystem);

            ProjectSettingsProvider.SetStructChangeCheck(m_EditorTaskDirectorTickRate,
                (float)Config.EditorTaskDirectorSystemTickRate,
                (float)ProjectSettingsProvider.WorkingConfig.EditorTaskDirectorSystemTickRate);
        }
    }
}