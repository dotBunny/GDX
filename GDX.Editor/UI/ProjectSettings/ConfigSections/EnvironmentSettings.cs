// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Editor;
using GDX.Editor.UI;
using GDX.Editor.UI.ProjectSettings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    internal class EnvironmentSettings : IConfigSection
    {
        private VisualElement _rootElement;
        private Toggle _toggleEnsureSymbol;
        private Toggle _toggleDebugConsole;
        private Toggle _toggleDevelopmentConsole;
        private MaskField _maskDevelopment;
        private MaskField _maskDebug;
        private MaskField _maskRelease;

        private static List<string> s_traceChoices = new List<string>()  { "Info", "Log", "Warning", "Error", "Exception", "Assertion", "Fatal" };
        private static List<int> s_traceValues = new List<int>() { 0, 1, 2, 4, 8, 16, 32};

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
                UI.SettingsProvider.CheckForChanges();
            });

            _toggleDevelopmentConsole = _rootElement.Q<Toggle>("toggle-console-development");
            _toggleDevelopmentConsole.value = UI.SettingsProvider.WorkingConfig.traceDevelopmentOutputToUnityConsole;
            _toggleDevelopmentConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDevelopmentOutputToUnityConsole = evt.newValue;
                UI.SettingsProvider.CheckForChanges();
            });
            _toggleDebugConsole = _rootElement.Q<Toggle>("toggle-console-debug");
            _toggleDebugConsole.value = UI.SettingsProvider.WorkingConfig.traceDebugOutputToUnityConsole;
            _toggleDebugConsole.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDebugOutputToUnityConsole = evt.newValue;
                UI.SettingsProvider.CheckForChanges();
            });

            _maskDevelopment = _rootElement.Q<MaskField>("mask-development");
            _maskDevelopment.choices = s_traceChoices;
            _maskDevelopment.choicesMasks = s_traceValues;
            _maskDevelopment.value = (int)UI.SettingsProvider.WorkingConfig.traceDevelopmentLevels;
            _maskDevelopment.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDevelopmentLevels = (Trace.TraceLevel)evt.newValue;
                UI.SettingsProvider.CheckForChanges();
            });

            _maskDebug = _rootElement.Q<MaskField>("mask-debug");
            _maskDebug.choices = s_traceChoices;
            _maskDebug.choicesMasks = s_traceValues;
            _maskDebug.value = (int)UI.SettingsProvider.WorkingConfig.traceDebugLevels;
            _maskDevelopment.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceDebugLevels = (Trace.TraceLevel)evt.newValue;
                UI.SettingsProvider.CheckForChanges();
            });

            _maskRelease = _rootElement.Q<MaskField>("mask-release");
            _maskRelease.choices = s_traceChoices;
            _maskRelease.choicesMasks = s_traceValues;
            _maskRelease.value = (int)UI.SettingsProvider.WorkingConfig.traceReleaseLevels;
            _maskRelease.RegisterValueChangedCallback(evt =>
            {
                UI.SettingsProvider.WorkingConfig.traceReleaseLevels = (Trace.TraceLevel)evt.newValue;
                UI.SettingsProvider.CheckForChanges();
            });
        }

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