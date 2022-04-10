// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System;
using System.Reflection;

namespace GDX.Editor
{
    public static class SerializedProperties
    {

        public static object GetParent(SerializedProperty prop)
        {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements.Take(elements.Length - 1))
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "")
                        .Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }

        public static object GetValue(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null)
            {
                var p = type.GetProperty(name,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null)
                    return null;
                return p.GetValue(source, null);
            }

            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}