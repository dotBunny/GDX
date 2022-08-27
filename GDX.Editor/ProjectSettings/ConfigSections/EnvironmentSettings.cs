// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;
using Toggle = UnityEngine.UIElements.Toggle;

namespace GDX.Editor.ProjectSettings
{
    /// <summary>
    ///     Environment Settings
    /// </summary>
    class EnvironmentSettings : IConfigSection
    {
        public const int SectionIndex = 4;
        public const string SectionKey = "GDX.Environment";
        static readonly string[] k_Keywords = { "environment", "define", "symbol" };
        VisualElement m_RootElement;
        Toggle m_ToggleEnsureSymbol;
        Toggle m_ToggleEnsureShaders;

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
            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleEnsureShaders,
                Config.EnvironmentAlwaysIncludeShaders,
                ProjectSettingsProvider.WorkingConfig.EnvironmentAlwaysIncludeShaders);

            ProjectSettingsProvider.SetStructChangeCheck(m_ToggleEnsureSymbol,
                Config.EnvironmentScriptingDefineSymbol,
                ProjectSettingsProvider.WorkingConfig.EnvironmentScriptingDefineSymbol);
        }
    }
}