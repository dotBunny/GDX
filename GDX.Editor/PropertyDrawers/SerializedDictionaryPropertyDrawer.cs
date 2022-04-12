// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
using UnityEditor;
using UnityEngine;
using GDX.Collections.Generic;
using Unity.VisualScripting;
using Object = UnityEngine.Object;

namespace GDX.Editor.PropertyDrawers
{
#if !UNITY_DOTSRUNTIME
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
    class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        public object m_AddKey;
        public bool m_IsExpanded;
        public bool m_ValidKey;


        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        ///     At a certain point (we noticed in 2020.3.33f1), Unity switched to caring about the names of fields
        ///     that were being serialized. So our old method of switching out field data at build time and padding the
        ///     required space was invalidated.
        /// </remarks>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        bool IsValidObjectKey(object targetObject)
        {
            object instanceObject = SerializedProperties.GetValue(m_TargetObject, m_TargetProperty.name);
            IDictionary instanceDictionary = (IDictionary)instanceObject;
            if (instanceDictionary == null) return false;
            return !instanceDictionary.Contains(targetObject);
        }

        /// <summary>
        ///     Cached ID of the <see cref="EditorGUI.PropertyField(UnityEngine.Rect,UnityEditor.SerializedProperty)" />, used to
        ///     determine if it has focus across updates.
        /// </summary>
        string m_AddKeyFieldID;

        /// <summary>
        ///     A cached version of the display name to use for the <see cref="PropertyDrawer" />.
        /// </summary>
        string m_DisplayName = "Serializable Dictionary";

        /// <summary>
        ///     The cached calculated height of the entire content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightContent;

        /// <summary>
        ///     The cached calculated height of the elements in the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightContentElements;

        /// <summary>
        ///     The cached calculated height of the footer for the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightContentFooter;

        /// <summary>
        ///     The cached calculated height of the header for the content portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightContentHeader;

        /// <summary>
        ///     The cached calculated height of the foldout portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightFoldout;

        /// <summary>
        ///     The cached calculated height of the footer portion of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightFooter;

        /// <summary>
        ///     The cached calculated total height of the <see cref="PropertyDrawer" />.
        /// </summary>
        float m_HeightTotal;

        /// <summary>
        ///     Does the key and value array counts match, and do they match the serialized length?
        /// </summary>
        /// <remarks>
        ///     Used as a bit of a checksum for the serialized data.
        /// </remarks>
        bool m_IsKeyValueCountValid = true;

        SerializedProperty m_PropertyCount;
        int m_PropertyCountCache = -1;
        SerializedProperty m_PropertyIsSerializable;
        bool m_PropertyIsSerializableCache;
        SerializedProperty m_PropertyKeys;
        SerializedProperty m_PropertyValues;
        Type m_KeyType = typeof(object);
        Type m_ValueType = typeof(object);
        bool m_KeyTypeNullable;

        /// <summary>
        ///     The index of in the data arrays to be considered selected.
        /// </summary>
        int m_SelectedIndex = -1;

        /// <summary>
        ///     The target object of the <see cref="PropertyDrawer" />.
        /// </summary>
        Object m_TargetObject;
        SerializedObject m_SerializedObjectClone;
        SerializedProperty m_SerializedKeysClone;
        SerializedProperty m_TargetProperty;


        ~SerializableDictionaryPropertyDrawer()
        {
            m_TargetObject = null;
            m_SerializedObjectClone?.Dispose();
            m_TargetProperty.Dispose();
            m_PropertyKeys.Dispose();
            m_PropertyValues.Dispose();
            m_PropertyIsSerializable.Dispose();
            m_PropertyCount.Dispose();
        }

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
            m_HeightFoldout = EditorGUIUtility.singleLineHeight;

            // Early out if the property drawer is not expanded, or we dont have any data
            if (!m_IsExpanded || !m_PropertyIsSerializableCache)
            {
                m_HeightTotal = m_HeightFoldout;
                return m_HeightTotal;
            }

            // Determine the height of the elements portion of the content area
            m_HeightContentHeader = 4;
            m_HeightContentElements = (EditorGUIUtility.singleLineHeight + Styles.ContentAreaElementSpacing) *
                Mathf.Max(
                    1, m_PropertyCountCache) - Styles.ContentAreaElementSpacing;
            m_HeightContentFooter = 4;
            m_HeightContent = m_HeightContentHeader +
                             m_HeightContentElements +
                             m_HeightContentFooter;

