// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEditor;
using UnityEngine;
using GDX.Classic.Collections.Generic;

namespace GDX.Editor.PropertyDrawers
{
#if UNITY_2020_1_OR_NEWER
    /// <summary>
    ///     A <see cref="PropertyDrawer" /> for <see cref="SerializableDictionary{TKey,TValue}" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Requires Unity 2020.1+ for the PropertyDrawer to function, as that's the point where generic attributes became
    ///         available on the editor side. While this doesnt produce exception if not wrapped by the UNITY_2020_1_OR_NEWER
    ///         it made sense to keep it just to explicitly show the version required.
    ///     </para>
    ///     <para>
    ///         Heavily influenced by
    ///         https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GUI/ReorderableList.cs
    ///     </para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    internal class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        /// <summary>
        ///     Cached ID of the <see cref="EditorGUI.PropertyField(UnityEngine.Rect,UnityEditor.SerializedProperty)" />, used to
        ///     determine if it has focus across updates.
        /// </summary>
        private string _addKeyFieldID;

        /// <summary>
        ///     A cached version of the display name to use for the <see cref="PropertyDrawer" />.
        /// </summary>
        private string _displayName = "Serializable Dictionary";

        /// <summary>
        ///     The cached calculated height of the entire content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightContent;

        /// <summary>
        ///     The cached calculated height of the elements in the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightContentElements;

        /// <summary>
        ///     The cached calculated height of the footer for the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightContentFooter;

        /// <summary>
        ///     The cached calculated height of the header for the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightContentHeader;

        /// <summary>
        ///     The cached calculated height of the foldout portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightFoldout;

        /// <summary>
        ///     The cached calculated height of the footer portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightFooter;

        /// <summary>
        ///     The cached calculated total height of the <see cref="PropertyDrawer" />.
        /// </summary>
        private float _heightTotal;

        /// <summary>
        ///     Does the key and value array counts match, and do they match the serialized length?
        /// </summary>
        /// <remarks>
        ///     Used as a bit of a checksum for the serialized data.
        /// </remarks>
        private bool _isKeyValueCountValid = true;

        private SerializedProperty _propertyAddKey;
        private SerializedProperty _propertyAddKeyValid;
        private bool _propertyAddKeyValidCache;
        private SerializedProperty _propertyCount;
        private int _propertyCountCache = -1;
        private SerializedProperty _propertyExpanded;
        private bool _propertyExpandedCache;
        private SerializedProperty _propertyIsSerializable;
        private bool _propertyIsSerializableCache;
        private SerializedProperty _propertyKeys;
        private SerializedProperty _propertyValues;

        /// <summary>
        ///     The index of in the data arrays to be considered selected.
        /// </summary>
        private int _selectedIndex = -1;

        /// <summary>
        ///     The target object of the <see cref="PropertyDrawer" />.
        /// </summary>
        private Object _targetObject;

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

            // Early out if the property drawer is not expanded, or we dont have any data
            if (!_propertyExpandedCache || !_propertyIsSerializableCache)
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
            // Safe to cache!
            _displayName = property.displayName;
            _addKeyFieldID = Content.AddKeyFieldIDPrefix + _displayName.GetStableHashCode();
            _targetObject = property.serializedObject.targetObject;

            // Build our top level position
            Rect foldoutRect = new Rect(position.x, position.y, position.width, _heightFoldout);

            // Editor only properties
            _propertyIsSerializable = property.FindPropertyRelative("isSerializable");
            _propertyIsSerializableCache = _propertyIsSerializable.boolValue;
            if (!_propertyIsSerializableCache)
            {
                DrawErrorMessage(foldoutRect, Content.InvalidTypesError);
                return;
            }

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
            _isKeyValueCountValid = _propertyCountCache == _propertyValues.arraySize &&
                                    _propertyCountCache == _propertyKeys.arraySize;

            if (!_isKeyValueCountValid)
            {
                DrawErrorMessage(foldoutRect, Content.CorruptDataError, true);
                return;
            }

            // Draw the foldout at the top of the space
            DrawFoldout(foldoutRect);

            // If the foldout is expanded, draw the actual content
            if (_propertyExpandedCache)
            {
                Rect contentRect = new Rect(position.x, foldoutRect.yMax + Styles.ContentAreaTopMargin, position.width,
                    _heightContent);
                DrawContentArea(contentRect);
                Rect footerRect = new Rect(position.x, contentRect.yMax, position.width, _heightFooter);
                DrawFooterActions(footerRect);
            }

