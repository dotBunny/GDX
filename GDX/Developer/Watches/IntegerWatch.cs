// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class IntegerWatch : WatchBase
    {
        public delegate int GetValueCallback();

        readonly Func<int> m_GetValue;
        readonly Func<int, Sentiment> m_GetSentiment;

        Sentiment m_CachedSentiment;
        int m_CachedValue = Int32.MinValue;

        readonly Label m_DisplayNameLabel;
        readonly Label m_ValueLabel;


        public IntegerWatch(string uniqueIdentifier, string displayName, Func<int> getValue, Func<int, Sentiment> getSentiment, bool enabled = true) :
            base(uniqueIdentifier, displayName, enabled)
        {


            m_GetValue = getValue;
            m_GetSentiment = getSentiment;

            m_DisplayNameLabel = new Label() { text = displayName };
            m_DisplayNameLabel.AddToClassList("gdx-watch-left");

            m_ValueLabel = new Label();
            m_ValueLabel.AddToClassList("gdx-watch-right");
            m_ValueLabel.AddToClassList("default");

            ContainerElement.Add(m_DisplayNameLabel);
            ContainerElement.Add(m_ValueLabel);
        }

        public override void Poll()
        {
            // Poll for our new value
            int getValue = m_GetValue();
            if (getValue != m_CachedValue)
            {
                m_ValueLabel.text = getValue.ToString();
                Sentiment sentiment = m_GetSentiment(getValue);
                if (sentiment != m_CachedSentiment)
                {
                    // Add colors
                    switch (sentiment)
                    {
                        case Sentiment.Good:
                            m_ValueLabel.RemoveFromClassList("warning");
                            m_ValueLabel.RemoveFromClassList("bad");
                            m_ValueLabel.RemoveFromClassList("default");
                            m_ValueLabel.AddToClassList("good");
                            break;
                        case Sentiment.Warning:
                            m_ValueLabel.RemoveFromClassList("good");
                            m_ValueLabel.RemoveFromClassList("bad");
                            m_ValueLabel.RemoveFromClassList("default");
                            m_ValueLabel.AddToClassList("warning");
                            break;
                        case Sentiment.Bad:
                            m_ValueLabel.RemoveFromClassList("good");
                            m_ValueLabel.RemoveFromClassList("warning");
                            m_ValueLabel.RemoveFromClassList("default");
                            m_ValueLabel.AddToClassList("bad");
                            break;
                        default:
                            m_ValueLabel.RemoveFromClassList("good");
                            m_ValueLabel.RemoveFromClassList("warning");
                            m_ValueLabel.RemoveFromClassList("bad");
                            m_ValueLabel.AddToClassList("default");
                            break;
                    }
                    m_CachedSentiment = sentiment;
                }
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER