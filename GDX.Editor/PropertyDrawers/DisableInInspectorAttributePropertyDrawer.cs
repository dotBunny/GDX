// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers
{
    /// <summary>
    ///     The drawing component of the <see cref="DisableInInspectorAttribute" />.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableInInspectorAttribute))]
    // ReSharper disable once UnusedType.Global
    public class DisableInInspectorAttributePropertyDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Unity IMGUI Draw Event
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <param name="label">The label of this property.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }

        /// <summary>
        /// Overrides the method to make a UIElements based GUI for the property.
        /// </summary>
        /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
        /// <returns>A disabled visual element.</returns>
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement();
            PropertyField propertyField = new PropertyField(property);
            container.Add(propertyField);
            container.SetEnabled(false);
            return container;
        }
    }
}