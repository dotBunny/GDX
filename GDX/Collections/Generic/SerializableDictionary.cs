// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if !UNITY_DOTSRUNTIME

using System;
using System.Collections.Generic;
using GDX.Experimental;
using UnityEngine;

namespace GDX.Collections.Generic
{

#pragma warning disable IDE0049
    /// <summary>
    ///     A Unity serializable <see cref="Dictionary{TKey,TValue}" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This will NOT work with <see cref="System.Object" /> based objects, use <see cref="UnityEngine.Object" /> if
    ///         you must. While .NET has solutions for creating custom serialization paths, Unity uses its own system to
    ///         serialize data into YAML structures. This also assumes that the types provided can be serialized by Unity.
    ///     </para>
    ///     <para>
    ///         The process of serializing and deserializing this dictionary should not be considered performant.
    ///     </para>
    ///     <para>
    ///         The property drawer will not show structs, it will be blank. It does work however if done through code.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TKey">The dictionary's key <see cref="Type" />.</typeparam>
    /// <typeparam name="TValue">The dictionary's value <see cref="Type" />.</typeparam>
    /// <exception cref="UnsupportedRuntimeException">Not supported on DOTS Runtime.</exception>
    [Serializable]
#pragma warning restore IDE0049
    [VisualScriptingCompatible(1)]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     Is the dictionary completely capable of being serialized by Unity?
        /// </summary>
        /// <remarks>This field is determined/cached in the constructor.</remarks>
        [SerializeField] bool m_IsSerializable;

        /// <summary>
        ///     The length of the serialized data arrays.
        /// </summary>
        [SerializeField] int m_SerializedLength = -1;

        /// <summary>
        ///     An array of all of the keys, in order, used to recreate the base <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        [SerializeField] TKey[] m_SerializedKeys;

        /// <summary>
        ///     An array of all of the values, in order, used to recreate the base <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        [SerializeField] TValue[] m_SerializedValues;

        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006

        /// <summary>
        ///     Type constructor.
        /// </summary>
        public SerializableDictionary()
        {
#if UNITY_EDITOR
            m_IsSerializable = IsSerializableType(typeof(TKey)) && IsSerializableType(typeof(TValue));
#endif // UNITY_EDITOR
        }

        /// <summary>
        ///     Get the length of the serialized data arrays.
        /// </summary>
        /// <returns>An integer value representing the count.</returns>
        public int GetSerializedDataLength()
        {
            return m_SerializedLength;
        }

        /// <summary>
        ///     Is the <paramref name="type" /> capable of being serialized by the
        ///     <see cref="SerializableDictionary{TKey,TValue}" />, utilizing Unity's own serialization system?
        /// </summary>
        /// <returns>true/false if the type is valid.</returns>
        public static bool IsSerializableType(Type type)
        {
            return type != typeof(object);
        }

        /// <summary>
        ///     Load the data into the <see cref="Dictionary{TKey,TValue}" /> cached in the serialized data.
        /// </summary>
        /// <param name="clearAfterLoad">Should the serialized data be cleared after loading?</param>
        public void LoadSerializedData(bool clearAfterLoad = true)
        {
            Clear();

            // If this is not serializable we need to do nothing
            if (!m_IsSerializable || m_SerializedLength <= 0)
            {
                return;
            }

            // Iterate over all the serialized data and put it back into the dictionary as it once was, in order.
            for (int i = 0; i < m_SerializedLength; i++)
            {
#if UNITY_EDITOR
                // If the key is already in the dataset what do we do?
                if (ContainsKey(m_SerializedKeys[i]))
                {
                    ManagedLog.Error(
                        "A duplicate key has been detected in the serialized dictionary, the item has been removed.\nYou can undo your last action to restore the previous state.");
                }
                else
                {
#endif // UNITY_EDITOR
                    Add(m_SerializedKeys[i], m_SerializedValues[i]);
#if UNITY_EDITOR
                }
#endif // UNITY_EDITOR
            }

            // Remove any data cached so that references are not held.
            if (!clearAfterLoad)
            {
                return;
            }

            m_SerializedLength = -1;
            m_SerializedKeys = null;
            m_SerializedValues = null;
        }

        /// <summary>
        ///     Rehydrate the serialized data arrays back into a cohesive <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <remarks>Invoked by Unity, calls <see cref="LoadSerializedData" />.</remarks>
        public void OnAfterDeserialize()
        {
            LoadSerializedData();
        }

        /// <summary>
        ///     Build out serialized data arrays and associative data, used to rehydrate during deserialization.
        /// </summary>
        /// <remarks>Invoked by Unity, calls <see cref="SaveSerializedData" />.</remarks>
        public void OnBeforeSerialize()
        {
            SaveSerializedData();
        }

        /// <summary>
        ///     Overwrite data in the serialized arrays with the provided data.
        /// </summary>
        /// <param name="keyArray">An array of keys.</param>
        /// <param name="valueArray">An array of values.</param>
        public void OverwriteSerializedData(TKey[] keyArray, TValue[] valueArray)
        {
            if (keyArray.Length != valueArray.Length)
            {
                ManagedLog.Error("The provided array lengths must match.");
                return;
            }

            m_SerializedKeys = keyArray;
            m_SerializedValues = valueArray;
            m_SerializedLength = keyArray.Length;
        }

        /// <summary>
        ///     Fill serializable arrays from dictionary data.
        /// </summary>
        /// <remarks>We will always create the arrays so the property drawers function nicely.</remarks>
        public void SaveSerializedData()
        {
            // If this is not serializable we need to do nothing
            if (!m_IsSerializable)
            {
                return;
            }

            // Stash our length for future usage
            m_SerializedLength = Count;

            // Create our serialized data arrays
            m_SerializedKeys = new TKey[m_SerializedLength];
            m_SerializedValues = new TValue[m_SerializedLength];

            // Stash our values
            int index = 0;
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                m_SerializedKeys[index] = pair.Key;
                m_SerializedValues[index] = pair.Value;
                index++;
            }
        }
    }
}
#endif // !UNITY_DOTSRUNTIME