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
    internal class SerializableDictionaryPropertyDrawer : PropertyDrawer
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

        private string _label = "Serializable Dictionary";


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

            return EditorGUIUtility.singleLineHeight * Mathf.Max(2, _propertyCountCache + 1) +
                   EditorGUIUtility.singleLineHeight +
                   Utility.Padding +
                   Utility.ButtonOffset +
                   Utility.ButtonVerticalPadding +
                   Utility.MarginBottom;
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
            _label = property.displayName;

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

            // Draw the foldout at the top of the space
            DrawFoldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight));


            // If the foldout is expanded, draw the actual content
            if (_propertyExpandedCache)
            {
                Rect contentRect = new Rect(position.x,
                    position.y + EditorGUIUtility.singleLineHeight + Utility.Padding,
                    position.width,
                    position.height - (EditorGUIUtility.singleLineHeight * 2 + Utility.ButtonOffset +
                                       Utility.ButtonVerticalPadding + Utility.MarginBottom));

                Rect footerRect = new Rect(position.x,
                    position.y + (position.height - (EditorGUIUtility.singleLineHeight + Utility.ButtonOffset +
                                                     Utility.ButtonVerticalPadding + Utility.MarginBottom)),
                    position.width, EditorGUIUtility.singleLineHeight + Utility.ButtonVerticalPadding);


                DrawContentEditor(contentRect, footerRect);
                DrawFooterActions(footerRect);
            }

            // Anything we changed property wise we should save
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draw the foldout header for the <see cref="SerializableDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="position">A <see cref="Rect"/> representing the space which the header will consume in its entirety.</param>
        private void DrawFoldout(Rect position)
        {
            // TODO: Could return the consumed height? so we can have a banner at the top if invalid?
            // Generate a foldout GUI element representing the name of the dictionary
            bool newExpanded =
                EditorGUI.Foldout(position,
                    _propertyExpanded.boolValue, _label, true, EditorStyles.foldout);
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

        /// <summary>
        ///     Draw actionable buttons offset of the footer.
        /// </summary>
        /// <param name="position">A <see cref="Rect"/> representing the space which the footer will be drawn, so that he the buttons can draw offset of it.</param>
        private void DrawFooterActions(Rect position)
        {
            float inputWidth = (position.width / 2) - Utility.ButtonWidth - Utility.ButtonHorizontalPadding;
            Rect addBackground = new Rect(position.xMin + 10f, position.y + Utility.ButtonOffset,
                Utility.ButtonWidth + inputWidth + Utility.ButtonHorizontalPadding * 2,
                position.height);


            // Create button rects
            Rect addRect = new Rect(addBackground.xMin + Utility.ButtonHorizontalPadding, addBackground.yMin,
                Utility.ButtonWidth, Utility.ButtonHeight);
            Rect inputRect = new Rect(addBackground.xMin + Utility.ButtonWidth + Utility.ButtonHorizontalPadding,
                addBackground.yMin, inputWidth, Utility.ButtonHeight);

            if (Event.current.type == EventType.Repaint)
            {
                Utility.ButtonBackground.Draw(addBackground, false, false, false, false);
            }

            // Draw the input before the button so it overlaps it
            EditorGUI.PropertyField(inputRect, _propertyAddKey, GUIContent.none);

            GUI.enabled = _propertyAddKeyValidCache;
            if (GUI.Button(addRect, Utility.iconToolbarPlus, Utility.preButton))
            {
                AddNewEntry();
            }

            GUI.enabled = true;


            // Remove button
            // TODO: Only enable when selecting?
            if (_propertyCountCache > 0)
            {
                Rect removeBackground = new Rect(
                    position.xMax - (10 + Utility.ButtonWidth + Utility.ButtonHorizontalPadding * 2),
                    position.y + Utility.ButtonOffset,
                    Utility.ButtonWidth + Utility.ButtonHorizontalPadding * 2,
                    position.height);

                if (Event.current.type == EventType.Repaint)
                {
                    Utility.ButtonBackground.Draw(removeBackground, false, false, false, false);
                }

                if (GUI.Button(
                    new Rect(removeBackground.xMin + Utility.ButtonHorizontalPadding, removeBackground.yMin,
                        Utility.ButtonWidth, Utility.ButtonHeight), Utility.iconToolbarMinus, Utility.preButton))
                {
                    RemoveLastEntry();
                }
            }
        }




        private void DrawContentEditor(Rect area, Rect footerRect)
        {
            //if (Event.current.type != EventType.Repaint) return;

            Rect contentFooterRect = new Rect(footerRect.x, footerRect.y, footerRect.width, 7);
            Utility.FooterBackground.fixedHeight = 0;
            if (_propertyCountCache == 0)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    Utility.RLEmptyHeader.Draw(area, false, false, false, false);
                    Utility.FooterBackground.Draw(contentFooterRect, false, false, false, false);
                }

                // apply the padding to get the internal rect
                area.xMin += Utility.Padding;
                area.xMax -= Utility.Padding;
                area.height -= 2;
                area.y += 1;

                EditorGUI.LabelField(area, Utility.EmptyDictionaryContent, EditorStyles.label);
                return;
            }


            if (Event.current.type == EventType.Repaint)
            {
                Utility.RLHeader.Draw(area, false, false, false, false);
                Utility.FooterBackground.Draw(contentFooterRect, false, false, false, false);
            }

            // apply the padding to get the internal rect
            area.xMin += Utility.Padding;
            area.xMax -= Utility.Padding;
            area.height -= 2;
            area.y += 1;

            const float spacing = 15;
            float columnWidth = area.width / 2 - spacing;

            EditorGUI.indentLevel++;
            for (int i = 0; i < _propertyCountCache; i++)
            {
                EditorGUI.PropertyField(
                    new Rect(area.x, area.y + EditorGUIUtility.singleLineHeight * i, columnWidth,
                        EditorGUIUtility.singleLineHeight), _propertyKeys.GetArrayElementAtIndex(i), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(area.x + spacing + columnWidth, area.y + EditorGUIUtility.singleLineHeight * i,
                        columnWidth, EditorGUIUtility.singleLineHeight), _propertyValues.GetArrayElementAtIndex(i),
                    GUIContent.none);
            }
            EditorGUI.indentLevel--;
        }

        private void AddNewEntry()
        {
            if (_propertyCountCache == -1 || _propertyKeys == null || _propertyValues == null)
            {
                return;
            }

            // Increase Size
            _propertyCountCache++;

            // Keys need to be unique
            _propertyKeys.arraySize = _propertyCountCache;

            SerializedProperty addedKey = _propertyKeys.GetArrayElementAtIndex(_propertyCountCache - 1);
            Utility.ShallowCopy(addedKey, _propertyAddKey);

            _propertyValues.arraySize = _propertyCountCache;
            _propertyCount.intValue = _propertyCountCache;
        }

        private void RemoveLastEntry()
        {
            if (_propertyCountCache == -1 || _propertyKeys == null || _propertyValues == null)
            {
                return;
            }

            _propertyCountCache--;
            if (_propertyCountCache < 0)
            {
                _propertyCountCache = 0;
            }

            _propertyKeys.arraySize = _propertyCountCache;
            _propertyValues.arraySize = _propertyCountCache;
            _propertyCount.intValue = _propertyCountCache;
        }





        
        private static class Utility
        {
            public const int dragHandleWidth = 20;
            public const int propertyDrawerPadding = 8;
            public const int minHeaderHeight = 2;


            public const int Padding = 6;

            public const float ButtonHorizontalPadding = 4f;
            public const float ButtonVerticalPadding = 2f;
            public const float MarginBottom = 2f;
            public const float ButtonOffset = 6.5f;
            public const float ButtonWidth = 25f;
            public const float ButtonHeight = 16f;

            public static readonly GUIContent iconToolbarPlus =
                EditorGUIUtility.TrIconContent("Toolbar Plus", "Add to list");

            public static GUIContent iconToolbarPlusMore =
                EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");

            public static readonly GUIContent iconToolbarMinus =
                EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection or last element from list");

            public static readonly GUIStyle draggingHandle = "RL DragHandle";


            public static readonly GUIStyle boxBackground = "RL Background";
            public static readonly GUIStyle preButton = "RL FooterButton";
            public static readonly GUIStyle elementBackground = "RL Element";
            public static readonly string undoAdd = "Add Element To Array";
            public static readonly string undoRemove = "Remove Element From Array";
            public static readonly string undoMove = "Reorder Element In Array";

            public static readonly Rect infinityRect = new Rect(float.NegativeInfinity, float.NegativeInfinity,
                float.PositiveInfinity, float.PositiveInfinity);

            public static readonly GUIContent EmptyDictionaryContent = new GUIContent("Dictionary is Empty");

            public static readonly GUIStyle RLEmptyHeader = "RL Empty Header";
            public static readonly GUIStyle FooterBackground = "RL Footer";
            public static readonly GUIStyle ButtonBackground = "RL Footer";
            public static readonly GUIStyle RLHeader = "RL Header";

            /// <summary>
            ///     A single level copy of value from the <paramref name="sourceProperty" /> to the <paramref name="targetProperty" />.
            /// </summary>
            /// <param name="targetProperty">The receiver of the value.</param>
            /// <param name="sourceProperty">The originator of the value to be used.</param>
            public static void ShallowCopy(SerializedProperty targetProperty, SerializedProperty sourceProperty)
            {
                switch (targetProperty.type)
                {
                    case "int":
                        targetProperty.intValue = sourceProperty.intValue;
                        break;
                    case "bool":
                        targetProperty.boolValue = sourceProperty.boolValue;
                        break;
                    case "bounds":
                        targetProperty.boundsValue = sourceProperty.boundsValue;
                        break;
                    case "color":
                        targetProperty.colorValue = sourceProperty.colorValue;
                        break;
                    case "double":
                        targetProperty.doubleValue = sourceProperty.doubleValue;
                        break;
                    case "float":
                        targetProperty.floatValue = sourceProperty.floatValue;
                        break;
                    case "long":
                        targetProperty.longValue = sourceProperty.longValue;
                        break;
                    case "quaternion":
                        targetProperty.quaternionValue = sourceProperty.quaternionValue;
                        break;
                    case "rect":
                        targetProperty.rectValue = sourceProperty.rectValue;
                        break;
                    case "string":
                        targetProperty.stringValue = sourceProperty.stringValue;
                        break;
                    case "vector2":
                        targetProperty.vector2Value = sourceProperty.vector2Value;
                        break;
                    case "vector3":
                        targetProperty.vector3Value = sourceProperty.vector3Value;
                        break;
                    case "vector4":
                        targetProperty.vector4Value = sourceProperty.vector4Value;
                        break;
                    case "animationCurve":
                        targetProperty.animationCurveValue = sourceProperty.animationCurveValue;
                        break;
                    case "boundsInt":
                        targetProperty.boundsIntValue = sourceProperty.boundsIntValue;
                        break;
                    case "enum":
                        targetProperty.enumValueIndex = sourceProperty.enumValueIndex;
                        break;
                    case "exposedReference":
                        targetProperty.exposedReferenceValue = sourceProperty.exposedReferenceValue;
                        break;
                    case "object":
                        targetProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                        targetProperty.objectReferenceInstanceIDValue = sourceProperty.objectReferenceInstanceIDValue;
                        break;
                }
            }
        }
    }
}