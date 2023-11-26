// Copyright (c) 2020-2023 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace GDX.Developer
{
#if UNITY_2022_2_OR_NEWER
    /// <summary>
    /// Base class for static watchers.
    /// </summary>
    [Preserve]
    public abstract class WatchBase
    {
        public enum Sentiment
        {
            Default,
            Good,
            Warning,
            Bad
        }

        public string Identifier { get; private set; }
        public string BaseIdentifier { get; private set; }

        public readonly string DisplayName;

        public readonly VisualElement ContainerElement;

        protected WatchBase(string uniqueIdentifier, string displayName, bool enabled = true, int minWidth =-1, int minHeight = -1)
        {
            BaseIdentifier = uniqueIdentifier;
            Identifier = BaseIdentifier;
            DisplayName = displayName;

            ContainerElement = new VisualElement();
            ContainerElement.AddToClassList("gdx-watch");

            if (minWidth != -1)
            {
                ContainerElement.style.minWidth = new StyleLength(new Length(minWidth, LengthUnit.Pixel));
            }

            if (minHeight != -1)
            {
                ContainerElement.style.minHeight = new StyleLength(new Length(minHeight, LengthUnit.Pixel));
            }

            WatchProvider.Register(this,
                WatchSettings.TryGetValue(uniqueIdentifier, out bool savedValue) ? savedValue : enabled);
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

        public static Sentiment SentimentRange(int min, int max, int value)
        {
            if (value < min)
            {
                return Sentiment.Bad;
            }
            return value > max ? Sentiment.Good : Sentiment.Warning;
        }
        public static Sentiment SentimentRange(float min, float max, float value)
        {
            if (value < min)
            {
                return Sentiment.Bad;
            }
            return value > max ? Sentiment.Good : Sentiment.Warning;
        }
        public static Sentiment SentimentRangeReversed(int min, int max, int value)
        {
            if (value < min)
            {
                return Sentiment.Good;
            }
            return value > max ? Sentiment.Bad : Sentiment.Warning;
        }
        public static Sentiment SentimentRangeReversed(float min, float max, float value)
        {
            if (value < min)
            {
                return Sentiment.Good;
            }
            return value > max ? Sentiment.Bad : Sentiment.Warning;
        }

        public static void AddSentimentToElement(VisualElement element, Sentiment sentiment)
        {
            switch (sentiment)
            {
                case Sentiment.Good:
                    element.RemoveFromClassList("warning");
                    element.RemoveFromClassList("bad");
                    element.RemoveFromClassList("default");
                    element.AddToClassList("good");
                    break;
                case Sentiment.Warning:
                    element.RemoveFromClassList("good");
                    element.RemoveFromClassList("bad");
                    element.RemoveFromClassList("default");
                    element.AddToClassList("warning");
                    break;
                case Sentiment.Bad:
                    element.RemoveFromClassList("good");
                    element.RemoveFromClassList("warning");
                    element.RemoveFromClassList("default");
                    element.AddToClassList("bad");
                    break;
                default:
                    element.RemoveFromClassList("good");
                    element.RemoveFromClassList("warning");
                    element.RemoveFromClassList("bad");
                    element.AddToClassList("default");
                    break;
            }
        }
    }
#endif // UNITY_2022_2_OR_NEWER
}