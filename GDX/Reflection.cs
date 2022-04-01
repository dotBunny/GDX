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
        ///     Access the field value of a specific <see cref="targetObject"/>, which may not be normally accessible.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="targetObject">The instanced object which will have it's field value read.</param>
        /// <param name="type">The explicit type of the <see cref="targetObject"/>.</param>
        /// <param name="name">The field's name to read.</param>
        /// <param name="flags">The field's access flags.</param>
        /// <typeparam name="T">The type of data being read from the field.</typeparam>
        /// <returns>The field's value.</returns>
        public static T GetFieldValue<T>(object targetObject, string type, string name, BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance)
        {
            foreach (Assembly targetAssembly in Platform.GetLoadedAssemblies())
            {
                Type targetType = targetAssembly.GetType(type);
                if (targetType == null)
                {
                    continue;
                }
                FieldInfo field = targetType.GetField(name, flags);
                return (T)field?.GetValue(targetObject);
            }
            return default;
        }
    }
}