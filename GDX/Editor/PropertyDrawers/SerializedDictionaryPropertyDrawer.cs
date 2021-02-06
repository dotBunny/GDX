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

        private float _heightTotal;
        private float _heightFoldout;
        private float _heightContent;
        private float _heightContentHeader;
        private float _heightContentElements;
        private float _heightContentFooter;
        private float _heightFooter;


        private bool _validKeyValueCount = true;

        /// <summary>
        ///     Provide an adjusted height for the entire element to be rendered.
        /// </summary>
        /// <remarks>Makes use of caching of heights to different sections of the property drawer.</remarks>
        /// <param name="property">The target <see cref="SerializedProperty" /> which is being evaluated.</param>
        /// <param name="label">Ignored.</param>
        /// <returns>Required height for element to be drawn.</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // In the case that we just have the foldout showing, we want the property drawer to just be a single line
            _heightFoldout = EditorGUIUtility.singleLineHeight;

            // Early out if the property drawer is not expanded
            if (!_propertyExpandedCache)
            {
                _heightTotal = _heightFoldout;
                return _heightTotal;
            }

            // Determine the height of the elements portion of the content area
            _heightContentHeader = 4;
            _heightContentElements = (EditorGUIUtility.singleLineHeight + Styles.ContentAreaElementSpacing) *
                Mathf.Max(
                    1, _propertyCountCache) - Styles.ContentAreaElementSpacing;
            _heightContentFooter = 4;
            _heightContent = _heightContentHeader +
                             _heightContentElements +
                             _heightContentFooter;

            // Figure out our footer total height
            _heightFooter = Styles.ActionButtonVerticalPadding +
                            EditorGUIUtility.singleLineHeight +
                            Styles.ActionButtonVerticalPadding;

            // Add up our total
            _heightTotal = _heightFoldout + Styles.ContentAreaTopMargin + _heightContent + _heightFooter;

            return _heightTotal;
        }

        /// <summary>
        ///     Generate some sort of inspector for <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="position">The <see cref="Rect" /> to draw into.</param>
        /// <param name="property">The <see cref="SerializedProperty" /> to query for data.</param>
        /// <param name="label">An ignored label value.</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
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
            Rect foldoutRect = new Rect(position.x, position.y, position.width, _heightFoldout);
            DrawFoldout(foldoutRect);

            // If the foldout is expanded, draw the actual content
            if (_propertyExpandedCache)
            {
                Rect contentRect = new Rect(position.x, foldoutRect.yMax + Styles.ContentAreaTopMargin , position.width, _heightContent);
                DrawContentEditor(contentRect);
                Rect footerRect = new Rect(position.x, contentRect.yMax, position.width, _heightFooter);
                DrawFooterActions(footerRect);
            }

            // Anything we changed property wise we should save
            property.serializedObject.ApplyModifiedProperties();
        }

        private void DrawContentEditor(Rect position)
        {
            // Paint the background
            if (Event.current.type == EventType.Repaint)
            {
                Rect headerBackgroundRect = new Rect(position.x, position.y, position.width, _heightContentHeader);

                // The extra 2 pixels are used to get rid of the rounded corners on the content box
                Rect contentBackgroundRect = new Rect(position.x, headerBackgroundRect.yMax, position.width,
                    _heightContentElements + 2);
                Rect footerBackgroundRect = new Rect(position.x, contentBackgroundRect.yMax - 2, position.width,
                    _heightContentFooter);

                Styles.BoxBackground.Draw(contentBackgroundRect, false, false, false, false);

                Styles.HeaderBackground.Draw(headerBackgroundRect, false, false, false, false);
                Styles.FooterBackground.Draw(footerBackgroundRect, false, false, false, false);
            }

            // Bring in the provided rect
            position.yMin += _heightContentHeader;
            position.yMax -= _heightContentFooter;
            position.xMin += Styles.ContentAreaHorizontalPadding;
            position.xMax -= Styles.ContentAreaHorizontalPadding;

            // If we have nothing simply display the message
            if (_propertyCountCache == 0)
            {
                EditorGUI.LabelField(position, Content.EmptyDictionary, EditorStyles.label);
                return;
            }


            float columnWidth = (position.width - 34) / 2f;



            for (int i = 0; i < _propertyCountCache; i++)
            {
                float topOffset = (EditorGUIUtility.singleLineHeight + Styles.ContentAreaElementSpacing) * i;

                // Draw Key Icon
                Rect keyIconRect = new Rect(position.x - 2, position.y + topOffset - 1, 17, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(keyIconRect, Content.IconKey);

                // Draw Key Property
                Rect keyPropertyRect = new Rect(keyIconRect.xMax, position.y + topOffset, columnWidth,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(keyPropertyRect, _propertyKeys.GetArrayElementAtIndex(i), GUIContent.none);

                // Draw Value Icon
                Rect valueIconRect = new Rect(keyPropertyRect.xMax - 2, position.y + topOffset - 1, 17, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(valueIconRect, Content.IconValue);

                // Draw Value Property
                Rect valuePropertyRect = new Rect(valueIconRect.xMax , position.y + topOffset, columnWidth, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(valuePropertyRect, _propertyValues.GetArrayElementAtIndex(i), GUIContent.none);
            }
        }

        /// <summary>
        ///     Draw actionable buttons offset of the footer.
        /// </summary>
        /// <param name="position">
        ///     A <see cref="Rect" /> representing the space which the footer will be drawn, so that he the
        ///     buttons can draw offset of it.
        /// </param>
        private void DrawFooterActions(Rect position)
        {
            float inputWidth = position.width / 2 - Styles.ActionButtonWidth - Styles.ActionButtonHorizontalPadding;
            Rect addBackgroundRect = new Rect(position.xMin + 10f, position.y,
                Styles.ActionButtonWidth + inputWidth + Styles.ActionButtonHorizontalPadding * 2,
                position.height);

            // Create button rects
            Rect addRect = new Rect(addBackgroundRect.xMin + Styles.ActionButtonHorizontalPadding, addBackgroundRect.yMin + Styles.ActionButtonVerticalPadding,
                Styles.ActionButtonWidth, Styles.ActionButtonHeight);
            Rect inputRect = new Rect(addBackgroundRect.xMin + Styles.ActionButtonWidth + Styles.ActionButtonHorizontalPadding,
                addBackgroundRect.yMin + Styles.ActionButtonVerticalPadding, inputWidth, Styles.ActionButtonHeight);

            if (Event.current.type == EventType.Repaint)
            {
                Styles.ButtonBackground.Draw(addBackgroundRect, false, false, false, false);
            }

            // Draw the input before the button so it overlaps it
            EditorGUI.PropertyField(inputRect, _propertyAddKey, GUIContent.none);

            GUI.enabled = _propertyAddKeyValidCache;
            if (GUI.Button(addRect, Content.IconPlus, Styles.FooterButton))
            {
                AddNewEntry();
            }

            GUI.enabled = true;


            // Remove button
            // TODO: Only enable when selecting?
            if (_propertyCountCache <= 0)
            {
                return;
            }

            Rect removeBackground = new Rect(
                position.xMax - (10 + Styles.ActionButtonWidth + Styles.ActionButtonHorizontalPadding * 2),
                position.y,
                Styles.ActionButtonWidth + Styles.ActionButtonHorizontalPadding * 2,
                position.height);

            if (Event.current.type == EventType.Repaint)
            {
                Styles.ButtonBackground.Draw(removeBackground, false, false, false, false);
            }

            if (GUI.Button(
                new Rect(
                    removeBackground.xMin + Styles.ActionButtonHorizontalPadding,
                    removeBackground.yMin  + Styles.ActionButtonVerticalPadding,
                    Styles.ActionButtonWidth, Styles.ActionButtonHeight),
                Content.IconMinus, Styles.FooterButton))
            {
                RemoveLastEntry();
            }
        }

        /// <summary>
        ///     Draw the foldout header for the <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="position">A <see cref="Rect" /> representing the space which the header will consume in its entirety.</param>
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
            ShallowCopy(addedKey, _propertyAddKey);

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

        /// <summary>
        ///     A single level copy of value from the <paramref name="sourceProperty" /> to the <paramref name="targetProperty" />.
        /// </summary>
        /// <param name="targetProperty">The receiver of the value.</param>
        /// <param name="sourceProperty">The originator of the value to be used.</param>
        private static void ShallowCopy(SerializedProperty targetProperty, SerializedProperty sourceProperty)
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

        private static class Styles
        {
            /// <summary>
            /// The width of an individual button in the footer.
            /// </summary>
            public const float ActionButtonWidth = 25f;

            /// <summary>
            /// The height of an individual button in the footer.
            /// </summary>
            public const float ActionButtonHeight = 16f;

            public const float ActionButtonHorizontalPadding = 4f;

            public const float ActionButtonVerticalPadding = 1f;

            /// <summary>
            ///     The distance from the foldout to the content area.
            /// </summary>
            public const int ContentAreaTopMargin = 2;

            /// <summary>
            ///     The horizontal padding of the content area, subtracted from both sides.
            /// </summary>
            public const int ContentAreaHorizontalPadding = 6;

            /// <summary>
            /// The space between each element in the content area.
            /// </summary>
            public const int ContentAreaElementSpacing = 3;

            public static readonly GUIStyle BoxBackground = "RL Background";
            public static readonly GUIStyle HeaderBackground = "RL Empty Header";
            public static readonly GUIStyle FooterBackground = "RL Footer";
            public static readonly GUIStyle ButtonBackground = "RL Footer";
            public static readonly GUIStyle FooterButton = "RL FooterButton";


            public static readonly GUIStyle elementBackground = "RL Element";

            /// <summary>
            ///     A few last minute changes to settings.
            /// </summary>
            static Styles()
            {
                HeaderBackground.fixedHeight = 0;
                FooterBackground.fixedHeight = 0;
            }
        }

        private static class Content
        {
            public static readonly GUIContent EmptyDictionary = new GUIContent("Dictionary is Empty");

            public static readonly GUIContent IconPlus =
                EditorGUIUtility.IconContent("Toolbar Plus", "Add element with provided key to the dictionary.");

            public static readonly GUIContent IconMinus =
                EditorGUIUtility.IconContent("Toolbar Minus", "Remove the selected element from the dictionary.");

            public static readonly GUIContent IconKey =
                EditorGUIUtility.IconContent("animationkeyframe", "Key");
            public static readonly GUIContent IconValue =
                EditorGUIUtility.IconContent("animationanimated", "Value");

        }
    }
}