            // Figure out our footer total height
            m_HeightFooter = Styles.ActionButtonVerticalPadding +
                            EditorGUIUtility.singleLineHeight +
                            Styles.ActionButtonVerticalPadding;

            // Add up our total
            m_HeightTotal = m_HeightFoldout + Styles.ContentAreaTopMargin + m_HeightContent + m_HeightFooter;

            return m_HeightTotal;
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
            m_DisplayName = property.displayName;
            m_AddKeyFieldID = Content.AddKeyFieldIDPrefix + m_DisplayName.GetStableHashCode();
            m_TargetProperty = property;
            m_IsExpanded = property.isExpanded;

            m_TargetObject = property.serializedObject.targetObject;
            if (fieldInfo.FieldType.BaseType != null)
            {
                m_KeyType = fieldInfo.FieldType.BaseType.GenericTypeArguments[0];
                m_ValueType = fieldInfo.FieldType.BaseType.GenericTypeArguments[1];
                m_KeyTypeNullable = (m_KeyType.IsReferenceType() || Nullable.GetUnderlyingType(m_KeyType) != null);
            }

            // Create default item
            m_AddKey ??= m_KeyType.Default();

            // Build our top level position
            Rect foldoutRect = new Rect(position.x, position.y, position.width, m_HeightFoldout);

            // Editor only properties
            m_PropertyIsSerializable = property.FindPropertyRelative("m_IsSerializable");
            m_PropertyIsSerializableCache = m_PropertyIsSerializable.boolValue;
            if (!m_PropertyIsSerializableCache)
            {
                DrawErrorMessage(foldoutRect, Content.InvalidTypesError);
                return;
            }

            // Grab the properties that matter!
            m_PropertyCount = property.FindPropertyRelative("m_SerializedLength");
            m_PropertyCountCache = m_PropertyCount.intValue;
            m_PropertyKeys = property.FindPropertyRelative("m_SerializedKeys");
            m_PropertyValues = property.FindPropertyRelative("m_SerializedValues");

            // Validate saved sizes
            m_IsKeyValueCountValid = m_PropertyCountCache == m_PropertyValues.arraySize &&
                                    m_PropertyCountCache == m_PropertyKeys.arraySize;

            if (!m_IsKeyValueCountValid)
            {
                DrawErrorMessage(foldoutRect, Content.CorruptDataError, true);
                return;
            }

            // Draw the foldout at the top of the space
            DrawFoldout(foldoutRect);

            // If the foldout is expanded, draw the actual content
            if (m_IsExpanded)
            {
                Rect contentRect = new Rect(position.x, foldoutRect.yMax + Styles.ContentAreaTopMargin, position.width,
                    m_HeightContent);
                DrawContentArea(contentRect);
                Rect footerRect = new Rect(position.x, contentRect.yMax, position.width, m_HeightFooter);
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
        ///     Draw the content area, including elements.
        /// </summary>
        /// <param name="position">A <see cref="Rect" /> representing the space which the content area will be drawn.</param>
        void DrawContentArea(Rect position)
        {
            // Paint the background
            if (Event.current.type == EventType.Repaint)
            {
                Rect headerBackgroundRect = new Rect(position.x, position.y, position.width, m_HeightContentHeader);

                // The extra 2 pixels are used to get rid of the rounded corners on the content box
                Rect contentBackgroundRect = new Rect(position.x, headerBackgroundRect.yMax, position.width,
                    m_HeightContentElements + 2);
                Rect footerBackgroundRect = new Rect(position.x, contentBackgroundRect.yMax - 2, position.width,
                    m_HeightContentFooter);

                Styles.BoxBackground.Draw(contentBackgroundRect, false, false, false, false);

                Styles.HeaderBackground.Draw(headerBackgroundRect, false, false, false, false);
                Styles.FooterBackground.Draw(footerBackgroundRect, false, false, false, false);
            }

            // Bring in the provided rect
            position.yMin += m_HeightContentHeader;
            position.yMax -= m_HeightContentFooter;
            position.xMin += Styles.ContentAreaHorizontalPadding;
            position.xMax -= Styles.ContentAreaHorizontalPadding;

            // If we have nothing simply display the message
            if (m_PropertyCountCache == 0)
            {
                EditorGUI.LabelField(position, Content.EmptyDictionary, EditorStyles.label);
                return;
            }


            float columnWidth = (position.width - 34) / 2f;


            for (int i = 0; i < m_PropertyCountCache; i++)
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
                    m_SelectedIndex = i;

                    // Attempt to force a redraw of the inspector
                    EditorUtility.SetDirty(m_TargetObject);
                }

                if (i == m_SelectedIndex)
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
                EditorGUI.PropertyField(keyPropertyRect, m_PropertyKeys.GetArrayElementAtIndex(i), GUIContent.none);

                // Draw Value Icon
                Rect valueIconRect = new Rect(keyPropertyRect.xMax + 3, position.y + topOffset - 1, 17,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(valueIconRect, Content.IconValue);

                // Draw Value Property
                Rect valuePropertyRect = new Rect(valueIconRect.xMax, position.y + topOffset, columnWidth - 1,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(valuePropertyRect, m_PropertyValues.GetArrayElementAtIndex(i), GUIContent.none);
            }
        }

        /// <summary>
        ///     Draw actionable buttons offset of the footer.
        /// </summary>
        /// <param name="position">
        ///     A <see cref="Rect" /> representing the space which the footer will be drawn, so that he the
        ///     buttons can draw offset of it.
        /// </param>
        void DrawFooterActions(Rect position)
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
            if (m_ValidKey &&
                Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ||
                 Event.current.character == '\n') &&
                GUI.GetNameOfFocusedControl() == m_AddKeyFieldID)
            {
                Event.current.Use();
                GUIUtility.hotControl = 0;
                AddElement();
            }

