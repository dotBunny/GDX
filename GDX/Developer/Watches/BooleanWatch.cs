// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    /// <summary>
    /// Thread-safe developer watch system.
    /// </summary>
    public class BooleanWatch : WatchBase
    {
        const string k_FalseText = "False";
        const string k_TrueText = "True";

        readonly Func<bool> m_GetValue;
        bool m_CachedHash;

        readonly Label m_DisplayNameLabel;
        readonly Label m_ValueLabel;

        public BooleanWatch(string uniqueIdentifier, string displayName, Func<bool> getValue, bool enabled = true, int minWidth =-1, int minHeight = -1)
            : base(uniqueIdentifier, displayName, enabled, minWidth, minHeight)
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
            bool getValue = m_GetValue();

            // Only change if legit change
            if (m_CachedHash != getValue)
            {
                m_ValueLabel.text = getValue ? k_TrueText : k_FalseText;
                m_CachedHash = getValue;
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER