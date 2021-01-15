// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using UnityEngine;
using UnityEditor;

namespace GDX.Editor
{
    [CustomPropertyDrawer(typeof(InspectorLabelAttribute))]
    public class InspectorLabelEditor : PropertyDrawer
    {
        public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
        {
            EditorGUI.PropertyField(position, property, new GUIContent( (attribute as InspectorLabelAttribute)?.Label ));
        }
    }

}