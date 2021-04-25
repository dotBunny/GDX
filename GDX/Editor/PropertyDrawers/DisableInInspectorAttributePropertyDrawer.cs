// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;

namespace GDX.Editor.PropertyDrawers
{
    /// <summary>
    ///     The drawing component of the <see cref="DisableInInspectorAttribute" />.
    /// </summary>
    [CustomPropertyDrawer(typeof(DisableInInspectorAttribute))]
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

    }
}