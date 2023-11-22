// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class WatchProvider
    {
        public struct WatchList
        {
            public string[] Identfiers;
            public string[] DisplayNames;
            public bool[] IsActive;

            public WatchList(int totalCount)
            {
                Identfiers = new string[totalCount];
                DisplayNames = new string[totalCount];
                IsActive = new bool[totalCount];
            }
        }

        public static ushort Version = 0;
        static readonly object k_Lock = new object();

        static readonly StringKeyDictionary<WatchBase>
            k_KnownWatches = new StringKeyDictionary<WatchBase>(50);

        static readonly List<WatchBase> k_KnownActiveWatches = new List<WatchBase>(50);

        public static void SetState(WatchBase watch, bool enabled)
        {
            if (enabled)
            {
                if (!k_KnownActiveWatches.Contains(watch))
                {
                    lock (k_Lock)
                    {
                        k_KnownActiveWatches.Add(watch);
                        Version++;
                    }
                }

                watch.ContainerElement.style.display = VisualElementStyles.DisplayVisible;
            }
            else
            {
                if (k_KnownActiveWatches.Contains(watch))
                {
                    lock (k_Lock)
                    {
                        k_KnownActiveWatches.Remove(watch);
                        Version++;
                    }
                }

                watch.ContainerElement.style.display = VisualElementStyles.DisplayHidden;
            }
        }

        public static WatchBase GetWatch(string identifier)
        {
            return k_KnownWatches[identifier];
        }

        public static WatchList GetWatchList()
        {
            lock (k_Lock)
            {
                int count = k_KnownWatches.Count;
                WatchList details = new WatchList(count);


                return details;
            }
        }

        public static VisualElement[] GetElements()
        {
            lock (k_Lock)
            {
                int count = k_KnownActiveWatches.Count;
                SimpleList<VisualElement> elements = new SimpleList<VisualElement>(count);
                for (int i = 0; i < count; i++)
                {
                    elements.AddUnchecked(k_KnownActiveWatches[i].GetElement());
                }

                return elements.Array;
            }
        }

        public static void Register(WatchBase watch, bool enabled = true)
        {
            lock (k_Lock)
            {
                if (k_KnownWatches.AddWithUniqueCheck(watch.Identifier, watch))
                {
                    SetState(watch, enabled);
                }
            }
        }

        public static void Unregister(WatchBase watch)
        {
            lock (k_Lock)
            {
                if (k_KnownWatches.TryRemove(watch.Identifier))
                {
                    SetState(watch, false);
                }
            }
        }

        public static void Poll()
        {
            lock (k_Lock)
            {
                int count = k_KnownActiveWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    k_KnownActiveWatches[i].Poll();
                }
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}