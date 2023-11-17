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
        static readonly object k_Lock = new object();

        static readonly List<Watch> k_KnownWatches = new List<Watch>();
        static readonly StringBuilder k_NamesBuilder = new StringBuilder();
        static readonly StringBuilder k_ValuesBuilder = new StringBuilder();

        static string s_CachedNames;
        static bool s_CachedNamesDirty = true;

        public bool Enabled { get; set; }

        readonly string m_Name;
        string m_Value;

        readonly Func<string> m_GetValue;

        public Watch(string name, Func<string> getValue, bool enabled = true)
        {
            m_Name = name;
            m_GetValue = getValue;
            Enabled = enabled;
            lock (k_Lock)
            {
                k_KnownWatches.Add(this);
            }
        }

        public void SetState(bool enabled)
        {
            if (Enabled == enabled)
            {
                return;
            }

            Enabled = enabled;
            s_CachedNamesDirty = true;
        }
        public void Poll()
        {
            // It's not turned on, so dont even bother
            if (!Enabled) return;

            // Poll for our new value
            m_Value = m_GetValue();
        }

        public static void PollKnown()
        {
            lock (k_Lock)
            {
                int count = k_KnownWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    k_KnownWatches[i].Poll();
                }
            }
        }

        public static (string, string) GetColumns()
        {
            lock (k_Lock)
            {
                if (s_CachedNamesDirty)
                {
                    k_NamesBuilder.Clear();
                }

                k_ValuesBuilder.Clear();
                int count = k_KnownWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    Watch watch = k_KnownWatches[i];
                    if (!watch.Enabled) continue;
                    if (s_CachedNamesDirty)
                    {
                        k_NamesBuilder.AppendLine(watch.m_Name);
                    }

                    k_ValuesBuilder.AppendLine(watch.m_Value);
                }

                if (s_CachedNamesDirty)
                {
                    s_CachedNames = k_NamesBuilder.ToString();
                    s_CachedNamesDirty = false;
                }

                return (s_CachedNames, k_ValuesBuilder.ToString());
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}