// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class FloatRangeWatch : WatchBase
    {
        readonly Func<float> m_GetValue;
        readonly Func<float, Sentiment> m_GetSentiment;

        Sentiment m_CachedSentiment;
        float m_CachedValue = float.MinValue;

        readonly Label m_ValueLabel;

        public FloatRangeWatch(string uniqueIdentifier, string displayName, Func<float> getValue, Func<float, Sentiment> getSentiment, bool enabled = true) :
            base(uniqueIdentifier, displayName, enabled)
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
            float getValue = m_GetValue();
            if (math.lengthsq(getValue - m_CachedValue) != Platform.FloatTolerance)
            {
                m_ValueLabel.text = getValue.ToString();
                Sentiment sentiment = m_GetSentiment(getValue);
                if (sentiment != m_CachedSentiment)
                {
                    // Add colors
                    AddSentimentToElement(m_ValueLabel, sentiment);
                    m_CachedSentiment = sentiment;
                    m_CachedValue = getValue;
                }
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER