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

        // Works, pass in the object, and the field name
        // wont do arrays?

        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
            {
                return f.GetValue(source);
            }

            PropertyInfo p = type.GetProperty(name,
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            return p == null ? null : p.GetValue(source, null);
        }

        public static void SetValue(this SerializedProperty property, object value)
        {
            property.boxedValue = value;

            // switch (property.propertyType)
            // {
            //     case SerializedPropertyType.Generic:
            //         break;
            //     case SerializedPropertyType.Integer:
            //         property.intValue = (int)value;
            //         break;
            //     case SerializedPropertyType.Boolean:
            //         property.boolValue = (bool)value;
            //         break;
            //     case SerializedPropertyType.Float:
            //         property.floatValue = (float)value;
            //         break;
            //     case SerializedPropertyType.String:
            //         property.stringValue = (string)value;
            //         break;
            //     case SerializedPropertyType.Color:
            //         property.colorValue = (Color)value;
            //         break;
            //     case SerializedPropertyType.ObjectReference:
            //         UnityEngine.Object unityObject = (UnityEngine.Object)value;
            //         property.objectReferenceValue = unityObject;
            //         property.objectReferenceInstanceIDValue = unityObject.GetInstanceID();
            //         break;
            //     case SerializedPropertyType.LayerMask:
            //         break;
            //     case SerializedPropertyType.Enum:
            //         break;
            //     case SerializedPropertyType.Vector2:
            //         property.vector2Value = (Vector2)value;
            //         break;
            //     case SerializedPropertyType.Vector3:
            //         property.vector3Value = (Vector3)value;
            //         break;
            //     case SerializedPropertyType.Vector4:
            //         property.vector4Value = (Vector4)value;
            //         break;
            //     case SerializedPropertyType.Rect:
            //         property.rectValue = (Rect)value;
            //         break;
            //     case SerializedPropertyType.ArraySize:
            //         break;
            //     case SerializedPropertyType.Character:
            //         break;
            //     case SerializedPropertyType.AnimationCurve:
            //         property.animationCurveValue = (AnimationCurve)value;
            //         break;
            //     case SerializedPropertyType.Bounds:
            //         property.boundsValue = (Bounds)value;
            //         break;
            //     case SerializedPropertyType.Gradient:
            //         break;
            //     case SerializedPropertyType.Quaternion:
            //         property.quaternionValue = (Quaternion)value;
            //         break;
            //     case SerializedPropertyType.ExposedReference:
            //         break;
            //     case SerializedPropertyType.FixedBufferSize:
            //         break;
            //     case SerializedPropertyType.Vector2Int:
            //         property.vector2IntValue = (Vector2Int)value;
            //         break;
            //     case SerializedPropertyType.Vector3Int:
            //         property.vector3IntValue = (Vector3Int)value;
            //         break;
            //     case SerializedPropertyType.RectInt:
            //         property.rectIntValue = (RectInt)value;
            //         break;
            //     case SerializedPropertyType.BoundsInt:
            //         property.boundsIntValue = (BoundsInt)value;
            //         break;
            //     case SerializedPropertyType.ManagedReference:
            //
            //         break;
            //     case SerializedPropertyType.Hash128:
            //         property.hash128Value = (Hash128)value;
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }

    }
}