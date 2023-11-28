// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using GDX.Logging;
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public static class WatchProvider
    {
        public static ushort Version;
        static uint s_OverrideTicket;
        static readonly object k_Lock = new object();

        static StringKeyDictionary<WatchBase>
            s_KnownWatches = new StringKeyDictionary<WatchBase>(50);

        static readonly List<WatchBase> k_KnownActiveWatches = new List<WatchBase>(50);

        public static void ToggleState(WatchBase watch)
        {
            SetState(watch, !k_KnownActiveWatches.Contains(watch));
        }

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

                if (watch.ContainerElement != null)
                {
                    watch.ContainerElement.style.display = VisualElementStyles.DisplayVisible;
                }
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

                if (watch.ContainerElement != null)
                {
                    watch.ContainerElement.style.display = VisualElementStyles.DisplayHidden;
                }
            }
        }

        public static int GetTotalCount()
        {
            return s_KnownWatches.Count;
        }

        public static bool HasActiveWatches()
        {
            return k_KnownActiveWatches.Count > 0;
        }

        public static WatchBase GetWatch(string identifier)
        {
            return !s_KnownWatches.ContainsKey(identifier) ? null : s_KnownWatches[identifier];
        }

        public static bool HasWatch(string identifier)
        {
            return s_KnownWatches.ContainsKey(identifier);
        }

        public static WatchList GetWatchList()
        {
            lock (k_Lock)
            {
                int count = s_KnownWatches.Count;
                WatchList details = new WatchList(count);
                int iteratedIndexCount = 0;
                int index = 0;
                while (s_KnownWatches.MoveNext(ref iteratedIndexCount, out StringKeyEntry<WatchBase> entry))
                {
                    WatchBase target = entry.Value;
                    details.IsActive[index] = k_KnownActiveWatches.Contains(target);
                    details.DisplayNames[index] = target.DisplayName;
                    details.Identfiers[index] = target.Identifier;
                    index++;
                }

                return details;
            }
        }

        public static VisualElement[] GetActiveElements()
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
                if (s_KnownWatches.ContainsKey(watch.Identifier))
                {
                    s_OverrideTicket++;
                    watch.SetOverrideIdentifier(s_OverrideTicket);
#if UNITY_EDITOR
                    ManagedLog.Warning(LogCategory.GDX,
                        $"Duplicate registered watch identifier for '{watch.BaseIdentifier}' attempting @ {s_OverrideTicket.ToString()}");
#endif
                    Register(watch, enabled);
                }
                else
                {
                    s_KnownWatches.AddWithExpandCheck(watch.Identifier, watch);
                    SetState(watch, enabled);
                }
            }
        }

        public static void Unregister(WatchBase watch, bool updateState = true)
        {
            lock (k_Lock)
            {
                if (s_KnownWatches.TryRemove(watch.Identifier))
                {
                    if (updateState)
                    {
                        SetState(watch, false);
                    }
                }
#if UNITY_EDITOR
                else
                {
                    ManagedLog.Warning(LogCategory.GDX,
                        $"Unable to unregister '{watch.Identifier}'");
                }
#endif
            }
        }

        public static void Poll()
        {
            // TODO: We could make this part of the managed update and self ticking at a rate?
            lock (k_Lock)
            {
                int count = k_KnownActiveWatches.Count;
                for (int i = 0; i < count; i++)
                {
                    k_KnownActiveWatches[i].Poll();
                }
            }
        }

        public struct WatchList
        {
            public int Count;
            public bool[] IsActive;
            public string[] Identfiers;
            public string[] DisplayNames;

            public WatchList(int totalCount)
            {
                Count = totalCount;
                IsActive = new bool[totalCount];
                Identfiers = new string[totalCount];
                DisplayNames = new string[totalCount];
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}