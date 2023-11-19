// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
using GDX.Collections.Generic;
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class WatchProvider
    {
        public static ushort Version = 0;
        static readonly object k_Lock = new object();
        static readonly List<WatchBase> k_KnownWatches = new List<WatchBase>();


        public static VisualElement[] GetElements()
        {
            lock (k_Lock)
            {
                int count = k_KnownWatches.Count;
                SimpleList<VisualElement> elements = new SimpleList<VisualElement>(count);
                for (int i = 0; i < count; i++)
                {
                    elements.AddUnchecked(k_KnownWatches[i].GetElement());
                }
                return elements.Array;
            }
        }

        public static void Register(WatchBase watch)
        {
            lock (k_Lock)
            {
                k_KnownWatches.Add(watch);
                Version++;
            }
        }
        public static void Unregister(WatchBase watch)
        {
            lock (k_Lock)
            {
                k_KnownWatches.Remove(watch);
                Version++;
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
    }
#endif // UNITY_2022_2_OR_NEWER
}