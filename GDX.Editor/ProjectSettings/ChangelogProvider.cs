// Copyright (c) 2020-2022 dotBunny Inc.
// dotBunny licenses this file to you under the BSL-1.0 license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using GDX.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace GDX.Editor
{
    public static class ChangelogProvider
    {
        public enum ChangelogStyle
        {
            Default,
            Header1,
            Header2,
            Header3,
            Header4,
            Bold,
            BoldItalic,
            Italic,
            RightArrow,
            Link,
            DotAdded,
            DotChanged,
            DotRemoved,
            DotFixed,
            Item,
            ItemText,
            Code
        }
        public struct Artifact
        {
            public ChangelogStyle Style;
            public string Content;
            public string Extra;
            public List<Artifact> SubArtifacts;

            public Artifact(string content)
            {
                Style = ChangelogStyle.Default;
                Content = content;
                Extra = null;
                SubArtifacts = new List<Artifact>();
            }
            public Artifact(string content, ChangelogStyle style)
            {
                Style = style;
                Content = content;
                Extra = null;
                SubArtifacts = new List<Artifact>();
            }
            public Artifact(string content, string extra, ChangelogStyle style)
            {
                Style = style;
                Content = content;
                Extra = extra;
                SubArtifacts = new List<Artifact>();
            }
        }

        public static VisualElement CreateVisualElement(this Artifact artifact)
        {
            switch (artifact.Style)
            {
                case ChangelogStyle.Header1:
                    Label h1 = new Label(artifact.Content);
                    h1.AddToClassList("gdx-md-h1");
                    return h1;
                case ChangelogStyle.Header2:
                    Label h2 = new Label(artifact.Content);
                    h2.AddToClassList("gdx-md-h2");
                    return h2;
                case ChangelogStyle.Header3:
                    Label h3 = new Label(artifact.Content);
                    h3.AddToClassList("gdx-md-h3");
                    return h3;
                case ChangelogStyle.Header4:
                    Label h4 = new Label(artifact.Content);
                    h4.AddToClassList("gdx-md-h4");
                    return h4;
                case ChangelogStyle.Bold:
                    Label bold = new Label(artifact.Content);
                    bold.AddToClassList("gdx-md-bold");
                    return bold;
                case ChangelogStyle.Italic:
                    Label italic = new Label(artifact.Content);
                    italic.AddToClassList("gdx-md-italic");
                    return italic;
                case ChangelogStyle.Link:
                    Button linkedButton = new Button();
                    linkedButton.text = artifact.Content;
                    linkedButton.clicked += () => { Application.OpenURL(artifact.Extra); };
                    linkedButton.AddToClassList("gdx-link");
                    return linkedButton;
                case ChangelogStyle.RightArrow:
                    VisualElement arrow = new VisualElement();
                    arrow.AddToClassList("gdx-right-arrow");
                    return arrow;
                case ChangelogStyle.DotAdded:
                    VisualElement dotAddedSurround = new VisualElement();
                    dotAddedSurround.AddToClassList("dot-container");
                    VisualElement dotAdded = new VisualElement();
                    dotAddedSurround.Add(dotAdded);
                    dotAdded.AddToClassList("dot");
                    dotAdded.AddToClassList("dot-added");
                    return dotAddedSurround;
                case ChangelogStyle.DotRemoved:
                    VisualElement dotRemovedSurround = new VisualElement();
                    dotRemovedSurround.AddToClassList("dot-container");
                    VisualElement dotRemoved = new VisualElement();
                    dotRemovedSurround.Add(dotRemoved);
                    dotRemoved.AddToClassList("dot");
                    dotRemoved.AddToClassList("dot-removed");
                    return dotRemovedSurround;
                case ChangelogStyle.DotChanged:
                    VisualElement dotChangedSurround = new VisualElement();
                    dotChangedSurround.AddToClassList("dot-container");
                    VisualElement dotChanged = new VisualElement();
                    dotChangedSurround.Add(dotChanged);
                    dotChanged.AddToClassList("dot");
                    dotChanged.AddToClassList("dot-changed");
                    return dotChangedSurround;
                case ChangelogStyle.DotFixed:
                    VisualElement dotModifiedSurround = new VisualElement();
                    dotModifiedSurround.AddToClassList("dot-container");
                    VisualElement dotModified = new VisualElement();
                    dotModifiedSurround.Add(dotModified);
                    dotModified.AddToClassList("dot");
                    dotModified.AddToClassList("dot-changed");
                    return dotModifiedSurround;
                case ChangelogStyle.Item:
                    VisualElement item = new VisualElement();
                    item.AddToClassList("gdx-changelog-item");

                    int itemChildCount = artifact.SubArtifacts.Count;
                    for (int i = 0; i < itemChildCount; i++)
                    {
                        item.Add(artifact.SubArtifacts[i].CreateVisualElement());
                    }
                    return item;
                case ChangelogStyle.ItemText:
                    VisualElement itemContainer = new VisualElement();
                    itemContainer.AddToClassList("text-container");
                    int itemTextCount = artifact.SubArtifacts.Count;
                    for (int i = 0; i < itemTextCount; i++)
                    {
                        itemContainer.Add(artifact.SubArtifacts[i].CreateVisualElement());
                    }
                    return itemContainer;
                case ChangelogStyle.Code:
                    Label code = new Label(artifact.Content);
                    code.AddToClassList("gdx-code");
                    return code;
            }
            return new Label(artifact.Content);
        }

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

        public static void StartTask(VisualElement container)
        {
            // TODO what if edit mode is off?
            new ParseChangelogTask(container, FinishedLoading).Enqueue();
            container.Clear();
        }

        static void FinishedLoading(TaskBase task)
        {
            ParseChangelogTask changelog = (ParseChangelogTask)task;
            changelog.Target.Clear();

            // Have to do the UIToolkit portion on the MT
            int count = changelog.Artifacts.Count;
            for (int i = 0; i < count; i++)
            {
                changelog.Target.Add(changelog.Artifacts[i].CreateVisualElement());
            }
        }


        static Artifact GenerateChangelogLineItem(string item, ChangelogSection section = ChangelogSection.Default)
        {
            Artifact artifact = new Artifact(null, ChangelogStyle.Item);
            switch (section)
            {
                case ChangelogSection.Added:
                    artifact.SubArtifacts.Add(new Artifact(null, ChangelogStyle.DotAdded));
                    break;
                case ChangelogSection.Changed:
                    artifact.SubArtifacts.Add(new Artifact(null, ChangelogStyle.DotChanged));
                    break;
                case ChangelogSection.Fixed:
                    artifact.SubArtifacts.Add(new Artifact(null, ChangelogStyle.DotFixed));
                    break;
                case ChangelogSection.Removed:
                    artifact.SubArtifacts.Add(new Artifact(null, ChangelogStyle.DotRemoved));
                    break;
            }

            Artifact textContainer = new Artifact(null, ChangelogStyle.ItemText);
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
                            textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                            newLine.Clear();
                        }
                    }
                    else
                    {
                        textContainer.SubArtifacts.Add(new Artifact(newLine.ToString().Trim(), ChangelogStyle.Code ));
                        newLine.Clear();
                    }
                    insideCodeIndicator = !insideCodeIndicator;
                }
                else
                {
                    newLine.Append(item[i]);
                    if (item[i] == ' ' && newLine.Length > 0 && insideDontBreak == 0)
                    {
                        textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                        newLine.Clear();
                    }
                }
            }

            if (newLine.Length > 0)
            {
                textContainer.SubArtifacts.Add(ParseRegularText(newLine.ToString().Trim()));
                newLine.Clear();
            }
            artifact.SubArtifacts.Add(textContainer);
            return artifact;
        }
        static Artifact ParseRegularText(string text)
        {
            int hash = text.GetStableHashCode();

            // Right quick arrow
            if (hash == s_ArrowHash)
            {
                return new Artifact(null, ChangelogStyle.RightArrow);
            }

            // Link
            if (text.StartsWith("[") && text.EndsWith(")") && text.Contains("]("))
            {
                return new Artifact(text.Substring(1, text.IndexOf("]", StringComparison.Ordinal) - 1),
                    text.Substring(text.IndexOf("](", StringComparison.Ordinal) + 2).TrimEnd(')'), ChangelogStyle.Link);
            }

            // text
            return new Artifact(text);
        }

        class ParseChangelogTask : TaskBase
        {
            public VisualElement Target;
            public List<Artifact> Artifacts = new List<Artifact>();
            public ParseChangelogTask(VisualElement target, Action<TaskBase> onCompletedMainThread)
            {
                Target = target;
                m_Name = "GDX Changelog";
                m_IsLogging = false;
                completedMainThread = onCompletedMainThread;
            }
            /// <inheritdoc />
            public override void DoWork()
            {
                ChangelogSection section = ChangelogSection.Default;

                string[] changelog = UpdateProvider.GetLocalChangelog(3);
                int lineCount = changelog.Length;

                Artifacts.Add(new Artifact("Changelog", ChangelogStyle.Header1));
                for (int i = 0; i < lineCount; i++)
                {
                    string s = changelog[i];

                    // Determine sections
                    int lineHash = s.GetStableHashCode();
                    if (lineHash == s_AddedLineHash)
                    {
                        section = ChangelogSection.Added;
                        Artifacts.Add(new Artifact("Added", ChangelogStyle.Header2));
                        i++; // skip next line
                        continue;
                    }
                    if (lineHash == s_ChangedLineHash)
                    {
                        section = ChangelogSection.Changed;
                        Artifacts.Add(new Artifact("Changed", ChangelogStyle.Header2));
                        i++; // skip next line
                        continue;
                    }
                    if (lineHash == s_FixedLineHash)
                    {
                        section = ChangelogSection.Fixed;
                        Artifacts.Add(new Artifact("Fixed", ChangelogStyle.Header2));
                        i++; // skip next line
                        continue;
                    }
                    if (lineHash == s_RemovedLineHash)
                    {
                        section = ChangelogSection.Removed;
                        Artifacts.Add(new Artifact("Removed", ChangelogStyle.Header2));
                        i++; // skip next line
                        continue;
                    }

                    // Special Cases
                    if (s.StartsWith("## "))
                    {
                        Artifacts.Add(new Artifact(s.Replace("## ", string.Empty),
                            ChangelogStyle.Header2));
                        i++; // skip next line
                        continue;
                    }
                    if (s.StartsWith("***"))
                    {
                        Artifacts.Add(new Artifact(
                            s.Replace("***\"", string.Empty)
                                .Replace("\"***", string.Empty).Trim(), ChangelogStyle.Bold));
                        continue;
                    }
                    if (s.StartsWith("> "))
                    {
                        Artifacts.Add(new Artifact(s.Replace("> ", string.Empty),
                            ChangelogStyle.Italic));
                        continue;
                    }
                    if (s.StartsWith("- "))
                    {
                        Artifacts.Add(GenerateChangelogLineItem(s.Replace("- ", string.Empty), section));
                        continue;
                    }

                    Artifacts.Add(new Artifact(s));
                }
            }
        }
    }
}