// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

#if UNITY_2022_2_OR_NEWER

using System;
using GDX.Collections.Generic;
using UnityEngine.UIElements;

namespace GDX.Developer
{
    public class FloatChartWatch : WatchBase
    {
        CircularBuffer<float> m_Values;

        int m_ValuesCount;
        readonly Func<float> m_GetValue;
        readonly Func<float, Sentiment> m_GetSentiment;

        readonly VisualElement m_ChartElement;

        public FloatChartWatch(string uniqueIdentifier, string displayName, Func<float> getValue, Func<float, Sentiment> getSentiment, int historicalCount = 250, bool showLatestValue = true, bool enabled = true) : base(uniqueIdentifier, displayName, enabled)
        {
            m_GetValue = getValue;
            m_GetSentiment = getSentiment;

            m_ValuesCount = historicalCount;
            m_Values = new CircularBuffer<float>(historicalCount);

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
            // var paint2D = ctx.painter2D;
            //
            // paint2D.fillColor = UnityEngine.Color.white;
            // paint2D.BeginPath();
            // paint2D.MoveTo(p0);
            // paint2D.LineTo(p1);
            // paint2D.LineTo(p2);
            // paint2D.LineTo(p3);
            // paint2D.ClosePath();
            // paint2D.Fill();
            //
            // paint2D.
        }

        public override void Poll()
        {
            float getValue = m_GetValue();
            m_Values.Add(getValue);
        }
    }
}

#endif // UNITY_2022_2_OR_NEWER