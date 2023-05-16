// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class VisualElementsProvider
    {
        public static VisualElement ActionableHelpBox(string message, HelpBoxMessageType type, string action, System.Action onAction)
        {
            VisualElement helpBox = new HelpBox(message, type);
            Button button = new Button(onAction) { text = action };
            helpBox.contentContainer.Add(button);
            return helpBox;
        }
    }
}