            // Draw the input before the button so it overlaps it
            GUI.SetNextControlName(m_AddKeyFieldID);

            if (m_KeyTypeNullable)
            {
                // Add Key Object Route - Efficient-ish
                object beforeObject = m_AddKey;
                m_AddKey = EditorGUI.ObjectField(inputRect, (Object)m_AddKey, m_KeyType, true);
                if (beforeObject != m_AddKey)
                {
                    m_ValidKey = IsValidObjectKey(m_AddKey);
                    if (!m_ValidKey)
                    {
                        m_AddKey = m_KeyType.Default();
                    }
                }
            }
            else
            {
                // We're going clone the serialized object so that we can do some fun stuff, but we need to remember
                // never to apply the changes to this particular object.


                // TODO Dont think we need to copy but for now this is how were gonna make sure were not doing bad
                m_SerializedObjectClone ??= new SerializedObject(m_TargetObject);
                //m_TargetProperty.Copy();
                m_SerializedKeysClone ??=

                    m_SerializedObjectClone.FindProperty(m_TargetProperty.propertyPath)
                    .FindPropertyRelative("m_SerializedKeys");

                // Expand our array if necessary
                if (m_SerializedKeysClone.arraySize == 0)
                {
                    m_SerializedKeysClone.arraySize = 1;
                }


                //SerializedProperties.SetValue(m_SerializedKeysClone, "data", m_AddKey, 0);

                SerializedProperty fakeKey = m_SerializedKeysClone.GetArrayElementAtIndex(0);
                EditorGUI.PropertyField(inputRect, fakeKey, GUIContent.none);
                if (m_AddKey != fakeKey.boxedValue)
                {
                    // TODO: Validate?
                    m_AddKey = fakeKey.boxedValue;
                    m_ValidKey = true;
                }
                else
                {
                    m_ValidKey = false;
                }
                fakeKey.Dispose();
            }

            GUI.enabled = m_ValidKey;
            if (GUI.Button(addRect, Content.IconPlus, Styles.FooterButton))
            {
                // Remove control focus
                GUIUtility.hotControl = 0;
                AddElement();
            }

            GUI.enabled = true;

            // Only visible when we have something selected (with a failsafe on item count)
            if (m_PropertyCountCache == 0 || m_SelectedIndex == -1)
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
                RemoveElementAt(m_SelectedIndex);

                // Keep next item selected.
                m_SelectedIndex--;
                if (m_SelectedIndex >= 0)
                {
                    return;
                }