            // Create undo point if we've changed something
            if (GUI.changed)
            {
                Undo.SetCurrentGroupName("Serializable Dictionary Action");
            }

            // Anything we changed property wise we should save
            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        ///     Resets the Add Key serialized data to default, if it is not already defaulted.
        /// </summary>
        private void ClearAddKeyReference()
        {
            // If we already default, dont bother.
            if (!HasValue(_propertyAddKey))
            {
                return;
            }

            DefaultValue(_propertyAddKey);
            _propertyAddKey.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        ///     Draw the content area, including elements.
        /// </summary>
        /// <param name="position">A <see cref="Rect" /> representing the space which the content area will be drawn.</param>
        private void DrawContentArea(Rect position)
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

                Rect selectionRect = new Rect(
                    position.x - Styles.ContentAreaHorizontalPadding + 1,
                    position.y + topOffset - 2,
                    position.width + Styles.ContentAreaHorizontalPadding * 2 - 3,
                    EditorGUIUtility.singleLineHeight + 4);

                // Handle selection (left-click), do not consume/use the event so that fields receive.
                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0 &&
                    selectionRect.Contains(Event.current.mousePosition))
                {
                    _selectedIndex = i;

                    // Attempt to force a redraw of the inspector
                    EditorUtility.SetDirty(_targetObject);
                }

                if (i == _selectedIndex)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        Styles.ElementBackground.Draw(selectionRect, false, true, true, true);
                    }
                }

                // Draw Key Icon
#if UNITY_2021_1_OR_NEWER
                Rect keyIconRect =
                    new Rect(position.x - 2, position.y + topOffset - 1, 17, EditorGUIUtility.singleLineHeight);
#else
                Rect keyIconRect = new Rect(position.x, position.y + topOffset, 17,
                    EditorGUIUtility.singleLineHeight);
