// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.
using System.Collections;
using System;
using System.Reflection;
using UnityEditor;

namespace GDX.Editor
{
    public static class SerializedProperties
    {
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

        public static object GetValue(object source, string name, int index)
        {
            IEnumerable enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return null;
            }

            IEnumerator enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }

        public static void SetValue(object source, string name, object value)
        {
            object abstractObject = GetValue(source, name);

            if (abstractObject == null)
            {
                return;
            }

            FieldInfo field = abstractObject.GetType().GetFieldUnambiguous(name);
            field.SetValue(abstractObject, value);
        }

        public static void SetValue(object source, string name, object value, int index)
        {
            IEnumerable enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null)
            {
                return;
            }

            IEnumerator enm = enumerable.GetEnumerator();
            while (index-- >= 0)
                enm.MoveNext();

            object abstractObject = enm.Current;
            if (abstractObject == null)
            {
                return;
            }

            FieldInfo field = abstractObject.GetType().GetFieldUnambiguous(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field.SetValue(abstractObject, value);
        }
    }
}