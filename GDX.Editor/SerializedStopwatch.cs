// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using UnityEngine;

namespace GDX.Editor
{
    /// <summary>
    ///     An obscure usage stopwatch meant to track times across domain-reload and serialization.
    /// </summary>
    [Serializable]
    public class SerializedStopwatch : ISerializationCallbackReceiver
    {
        /// <summary>
        ///     Returns the total number of ticks elapsed since <see cref="GetStartTick" /> was called.
        /// </summary>
        public double ElapsedTicks => DateTime.Now.Ticks - m_StartTick;

        public double SerializationSpan { get; private set; }

        public void OnBeforeSerialize()
        {
            m_SerializationTick = DateTime.Now.Ticks;
        }

        public void OnAfterDeserialize()
        {
            SerializationSpan = DateTime.Now.Ticks - m_SerializationTick;
        }


        /// <summary>
        ///     Get the starting point for the stopwatch.
        /// </summary>
        /// <returns>The <see cref="DateTime.Now" /> set by <see cref="SetStartTick" />.</returns>
        public double GetStartTick()
        {
            return m_StartTick;
        }


        public void SetStartTick()
        {
            m_StartTick = DateTime.Now.Ticks;
        }
#pragma warning disable IDE1006
        // ReSharper disable InconsistentNaming
        [SerializeField] double m_SerializationTick;

        [SerializeField] double m_StartTick;
        // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
    }
}