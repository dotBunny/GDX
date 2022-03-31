// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using UnityEngine;

namespace GDX
{
    // TODO: Maybe
    public static class Reflection
    {
        public static T GetFieldValue<T>(object obj, string name) {
            FieldInfo field = obj.GetType().GetField(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            // if (field == null)
            // {
            //     Debug.Log("FIELD NOT FOUND");
            // }
            return (T)field?.GetValue(obj);
        }
    }
}