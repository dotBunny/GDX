// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class UIElementsProvider
    {
        public static VisualElement GetDefaultInspector(this SerializedObject serializedObject)
        {
            var container = new VisualElement();

            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(true))
            {
                do
                {
                    var propertyField = new PropertyField(iterator.Copy()) { name = "PropertyField:" + iterator.propertyPath };

                    if (iterator.propertyPath == "m_Script" && serializedObject.targetObject != null)
                        propertyField.SetEnabled(value: false);

                    container.Add(propertyField);
                }
                while (iterator.NextVisible(false));
            }
            return container;
        }

        public static VisualElement ActionableHelpBox(string message, HelpBoxMessageType type, string action, System.Action onAction)
        {
            // Create the helpbox stub
            VisualElement helpBox = new HelpBox(message, type);
            Button button = new Button(onAction) { text = action };
            helpBox.contentContainer.Add(button);
            return helpBox;
        }
    }
}