                if (m_PropertyCountCache > 1)
                {
                    m_SelectedIndex = 0;
                }
                else
                {
                    m_SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        ///     Draw the foldout header for the <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="position">A <see cref="Rect" /> representing the space which the header will consume in its entirety.</param>
        void DrawFoldout(Rect position)
        {
            // Generate a foldout GUI element representing the name of the dictionary
            bool newExpanded =
                EditorGUI.Foldout(position, m_IsExpanded, m_DisplayName, true, EditorStyles.foldout);
            if (newExpanded != m_IsExpanded)
            {
                m_IsExpanded = newExpanded;
                m_TargetProperty.isExpanded = newExpanded;

                // If we're collapsing the foldout, make sure there is no key cached
                if (!newExpanded)
                {
                    m_AddKey = null;
                }
            }

            // Indicate Count - but ready only
            GUI.enabled = false;
            const int k_ItemCountSize = 48;
            EditorGUI.TextField(new Rect(position.x + position.width - k_ItemCountSize,
                position.y, k_ItemCountSize, position.height), m_PropertyCountCache.ToString());
            GUI.enabled = true;
        }

        /// <summary>
        ///     Draw a message instead of the normal property drawer.
        /// </summary>
        /// <remarks>Useful for when data is corrupted, or types are incompatible.</remarks>
        /// <param name="position">A <see cref="Rect" /> representing where it should be drawn.</param>
        /// <param name="tooltip">The message to display when mousing over.</param>
        /// <param name="displayResetButton">Should a reset button be displayed?</param>
        void DrawErrorMessage(Rect position, string tooltip, bool displayResetButton = false)
        {
            Rect iconRect = new Rect(position.x - 16, position.y, 16, position.height);
            Content.IconError.tooltip = tooltip;
            EditorGUI.LabelField(iconRect, Content.IconError);
            Rect messageRect = new Rect(position.x, position.y, position.width - 17, position.height);
            EditorGUI.LabelField(messageRect, new GUIContent(m_DisplayName, tooltip), EditorStyles.label);

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
        void AddElement(bool defaultValue = true)
        {
            if (m_PropertyCountCache == -1 || m_PropertyKeys == null || m_PropertyValues == null)
            {
                return;
            }

            m_PropertyKeys.arraySize++;

            // This issue is Array.data doesnt exist, its
            //GameObjectToInteger.m_SerializedKeys.Array.data[0]
            SerializedProperty newKeyProperty = m_PropertyKeys.GetArrayElementAtIndex(m_PropertyCountCache);
            newKeyProperty.boxedValue = m_AddKey;
            //newKeyProperty.SetValue(m_AddKey);

            // Add new value element, and optionally default its value
            m_PropertyValues.arraySize++;
            if (defaultValue)
            {
                SerializedProperty newValueProperty = m_PropertyValues.GetArrayElementAtIndex(m_PropertyCountCache);
                newValueProperty.boxedValue = m_ValueType.Default();
                //newValueProperty.SetValue(m_ValueType.Default());
            }
            // We do this to force serialization
            m_PropertyValues.isExpanded = true;

            // Increase Size
            m_PropertyCountCache++;
            m_PropertyCount.intValue = m_PropertyCountCache;
            m_AddKey = m_KeyType.Default();
        }

        /// <summary>
        ///     Remove an element from the targeted <see cref="SerializableDictionary{TKey,TValue}" /> at the provided index.
        /// </summary>
        /// <remarks>
        ///     This alters the underlying backing serialized data, after this change, the actual
        ///     <see cref="SerializableDictionary{TKey,TValue}" /> picks up the change.
        /// </remarks>
        /// <param name="index">The index (really ordered serialized data) to remove.</param>
        void RemoveElementAt(int index)
        {
            if (index == -1)
            {
                m_SelectedIndex = -1;
            }
            else if (m_PropertyKeys != null && m_PropertyValues != null)
            {
                m_PropertyKeys.DeleteArrayElementAtIndex(index);
                m_PropertyValues.DeleteArrayElementAtIndex(index);

                m_PropertyCountCache--;
                if (m_PropertyCountCache < 0)
                {
                    m_PropertyCountCache = 0;
                }

                m_PropertyCount.intValue = m_PropertyCountCache;
            }
        }

        /// <summary>
        ///     Reset the serialized data as if there were no entries in the dictionary.
        /// </summary>
        void ResetSerializedData()
        {
            m_PropertyKeys.ClearArray();
            m_PropertyValues.ClearArray();
            m_PropertyCount.intValue = 0;
            m_PropertyCountCache = 0;
            m_SelectedIndex = -1;
        }

        /// <summary>
        ///     <see cref="PropertyDrawer" /> Fixed Content.
        /// </summary>
        static class Content
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
                EditorGUIUtility.IconContent("animationanimated", "Value");
        }

        /// <summary>
        ///     <see cref="PropertyDrawer" /> Styles.
        /// </summary>
        static class Styles
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
#endif // !UNITY_DOTSRUNTIME
}