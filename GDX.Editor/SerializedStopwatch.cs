// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    /// An obscure usage stopwatch meant to track times across domain-reload and serialization.
    /// </summary>
    [Serializable]
    public class SerializedStopwatch : ISerializationCallbackReceiver
    {
        [SerializeField]
        double m_SerializationTick;
        double m_SerializationSpan;

        [SerializeField]
        double m_StartTick;

        /// <summary>
        /// Get the starting point for the stopwatch.
        /// </summary>
        /// <returns>The <see cref="DateTime.Now"/> set by <see cref="SetStartTick"/>.</returns>
        public double GetStartTick()
        {
            return m_StartTick;
        }


        public void SetStartTick()
        {
            m_StartTick = DateTime.Now.Ticks;
        }

        /// <summary>
        /// Returns the total number of ticks elapsed since <see cref="GetStartTick"/> was called.
        /// </summary>
        public double ElapsedTicks
        {
            get
            {
                return DateTime.Now.Ticks - m_StartTick;
            }
        }

        public double SerializationSpan
        {
            get
            {
                return m_SerializationSpan;
            }
        }

        public void OnBeforeSerialize()
        {
            m_SerializationTick = DateTime.Now.Ticks;
        }

        public void OnAfterDeserialize()
        {
            m_SerializationSpan = DateTime.Now.Ticks - m_SerializationTick;
        }
    }
}