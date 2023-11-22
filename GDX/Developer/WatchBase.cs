// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    /// <summary>
    /// Thread-safe developer watch system.
    /// </summary>
    public abstract class WatchBase
    {

        public string Identifier { get; private set; }
        public string BaseIdentifier { get; private set; }

        public readonly string DisplayName;

        public readonly VisualElement ContainerElement;

        protected WatchBase(string uniqueIdentifier, string displayName, bool enabled = true)
        {
            BaseIdentifier = uniqueIdentifier;
            Identifier = BaseIdentifier;
            DisplayName = displayName;

            ContainerElement = new VisualElement();
            ContainerElement.AddToClassList("gdx-watch");

            WatchProvider.Register(this, enabled);
        }

        ~WatchBase()
        {
            // We have to ensure when we become out of scope that we no longer registered
            WatchProvider.Unregister(this);
        }

        public VisualElement GetElement()
        {
            return ContainerElement;
        }

        public void SetOverrideIdentifier(uint index)
        {
            Identifier = $"{BaseIdentifier}.{index.ToString()}";
        }

        public abstract void Poll();
    }
#endif // UNITY_2022_2_OR_NEWER
}