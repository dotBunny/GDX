// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using GDX.Collections;
using UnityEditor;
using UnityEngine;
using GDX.Collections.Generic;
using PlasticPipe.Server;
using Unity.Collections;
using Object = UnityEngine.Object;
using Unity.UI;
using Unity.UI.Builder;
using Unity.UIElements;
using Unity.UIElements.Editor;
using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace GDX.Editor.PropertyDrawers
{
    public static class StringKeyPropertyDrawerDB<T>
    {
        public struct PropertyValue
        {
            public StringKeyDictionary<EntryValue<T>> StringKeyDictionary;
            public int Selection;
            public bool SeenThisFrame;
        }

        public static object DefaultValue = typeof(T).GetDefault();

        public static SimpleList<StringKeyDictionary<EntryValue<T>>> stringKeyDictionaryPool = new SimpleList<StringKeyDictionary<EntryValue<T>>>(16);

        public static StringHashedStructKeyDictionary<StringKeyDictionaryPropertyDrawerDB.PropertyKey, PropertyValue> serializedObjectToPropertyMap = new StringHashedStructKeyDictionary<StringKeyDictionaryPropertyDrawerDB.PropertyKey, PropertyValue>(16);
    }




    public struct EntryValue<T>
    {
        public string UserEnteredKey;
        public object UserEnteredValue;
        public bool HasValueToSet;
    }
    public static class StringKeyDictionaryPropertyDrawerDB
    {
        public struct PropertyKey : IEquatable<PropertyKey>, IStringHashedStructKey
        {
            public string PropertyPath;
            public SerializedObject SerializedObject;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Equals(PropertyKey other)
            {
                return PropertyPath == other.PropertyPath && SerializedObject == other.SerializedObject;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override int GetHashCode()
            {
                return PropertyPath.GetStableHashCode();
            }


            public string stringToHash { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return PropertyPath; } }
        }

        // public static GDX.Collections.Pooling.ArrayPool<string> arrayPool = new Collections.Pooling.ArrayPool<string>
        // (
        //     new int[31],
        //     new int[]{65536, 65536, 65536, 65536, 65536, 65536, 65536, 65536, 65536, 65536, 65536, 65536,
        //                             0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        // );

        public static object[] parameterScratchpadArray = new object[1];
        public static MethodInfo baseMethodInfo = typeof(StringKeyDictionaryPropertyDrawer).GetMethod(nameof(ProcessDictionary));
        public static Dictionary<Type, MethodInfo> genericRegistry = new Dictionary<Type, MethodInfo>(128);
        public static SimpleList<int> modifiedEntries = new SimpleList<int>(128);
        public static SimpleList<int> modifiedBucketEntries = new SimpleList<int>(128);

        public static VisualElement ProcessDictionary<T>(SerializedProperty property, Rect position)
        {
            Object targetObject = property.serializedObject.targetObject;
            SerializedProperty arrayProp = property.FindPropertyRelative(nameof(StringKeyDictionary<T>.Entries));
            int arraySize = arrayProp.arraySize;

            // First: determine if the dictionary is new, and extract all values if it is. Sort them for visualization.

            PropertyKey key = new PropertyKey() { PropertyPath = property.propertyPath, SerializedObject = property.serializedObject };

            object defaultValue = StringKeyPropertyDrawerDB<T>.DefaultValue;

            StringKeyPropertyDrawerDB<T>.PropertyValue propertyValue = default;
            if (!StringKeyPropertyDrawerDB<T>.serializedObjectToPropertyMap.TryGetValue(key, ref propertyValue))
            {
                propertyValue.StringKeyDictionary = new StringKeyDictionary<EntryValue<T>>(arraySize);
                StringKeyDictionary<T> stringKeyDictionary = (StringKeyDictionary<T>)property.boxedValue;

                propertyValue.StringKeyDictionary.Count = stringKeyDictionary.Count;
                propertyValue.StringKeyDictionary.FreeListHead = stringKeyDictionary.FreeListHead;
                propertyValue.Selection = -1;

                for (int i = 0; i < arraySize; i++)
                {
                    ref StringKeyEntry<T> serializedEntry = ref stringKeyDictionary.Entries[i];
                    ref StringKeyEntry<EntryValue<T>> cachedEntry = ref propertyValue.StringKeyDictionary.Entries[i];

                    cachedEntry.Key = serializedEntry.Key;
                    cachedEntry.Value.UserEnteredKey = serializedEntry.Key;
                    cachedEntry.Next = serializedEntry.Next;
                    cachedEntry.HashCode = serializedEntry.HashCode;

                    propertyValue.StringKeyDictionary.Buckets[i] = stringKeyDictionary.Buckets[i];
                }

                StringKeyPropertyDrawerDB<T>.serializedObjectToPropertyMap.AddWithExpandCheck(key, propertyValue);
            }

            // Second: Process user input, and update visual representation for added, removed, modified,
            //         and invalid modified entries. Extract deltas from modified entries. Mark property as "seen".
            //         This section is going to get bigger with entry reversion etc.

            SimpleList<int> modified = modifiedEntries;
            SimpleList<int> modifiedBucket = modifiedBucketEntries;

            modified.Reserve(propertyValue.StringKeyDictionary.Entries.Length * 2);
            modifiedBucket.Reserve(propertyValue.StringKeyDictionary.Entries.Length * 2);

            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float footerHeight = SerializableDictionaryPropertyDrawer.Styles.ActionButtonVerticalPadding +
                                 singleLineHeight +
                                 SerializableDictionaryPropertyDrawer.Styles.ActionButtonVerticalPadding;
            // Build our top level position
            Rect foldoutRect = new Rect(position.x, position.y, position.width, singleLineHeight);

            int currEntryCount = property.FindPropertyRelative(nameof(StringKeyDictionary<T>.Count)).intValue;
            // Draw the foldout at the top of the space
            bool clearAddKey = DrawFoldout(foldoutRect, arrayProp, currEntryCount);

            float heightContentElements = (EditorGUIUtility.singleLineHeight + SerializableDictionaryPropertyDrawer.Styles.ContentAreaElementSpacing) *
                Mathf.Max(
                    1, currEntryCount) - SerializableDictionaryPropertyDrawer.Styles.ContentAreaElementSpacing;
            // If the foldout is expanded, draw the actual content
            if (arrayProp.isExpanded)
            {
                Rect contentRect = new Rect(position.x, foldoutRect.yMax + SerializableDictionaryPropertyDrawer.Styles.ContentAreaTopMargin, position.width,
                    singleLineHeight);

                // Paint the background
                if (Event.current.type == EventType.Repaint)
                {
                    Rect headerBackgroundRect = new Rect(contentRect.x, contentRect.y, contentRect.width, 4);

                    // The extra 2 pixels are used to get rid of the rounded corners on the content box
                    Rect contentBackgroundRect = new Rect(contentRect.x, headerBackgroundRect.yMax, contentRect.width,
                        heightContentElements + 2);
                    Rect footerBackgroundRect = new Rect(contentRect.x, contentBackgroundRect.yMax - 2, contentRect.width,
                        4);

                    SerializableDictionaryPropertyDrawer.Styles.BoxBackground.Draw(contentBackgroundRect, false, false, false, false);

                    SerializableDictionaryPropertyDrawer.Styles.HeaderBackground.Draw(headerBackgroundRect, false, false, false, false);
                    SerializableDictionaryPropertyDrawer.Styles.FooterBackground.Draw(footerBackgroundRect, false, false, false, false);
                }

                // Bring in the provided rect
                contentRect.yMin += 4;
                contentRect.yMax -= 4;
                contentRect.xMin += SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding;
                contentRect.xMax -= SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding;

                // If we have nothing simply display the message
                if (currEntryCount == 0)
                {
                    EditorGUI.LabelField(contentRect, SerializableDictionaryPropertyDrawer.Content.EmptyDictionary, EditorStyles.label);
                    return default;
                }

                float columnWidth = (contentRect.width - 34) / 2f;

                int iterator = 0;
                while (propertyValue.StringKeyDictionary.MoveNext(ref iterator))
                {
                    int iteratorIndex = iterator - 1;
                    float topOffset = (EditorGUIUtility.singleLineHeight + SerializableDictionaryPropertyDrawer.Styles.ContentAreaElementSpacing) * iteratorIndex;

                    Rect selectionRect = new Rect(
                        contentRect.x - SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding + 1,
                        contentRect.y + topOffset - 2,
                        contentRect.width + SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding * 2 - 3,
                        EditorGUIUtility.singleLineHeight + 4);

                    // Handle selection (left-click), do not consume/use the event so that fields receive.
                    if (Event.current.type == EventType.MouseDown &&
                        Event.current.button == 0 &&
                        selectionRect.Contains(Event.current.mousePosition))
                    {
                        propertyValue.Selection = iteratorIndex;

                        // Attempt to force a redraw of the inspector
                        EditorUtility.SetDirty(targetObject);
                    }

                    if (iteratorIndex == propertyValue.Selection)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            SerializableDictionaryPropertyDrawer.Styles.ElementBackground.Draw(selectionRect, false, true, true, true);
                        }
                    }

                    // Draw Key Icon
#if UNITY_2021_1_OR_NEWER
                    Rect keyIconRect =
                        new Rect(contentRect.x - 2, contentRect.y + topOffset - 1, 17, EditorGUIUtility.singleLineHeight);
#else
                    Rect keyIconRect = new Rect(position.x, position.y + topOffset, 17,
                    EditorGUIUtility.singleLineHeight);
#endif
                    EditorGUI.LabelField(keyIconRect, SerializableDictionaryPropertyDrawer.Content.IconKey);

                    var entry = propertyValue.StringKeyDictionary.Entries[iterator - 1];
                    string entryKey = entry.Key;
                    EntryValue<T> entryValue = entry.Value;

                    // Draw Key Property
                    Rect keyPropertyRect = new Rect(keyIconRect.xMax, contentRect.y + topOffset, columnWidth,
                        EditorGUIUtility.singleLineHeight);

                    string modifiedKeyString = EditorGUI.TextField(keyPropertyRect, entry.Value.UserEnteredKey);
                    SerializedProperty arrayPropAtIndex = arrayProp.GetArrayElementAtIndex(iterator - 1);
                    SerializedProperty valueAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Value));

                    // Draw Value Icon
                    Rect valueIconRect = new Rect(keyPropertyRect.xMax + 3, contentRect.y + topOffset - 1, 17,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(valueIconRect, SerializableDictionaryPropertyDrawer.Content.IconValue);

                    // Draw Value Property
                    Rect valuePropertyRect = new Rect(valueIconRect.xMax, contentRect.y + topOffset, columnWidth - 1,
                        EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(valuePropertyRect, valueAtIndex, GUIContent.none);

                    Event currentEvent = Event.current;

                    var newValue = entryValue;
                    newValue.UserEnteredKey = modifiedKeyString;

                    if (modifiedKeyString != entryValue.UserEnteredKey &&
                        currentEvent.type == EventType.KeyDown &&
                        (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter ||
                        currentEvent.character == '\n'))
                    {
                        // Reinsert if valid

                        if (!propertyValue.StringKeyDictionary.ContainsKey(modifiedKeyString))
                        {
                            modified.AddUnchecked(iterator - 1);

                            int modifiedIndex = propertyValue.StringKeyDictionary.ModifyKeyUnchecked(iterator - 1, modifiedKeyString, out bool wasFirstEntryInChain);
                            if (wasFirstEntryInChain)
                            {
                               modifiedBucket.AddUnchecked(modifiedIndex);
                            }
                            else
                            {
                                modified.AddUnchecked(modifiedIndex);
                            }

                            int newBucketIndex = propertyValue.StringKeyDictionary.BucketOf(entryKey);
                            modifiedBucket.AddUnchecked(newBucketIndex);
                        }
                    }

                    propertyValue.StringKeyDictionary.Entries[iterator - 1].Value = newValue;
                }

                propertyValue.SeenThisFrame = true;
                StringKeyPropertyDrawerDB<T>.serializedObjectToPropertyMap[key] = propertyValue;

                // Event footerEvent = Event.current;
                // if (modifiedKeyString != entryValue.UserEnteredKey &&
                //     footerEvent.type == EventType.KeyDown &&
                //     (footerEvent.keyCode == KeyCode.Return || footerEvent.keyCode == KeyCode.KeypadEnter ||
                //      footerEvent.character == '\n'))
                // {
                //
                // }

                // Finally: Take deltas and update their serialized entries.

                int modifiedCount = modified.Count;
                for (int i = 0; i < modifiedCount; i++)
                {
                    int modifiedIndex = modified.Array[i];
                    var entry = propertyValue.StringKeyDictionary.Entries[modifiedIndex];
                    SerializedProperty arrayPropAtIndex = arrayProp.GetArrayElementAtIndex(modifiedIndex);
                    SerializedProperty keyAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Key));
                    SerializedProperty nextAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Next));
                    SerializedProperty hashAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.HashCode));
                    SerializedProperty valueAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Value));
                    keyAtIndex.stringValue = entry.Key;
                    nextAtIndex.intValue = entry.Next;
                    hashAtIndex.intValue = entry.HashCode;

                    if (entry.Value.HasValueToSet)
                    {
                        valueAtIndex.boxedValue = entry.Value.UserEnteredValue;
                        propertyValue.StringKeyDictionary.Entries[modifiedIndex].Value.HasValueToSet = false;
                        propertyValue.StringKeyDictionary.Entries[modifiedIndex].Value.UserEnteredValue = defaultValue;
                    }
                }

                SerializedProperty bucketsProp = property.FindPropertyRelative(nameof(StringKeyDictionary<T>.Buckets));
                int modifiedBucketCount = modifiedBucket.Count;
                for (int i = 0; i < modifiedBucketCount; i++)
                {
                    int modifiedIndex = modifiedBucket.Array[i];
                    int bucketEntry = propertyValue.StringKeyDictionary.Buckets[modifiedIndex];
                    SerializedProperty arrayPropAtIndex = bucketsProp.GetArrayElementAtIndex(modifiedIndex);
                    arrayPropAtIndex.intValue = bucketEntry;
                }

                modified.Count = 0;
                modifiedBucket.Count = 0;
                modifiedEntries = modified;
                modifiedBucketEntries = modifiedBucket;

                int finalEntriesArraySize = propertyValue.StringKeyDictionary.Entries.Length;
                if (arraySize < finalEntriesArraySize)
                {
                    arrayProp.arraySize = finalEntriesArraySize;
                    bucketsProp.arraySize = finalEntriesArraySize;

                    for (int i = 0; i < finalEntriesArraySize; i++)
                    {
                        bucketsProp.GetArrayElementAtIndex(i).intValue = propertyValue.StringKeyDictionary.Buckets[i];
                    }

                    for (int i = arraySize; i < finalEntriesArraySize; i++)
                    {
                        var entry = propertyValue.StringKeyDictionary.Entries[i];
                        SerializedProperty arrayPropAtIndex = arrayProp.GetArrayElementAtIndex(i);
                        SerializedProperty keyAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Key));
                        SerializedProperty nextAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Next));
                        SerializedProperty hashAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.HashCode));
                        SerializedProperty valueAtIndex = arrayPropAtIndex.FindPropertyRelative(nameof(StringKeyEntry<T>.Value));
                        keyAtIndex.stringValue = entry.Key;
                        nextAtIndex.intValue = entry.Next;
                        hashAtIndex.intValue = entry.HashCode;

                        if (entry.Value.HasValueToSet)
                        {
                            valueAtIndex.boxedValue = entry.Value.UserEnteredValue;
                            propertyValue.StringKeyDictionary.Entries[i].Value.HasValueToSet = false;
                            propertyValue.StringKeyDictionary.Entries[i].Value.UserEnteredValue = defaultValue;
                        }
                    }
                }

                property.FindPropertyRelative(nameof(StringKeyDictionary<T>.Count)).intValue = propertyValue.StringKeyDictionary.Count;
                property.FindPropertyRelative(nameof(StringKeyDictionary<T>.FreeListHead)).intValue = propertyValue.StringKeyDictionary.FreeListHead;

                //DrawContentArea(contentRect);
                Rect footerRect = new Rect(position.x, contentRect.yMax, position.width, footerHeight);
                DrawFooterActions(footerRect);
            }

            // Create undo point if we've changed something
            if (GUI.changed)
            {
                Undo.SetCurrentGroupName("Serializable Dictionary Action");
            }

            // Anything we changed property wise we should save
            property.serializedObject.ApplyModifiedProperties();



            // End of frame: Remove all dictionaries that were not seen this frame.

            int count = 0;

            for (int i = 0; i < count; i++)
            {


            }

            VisualElement returnElement = new VisualElement();

            return returnElement;
        }

        static bool DrawFoldout(Rect position, SerializedProperty property, int entryCount)
        {
            bool clearAddKey = false;
            // Generate a foldout GUI element representing the name of the dictionary
            bool newExpanded =
                EditorGUI.Foldout(position, property.isExpanded, "String Key Dictionary", true, EditorStyles.foldout);
            if (newExpanded != property.isExpanded)
            {
                property.isExpanded = newExpanded;

                // If we're collapsing the foldout, make sure there is no key cached
                if (!newExpanded)
                {
                    clearAddKey = true;
                }
            }

            // Indicate Count - but ready only
            GUI.enabled = false;
            const int k_ItemCountSize = 48;
            EditorGUI.TextField(new Rect(position.x + position.width - k_ItemCountSize,
                position.y, k_ItemCountSize, position.height), entryCount.ToString());
            GUI.enabled = true;

            return clearAddKey;
        }

        // <summary>
        ///     Draw the content area, including elements.
        /// </summary>
        /// <param name="contentRect">A <see cref="Rect" /> representing the space which the content area will be drawn.</param>
        static void DrawContentArea(Rect contentRect, float heightContentHeader, float heightContentElements, float heightContentFooter, ref int selectionIndex)
        {
            // Paint the background
            if (Event.current.type == EventType.Repaint)
            {
                Rect headerBackgroundRect = new Rect(contentRect.x, contentRect.y, contentRect.width, heightContentHeader);

                // The extra 2 pixels are used to get rid of the rounded corners on the content box
                Rect contentBackgroundRect = new Rect(contentRect.x, headerBackgroundRect.yMax, contentRect.width,
                    heightContentElements + 2);
                Rect footerBackgroundRect = new Rect(contentRect.x, contentBackgroundRect.yMax - 2, contentRect.width,
                    heightContentFooter);

                SerializableDictionaryPropertyDrawer.Styles.BoxBackground.Draw(contentBackgroundRect, false, false, false, false);

                SerializableDictionaryPropertyDrawer.Styles.HeaderBackground.Draw(headerBackgroundRect, false, false, false, false);
                SerializableDictionaryPropertyDrawer.Styles.FooterBackground.Draw(footerBackgroundRect, false, false, false, false);
            }

            // Bring in the provided rect
            contentRect.yMin += heightContentHeader;
            contentRect.yMax -= heightContentFooter;
            contentRect.xMin += SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding;
            contentRect.xMax -= SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding;

            // If we have nothing simply display the message
            if (m_PropertyCountCache == 0)
            {
                EditorGUI.LabelField(contentRect, SerializableDictionaryPropertyDrawer.Content.EmptyDictionary, EditorStyles.label);
                return;
            }


            float columnWidth = (contentRect.width - 34) / 2f;


            for (int iteratorIndex = 0; iteratorIndex < m_PropertyCountCache; iteratorIndex++)
            {
                float topOffset = (EditorGUIUtility.singleLineHeight + SerializableDictionaryPropertyDrawer.Styles.ContentAreaElementSpacing) * iteratorIndex;

                Rect selectionRect = new Rect(
                    contentRect.x - SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding + 1,
                    contentRect.y + topOffset - 2,
                    contentRect.width + SerializableDictionaryPropertyDrawer.Styles.ContentAreaHorizontalPadding * 2 - 3,
                    EditorGUIUtility.singleLineHeight + 4);

                // Handle selection (left-click), do not consume/use the event so that fields receive.
                if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0 &&
                    selectionRect.Contains(Event.current.mousePosition))
                {
                    selectionIndex = iteratorIndex;

                    // Attempt to force a redraw of the inspector
                    EditorUtility.SetDirty(m_TargetObject);
                }

                if (iteratorIndex == selectionIndex)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        SerializableDictionaryPropertyDrawer.Styles.ElementBackground.Draw(selectionRect, false, true, true, true);
                    }
                }

                // Draw Key Icon
