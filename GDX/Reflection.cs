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
        public const BindingFlags PrivateFieldFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        public const BindingFlags PrivateStaticFlags = BindingFlags.Static | BindingFlags.NonPublic;
        public const BindingFlags PublicStaticFlags = BindingFlags.Static | BindingFlags.Public;
        public const BindingFlags SerializationFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        ///     Access the field value of a specific <see cref="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="targetObject">The instanced object which will have it's field value read.</param>
        /// <param name="type">The explicit type of the <see cref="targetObject"/>.</param>
        /// <param name="name">The field's name to read.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <typeparam name="T">The type of data being read from the field.</typeparam>
        /// <returns>The field's value.</returns>
        public static T GetFieldValue<T>(object targetObject, string type, string name, BindingFlags flags = PrivateFieldFlags)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            int loadedAssembliesCount = loadedAssemblies.Length;
            for (int i = 0; i < loadedAssembliesCount; i++)
            {
                Type targetType = loadedAssemblies[i].GetType(type);
                if (targetType == null)
                {
                    continue;
                }
                FieldInfo field = targetType.GetField(name, flags);
                return (T)field?.GetValue(targetObject);
            }
            return default;
        }

        public static FieldInfo GetFieldUnambiguous(this Type type, string name, BindingFlags flags = SerializationFlags)
        {
           flags |= BindingFlags.DeclaredOnly;

            while (type != null)
            {
                FieldInfo field = type.GetField(name, flags);

                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }

            return null;
        }

        public static object GetDefault(this Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }
            return default;
        }
        /// <summary>
        ///     Invokes a known static method.
        /// </summary>
        /// <param name="type">The explicit type of the static class.</param>
        /// <param name="method">The name of the method to invoke.</param>
        /// <param name="parameters">Any parameters that should be passed to the method?</param>
        /// <param name="flags">The <see cref="method"/>s' access flags.</param>
        /// <returns>An <see cref="object"/> of the return value. This can be null.</returns>
        public static object InvokeStaticMethod(string type, string method, object[] parameters = null,
            BindingFlags flags = PublicStaticFlags)
        {
            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            int loadedAssembliesCount = loadedAssemblies.Length;
            for (int i = 0; i < loadedAssembliesCount; i++)
            {
                Type targetType = loadedAssemblies[i].GetType(type);
                if (targetType == null)
                {
                    continue;
                }
                MethodInfo targetMethod = targetType.GetMethod(method, flags);

                if (targetMethod != null)
                {
                    return targetMethod.Invoke(null, parameters ?? new object[] { });
                }
                break;
            }
            return null;
        }
    }
}