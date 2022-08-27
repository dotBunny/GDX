﻿// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class ChangelogProvider
    {
        static VisualElement s_TargetContainer;

        static int s_AddedLineHash = "### Added".GetStableHashCode();
        static int s_ChangedLineHash = "### Changed".GetStableHashCode();
        static int s_FixedLineHash = "### Fixed".GetStableHashCode();
        static int s_RemovedLineHash = "### Removed".GetStableHashCode();

        static int s_ArrowHash = "->".GetStableHashCode();

        enum ChangelogSection
        {
            Default = 0,
            Added = 1,
            Changed = 2,
            Fixed = 3,
            Removed = 4
        }

        public static void StartProcess(VisualElement container)
        {
            container.Clear();
            ChangelogSection section = ChangelogSection.Default;

            string[] changelog = UpdateProvider.GetLocalChangelog(3);
            int lineCount = changelog.Length;

            Label title = new Label("Changelog");
            title.AddToClassList("gdx-changelog-title");
            container.Add(title);

            for (int i = 0; i < lineCount; i++)
            {
                string s = changelog[i];



                // Determine sections
                int lineHash = s.GetStableHashCode();
                if (lineHash == s_AddedLineHash)
                {
                    section = ChangelogSection.Added;
                    Label newHeader = new Label("Added");
                    newHeader.AddToClassList("gdx-changelog-h2");
                    container.Add(newHeader);
                    i++; // skip next line
                    continue;
                }
                else if (lineHash == s_ChangedLineHash)
                {
                    section = ChangelogSection.Changed;
                    Label newHeader = new Label("Changed");
                    newHeader.AddToClassList("gdx-changelog-h2");
                    container.Add(newHeader);
                    i++; // skip next line
                    continue;
                }
                else if (lineHash == s_FixedLineHash)
                {
                    section = ChangelogSection.Fixed;
                    Label newHeader = new Label("Fixed");
                    newHeader.AddToClassList("gdx-changelog-h2");
                    container.Add(newHeader);
                    i++; // skip next line
                    continue;
                }
                else if (lineHash == s_RemovedLineHash)
                {
                    section = ChangelogSection.Removed;
                    Label newHeader = new Label("Removed");
                    newHeader.AddToClassList("gdx-changelog-h2");
                    container.Add(newHeader);
                    i++; // skip next line
                    continue;
                }


                if (s.StartsWith("## "))
                {
                    Label versionText = new Label(s.Replace("## ", string.Empty));
                    versionText.AddToClassList("gdx-changelog-h1");
                    container.Add(versionText);
                    i++; // skip next line
                    continue;
                }
                else if (s.StartsWith("***"))
                {
                    Label versionText = new Label(s.Replace("## ", string.Empty)) { text = s
                        .Replace("***\"", string.Empty)
                        .Replace("\"***", string.Empty).Trim()
                    };
                    versionText.AddToClassList("gdx-changelog-h3");
                    container.Add(versionText);
                    continue;
                }
                else if (s.StartsWith("> "))
                {
                    Label versionText = new Label(s.Replace("> ", string.Empty));
                    versionText.AddToClassList("gdx-changelog-quote");
                    container.Add(versionText);
                    continue;
                }
                else if (s.StartsWith("- "))
                {
                    container.Add(GenerateChangelogLineItem(s.Replace("- ", string.Empty), section));
                    continue;
                }

                container.Add(new Label(s));
            }

        }

        static VisualElement GenerateChangelogLineItem(string item, ChangelogSection section = ChangelogSection.Default)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList("gdx-changelog-item");

            VisualElement dotSurround = new VisualElement();
            dotSurround.AddToClassList("dot-container");

            VisualElement dot = new VisualElement();
            dotSurround.Add(dot);

            dot.AddToClassList("dot");
            switch (section)
            {
                case ChangelogSection.Default:
                    break;
                case ChangelogSection.Added:
                    dot.AddToClassList("dot-added");
                    break;
                case ChangelogSection.Changed:
                    dot.AddToClassList("dot-changed");
                    break;
                case ChangelogSection.Fixed:
                    dot.AddToClassList("dot-fixed");
                    break;
                case ChangelogSection.Removed:
                    dot.AddToClassList("dot-removed");
                    break;
            }
            container.Add(dotSurround);

            VisualElement textContainer  = new VisualElement();
            textContainer.AddToClassList("text-container");
            int length = item.Length;

            StringBuilder newLine = new StringBuilder();
            bool insideCodeIndicator = false;
            int insideDontBreak = 0;
            for (int i = 0; i < length; i++)
            {
                if (item[i] == '[')
                {
                    insideDontBreak++;
                }

                if (item[i] == ']')
                {
                    insideDontBreak--;
                }
                if (item[i] == '`')
                {
                    if (!insideCodeIndicator)
                    {
                        if (newLine.Length > 0)
                        {
                            textContainer.Add(ParseRegularText(newLine.ToString().Trim()));
                            newLine.Clear();
                        }
                    }
                    else
                    {
                        Label codeText = new Label(newLine.ToString().Trim());
                        codeText.AddToClassList("gdx-code");
                        textContainer.Add(codeText);
                        newLine.Clear();
                    }
                    insideCodeIndicator = !insideCodeIndicator;
                }
                else
                {
                    newLine.Append(item[i]);
                    if (item[i] == ' ' && newLine.Length > 0 && insideDontBreak == 0)
                    {
                        textContainer.Add(ParseRegularText(newLine.ToString().Trim()));
                        newLine.Clear();
                    }
                }

            }

            if (newLine.Length > 0)
            {
                textContainer.Add(ParseRegularText(newLine.ToString().Trim()));
                newLine.Clear();
            }

            container.Add(textContainer);
            return container;
        }

        static VisualElement ParseRegularText(string text)
        {
            int hash = text.GetStableHashCode();

            // Right quick arrow
            if (hash == s_ArrowHash)
            {
                VisualElement arrow = new VisualElement();
                arrow.AddToClassList("gdx-right-arrow");
                return arrow;
            }

            // Link
            if (text.StartsWith("[") && text.EndsWith(")") && text.Contains("]("))
            {
                string display = text.Substring(1, text.IndexOf("]", StringComparison.Ordinal) - 1);
                string link = text.Substring(text.IndexOf("](") + 2).TrimEnd(')');
                Button linkedButton = new Button();
                linkedButton.text = display;
                linkedButton.clicked += () =>
                {
                    UnityEngine.Application.OpenURL(link);
                };
                linkedButton.AddToClassList("gdx-link");
                return linkedButton;
            }

            // text
            return new Label(text);
        }

        static void ProcessChangelog()
        {

        }
    }
}