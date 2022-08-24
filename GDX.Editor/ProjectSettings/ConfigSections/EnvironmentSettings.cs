// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;
#if !GDX_MASKFIELD
using UnityEngine.UI;
using System.Reflection;
#endif

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    class EnvironmentSettings : IConfigSection
    {
        public const int SectionIndex = 4;
        public const string SectionKey = "GDX.Environment";
        static readonly string[] k_Keywords = { "environment", "debug", "culture", "define", "symbol", "trace", "task" };
        VisualElement m_RootElement;
        Toggle m_ToggleEnsureSymbol;
        Toggle m_ToggleEnsureShaders;
        Toggle m_ToggleDebugConsole;
        Toggle m_ToggleDevelopmentConsole;
        MaskField m_MaskDevelopment;
        MaskField m_MaskDebug;
        MaskField m_MaskRelease;

        Slider m_EditorTaskDirectorTickRate;
        Toggle m_EditorTaskDirectorToggle;
        Slider m_TaskDirectorTickRate;
        Toggle m_TaskDirectorToggle;


        static readonly List<string> k_TraceChoices = new List<string>()
        {
            "Info",
            "Log",
            "Warning",
            "Error",
            "Exception",
            "Assertion",
            "Fatal"
        };
        static readonly List<int> k_TraceValues = new List<int>()
        {
            0,
            1,
            2,
            4,
            8,
            16,
            32
        };

        public string[] GetSearchKeywords()
        {
            return k_Keywords;
        }

        public string GetTemplateName()
        {
            return "GDXProjectSettingsEnvironment";
        }

        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_ToggleEnsureSymbol = m_RootElement.Q<Toggle>("toggle-ensure-symbol");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleEnsureSymbol);
            m_ToggleEnsureSymbol.value = ProjectSettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol;
            m_ToggleEnsureSymbol.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol = evt.newValue;
                if (Config.EnvironmentScriptingDefineSymbol != evt.newValue)
                {
                    m_ToggleEnsureSymbol.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleEnsureSymbol.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });

            m_ToggleEnsureShaders = m_RootElement.Q<Toggle>("toggle-ensure-shaders");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleEnsureShaders);
            m_ToggleEnsureShaders.value = ProjectSettingsProvider.WorkingConfig.EnvironmentAlwaysIncludeShaders;
            m_ToggleEnsureShaders.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EnvironmentAlwaysIncludeShaders = evt.newValue;
                if (Config.EnvironmentAlwaysIncludeShaders != evt.newValue)
                {
                    m_ToggleEnsureShaders.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleEnsureShaders.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }

                ProjectSettingsProvider.UpdateForChanges();
            });


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

            m_ToggleDevelopmentConsole = m_RootElement.Q<Toggle>("toggle-console-development");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleDevelopmentConsole);
            m_ToggleDevelopmentConsole.value = ProjectSettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole;
            m_ToggleDevelopmentConsole.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole = evt.newValue;
                if (Config.TraceDevelopmentOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDevelopmentConsole.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDevelopmentConsole.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });
            m_ToggleDebugConsole = m_RootElement.Q<Toggle>("toggle-console-debug");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleDebugConsole);
            m_ToggleDebugConsole.value = ProjectSettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole;
            m_ToggleDebugConsole.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole = evt.newValue;
                if (Config.TraceDebugOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDebugConsole.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDebugConsole.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

            m_MaskDevelopment = m_RootElement.Q<MaskField>("mask-development");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_MaskDevelopment);


#if GDX_MASKFIELD
            m_MaskDevelopment.choices = k_TraceChoices;
            m_MaskDevelopment.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskDevelopment);
#endif
            m_MaskDevelopment.value = (int)ProjectSettingsProvider.WorkingConfig.TraceDevelopmentLevels;
            m_MaskDevelopment.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TraceDevelopmentLevels = (Trace.TraceLevel)evt.newValue;
                if (Config.TraceDevelopmentLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDevelopment.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_MaskDevelopment.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

            m_MaskDebug = m_RootElement.Q<MaskField>("mask-debug");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_MaskDebug);
#if GDX_MASKFIELD
            m_MaskDebug.choices = k_TraceChoices;
            m_MaskDebug.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskDebug);
#endif
            m_MaskDebug.value = (int)ProjectSettingsProvider.WorkingConfig.TraceDebugLevels;
            m_MaskDebug.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TraceDebugLevels = (Trace.TraceLevel)evt.newValue;
                if (Config.TraceDebugLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDebug.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_MaskDebug.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

            m_MaskRelease = m_RootElement.Q<MaskField>("mask-release");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_MaskRelease);
#if GDX_MASKFIELD
            m_MaskRelease.choices = k_TraceChoices;
            m_MaskRelease.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskRelease);
#endif
            m_MaskRelease.value = (int)ProjectSettingsProvider.WorkingConfig.TraceReleaseLevels;
            m_MaskRelease.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.TraceReleaseLevels = (Trace.TraceLevel)evt.newValue;
                if (Config.TraceReleaseLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskRelease.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_MaskRelease.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });
        }

#if !GDX_MASKFIELD
        void SetMaskFieldValues(MaskField maskField)
        {
            System.Type type = maskField.GetType();
            PropertyInfo choiceProperty = type.GetProperty("choices", BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo choiceMaskProperty = type.GetProperty("choicesMasks", BindingFlags.NonPublic | BindingFlags.Instance);
            if (choiceProperty != null)
            {
                choiceProperty.SetValue(maskField, s_TraceChoices, null);
            }

            if (choiceMaskProperty != null)
            {
                choiceMaskProperty.SetValue(maskField, s_TraceValues, null);
            }
        }
#endif

        public bool GetDefaultVisibility()
        {
            return false;
        }
        public string GetSectionHeaderLabel()
        {
            return "Environment";
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

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleEnsureShaders,
                Config.EnvironmentAlwaysIncludeShaders,
                ProjectSettingsProvider.WorkingConfig.EnvironmentAlwaysIncludeShaders);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleEnsureSymbol,
                Config.EnvironmentScriptingDefineSymbol,
                ProjectSettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol);

            ProjectSettingsProvider.SetMaskChangeCheck(m_MaskDevelopment,
                (int)Config.TraceDevelopmentLevels,
                (int)ProjectSettingsProvider.WorkingConfig.TraceDevelopmentLevels);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleDevelopmentConsole,
                Config.TraceDevelopmentOutputToUnityConsole,
                ProjectSettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole);

            ProjectSettingsProvider.SetMaskChangeCheck(m_MaskDebug,
                (int)Config.TraceDebugLevels,
                (int)ProjectSettingsProvider.WorkingConfig.TraceDebugLevels);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleDebugConsole,
                Config.TraceDebugOutputToUnityConsole,
                ProjectSettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole);

            ProjectSettingsProvider.SetMaskChangeCheck(m_MaskRelease,
                (int)Config.TraceReleaseLevels,
                (int)ProjectSettingsProvider.WorkingConfig.TraceReleaseLevels);
        }
    }
}