// dotBunny licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GDX.Collections.Generic
{
    /// <summary>
    ///     A Unity serializable <see cref="Dictionary{TKey,TValue}" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This will NOT work with <see cref="System.Object" /> based objects, use <see cref="UnityEngine.Object" /> if
    ///         you must.
    ///     </para>
    ///     <para>
    ///         While .NET has solutions for creating custom serialization paths, Unity uses its own system to serialize data
    ///         into YAML structures. This also assumes that the types provided can be serialized by Unity.
    ///     </para>
    ///     <para>
    ///         The process of serializing and deserializing this dictionary is not to be considered performant in any way.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TKey">The <see cref="KeyValuePair{TKey,TValue}" />'s key type.</typeparam>
    /// <typeparam name="TValue">The <see cref="KeyValuePair{TKey,TValue}" />'s value type.</typeparam>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        /// <summary>
        ///     A cached length of the serialized data arrays.
        /// </summary>
        [HideInInspector] [SerializeField] private int serializedLength = -1;

        /// <summary>
        ///     An array of all of the keys, in order, used by the dictionary.
        /// </summary>
        [HideInInspector] [SerializeField] private TKey[] serializedKeys;

        /// <summary>
        ///     An array of all the data, in order, used by the dictionary.
        /// </summary>
        [HideInInspector] [SerializeField] private TValue[] serializedValues;

        public SerializableDictionary()
        {
            isSerializable = IsKeyValidType() && IsValueValidType();
        }


        /// <summary>
        ///     Rehydrate the serialized data arrays back into a <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <remarks>Calls <see cref="LoadSerializedData" />.</remarks>
        public void OnAfterDeserialize()
        {
            LoadSerializedData();
        }

        /// <summary>
        ///     Prepare our data for serialization, moving it into arrays.
        /// </summary>
        /// <remarks>Calls <see cref="SaveSerializedData" />.</remarks>
        public void OnBeforeSerialize()
        {
            SaveSerializedData();
        }

        /// <summary>
        ///     Creates a copy from the serialized data.
        /// </summary>
        /// <returns>
        ///     A new <see cref="SerializableDictionary{TKey,TValue}" /> filled with the the serialized data of this
        ///     <see cref="SerializableDictionary{TKey,TValue}" />.
        /// </returns>
        public SerializableDictionary<TKey, TValue> CreateFromSerializedData()
        {
            // Make sure we have some form of data to work with
            SerializableDictionary<TKey, TValue> newDictionary = new SerializableDictionary<TKey, TValue>();

            if (serializedLength <= 0)
            {
                return newDictionary;
            }

            // Iterate over all the serialized data and put it back into the dictionary as it once was, in order.
            for (int i = 0; i < serializedLength; i++)
            {
                newDictionary.Add(serializedKeys[i], serializedValues[i]);
            }

            // Return filled dictionary
            return newDictionary;
        }

        /// <summary>
        ///     Get the length of the serialized data arrays.
        /// </summary>
        /// <returns>An integer value representing the count.</returns>
        public int GetSerializedDataLength()
        {
            return serializedLength;
        }

        /// <summary>
        ///     Is the dictionaries key nullable?
        /// </summary>
        /// <returns>true/false if nullable.</returns>
        public bool IsNullableKey()
        {
            Type type = typeof(TKey);
            if (!type.IsValueType)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        ///     Is the dictionaries value nullable?
        /// </summary>
        /// <returns>true/false if nullable.</returns>
        public bool IsNullableValue()
        {
            Type type = typeof(TValue);
            if (!type.IsValueType)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        ///     Is the dictionary <see cref="TKey" /> a valid type?
        /// </summary>
        /// <returns>true/false if the type is valid.</returns>
        public bool IsKeyValidType()
        {
            Type type = typeof(TKey);
            return type != typeof(object);
        }

        /// <summary>
        ///     Is the dictionary <see cref="TValue" /> a valid type?
        /// </summary>
        /// <returns>true/false if the type is valid.</returns>
        public bool IsValueValidType()
        {
            Type type = typeof(TValue);
            return type != typeof(object);
        }

        /// <summary>
        ///     Load the data into the <see cref="Dictionary{TKey,TValue}" /> cached in the serialized data.
        /// </summary>
        /// <param name="clearAfterLoad">Should the serialized data be cleared after loading?</param>
        public void LoadSerializedData(bool clearAfterLoad = true)
        {
            Clear();

#if UNITY_EDITOR
            // If this is not serializable we need to do nothing
            if (!isSerializable)
            {
                return;
            }
#endif

            if (serializedLength <= 0)
            {
                // Don't allow null keys for non-nullables
                if (!IsNullableKey() && serializedAddKey == null)
                {
                    serializedAddKeyValid = false;
                }
                else
                {
                    serializedAddKeyValid = true;
                }

                return;
            }

            // Iterate over all the serialized data and put it back into the dictionary as it once was, in order.
            for (int i = 0; i < serializedLength; i++)
            {
                Add(serializedKeys[i], serializedValues[i]);
            }

#if UNITY_EDITOR

            // We need to check if the key is actually nullable
            if (!IsNullableKey() && serializedAddKey == null)
            {
                serializedAddKeyValid = false;
            }
            else
            {
                serializedAddKeyValid = !ContainsKey(serializedAddKey);
                if (!serializedAddKeyValid)
                {
                    serializedAddKey = default;
                }
            }
#endif

            // Remove any data cached so that references are not held.
            if (!clearAfterLoad)
            {
                return;
            }

            serializedLength = -1;
            serializedKeys = null;
            serializedValues = null;
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
                Debug.LogError("The provided array lengths must match.");
                return;
            }

            serializedKeys = keyArray;
            serializedValues = valueArray;
            serializedLength = keyArray.Length;
        }

        /// <summary>
        ///     Fill serializable arrays from dictionary data.
        /// </summary>
        /// <remarks>We will always create the arrays so the property drawers function nicely.</remarks>
        public void SaveSerializedData()
        {
#if UNITY_EDITOR
            // If this is not serializable we need to do nothing
            if (!isSerializable)
            {
                return;
            }
#endif

            // Stash our length for future usage
            serializedLength = Count;

            // Create our serialized data arrays
            serializedKeys = new TKey[serializedLength];
            serializedValues = new TValue[serializedLength];

            // Stash our values
            int index = 0;
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                serializedKeys[index] = pair.Key;
                serializedValues[index] = pair.Value;
                index++;
            }
        }
#if UNITY_EDITOR

        /// <summary>
        ///     Editor only data indicating if the property drawer is expanded.
        /// </summary>
#pragma warning disable 414
        [HideInInspector] [SerializeField] private bool drawerExpanded;
#pragma warning restore 414

        /// <summary>
        ///     Editor only data indicating if the property drawer is expanded.
        /// </summary>
        [HideInInspector] [SerializeField] private bool isSerializable;

        /// <summary>
        ///     Temporary placement for keys to be added.
        /// </summary>
        [HideInInspector] [SerializeField] private TKey serializedAddKey;

        /// <summary>
        ///     Is the provided key a valid key (unique).
        /// </summary>
        [HideInInspector] [SerializeField] private bool serializedAddKeyValid;
#endif
    }
}