#if UNITY_2021_1_OR_NEWER
                Rect keyIconRect =
                    new Rect(contentRect.x - 2, contentRect.y + topOffset - 1, 17, EditorGUIUtility.singleLineHeight);
#else
                Rect keyIconRect = new Rect(position.x, position.y + topOffset, 17,
                    EditorGUIUtility.singleLineHeight);
#endif
                EditorGUI.LabelField(keyIconRect, SerializableDictionaryPropertyDrawer.Content.IconKey);

                // Draw Key Property
                Rect keyPropertyRect = new Rect(keyIconRect.xMax, contentRect.y + topOffset, columnWidth,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(keyPropertyRect, m_PropertyKeys.GetArrayElementAtIndex(iteratorIndex), GUIContent.none);

                // Draw Value Icon
                Rect valueIconRect = new Rect(keyPropertyRect.xMax + 3, contentRect.y + topOffset - 1, 17,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(valueIconRect, SerializableDictionaryPropertyDrawer.Content.IconValue);

                // Draw Value Property
                Rect valuePropertyRect = new Rect(valueIconRect.xMax, contentRect.y + topOffset, columnWidth - 1,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(valuePropertyRect, m_PropertyValues.GetArrayElementAtIndex(iteratorIndex), GUIContent.none);
            }
        }
    }

    [CustomPropertyDrawer(typeof(StringKeyDictionary<>))]
    public class StringKeyDictionaryPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Type propertyType = fieldInfo.FieldType;

            MethodInfo genericMethod;
            var genericRegistry = StringKeyDictionaryPropertyDrawerDB.genericRegistry;
            var baseMethodInfo = StringKeyDictionaryPropertyDrawerDB.baseMethodInfo;
            var parameterScratchpadArray = StringKeyDictionaryPropertyDrawerDB.parameterScratchpadArray;

            if (!genericRegistry.TryGetValue(propertyType, out genericMethod))
            {
                genericMethod = baseMethodInfo.MakeGenericMethod(propertyType);
                genericRegistry.Add(propertyType, genericMethod);
            }

            parameterScratchpadArray[0] = property;
            object returnVal = genericMethod.Invoke(null, parameterScratchpadArray);
            parameterScratchpadArray[0] = null;

            return (VisualElement)returnVal;
        }
    }

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
        /// <summary>
        ///     A reference to an <see cref="object"/> which represents the value to be added as a key.
        /// </summary>
        /// <remarks>
        ///     This can be a proper object, or a boxed value.
        /// </remarks>
        object m_AddKey;

        /// <summary>
        ///     Cached ID of the <see cref="EditorGUI.PropertyField(UnityEngine.Rect,UnityEditor.SerializedProperty)" />, used to
        ///     determine if it has focus across updates.
        /// </summary>
        string m_AddKeyFieldIdentifier;

        /// <summary>
        ///     A cached value of the base properties isExpanded flag.
        /// </summary>
        bool m_CachedExpandedStatus;

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
        ///     Is the <see cref="m_AddKey"/> a valid key to add to the target <see cref="SerializableDictionary{TKey,TValue}"/>
        /// </summary>
        bool m_IsAddKeyValid;

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

        /// <summary>
        ///     Clean up our allocations.
        /// </summary>
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
            if (!m_CachedExpandedStatus || !m_PropertyIsSerializableCache)
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
            m_AddKeyFieldIdentifier = Content.AddKeyFieldIdentifierPrefix + m_DisplayName.GetStableHashCode();
            m_TargetProperty = property;
            m_CachedExpandedStatus = property.isExpanded;

            m_TargetObject = property.serializedObject.targetObject;
            if (fieldInfo.FieldType.BaseType != null)
            {
                m_KeyType = fieldInfo.FieldType.BaseType.GenericTypeArguments[0];
                m_ValueType = fieldInfo.FieldType.BaseType.GenericTypeArguments[1];
                m_KeyTypeNullable = !m_KeyType.IsValueType || Nullable.GetUnderlyingType(m_KeyType) != null;
            }

            // Create default item
            m_AddKey ??= m_KeyType.GetDefault();

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
            m_IsKeyValueCountValid = m_PropertyValues != null &&
                                     m_PropertyCountCache == m_PropertyValues.arraySize &&
                                     m_PropertyKeys != null &&
                                     m_PropertyCountCache == m_PropertyKeys.arraySize;

            if (!m_IsKeyValueCountValid)
            {
                DrawErrorMessage(foldoutRect, Content.CorruptDataError, true);
                return;
            }

            // Draw the foldout at the top of the space
            DrawFoldout(foldoutRect);

            // If the foldout is expanded, draw the actual content
            if (m_CachedExpandedStatus)
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

            // This issue is Array.data doesnt exist, its GameObjectToInteger.m_SerializedKeys.Array.data[0]
            SerializedProperty newKeyProperty = m_PropertyKeys.GetArrayElementAtIndex(m_PropertyCountCache);
            newKeyProperty.SetValue(m_AddKey);

            // Add new value element, and optionally default its value
            m_PropertyValues.arraySize++;
            if (defaultValue)
            {
                SerializedProperty newValueProperty = m_PropertyValues.GetArrayElementAtIndex(m_PropertyCountCache);
                newValueProperty.SetValue(m_ValueType.GetDefault());
            }
            // We do this to force serialization
            m_PropertyValues.isExpanded = true;

            // Increase Size
            m_PropertyCountCache++;
            m_PropertyCount.intValue = m_PropertyCountCache;
            m_AddKey = m_KeyType.GetDefault();
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
            if (m_IsAddKeyValid &&
                Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter ||
                 Event.current.character == '\n') &&
                GUI.GetNameOfFocusedControl() == m_AddKeyFieldIdentifier)
            {
                Event.current.Use();
                GUIUtility.hotControl = 0;
                AddElement();
                m_IsAddKeyValid = IsValidKey(m_AddKey);
            }

            // Draw the input before the button so it overlaps it
            GUI.SetNextControlName(m_AddKeyFieldIdentifier);

            if (m_KeyTypeNullable && m_KeyType != typeof(string) && m_KeyTypeNullable)
            {
                // Add Key Object Route - Efficient-ish
                object beforeObject = m_AddKey;
                m_AddKey = EditorGUI.ObjectField(inputRect, (Object)m_AddKey, m_KeyType, true);
                if (beforeObject != m_AddKey)
                {
                    m_IsAddKeyValid = IsValidKey(m_AddKey);
                    if (!m_IsAddKeyValid)
                    {
                        m_AddKey = m_KeyType.GetDefault();
                    }
                }
            }
            else
            {
                m_SerializedObjectClone ??= new SerializedObject(m_TargetObject);
                m_SerializedKeysClone ??=
                    m_SerializedObjectClone.FindProperty(m_TargetProperty.propertyPath)
                    .FindPropertyRelative("m_SerializedKeys");

                // Expand our array if necessary
                if (m_SerializedKeysClone.arraySize == 0)
                {
                    m_SerializedKeysClone.arraySize = 1;
                }
                SerializedProperty fakeKey = m_SerializedKeysClone.GetArrayElementAtIndex(0);
                EditorGUI.PropertyField(inputRect, fakeKey, GUIContent.none);
                object newObject = fakeKey.GetValue();
                if (m_AddKey != newObject)
                {
                    m_AddKey = newObject;
                    m_IsAddKeyValid = IsValidKey(m_AddKey);
                    if (!m_IsAddKeyValid)
                    {
                        m_AddKey = m_KeyType.GetDefault();
                    }
                }
                else
                {
                    m_IsAddKeyValid = false;
                }
                fakeKey.Dispose();
            }

            GUI.enabled = m_IsAddKeyValid;
            if (GUI.Button(addRect, Content.IconPlus, Styles.FooterButton))
            {
                // Remove control focus
                GUIUtility.hotControl = 0;
                AddElement();
                m_IsAddKeyValid = IsValidKey(m_AddKey);
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
                m_IsAddKeyValid = IsValidKey(m_AddKey);

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
                EditorGUI.Foldout(position, m_CachedExpandedStatus, m_DisplayName, true, EditorStyles.foldout);
            if (newExpanded != m_CachedExpandedStatus)
            {
                m_CachedExpandedStatus = newExpanded;
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
        ///
        /// </summary>
        /// <remarks>
        ///     At a certain point (we noticed in 2020.3.33f1), Unity switched to caring about the names of fields
        ///     that were being serialized. So our old method of switching out field data at build time and padding the
        ///     required space was invalidated.
        /// </remarks>
        /// <param name="targetObject"></param>
        /// <returns></returns>
        bool IsValidKey(object targetObject)
        {
            if (targetObject == null) return false;

            bool isFound = Reflection.TryGetFieldOrPropertyValue(m_TargetObject, m_TargetProperty.name,
                out object instanceObject, SerializedProperties.SerializationFieldFlags,
                SerializedProperties.SerializationPropertyFlags);
            if (!isFound) return false;

            IDictionary instanceDictionary = (IDictionary)instanceObject;
            if (instanceDictionary == null ) return false;
            return !instanceDictionary.Contains(targetObject);
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
            m_PropertyKeys?.ClearArray();
            m_PropertyValues?.ClearArray();
            m_PropertyCount.intValue = 0;
            m_PropertyCountCache = 0;
            m_SelectedIndex = -1;
        }

        /// <summary>
        ///     <see cref="PropertyDrawer" /> Fixed Content.
        /// </summary>
        internal static class Content
        {
            /// <summary>
            ///     The prefix of the <see cref="string" /> identifier of the add field.
            /// </summary>
            public const string AddKeyFieldIdentifierPrefix = "SD_KeyToAdd_";

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
        internal static class Styles
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
#endif
}