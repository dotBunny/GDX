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
    ///         you must. While .NET has solutions for creating custom serialization paths, Unity uses its own system to
    ///         serialize data into YAML structures. This also assumes that the types provided can be serialized by Unity.
    ///     </para>
    ///     <para>
    ///         The process of serializing and deserializing this dictionary should not be considered performant.
    ///     </para>
    /// </remarks>
    /// <typeparam name="TKey">The dictionary's key <see cref="System.Type" />.</typeparam>
    /// <typeparam name="TValue">The dictionary's value <see cref="System.Type" />.</typeparam>
    [Serializable]
    [VisualScriptingCollection]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        /// <summary>
        ///     Is the dictionary completely capable of being serialized by Unity?
        /// </summary>
        /// <remarks>This field is determined/cached in the constructor.</remarks>
        [HideInInspector] [SerializeField] private bool isSerializable;

        /// <summary>
        ///     The length of the serialized data arrays.
        /// </summary>
        [HideInInspector] [SerializeField] private int serializedLength = -1;

        /// <summary>
        ///     An array of all of the keys, in order, used to recreate the base <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        [HideInInspector] [SerializeField] private TKey[] serializedKeys;

        /// <summary>
        ///     An array of all of the values, in order, used to recreate the base <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        [HideInInspector] [SerializeField] private TValue[] serializedValues;

        /// <summary>
        ///     Type constructor.
        /// </summary>
        public SerializableDictionary()
        {
#if UNITY_EDITOR
            isSerializable = IsSerializableType(typeof(TKey)) && IsSerializableType(typeof(TValue));
#endif
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

            // While a Behaviour itself can be null, when its a MonoBehaviour we cannot.
            if (type == typeof(MonoBehaviour))
            {
                return false;
            }

            if (!type.IsValueType)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(type) != null;
        }

        /// <summary>
        ///     Is the <paramref name="type" /> capable of being serialized by the
        ///     <see cref="SerializableDictionary{TKey,TValue}" />, utilizing Unity's own serialization system?
        /// </summary>
        /// <returns>true/false if the type is valid.</returns>
        public bool IsSerializableType(Type type)
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
            if (!isSerializable)
            {
                return;
            }

            if (serializedLength <= 0)
            {
#if UNITY_EDITOR
                // Don't allow null keys for non-nullables
                if (!IsNullableKey() && serializedAddKey == null)
                {
                    serializedAddKeyValid = false;
                }
                else
                {
                    serializedAddKeyValid = true;
                }
#endif

                return;
            }

            // Iterate over all the serialized data and put it back into the dictionary as it once was, in order.
            for (int i = 0; i < serializedLength; i++)
            {
#if UNITY_EDITOR
                // If the key is already in the dataset what do we do?
                if (ContainsKey(serializedKeys[i]))
                {
                    Trace.Output(Trace.TraceLevel.Error, "A duplicate key has been detected in the serialized dictionary, the item has been removed.\nYou can undo your last action to restore the previous state.");
                }
                else
                {
#endif
                    Add(serializedKeys[i], serializedValues[i]);
#if UNITY_EDITOR
                }
#endif
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
        ///     Rehydrate the serialized data arrays back into a cohesive <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <remarks>Invoked by Unity, calls <see cref="LoadSerializedData" />.</remarks>
        public void OnAfterDeserialize()
        {
            LoadSerializedData();
#if !UNITY_EDITOR
            // Make sure nothing is there holding a reference.
            _padForSerializationKey = default;
#endif
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
                Trace.Output(Trace.TraceLevel.Error,"The provided array lengths must match.");
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
            // If this is not serializable we need to do nothing
            if (!isSerializable)
            {
                return;
            }

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

#pragma warning disable 414
#if UNITY_EDITOR
        /// <summary>
        ///     Editor only data indicating if the property drawer is expanded.
        /// </summary>
        // ReSharper disable once NotAccessedField.Local
        [HideInInspector] [SerializeField] private bool drawerExpanded;

        /// <summary>
        ///     Is the provided key a valid key (unique).
        /// </summary>
        [HideInInspector] [SerializeField] private bool serializedAddKeyValid;

        /// <summary>
        ///     Temporary placement for keys to be added.
        /// </summary>
        [HideInInspector] [SerializeField] private TKey serializedAddKey;
#else
        // We need to pad the size of the serialized data due to Unity checking the size of the serialized object.
        // This is a problem where "classic" Unity author-time data, is the same as runtime data.
        [SerializeField] private bool _padForSerializationBoolA;
        [SerializeField] private bool _padForSerializationBoolB;
        [SerializeField] private TKey _padForSerializationKey;
#endif
#pragma warning restore 414
    }
}