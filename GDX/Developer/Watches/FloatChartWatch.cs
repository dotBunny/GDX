// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using GDX.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class FloatChartWatch : WatchBase
    {
        CircularBuffer<float> m_ChartPercentage;

        int m_ValuesCount;
        float m_MinimumValue = 0f;
        float m_MaximumValue = 1f;
        float m_Range;
        float m_CachedValue = float.MinValue;
        Sentiment m_CachedSentiment = Sentiment.Default;

        bool m_ShowLatestValue;
        string m_LatestValueFormat = null;
        readonly Func<float> m_GetValue;
        readonly Func<float, Sentiment> m_GetSentiment;

        readonly VisualElement m_ChartElement;
        readonly Label m_ValueLabel;

        public FloatChartWatch(string uniqueIdentifier, string displayName, Func<float> getValue, Func<float, Sentiment> getSentiment,
            float minimumValue = 0, float maximumValue = 1, int historicalCount = 250, bool showLatestValue = true, string latestValueFormat = "F2", bool enabled = true, int minWidth =-1, int minHeight = -1)
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
            m_LatestValueFormat = latestValueFormat;

            Label displayNameLabel = new Label() { text = displayName };
            displayNameLabel.AddToClassList("gdx-watch-left");

            m_ChartElement = new VisualElement();
            m_ChartElement.AddToClassList("gdx-watch-right");
            m_ChartElement.AddToClassList("inner-box");

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
            paint2D.lineJoin = LineJoin.Round;


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
            float getValue = m_GetValue();
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
            if (m_ShowLatestValue && math.lengthsq(getValue - m_CachedValue) != Platform.FloatTolerance)
            {
                m_ValueLabel.text = getValue.ToString(m_LatestValueFormat);
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