#endif
                EditorGUI.LabelField(keyIconRect, Content.IconKey);

                // Draw Key Property
                Rect keyPropertyRect = new Rect(keyIconRect.xMax, position.y + topOffset, columnWidth,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(keyPropertyRect, _propertyKeys.GetArrayElementAtIndex(i), GUIContent.none);

                // Draw Value Icon
                Rect valueIconRect = new Rect(keyPropertyRect.xMax + 3, position.y + topOffset - 1, 17,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(valueIconRect, Content.IconValue);

                // Draw Value Property
                Rect valuePropertyRect = new Rect(valueIconRect.xMax, position.y + topOffset, columnWidth - 1,
                    EditorGUIUtility.singleLineHeight);
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
            Rect addRect = new Rect(addBackgroundRect.xMin + Styles.ActionButtonHorizontalPadding,
                addBackgroundRect.yMin + Styles.ActionButtonVerticalPadding,
                Styles.ActionButtonWidth, Styles.ActionButtonHeight);
            Rect inputRect = new Rect(
                addBackgroundRect.xMin + Styles.ActionButtonWidth + Styles.ActionButtonHorizontalPadding,
                addBackgroundRect.yMin + Styles.ActionButtonVerticalPadding, inputWidth, Styles.ActionButtonHeight);

            if (Event.current.type == EventType.Repaint)
            {
                Styles.ButtonBackground.Draw(addBackgroundRect, false, false, false, false);
            }

            // Hitting enter while the add key field is selected
            if (_propertyAddKeyValidCache &&
                Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ||
                 Event.current.character == '\n') &&
                GUI.GetNameOfFocusedControl() == _addKeyFieldID)
            {
                Event.current.Use();
                GUIUtility.hotControl = 0;
                AddElement();
            }

            // Draw the input before the button so it overlaps it
            GUI.SetNextControlName(_addKeyFieldID);
            EditorGUI.PropertyField(inputRect, _propertyAddKey, GUIContent.none);

            GUI.enabled = _propertyAddKeyValidCache;
            if (GUI.Button(addRect, Content.IconPlus, Styles.FooterButton))
            {
                // Remove control focus
                GUIUtility.hotControl = 0;

                AddElement();
            }

            GUI.enabled = true;

            // Only visible when we have something selected (with a failsafe on item count)
            if (_propertyCountCache == 0 || _selectedIndex == -1)
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
                    removeBackground.yMin + Styles.ActionButtonVerticalPadding,
                    Styles.ActionButtonWidth, Styles.ActionButtonHeight),
                Content.IconMinus, Styles.FooterButton))
            {
                // Remove control focus
                GUIUtility.hotControl = 0;
                RemoveElementAt(_selectedIndex);

                // Keep next item selected.
                _selectedIndex--;
                if (_selectedIndex >= 0)
                {
                    return;
                }

                if (_propertyCountCache > 1)
                {
                    _selectedIndex = 0;
                }
                else
                {
                    _selectedIndex = -1;
                }
            }
        }

        /// <summary>
        ///     Draw the foldout header for the <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="position">A <see cref="Rect" /> representing the space which the header will consume in its entirety.</param>
        private void DrawFoldout(Rect position)
        {
            // Generate a foldout GUI element representing the name of the dictionary
            bool newExpanded =
                EditorGUI.Foldout(position,
                    _propertyExpanded.boolValue, _displayName, true, EditorStyles.foldout);
            if (newExpanded != _propertyExpandedCache)
            {
                // TODO: Is there some editor cache for whats unfolded? This would prevent over serialization / extra data
                _propertyExpanded.boolValue = newExpanded;
                _propertyExpandedCache = newExpanded;

                // If we're collapsing the foldout, make sure there is no key cached
                if (!newExpanded)
                {
                    ClearAddKeyReference();
                }
            }

            // Indicate Count - but ready only
            GUI.enabled = false;
            const int itemCountSize = 48;
            EditorGUI.TextField(new Rect(position.x + position.width - itemCountSize,
                position.y, itemCountSize, position.height), _propertyCountCache.ToString());
            GUI.enabled = true;
        }

        /// <summary>
        ///     Draw a message instead of the normal property drawer.
        /// </summary>
        /// <remarks>Useful for when data is corrupted, or types are incompatible.</remarks>
        /// <param name="position">A <see cref="Rect" /> representing where it should be drawn.</param>
        /// <param name="tooltip">The message to display when mousing over.</param>
        /// <param name="displayResetButton">Should a reset button be displayed?</param>
        private void DrawErrorMessage(Rect position, string tooltip, bool displayResetButton = false)
        {
            Rect iconRect = new Rect(position.x - 16, position.y, 16, position.height);
            Content.IconError.tooltip = tooltip;
            EditorGUI.LabelField(iconRect, Content.IconError);
            Rect messageRect = new Rect(position.x, position.y, position.width - 17, position.height);
            EditorGUI.LabelField(messageRect, new GUIContent(_displayName, tooltip), EditorStyles.label);

            if (!displayResetButton)
            {
                return;
            }

            Rect buttonRect = new Rect(position.width, position.y, 17, position.height);
            if (GUI.Button(buttonRect, Content.IconTrash, GUIStyle.none))
            {
                ResetSerializedData();
            }
        }


        /// <summary>
        ///     Add an element to the targeted <see cref="SerializableDictionary{TKey,TValue}" /> using the provided key.
        /// </summary>
        /// <remarks>
        ///     This alters the underlying backing serialized data, after this change, the actual
        ///     <see cref="SerializableDictionary{TKey,TValue}" /> picks up the change.
        /// </remarks>
        /// <param name="defaultValue">
        ///     Should the value be reset to its default value. It holds the previous element in the array's
        ///     value due to how the value array was expanded.
        /// </param>
        private void AddElement(bool defaultValue = true)
        {
            if (_propertyCountCache == -1 || _propertyKeys == null || _propertyValues == null)
            {
                return;
            }

            // Add new key element, and fill with predetermined good key
            _propertyKeys.arraySize++;
            SerializedProperty addedKey = _propertyKeys.GetArrayElementAtIndex(_propertyCountCache);
            ShallowCopy(addedKey, _propertyAddKey);

            // Add new value element, and optionally default its value
            _propertyValues.arraySize++;
            if (defaultValue)
            {
                SerializedProperty addedValue = _propertyValues.GetArrayElementAtIndex(_propertyCountCache);
                DefaultValue(addedValue);
            }

            // Increase Size
            _propertyCountCache++;
            _propertyCount.intValue = _propertyCountCache;

            ClearAddKeyReference();
        }

        /// <summary>
        ///     Does the provided <see cref="SerializedProperty" /> contain a non <c>default</c> value.
        /// </summary>
        /// <param name="targetProperty">The target to evaluate.</param>
        private static bool HasValue(SerializedProperty targetProperty)
        {
            switch (targetProperty.type)
            {
                case "int":
                    return targetProperty.intValue != 0;
                case "bool":
                    return targetProperty.boolValue;
                case "bounds":
                    return targetProperty.boundsValue != default;
                case "color":
                    return targetProperty.colorValue != default;
                case "double":
                    return targetProperty.doubleValue != 0;
                case "float":
                    return targetProperty.floatValue != 0;
                case "long":
                    return targetProperty.longValue != 0;
                case "quaternion":
                    return targetProperty.quaternionValue != default;
                case "rect":
                    return targetProperty.rectValue != default;
                case "string":
                    return targetProperty.stringValue != default;
                case "vector2":
                    return targetProperty.vector2Value != default;
                case "vector3":
                    return targetProperty.vector3Value != default;
                case "vector4":
                    return targetProperty.vector4Value != default;
                case "animationCurve":
                    return targetProperty.animationCurveValue.keys.Length > 0;
                case "boundsInt":
                    return targetProperty.boundsIntValue != default;
                case "enum":
                    return targetProperty.enumValueIndex != default;
                case "exposedReference":
                    return targetProperty.exposedReferenceValue != default;
                default:
                    return targetProperty.objectReferenceValue != default ||
                           targetProperty.objectReferenceInstanceIDValue != default;
            }
        }

        /// <summary>
        ///     Remove an element from the targeted <see cref="SerializableDictionary{TKey,TValue}" /> at the provided index.
        /// </summary>
        /// <remarks>
        ///     This alters the underlying backing serialized data, after this change, the actual
        ///     <see cref="SerializableDictionary{TKey,TValue}" /> picks up the change.
        /// </remarks>
        /// <param name="index">The index (really ordered serialized data) to remove.</param>
        private void RemoveElementAt(int index)
        {
            if (index == -1)
            {
                _selectedIndex = -1;
            }
            else if (_propertyKeys != null && _propertyValues != null)
            {
                _propertyKeys.DeleteArrayElementAtIndex(index);
                _propertyValues.DeleteArrayElementAtIndex(index);

                _propertyCountCache--;
                if (_propertyCountCache < 0)
                {
                    _propertyCountCache = 0;
                }

                _propertyCount.intValue = _propertyCountCache;
            }
        }

        /// <summary>
        ///     Reset the serialized data as if there were no entries in the dictionary.
        /// </summary>
        private void ResetSerializedData()
        {
            _propertyKeys.ClearArray();
            _propertyValues.ClearArray();
            _propertyCount.intValue = 0;
            _propertyCountCache = 0;
            _selectedIndex = -1;
        }

        /// <summary>
        ///     Clear the value of a provided <see cref="SerializedProperty" />, setting it to the <c>default</c>.
        /// </summary>
        /// <param name="targetProperty">The receiver of the value.</param>
        private static void DefaultValue(SerializedProperty targetProperty)
        {
            switch (targetProperty.type)
            {
                case "int":
                    targetProperty.intValue = default;
                    break;
                case "bool":
                    targetProperty.boolValue = default;
                    break;
                case "bounds":
                    targetProperty.boundsValue = default;
                    break;
                case "color":
                    targetProperty.colorValue = default;
                    break;
                case "double":
                    targetProperty.doubleValue = default;
                    break;
                case "float":
                    targetProperty.floatValue = default;
                    break;
                case "long":
                    targetProperty.longValue = default;
                    break;
                case "quaternion":
                    targetProperty.quaternionValue = default;
                    break;
                case "rect":
                    targetProperty.rectValue = default;
                    break;
                case "string":
                    targetProperty.stringValue = default;
                    break;
                case "vector2":
                    targetProperty.vector2Value = default;
                    break;
                case "vector3":
                    targetProperty.vector3Value = default;
                    break;
                case "vector4":
                    targetProperty.vector4Value = default;
                    break;
                case "animationCurve":
                    targetProperty.animationCurveValue = default;
                    break;
                case "boundsInt":
                    targetProperty.boundsIntValue = default;
                    break;
                case "enum":
                    targetProperty.enumValueIndex = default;
                    break;
                case "exposedReference":
                    targetProperty.exposedReferenceValue = default;
                    break;
                default:
                    // This covers the whole "PPtr<$NAME> outcome, could probably handle differently.
                    targetProperty.objectReferenceValue = default;
                    targetProperty.objectReferenceInstanceIDValue = default;
                    break;
            }
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
                default:
                    // This covers the whole "PPtr<$NAME> outcome, could probably handle differently.
                    targetProperty.objectReferenceValue = sourceProperty.objectReferenceValue;
                    targetProperty.objectReferenceInstanceIDValue = sourceProperty.objectReferenceInstanceIDValue;
                    break;
            }
        }

        /// <summary>
        ///     <see cref="PropertyDrawer" /> Fixed Content.
        /// </summary>
        private static class Content
        {
            /// <summary>
            ///     The prefix of the <see cref="string" /> identifier of the add field.
            /// </summary>
            public const string AddKeyFieldIDPrefix = "SD_KeyToAdd_";

            /// <summary>
            ///     Message relayed when bad serialized data is detected.
            /// </summary>
            public const string CorruptDataError =
                "Serialized data is checked that the number of keys and values match, as well as the cached length. If any of these does not match the data is considered corrupt.";

            /// <summary>
            ///     Message relayed when an invalid type is attempted to be serialized.
            /// </summary>
            public const string InvalidTypesError =
                "System.Object is not compatible with Unity's serialization system.";

            /// <summary>
            ///     Message displayed when no content is found in the serialized data.
            /// </summary>
            public static readonly GUIContent EmptyDictionary = new GUIContent("Dictionary is Empty");

            /// <summary>
            ///     A white exclamation mark, surround by a red hexagon.
            /// </summary>
            public static readonly GUIContent IconError = EditorGUIUtility.IconContent("Error", "Issue Detected!");

            /// <summary>
            ///     A plus symbol.
            /// </summary>
            public static readonly GUIContent IconPlus =
                EditorGUIUtility.IconContent("Toolbar Plus", "Add element with provided key to the dictionary.");

            /// <summary>
            ///     A minus symbol.
            /// </summary>
            public static readonly GUIContent IconMinus =
                EditorGUIUtility.IconContent("Toolbar Minus", "Remove the selected element from the dictionary.");

            /// <summary>
            ///     A key-frame indicator, attempting to relay the concept of a key.
            /// </summary>
            public static readonly GUIContent IconKey =
                // ReSharper disable once StringLiteralTypo
                EditorGUIUtility.IconContent("animationkeyframe", "Key");

            /// <summary>
            ///     An eraser icon.
            /// </summary>
            public static readonly GUIContent IconTrash =
                EditorGUIUtility.IconContent("Grid.EraserTool",
                    "Reset");

            /// <summary>
            ///     An animation dope sheet value indicator.
            /// </summary>
            public static readonly GUIContent IconValue =
                // ReSharper disable once StringLiteralTypo
                EditorGUIUtility.IconContent("animationanimated", "Value");
        }

        /// <summary>
        ///     <see cref="PropertyDrawer" /> Styles.
        /// </summary>
        private static class Styles
        {
            /// <summary>
            ///     The width of an individual button in the footer.
            /// </summary>
            public const float ActionButtonWidth = 20f;

            /// <summary>
            ///     The height of an individual button in the footer.
            /// </summary>
            public const float ActionButtonHeight = 16f;

            /// <summary>
            ///     The horizontal padding around a button, making it appear wider then it would normally.
            /// </summary>
            public const float ActionButtonHorizontalPadding = 4f;

            /// <summary>
            ///     The vertical padding around a button, making it appear taller then it would normally.
            /// </summary>
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
            ///     The space between each element in the content area.
            /// </summary>
            public const int ContentAreaElementSpacing = 3;

            /// <summary>
            ///     The reorderable list's main background.
            /// </summary>
            public static readonly GUIStyle BoxBackground = "RL Background";

            /// <summary>
            ///     The reorderable list's footer button background.
            /// </summary>
            public static readonly GUIStyle ButtonBackground = "RL Footer";

            /// <summary>
            ///     The reorderable list's element background.
            /// </summary>
            public static readonly GUIStyle ElementBackground = "RL Element";

            /// <summary>
            ///     The reorderable list's footer background.
            /// </summary>
            public static readonly GUIStyle FooterBackground = "RL Footer";

            /// <summary>
            ///     The reorderable list's footer buttons.
            /// </summary>
            public static readonly GUIStyle FooterButton = "RL FooterButton";


            /// <summary>
            ///     The reorderable list's empty header.
            /// </summary>
            public static readonly GUIStyle HeaderBackground = "RL Empty Header";

            /// <summary>
            ///     Initialize some of the styles slightly different then expected.
            /// </summary>
            static Styles()
            {
                HeaderBackground.fixedHeight = 0;
                FooterBackground.fixedHeight = 0;
            }
        }
    }
#endif // UNITY_2020_1_OR_NEWER
}