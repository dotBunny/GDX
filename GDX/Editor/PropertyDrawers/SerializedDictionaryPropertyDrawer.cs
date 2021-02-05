// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GDX.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor.PropertyDrawers
{
    /// <summary>
    ///     Shows an item count for known configurations of <see cref="SerializableDictionary{TKey,TValue}" />.
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, int>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, string>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, float>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, bool>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, Object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, GameObject>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, MonoBehaviour>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, int>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, string>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, float>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, bool>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, Object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, GameObject>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, MonoBehaviour>))]
    internal class SerializedDictionaryPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Generate some sort of inspector for <see cref="SerializableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="position">The <see cref="Rect"/> to draw into.</param>
        /// <param name="property">The <see cref="SerializedProperty"/> to query for data.</param>
        /// <param name="label">An ignored label value.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TODO: It would be way better if we had a way to show the two arrays like the UIElement ListView.
            SerializedProperty keysProperty = property.FindPropertyRelative("serializedKeys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("serializedValues");

            int keyCount = keysProperty.arraySize;
            int valuesCount = valuesProperty.arraySize;
            bool match = keyCount == valuesCount;

            EditorGUI.LabelField(position, property.displayName,
                match ? $"{keyCount.ToString()} Items" : "ERROR! Key and Value count do not match.",
                match ? EditorStyles.label : EditorStyles.boldLabel);
        }
    }
}