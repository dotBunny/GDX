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
    /// <remarks>
    ///     <para>
    ///         Versions of Unity after 2020.1 are able to have generic types for custom property drawers, otherwise we try to
    ///         just pre-populate a bunch of possible configurations.
    ///     </para>
    ///     <para>
    ///         Heavily influenced by
    ///         https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GUI/ReorderableList.cs
    ///     </para>
    /// </remarks>
#if UNITY_2020_1_OR_NEWER
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
#else
    // Viable serialization types
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, int>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, string>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, float>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, bool>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, int>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, string>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, float>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, bool>))]

    // Warn about object referencing
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, Object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, GameObject>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<int, MonoBehaviour>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, Object>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, GameObject>))]
    [CustomPropertyDrawer(typeof(SerializableDictionary<string, MonoBehaviour>))]
#endif
    internal class SerializedDictionaryPropertyDrawer : PropertyDrawer
    {
        private SerializedObject _serializedObject;

        private SerializedProperty _propertyExpanded;
        private bool _propertyExpandedCache;
        private SerializedProperty _propertyAddKey;
        private SerializedProperty _propertyAddKeyValid;
        private bool _propertyAddKeyValidCache;

        private SerializedProperty _propertyCount;
        private int _propertyCountCache = -1;
        private SerializedProperty _propertyKeys;
        private SerializedProperty _propertyValues;

        private bool _validKeyValueCount = true;

        /// <summary>
        ///     Provide an adjusted height for the entire element to be rendered.
        /// </summary>
        /// <param name="property">The target <see cref="SerializedProperty" /> which is being evaluated.</param>
        /// <param name="label">Ignored.</param>
        /// <returns>Required height for element.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!_propertyExpandedCache)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return (EditorGUIUtility.singleLineHeight * Mathf.Max(2, _propertyCountCache + 1)) +
                   EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        ///     Generate some sort of inspector for <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="position">The <see cref="Rect" /> to draw into.</param>
        /// <param name="property">The <see cref="SerializedProperty" /> to query for data.</param>
        /// <param name="label">An ignored label value.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Cache Serialized Object
            _serializedObject = property.serializedObject;

            // Editor only properties
            _propertyExpanded = property.FindPropertyRelative("drawerExpanded");
            _propertyExpandedCache = _propertyExpanded.boolValue;
            _propertyAddKey = property.FindPropertyRelative("serializedAddKey");
            _propertyAddKeyValid = property.FindPropertyRelative("serializedAddKeyValid");
            _propertyAddKeyValidCache = _propertyAddKeyValid.boolValue;

            // Grab the properties that matter!
            _propertyCount = property.FindPropertyRelative("serializedLength");
            _propertyCountCache = _propertyCount.intValue;
            _propertyKeys = property.FindPropertyRelative("serializedKeys");
            _propertyValues = property.FindPropertyRelative("serializedValues");

            // Validate saved sizes
            _validKeyValueCount = _propertyCountCache == _propertyValues.arraySize &&
                                  _propertyCountCache == _propertyKeys.arraySize;


            DrawHeader(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.displayName);
            if (_propertyExpandedCache)
            {
                DrawDictionaryEditor(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight,
                    position.width, position.height - (EditorGUIUtility.singleLineHeight * 2)));

                DrawFooter(new Rect(position.x, position.y + (position.height - EditorGUIUtility.singleLineHeight), position.width, EditorGUIUtility.singleLineHeight));
            }

            // Anything we changed property wise we should save
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawFooter(Rect position)
        {
            GUI.enabled = _propertyAddKeyValidCache;
            if (GUI.Button(new Rect(position.x, position.y, (position.width / 2) - 100, position.height), "Add"))
            {
                AddNewEntry();
            }
            GUI.enabled = true;

            EditorGUI.PropertyField(new Rect(position.x + (position.width / 2), position.y, 100, position.height),
                _propertyAddKey);

            if (_propertyCountCache > 0)
            {
                if (GUI.Button(
                    new Rect(position.x + (position.width / 2), position.y, position.width / 2, position.height),
                    "Remove"))
                {
                    RemoveLastEntry();
                }
            }
        }

        private void DrawHeader(Rect position, string displayName)
        {
            // TODO: Could return the consumed height? so we can have a banner at the top if invalid?

            // Generate a foldout GUI element representing the name of the dictionary
            bool newExpanded =
                EditorGUI.Foldout(position,
                    _propertyExpanded.boolValue, displayName, false, EditorStyles.foldout);
            if (newExpanded != _propertyExpandedCache)
            {
                // TODO: Is there some editor cache for whats unfolded? This would prevent over serialization / extra data
                _propertyExpanded.boolValue = newExpanded;
                _propertyExpandedCache = newExpanded;
            }

            // Indicate Count - but ready only
            GUI.enabled = false;
            const int itemCountSize = 48;
            EditorGUI.TextField(new Rect(position.x + position.width - itemCountSize,
                position.y, itemCountSize, position.height), _propertyCountCache.ToString());
            GUI.enabled = true;

        }

        private void DrawDictionaryEditor(Rect area)
        {
            // RL Empty header
            // RL Background
            // RL Element
            // RL Footer
            // RL FooterButton

            if (_propertyCountCache == 0)
            {
                EditorGUI.LabelField(area, "Empty");
                return;
            }

            const float spacing = 15;
            float columnWidth = (area.width / 2) - spacing;

            for (int i = 0; i < _propertyCountCache; i++)
            {
                EditorGUI.PropertyField(new Rect(area.x, area.y + (EditorGUIUtility.singleLineHeight * i), columnWidth, EditorGUIUtility.singleLineHeight), _propertyKeys.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUI.PropertyField(new Rect(area.x + spacing + columnWidth, area.y + (EditorGUIUtility.singleLineHeight * i), columnWidth, EditorGUIUtility.singleLineHeight), _propertyValues.GetArrayElementAtIndex(i), GUIContent.none);
            }
        }

        private void AddNewEntry()
        {
            if (_propertyCountCache == -1 || _propertyKeys == null || _propertyValues == null) return;

            // Increase Size
            _propertyCountCache++;

            // Keys need to be unique
            _propertyKeys.arraySize = _propertyCountCache;

            SerializedProperty addedKey = _propertyKeys.GetArrayElementAtIndex(_propertyCountCache-1);
            addedKey.ShallowCopy(_propertyAddKey);

            _propertyValues.arraySize = _propertyCountCache;
            _propertyCount.intValue = _propertyCountCache;
        }

        private void RemoveLastEntry()
        {
            if (_propertyCountCache == -1 || _propertyKeys == null || _propertyValues == null) return;

            _propertyCountCache--;
            if (_propertyCountCache < 0) _propertyCountCache = 0;

            _propertyKeys.arraySize = _propertyCountCache;
            _propertyValues.arraySize = _propertyCountCache;
            _propertyCount.intValue = _propertyCountCache;
        }
    }
}