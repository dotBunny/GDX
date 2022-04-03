// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using GDX.Collections.Generic;
using UnityEngine.UIElements;

namespace GDX.Editor.ProjectSettings
{
    public static class SearchProvider
    {
        static readonly string[] k_Exclusions = new string[]
        {
            "the", "is", "a"
        };

        static SimpleList<string> s_Keywords = new SimpleList<string>(100);

        static StringKeyDictionary<SimpleList<VisualElement>> s_KeywordMap =
            new StringKeyDictionary<SimpleList<VisualElement>>(100);

        static readonly Dictionary<IConfigSection, SimpleList<VisualElement>> k_SectionElementMap =
            new Dictionary<IConfigSection, SimpleList<VisualElement>>(ProjectSettingsProvider.SectionCount);

        public static string[] GetKeywords()
        {
            return s_Keywords.Array;
        }
        public static void Reset()
        {
            s_Keywords.Clear();
            s_KeywordMap.Clear();
            k_SectionElementMap.Clear();
        }
        public static VisualElement[] Query(string searchContext)
        {
            SimpleList<VisualElement> results = new SimpleList<VisualElement>(10);
            int count = s_Keywords.Count;
            for (int i = 0; i < count; i++)
            {
                string keyword = s_Keywords.Array[i];
                if (!keyword.Contains(searchContext))
                {
                    continue;
                }

                int elementCount = s_KeywordMap[keyword].Count;
                for (int j = 0; j < elementCount; j++)
                {
                    results.AddWithExpandCheck(s_KeywordMap[keyword].Array[i]);
                }
            }
            return results.Array;
        }
        public static void RegisterElement<T>(IConfigSection parent, VisualElement element, string[] additionalKeywords = null)
        {
            // Create our working list
            SimpleList<string> validWords = new SimpleList<string>(25);

            // Parse element
            if (element is BaseField<T> field)
            {
                string[] labelWords = field.labelElement.text.Split(' ');
                int labelWordsCount = labelWords.Length;
                for (int i = 0; i < labelWordsCount; i++)
                {
                    string word = labelWords[i];
                    if (word.Length > 2 && k_Exclusions.ContainsValue(word))
                    {
                        validWords.AddWithExpandCheckUniqueItem(word);
                    }
                }

                string[] tooltipWords = field.labelElement.tooltip.Split(' ');
                int tooltipWordsCount = tooltipWords.Length;
                for (int i = 0; i < tooltipWordsCount; i++)
                {
                    string word = tooltipWords[i];
                    if (word.Length > 2 && k_Exclusions.ContainsValue(word))
                    {
                        validWords.AddWithExpandCheckUniqueItem(word);
                    }
                }
            }

            // Passed in words
            if (additionalKeywords != null)
            {
                int additionalKeywordsCount = additionalKeywords.Length;
                for (int i = 0; i < additionalKeywordsCount; i++)
                {
                    string word = additionalKeywords[i];
                    if (word.Length > 2 && k_Exclusions.ContainsValue(word))
                    {
                        validWords.AddWithExpandCheckUniqueItem(word);
                    }
                }
            }

            // Build Map
            int validWordsCount = validWords.Count;
            for (int i = 0; i < validWordsCount; i++)
            {
                string word = validWords.Array[i];

                if (!s_Keywords.ContainsItem(word))
                {
                    s_Keywords.AddWithExpandCheck(word);
                    s_KeywordMap.AddWithExpandCheck(word, new SimpleList<VisualElement>(20));
                }
                s_KeywordMap[word].AddWithExpandCheck(element);
            }

            // Register element to a section
            if (k_SectionElementMap.ContainsKey(parent))
            {
                k_SectionElementMap[parent].AddWithExpandCheck(element);
            }
            else
            {
                k_SectionElementMap.Add(parent, new SimpleList<VisualElement>(20));
            }
        }
    }
}