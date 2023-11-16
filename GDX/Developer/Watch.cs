// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
        static readonly StringBuilder k_StringBuilder = new StringBuilder();
        static int s_LongestDisplayText;

        public bool Enabled { get; set; }

        readonly string m_DisplayText;
        string m_DisplayValue;
        bool m_HasChangeThisPoll;

        readonly Func<string> m_GetValue;

        public Watch(string displayText, Func<string> getValue, bool enabled = true)
        {
            m_DisplayText = displayText;
            m_GetValue = getValue;
            Enabled = true;
            lock (k_Lock)
            {
                if (displayText.Length > s_LongestDisplayText)
                {
                    s_LongestDisplayText = displayText.Length;
                }
                k_KnownWatches.Add(this);
            }
        }

        public static void PollKnown()
        {
            lock (k_Lock)
            {
                int count = k_KnownWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    Watch w = k_KnownWatches[i];

                    // It's not turned on, so dont even bother
                    if (!w.Enabled) continue;

                    // Poll for our new value
                    string newValue = w.m_GetValue();

                    // Do we have an actual new value to update our UI with
                    w.m_HasChangeThisPoll = (newValue != w.m_DisplayValue);

                    // Only update the value if necessary
                    if (w.m_HasChangeThisPoll)
                    {
                        w.m_DisplayValue = newValue;
                    }
                }
            }
        }

        public static string GetPanelContent()
        {
            k_StringBuilder.Clear();
            lock (k_Lock)
            {
                int count = k_KnownWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    Watch w = k_KnownWatches[i];
                    //k_StringBuilder.Append()
                }
            }
            return k_StringBuilder.ToString();
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}