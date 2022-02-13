// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
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
        private VisualElement _rootElement;
        private Toggle _toggleEnsureSymbol;
        private Toggle _toggleDebugConsole;
        private Toggle _toggleDevelopmentConsole;
        private MaskField _maskDevelopment;
        private MaskField _maskDebug;
        private MaskField _maskRelease;

        private static List<string> s_traceChoices = new List<string>()
        {
            "Info",
            "Log",
            "Warning",
            "Error",
            "Exception",
            "Assertion",
            "Fatal"
        };
        private static List<int> s_traceValues = new List<int>()
        {
            0,
            1,
            2,
            4,
            8,
            16,
            32
        };

        [InitializeOnLoadMethod]
        static void Register()
        {
            UI.SettingsProvider.RegisterConfigSection(new EnvironmentSettings());
        }
        public string GetTemplateName()
        {
            return "GDXProjectSettingsEnvironment";
        }

        public void BindSectionContent(VisualElement rootElement)
        {
            _rootElement = rootElement;

            _toggleEnsureSymbol = _rootElement.Q<Toggle>("toggle-ensure-symbol");
            _toggleEnsureSymbol.value = UI.SettingsProvider.WorkingConfig.environmentScriptingDefineSymbol;
            _toggleEnsureSymbol.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.environmentScriptingDefineSymbol = evt.newValue;
                if (Core.Config.environmentScriptingDefineSymbol != evt.newValue)
                {
                    _toggleEnsureSymbol.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _toggleEnsureSymbol.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }

                UI.SettingsProvider.CheckForChanges();
            });

            _toggleDevelopmentConsole = _rootElement.Q<Toggle>("toggle-console-development");
            _toggleDevelopmentConsole.value = UI.SettingsProvider.WorkingConfig.traceDevelopmentOutputToUnityConsole;
            _toggleDevelopmentConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDevelopmentOutputToUnityConsole = evt.newValue;
                if (Core.Config.traceDevelopmentOutputToUnityConsole != evt.newValue)
                {
                    _toggleDevelopmentConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _toggleDevelopmentConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });
            _toggleDebugConsole = _rootElement.Q<Toggle>("toggle-console-debug");
            _toggleDebugConsole.value = UI.SettingsProvider.WorkingConfig.traceDebugOutputToUnityConsole;
            _toggleDebugConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDebugOutputToUnityConsole = evt.newValue;
                if (Core.Config.traceDebugOutputToUnityConsole != evt.newValue)
                {
                    _toggleDebugConsole.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _toggleDebugConsole.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            _maskDevelopment = _rootElement.Q<MaskField>("mask-development");


#if GDX_MASKFIELD
            _maskDevelopment.choices = s_traceChoices;
            _maskDevelopment.choicesMasks = s_traceValues;
#else
            SetMaskFieldValues(_maskDevelopment);
#endif
            _maskDevelopment.value = (int)UI.SettingsProvider.WorkingConfig.traceDevelopmentLevels;
            _maskDevelopment.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDevelopmentLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.traceDevelopmentLevels != (Trace.TraceLevel)evt.newValue)
                {
                    _maskDevelopment.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _maskDevelopment.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            _maskDebug = _rootElement.Q<MaskField>("mask-debug");
#if GDX_MASKFIELD
            _maskDebug.choices = s_traceChoices;
            _maskDebug.choicesMasks = s_traceValues;
#else
            SetMaskFieldValues(_maskDebug);
#endif
            _maskDebug.value = (int)UI.SettingsProvider.WorkingConfig.traceDebugLevels;
            _maskDebug.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDebugLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.traceDebugLevels != (Trace.TraceLevel)evt.newValue)
                {
                    _maskDebug.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _maskDebug.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
                }
                UI.SettingsProvider.CheckForChanges();
            });

            _maskRelease = _rootElement.Q<MaskField>("mask-release");
#if GDX_MASKFIELD
            _maskRelease.choices = s_traceChoices;
            _maskRelease.choicesMasks = s_traceValues;
#else
            SetMaskFieldValues(_maskRelease);
#endif
            _maskRelease.value = (int)UI.SettingsProvider.WorkingConfig.traceReleaseLevels;
            _maskRelease.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceReleaseLevels = (Trace.TraceLevel)evt.newValue;
                if (Core.Config.traceReleaseLevels != (Trace.TraceLevel)evt.newValue)
                {
                    _maskRelease.AddToClassList(ConfigSectionsProvider.ChangedClass);
                }
                else
                {
                    _maskRelease.RemoveFromClassList(ConfigSectionsProvider.ChangedClass);
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
        public int GetPriority()
        {
            return 700;
        }
        public string GetSectionHeaderLabel()
        {
            return "Environment";
        }
        public string GetSectionID()
        {
            return "GDX.Environment";
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
            ConfigSectionsProvider.SetStructChangeCheck(_toggleEnsureSymbol,
                Core.Config.environmentScriptingDefineSymbol,
                UI.SettingsProvider.WorkingConfig.environmentScriptingDefineSymbol);

            ConfigSectionsProvider.SetMaskChangeCheck(_maskDevelopment,
                (int)Core.Config.traceDevelopmentLevels,
                (int)UI.SettingsProvider.WorkingConfig.traceDevelopmentLevels);

            ConfigSectionsProvider.SetStructChangeCheck(_toggleDevelopmentConsole,
                Core.Config.traceDevelopmentOutputToUnityConsole,
                UI.SettingsProvider.WorkingConfig.traceDevelopmentOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(_maskDebug,
                (int)Core.Config.traceDebugLevels,
                (int)UI.SettingsProvider.WorkingConfig.traceDebugLevels);

            ConfigSectionsProvider.SetStructChangeCheck(_toggleDebugConsole,
                Core.Config.traceDebugOutputToUnityConsole,
                UI.SettingsProvider.WorkingConfig.traceDebugOutputToUnityConsole);

            ConfigSectionsProvider.SetMaskChangeCheck(_maskRelease,
                (int)Core.Config.traceReleaseLevels,
                (int)UI.SettingsProvider.WorkingConfig.traceReleaseLevels);
        }
    }
}