// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class WatchProvider
    {
        public static bool HasWatches;
        public static bool DisplayNamesChanged;
        public static string DisplayNames;
        public static string DisplayValues;

        static readonly object k_Lock = new object();
        static readonly List<Watch> k_KnownWatches = new List<Watch>();
        static readonly StringBuilder k_NamesBuilder = new StringBuilder();
        static readonly StringBuilder k_ValuesBuilder = new StringBuilder();

        static bool s_IsDisplayNamesDirty = true;

        public static void Register(Watch watch)
        {
            lock (k_Lock)
            {
                k_KnownWatches.Add(watch);
                HasWatches = true;
            }
        }

        public static void SetDirty()
        {
            lock (k_Lock)
            {
                s_IsDisplayNamesDirty = true;
            }
        }

        public static void Poll()
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

        public static void Process()
        {
            lock (k_Lock)
            {
                if (s_IsDisplayNamesDirty)
                {
                    k_NamesBuilder.Clear();
                }

                k_ValuesBuilder.Clear();
                int count = k_KnownWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    Watch watch = k_KnownWatches[i];
                    if (!watch.Enabled) continue;
                    if (s_IsDisplayNamesDirty)
                    {
                        k_NamesBuilder.AppendLine(watch.Name);
                    }

                    k_ValuesBuilder.AppendLine(watch.m_Value);
                }


                DisplayValues = k_ValuesBuilder.ToString();
                if (!s_IsDisplayNamesDirty)
                {
                    DisplayNamesChanged = false;
                    return;
                }

                DisplayNames = k_NamesBuilder.ToString();
                DisplayNamesChanged = true;
                s_IsDisplayNamesDirty = false;
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}