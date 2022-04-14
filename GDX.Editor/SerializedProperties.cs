// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.
using System.Collections;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GDX.Editor
{
    public static class SerializedProperties
    {
        public const BindingFlags SerializationFieldFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public const BindingFlags SerializationPropertyFlags = BindingFlags.NonPublic | BindingFlags.Public |
                                                               BindingFlags.Instance | BindingFlags.IgnoreCase;

        public static object GetValue(this SerializedProperty property)
        {
#if UNITY_2022_1_OR_NEWER
            return property.boxedValue;
#else
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Enum:
                case SerializedPropertyType.LayerMask:
                    return property.intValue;
                case SerializedPropertyType.Boolean:
                    return property.boolValue;
                case SerializedPropertyType.Float:
                    return property.floatValue;
                case SerializedPropertyType.String:
                    return property.stringValue;
                case SerializedPropertyType.Color:
                    return property.colorValue;
                case SerializedPropertyType.ObjectReference:
                    return property.objectReferenceValue;
                case SerializedPropertyType.Vector2:
                    return property.vector2Value;
                case SerializedPropertyType.Vector3:
                    return property.vector3Value;
                case SerializedPropertyType.Vector4:
                    return property.vector4Value;
                case SerializedPropertyType.Rect:
                    return property.rectValue;
                case SerializedPropertyType.AnimationCurve:
                    return property.animationCurveValue;
                case SerializedPropertyType.Bounds:
                    return property.boundsValue;
                case SerializedPropertyType.Quaternion:
                    return property.quaternionValue;
                case SerializedPropertyType.Vector2Int:
                    return property.vector2IntValue;
                case SerializedPropertyType.Vector3Int:
                    return property.vector3IntValue;
                case SerializedPropertyType.RectInt:
                    return property.rectIntValue;
                case SerializedPropertyType.BoundsInt:
                    return property.boundsIntValue;
#if UNITY_2021_1_OR_NEWER
                case SerializedPropertyType.ManagedReference:
                    return property.managedReferenceValue;
                case SerializedPropertyType.Hash128:
                    return property.hash128Value;
#endif
                default:
                    return null;
            }
#endif
        }
        public static void SetValue(this SerializedProperty property, object value)
        {
#if UNITY_2022_1_OR_NEWER
            property.boxedValue = value;
#else
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Enum:
                    property.intValue = (int)value;
                    break;
                case SerializedPropertyType.Boolean:
                    property.boolValue = (bool)value;
                    break;
                case SerializedPropertyType.Float:
                    property.floatValue = (float)value;
                    break;
                case SerializedPropertyType.String:
                    property.stringValue = (string)value;
                    break;
                case SerializedPropertyType.Color:
                    property.colorValue = (Color)value;
                    break;
                case SerializedPropertyType.ObjectReference:
                    UnityEngine.Object unityObject = (UnityEngine.Object)value;
                    property.objectReferenceValue = unityObject;
                    property.objectReferenceInstanceIDValue = unityObject.GetInstanceID();
                    break;
                case SerializedPropertyType.LayerMask:
                    property.intValue = ((LayerMask)value).value;
                    break;
                case SerializedPropertyType.Vector2:
                    property.vector2Value = (Vector2)value;
                    break;
                case SerializedPropertyType.Vector3:
                    property.vector3Value = (Vector3)value;
                    break;
                case SerializedPropertyType.Vector4:
                    property.vector4Value = (Vector4)value;
                    break;
                case SerializedPropertyType.Rect:
                    property.rectValue = (Rect)value;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    property.animationCurveValue = (AnimationCurve)value;
                    break;
                case SerializedPropertyType.Bounds:
                    property.boundsValue = (Bounds)value;
                    break;
                case SerializedPropertyType.Quaternion:
                    property.quaternionValue = (Quaternion)value;
                    break;
                case SerializedPropertyType.Vector2Int:
                    property.vector2IntValue = (Vector2Int)value;
                    break;
                case SerializedPropertyType.Vector3Int:
                    property.vector3IntValue = (Vector3Int)value;
                    break;
                case SerializedPropertyType.RectInt:
                    property.rectIntValue = (RectInt)value;
                    break;
                case SerializedPropertyType.BoundsInt:
                    property.boundsIntValue = (BoundsInt)value;
                    break;
#if UNITY_2021_1_OR_NEWER
                case SerializedPropertyType.ManagedReference:
                    property.managedReferenceValue = value;
                    break;
                case SerializedPropertyType.Hash128:
                    property.hash128Value = (Hash128)value;
                    break;
#endif
            }
#endif
        }
    }
}