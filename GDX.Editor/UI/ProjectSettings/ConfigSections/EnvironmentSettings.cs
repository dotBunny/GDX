// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Editor.UI.ProjectSettings;
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
    // ReSharper disable once UnusedType.Global
    internal class EnvironmentSettings : IConfigSection
    {
        public const string SectionID = "GDX.Environment";
        private VisualElement m_RootElement;
        private Toggle m_ToggleEnsureSymbol;
        private Toggle m_ToggleDebugConsole;
        private Toggle m_ToggleDevelopmentConsole;
        private MaskField m_MaskDevelopment;
        private MaskField m_MaskDebug;
        private MaskField m_MaskRelease;

        private static readonly List<string> s_TraceChoices = new List<string>()
        {
            "Info",
            "Log",
            "Warning",
            "Error",
            "Exception",
            "Assertion",
            "Fatal"
        };
        private static readonly List<int> s_TraceValues = new List<int>()
        {
            0,
            1,
            2,
            4,
            8,
            16,
            32
        };

        public string GetTemplateName()
        {
            return "GDXProjectSettingsEnvironment";
        }

        public void BindSectionContent(VisualElement rootElement)
        {
            m_RootElement = rootElement;

            m_ToggleEnsureSymbol = m_RootElement.Q<Toggle>("toggle-ensure-symbol");
            m_ToggleEnsureSymbol.value = UI.SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol;
            m_ToggleEnsureSymbol.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol = evt.newValue;
                if (Core.Config.EnvironmentScriptingDefineSymbol != evt.newValue)
                {
                    m_ToggleEnsureSymbol.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleEnsureSymbol.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            m_ToggleDevelopmentConsole = m_RootElement.Q<Toggle>("toggle-console-development");
            m_ToggleDevelopmentConsole.value = UI.SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole;
            m_ToggleDevelopmentConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole = evt.newValue;
                if (Core.Config.TraceDevelopmentOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDevelopmentConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDevelopmentConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });
            m_ToggleDebugConsole = m_RootElement.Q<Toggle>("toggle-console-debug");
            m_ToggleDebugConsole.value = UI.SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole;
            m_ToggleDebugConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole = evt.newValue;
                if (Core.Config.TraceDebugOutputToUnityConsole != evt.newValue)
                {
                    m_ToggleDebugConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDebugConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            m_MaskDevelopment = m_RootElement.Q<MaskField>("mask-development");


#if GDX_MASKFIELD
            m_MaskDevelopment.choices = s_TraceChoices;
            m_MaskDevelopment.choicesMasks = s_TraceValues;
#else
            SetMaskFieldValues(_maskDevelopment);
#endif
            m_MaskDevelopment.value = (int)UI.SettingsProvider.WorkingConfig.TraceDevelopmentLevels;
            m_MaskDevelopment.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.TraceDevelopmentLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceDevelopmentLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDevelopment.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskDevelopment.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            m_MaskDebug = m_RootElement.Q<MaskField>("mask-debug");
#if GDX_MASKFIELD
            m_MaskDebug.choices = s_TraceChoices;
            m_MaskDebug.choicesMasks = s_TraceValues;
#else
            SetMaskFieldValues(_maskDebug);
#endif
            m_MaskDebug.value = (int)UI.SettingsProvider.WorkingConfig.TraceDebugLevels;
            m_MaskDebug.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.TraceDebugLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceDebugLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskDebug.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskDebug.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            m_MaskRelease = m_RootElement.Q<MaskField>("mask-release");
#if GDX_MASKFIELD
            m_MaskRelease.choices = s_TraceChoices;
            m_MaskRelease.choicesMasks = s_TraceValues;
#else
            SetMaskFieldValues(_maskRelease);
#endif
            m_MaskRelease.value = (int)UI.SettingsProvider.WorkingConfig.TraceReleaseLevels;
            m_MaskRelease.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.TraceReleaseLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.TraceReleaseLevels != (Trace.TraceLevel)evt.newValue)
                {
                    m_MaskRelease.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    m_MaskRelease.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
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
                choiceProperty.SetValue(maskField, s_traceChoices, null);
            }

            if (choiceMaskProperty != null)
            {
                choiceMaskProperty.SetValue(maskField, s_traceValues, null);
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
        public string GetSectionID()
        {
            return SectionID;
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
                UI.SettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskDevelopment,
                (int)Core.Config.TraceDevelopmentLevels,
                (int)UI.SettingsProvider.WorkingConfig.TraceDevelopmentLevels);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleDevelopmentConsole,
                Core.Config.TraceDevelopmentOutputToUnityConsole,
                UI.SettingsProvider.WorkingConfig.TraceDevelopmentOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskDebug,
                (int)Core.Config.TraceDebugLevels,
                (int)UI.SettingsProvider.WorkingConfig.TraceDebugLevels);

            ConfigSectionsProvider.SetStructChangeCheck(m_ToggleDebugConsole,
                Core.Config.TraceDebugOutputToUnityConsole,
                UI.SettingsProvider.WorkingConfig.TraceDebugOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(m_MaskRelease,
                (int)Core.Config.TraceReleaseLevels,
                (int)UI.SettingsProvider.WorkingConfig.TraceReleaseLevels);
        }
    }
}