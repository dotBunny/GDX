// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
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

        readonly Label m_DisplayNameLabel;
        readonly Label m_ValueLabel;

        public ValueWatch(string uniqueIdentifier, string displayName, Func<string> getValue, bool enabled = true) :
            base(uniqueIdentifier, displayName, enabled)
        {
            m_GetValue = getValue;

            m_DisplayNameLabel = new Label() { text = displayName };
            m_DisplayNameLabel.AddToClassList("gdx-watch-left");

            m_ValueLabel = new Label();
            m_ValueLabel.AddToClassList("gdx-watch-right");

            ContainerElement.Add(m_DisplayNameLabel);
            ContainerElement.Add(m_ValueLabel);
        }

        public override void Poll()
        {
            // Poll for our new value
            string getValue = m_GetValue();
            int getValueHash = getValue.GetStableHashCode();

            // Only change if legit change
            if (m_CachedHash != getValueHash)
            {
                m_ValueLabel.text = getValue;
                m_CachedHash = getValueHash;
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}