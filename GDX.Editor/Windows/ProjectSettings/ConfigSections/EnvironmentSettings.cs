// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings.ConfigSections
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    class EnvironmentSettings : IConfigSection
    {
        public const int SectionIndex = 4;
        public const string SectionKey = "GDX.Environment";
        static readonly string[] k_Keywords = { "environment", "define", "symbol", "console" };
        VisualElement m_RootElement;
        Toggle m_ToggleAutoCaptureUnityLogs;
        Toggle m_ToggleDeveloperConsole;
        Toggle m_ToggleEnsureShaders;
        Toggle m_ToggleEnsureSymbol;
        Toggle m_ToggleToolsMenu;

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

            m_ToggleDeveloperConsole = m_RootElement.Q<Toggle>("toggle-developer-console");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleDeveloperConsole);
            m_ToggleDeveloperConsole.value = ProjectSettingsProvider.WorkingConfig.EnvironmentDeveloperConsole;
            m_ToggleDeveloperConsole.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EnvironmentDeveloperConsole = evt.newValue;
                if (Config.EnvironmentDeveloperConsole != evt.newValue)
                {
                    m_ToggleDeveloperConsole.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleDeveloperConsole.RemoveFromClassList(ResourcesProvider.ChangedClass);
                }
                ProjectSettingsProvider.UpdateForChanges();
            });

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

            m_ToggleToolsMenu = m_RootElement.Q<Toggle>("toggle-tools-menu");
            ProjectSettingsProvider.RegisterElementForSearch(SectionIndex, m_ToggleToolsMenu);
            m_ToggleToolsMenu.value = ProjectSettingsProvider.WorkingConfig.EnvironmentToolsMenu;
            m_ToggleToolsMenu.RegisterValueChangedCallback(evt =>
            {
                ProjectSettingsProvider.WorkingConfig.EnvironmentToolsMenu = evt.newValue;
                if (Config.EnvironmentToolsMenu != evt.newValue)
                {
                    m_ToggleToolsMenu.AddToClassList(ResourcesProvider.ChangedClass);
                }
                else
                {
                    m_ToggleToolsMenu.RemoveFromClassList(ResourcesProvider.ChangedClass);
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


            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleEnsureSymbol,
                Config.EnvironmentScriptingDefineSymbol,
                ProjectSettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleDeveloperConsole,
                Config.EnvironmentDeveloperConsole,
                ProjectSettingsProvider.WorkingConfig.EnvironmentDeveloperConsole);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleToolsMenu,
                Config.EnvironmentToolsMenu,
                ProjectSettingsProvider.WorkingConfig.EnvironmentToolsMenu);
        }
    }
}