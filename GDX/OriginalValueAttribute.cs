// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace GDX
{
    /// <summary>
    ///     An attribute allowing for the storage of some additional data along side of a field or property.
    /// </summary>
    [HideFromDocFX]
    [ExcludeFromCodeCoverage]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OriginalValueAttribute : Attribute
    {
        readonly object m_BoxedValue;

        public OriginalValueAttribute(object value)
        {
            m_BoxedValue = value;
        }

        public T As<T>()
        {
            return (T)m_BoxedValue;
        }

        /// <summary>
        /// Get the original value associated with the given field.
        /// </summary>
        /// <param name="field">A given <see cref="FieldInfo"/>.</param>
        /// <typeparam name="T">The type of data being read.</typeparam>
        /// <returns>The original value.</returns>
        public static T GetValue<T>(FieldInfo field)
        {
            if (field == null) return default;

            OriginalValueAttribute attribute = (OriginalValueAttribute)field
                .GetCustomAttribute(typeof(OriginalValueAttribute), false);

            return attribute.As<T>();
        }

        /// <summary>
        /// Get the original value associated with the given property.
        /// </summary>
        /// <param name="property">A given <see cref="PropertyInfo"/>.</param>
        /// <typeparam name="T">The type of data being read.</typeparam>
        /// <returns>The original value.</returns>
        public static T GetValue<T>(PropertyInfo property)
        {
            if (property == null) return default;

            OriginalValueAttribute attribute = (OriginalValueAttribute)property
                .GetCustomAttribute(typeof(OriginalValueAttribute), false);

            return attribute.As<T>();
        }
    }
}