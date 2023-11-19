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
        protected VisualElement m_ContainerElement;
        protected bool m_Enabled;

        protected WatchBase(bool enabled = true)
        {
            m_ContainerElement = new VisualElement();
            m_ContainerElement.AddToClassList("gdx-watch");

            SetState(enabled);

            WatchProvider.Register(this);
        }

        ~WatchBase()
        {
            WatchProvider.Unregister(this);
        }

        public void SetState(bool enabled)
        {
            m_ContainerElement.style.display = enabled ? DisplayStyle.Flex : DisplayStyle.None;
            m_Enabled = enabled;
        }

        public VisualElement GetElement()
        {
            return m_ContainerElement;
        }

        public abstract void Poll();
    }
#endif // UNITY_2022_2_OR_NEWER
}