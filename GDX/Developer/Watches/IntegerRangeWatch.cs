// Copyright (c) 2020-2024 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class IntegerRangeWatch : WatchBase
    {
        readonly Func<int> m_GetValue;
        readonly Func<int, Sentiment> m_GetSentiment;

        Sentiment m_CachedSentiment;
        int m_CachedValue = int.MinValue;

        readonly Label m_ValueLabel;

        public IntegerRangeWatch(string uniqueIdentifier, string displayName, Func<int> getValue, Func<int, Sentiment> getSentiment, bool enabled = true, int minWidth =-1, int minHeight = -1)
            : base(uniqueIdentifier, displayName, enabled, minWidth, minHeight)
        {
            m_GetValue = getValue;
            m_GetSentiment = getSentiment;

            Label displayNameLabel = new Label() { text = displayName };
            displayNameLabel.AddToClassList("gdx-watch-left");

            m_ValueLabel = new Label();
            m_ValueLabel.AddToClassList("gdx-watch-right");
            m_ValueLabel.AddToClassList("default");

            ContainerElement.Add(displayNameLabel);
            ContainerElement.Add(m_ValueLabel);
        }

        public override void Poll()
        {
            int getValue = m_GetValue();
            if (getValue != m_CachedValue)
            {
                m_ValueLabel.text = getValue.ToString();
                Sentiment sentiment = m_GetSentiment(getValue);
                if (sentiment != m_CachedSentiment)
                {
                    AddSentimentToElement(m_ValueLabel, sentiment);
                    m_CachedSentiment = sentiment;
                    m_CachedValue = getValue;
                }
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER