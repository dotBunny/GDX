// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
#if !GDX_MASKFIELD
using System.Reflection;
#endif

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    class EnvironmentSettings : IConfigSection
    {
        public const string SectionKey = "GDX.Environment";
        static readonly string[] k_Keywords = { "environment", "debug", "culture", "define", "symbol", "trace" };
        VisualElement m_RootElement;
        Toggle m_ToggleEnsureSymbol;
        Toggle m_ToggleDebugConsole;
        Toggle m_ToggleDevelopmentConsole;
        MaskField m_MaskDevelopment;
        MaskField m_MaskDebug;
        MaskField m_MaskRelease;

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
            m_ToggleEnsureSymbol.value = SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol;
            m_ToggleEnsureSymbol.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol = evt.newValue;
                if (Core.Config.EnvironmentScriptingDefineSymbol != evt.newValue)
                {
                    m_ToggleEnsureSymbol.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleEnsureSymbol.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                SettingsProvider.CheckForChanges();
            });

            m_ToggleDevelopmentConsole = m_RootElement.Q<Toggle>("toggle-console-development");
            m_ToggleDevelopmentConsole.value = SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole;
            m_ToggleDevelopmentConsole.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole = evt.newValue;
                if (Core.Config.TraceDevelopmentOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDevelopmentConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDevelopmentConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                SettingsProvider.CheckForChanges();
            });
            m_ToggleDebugConsole = m_RootElement.Q<Toggle>("toggle-console-debug");
            m_ToggleDebugConsole.value = SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole;
            m_ToggleDebugConsole.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole = evt.newValue;
                if (Core.Config.TraceDebugOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDebugConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDebugConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                SettingsProvider.CheckForChanges();
            });

            m_MaskDevelopment = m_RootElement.Q<MaskField>("mask-development");


#if GDX_MASKFIELD
            m_MaskDevelopment.choices = k_TraceChoices;
            m_MaskDevelopment.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskDevelopment);
#endif
            m_MaskDevelopment.value = (int)SettingsProvider.WorkingConfig.TraceDevelopmentLevels;
            m_MaskDevelopment.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.TraceDevelopmentLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceDevelopmentLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDevelopment.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskDevelopment.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                SettingsProvider.CheckForChanges();
            });

            m_MaskDebug = m_RootElement.Q<MaskField>("mask-debug");
#if GDX_MASKFIELD
            m_MaskDebug.choices = k_TraceChoices;
            m_MaskDebug.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskDebug);
#endif
            m_MaskDebug.value = (int)SettingsProvider.WorkingConfig.TraceDebugLevels;
            m_MaskDebug.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.TraceDebugLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceDebugLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDebug.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskDebug.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                SettingsProvider.CheckForChanges();
            });

            m_MaskRelease = m_RootElement.Q<MaskField>("mask-release");
#if GDX_MASKFIELD
            m_MaskRelease.choices = k_TraceChoices;
            m_MaskRelease.choicesMasks = k_TraceValues;
#else
            SetMaskFieldValues(m_MaskRelease);
#endif
            m_MaskRelease.value = (int)SettingsProvider.WorkingConfig.TraceReleaseLevels;
            m_MaskRelease.RegisterValueChangedCallback(evt =>
            {
                SettingsProvider.WorkingConfig.TraceReleaseLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceReleaseLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskRelease.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskRelease.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                SettingsProvider.CheckForChanges();
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
            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleEnsureSymbol,
                Core.Config.EnvironmentScriptingDefineSymbol,
                SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskDevelopment,
                (int)Core.Config.TraceDevelopmentLevels,
                (int)SettingsProvider.WorkingConfig.TraceDevelopmentLevels);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleDevelopmentConsole,
                Core.Config.TraceDevelopmentOutputToUnityConsole,
                SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskDebug,
                (int)Core.Config.TraceDebugLevels,
                (int)SettingsProvider.WorkingConfig.TraceDebugLevels);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleDebugConsole,
                Core.Config.TraceDebugOutputToUnityConsole,
                SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskRelease,
                (int)Core.Config.TraceReleaseLevels,
                (int)SettingsProvider.WorkingConfig.TraceReleaseLevels);
        }
    }
}