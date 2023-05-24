// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor.Windows.ProjectSettings
{
    public interface IConfigSection
    {
        bool GetDefaultVisibility();
        int GetSectionIndex();
        string GetSectionKey();
        string GetSectionHeaderLabel();
        string GetSectionHelpLink();
        bool GetToggleState();
        void SetToggleState(VisualElement toggleElement, bool newState);
        bool GetToggleSupport();
        string GetToggleTooltip();
        void BindSectionContent(VisualElement rootElement);
        void UpdateSectionContent();

        string GetTemplateName();
        string[] GetSearchKeywords();
    }
}