// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    /// <summary>
    /// Thread-safe developer watch system.
    /// </summary>
    public class Watch
    {
        public bool Enabled { get; private set; }

        internal readonly string Name;
        internal string m_Value;

        readonly Func<string> m_GetValue;

        public Watch(string name, Func<string> getValue, bool enabled = true)
        {
            Name = name;
            m_GetValue = getValue;
            Enabled = enabled;
            WatchProvider.Register(this);

        }

        public void SetState(bool enabled)
        {
            if (Enabled == enabled)
            {
                return;
            }
            Enabled = enabled;
            WatchProvider.SetDirty();
        }

        public void Poll()
        {
            // It's not turned on, so dont even bother
            if (!Enabled) return;

            // Poll for our new value
            m_Value = m_GetValue();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}