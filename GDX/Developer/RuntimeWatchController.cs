﻿// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER
using System.Net.Mail;
using GDX.Developer.ConsoleVariables;
using GDX.RuntimeContent;
using UnityEngine;
using UnityEngine.UIElements;
#endif

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    public class RuntimeWatchController
    {
        const string k_GameObjectName = "GDX_Watches";

        public UIDocument Document { get; private set; }
        public GameObject WatchesGameObject { get; private set; }

        int m_FontSize = 14;
        ushort m_LastVersion;
        readonly VisualElement m_RootElement;
        bool m_ShouldShow = false;

        static readonly BooleanConsoleVariable k_WatchesEnabled =
            new BooleanConsoleVariable("watches.enabled", "Watches Enabled", true,
                ConsoleVariableBase.ConsoleVariableFlags.Setting);

        public RuntimeWatchController(GameObject parentGameObject, int initialFontSize, int position, bool visible)
        {
            // UIDocuments do not allow multiple components per Game Object so we have to make a child object.
            WatchesGameObject = new GameObject(k_GameObjectName);
            WatchesGameObject.transform.SetParent(parentGameObject.transform, false);

            // Create isolated UI document  (thanks Damian, boy do I feel stupid.)
            Document = WatchesGameObject.AddComponent<UIDocument>();
            Document.sortingOrder = float.MaxValue; // Above all
            Document.visualTreeAsset = ResourceProvider.GetUIElements().Watches;

            m_RootElement = Document.rootVisualElement.Q<VisualElement>("gdx-watches");

            k_WatchesEnabled.OnValueChanged += UpdateShow;

            UpdateFontSize(initialFontSize);
            UpdatePosition(position);
            UpdateShow(visible);
        }

        public void UpdateFontSize(int fontSize)
        {
            m_FontSize = fontSize;
        }

        public void UpdatePosition(int position)
        {
            m_RootElement.ApplyAlignment((VisualElementStyles.Alignment)position);
        }

        public void UpdateShow(bool show)
        {
            WatchProvider.SetGlobalState(show);
            if (show)
            {
                m_RootElement.Show();
                m_ShouldShow = true;
            }
            else
            {
                m_RootElement.Hide();
                m_ShouldShow = false;
            }
        }

        public void Tick()
        {
            m_ShouldShow = WatchProvider.HasActiveWatches();
            if (m_ShouldShow && !m_RootElement.IsVisible())
            {
                m_RootElement.Show();
            }
            else if(!m_ShouldShow && m_RootElement.IsVisible())
            {
                m_RootElement.Hide();
            }

            if (m_ShouldShow)
            {
                WatchProvider.Poll();

                if (WatchProvider.Version != m_LastVersion)
                {
                    // We need to check that all of our elements are accounted for
                    VisualElement[] elements = WatchProvider.GetActiveElements();
                    int elementCount = elements.Length;

                    // Clear from root
                    m_RootElement.Clear();
                    for (int i = 0; i < elementCount; i++)
                    {
                        m_RootElement.Add(elements[i]);
                    }

                    m_LastVersion = WatchProvider.Version;
                }
            }
            else
            {

            }

        }
    }
#endif // UNITY_2022_2_OR_NEWER
}