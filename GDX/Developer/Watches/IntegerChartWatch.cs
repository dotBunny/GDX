// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using GDX.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class IntegerChartWatch : WatchBase
    {
        CircularBuffer<float> m_ChartPercentage;

        int m_ValuesCount;
        int m_MinimumValue = 0;
        int m_MaximumValue = 100;
        int m_Range;
        int m_CachedValue = int.MinValue;
        Sentiment m_CachedSentiment = Sentiment.Default;

        bool m_ShowLatestValue;
        readonly Func<int> m_GetValue;
        readonly Func<int, Sentiment> m_GetSentiment;

        readonly VisualElement m_ChartElement;
        readonly Label m_ValueLabel;

        public IntegerChartWatch(string uniqueIdentifier, string displayName, Func<int> getValue, Func<int, Sentiment> getSentiment,
            int minimumValue = 0, int maximumValue = 100, int historicalCount = 250, bool showLatestValue = true, bool enabled = true, int minWidth =-1, int minHeight = -1)
            : base(uniqueIdentifier, displayName, enabled, minWidth, minHeight)
        {
            m_GetValue = getValue;
            m_GetSentiment = getSentiment;

            m_ValuesCount = historicalCount;
            m_ChartPercentage = new CircularBuffer<float>(historicalCount);

            m_MinimumValue = minimumValue;
            m_MaximumValue = maximumValue;
            m_Range = m_MaximumValue - m_MinimumValue;

            m_ShowLatestValue = showLatestValue;

            Label displayNameLabel = new Label() { text = displayName };
            displayNameLabel.AddToClassList("gdx-watch-left");

            m_ChartElement = new VisualElement();
            m_ChartElement.AddToClassList("gdx-watch-right");

            if (showLatestValue)
            {
                m_ValueLabel = new Label();
                m_ValueLabel.AddToClassList("default");
                m_ChartElement.Add(m_ValueLabel);
            }

            ContainerElement.Add(displayNameLabel);
            ContainerElement.Add(m_ChartElement);

            m_ChartElement.generateVisualContent = GenerateVisualContent;
        }

        void GenerateVisualContent(MeshGenerationContext ctx)
        {
            Painter2D paint2D = ctx.painter2D;
            paint2D.strokeColor = m_CachedSentiment switch
            {
                Sentiment.Good => Color.green,
                Sentiment.Warning => Color.yellow,
                Sentiment.Bad => Color.red,
                _ => Color.white
            };

            float horizontalIncrement  = m_ChartElement.resolvedStyle.width / m_ValuesCount;
            float verticalIncrement = m_ChartElement.resolvedStyle.height;

            paint2D.BeginPath();
            for (int i = 0; i < m_ValuesCount; i++)
            {
                paint2D.LineTo(new Vector2(horizontalIncrement * i,
                    m_ChartPercentage[i] * verticalIncrement));
            }
            paint2D.Stroke();
        }

        public override void Poll()
        {
            int getValue = m_GetValue();
            if (m_GetSentiment != null)
            {
                m_CachedSentiment = m_GetSentiment(getValue);
            }

            if (getValue <= m_MinimumValue)
            {
                m_ChartPercentage.Add(1f);
            }
            else if (getValue >= m_MaximumValue)
            {
                m_ChartPercentage.Add(0f);
            }
            else
            {
                m_ChartPercentage.Add((1f - (getValue - m_MinimumValue) / m_Range));
            }

            m_ChartElement.MarkDirtyRepaint();

            // Latest Value Text
            if (m_ShowLatestValue && getValue != m_CachedValue)
            {
                m_ValueLabel.text = getValue.ToString();
                if (m_GetSentiment != null)
                {
                    Sentiment sentiment = m_GetSentiment(getValue);
                    if (sentiment != m_CachedSentiment)
                    {
                        // Add colors
                        AddSentimentToElement(m_ValueLabel, sentiment);
                        m_CachedValue = getValue;
                    }
                }
            }
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER