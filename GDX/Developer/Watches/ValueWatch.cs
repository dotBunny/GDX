// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    /// <summary>
    /// Thread-safe developer watch system.
    /// </summary>
    public class ValueWatch : WatchBase
    {
        readonly Func<string> m_GetValue;
        int m_CachedHash;

        readonly Label m_NameElement;
        readonly Label m_ValueElement;

        public ValueWatch(string name, Func<string> getValue, bool enabled = true) : base(enabled)
        {
            // TOOD: enabled now doesnt work - cause the set state is done in base, and the turning off of the container is done there :/
            m_GetValue = getValue;

            m_NameElement = new Label() { text = name };
            m_ValueElement = new Label();

            m_ContainerElement.Add(m_NameElement);
            m_ContainerElement.Add(m_ValueElement);
        }

        public override void Poll()
        {
            // It's not turned on, so dont even bother
            if (!m_Enabled) return;

            // Poll for our new value
            string getValue = m_GetValue();
            int getValueHash = getValue.GetStableHashCode();

            // Only change if legit change
            if (m_CachedHash != getValueHash)
            {
                m_ValueElement.text = getValue;
                m_CachedHash = getValueHash;
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}