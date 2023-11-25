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
    public class FloatChartWatch : WatchBase
    {
        CircularBuffer<float> m_Values;
        CircularBuffer<float> m_ChartPercentage;

        int m_ValuesCount;
        float m_MinimumValue = 0f;
        float m_MaximumValue = 1f;
        float m_Range;

        bool m_ShowLatestValue;
        readonly Func<float> m_GetValue;
        readonly Func<float, Sentiment> m_GetSentiment;

        readonly VisualElement m_ChartElement;

        public FloatChartWatch(string uniqueIdentifier, string displayName, Func<float> getValue, Func<float, Sentiment> getSentiment, float minimumValue = 0, float maximumValue = 1, int historicalCount = 250, bool showLatestValue = true, bool enabled = true) : base(uniqueIdentifier, displayName, enabled)
        {
            m_GetValue = getValue;
            m_GetSentiment = getSentiment;

            m_ValuesCount = historicalCount;
            m_Values = new CircularBuffer<float>(historicalCount);
            m_ChartPercentage = new CircularBuffer<float>(historicalCount);
            m_MinimumValue = minimumValue;
            m_MaximumValue = maximumValue;
            m_Range = m_MaximumValue - m_MinimumValue;
            m_ShowLatestValue = showLatestValue;

            Label displayNameLabel = new Label() { text = displayName };
            displayNameLabel.AddToClassList("gdx-watch-left");

            m_ChartElement = new VisualElement();
            m_ChartElement.AddToClassList("gdx-watch-right");

            ContainerElement.Add(displayNameLabel);
            ContainerElement.Add(m_ChartElement);

            m_ChartElement.generateVisualContent = GenerateVisualContent;
        }

        void GenerateVisualContent(MeshGenerationContext ctx)
        {
            Painter2D paint2D = ctx.painter2D;

            // TODO: Sentiment? Use latest
            paint2D.strokeColor = UnityEngine.Color.white;

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
            m_Values.Add(getValue);

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
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER