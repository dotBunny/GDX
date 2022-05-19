// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;

namespace GDX
{
    /// <summary>
    ///     A collection of reflection related helper utilities.
    /// </summary>
    /// <remarks>Torn about the existence of this utility class, yet alone the conditions dictating it.</remarks>
    public static class Reflection
    {
        /// <summary>
        ///     <see cref="BindingFlags"/> for a private field.
        /// </summary>
        public const BindingFlags PrivateFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        /// <summary>
        ///     <see cref="BindingFlags"/> for a private static.
        /// </summary>
        public const BindingFlags PrivateStaticFlags = BindingFlags.Static | BindingFlags.NonPublic;
        /// <summary>
        ///     <see cref="BindingFlags"/> for a public static.
        /// </summary>
        public const BindingFlags PublicStaticFlags = BindingFlags.Static | BindingFlags.Public;

        /// <summary>
        ///     Returns the default value for a given type.
        /// </summary>
        /// <param name="type">A qualified type.</param>
        /// <returns>The default value.</returns>
        public static object GetDefault(this Type type)
        {
            if (type.IsClass || !type.IsValueType)
            {
                return null;
            }
            return Activator.CreateInstance(type);
        }

        /// <summary>
        ///     Access the field value of a specific <paramref name="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="targetObject">The instanced object which will have it's field value read; use a null value if this is a static field.</param>
        /// <param name="type">The qualified type of the <paramref name="targetObject"/>.</param>
        /// <param name="name">The field's name to read.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <typeparam name="T">The type of data being read from the field.</typeparam>
        /// <returns>The field's value.</returns>
        public static T GetFieldValue<T>(object targetObject, Type type, string name, BindingFlags flags = PrivateFieldFlags)
        {
            if (type == null)
            {
                return default;
            }
            FieldInfo field = type.GetField(name, flags);
            return (T)field?.GetValue(targetObject);
        }

        /// <summary>
        ///     Access the field or property value of a specific <paramref name="targetObject"/>, which may not be
        ///     normally accessible.
        /// </summary>
        /// <remarks>Useful for when you really do not know the <see cref="System.Type"/>.</remarks>
        /// <param name="targetObject">The instanced object which will have it's field or property value read.</param>
        /// <param name="name">The field or property's name to read.</param>
        /// <param name="fieldFlags">The field's access flags.</param>
        /// <param name="propertyFlags">The property's access flags.</param>
        /// <returns>The field/property value.</returns>
        public static object GetFieldOrPropertyValue(object targetObject, string name,
            BindingFlags fieldFlags = PrivateFieldFlags, BindingFlags propertyFlags = PrivateFieldFlags)
        {
            if (targetObject == null)
                return null;

            Type type = targetObject.GetType();
            FieldInfo f = type.GetField(name, fieldFlags);
            if (f != null)
            {
                return f.GetValue(targetObject);
            }

            PropertyInfo p = type.GetProperty(name,propertyFlags);
            return p == null ? null : p.GetValue(targetObject, null);
        }

        /// <summary>
        ///     Access the field value of a specific <paramref name="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="targetObject">The instanced object which will have it's field value read; use a null value if this is a static property.</param>
        /// <param name="type">The explicit type of the <paramref name="targetObject"/>.</param>
        /// <param name="name">The field's name to read.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <typeparam name="T">The type of data being read from the field.</typeparam>
        /// <returns>The field's value.</returns>
        public static T GetPropertyValue<T>(object targetObject, Type type, string name, BindingFlags flags = PrivateFieldFlags)
        {
            if (type == null)
            {
                return default;
            }
            PropertyInfo propertyInfo = type.GetProperty(name, flags);
            return (T)propertyInfo?.GetValue(targetObject);
        }

        /// <summary>
        ///     Returns a qualified type..
        /// </summary>
        /// <param name="type">The full name of a type.</param>
        /// <returns>A <see cref="System.Type"/> if found.</returns>
        public static Type GetType(string type)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            int loadedAssembliesCount = loadedAssemblies.Length;
            for (int i = 0; i < loadedAssembliesCount; i++)
            {
                Type targetType = loadedAssemblies[i].GetType(type);
                if (targetType != null)
                {
                    return targetType;
                }
            }
            return null;
        }

        /// <summary>
        ///     Invokes a known static method.
        /// </summary>
        /// <param name="type">The explicit type of the static class.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="parameters">Any parameters that should be passed to the method?</param>
        /// <param name="flags">The <paramref name="method"/>'s access flags.</param>
        /// <returns>An <see cref="object"/> of the return value. This can be null.</returns>
        public static object InvokeStaticMethod(string type, string method, object[] parameters = null,
            BindingFlags flags = PublicStaticFlags)
        {
            Type targetType = GetType(type);
            if (targetType != null)
            {
                MethodInfo targetMethod = targetType.GetMethod(method, flags);
                if (targetMethod != null)
                {
                    return targetMethod.Invoke(null, parameters ?? Core.EmptyObjectArray);
                }
            }
            return null;
        }

        /// <summary>
        ///     Set the field or property value of a specific <paramref name="targetObject"/>, which may not be
        ///     normally accessible.
        /// </summary>
        /// <param name="targetObject">The instanced object which will have it's field or property value set.</param>
        /// <param name="name">The field or property's name to set.</param>
        /// <param name="value">The value to set the field or property to.</param>
        /// <param name="fieldFlags">The field's access flags.</param>
        /// <param name="propertyFlags">The property's access flags.</param>
        /// <returns>true/false if the value was set.</returns>
        public static bool SetFieldOrPropertyValue(object targetObject, string name, object value,
            BindingFlags fieldFlags = PrivateFieldFlags, BindingFlags propertyFlags = PrivateFieldFlags)
        {
            if (targetObject == null)
                return false;

            Type type = targetObject.GetType();
            FieldInfo f = type.GetField(name, fieldFlags);
            if (f != null)
            {
                f.SetValue(targetObject, value);
                return true;
            }

            PropertyInfo p = type.GetProperty(name,propertyFlags);
            if (p != null)
            {
                p.SetValue(targetObject, value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Set the field value of a specific <paramref name="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <param name="targetObject">The instanced object which will have it's field value set; use a null value if this is a static field.</param>
        /// <param name="type">The explicit type of the <paramref name="targetObject"/>.</param>
        /// <param name="name">The field's name to set.</param>
        /// <param name="value">The value to set the field to.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <returns>true/false if the value was set.</returns>
        public static bool SetFieldValue(object targetObject, Type type, string name, object value,
            BindingFlags flags = PrivateFieldFlags)
        {
            if (type != null)
            {
                FieldInfo field = type.GetField(name, flags);
                if (field != null)
                {
                    field.SetValue(targetObject, value);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Set the field value of a specific <paramref name="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <param name="targetObject">The instanced object which will have it's field value set; use a null value if this is a static property.</param>
        /// <param name="type">The type of the <paramref name="targetObject"/>.</param>
        /// <param name="name">The field's name to set.</param>
        /// <param name="value">The value to set the field to.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <returns>true/false if the value was set.</returns>
        public static bool SetPropertyValue(object targetObject, Type type, string name, object value,
            BindingFlags flags = PrivateFieldFlags)
        {
            if (type != null)
            {
                PropertyInfo property = type.GetProperty(name, flags);
                if (property != null)
                {
                    property.SetValue(targetObject, value);
                    return true;
                }
            }
            return false;
        }
    }
}